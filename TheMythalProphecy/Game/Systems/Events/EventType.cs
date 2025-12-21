namespace TheMythalProphecy.Game.Systems.Events;

/// <summary>
/// Categories of game events for organization and filtering
/// </summary>
public enum EventType
{
    // Combat events
    CombatStarted,
    CombatEnded,
    TurnStarted,
    ActionExecuted,
    DamageDealt,
    HealingApplied,
    StatusEffectApplied,
    StatusEffectRemoved,
    CharacterDefeated,

    // Character events
    LevelUp,
    SkillLearned,
    EquipmentChanged,
    StatsChanged,

    // Party events
    PartyMemberAdded,
    PartyMemberRemoved,
    PartyChanged,

    // Inventory events
    ItemAdded,
    ItemRemoved,
    ItemUsed,
    EquipmentEquipped,
    EquipmentUnequipped,

    // Quest events
    QuestStarted,
    QuestUpdated,
    QuestCompleted,
    QuestFailed,
    ObjectiveCompleted,

    // Dialog events
    DialogStarted,
    DialogEnded,
    ChoiceMade,

    // World events
    LocationChanged,
    TriggerActivated,
    NPCInteraction,

    // Game state events
    GameSaved,
    GameLoaded,
    GamePaused,
    GameResumed
}
