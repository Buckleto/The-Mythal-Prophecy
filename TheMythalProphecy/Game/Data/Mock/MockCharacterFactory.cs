using Microsoft.Xna.Framework;
using TheMythalProphecy.Game.Characters.Stats;
using TheMythalProphecy.Game.Core;
using TheMythalProphecy.Game.Data.Definitions;
using TheMythalProphecy.Game.Data.Definitions.Databases;
using TheMythalProphecy.Game.Entities;
using TheMythalProphecy.Game.Entities.Components;

namespace TheMythalProphecy.Game.Data.Mock;

/// <summary>
/// Factory for creating Entity instances from CharacterDefinitions
/// </summary>
public static class MockCharacterFactory
{
    /// <summary>
    /// Create a character entity from a definition at a specific level
    /// </summary>
    public static Entity CreateCharacter(CharacterDefinition def, int level = 1)
    {
        var entity = new Entity(def.Id, def.Name);

        // Add transform component
        entity.AddComponent(new TransformComponent(Vector2.Zero));

        // Add stats component with calculated stats for the level
        var stats = new StatsComponent();
        ApplyBaseStats(stats, def, level);
        entity.AddComponent(stats);

        // Apply starting equipment if any
        ApplyStartingEquipment(stats, def);

        return entity;
    }

    /// <summary>
    /// Apply base stats scaled to level
    /// </summary>
    private static void ApplyBaseStats(StatsComponent stats, CharacterDefinition def, int level)
    {
        stats.SetBaseStat(StatType.Level, level);

        // Calculate stats based on base + (growth * level)
        int maxHP = def.BaseStats.MaxHP + (int)(def.GrowthRates.HP * level);
        int maxMP = def.BaseStats.MaxMP + (int)(def.GrowthRates.MP * level);

        stats.SetBaseStat(StatType.MaxHP, maxHP);
        stats.SetBaseStat(StatType.MaxMP, maxMP);
        stats.SetBaseStat(StatType.HP, maxHP);
        stats.SetBaseStat(StatType.MP, maxMP);

        stats.SetBaseStat(StatType.Strength,
            def.BaseStats.Strength + (int)(def.GrowthRates.Strength * level));
        stats.SetBaseStat(StatType.Defense,
            def.BaseStats.Defense + (int)(def.GrowthRates.Defense * level));
        stats.SetBaseStat(StatType.MagicPower,
            def.BaseStats.MagicPower + (int)(def.GrowthRates.MagicPower * level));
        stats.SetBaseStat(StatType.MagicDefense,
            def.BaseStats.MagicDefense + (int)(def.GrowthRates.MagicDefense * level));
        stats.SetBaseStat(StatType.Speed,
            def.BaseStats.Speed + (int)(def.GrowthRates.Speed * level));
        stats.SetBaseStat(StatType.Luck,
            def.BaseStats.Luck + (int)(def.GrowthRates.Luck * level));
    }

    /// <summary>
    /// Apply starting equipment to character
    /// </summary>
    private static void ApplyStartingEquipment(StatsComponent stats, CharacterDefinition def)
    {
        if (def.StartingEquipment == null)
            return;

        var equipDb = GameServices.GameData.EquipmentDatabase;

        if (!string.IsNullOrEmpty(def.StartingEquipment.Weapon))
        {
            EquipItem(stats, def.StartingEquipment.Weapon, equipDb);
        }

        if (!string.IsNullOrEmpty(def.StartingEquipment.Armor))
        {
            EquipItem(stats, def.StartingEquipment.Armor, equipDb);
        }

        if (!string.IsNullOrEmpty(def.StartingEquipment.Accessory))
        {
            EquipItem(stats, def.StartingEquipment.Accessory, equipDb);
        }
    }

    private static void EquipItem(StatsComponent stats, string equipmentId, EquipmentDatabase equipDb)
    {
        var equipment = equipDb.Get(equipmentId);
        if (equipment == null) return;

        // Apply stat bonuses
        if (equipment.StatBonuses != null)
        {
            foreach (var bonus in equipment.StatBonuses)
            {
                stats.AddEquipmentBonus(bonus.Key, bonus.Value);
            }
        }

        // Mark as equipped
        stats.SetEquippedItem(equipment.Slot, equipmentId);
    }

    /// <summary>
    /// Add a test status effect to a character
    /// </summary>
    public static void ApplyTestStatusEffect(Entity character, StatusEffectType effectType)
    {
        var stats = character.GetComponent<StatsComponent>();
        if (stats == null) return;

        StatusEffect effect = effectType switch
        {
            StatusEffectType.Regen => new StatusEffect(StatusEffectType.Regen, "Regen", 30f, true)
            {
                Description = "Gradually restores HP over time",
                TickDamage = -5, // Negative for healing
                TickInterval = 3f
            },
            StatusEffectType.Haste => new StatusEffect(StatusEffectType.Haste, "Haste", 60f, true)
            {
                Description = "Increases action speed",
                TickDamage = 0,
                TickInterval = 1f
            },
            StatusEffectType.Protect => new StatusEffect(StatusEffectType.Protect, "Protect", 120f, true)
            {
                Description = "Increases physical defense",
                TickDamage = 0,
                TickInterval = 1f
            },
            _ => null
        };

        if (effect != null)
        {
            // Add stat modifiers for certain effects
            if (effectType == StatusEffectType.Haste)
            {
                effect.AddStatModifier(StatType.Speed, 10);
            }
            else if (effectType == StatusEffectType.Protect)
            {
                effect.AddStatModifier(StatType.Defense, 5);
            }

            stats.AddStatusEffect(effect);
        }
    }
}
