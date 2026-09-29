using System;
using System.Collections.Generic;

namespace VoxDetroit.Story
{
    [Serializable]
    public sealed class StoryFlag
    {
        public string key;
        public bool value;
    }

    [Serializable]
    public sealed class StoryVariable
    {
        public string key;
        public int value;
    }

    public enum StoryConditionType
    {
        FlagEquals,
        VariableAtLeast,
        VariableAtMost
    }

    [Serializable]
    public sealed class StoryCondition
    {
        public StoryConditionType type;
        public string key;
        public bool boolValue;
        public int intValue;
    }

    public enum StoryActionType
    {
        SetFlag,
        SetVariable,
        AddVariable
    }

    [Serializable]
    public sealed class StoryAction
    {
        public StoryActionType type;
        public string key;
        public bool boolValue;
        public int intValue;
    }

    [Serializable]
    public sealed class StoryEventDefinition
    {
        public string id;
        public bool triggerOnce = true;
        public List<StoryCondition> conditions =
            new List<StoryCondition>();
        public List<StoryAction> actions =
            new List<StoryAction>();
    }

    [Serializable]
    public sealed class StoryState
    {
        public List<StoryFlag> flags = new List<StoryFlag>();
        public List<StoryVariable> variables =
            new List<StoryVariable>();
        public List<string> triggeredEventIds =
            new List<string>();
    }
}
