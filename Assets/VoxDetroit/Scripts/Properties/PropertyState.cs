using System;
using System.Collections.Generic;

namespace VoxDetroit.Properties
{
    public enum PropertyUse
    {
        Residential,
        Retail,
        Office,
        Industrial,
        MixedUse,
        VacantLot
    }

    [Serializable]
    public sealed class PropertyRecord
    {
        public string id;
        public string buildingId;
        public string addressLabel;
        public PropertyUse use;
        public string ownerId;
        public long marketValueCents;
        public long purchasePriceCents;
        public long monthlyRentCents;
        public int condition = 50;
        public bool enterable;
        public List<string> installedUpgradeIds =
            new List<string>();
    }

    [Serializable]
    public sealed class PropertyWorldState
    {
        public List<PropertyRecord> properties =
            new List<PropertyRecord>();
    }

    [Serializable]
    public sealed class RenovationDefinition
    {
        public string id;
        public string displayName;
        public long costCents;
        public int conditionGain;
        public int valueGainPercent;
    }
}
