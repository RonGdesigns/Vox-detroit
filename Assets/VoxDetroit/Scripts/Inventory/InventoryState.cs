using System;
using System.Collections.Generic;

namespace VoxDetroit.Inventory
{
    public enum ItemCategory
    {
        Material,
        Tool,
        Food,
        VehiclePart,
        Furniture,
        Clothing,
        Quest,
        Misc
    }

    [Serializable]
    public sealed class ItemDefinition
    {
        public string id;
        public string displayName;
        public ItemCategory category;
        public int maxStack = 99;
        public long baseValueCents;
    }

    [Serializable]
    public sealed class ItemStack
    {
        public string itemId;
        public int quantity;
    }

    [Serializable]
    public sealed class InventoryState
    {
        public List<ItemStack> stacks = new List<ItemStack>();
    }

    [Serializable]
    public sealed class ItemRequirement
    {
        public string itemId;
        public int quantity;
    }
}
