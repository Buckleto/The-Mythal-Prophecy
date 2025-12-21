using System;
using System.Collections.Generic;
using System.Linq;

namespace TheMythalProphecy.Game.Data.Definitions.Databases;

/// <summary>
/// Centralized database for all item definitions
/// Accessible via GameServices.GameData.ItemDatabase
/// </summary>
public class ItemDatabase
{
    private readonly Dictionary<string, ItemDefinition> _items = new();

    public IReadOnlyDictionary<string, ItemDefinition> Items => _items;

    /// <summary>
    /// Register an item definition
    /// </summary>
    public void Register(ItemDefinition item)
    {
        if (string.IsNullOrEmpty(item.Id))
            throw new ArgumentException("Item ID cannot be null or empty");

        _items[item.Id] = item;
    }

    /// <summary>
    /// Get item definition by ID
    /// </summary>
    public ItemDefinition Get(string itemId)
    {
        return _items.TryGetValue(itemId, out var item) ? item : null;
    }

    /// <summary>
    /// Check if item exists
    /// </summary>
    public bool Exists(string itemId)
    {
        return _items.ContainsKey(itemId);
    }

    /// <summary>
    /// Get all items of a specific category
    /// </summary>
    public IEnumerable<ItemDefinition> GetByCategory(ItemCategory category)
    {
        return _items.Values.Where(i => i.Category == category);
    }

    /// <summary>
    /// Clear all definitions (for reinitialization)
    /// </summary>
    public void Clear()
    {
        _items.Clear();
    }
}
