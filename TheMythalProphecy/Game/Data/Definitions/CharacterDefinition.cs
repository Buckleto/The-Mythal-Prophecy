using System.Collections.Generic;
using TheMythalProphecy.Game.Characters.Stats;

namespace TheMythalProphecy.Game.Data.Definitions;

/// <summary>
/// Defines a character template loaded from JSON
/// </summary>
public class CharacterDefinition
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // Base stats at level 1
    public BaseStats BaseStats { get; set; }

    // Stat growth per level
    public GrowthRates GrowthRates { get; set; }

    // Starting equipment IDs
    public StartingEquipment StartingEquipment { get; set; }

    // Starting skills
    public List<string> StartingSkills { get; set; } = new();

    // Skills learned at specific levels
    public List<SkillLearn> LearnedSkills { get; set; } = new();

    // Asset paths
    public string SpriteSheet { get; set; }
    public string Portrait { get; set; }
}

public class BaseStats
{
    public int Level { get; set; } = 1;
    public int MaxHP { get; set; }
    public int MaxMP { get; set; }
    public int Strength { get; set; }
    public int Defense { get; set; }
    public int MagicPower { get; set; }
    public int MagicDefense { get; set; }
    public int Speed { get; set; }
    public int Luck { get; set; }
}

public class GrowthRates
{
    public float HP { get; set; }
    public float MP { get; set; }
    public float Strength { get; set; }
    public float Defense { get; set; }
    public float MagicPower { get; set; }
    public float MagicDefense { get; set; }
    public float Speed { get; set; }
    public float Luck { get; set; }
}

public class StartingEquipment
{
    public string Weapon { get; set; }
    public string Armor { get; set; }
    public string Accessory { get; set; }
}

public class SkillLearn
{
    public int Level { get; set; }
    public string Skill { get; set; }
}

/// <summary>
/// Container for multiple character definitions from JSON
/// </summary>
public class CharacterCollection
{
    public List<CharacterDefinition> Characters { get; set; } = new();
}
