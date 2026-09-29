using System;

namespace VoxDetroit.Economy
{
    public sealed class FinanceService
    {
        private const int MaxTransactionHistory = 1000;
        private readonly FinanceState _state;

        public FinanceService(FinanceState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public AccountState GetOrCreateAccount(
            string accountId,
            long startingBalanceCents = 0)
        {
            ValidateAccountId(accountId);

            AccountState existing = FindAccount(accountId);
            if (existing != null)
            {
                return existing;
            }

            var account = new AccountState
            {
                id = accountId,
                balanceCents = startingBalanceCents
            };

            _state.accounts.Add(account);
            return account;
        }

        public Money GetBalance(string accountId)
        {
            AccountState account = FindAccount(accountId);
            return account == null
                ? Money.Zero
                : new Money(account.balanceCents);
        }

        public bool CanAfford(string accountId, Money amount)
        {
            ValidateNonNegative(amount);
            return GetBalance(accountId) >= amount;
        }

        public void Credit(
            string accountId,
            Money amount,
            long gameMinute,
            string category,
            string memo = null)
        {
            ValidatePositive(amount);
            AccountState target = GetOrCreateAccount(accountId);

            target.balanceCents =
                checked(target.balanceCents + amount.Cents);

            Record(
                null,
                accountId,
                amount,
                gameMinute,
                category,
                memo);
        }

        public bool TryDebit(
            string accountId,
            Money amount,
            long gameMinute,
            string category,
            string memo = null)
        {
            ValidatePositive(amount);
            AccountState source = GetOrCreateAccount(accountId);

            if (source.balanceCents < amount.Cents)
            {
                return false;
            }

            source.balanceCents =
                checked(source.balanceCents - amount.Cents);

            Record(
                accountId,
                null,
                amount,
                gameMinute,
                category,
                memo);

            return true;
        }

        public bool TryTransfer(
            string sourceAccountId,
            string targetAccountId,
            Money amount,
            long gameMinute,
            string category,
            string memo = null)
        {
            ValidatePositive(amount);

            if (sourceAccountId == targetAccountId)
            {
                throw new ArgumentException(
                    "Source and target accounts must differ.");
            }

            AccountState source =
                GetOrCreateAccount(sourceAccountId);

            AccountState target =
                GetOrCreateAccount(targetAccountId);

            if (source.balanceCents < amount.Cents)
            {
                return false;
            }

            source.balanceCents =
                checked(source.balanceCents - amount.Cents);

            target.balanceCents =
                checked(target.balanceCents + amount.Cents);

            Record(
                sourceAccountId,
                targetAccountId,
                amount,
                gameMinute,
                category,
                memo);

            return true;
        }

        private AccountState FindAccount(string accountId)
        {
            ValidateAccountId(accountId);

            foreach (AccountState account in _state.accounts)
            {
                if (account != null &&
                    string.Equals(
                        account.id,
                        accountId,
                        StringComparison.Ordinal))
                {
                    return account;
                }
            }

            return null;
        }

        private void Record(
            string from,
            string to,
            Money amount,
            long gameMinute,
            string category,
            string memo)
        {
            _state.transactions.Add(
                new TransactionRecord
                {
                    id = Guid.NewGuid().ToString("N"),
                    gameMinute = gameMinute,
                    fromAccountId = from,
                    toAccountId = to,
                    amountCents = amount.Cents,
                    category = category ?? string.Empty,
                    memo = memo ?? string.Empty
                });

            int excess =
                _state.transactions.Count -
                MaxTransactionHistory;

            if (excess > 0)
            {
                _state.transactions.RemoveRange(0, excess);
            }
        }

        private static void ValidateAccountId(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
            {
                throw new ArgumentException(
                    "Account ID is required.",
                    nameof(accountId));
            }
        }

        private static void ValidatePositive(Money amount)
        {
            if (amount.Cents <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    "Amount must be greater than zero.");
            }
        }

        private static void ValidateNonNegative(Money amount)
        {
            if (amount.Cents < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    "Amount cannot be negative.");
            }
        }
    }
}
