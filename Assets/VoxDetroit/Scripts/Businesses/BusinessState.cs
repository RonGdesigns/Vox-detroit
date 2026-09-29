using System;
using System.Collections.Generic;

namespace VoxDetroit.Businesses
{
    public enum BusinessType
    {
        Restaurant,
        AutoShop,
        Contractor,
        Retail,
        MediaStudio,
        PropertyManagement,
        DeliveryCompany
    }

    [Serializable]
    public sealed class BusinessRecord
    {
        public string id;
        public string displayName;
        public BusinessType type;
        public string ownerId;
        public string propertyId;
        public string cashAccountId;
        public bool open;
        public int reputation;
        public int employeeCount;
        public long baseDailyRevenueCents;
        public long baseDailyExpenseCents;
        public long lifetimeRevenueCents;
        public long lifetimeExpenseCents;
    }

    [Serializable]
    public sealed class BusinessWorldState
    {
        public List<BusinessRecord> businesses =
            new List<BusinessRecord>();
    }
}
