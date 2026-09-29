using System;
using System.Collections.Generic;

namespace VoxDetroit.Commerce
{
    [Serializable]
    public sealed class StoreCatalogEntry
    {
        public string itemId;
        public long unitPriceCents;
        public int stock = -1;
    }

    [Serializable]
    public sealed class StoreRecord
    {
        public string id;
        public string displayName;
        public string financeAccountId;
        public List<StoreCatalogEntry> catalog =
            new List<StoreCatalogEntry>();
    }

    [Serializable]
    public sealed class StoreWorldState
    {
        public List<StoreRecord> stores =
            new List<StoreRecord>();
    }
}
