using System;
using VoxDetroit.Economy;
using VoxDetroit.Inventory;

namespace VoxDetroit.Commerce
{
    public sealed class StoreService
    {
        private readonly StoreWorldState _state;

        public StoreService(StoreWorldState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public StoreRecord Find(string storeId)
        {
            foreach (StoreRecord store in _state.stores)
            {
                if (store != null &&
                    string.Equals(
                        store.id,
                        storeId,
                        StringComparison.Ordinal))
                {
                    return store;
                }
            }

            return null;
        }

        public bool TryBuy(
            string storeId,
            string itemId,
            int quantity,
            string payerAccountId,
            FinanceService finance,
            InventoryService inventory,
            long gameMinute)
        {
            if (quantity <= 0 ||
                finance == null ||
                inventory == null)
            {
                return false;
            }

            StoreRecord store = Find(storeId);
            StoreCatalogEntry entry =
                FindEntry(store, itemId);

            if (entry == null ||
                entry.unitPriceCents < 0 ||
                (entry.stock >= 0 &&
                 entry.stock < quantity))
            {
                return false;
            }

            long totalCents = checked(
                entry.unitPriceCents * quantity);

            if (totalCents > 0)
            {
                var total = new Money(totalCents);

                bool paid;

                if (string.IsNullOrWhiteSpace(
                        store.financeAccountId))
                {
                    paid = finance.TryDebit(
                        payerAccountId,
                        total,
                        gameMinute,
                        "store-purchase",
                        $"{store.displayName}: {itemId}");
                }
                else
                {
                    paid = finance.TryTransfer(
                        payerAccountId,
                        store.financeAccountId,
                        total,
                        gameMinute,
                        "store-purchase",
                        $"{store.displayName}: {itemId}");
                }

                if (!paid)
                {
                    return false;
                }
            }

            inventory.Add(itemId, quantity);

            if (entry.stock >= 0)
            {
                entry.stock -= quantity;
            }

            return true;
        }

        private static StoreCatalogEntry FindEntry(
            StoreRecord store,
            string itemId)
        {
            if (store == null ||
                string.IsNullOrWhiteSpace(itemId))
            {
                return null;
            }

            foreach (StoreCatalogEntry entry in store.catalog)
            {
                if (entry != null &&
                    string.Equals(
                        entry.itemId,
                        itemId,
                        StringComparison.Ordinal))
                {
                    return entry;
                }
            }

            return null;
        }
    }
}
