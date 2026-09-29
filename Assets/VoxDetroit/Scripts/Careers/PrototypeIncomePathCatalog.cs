using System.Collections.Generic;

namespace VoxDetroit.Careers
{
    public static class PrototypeIncomePathCatalog
    {
        public static List<IncomePathDefinition> Create()
        {
            return new List<IncomePathDefinition>
            {
                Create(
                    "career.mail",
                    "Mail Carrier",
                    IncomePathKind.WageEmployment,
                    "Route-based public delivery work.",
                    0, 10, 5, 10, 5),
                Create(
                    "career.parcel",
                    "Parcel Delivery",
                    IncomePathKind.WageEmployment,
                    "Van loading, routing and package delivery.",
                    0, 15, 5, 15, 5),
                Create(
                    "career.rideshare",
                    "Taxi / Rideshare",
                    IncomePathKind.GigWork,
                    "Passenger transport with route and rating pressure.",
                    0, 20, 5, 25, 10),
                Create(
                    "career.software",
                    "Software Developer",
                    IncomePathKind.ProfessionalCareer,
                    "Logic, debugging, client and deployment work.",
                    0, 5, 0, 15, 5),
                Create(
                    "career.fastfood",
                    "Quick-Service Restaurant",
                    IncomePathKind.WageEmployment,
                    "High-pressure food-service station work.",
                    0, 10, 5, 10, 5),
                Create(
                    "career.manufacturing",
                    "Manufacturing",
                    IncomePathKind.WageEmployment,
                    "Production, quality and maintenance work.",
                    0, 20, 10, 10, 5),
                Create(
                    "career.construction",
                    "Construction",
                    IncomePathKind.SkilledTrade,
                    "Physical construction and renovation work.",
                    0, 30, 10, 15, 5),
                Create(
                    "career.cannabis.licensed",
                    "Licensed Cannabis Retail",
                    IncomePathKind.RegulatedCannabis,
                    "Regulated retail, inventory and business progression.",
                    10, 10, 5, 15, 10),
                Create(
                    "career.underground",
                    "Underground Market",
                    IncomePathKind.Underground,
                    "High-risk illegal-income story routes.",
                    80, 60, 50, 65, 60)
            };
        }

        private static IncomePathDefinition Create(
            string id,
            string name,
            IncomePathKind kind,
            string description,
            int legal,
            int safety,
            int health,
            int financial,
            int reputation)
        {
            return new IncomePathDefinition
            {
                id = id,
                displayName = name,
                kind = kind,
                description = description,
                risk = new CareerRiskProfile
                {
                    legalExposure = legal,
                    personalSafety = safety,
                    healthHarm = health,
                    financialVolatility = financial,
                    reputation = reputation
                }
            };
        }
    }

    public static class PrototypeSubstanceRiskCatalog
    {
        public static List<SubstanceRiskProfile> Create()
        {
            return new List<SubstanceRiskProfile>
            {
                new SubstanceRiskProfile
                {
                    category = SubstanceCategory.Cannabis,
                    marketStatus = SubstanceMarketStatus.RegulatedLegal,
                    enforcementRisk = 10,
                    violenceRisk = 5,
                    customerHealthRisk = 10,
                    dependencyRisk = 15,
                    supplyVolatility = 10,
                    socialReputationRisk = 10
                },
                new SubstanceRiskProfile
                {
                    category = SubstanceCategory.Cannabis,
                    marketStatus = SubstanceMarketStatus.Illegal,
                    enforcementRisk = 55,
                    violenceRisk = 30,
                    customerHealthRisk = 15,
                    dependencyRisk = 20,
                    supplyVolatility = 35,
                    socialReputationRisk = 35
                },
                new SubstanceRiskProfile
                {
                    category = SubstanceCategory.Stimulant,
                    marketStatus = SubstanceMarketStatus.Illegal,
                    enforcementRisk = 80,
                    violenceRisk = 60,
                    customerHealthRisk = 65,
                    dependencyRisk = 70,
                    supplyVolatility = 60,
                    socialReputationRisk = 65
                },
                new SubstanceRiskProfile
                {
                    category = SubstanceCategory.Opioid,
                    marketStatus = SubstanceMarketStatus.Illegal,
                    enforcementRisk = 90,
                    violenceRisk = 65,
                    customerHealthRisk = 100,
                    dependencyRisk = 100,
                    supplyVolatility = 75,
                    socialReputationRisk = 90
                },
                new SubstanceRiskProfile
                {
                    category = SubstanceCategory.Psychedelic,
                    marketStatus = SubstanceMarketStatus.Illegal,
                    enforcementRisk = 65,
                    violenceRisk = 30,
                    customerHealthRisk = 45,
                    dependencyRisk = 20,
                    supplyVolatility = 55,
                    socialReputationRisk = 45
                },
                new SubstanceRiskProfile
                {
                    category = SubstanceCategory.Sedative,
                    marketStatus = SubstanceMarketStatus.Illegal,
                    enforcementRisk = 80,
                    violenceRisk = 45,
                    customerHealthRisk = 80,
                    dependencyRisk = 85,
                    supplyVolatility = 65,
                    socialReputationRisk = 70
                },
                new SubstanceRiskProfile
                {
                    category = SubstanceCategory.Synthetic,
                    marketStatus = SubstanceMarketStatus.Illegal,
                    enforcementRisk = 85,
                    violenceRisk = 55,
                    customerHealthRisk = 95,
                    dependencyRisk = 75,
                    supplyVolatility = 90,
                    socialReputationRisk = 85
                }
            };
        }
    }
}
