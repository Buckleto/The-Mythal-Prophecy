using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI.Gleam;

/// <summary>
/// Left/right selector for cycling through options like resolution.
/// </summary>
public class GleamSelector : GleamElement
{
    private readonly List<string> _options = new();
    private int _selectedIndex;

    private Rectangle _leftArrowBounds;
    private Rectangle _rightArrowBounds;
    private bool _leftHovered;
    private bool _rightHovered;

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            int newIndex = _options.Count > 0 ? Math.Clamp(value, 0, _options.Count - 1) : 0;
            if (_selectedIndex != newIndex)
            {
                _selectedIndex = newIndex;
                OnSelectionChanged?.Invoke(this, _selectedIndex);
            }
        }
    }

    public string SelectedOption => _options.Count > 0 ? _options[_selectedIndex] : "";

    public SpriteFont Font { get; set; }
    public int ArrowWidth { get; set; } = 30;

    public event Action<GleamSelector, int> OnSelectionChanged;

    public GleamSelector(Vector2 position, Vector2 size)
    {
        Position = position;
        Size = size;
    }

    public void SetOptions(IEnumerable<string> options, int selectedIndex = 0)
    {
        _options.Clear();
        _options.AddRange(options);
        _selectedIndex = _options.Count > 0 ? Math.Clamp(selectedIndex, 0, _options.Count - 1) : 0;
    }

    public void AddOption(string option)
    {
        _options.Add(option);
    }

    public override bool HandleInput(Vector2 mousePosition, bool mouseDown, bool mouseClicked)
    {
        if (!Enabled || !Visible) return false;

        Rectangle bounds = Bounds;
        _leftArrowBounds = new Rectangle(bounds.X, bounds.Y, ArrowWidth, bounds.Height);
        _rightArrowBounds = new Rectangle(bounds.Right - ArrowWidth, bounds.Y, ArrowWidth, bounds.Height);

        _leftHovered = _leftArrowBounds.Contains(mousePosition);
        _rightHovered = _rightArrowBounds.Contains(mousePosition);
        IsHovered = bounds.Contains(mousePosition);

        if (mouseClicked && _options.Count > 0)
        {
            if (_leftHovered && _selectedIndex > 0)
            {
                SelectedIndex--;
                return true;
            }
            if (_rightHovered && _selectedIndex < _options.Count - 1)
            {
                SelectedIndex++;
                return true;
            }
        }

        return false;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, GleamRenderer renderer)
    {
        var theme = renderer.Theme;
        Rectangle bounds = Bounds;

        // Calculate regions
        _leftArrowBounds = new Rectangle(bounds.X, bounds.Y, ArrowWidth, bounds.Height);
        _rightArrowBounds = new Rectangle(bounds.Right - ArrowWidth, bounds.Y, ArrowWidth, bounds.Height);
        Rectangle valueBounds = new Rectangle(bounds.X + ArrowWidth, bounds.Y, bounds.Width - ArrowWidth * 2, bounds.Height);

        // Background
        renderer.DrawRect(spriteBatch, bounds, theme.DeepPurple, Alpha);
        renderer.DrawRectBorder(spriteBatch, bounds, theme.Gold, 2, Alpha);

        // Left arrow
        bool canGoLeft = _selectedIndex > 0;
        Color leftColor = canGoLeft ? (_leftHovered ? theme.GoldBright : theme.Gold) : theme.GoldDim;
        DrawArrow(spriteBatch, renderer, _leftArrowBounds, leftColor, true);

        // Right arrow
        bool canGoRight = _selectedIndex < _options.Count - 1;
        Color rightColor = canGoRight ? (_rightHovered ? theme.GoldBright : theme.Gold) : theme.GoldDim;
        DrawArrow(spriteBatch, renderer, _rightArrowBounds, rightColor, false);

        // Current value text
        var font = Font ?? theme.DefaultFont;
        if (font != null && _options.Count > 0)
        {
            renderer.DrawTextCentered(spriteBatch, font, SelectedOption, valueBounds, theme.TextPrimary, true, Alpha);
        }
    }

    private void DrawArrow(SpriteBatch spriteBatch, GleamRenderer renderer, Rectangle bounds, Color color, bool pointLeft)
    {
        // Draw simple < or > arrow using lines
        int padding = 8;
        int centerY = bounds.Y + bounds.Height / 2;
        int arrowHeight = bounds.Height / 3;

        Vector2 tip, top, bottom;

        if (pointLeft)
        {
            tip = new Vector2(bounds.X + padding, centerY);
            top = new Vector2(bounds.Right - padding, centerY - arrowHeight);
            bottom = new Vector2(bounds.Right - padding, centerY + arrowHeight);
        }
        else
        {
            tip = new Vector2(bounds.Right - padding, centerY);
            top = new Vector2(bounds.X + padding, centerY - arrowHeight);
            bottom = new Vector2(bounds.X + padding, centerY + arrowHeight);
        }

        renderer.DrawLine(spriteBatch, top, tip, 2, color);
        renderer.DrawLine(spriteBatch, bottom, tip, 2, color);
    }
}
