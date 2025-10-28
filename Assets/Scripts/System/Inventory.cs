using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Systems
{
    public enum ItemType
    {
        Capture,
        Potion,
        Key
    }

    [Serializable]
    public class ItemStack
    {
        [SerializeField] private string itemId;
        [SerializeField] private ItemType itemType;
        [SerializeField] private int quantity;

        public string ItemId => itemId;
        public ItemType ItemType => itemType;
        public int Quantity => quantity;

        public ItemStack(string itemId, ItemType itemType, int quantity)
        {
            this.itemId = itemId;
            this.itemType = itemType;
            this.quantity = Mathf.Max(0, quantity);
        }

        public void Add(int amount)
        {
            quantity = Mathf.Max(0, quantity + amount);
        }

        public bool Remove(int amount)
        {
            if (quantity < amount)
            {
                return false;
            }

            quantity -= amount;
            return true;
        }
    }

    /// <summary>
    /// Simple inventory tracking stackable items.
    /// </summary>
    [Serializable]
    public class Inventory
    {
        [SerializeField] private List<ItemStack> items = new List<ItemStack>();
        public IReadOnlyList<ItemStack> Items => items;

        [field: NonSerialized]
        public event Action Changed;

        public void AddItem(string id, ItemType type, int amount)
        {
            var stack = items.Find(i => i.ItemId == id);
            if (stack == null)
            {
                stack = new ItemStack(id, type, 0);
                items.Add(stack);
            }

            stack.Add(amount);
            Changed?.Invoke();
        }

        public bool ConsumeItem(string id, int amount)
        {
            var stack = items.Find(i => i.ItemId == id);
            if (stack == null)
            {
                return false;
            }

            bool removed = stack.Remove(amount);
            if (removed && stack.Quantity == 0)
            {
                items.Remove(stack);
            }

            if (removed)
            {
                Changed?.Invoke();
            }

            return removed;
        }
    }
}
