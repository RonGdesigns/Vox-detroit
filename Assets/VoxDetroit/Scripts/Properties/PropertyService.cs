using System;
using VoxDetroit.Economy;

namespace VoxDetroit.Properties
{
    public sealed class PropertyService
    {
        private readonly PropertyWorldState _state;

        public PropertyService(PropertyWorldState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public PropertyRecord Find(string propertyId)
        {
            foreach (PropertyRecord property in _state.properties)
            {
                if (property != null &&
                    string.Equals(
                        property.id,
                        propertyId,
                        StringComparison.Ordinal))
                {
                    return property;
                }
            }

            return null;
        }

        public bool TryPurchase(
            string propertyId,
            string buyerOwnerId,
            string buyerAccountId,
            FinanceService finance,
            long gameMinute)
        {
            PropertyRecord property = Find(propertyId);

            if (property == null ||
                string.IsNullOrWhiteSpace(buyerOwnerId) ||
                !string.IsNullOrWhiteSpace(property.ownerId))
            {
                return false;
            }

            long price =
                property.purchasePriceCents > 0
                    ? property.purchasePriceCents
                    : property.marketValueCents;

            if (price <= 0)
            {
                return false;
            }

            var purchase = new Money(price);

            if (!finance.TryDebit(
                    buyerAccountId,
                    purchase,
                    gameMinute,
                    "property-purchase",
                    property.addressLabel))
            {
                return false;
            }

            property.ownerId = buyerOwnerId;
            return true;
        }

        public bool TryRenovate(
            string propertyId,
            RenovationDefinition renovation,
            string payerAccountId,
            FinanceService finance,
            long gameMinute)
        {
            PropertyRecord property = Find(propertyId);

            if (property == null ||
                renovation == null ||
                string.IsNullOrWhiteSpace(renovation.id) ||
                property.installedUpgradeIds.Contains(renovation.id))
            {
                return false;
            }

            var cost = new Money(
                Math.Max(0, renovation.costCents));

            if (cost.Cents > 0 &&
                !finance.TryDebit(
                    payerAccountId,
                    cost,
                    gameMinute,
                    "property-renovation",
                    renovation.displayName))
            {
                return false;
            }

            property.condition = Math.Min(
                100,
                Math.Max(
                    0,
                    property.condition +
                    Math.Max(0, renovation.conditionGain)));

            if (renovation.valueGainPercent > 0 &&
                property.marketValueCents > 0)
            {
                long increase = checked(
                    property.marketValueCents *
                    renovation.valueGainPercent /
                    100L);

                property.marketValueCents =
                    checked(property.marketValueCents + increase);
            }

            property.installedUpgradeIds.Add(renovation.id);
            return true;
        }
    }
}
