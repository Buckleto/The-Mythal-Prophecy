using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI.Gleam;

/// <summary>
/// Progress bar for displaying values like HP/MP with cosmic styling.
/// Supports smooth value transitions and low-value color change.
/// </summary>
public class GleamProgressBar : GleamElement
{
    private float _currentValue;
    private float _maxValue;
    private float _displayValue;

    public float CurrentValue
    {
        get => _currentValue;
        set => _currentValue = MathHelper.Clamp(value, 0, _maxValue);
    }

    public float MaxValue
    {
        get => _maxValue;
        set
        {
            _maxValue = MathHelper.Max(value, 0);
            _currentValue = MathHelper.Clamp(_currentValue, 0, _maxValue);
        }
    }

    public float Percentage => _maxValue > 0 ? _currentValue / _maxValue : 0;

    public Color FillColor { get; set; } = new Color(100, 220, 100);
    public Color LowFillColor { get; set; } = new Color(220, 60, 60);
    public float LowThreshold { get; set; } = 0.25f;
    public float TransitionSpeed { get; set; } = 5f;
    public bool ShowText { get; set; } = true;
    public string TextFormat { get; set; } = "{0}/{1}";
    public SpriteFont Font { get; set; }

    public GleamProgressBar(Vector2 position, Vector2 size, float maxValue = 100f)
    {
        Position = position;
        Size = size;
        _maxValue = maxValue;
        _currentValue = maxValue;
        _displayValue = maxValue;
    }

    public override void Update(GameTime gameTime, GleamRenderer renderer)
    {
        base.Update(gameTime, renderer);

        // Smooth transition
        if (Math.Abs(_displayValue - _currentValue) > 0.1f)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float difference = _currentValue - _displayValue;
            _displayValue += difference * TransitionSpeed * delta;
        }
        else
        {
            _displayValue = _currentValue;
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, GleamRenderer renderer)
    {
        var theme = renderer.Theme;
        Rectangle bounds = Bounds;

        // Background
        renderer.DrawRect(spriteBatch, bounds, theme.DarkPurple, Alpha);

        // Fill
        float fillPercentage = _maxValue > 0 ? MathHelper.Clamp(_displayValue / _maxValue, 0, 1) : 0;
        int fillWidth = (int)(bounds.Width * fillPercentage);

        if (fillWidth > 0)
        {
            Rectangle fillBounds = new Rectangle(bounds.X, bounds.Y, fillWidth, bounds.Height);
            Color fillColor = fillPercentage <= LowThreshold ? LowFillColor : FillColor;
            renderer.DrawRect(spriteBatch, fillBounds, fillColor, Alpha);
        }

        // Border
        renderer.DrawRectBorder(spriteBatch, bounds, theme.Gold, 2, Alpha);

        // Text
        if (ShowText)
        {
            var font = Font ?? theme.DefaultFont;
            if (font != null)
            {
                string text = string.Format(TextFormat, (int)_currentValue, (int)_maxValue);
                renderer.DrawTextCentered(spriteBatch, font, text, bounds, theme.TextPrimary, true, Alpha);
            }
        }
    }

    public override bool HandleInput(Vector2 mousePosition, bool mouseDown, bool mouseClicked)
    {
        // Progress bars don't handle input
        return false;
    }

    public void SetPercentage(float percentage)
    {
        CurrentValue = _maxValue * MathHelper.Clamp(percentage, 0, 1);
    }
}
