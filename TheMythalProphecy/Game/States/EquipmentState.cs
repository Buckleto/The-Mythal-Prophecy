using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using TheMythalProphecy.Game.Core;
using TheMythalProphecy.Game.Data.Definitions;
using TheMythalProphecy.Game.Entities;
using TheMythalProphecy.Game.Entities.Components;
using TheMythalProphecy.Game.Characters.Stats;
using TheMythalProphecy.Game.UI.Components;

namespace TheMythalProphecy.Game.States;

/// <summary>
/// Equipment menu state - equip/unequip weapons, armor, and accessories
/// </summary>
public class EquipmentState : IGameState
{
    private readonly GameStateManager _stateManager;
    private UIWindow _window;
    private UIListBox _characterList;
    private UIPanel _equippedPanel;
    private UILabel _weaponLabel;
    private UILabel _armorLabel;
    private UILabel _accessoryLabel;
    private UIListBox _availableEquipmentList;
    private UIPanel _statsPanel;
    private UILabel _statsComparisonLabel;
    private KeyboardState _previousKeyState;

    // Currently selected character and slot
    private Entity _selectedCharacter;
    private EquipmentSlot _selectedSlot = EquipmentSlot.Weapon;
    private int _focusedPanel = 0; // 0=characters, 1=slots, 2=available equipment

    public EquipmentState(GameStateManager stateManager)
    {
        _stateManager = stateManager;
    }

    public void Enter()
    {
        // Create window (1000x600)
        int screenWidth = GameServices.GraphicsDevice.Viewport.Width;
        int screenHeight = GameServices.GraphicsDevice.Viewport.Height;

        Vector2 windowSize = new Vector2(1000, 600);
        Vector2 windowPos = new Vector2(
            (screenWidth - windowSize.X) / 2,
            (screenHeight - windowSize.Y) / 2
        );

        _window = new UIWindow(windowPos, windowSize, "Equipment")
        {
            IsModal = true,
            ShowCloseButton = false
        };

        // Left: Character list (200px)
        _characterList = new UIListBox(new Vector2(10, 10), new Vector2(200, 450))
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

        // Middle: Equipped items panel (250px)
        _equippedPanel = new UIPanel(new Vector2(220, 10), new Vector2(250, 450))
        {
            BackgroundColor = new Color(30, 30, 50, 200)
        };

        var equippedTitle = new UILabel("Equipped", new Vector2(10, 10));
        equippedTitle.TextColor = Color.Gold;
        _equippedPanel.AddChild(equippedTitle);

        _weaponLabel = new UILabel("[Weapon]\nNone", new Vector2(10, 50));
        _weaponLabel.TextColor = Color.White;
        _equippedPanel.AddChild(_weaponLabel);

        _armorLabel = new UILabel("[Armor]\nNone", new Vector2(10, 150));
        _armorLabel.TextColor = Color.White;
        _equippedPanel.AddChild(_armorLabel);

        _accessoryLabel = new UILabel("[Accessory]\nNone", new Vector2(10, 250));
        _accessoryLabel.TextColor = Color.White;
        _equippedPanel.AddChild(_accessoryLabel);

        _window.ContentPanel.AddChild(_equippedPanel);

        // Right: Available equipment list (280px)
        _availableEquipmentList = new UIListBox(new Vector2(480, 10), new Vector2(280, 300))
        {
            ItemHeight = 35
        };
        _availableEquipmentList.OnSelectionChanged += OnEquipmentSelectionChanged;
        _availableEquipmentList.OnItemActivated += OnEquipmentActivated;
        _window.ContentPanel.AddChild(_availableEquipmentList);

        // Stats comparison panel (280px)
        _statsPanel = new UIPanel(new Vector2(480, 320), new Vector2(280, 140))
        {
            BackgroundColor = new Color(40, 40, 60, 200)
        };

        _statsComparisonLabel = new UILabel("Select equipment to compare", new Vector2(10, 10));
        _statsComparisonLabel.TextColor = Color.LightGray;
        _statsComparisonLabel.Size = new Vector2(260, 120);
        _statsPanel.AddChild(_statsComparisonLabel);

        _window.ContentPanel.AddChild(_statsPanel);

        // Instructions
        var instructionsLabel = new UILabel("Tab: Switch Focus | Enter: Equip/Unequip | Esc: Close", new Vector2(10, 470));
        instructionsLabel.TextColor = Color.LightGray;
        _window.ContentPanel.AddChild(instructionsLabel);

        // Register window
        GameServices.UI.AddElement(_window);

        // Give focus to character list
        _characterList.IsFocused = true;

        // Refresh displays
        RefreshEquippedDisplay();
        RefreshAvailableEquipment();
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

        // Tab to switch focus
        if (keyState.IsKeyDown(Keys.Tab) && !_previousKeyState.IsKeyDown(Keys.Tab))
        {
            CycleFocus();
        }

        // Arrow keys to navigate slots when in equipped panel
        if (_focusedPanel == 1)
        {
            if (keyState.IsKeyDown(Keys.Down) && !_previousKeyState.IsKeyDown(Keys.Down))
            {
                _selectedSlot = _selectedSlot switch
                {
                    EquipmentSlot.Weapon => EquipmentSlot.Armor,
                    EquipmentSlot.Armor => EquipmentSlot.Accessory,
                    _ => _selectedSlot
                };
                RefreshAvailableEquipment();
                UpdateSlotHighlight();
            }
            else if (keyState.IsKeyDown(Keys.Up) && !_previousKeyState.IsKeyDown(Keys.Up))
            {
                _selectedSlot = _selectedSlot switch
                {
                    EquipmentSlot.Accessory => EquipmentSlot.Armor,
                    EquipmentSlot.Armor => EquipmentSlot.Weapon,
                    _ => _selectedSlot
                };
                RefreshAvailableEquipment();
                UpdateSlotHighlight();
            }
            else if ((keyState.IsKeyDown(Keys.Enter) && !_previousKeyState.IsKeyDown(Keys.Enter)) ||
                     GameServices.Input.IsAcceptPressed())
            {
                // Unequip current item in selected slot
                UnequipSlot(_selectedSlot);
            }
        }

        _previousKeyState = keyState;
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        // UI Manager handles drawing
    }

    /// <summary>
    /// Cycle focus between panels
    /// </summary>
    private void CycleFocus()
    {
        _characterList.IsFocused = false;
        _availableEquipmentList.IsFocused = false;

        _focusedPanel = (_focusedPanel + 1) % 3;

        switch (_focusedPanel)
        {
            case 0:
                _characterList.IsFocused = true;
                break;
            case 1:
                // Equipped panel (manual keyboard handling)
                UpdateSlotHighlight();
                break;
            case 2:
                _availableEquipmentList.IsFocused = true;
                break;
        }
    }

    /// <summary>
    /// Update slot highlight colors
    /// </summary>
    private void UpdateSlotHighlight()
    {
        _weaponLabel.TextColor = _selectedSlot == EquipmentSlot.Weapon ? Color.Yellow : Color.White;
        _armorLabel.TextColor = _selectedSlot == EquipmentSlot.Armor ? Color.Yellow : Color.White;
        _accessoryLabel.TextColor = _selectedSlot == EquipmentSlot.Accessory ? Color.Yellow : Color.White;
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
            RefreshEquippedDisplay();
            RefreshAvailableEquipment();
        }
    }

    /// <summary>
    /// Refresh the equipped items display
    /// </summary>
    private void RefreshEquippedDisplay()
    {
        if (_selectedCharacter == null) return;

        var stats = _selectedCharacter.GetComponent<StatsComponent>();
        if (stats == null) return;

        // Update weapon
        string weaponId = stats.GetEquippedItem(EquipmentSlot.Weapon);
        var weapon = weaponId != null ? GameServices.GameData.EquipmentDatabase.Get(weaponId) : null;
        _weaponLabel.Text = weapon != null
            ? $"[Weapon]\n{weapon.Name}"
            : "[Weapon]\nNone";

        // Update armor
        string armorId = stats.GetEquippedItem(EquipmentSlot.Armor);
        var armor = armorId != null ? GameServices.GameData.EquipmentDatabase.Get(armorId) : null;
        _armorLabel.Text = armor != null
            ? $"[Armor]\n{armor.Name}"
            : "[Armor]\nNone";

        // Update accessory
        string accessoryId = stats.GetEquippedItem(EquipmentSlot.Accessory);
        var accessory = accessoryId != null ? GameServices.GameData.EquipmentDatabase.Get(accessoryId) : null;
        _accessoryLabel.Text = accessory != null
            ? $"[Accessory]\n{accessory.Name}"
            : "[Accessory]\nNone";

        UpdateSlotHighlight();
    }

    /// <summary>
    /// Refresh available equipment list based on selected slot
    /// </summary>
    private void RefreshAvailableEquipment()
    {
        _availableEquipmentList.ClearItems();

        var inventory = GameServices.GameData.Inventory;

        // Add "None" option to unequip
        _availableEquipmentList.AddItem("--- None ---");

        foreach (var itemId in inventory.GetItemIds())
        {
            var equipment = GameServices.GameData.EquipmentDatabase.Get(itemId);
            if (equipment != null)
            {
                if (equipment.Slot == _selectedSlot)
                {
                    _availableEquipmentList.AddItem(equipment.Name);
                }
            }
        }

        if (_availableEquipmentList.Items.Count > 0)
        {
            _availableEquipmentList.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Handle equipment selection change for stat comparison
    /// </summary>
    private void OnEquipmentSelectionChanged(UIListBox sender, int index)
    {
        if (_selectedCharacter == null) return;
        if (index <= 0) // "None" selected
        {
            _statsComparisonLabel.Text = "Select equipment to compare";
            return;
        }

        // Get selected equipment
        var equipmentIds = GameServices.GameData.Inventory.GetItemIds()
            .Where(id =>
            {
                var eq = GameServices.GameData.EquipmentDatabase.Get(id);
                return eq != null && eq.Slot == _selectedSlot;
            })
            .ToList();

        int equipmentIndex = index - 1; // Adjust for "None" option
        if (equipmentIndex < 0 || equipmentIndex >= equipmentIds.Count) return;

        string selectedEquipmentId = equipmentIds[equipmentIndex];
        var newEquipment = GameServices.GameData.EquipmentDatabase.Get(selectedEquipmentId);
        if (newEquipment == null) return;

        // Calculate stat changes
        var stats = _selectedCharacter.GetComponent<StatsComponent>();
        var comparison = CompareEquipment(stats, _selectedSlot, newEquipment);

        _statsComparisonLabel.Text = comparison;
    }

    /// <summary>
    /// Handle equipment activation (equip item)
    /// </summary>
    private void OnEquipmentActivated(UIListBox sender, int index)
    {
        if (_selectedCharacter == null) return;

        if (index == 0) // "None" selected
        {
            UnequipSlot(_selectedSlot);
            return;
        }

        // Get selected equipment
        var equipmentIds = GameServices.GameData.Inventory.GetItemIds()
            .Where(id =>
            {
                var eq = GameServices.GameData.EquipmentDatabase.Get(id);
                return eq != null && eq.Slot == _selectedSlot;
            })
            .ToList();

        int equipmentIndex = index - 1;
        if (equipmentIndex < 0 || equipmentIndex >= equipmentIds.Count) return;

        string selectedEquipmentId = equipmentIds[equipmentIndex];
        EquipItem(selectedEquipmentId);
    }

    /// <summary>
    /// Equip an item to the character
    /// </summary>
    private void EquipItem(string equipmentId)
    {
        if (_selectedCharacter == null) return;

        var equipment = GameServices.GameData.EquipmentDatabase.Get(equipmentId);
        if (equipment == null) return;

        var stats = _selectedCharacter.GetComponent<StatsComponent>();
        if (stats == null) return;

        // Unequip current item in this slot (if any)
        string currentEquipmentId = stats.GetEquippedItem(equipment.Slot);
        if (currentEquipmentId != null)
        {
            UnequipItem(currentEquipmentId, equipment.Slot);
        }

        // Apply stat bonuses
        foreach (var bonus in equipment.StatBonuses)
        {
            stats.AddEquipmentBonus(bonus.Key, bonus.Value);
        }

        // Mark as equipped
        stats.SetEquippedItem(equipment.Slot, equipmentId);

        // Remove from inventory
        GameServices.GameData.Inventory.RemoveItem(equipmentId, 1);

        // Refresh displays
        RefreshEquippedDisplay();
        RefreshAvailableEquipment();

        // TODO: Publish EquipmentChangedEvent
    }

    /// <summary>
    /// Unequip an item from a slot
    /// </summary>
    private void UnequipSlot(EquipmentSlot slot)
    {
        if (_selectedCharacter == null) return;

        var stats = _selectedCharacter.GetComponent<StatsComponent>();
        if (stats == null) return;

        string equipmentId = stats.GetEquippedItem(slot);
        if (equipmentId == null) return;

        UnequipItem(equipmentId, slot);

        RefreshEquippedDisplay();
        RefreshAvailableEquipment();
    }

    /// <summary>
    /// Unequip an item and return it to inventory
    /// </summary>
    private void UnequipItem(string equipmentId, EquipmentSlot slot)
    {
        var equipment = GameServices.GameData.EquipmentDatabase.Get(equipmentId);
        if (equipment == null) return;

        var stats = _selectedCharacter.GetComponent<StatsComponent>();
        if (stats == null) return;

        // Remove stat bonuses
        foreach (var bonus in equipment.StatBonuses)
        {
            stats.RemoveEquipmentBonus(bonus.Key, bonus.Value);
        }

        // Mark as unequipped
        stats.SetEquippedItem(slot, null);

        // Add back to inventory
        GameServices.GameData.Inventory.AddItem(equipmentId, 1);
    }

    /// <summary>
    /// Compare equipment and generate stat comparison text
    /// </summary>
    private string CompareEquipment(StatsComponent stats, EquipmentSlot slot, EquipmentDefinition newEquipment)
    {
        var comparison = new List<string>();
        comparison.Add($"{newEquipment.Name}\n");

        // Get current equipment in this slot
        string currentEquipmentId = stats.GetEquippedItem(slot);
        Dictionary<StatType, int> currentBonuses = new();

        var currentEquipment = currentEquipmentId != null ? GameServices.GameData.EquipmentDatabase.Get(currentEquipmentId) : null;
        if (currentEquipment != null)
        {
            currentBonuses = currentEquipment.StatBonuses;
        }

        // Compare stats
        foreach (var newBonus in newEquipment.StatBonuses)
        {
            int currentBonus = currentBonuses.GetValueOrDefault(newBonus.Key, 0);
            int difference = newBonus.Value - currentBonus;

            string statName = newBonus.Key.ToString();
            string arrow = difference > 0 ? "↑" : difference < 0 ? "↓" : "=";
            Color color = difference > 0 ? Color.Green : difference < 0 ? Color.Red : Color.White;

            comparison.Add($"{statName}: {newBonus.Value} ({arrow}{System.Math.Abs(difference)})");
        }

        return string.Join("\n", comparison);
    }
}
