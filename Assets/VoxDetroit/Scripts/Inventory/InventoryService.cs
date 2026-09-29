using System;
using System.Collections.Generic;

namespace VoxDetroit.Inventory
{
    public sealed class InventoryService
    {
        private readonly InventoryState _state;

        public InventoryService(InventoryState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public int Count(string itemId)
        {
            ItemStack stack = Find(itemId);
            return stack?.quantity ?? 0;
        }

        public bool Has(string itemId, int quantity)
        {
            Validate(itemId, quantity);
            return Count(itemId) >= quantity;
        }

        public void Add(string itemId, int quantity)
        {
            Validate(itemId, quantity);

            ItemStack stack = Find(itemId);

            if (stack == null)
            {
                _state.stacks.Add(
                    new ItemStack
                    {
                        itemId = itemId,
                        quantity = quantity
                    });

                return;
            }

            stack.quantity = checked(stack.quantity + quantity);
        }

        public bool TryRemove(string itemId, int quantity)
        {
            Validate(itemId, quantity);

            ItemStack stack = Find(itemId);

            if (stack == null || stack.quantity < quantity)
            {
                return false;
            }

            stack.quantity -= quantity;

            if (stack.quantity == 0)
            {
                _state.stacks.Remove(stack);
            }

            return true;
        }

        public bool HasAll(IReadOnlyList<ItemRequirement> requirements)
        {
            if (requirements == null)
            {
                return true;
            }

            foreach (ItemRequirement requirement in requirements)
            {
                if (requirement == null)
                {
                    continue;
                }

                if (!Has(requirement.itemId, requirement.quantity))
                {
                    return false;
                }
            }

            return true;
        }

        public bool TryConsumeAll(
            IReadOnlyList<ItemRequirement> requirements)
        {
            if (!HasAll(requirements))
            {
                return false;
            }

            if (requirements == null)
            {
                return true;
            }

            foreach (ItemRequirement requirement in requirements)
            {
                if (requirement != null)
                {
                    TryRemove(
                        requirement.itemId,
                        requirement.quantity);
                }
            }

            return true;
        }

        private ItemStack Find(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return null;
            }

            foreach (ItemStack stack in _state.stacks)
            {
                if (stack != null &&
                    string.Equals(
                        stack.itemId,
                        itemId,
                        StringComparison.Ordinal))
                {
                    return stack;
                }
            }

            return null;
        }

        private static void Validate(
            string itemId,
            int quantity)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                throw new ArgumentException(
                    "Item ID is required.",
                    nameof(itemId));
            }

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity));
            }
        }
    }
}
