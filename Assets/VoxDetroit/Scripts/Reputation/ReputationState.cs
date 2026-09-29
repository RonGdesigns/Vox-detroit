using System;
using System.Collections.Generic;

namespace VoxDetroit.Reputation
{
    [Serializable]
    public sealed class ReputationRecord
    {
        public string contextId;
        public int value;
    }

    [Serializable]
    public sealed class ReputationState
    {
        public List<ReputationRecord> records =
            new List<ReputationRecord>();
    }
}
