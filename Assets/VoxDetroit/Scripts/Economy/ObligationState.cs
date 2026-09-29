using System;
using System.Collections.Generic;

namespace VoxDetroit.Economy
{
    public enum ObligationType
    {
        Rent,
        Utilities,
        Insurance,
        LoanPayment,
        PropertyTax,
        BusinessExpense,
        Subscription,
        Other
    }

    [Serializable]
    public sealed class ObligationRecord
    {
        public string id;
        public string displayName;
        public ObligationType type;
        public string payerAccountId = AccountIds.PlayerChecking;
        public long amountCents;
        public long intervalMinutes;
        public long nextDueMinute;
        public bool active = true;
        public int missedPayments;
        public long totalPaidCents;
        public string relatedEntityId;
    }

    [Serializable]
    public sealed class ObligationState
    {
        public List<ObligationRecord> obligations =
            new List<ObligationRecord>();
    }
}
