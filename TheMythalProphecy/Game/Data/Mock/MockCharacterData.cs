using System.Collections.Generic;
using TheMythalProphecy.Game.Data.Definitions;
using TheMythalProphecy.Game.Data.Definitions.Databases;

namespace TheMythalProphecy.Game.Data.Mock;

/// <summary>
/// Defines mock character templates for testing
/// </summary>
public static class MockCharacterData
{
    public static void PopulateDatabase(CharacterDatabase database)
    {
        database.Register(CreateAria());
        database.Register(CreateKael());
        database.Register(CreateLyra());
        database.Register(CreateZephyr());
        database.Register(CreateFinn());
    }

    // Warrior archetype
    private static CharacterDefinition CreateAria()
    {
        return new CharacterDefinition
        {
            Id = "aria",
            Name = "Aria",
            Description = "A skilled warrior with high strength and defense.",
            BaseStats = new BaseStats
            {
                MaxHP = 120,
                MaxMP = 20,
                Strength = 14,
                Defense = 12,
                MagicPower = 6,
                MagicDefense = 8,
                Speed = 10,
                Luck = 8
            },
            GrowthRates = new GrowthRates
            {
                HP = 12f,
                MP = 2f,
                Strength = 2.5f,
                Defense = 2.0f,
                MagicPower = 0.5f,
                MagicDefense = 1.0f,
                Speed = 1.0f,
                Luck = 0.8f
            },
            StartingEquipment = new StartingEquipment
            {
                Weapon = "iron_sword",
                Armor = "iron_armor",
                Accessory = null
            },
            StartingSkills = new List<string> { "attack", "power_strike", "defend" }
        };
    }

    // Mage archetype
    private static CharacterDefinition CreateKael()
    {
        return new CharacterDefinition
        {
            Id = "kael",
            Name = "Kael",
            Description = "A powerful mage with strong magical abilities.",
            BaseStats = new BaseStats
            {
                MaxHP = 80,
                MaxMP = 50,
                Strength = 6,
                Defense = 6,
                MagicPower = 16,
                MagicDefense = 14,
                Speed = 9,
                Luck = 10
            },
            GrowthRates = new GrowthRates
            {
                HP = 8f,
                MP = 5f,
                Strength = 0.5f,
                Defense = 0.8f,
                MagicPower = 3.0f,
                MagicDefense = 2.5f,
                Speed = 1.2f,
                Luck = 1.0f
            },
            StartingEquipment = new StartingEquipment
            {
                Weapon = "mystic_staff",
                Armor = "mystic_robe",
                Accessory = "magic_ring"
            },
            StartingSkills = new List<string> { "attack", "fireball", "ice_shard", "heal" }
        };
    }

    // Rogue/Ranger archetype
    private static CharacterDefinition CreateLyra()
    {
        return new CharacterDefinition
        {
            Id = "lyra",
            Name = "Lyra",
            Description = "A nimble ranger with high speed and critical hit rate.",
            BaseStats = new BaseStats
            {
                MaxHP = 95,
                MaxMP = 30,
                Strength = 11,
                Defense = 8,
                MagicPower = 9,
                MagicDefense = 9,
                Speed = 16,
                Luck = 14
            },
            GrowthRates = new GrowthRates
            {
                HP = 10f,
                MP = 3f,
                Strength = 1.8f,
                Defense = 1.2f,
                MagicPower = 1.2f,
                MagicDefense = 1.2f,
                Speed = 2.5f,
                Luck = 2.0f
            },
            StartingEquipment = new StartingEquipment
            {
                Weapon = "hunters_bow",
                Armor = "leather_armor",
                Accessory = "speed_boots"
            },
            StartingSkills = new List<string> { "attack", "quick_shot", "poison_arrow" }
        };
    }

    // Paladin/Cleric archetype
    private static CharacterDefinition CreateZephyr()
    {
        return new CharacterDefinition
        {
            Id = "zephyr",
            Name = "Zephyr",
            Description = "A holy knight with balanced stats and healing abilities.",
            BaseStats = new BaseStats
            {
                MaxHP = 110,
                MaxMP = 40,
                Strength = 12,
                Defense = 14,
                MagicPower = 12,
                MagicDefense = 13,
                Speed = 8,
                Luck = 9
            },
            GrowthRates = new GrowthRates
            {
                HP = 11f,
                MP = 4f,
                Strength = 2.0f,
                Defense = 2.2f,
                MagicPower = 2.0f,
                MagicDefense = 2.0f,
                Speed = 0.8f,
                Luck = 1.0f
            },
            StartingEquipment = new StartingEquipment
            {
                Weapon = "steel_sword",
                Armor = "iron_armor",
                Accessory = "guardian_amulet"
            },
            StartingSkills = new List<string> { "attack", "holy_strike", "heal", "protect" }
        };
    }

    // Lower level reserve character
    private static CharacterDefinition CreateFinn()
    {
        return new CharacterDefinition
        {
            Id = "finn",
            Name = "Finn",
            Description = "A young adventurer with potential to grow.",
            BaseStats = new BaseStats
            {
                MaxHP = 90,
                MaxMP = 25,
                Strength = 10,
                Defense = 9,
                MagicPower = 8,
                MagicDefense = 8,
                Speed = 12,
                Luck = 11
            },
            GrowthRates = new GrowthRates
            {
                HP = 10f,
                MP = 3f,
                Strength = 2.0f,
                Defense = 1.8f,
                MagicPower = 1.5f,
                MagicDefense = 1.5f,
                Speed = 2.0f,
                Luck = 1.5f
            },
            StartingEquipment = new StartingEquipment
            {
                Weapon = "iron_sword",
                Armor = "leather_armor",
                Accessory = null
            },
            StartingSkills = new List<string> { "attack", "defend" }
        };
    }
}
