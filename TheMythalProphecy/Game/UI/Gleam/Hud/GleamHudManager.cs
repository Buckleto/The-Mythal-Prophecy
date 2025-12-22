using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheMythalProphecy.Game.Systems.Events;
using TheMythalProphecy.Game.Entities.Components;

namespace TheMythalProphecy.Game.UI.Gleam;

/// <summary>
/// Manages GleamUI-based HUD elements.
/// Subscribes to game events for automatic updates.
/// </summary>
public class GleamHudManager
{
    private readonly List<GleamElement> _hudElements;
    private readonly HudTheme _hudTheme;
    private readonly GleamRenderer _renderer;
    private readonly HudPartyStatus _partyStatus;
    private readonly HudMessageLog _messageLog;
    private bool _isVisible;
    private bool _eventsSubscribed;

    public HudPartyStatus PartyStatus => _partyStatus;
    public HudMessageLog MessageLog => _messageLog;
    public HudTheme Theme => _hudTheme;

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            foreach (var element in _hudElements)
            {
                element.Visible = value;
            }
        }
    }

    public GleamHudManager(int screenWidth, int screenHeight, GleamRenderer renderer, HudTheme hudTheme)
    {
        _renderer = renderer;
        _hudTheme = hudTheme;
        _hudElements = new List<GleamElement>();
        _isVisible = true;
        _eventsSubscribed = false;

        // Create HUD elements
        _partyStatus = new HudPartyStatus(
            new Vector2(10, screenHeight - 120),
            new Vector2(screenWidth - 20, 110),
            hudTheme
        );

        _messageLog = new HudMessageLog(
            new Vector2(10, 10),
            new Vector2(400, 150),
            hudTheme
        );

        _hudElements.Add(_partyStatus);
        _hudElements.Add(_messageLog);

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (_eventsSubscribed) return;

        var eventManager = Core.GameServices.Events;
        if (eventManager == null) return;

        // Party events
        eventManager.Subscribe<PartyChangedEvent>(OnPartyChanged);

        // Combat events
        eventManager.Subscribe<DamageDealtEvent>(OnDamageDealt);
        eventManager.Subscribe<HealingAppliedEvent>(OnHealingApplied);
        eventManager.Subscribe<CombatStartedEvent>(OnCombatStarted);

        // Character events
        eventManager.Subscribe<LevelUpEvent>(OnLevelUp);
        eventManager.Subscribe<SkillLearnedEvent>(OnSkillLearned);

        // Inventory events
        eventManager.Subscribe<ItemUsedEvent>(OnItemUsed);
        eventManager.Subscribe<ItemAddedEvent>(OnItemAdded);

        _eventsSubscribed = true;
    }

    public void UnsubscribeFromEvents()
    {
        if (!_eventsSubscribed) return;

        var eventManager = Core.GameServices.Events;
        if (eventManager == null) return;

        eventManager.Unsubscribe<PartyChangedEvent>(OnPartyChanged);
        eventManager.Unsubscribe<DamageDealtEvent>(OnDamageDealt);
        eventManager.Unsubscribe<HealingAppliedEvent>(OnHealingApplied);
        eventManager.Unsubscribe<CombatStartedEvent>(OnCombatStarted);
        eventManager.Unsubscribe<LevelUpEvent>(OnLevelUp);
        eventManager.Unsubscribe<SkillLearnedEvent>(OnSkillLearned);
        eventManager.Unsubscribe<ItemUsedEvent>(OnItemUsed);
        eventManager.Unsubscribe<ItemAddedEvent>(OnItemAdded);

        _eventsSubscribed = false;
    }

    #region Event Handlers

    private void OnPartyChanged(PartyChangedEvent evt)
    {
        RefreshPartyStatus();
        _messageLog.AddSystemMessage("Party composition changed.");
    }

    private void OnDamageDealt(DamageDealtEvent evt)
    {
        string message = evt.IsCritical
            ? $"Critical hit! {evt.Amount} damage!"
            : $"{evt.Amount} damage dealt.";
        _messageLog.AddDamageMessage(message);
    }

    private void OnHealingApplied(HealingAppliedEvent evt)
    {
        _messageLog.AddHealMessage($"{evt.Amount} HP restored.");
        RefreshPartyStatus();
    }

    private void OnCombatStarted(CombatStartedEvent evt)
    {
        _messageLog.AddCombatMessage($"Battle started! {evt.Enemies.Count} enemies!");
    }

    private void OnLevelUp(LevelUpEvent evt)
    {
        _messageLog.AddSystemMessage($"Level up! Now level {evt.NewLevel}!");
        RefreshPartyStatus();
    }

    private void OnSkillLearned(SkillLearnedEvent evt)
    {
        _messageLog.AddSystemMessage($"New skill learned: {evt.SkillId}!");
    }

    private void OnItemUsed(ItemUsedEvent evt)
    {
        _messageLog.AddSystemMessage($"Used {evt.ItemId}.");
        RefreshPartyStatus();
    }

    private void OnItemAdded(ItemAddedEvent evt)
    {
        _messageLog.AddSystemMessage($"Obtained {evt.ItemId} x{evt.Quantity}.");
    }

    #endregion

    public void RefreshPartyStatus()
    {
        var party = Core.GameServices.GameData?.Party;
        if (party == null) return;

        var activeParty = party.ActiveParty;
        _partyStatus.ClearAll();
        _partyStatus.SetActiveCount(activeParty.Count);

        for (int i = 0; i < activeParty.Count; i++)
        {
            var character = activeParty[i];
            var stats = character.GetComponent<StatsComponent>();
            if (stats != null)
            {
                _partyStatus.UpdateMember(
                    i,
                    character.Name,
                    stats.CurrentHP,
                    stats.MaxHP,
                    stats.CurrentMP,
                    stats.MaxMP
                );
            }
        }
    }

    public void Update(GameTime gameTime)
    {
        if (!_isVisible) return;

        foreach (var element in _hudElements)
        {
            if (element.Visible)
            {
                element.Update(gameTime, _renderer);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!_isVisible) return;

        foreach (var element in _hudElements)
        {
            if (element.Visible)
            {
                element.Draw(spriteBatch, _renderer);
            }
        }
    }

    public void ShowPartyStatus(bool show) => _partyStatus.Visible = show;
    public void ShowMessageLog(bool show) => _messageLog.Visible = show;
    public void Hide() => IsVisible = false;
    public void Show() => IsVisible = true;
}
