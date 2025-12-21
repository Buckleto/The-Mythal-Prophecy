using System.Collections.Generic;
using TheMythalProphecy.Game.Characters.Stats;

namespace TheMythalProphecy.Game.Data.Definitions;

/// <summary>
/// Defines an equipment item template (weapons, armor, accessories)
/// </summary>
public class EquipmentDefinition
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public EquipmentSlot Slot { get; set; }
    public int BuyPrice { get; set; }
    public int SellPrice { get; set; }

    // Stat bonuses provided by this equipment
    public Dictionary<StatType, int> StatBonuses { get; set; } = new();

    // Equipment restrictions
    public List<string> EquippableBy { get; set; } = new(); // Character IDs that can equip
    public int MinimumLevel { get; set; } = 1;

    // Icon and visual properties
    public string IconPath { get; set; }
    public string SpritePath { get; set; } // For visual representation on character
}

/// <summary>
/// Equipment slot types
/// </summary>
public enum EquipmentSlot
{
    Weapon,
    Armor,
    Accessory
}

/// <summary>
/// Container for loading multiple equipment definitions from JSON
/// </summary>
public class EquipmentCollection
{
    public List<EquipmentDefinition> Equipment { get; set; } = new();
}
