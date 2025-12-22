using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI.Gleam;

/// <summary>
/// Multi-line scrollable text display with word wrapping.
/// </summary>
public class GleamTextBox : GleamElement
{
    private string _text = string.Empty;
    private readonly List<string> _wrappedLines = new();
    private int _scrollOffset;
    private int _maxVisibleLines;
    private bool _needsReflow = true;
    private float _lastWidth;

    public string Text
    {
        get => _text;
        set
        {
            if (_text != value)
            {
                _text = value ?? string.Empty;
                _needsReflow = true;
            }
        }
    }

    public Color? TextColor { get; set; }
    public float LineSpacing { get; set; } = 1.2f;
    public bool AutoScroll { get; set; }
    public int Padding { get; set; } = 8;
    public SpriteFont Font { get; set; }

    public int ScrollOffset
    {
        get => _scrollOffset;
        set => _scrollOffset = Math.Clamp(value, 0, MaxScrollOffset);
    }

    public int MaxScrollOffset => Math.Max(0, _wrappedLines.Count - _maxVisibleLines);
    public bool CanScroll => _wrappedLines.Count > _maxVisibleLines;

    public GleamTextBox(Vector2 position, Vector2 size)
    {
        Position = position;
        Size = size;
    }

    public override void Update(GameTime gameTime, GleamRenderer renderer)
    {
        base.Update(gameTime, renderer);

        // Reflow if size changed
        if (Math.Abs(_lastWidth - Size.X) > 1f)
        {
            _needsReflow = true;
            _lastWidth = Size.X;
        }

        if (_needsReflow)
        {
            ReflowText(renderer);
            _needsReflow = false;

            if (AutoScroll)
            {
                ScrollOffset = MaxScrollOffset;
            }
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, GleamRenderer renderer)
    {
        var theme = renderer.Theme;
        Rectangle bounds = Bounds;

        // Background
        renderer.DrawRect(spriteBatch, bounds, theme.DeepPurple, Alpha * 0.9f);

        // Border
        renderer.DrawRectBorder(spriteBatch, bounds, theme.Gold, 2, Alpha);

        // Text
        var font = Font ?? theme.DefaultFont;
        if (font == null || _wrappedLines.Count == 0) return;

        float lineHeight = font.LineSpacing * LineSpacing;
        Vector2 textPos = new Vector2(bounds.X + Padding, bounds.Y + Padding);

        int startLine = _scrollOffset;
        int endLine = Math.Min(_scrollOffset + _maxVisibleLines, _wrappedLines.Count);

        Color textColor = TextColor ?? theme.TextPrimary;

        for (int i = startLine; i < endLine; i++)
        {
            renderer.DrawText(spriteBatch, font, _wrappedLines[i], textPos, textColor, true, Alpha);
            textPos.Y += lineHeight;
        }

        // Scrollbar
        if (CanScroll)
        {
            DrawScrollbar(spriteBatch, renderer, bounds);
        }
    }

    private void DrawScrollbar(SpriteBatch spriteBatch, GleamRenderer renderer, Rectangle bounds)
    {
        var theme = renderer.Theme;
        int scrollbarWidth = 6;
        int scrollbarX = bounds.Right - scrollbarWidth - Padding;
        int scrollbarY = bounds.Y + Padding;
        int scrollbarHeight = bounds.Height - Padding * 2;

        // Track
        Rectangle track = new Rectangle(scrollbarX, scrollbarY, scrollbarWidth, scrollbarHeight);
        renderer.DrawRect(spriteBatch, track, theme.DarkPurple, Alpha);

        // Thumb
        float thumbHeight = Math.Max(20f, scrollbarHeight * ((float)_maxVisibleLines / _wrappedLines.Count));
        float thumbY = scrollbarY + (scrollbarHeight - thumbHeight) * (_scrollOffset / (float)Math.Max(1, MaxScrollOffset));

        Rectangle thumb = new Rectangle(scrollbarX, (int)thumbY, scrollbarWidth, (int)thumbHeight);
        renderer.DrawRect(spriteBatch, thumb, theme.Gold, Alpha);
    }

    private void ReflowText(GleamRenderer renderer)
    {
        _wrappedLines.Clear();

        var font = Font ?? renderer.Theme.DefaultFont;
        if (font == null || string.IsNullOrEmpty(_text))
        {
            _maxVisibleLines = 0;
            return;
        }

        float availableWidth = Size.X - Padding * 2;
        if (CanScroll) availableWidth -= 6 + Padding; // Account for scrollbar

        // Split by newlines first
        string[] paragraphs = _text.Split('\n');
        foreach (string paragraph in paragraphs)
        {
            if (string.IsNullOrEmpty(paragraph))
            {
                _wrappedLines.Add(string.Empty);
                continue;
            }

            WrapLine(paragraph, font, availableWidth);
        }

        // Calculate visible lines
        float lineHeight = font.LineSpacing * LineSpacing;
        float availableHeight = Size.Y - Padding * 2;
        _maxVisibleLines = Math.Max(1, (int)(availableHeight / lineHeight));
    }

    private void WrapLine(string line, SpriteFont font, float maxWidth)
    {
        if (font.MeasureString(line).X <= maxWidth)
        {
            _wrappedLines.Add(line);
            return;
        }

        string[] words = line.Split(' ');
        string currentLine = string.Empty;

        foreach (string word in words)
        {
            string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;

            if (font.MeasureString(testLine).X > maxWidth && !string.IsNullOrEmpty(currentLine))
            {
                _wrappedLines.Add(currentLine);
                currentLine = word;
            }
            else
            {
                currentLine = testLine;
            }
        }

        if (!string.IsNullOrEmpty(currentLine))
        {
            _wrappedLines.Add(currentLine);
        }
    }

    public override bool HandleInput(Vector2 mousePosition, bool mouseDown, bool mouseClicked)
    {
        // Text boxes don't handle input (read-only display)
        return false;
    }

    public void ScrollUp(int lines = 1) => ScrollOffset -= lines;
    public void ScrollDown(int lines = 1) => ScrollOffset += lines;
    public void ScrollToTop() => ScrollOffset = 0;
    public void ScrollToBottom() => ScrollOffset = MaxScrollOffset;

    public void AppendText(string text)
    {
        Text += text;
    }

    public void Clear()
    {
        Text = string.Empty;
        ScrollOffset = 0;
    }
}
