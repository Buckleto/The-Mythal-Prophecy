namespace TheMythalProphecy.Game.Characters.Stats;

/// <summary>
/// Types of character statistics
/// </summary>
public enum StatType
{
    // Primary Resources
    HP,              // Hit Points (current health)
    MaxHP,           // Maximum Hit Points
    MP,              // Magic Points (current mana)
    MaxMP,           // Maximum Magic Points

    // Combat Stats
    Strength,        // Physical attack power
    Defense,         // Physical damage reduction
    MagicPower,      // Magic attack power
    MagicDefense,    // Magic damage reduction
    Speed,           // Turn order / evasion
    Luck,            // Critical hit rate / status resistance

    // Level & Experience
    Level,           // Character level
    Experience,      // Current XP
    ExperienceToNext // XP needed for next level
}
