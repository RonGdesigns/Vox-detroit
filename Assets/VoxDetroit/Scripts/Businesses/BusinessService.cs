using System;
using VoxDetroit.Economy;

namespace VoxDetroit.Businesses
{
    public sealed class BusinessService
    {
        private readonly BusinessWorldState _state;

        public BusinessService(BusinessWorldState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public BusinessRecord Find(string businessId)
        {
            foreach (BusinessRecord business in _state.businesses)
            {
                if (business != null &&
                    string.Equals(
                        business.id,
                        businessId,
                        StringComparison.Ordinal))
                {
                    return business;
                }
            }

            return null;
        }

        public Money SimulateDay(
            string businessId,
            FinanceService finance,
            long gameMinute,
            int demandPercent = 100)
        {
            BusinessRecord business = Find(businessId);

            if (business == null ||
                !business.open ||
                string.IsNullOrWhiteSpace(business.cashAccountId))
            {
                return Money.Zero;
            }

            int demand = Math.Max(0, demandPercent);
            int reputationMultiplier =
                Math.Max(50, 100 + business.reputation);
            int staffingMultiplier =
                business.employeeCount <= 0
                    ? 25
                    : Math.Min(
                        150,
                        75 + (business.employeeCount * 10));

            long revenue = checked(
                business.baseDailyRevenueCents *
                demand *
                reputationMultiplier *
                staffingMultiplier /
                100L /
                100L /
                100L);

            long expense = Math.Max(
                0,
                business.baseDailyExpenseCents);

            if (revenue > 0)
            {
                finance.Credit(
                    business.cashAccountId,
                    new Money(revenue),
                    gameMinute,
                    "business-revenue",
                    business.displayName);

                business.lifetimeRevenueCents =
                    checked(
                        business.lifetimeRevenueCents + revenue);
            }

            if (expense > 0)
            {
                Money expenseMoney = new Money(expense);

                if (finance.TryDebit(
                        business.cashAccountId,
                        expenseMoney,
                        gameMinute,
                        "business-expense",
                        business.displayName))
                {
                    business.lifetimeExpenseCents =
                        checked(
                            business.lifetimeExpenseCents + expense);
                }
            }

            return new Money(revenue - expense);
        }
    }
}
