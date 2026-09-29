using System;

namespace VoxDetroit.Story
{
    public sealed class StoryService
    {
        private readonly StoryState _state;

        public StoryService(StoryState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public bool GetFlag(string key)
        {
            StoryFlag flag = FindFlag(key);
            return flag != null && flag.value;
        }

        public void SetFlag(string key, bool value)
        {
            StoryFlag flag = FindFlag(key);

            if (flag == null)
            {
                flag = new StoryFlag { key = key };
                _state.flags.Add(flag);
            }

            flag.value = value;
        }

        public int GetVariable(string key)
        {
            StoryVariable variable = FindVariable(key);
            return variable?.value ?? 0;
        }

        public void SetVariable(string key, int value)
        {
            StoryVariable variable = FindVariable(key);

            if (variable == null)
            {
                variable = new StoryVariable { key = key };
                _state.variables.Add(variable);
            }

            variable.value = value;
        }

        public bool CanTrigger(StoryEventDefinition definition)
        {
            if (definition == null ||
                string.IsNullOrWhiteSpace(definition.id))
            {
                return false;
            }

            if (definition.triggerOnce &&
                _state.triggeredEventIds.Contains(definition.id))
            {
                return false;
            }

            foreach (StoryCondition condition in definition.conditions)
            {
                if (condition == null || !Evaluate(condition))
                {
                    return false;
                }
            }

            return true;
        }

        public bool TryTrigger(StoryEventDefinition definition)
        {
            if (!CanTrigger(definition))
            {
                return false;
            }

            foreach (StoryAction action in definition.actions)
            {
                if (action != null)
                {
                    Apply(action);
                }
            }

            if (definition.triggerOnce)
            {
                _state.triggeredEventIds.Add(definition.id);
            }

            return true;
        }

        private bool Evaluate(StoryCondition condition)
        {
            switch (condition.type)
            {
                case StoryConditionType.FlagEquals:
                    return GetFlag(condition.key) ==
                           condition.boolValue;

                case StoryConditionType.VariableAtLeast:
                    return GetVariable(condition.key) >=
                           condition.intValue;

                case StoryConditionType.VariableAtMost:
                    return GetVariable(condition.key) <=
                           condition.intValue;

                default:
                    return false;
            }
        }

        private void Apply(StoryAction action)
        {
            switch (action.type)
            {
                case StoryActionType.SetFlag:
                    SetFlag(action.key, action.boolValue);
                    break;

                case StoryActionType.SetVariable:
                    SetVariable(action.key, action.intValue);
                    break;

                case StoryActionType.AddVariable:
                    SetVariable(
                        action.key,
                        checked(
                            GetVariable(action.key) +
                            action.intValue));
                    break;
            }
        }

        private StoryFlag FindFlag(string key)
        {
            ValidateKey(key);

            foreach (StoryFlag flag in _state.flags)
            {
                if (flag != null &&
                    string.Equals(
                        flag.key,
                        key,
                        StringComparison.Ordinal))
                {
                    return flag;
                }
            }

            return null;
        }

        private StoryVariable FindVariable(string key)
        {
            ValidateKey(key);

            foreach (StoryVariable variable in _state.variables)
            {
                if (variable != null &&
                    string.Equals(
                        variable.key,
                        key,
                        StringComparison.Ordinal))
                {
                    return variable;
                }
            }

            return null;
        }

        private static void ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Story key is required.");
            }
        }
    }
}
