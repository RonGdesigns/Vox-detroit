using System;
using System.Collections.Generic;

namespace VoxDetroit.Careers
{
    public enum SubstanceCategory
    {
        Cannabis,
        Stimulant,
        Opioid,
        Psychedelic,
        Sedative,
        Synthetic
    }

    public enum SubstanceMarketStatus
    {
        RegulatedLegal,
        Illegal
    }

    [Serializable]
    public sealed class SubstanceRiskProfile
    {
        public SubstanceCategory category;
        public SubstanceMarketStatus marketStatus;
        public int enforcementRisk;
        public int violenceRisk;
        public int customerHealthRisk;
        public int dependencyRisk;
        public int supplyVolatility;
        public int socialReputationRisk;
    }

    [Serializable]
    public sealed class SubstanceMarketProgress
    {
        public SubstanceCategory category;
        public int experience;
        public int trust;
        public int heat;
        public bool unlocked;
    }

    [Serializable]
    public sealed class SubstanceMarketState
    {
        public List<SubstanceMarketProgress> categories =
            new List<SubstanceMarketProgress>();
    }
}
