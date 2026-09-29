using System;

namespace VoxDetroit.Reputation
{
    public sealed class ReputationService
    {
        private readonly ReputationState _state;

        public ReputationService(ReputationState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public int Get(string contextId)
        {
            ReputationRecord record = Find(contextId);
            return record?.value ?? 0;
        }

        public void Set(string contextId, int value)
        {
            ReputationRecord record = Find(contextId);

            if (record == null)
            {
                record = new ReputationRecord
                {
                    contextId = contextId
                };

                _state.records.Add(record);
            }

            record.value = value;
        }

        public int Add(string contextId, int amount)
        {
            int next = checked(Get(contextId) + amount);
            Set(contextId, next);
            return next;
        }

        private ReputationRecord Find(string contextId)
        {
            if (string.IsNullOrWhiteSpace(contextId))
            {
                throw new ArgumentException(
                    "Reputation context is required.",
                    nameof(contextId));
            }

            foreach (ReputationRecord record in _state.records)
            {
                if (record != null &&
                    string.Equals(
                        record.contextId,
                        contextId,
                        StringComparison.Ordinal))
                {
                    return record;
                }
            }

            return null;
        }
    }
}
