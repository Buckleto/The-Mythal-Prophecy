using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using TheMythalProphecy.Game.Core;
using TheMythalProphecy.Game.Entities;
using TheMythalProphecy.Game.Entities.Components;
using TheMythalProphecy.Game.Characters.Stats;
using TheMythalProphecy.Game.UI.Components;

namespace TheMythalProphecy.Game.States;

/// <summary>
/// Character status menu - view detailed character stats and information
/// </summary>
public class CharacterStatusState : IGameState
{
    private readonly GameStateManager _stateManager;
    private UIWindow _window;
    private UIListBox _characterList;
    private UIPanel _statusPanel;
    private UILabel _nameLabel;
    private UILabel _levelLabel;
    private UILabel _hpLabel;
    private UILabel _mpLabel;
    private UILabel _expLabel;
    private UIProgressBar _expBar;
    private UILabel _statsLabel;
    private UILabel _statusEffectsLabel;
    private KeyboardState _previousKeyState;

    private Entity _selectedCharacter;

    public CharacterStatusState(GameStateManager stateManager)
    {
        _stateManager = stateManager;
    }

    public void Enter()
    {
        // Create window (900x650)
        int screenWidth = GameServices.GraphicsDevice.Viewport.Width;
        int screenHeight = GameServices.GraphicsDevice.Viewport.Height;

        Vector2 windowSize = new Vector2(900, 650);
        Vector2 windowPos = new Vector2(
            (screenWidth - windowSize.X) / 2,
            (screenHeight - windowSize.Y) / 2
        );

        _window = new UIWindow(windowPos, windowSize, "Character Status")
        {
            IsModal = true,
            ShowCloseButton = false
        };

        // Left: Character list (180px)
        _characterList = new UIListBox(new Vector2(10, 10), new Vector2(180, 550))
        {
            ItemHeight = 50
        };
        _characterList.OnSelectionChanged += OnCharacterSelectionChanged;
        _window.ContentPanel.AddChild(_characterList);

        // Populate character list
        var party = GameServices.GameData.Party.ActiveParty;
        foreach (var character in party)
        {
            var stats = character.GetComponent<StatsComponent>();
            string displayName = $"{character.Name}\nLv.{stats.Level}";
            _characterList.AddItem(displayName);
        }
        if (_characterList.Items.Count > 0)
        {
            _characterList.SelectedIndex = 0;
        }

        // Right: Status panel (680px)
        _statusPanel = new UIPanel(new Vector2(200, 10), new Vector2(680, 550))
        {
            BackgroundColor = new Color(30, 30, 50, 200)
        };

        // Character header
        _nameLabel = new UILabel("Character Name", new Vector2(10, 10));
        _nameLabel.TextColor = Color.Gold;
        _nameLabel.Scale = 1.2f;
        _statusPanel.AddChild(_nameLabel);

        _levelLabel = new UILabel("Level: 1", new Vector2(10, 40));
        _levelLabel.TextColor = Color.White;
        _statusPanel.AddChild(_levelLabel);

        // HP/MP display
        _hpLabel = new UILabel("HP: 100 / 100", new Vector2(10, 70));
        _hpLabel.TextColor = Color.LightGreen;
        _statusPanel.AddChild(_hpLabel);

        _mpLabel = new UILabel("MP: 20 / 20", new Vector2(10, 95));
        _mpLabel.TextColor = Color.LightBlue;
        _statusPanel.AddChild(_mpLabel);

        // Experience
        _expLabel = new UILabel("EXP: 0 / 100", new Vector2(10, 130));
        _expLabel.TextColor = Color.Yellow;
        _statusPanel.AddChild(_expLabel);

        _expBar = new UIProgressBar(new Vector2(10, 155), new Vector2(660, 20));
        _expBar.FillColor = Color.Yellow;
        _expBar.BackgroundColor = new Color(40, 40, 40);
        _expBar.ShowText = false;
        _statusPanel.AddChild(_expBar);

        // Stats display
        var statsTitle = new UILabel("Stats", new Vector2(10, 190));
        statsTitle.TextColor = Color.Cyan;
        statsTitle.Scale = 1.1f;
        _statusPanel.AddChild(statsTitle);

        _statsLabel = new UILabel("", new Vector2(10, 220));
        _statsLabel.TextColor = Color.White;
        _statsLabel.Size = new Vector2(660, 200);
        _statusPanel.AddChild(_statsLabel);

        // Status effects
        var statusEffectsTitle = new UILabel("Status Effects", new Vector2(10, 430));
        statusEffectsTitle.TextColor = Color.Magenta;
        statusEffectsTitle.Scale = 1.1f;
        _statusPanel.AddChild(statusEffectsTitle);

        _statusEffectsLabel = new UILabel("None", new Vector2(10, 460));
        _statusEffectsLabel.TextColor = Color.LightGray;
        _statusEffectsLabel.Size = new Vector2(660, 80);
        _statusPanel.AddChild(_statusEffectsLabel);

        _window.ContentPanel.AddChild(_statusPanel);

        // Instructions
        var instructionsLabel = new UILabel("Esc: Close", new Vector2(10, 570));
        instructionsLabel.TextColor = Color.LightGray;
        _window.ContentPanel.AddChild(instructionsLabel);

        // Register window
        GameServices.UI.AddElement(_window);

        // Give focus to character list
        _characterList.IsFocused = true;

        // Refresh display
        if (_characterList.Items.Count > 0)
        {
            RefreshStatusDisplay();
        }
    }

    public void Exit()
    {
        GameServices.UI.RemoveElement(_window);
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState keyState = Keyboard.GetState();

        // Escape to close
        if ((keyState.IsKeyDown(Keys.Escape) && !_previousKeyState.IsKeyDown(Keys.Escape)) ||
            GameServices.Input.IsCancelPressed())
        {
            _stateManager.PopState();
        }

        _previousKeyState = keyState;
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        // UI Manager handles drawing
    }

    /// <summary>
    /// Handle character selection change
    /// </summary>
    private void OnCharacterSelectionChanged(UIListBox sender, int index)
    {
        var party = GameServices.GameData.Party.ActiveParty;
        if (index >= 0 && index < party.Count)
        {
            _selectedCharacter = party[index];
            RefreshStatusDisplay();
        }
    }

    /// <summary>
    /// Refresh the status display for the selected character
    /// </summary>
    private void RefreshStatusDisplay()
    {
        if (_selectedCharacter == null) return;

        var stats = _selectedCharacter.GetComponent<StatsComponent>();
        if (stats == null) return;

        // Update name and level
        _nameLabel.Text = _selectedCharacter.Name;
        _levelLabel.Text = $"Level: {stats.Level}";

        // Update HP/MP
        _hpLabel.Text = $"HP: {stats.CurrentHP} / {stats.MaxHP}";
        _mpLabel.Text = $"MP: {stats.CurrentMP} / {stats.MaxMP}";

        // Update experience
        int currentExp = stats.Experience;
        int expToNext = stats.ExperienceToNext;
        _expLabel.Text = $"EXP: {currentExp} / {expToNext}";
        _expBar.CurrentValue = currentExp;
        _expBar.MaxValue = expToNext;

        // Update stats
        var statsText = BuildStatsText(stats);
        _statsLabel.Text = statsText;

        // Update status effects
        var statusEffectsText = BuildStatusEffectsText(stats);
        _statusEffectsLabel.Text = statusEffectsText;
    }

    /// <summary>
    /// Build stats text display
    /// </summary>
    private string BuildStatsText(StatsComponent stats)
    {
        var lines = new System.Collections.Generic.List<string>();

        // Base stats
        lines.Add("--- Combat Stats ---");
        lines.Add($"Strength:      {stats.GetStat(StatType.Strength):D3}");
        lines.Add($"Defense:       {stats.GetStat(StatType.Defense):D3}");
        lines.Add($"Magic Power:   {stats.GetStat(StatType.MagicPower):D3}");
        lines.Add($"Magic Defense: {stats.GetStat(StatType.MagicDefense):D3}");
        lines.Add($"Speed:         {stats.GetStat(StatType.Speed):D3}");
        lines.Add($"Luck:          {stats.GetStat(StatType.Luck):D3}");

        // Show equipped items
        lines.Add("");
        lines.Add("--- Equipment ---");
        var equippedItems = stats.GetAllEquippedItems();
        if (equippedItems.Count > 0)
        {
            foreach (var kvp in equippedItems)
            {
                lines.Add($"{kvp.Key}: {kvp.Value}");
            }
        }
        else
        {
            lines.Add("No equipment");
        }

        return string.Join("\n", lines);
    }

    /// <summary>
    /// Build status effects text display
    /// </summary>
    private string BuildStatusEffectsText(StatsComponent stats)
    {
        // TODO: Get actual status effects from StatsComponent
        // For now, just return "None"
        return "None";
    }
}
