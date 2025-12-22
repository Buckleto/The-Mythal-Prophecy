using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI.Gleam;

/// <summary>
/// Interface for GleamUI themes. Allows GleamRenderer to work with different theme types.
/// </summary>
public interface IGleamTheme
{
    // Core colors
    Color DeepPurple { get; }
    Color MidPurple { get; }
    Color DarkPurple { get; }
    Color MutedPurple { get; }
    Color Gold { get; }
    Color GoldBright { get; }
    Color GoldDim { get; }
    Color TextPrimary { get; }
    Color TextSecondary { get; }
    Color TextDisabled { get; }

    // Animation timing
    float HoverTransitionDuration { get; }

    // Sizing
    int BorderThickness { get; }
    int PaddingSmall { get; }
    int PaddingMedium { get; }
    int PaddingLarge { get; }

    // Fonts
    SpriteFont DefaultFont { get; }
    SpriteFont MenuFont { get; }
    bool IsInitialized { get; }
}

/// <summary>
/// Cosmic/mystical theme for GleamUI components.
/// Color palette derived from title screen aesthetic.
/// </summary>
public class GleamTheme : IGleamTheme
{
    // Core cosmic palette
    public Color DeepPurple { get; } = new Color(15, 4, 31);
    public Color MidPurple { get; } = new Color(31, 8, 56);
    public Color DarkPurple { get; } = new Color(8, 3, 15);
    public Color MutedPurple { get; } = new Color(20, 10, 30);

    // Gold accents
    public Color Gold { get; } = new Color(179, 128, 51);
    public Color GoldBright { get; } = new Color(220, 180, 80);
    public Color GoldDim { get; } = new Color(120, 85, 35);

    // Text colors
    public Color TextPrimary { get; } = Color.White;
    public Color TextSecondary { get; } = new Color(200, 200, 220);
    public Color TextDisabled { get; } = new Color(100, 100, 120);

    // Animation timings (in seconds)
    public float HoverTransitionDuration { get; } = 0.15f;
    public float FocusTransitionDuration { get; } = 0.2f;
    public float ShimmerDuration { get; } = 2.0f;

    // Sizing
    public float ButtonSkewFactor { get; } = 0.3f;
    public int BorderThickness { get; } = 2;
    public int PaddingSmall { get; } = 4;
    public int PaddingMedium { get; } = 8;
    public int PaddingLarge { get; } = 16;

    // Fonts
    public SpriteFont DefaultFont { get; private set; }
    public SpriteFont MenuFont { get; private set; }

    public bool IsInitialized => DefaultFont != null;

    public void Initialize(SpriteFont defaultFont, SpriteFont menuFont = null)
    {
        DefaultFont = defaultFont;
        MenuFont = menuFont ?? defaultFont;
    }
}
