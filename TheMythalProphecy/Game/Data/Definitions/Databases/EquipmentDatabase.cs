using System;
using System.Collections.Generic;
using System.Linq;

namespace TheMythalProphecy.Game.Data.Definitions.Databases;

/// <summary>
/// Centralized database for all equipment definitions
/// Accessible via GameServices.GameData.EquipmentDatabase
/// </summary>
public class EquipmentDatabase
{
    private readonly Dictionary<string, EquipmentDefinition> _equipment = new();

    public IReadOnlyDictionary<string, EquipmentDefinition> Equipment => _equipment;

    /// <summary>
    /// Register an equipment definition
    /// </summary>
    public void Register(EquipmentDefinition equipment)
    {
        if (string.IsNullOrEmpty(equipment.Id))
            throw new ArgumentException("Equipment ID cannot be null or empty");

        _equipment[equipment.Id] = equipment;
    }

    /// <summary>
    /// Get equipment definition by ID
    /// </summary>
    public EquipmentDefinition Get(string equipmentId)
    {
        return _equipment.TryGetValue(equipmentId, out var eq) ? eq : null;
    }

    /// <summary>
    /// Check if equipment exists
    /// </summary>
    public bool Exists(string equipmentId)
    {
        return _equipment.ContainsKey(equipmentId);
    }

    /// <summary>
    /// Get all equipment for a specific slot
    /// </summary>
    public IEnumerable<EquipmentDefinition> GetBySlot(EquipmentSlot slot)
    {
        return _equipment.Values.Where(e => e.Slot == slot);
    }

    /// <summary>
    /// Clear all definitions (for reinitialization)
    /// </summary>
    public void Clear()
    {
        _equipment.Clear();
    }
}
