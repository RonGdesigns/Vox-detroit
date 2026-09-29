using System;
using System.Collections.Generic;

namespace VoxDetroit.Careers
{
    public enum IncomePathKind
    {
        WageEmployment,
        GigWork,
        SkilledTrade,
        ProfessionalCareer,
        BusinessOwner,
        RegulatedCannabis,
        Underground
    }

    public enum CareerRiskDimension
    {
        LegalExposure,
        PersonalSafety,
        HealthHarm,
        FinancialVolatility,
        Reputation
    }

    [Serializable]
    public sealed class CareerRiskProfile
    {
        public int legalExposure;
        public int personalSafety;
        public int healthHarm;
        public int financialVolatility;
        public int reputation;

        public int Get(CareerRiskDimension dimension)
        {
            switch (dimension)
            {
                case CareerRiskDimension.LegalExposure:
                    return legalExposure;
                case CareerRiskDimension.PersonalSafety:
                    return personalSafety;
                case CareerRiskDimension.HealthHarm:
                    return healthHarm;
                case CareerRiskDimension.FinancialVolatility:
                    return financialVolatility;
                case CareerRiskDimension.Reputation:
                    return reputation;
                default:
                    return 0;
            }
        }
    }

    [Serializable]
    public sealed class IncomePathDefinition
    {
        public string id;
        public string displayName;
        public IncomePathKind kind;
        public string description;
        public CareerRiskProfile risk =
            new CareerRiskProfile();
        public List<string> requiredTags =
            new List<string>();
        public List<string> unlocks =
            new List<string>();
    }

    [Serializable]
    public sealed class IncomePathProgress
    {
        public string pathId;
        public int experience;
        public int reputation;
        public int trust;
        public int heat;
        public bool unlocked;
    }

    [Serializable]
    public sealed class CareerWorldState
    {
        public List<IncomePathProgress> paths =
            new List<IncomePathProgress>();
    }
}
