using System;
using System.Collections.Generic;

namespace VoxDetroit.Jobs
{
    public enum JobLoopType
    {
        Delivery,
        Construction,
        Mechanic,
        Restaurant,
        Photography,
        Security,
        Rideshare,
        PropertyMaintenance
    }

    [Serializable]
    public sealed class JobDefinition
    {
        public string id;
        public string displayName;
        public JobLoopType loopType;
        public long hourlyPayCents;
        public int reputationRequired;
        public int defaultShiftMinutes = 480;
        public string employerId;
    }

    public static class PrototypeJobCatalog
    {
        public static List<JobDefinition> Create()
        {
            return new List<JobDefinition>
            {
                Create(
                    "job.delivery.entry",
                    "Delivery Driver",
                    JobLoopType.Delivery,
                    1850,
                    0,
                    "employer.prototype.delivery"),
                Create(
                    "job.construction.entry",
                    "Construction Laborer",
                    JobLoopType.Construction,
                    2400,
                    0,
                    "employer.prototype.contractor"),
                Create(
                    "job.mechanic.entry",
                    "Auto Shop Assistant",
                    JobLoopType.Mechanic,
                    2200,
                    5,
                    "employer.prototype.autoshop"),
                Create(
                    "job.restaurant.entry",
                    "Restaurant Crew",
                    JobLoopType.Restaurant,
                    1700,
                    0,
                    "employer.prototype.restaurant"),
                Create(
                    "job.photography.entry",
                    "Freelance Photographer",
                    JobLoopType.Photography,
                    2600,
                    10,
                    "employer.prototype.media"),
                Create(
                    "job.security.entry",
                    "Security Officer",
                    JobLoopType.Security,
                    2100,
                    5,
                    "employer.prototype.security"),
                Create(
                    "job.rideshare.entry",
                    "Rideshare Driver",
                    JobLoopType.Rideshare,
                    2000,
                    0,
                    "employer.prototype.rideshare")
            };
        }

        private static JobDefinition Create(
            string id,
            string name,
            JobLoopType type,
            long hourlyPayCents,
            int reputation,
            string employer)
        {
            return new JobDefinition
            {
                id = id,
                displayName = name,
                loopType = type,
                hourlyPayCents = hourlyPayCents,
                reputationRequired = reputation,
                employerId = employer
            };
        }
    }
}
