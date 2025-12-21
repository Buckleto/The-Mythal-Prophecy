using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheMythalProphecy.Game.UI.Components;

/// <summary>
/// Text alignment options
/// </summary>
public enum TextAlignment
{
    Left,
    Center,
    Right
}

/// <summary>
/// Label for displaying text
/// </summary>
public class UILabel : UIElement
{
    private string _text = string.Empty;
    private Vector2 _textSize;

    public string Text
    {
        get => _text;
        set
        {
            if (_text != value)
            {
                _text = value;
                _textSize = Vector2.Zero; // Recalculate on next draw
            }
        }
    }

    public Color TextColor { get; set; } = Color.White;
    public SpriteFont Font { get; set; }
    public TextAlignment Alignment { get; set; } = TextAlignment.Left;
    public bool WordWrap { get; set; } = false;
    public float Scale { get; set; } = 1.0f;

    public UILabel()
    {
    }

    public UILabel(string text, Vector2 position)
    {
        Text = text;
        Position = position;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, UITheme theme)
    {
        if (string.IsNullOrEmpty(Text)) return;

        SpriteFont font = Font ?? theme.DefaultFont;
        if (font == null) return;

        // Measure text if needed
        if (_textSize == Vector2.Zero)
        {
            _textSize = font.MeasureString(Text) * Scale;

            // Auto-size if size not set
            if (Size == Vector2.Zero)
            {
                Size = _textSize;
            }
        }

        Rectangle bounds = Bounds;
        Vector2 textPosition = new Vector2(bounds.X, bounds.Y);

        // Apply alignment
        switch (Alignment)
        {
            case TextAlignment.Center:
                textPosition.X = bounds.X + (bounds.Width - _textSize.X) / 2;
                textPosition.Y = bounds.Y + (bounds.Height - _textSize.Y) / 2;
                break;

            case TextAlignment.Right:
                textPosition.X = bounds.Right - _textSize.X;
                textPosition.Y = bounds.Y + (bounds.Height - _textSize.Y) / 2;
                break;

            case TextAlignment.Left:
            default:
                textPosition.Y = bounds.Y + (bounds.Height - _textSize.Y) / 2;
                break;
        }

        // Draw text
        Color finalColor = Enabled ? TextColor : theme.DisabledTextColor;
        finalColor *= Alpha;

        if (WordWrap && Size.X > 0)
        {
            DrawWrappedText(spriteBatch, font, textPosition, bounds.Width, finalColor);
        }
        else
        {
            spriteBatch.DrawString(font, Text, textPosition, finalColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }
    }

    /// <summary>
    /// Draw text with word wrapping
    /// </summary>
    private void DrawWrappedText(SpriteBatch spriteBatch, SpriteFont font, Vector2 position, float maxWidth, Color color)
    {
        string[] words = Text.Split(' ');
        float x = position.X;
        float y = position.Y;
        float spaceWidth = font.MeasureString(" ").X * Scale;
        float lineHeight = font.LineSpacing * Scale;

        string currentLine = string.Empty;

        foreach (string word in words)
        {
            Vector2 size = font.MeasureString(currentLine + word) * Scale;

            if (size.X > maxWidth && !string.IsNullOrEmpty(currentLine))
            {
                // Draw current line
                spriteBatch.DrawString(font, currentLine, new Vector2(x, y), color, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

                y += lineHeight;
                currentLine = string.Empty;
            }

            currentLine += word + " ";
        }

        // Draw remaining text
        if (!string.IsNullOrEmpty(currentLine))
        {
            spriteBatch.DrawString(font, currentLine, new Vector2(x, y), color, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }
    }

    /// <summary>
    /// Convenience method to create a centered label
    /// </summary>
    public static UILabel CreateCentered(string text, Vector2 position, Vector2 size)
    {
        return new UILabel(text, position)
        {
            Size = size,
            Alignment = TextAlignment.Center
        };
    }
}
