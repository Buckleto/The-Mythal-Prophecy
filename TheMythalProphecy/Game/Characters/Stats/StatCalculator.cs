using System;
using System.Collections.Generic;

namespace TheMythalProphecy.Game.Characters.Stats;

/// <summary>
/// Handles calculation of final character stats from base stats, equipment, and buffs
/// </summary>
public static class StatCalculator
{
    /// <summary>
    /// Calculate final stat value
    /// </summary>
    public static int CalculateStat(
        StatType statType,
        int baseStat,
        Dictionary<StatType, int> equipmentBonuses,
        List<StatusEffect> statusEffects)
    {
        int finalStat = baseStat;

        // Add equipment bonuses
        if (equipmentBonuses != null && equipmentBonuses.TryGetValue(statType, out int equipBonus))
        {
            finalStat += equipBonus;
        }

        // Add status effect modifiers
        if (statusEffects != null)
        {
            foreach (var effect in statusEffects)
            {
                if (effect.StatModifiers.TryGetValue(statType, out int effectMod))
                {
                    finalStat += effectMod;
                }
            }
        }

        // Stats can't go below 0 (except HP which can be 0)
        return Math.Max(0, finalStat);
    }

    /// <summary>
    /// Calculate experience needed for next level
    /// Uses standard JRPG curve: BaseXP * (Level ^ 1.5)
    /// </summary>
    public static int CalculateExperienceToNextLevel(int currentLevel, int baseXP = 100)
    {
        return (int)(baseXP * Math.Pow(currentLevel, 1.5));
    }

    /// <summary>
    /// Calculate stat growth on level up
    /// </summary>
    public static int CalculateStatGrowth(int baseGrowth, float growthRate, Random random = null)
    {
        random ??= new Random();

        // Base growth + random variance
        float growth = baseGrowth + growthRate;

        // Add some randomness (-20% to +20%)
        float variance = (float)(random.NextDouble() * 0.4 - 0.2);
        growth *= (1 + variance);

        return Math.Max(1, (int)Math.Round(growth));
    }

    /// <summary>
    /// Calculate critical hit chance
    /// </summary>
    public static float CalculateCriticalChance(int luck, float baseChance = 0.05f)
    {
        // Base 5% + 0.5% per luck point
        return baseChance + (luck * 0.005f);
    }

    /// <summary>
    /// Calculate evasion chance
    /// </summary>
    public static float CalculateEvasionChance(int speed, int luck)
    {
        // 0.3% per speed point + 0.2% per luck point
        float evasion = (speed * 0.003f) + (luck * 0.002f);
        return Math.Clamp(evasion, 0f, 0.3f); // Cap at 30%
    }

    /// <summary>
    /// Calculate status resistance chance
    /// </summary>
    public static float CalculateStatusResistance(int luck, StatusEffectType effectType)
    {
        // Base resistance varies by effect type
        float baseResistance = effectType switch
        {
            StatusEffectType.Poison => 0.1f,
            StatusEffectType.Paralyzed => 0.15f,
            StatusEffectType.Sleep => 0.2f,
            StatusEffectType.Confusion => 0.25f,
            _ => 0.15f
        };

        // Add 1% per luck point
        return baseResistance + (luck * 0.01f);
    }
}
