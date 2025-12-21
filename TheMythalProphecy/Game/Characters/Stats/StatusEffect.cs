using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TheMythalProphecy.Game.Characters.Stats;

/// <summary>
/// Represents a temporary status effect (buff or debuff) on a character
/// </summary>
public class StatusEffect
{
    public StatusEffectType Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public float Duration { get; set; }
    public float RemainingTime { get; set; }
    public bool IsBuff { get; set; }

    // Stat modifiers
    public Dictionary<StatType, int> StatModifiers { get; set; } = new();

    // Damage over time / Healing over time
    public int TickDamage { get; set; }  // Negative for healing
    public float TickInterval { get; set; } = 1f;
    private float _tickTimer;

    public bool IsExpired => RemainingTime <= 0;

    public StatusEffect(StatusEffectType type, string name, float duration, bool isBuff = true)
    {
        Type = type;
        Name = name;
        Duration = duration;
        RemainingTime = duration;
        IsBuff = isBuff;
        _tickTimer = TickInterval;
    }

    /// <summary>
    /// Update the status effect timer
    /// </summary>
    /// <returns>Damage/healing dealt this frame (0 if no tick)</returns>
    public int Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        RemainingTime -= deltaTime;

        // Handle damage/healing over time
        if (TickDamage != 0)
        {
            _tickTimer -= deltaTime;
            if (_tickTimer <= 0)
            {
                _tickTimer = TickInterval;
                return TickDamage;
            }
        }

        return 0;
    }

    /// <summary>
    /// Add a stat modifier to this status effect
    /// </summary>
    public void AddStatModifier(StatType stat, int amount)
    {
        StatModifiers[stat] = amount;
    }
}

/// <summary>
/// Types of status effects
/// </summary>
public enum StatusEffectType
{
    // Negative Effects
    Poison,
    Burn,
    Frozen,
    Paralyzed,
    Sleep,
    Confusion,
    Blind,
    Silence,
    Slow,
    Weak,

    // Positive Effects
    Regen,
    Haste,
    Protect,
    Shell,
    Berserk,
    Focus,
    Barrier,
    Reflect
}
