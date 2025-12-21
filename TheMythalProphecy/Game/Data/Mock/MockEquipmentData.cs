using System.Collections.Generic;
using TheMythalProphecy.Game.Characters.Stats;
using TheMythalProphecy.Game.Data.Definitions;
using TheMythalProphecy.Game.Data.Definitions.Databases;

namespace TheMythalProphecy.Game.Data.Mock;

/// <summary>
/// Defines mock equipment for testing
/// </summary>
public static class MockEquipmentData
{
    public static void PopulateDatabase(EquipmentDatabase database)
    {
        // Weapons
        database.Register(new EquipmentDefinition
        {
            Id = "iron_sword",
            Name = "Iron Sword",
            Description = "A basic iron sword.",
            Slot = EquipmentSlot.Weapon,
            StatBonuses = new Dictionary<StatType, int>
            {
                { StatType.Strength, 5 },
                { StatType.Defense, 1 }
            },
            BuyPrice = 100,
            SellPrice = 50
        });

        database.Register(new EquipmentDefinition
        {
            Id = "steel_sword",
            Name = "Steel Sword",
            Description = "A well-crafted steel sword.",
            Slot = EquipmentSlot.Weapon,
            StatBonuses = new Dictionary<StatType, int>
            {
                { StatType.Strength, 12 },
                { StatType.Speed, 2 }
            },
            BuyPrice = 300,
            SellPrice = 150
        });

        database.Register(new EquipmentDefinition
        {
            Id = "mystic_staff",
            Name = "Mystic Staff",
            Description = "A staff imbued with magical energy.",
            Slot = EquipmentSlot.Weapon,
            StatBonuses = new Dictionary<StatType, int>
            {
                { StatType.MagicPower, 15 },
                { StatType.MaxMP, 10 }
            },
            BuyPrice = 350,
            SellPrice = 175
        });

        database.Register(new EquipmentDefinition
        {
            Id = "hunters_bow",
            Name = "Hunter's Bow",
            Description = "A precision bow for skilled archers.",
            Slot = EquipmentSlot.Weapon,
            StatBonuses = new Dictionary<StatType, int>
            {
                { StatType.Strength, 8 },
                { StatType.Speed, 5 },
                { StatType.Luck, 3 }
            },
            BuyPrice = 250,
            SellPrice = 125
        });

        // Armor
        database.Register(new EquipmentDefinition
        {
            Id = "leather_armor",
            Name = "Leather Armor",
            Description = "Basic leather protection.",
            Slot = EquipmentSlot.Armor,
            StatBonuses = new Dictionary<StatType, int>
            {
                { StatType.Defense, 5 },
                { StatType.MaxHP, 10 }
            },
            BuyPrice = 80,
            SellPrice = 40
        });

        database.Register(new EquipmentDefinition
        {
            Id = "iron_armor",
            Name = "Iron Armor",
            Description = "Heavy iron plate armor.",
            Slot = EquipmentSlot.Armor,
            StatBonuses = new Dictionary<StatType, int>
            {
                { StatType.Defense, 12 },
                { StatType.MagicDefense, 5 },
                { StatType.MaxHP, 25 }
            },
            BuyPrice = 250,
            SellPrice = 125
        });

        database.Register(new EquipmentDefinition
        {
            Id = "mystic_robe",
            Name = "Mystic Robe",
            Description = "A robe woven with protective spells.",
            Slot = EquipmentSlot.Armor,
            StatBonuses = new Dictionary<StatType, int>
            {
                { StatType.MagicDefense, 15 },
                { StatType.MaxMP, 15 },
                { StatType.Defense, 3 }
            },
            BuyPrice = 300,
            SellPrice = 150
        });

        // Accessories
        database.Register(new EquipmentDefinition
        {
            Id = "power_ring",
            Name = "Power Ring",
            Description = "A ring that enhances physical strength.",
            Slot = EquipmentSlot.Accessory,
            StatBonuses = new Dictionary<StatType, int>
            {
                { StatType.Strength, 8 },
                { StatType.MaxHP, 15 }
            },
            BuyPrice = 200,
            SellPrice = 100
        });

        database.Register(new EquipmentDefinition
        {
            Id = "magic_ring",
            Name = "Magic Ring",
            Description = "A ring that amplifies magical power.",
            Slot = EquipmentSlot.Accessory,
            StatBonuses = new Dictionary<StatType, int>
            {
                { StatType.MagicPower, 10 },
                { StatType.MaxMP, 10 }
            },
            BuyPrice = 200,
            SellPrice = 100
        });

        database.Register(new EquipmentDefinition
        {
            Id = "speed_boots",
            Name = "Speed Boots",
            Description = "Enchanted boots that increase agility.",
            Slot = EquipmentSlot.Accessory,
            StatBonuses = new Dictionary<StatType, int>
            {
                { StatType.Speed, 12 },
                { StatType.Luck, 5 }
            },
            BuyPrice = 220,
            SellPrice = 110
        });

        database.Register(new EquipmentDefinition
        {
            Id = "guardian_amulet",
            Name = "Guardian Amulet",
            Description = "An amulet blessed with protective magic.",
            Slot = EquipmentSlot.Accessory,
            StatBonuses = new Dictionary<StatType, int>
            {
                { StatType.Defense, 8 },
                { StatType.MagicDefense, 8 },
                { StatType.MaxHP, 20 }
            },
            BuyPrice = 400,
            SellPrice = 200
        });
    }
}
