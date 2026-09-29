using System;
using System.Collections.Generic;

namespace VoxDetroit.Jobs
{
    public enum JobLoopType
    {
        Delivery,
        MailCarrier,
        ParcelCourier,
        TaxiDriver,
        Construction,
        Mechanic,
        Restaurant,
        QuickServiceRestaurant,
        Photography,
        Security,
        Rideshare,
        PropertyMaintenance,
        Warehouse,
        Manufacturing,
        SoftwareDeveloper,
        HealthcareSupport,
        TransitOperator,
        OfficeAdministration,
        Retail,
        CannabisRetail
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
                Create("job.delivery.entry","Delivery Driver",JobLoopType.Delivery,1850,0,"employer.prototype.delivery"),
                Create("job.mail.entry","Mail Carrier",JobLoopType.MailCarrier,2200,0,"employer.prototype.mail"),
                Create("job.parcel.entry","Parcel Driver",JobLoopType.ParcelCourier,2250,0,"employer.prototype.parcel"),
                Create("job.taxi.entry","Taxi Driver",JobLoopType.TaxiDriver,2000,0,"employer.prototype.taxi"),
                Create("job.construction.entry","Construction Laborer",JobLoopType.Construction,2400,0,"employer.prototype.contractor"),
                Create("job.mechanic.entry","Auto Shop Assistant",JobLoopType.Mechanic,2200,5,"employer.prototype.autoshop"),
                Create("job.restaurant.entry","Restaurant Crew",JobLoopType.Restaurant,1700,0,"employer.prototype.restaurant"),
                Create("job.quickservice.entry","Quick-Service Crew",JobLoopType.QuickServiceRestaurant,1650,0,"employer.prototype.quickservice"),
                Create("job.warehouse.entry","Warehouse Associate",JobLoopType.Warehouse,1950,0,"employer.prototype.warehouse"),
                Create("job.manufacturing.entry","Production Worker",JobLoopType.Manufacturing,2350,0,"employer.prototype.manufacturing"),
                Create("job.software.entry","Junior Software Developer",JobLoopType.SoftwareDeveloper,3200,10,"employer.prototype.software"),
                Create("job.healthsupport.entry","Healthcare Support",JobLoopType.HealthcareSupport,2050,5,"employer.prototype.health"),
                Create("job.transit.entry","Transit Operator",JobLoopType.TransitOperator,2400,5,"employer.prototype.transit"),
                Create("job.office.entry","Office Assistant",JobLoopType.OfficeAdministration,2100,0,"employer.prototype.office"),
                Create("job.retail.entry","Retail Associate",JobLoopType.Retail,1750,0,"employer.prototype.retail"),
                Create("job.cannabisretail.entry","Licensed Cannabis Retail Associate",JobLoopType.CannabisRetail,1900,5,"employer.prototype.cannabis"),
                Create("job.photography.entry","Freelance Photographer",JobLoopType.Photography,2600,10,"employer.prototype.media"),
                Create("job.security.entry","Security Officer",JobLoopType.Security,2100,5,"employer.prototype.security"),
                Create("job.rideshare.entry","Rideshare Driver",JobLoopType.Rideshare,2000,0,"employer.prototype.rideshare")
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
