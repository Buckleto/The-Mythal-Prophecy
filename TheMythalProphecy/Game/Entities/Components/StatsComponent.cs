using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TheMythalProphecy.Game.Characters.Stats;
using TheMythalProphecy.Game.Systems.Events;

namespace TheMythalProphecy.Game.Entities.Components;

/// <summary>
/// Component that holds character statistics (HP, MP, Strength, etc.)
/// </summary>
public class StatsComponent : IComponent
{
    public Entity Owner { get; set; }
    public bool Enabled { get; set; } = true;

    // Base stats (before equipment/buffs)
    private readonly Dictionary<StatType, int> _baseStats = new();

    // Equipment bonuses
    private readonly Dictionary<StatType, int> _equipmentBonuses = new();

    // Equipped items by slot
    private readonly Dictionary<Data.Definitions.EquipmentSlot, string> _equippedItems = new();

    // Active status effects
    private readonly List<StatusEffect> _statusEffects = new();

    // Quick access properties
    public int CurrentHP
    {
        get => _baseStats.GetValueOrDefault(StatType.HP);
        set => _baseStats[StatType.HP] = Math.Clamp(value, 0, MaxHP);
    }

    public int MaxHP => GetStat(StatType.MaxHP);

    public int CurrentMP
    {
        get => _baseStats.GetValueOrDefault(StatType.MP);
        set => _baseStats[StatType.MP] = Math.Clamp(value, 0, MaxMP);
    }

    public int MaxMP => GetStat(StatType.MaxMP);

    public int Level
    {
        get => _baseStats.GetValueOrDefault(StatType.Level, 1);
        set => _baseStats[StatType.Level] = Math.Max(1, value);
    }

    public int Experience
    {
        get => _baseStats.GetValueOrDefault(StatType.Experience);
        set => _baseStats[StatType.Experience] = Math.Max(0, value);
    }

    public int ExperienceToNext => StatCalculator.CalculateExperienceToNextLevel(Level);

    public bool IsAlive => CurrentHP > 0;
    public bool IsDead => CurrentHP <= 0;

    public StatsComponent()
    {
        InitializeDefaultStats();
    }

    public void Initialize()
    {
        // Component initialization
    }

    public void Update(GameTime gameTime)
    {
        // Update status effects
        for (int i = _statusEffects.Count - 1; i >= 0; i--)
        {
            var effect = _statusEffects[i];
            int tickDamage = effect.Update(gameTime);

            // Apply tick damage/healing
            if (tickDamage != 0)
            {
                TakeDamage(tickDamage);
            }

            // Remove expired effects
            if (effect.IsExpired)
            {
                _statusEffects.RemoveAt(i);
                Core.GameServices.Events?.Publish(new StatusEffectRemovedEvent(Owner, effect));
            }
        }
    }

    /// <summary>
    /// Initialize default stat values
    /// </summary>
    private void InitializeDefaultStats()
    {
        SetBaseStat(StatType.MaxHP, 100);
        SetBaseStat(StatType.MaxMP, 20);
        SetBaseStat(StatType.HP, 100);
        SetBaseStat(StatType.MP, 20);
        SetBaseStat(StatType.Level, 1);
        SetBaseStat(StatType.Strength, 10);
        SetBaseStat(StatType.Defense, 10);
        SetBaseStat(StatType.MagicPower, 10);
        SetBaseStat(StatType.MagicDefense, 10);
        SetBaseStat(StatType.Speed, 10);
        SetBaseStat(StatType.Luck, 10);
    }

    /// <summary>
    /// Set a base stat value
    /// </summary>
    public void SetBaseStat(StatType stat, int value)
    {
        _baseStats[stat] = value;
    }

    /// <summary>
    /// Get base stat value (before modifiers)
    /// </summary>
    public int GetBaseStat(StatType stat)
    {
        return _baseStats.GetValueOrDefault(stat);
    }

    /// <summary>
    /// Get final stat value (after equipment and buffs)
    /// </summary>
    public int GetStat(StatType stat)
    {
        // HP and MP use current values, not calculated
        if (stat == StatType.HP) return CurrentHP;
        if (stat == StatType.MP) return CurrentMP;

        return StatCalculator.CalculateStat(
            stat,
            GetBaseStat(stat),
            _equipmentBonuses,
            _statusEffects
        );
    }

    /// <summary>
    /// Add equipment stat bonus
    /// </summary>
    public void AddEquipmentBonus(StatType stat, int amount)
    {
        _equipmentBonuses.TryGetValue(stat, out int current);
        _equipmentBonuses[stat] = current + amount;
    }

    /// <summary>
    /// Remove equipment stat bonus
    /// </summary>
    public void RemoveEquipmentBonus(StatType stat, int amount)
    {
        _equipmentBonuses.TryGetValue(stat, out int current);
        _equipmentBonuses[stat] = current - amount;
    }

    /// <summary>
    /// Clear all equipment bonuses
    /// </summary>
    public void ClearEquipmentBonuses()
    {
        _equipmentBonuses.Clear();
    }

    /// <summary>
    /// Get equipped item ID for a slot (returns null if no item equipped)
    /// </summary>
    public string GetEquippedItem(Data.Definitions.EquipmentSlot slot)
    {
        return _equippedItems.TryGetValue(slot, out string itemId) ? itemId : null;
    }

    /// <summary>
    /// Set equipped item for a slot
    /// </summary>
    public void SetEquippedItem(Data.Definitions.EquipmentSlot slot, string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            _equippedItems.Remove(slot);
        }
        else
        {
            _equippedItems[slot] = itemId;
        }
    }

    /// <summary>
    /// Get all equipped items
    /// </summary>
    public IReadOnlyDictionary<Data.Definitions.EquipmentSlot, string> GetAllEquippedItems()
    {
        return _equippedItems;
    }

    /// <summary>
    /// Apply damage to this character
    /// </summary>
    /// <returns>Actual damage dealt</returns>
    public int TakeDamage(int amount)
    {
        if (amount <= 0) return 0;

        int actualDamage = Math.Min(amount, CurrentHP);
        CurrentHP -= actualDamage;

        if (IsDead)
        {
            Core.GameServices.Events?.Publish(new CharacterDefeatedEvent(Owner));
        }

        return actualDamage;
    }

    /// <summary>
    /// Heal this character
    /// </summary>
    /// <returns>Actual healing done</returns>
    public int Heal(int amount)
    {
        if (amount <= 0 || IsDead) return 0;

        int oldHP = CurrentHP;
        CurrentHP += amount;
        return CurrentHP - oldHP;
    }

    /// <summary>
    /// Restore MP
    /// </summary>
    /// <returns>Actual MP restored</returns>
    public int RestoreMP(int amount)
    {
        if (amount <= 0) return 0;

        int oldMP = CurrentMP;
        CurrentMP += amount;
        return CurrentMP - oldMP;
    }

    /// <summary>
    /// Consume MP
    /// </summary>
    /// <returns>True if had enough MP</returns>
    public bool ConsumeMP(int amount)
    {
        if (CurrentMP < amount) return false;

        CurrentMP -= amount;
        return true;
    }

    /// <summary>
    /// Add a status effect
    /// </summary>
    public void AddStatusEffect(StatusEffect effect)
    {
        _statusEffects.Add(effect);
        Core.GameServices.Events?.Publish(new StatusEffectAppliedEvent(Owner, effect));
    }

    /// <summary>
    /// Remove all status effects of a specific type
    /// </summary>
    public void RemoveStatusEffect(StatusEffectType type)
    {
        _statusEffects.RemoveAll(e => e.Type == type);
    }

    /// <summary>
    /// Check if has a specific status effect
    /// </summary>
    public bool HasStatusEffect(StatusEffectType type)
    {
        return _statusEffects.Any(e => e.Type == type);
    }

    /// <summary>
    /// Get all active status effects
    /// </summary>
    public IReadOnlyList<StatusEffect> GetStatusEffects()
    {
        return _statusEffects.AsReadOnly();
    }

    /// <summary>
    /// Add experience and check for level up
    /// </summary>
    /// <returns>True if leveled up</returns>
    public bool AddExperience(int amount)
    {
        Experience += amount;

        if (Experience >= ExperienceToNext)
        {
            LevelUp();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Level up the character
    /// </summary>
    private void LevelUp()
    {
        Level++;
        Experience -= ExperienceToNext;

        // This will be enhanced with growth curves from character definitions later
        // For now, simple stat increases
        SetBaseStat(StatType.MaxHP, MaxHP + 10);
        SetBaseStat(StatType.MaxMP, MaxMP + 3);
        SetBaseStat(StatType.Strength, GetBaseStat(StatType.Strength) + 2);
        SetBaseStat(StatType.Defense, GetBaseStat(StatType.Defense) + 2);
        SetBaseStat(StatType.MagicPower, GetBaseStat(StatType.MagicPower) + 2);
        SetBaseStat(StatType.MagicDefense, GetBaseStat(StatType.MagicDefense) + 2);
        SetBaseStat(StatType.Speed, GetBaseStat(StatType.Speed) + 1);
        SetBaseStat(StatType.Luck, GetBaseStat(StatType.Luck) + 1);

        // Restore HP/MP on level up
        CurrentHP = MaxHP;
        CurrentMP = MaxMP;

        Core.GameServices.Events?.Publish(new LevelUpEvent(Owner, Level));
    }

    /// <summary>
    /// Fully restore HP and MP
    /// </summary>
    public void FullRestore()
    {
        CurrentHP = MaxHP;
        CurrentMP = MaxMP;
        _statusEffects.Clear();
    }
}

// Additional event classes
public class CharacterDefeatedEvent : GameEvent
{
    public object Character { get; }

    public CharacterDefeatedEvent(object character) : base(EventType.CharacterDefeated)
    {
        Character = character;
    }
}

public class StatusEffectAppliedEvent : GameEvent
{
    public object Target { get; }
    public StatusEffect Effect { get; }

    public StatusEffectAppliedEvent(object target, StatusEffect effect)
        : base(EventType.StatusEffectApplied)
    {
        Target = target;
        Effect = effect;
    }
}

public class StatusEffectRemovedEvent : GameEvent
{
    public object Target { get; }
    public StatusEffect Effect { get; }

    public StatusEffectRemovedEvent(object target, StatusEffect effect)
        : base(EventType.StatusEffectRemoved)
    {
        Target = target;
        Effect = effect;
    }
}
