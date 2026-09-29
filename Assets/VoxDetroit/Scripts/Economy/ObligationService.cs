using System;
using System.Collections.Generic;

namespace VoxDetroit.Economy
{
    public sealed class ObligationService
    {
        private readonly ObligationState _state;

        public ObligationService(ObligationState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public IReadOnlyList<ObligationRecord> ProcessDue(
            long currentGameMinute,
            FinanceService finance)
        {
            if (finance == null)
            {
                throw new ArgumentNullException(nameof(finance));
            }

            var missed = new List<ObligationRecord>();

            foreach (ObligationRecord obligation in _state.obligations)
            {
                if (obligation == null ||
                    !obligation.active ||
                    obligation.intervalMinutes <= 0 ||
                    obligation.nextDueMinute > currentGameMinute)
                {
                    continue;
                }

                while (obligation.active &&
                       obligation.nextDueMinute <= currentGameMinute)
                {
                    ProcessOne(obligation, finance, missed);
                    obligation.nextDueMinute = checked(
                        obligation.nextDueMinute +
                        obligation.intervalMinutes);
                }
            }

            return missed;
        }

        private static void ProcessOne(
            ObligationRecord obligation,
            FinanceService finance,
            List<ObligationRecord> missed)
        {
            if (obligation.amountCents <= 0)
            {
                return;
            }

            var amount = new Money(obligation.amountCents);

            bool paid = finance.TryDebit(
                obligation.payerAccountId,
                amount,
                obligation.nextDueMinute,
                "obligation",
                obligation.displayName);

            if (paid)
            {
                obligation.totalPaidCents = checked(
                    obligation.totalPaidCents +
                    obligation.amountCents);
            }
            else
            {
                obligation.missedPayments = checked(
                    obligation.missedPayments + 1);
                missed.Add(obligation);
            }
        }
    }
}
