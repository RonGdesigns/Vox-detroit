using System;
using System.Collections.Generic;

namespace VoxDetroit.Economy
{
    public static class AccountIds
    {
        public const string PlayerCash = "player.cash";
        public const string PlayerChecking = "player.checking";
        public const string PlayerSavings = "player.savings";
    }

    [Serializable]
    public sealed class AccountState
    {
        public string id;
        public long balanceCents;
    }

    [Serializable]
    public sealed class TransactionRecord
    {
        public string id;
        public long gameMinute;
        public string fromAccountId;
        public string toAccountId;
        public long amountCents;
        public string category;
        public string memo;
    }

    [Serializable]
    public sealed class FinanceState
    {
        public List<AccountState> accounts = new List<AccountState>();
        public List<TransactionRecord> transactions =
            new List<TransactionRecord>();
    }
}
