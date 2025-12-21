using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheMythalProphecy.Game.Core;
using TheMythalProphecy.Game.Data;
using TheMythalProphecy.Game.UI.Components;

namespace TheMythalProphecy.Game.States;

/// <summary>
/// Options/Settings menu - configure audio, video, and controls
/// </summary>
public class OptionsMenuState : IGameState
{
    private readonly GameStateManager _stateManager;
    private UIWindow _window;
    private UIListBox _categoryList;
    private UIPanel _settingsPanel;
    private KeyboardState _previousKeyState;

    private GameSettings _settings;

    // Audio controls
    private UILabel _masterVolumeLabel;
    private UISlider _masterVolumeSlider;
    private UILabel _musicVolumeLabel;
    private UISlider _musicVolumeSlider;
    private UILabel _sfxVolumeLabel;
    private UISlider _sfxVolumeSlider;

    // Video controls
    private UILabel _resolutionLabel;
    private UIButton _resolutionButton;
    private UILabel _fullscreenLabel;
    private UIButton _fullscreenButton;

    // Buttons
    private UIButton _applyButton;
    private UIButton _cancelButton;

    private int _selectedResolutionIndex = 0;
    private readonly (int Width, int Height)[] _resolutions = new[]
    {
        (1280, 720),
        (1920, 1080),
        (2560, 1440),
        (3840, 2160)
    };

    public OptionsMenuState(GameStateManager stateManager)
    {
        _stateManager = stateManager;
    }

    public void Enter()
    {
        // Load current settings
        _settings = GameSettings.Load();

        // Create window (800x600)
        int screenWidth = GameServices.GraphicsDevice.Viewport.Width;
        int screenHeight = GameServices.GraphicsDevice.Viewport.Height;

        Vector2 windowSize = new Vector2(800, 600);
        Vector2 windowPos = new Vector2(
            (screenWidth - windowSize.X) / 2,
            (screenHeight - windowSize.Y) / 2
        );

        _window = new UIWindow(windowPos, windowSize, "Options")
        {
            IsModal = true,
            ShowCloseButton = false
        };

        // Left: Category list
        _categoryList = new UIListBox(new Vector2(10, 10), new Vector2(150, 480))
        {
            ItemHeight = 40
        };
        _categoryList.AddItem("Audio");
        _categoryList.AddItem("Video");
        _categoryList.SelectedIndex = 0;
        _categoryList.OnSelectionChanged += OnCategoryChanged;
        _window.ContentPanel.AddChild(_categoryList);

        // Right: Settings panel
        _settingsPanel = new UIPanel(new Vector2(170, 10), new Vector2(610, 480))
        {
            BackgroundColor = new Color(30, 30, 50, 200)
        };
        _window.ContentPanel.AddChild(_settingsPanel);

        // Bottom: Apply/Cancel buttons
        _applyButton = new UIButton("Apply", new Vector2(10, 500), new Vector2(150, 40));
        _applyButton.OnClick += OnApplyClicked;
        _window.ContentPanel.AddChild(_applyButton);

        _cancelButton = new UIButton("Cancel", new Vector2(170, 500), new Vector2(150, 40));
        _cancelButton.OnClick += OnCancelClicked;
        _window.ContentPanel.AddChild(_cancelButton);

        // Create audio controls
        CreateAudioControls();

        // Create video controls (hidden initially)
        CreateVideoControls();

        // Register window
        GameServices.UI.AddElement(_window);

        // Give focus to category list
        _categoryList.IsFocused = true;

        // Show audio settings by default
        ShowAudioSettings();
    }

    public void Exit()
    {
        GameServices.UI.RemoveElement(_window);
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState keyState = Keyboard.GetState();

        // Escape to cancel
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
    /// Create audio control widgets
    /// </summary>
    private void CreateAudioControls()
    {
        // Master Volume
        _masterVolumeLabel = new UILabel($"Master Volume: {(int)(_settings.MasterVolume * 100)}%", new Vector2(10, 10));
        _masterVolumeLabel.TextColor = Color.White;
        _settingsPanel.AddChild(_masterVolumeLabel);

        _masterVolumeSlider = new UISlider(new Vector2(10, 40), new Vector2(580, 20), 0, 1, _settings.MasterVolume);
        _masterVolumeSlider.OnValueChanged += (slider, value) =>
        {
            _settings.MasterVolume = value;
            _masterVolumeLabel.Text = $"Master Volume: {(int)(value * 100)}%";
        };
        _settingsPanel.AddChild(_masterVolumeSlider);

        // Music Volume
        _musicVolumeLabel = new UILabel($"Music Volume: {(int)(_settings.MusicVolume * 100)}%", new Vector2(10, 80));
        _musicVolumeLabel.TextColor = Color.White;
        _settingsPanel.AddChild(_musicVolumeLabel);

        _musicVolumeSlider = new UISlider(new Vector2(10, 110), new Vector2(580, 20), 0, 1, _settings.MusicVolume);
        _musicVolumeSlider.OnValueChanged += (slider, value) =>
        {
            _settings.MusicVolume = value;
            _musicVolumeLabel.Text = $"Music Volume: {(int)(value * 100)}%";
        };
        _settingsPanel.AddChild(_musicVolumeSlider);

        // SFX Volume
        _sfxVolumeLabel = new UILabel($"SFX Volume: {(int)(_settings.SFXVolume * 100)}%", new Vector2(10, 150));
        _sfxVolumeLabel.TextColor = Color.White;
        _settingsPanel.AddChild(_sfxVolumeLabel);

        _sfxVolumeSlider = new UISlider(new Vector2(10, 180), new Vector2(580, 20), 0, 1, _settings.SFXVolume);
        _sfxVolumeSlider.OnValueChanged += (slider, value) =>
        {
            _settings.SFXVolume = value;
            _sfxVolumeLabel.Text = $"SFX Volume: {(int)(value * 100)}%";
        };
        _settingsPanel.AddChild(_sfxVolumeSlider);
    }

    /// <summary>
    /// Create video control widgets
    /// </summary>
    private void CreateVideoControls()
    {
        // Resolution
        _resolutionLabel = new UILabel("Resolution:", new Vector2(10, 10));
        _resolutionLabel.TextColor = Color.White;
        _settingsPanel.AddChild(_resolutionLabel);

        // Find current resolution index
        for (int i = 0; i < _resolutions.Length; i++)
        {
            if (_resolutions[i].Width == _settings.ResolutionWidth &&
                _resolutions[i].Height == _settings.ResolutionHeight)
            {
                _selectedResolutionIndex = i;
                break;
            }
        }

        var currentRes = _resolutions[_selectedResolutionIndex];
        _resolutionButton = new UIButton($"{currentRes.Width} x {currentRes.Height}", new Vector2(10, 40), new Vector2(300, 40));
        _resolutionButton.OnClick += OnResolutionClicked;
        _settingsPanel.AddChild(_resolutionButton);

        // Fullscreen
        _fullscreenLabel = new UILabel("Fullscreen:", new Vector2(10, 100));
        _fullscreenLabel.TextColor = Color.White;
        _settingsPanel.AddChild(_fullscreenLabel);

        _fullscreenButton = new UIButton(_settings.Fullscreen ? "On" : "Off", new Vector2(10, 130), new Vector2(150, 40));
        _fullscreenButton.OnClick += OnFullscreenClicked;
        _settingsPanel.AddChild(_fullscreenButton);
    }

    /// <summary>
    /// Show audio settings
    /// </summary>
    private void ShowAudioSettings()
    {
        // Hide video controls
        _resolutionLabel.Visible = false;
        _resolutionButton.Visible = false;
        _fullscreenLabel.Visible = false;
        _fullscreenButton.Visible = false;

        // Show audio controls
        _masterVolumeLabel.Visible = true;
        _masterVolumeSlider.Visible = true;
        _musicVolumeLabel.Visible = true;
        _musicVolumeSlider.Visible = true;
        _sfxVolumeLabel.Visible = true;
        _sfxVolumeSlider.Visible = true;
    }

    /// <summary>
    /// Show video settings
    /// </summary>
    private void ShowVideoSettings()
    {
        // Hide audio controls
        _masterVolumeLabel.Visible = false;
        _masterVolumeSlider.Visible = false;
        _musicVolumeLabel.Visible = false;
        _musicVolumeSlider.Visible = false;
        _sfxVolumeLabel.Visible = false;
        _sfxVolumeSlider.Visible = false;

        // Show video controls
        _resolutionLabel.Visible = true;
        _resolutionButton.Visible = true;
        _fullscreenLabel.Visible = true;
        _fullscreenButton.Visible = true;
    }

    /// <summary>
    /// Handle category change
    /// </summary>
    private void OnCategoryChanged(UIListBox sender, int index)
    {
        switch (index)
        {
            case 0: // Audio
                ShowAudioSettings();
                break;
            case 1: // Video
                ShowVideoSettings();
                break;
        }
    }

    /// <summary>
    /// Cycle through resolutions
    /// </summary>
    private void OnResolutionClicked(UIButton sender)
    {
        _selectedResolutionIndex = (_selectedResolutionIndex + 1) % _resolutions.Length;
        var res = _resolutions[_selectedResolutionIndex];
        _resolutionButton.Text = $"{res.Width} x {res.Height}";
        _settings.ResolutionWidth = res.Width;
        _settings.ResolutionHeight = res.Height;
    }

    /// <summary>
    /// Toggle fullscreen
    /// </summary>
    private void OnFullscreenClicked(UIButton sender)
    {
        _settings.Fullscreen = !_settings.Fullscreen;
        _fullscreenButton.Text = _settings.Fullscreen ? "On" : "Off";
    }

    /// <summary>
    /// Apply settings and close
    /// </summary>
    private void OnApplyClicked(UIButton sender)
    {
        // Apply audio settings
        _settings.ApplyAudioSettings();

        // Save settings
        _settings.Save();

        // Close menu
        _stateManager.PopState();
    }

    /// <summary>
    /// Cancel and close without saving
    /// </summary>
    private void OnCancelClicked(UIButton sender)
    {
        _stateManager.PopState();
    }
}
