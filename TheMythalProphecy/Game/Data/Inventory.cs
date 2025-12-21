using System;
using System.Collections.Generic;
using System.Linq;
using TheMythalProphecy.Game.Systems.Events;

namespace TheMythalProphecy.Game.Data;

/// <summary>
/// Manages player inventory with item stacking
/// </summary>
public class Inventory
{
    private readonly Dictionary<string, ItemStack> _items = new();

    /// <summary>
    /// Event raised when an item is added to inventory
    /// </summary>
    public event Action<string, int> OnItemAdded;

    /// <summary>
    /// Event raised when an item is removed from inventory
    /// </summary>
    public event Action<string, int> OnItemRemoved;

    /// <summary>
    /// Gets all items in the inventory
    /// </summary>
    public IReadOnlyDictionary<string, ItemStack> Items => _items;

    /// <summary>
    /// Adds an item to the inventory
    /// </summary>
    /// <param name="itemId">The item ID</param>
    /// <param name="quantity">Quantity to add (default 1)</param>
    /// <returns>True if successful</returns>
    public bool AddItem(string itemId, int quantity = 1)
    {
        if (string.IsNullOrEmpty(itemId) || quantity <= 0)
            return false;

        if (_items.TryGetValue(itemId, out var stack))
        {
            stack.Quantity += quantity;
        }
        else
        {
            _items[itemId] = new ItemStack { ItemId = itemId, Quantity = quantity };
        }

        OnItemAdded?.Invoke(itemId, quantity);
        Core.GameServices.Events?.Publish(new ItemAddedEvent(itemId, quantity));
        return true;
    }

    /// <summary>
    /// Removes an item from the inventory
    /// </summary>
    /// <param name="itemId">The item ID</param>
    /// <param name="quantity">Quantity to remove (default 1)</param>
    /// <returns>True if successful</returns>
    public bool RemoveItem(string itemId, int quantity = 1)
    {
        if (string.IsNullOrEmpty(itemId) || quantity <= 0)
            return false;

        if (!_items.TryGetValue(itemId, out var stack))
            return false;

        if (stack.Quantity < quantity)
            return false;

        stack.Quantity -= quantity;

        // Remove the stack if quantity reaches 0
        if (stack.Quantity <= 0)
        {
            _items.Remove(itemId);
        }

        OnItemRemoved?.Invoke(itemId, quantity);
        Core.GameServices.Events?.Publish(new ItemRemovedEvent(itemId, quantity));
        return true;
    }

    /// <summary>
    /// Gets the quantity of an item in inventory
    /// </summary>
    public int GetItemCount(string itemId)
    {
        return _items.TryGetValue(itemId, out var stack) ? stack.Quantity : 0;
    }

    /// <summary>
    /// Checks if the inventory contains at least the specified quantity of an item
    /// </summary>
    public bool HasItem(string itemId, int quantity = 1)
    {
        return GetItemCount(itemId) >= quantity;
    }

    /// <summary>
    /// Gets all item IDs in the inventory
    /// </summary>
    public IEnumerable<string> GetItemIds()
    {
        return _items.Keys;
    }

    /// <summary>
    /// Clears the entire inventory
    /// </summary>
    public void Clear()
    {
        _items.Clear();
    }
}

/// <summary>
/// Represents a stack of items in the inventory
/// </summary>
public class ItemStack
{
    public string ItemId { get; set; }
    public int Quantity { get; set; }
}
