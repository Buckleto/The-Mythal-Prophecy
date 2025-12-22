using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI.Gleam;

/// <summary>
/// Text label with cosmic styling.
/// </summary>
public class GleamLabel : GleamElement
{
    public string Text { get; set; }
    public SpriteFont Font { get; set; }
    public Color? TextColor { get; set; }
    public bool ShowShadow { get; set; } = true;
    public TextAlignment Alignment { get; set; } = TextAlignment.Left;

    public GleamLabel(string text, Vector2 position)
    {
        Text = text;
        Position = position;
        Size = Vector2.Zero; // Auto-size from text
    }

    public GleamLabel(string text, Vector2 position, Vector2 size)
    {
        Text = text;
        Position = position;
        Size = size;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, GleamRenderer renderer)
    {
        if (string.IsNullOrEmpty(Text)) return;

        var theme = renderer.Theme;
        var font = Font ?? theme.DefaultFont;
        if (font == null) return;

        Color color = TextColor ?? theme.TextPrimary;
        Vector2 textSize = font.MeasureString(Text);
        Rectangle bounds = Bounds;

        // If size is zero, auto-size
        if (Size == Vector2.Zero)
        {
            bounds = new Rectangle((int)AbsolutePosition.X, (int)AbsolutePosition.Y, (int)textSize.X, (int)textSize.Y);
        }

        Vector2 position;
        switch (Alignment)
        {
            case TextAlignment.Center:
                position = new Vector2(
                    bounds.X + (bounds.Width - textSize.X) / 2f,
                    bounds.Y + (bounds.Height - textSize.Y) / 2f
                );
                break;
            case TextAlignment.Right:
                position = new Vector2(
                    bounds.Right - textSize.X,
                    bounds.Y + (bounds.Height - textSize.Y) / 2f
                );
                break;
            default: // Left
                position = new Vector2(
                    bounds.X,
                    bounds.Y + (bounds.Height - textSize.Y) / 2f
                );
                break;
        }

        renderer.DrawText(spriteBatch, font, Text, position, color, ShowShadow, Alpha);
    }

    public override bool HandleInput(Vector2 mousePosition, bool mouseDown, bool mouseClicked)
    {
        // Labels don't handle input
        return false;
    }
}

public enum TextAlignment
{
    Left,
    Center,
    Right
}
