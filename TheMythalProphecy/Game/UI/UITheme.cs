using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheMythalProphecy.Game.UI;

/// <summary>
/// Centralized UI styling and theming
/// </summary>
public class UITheme
{
    // Colors
    public Color PrimaryColor { get; set; } = new Color(40, 40, 60);
    public Color SecondaryColor { get; set; } = new Color(60, 60, 80);
    public Color AccentColor { get; set; } = new Color(100, 150, 200);
    public Color TextColor { get; set; } = Color.White;
    public Color DisabledTextColor { get; set; } = new Color(128, 128, 128);
    public Color BorderColor { get; set; } = new Color(200, 200, 220);
    public Color HighlightColor { get; set; } = new Color(255, 255, 100);
    public Color HoverColor { get; set; } = new Color(80, 120, 160);
    public Color PressedColor { get; set; } = new Color(60, 100, 140);

    // HP/MP Bar Colors
    public Color HPColor { get; set; } = new Color(100, 220, 100);
    public Color HPLowColor { get; set; } = new Color(220, 60, 60);
    public Color MPColor { get; set; } = new Color(100, 150, 255);
    public Color BarBackgroundColor { get; set; } = new Color(40, 40, 40);

    // Fonts
    public SpriteFont DefaultFont { get; set; }
    public SpriteFont TitleFont { get; set; }
    public SpriteFont SmallFont { get; set; }

    // Spacing
    public int PaddingSmall { get; set; } = 4;
    public int PaddingMedium { get; set; } = 8;
    public int PaddingLarge { get; set; } = 16;
    public int BorderThickness { get; set; } = 2;

    // Button sizing
    public int ButtonHeight { get; set; } = 40;
    public int ButtonMinWidth { get; set; } = 120;

    // Animation
    public float TransitionDuration { get; set; } = 0.2f; // seconds

    // Textures (optional - can use solid colors if textures not available)
    public Texture2D PanelTexture { get; set; }
    public Texture2D ButtonNormalTexture { get; set; }
    public Texture2D ButtonHoverTexture { get; set; }
    public Texture2D ButtonPressedTexture { get; set; }
    public Texture2D CursorTexture { get; set; }

    /// <summary>
    /// Creates a default theme
    /// </summary>
    public UITheme()
    {
    }

    /// <summary>
    /// Initialize theme with required font
    /// </summary>
    public void Initialize(SpriteFont defaultFont)
    {
        if (defaultFont == null)
        {
            throw new ArgumentNullException(nameof(defaultFont), "UITheme requires a non-null default font");
        }

        DefaultFont = defaultFont;
        TitleFont = defaultFont; // Can be replaced with larger font later
        SmallFont = defaultFont; // Can be replaced with smaller font later

        Console.WriteLine($"[UITheme] Initialized with font: {DefaultFont != null}");
    }
}
