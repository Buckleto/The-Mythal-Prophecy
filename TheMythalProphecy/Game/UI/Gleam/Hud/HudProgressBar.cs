using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI.Gleam;

public enum HudBarType { HP, MP }

/// <summary>
/// Progress bar for HUD with HP/MP coloring from HudTheme.
/// HP bars use 3-tier coloring (full/medium/low).
/// </summary>
public class HudProgressBar : GleamElement
{
    private float _currentValue;
    private float _maxValue;
    private float _displayValue;
    private readonly HudTheme _hudTheme;
    private readonly HudBarType _barType;

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
            _maxValue = MathHelper.Max(value, 1);
            _currentValue = MathHelper.Clamp(_currentValue, 0, _maxValue);
        }
    }

    public float Percentage => _maxValue > 0 ? _currentValue / _maxValue : 0;
    public float TransitionSpeed { get; set; } = 8f;
    public bool ShowText { get; set; } = true;

    public HudProgressBar(Vector2 position, Vector2 size, HudTheme hudTheme, HudBarType barType)
    {
        Position = position;
        Size = size;
        _hudTheme = hudTheme;
        _barType = barType;
        _maxValue = 100;
        _currentValue = 100;
        _displayValue = 100;
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
        Rectangle bounds = Bounds;
        float displayPercentage = _maxValue > 0 ? MathHelper.Clamp(_displayValue / _maxValue, 0, 1) : 0;

        // Background color based on bar type
        Color bgColor = _barType == HudBarType.HP ? _hudTheme.HpBackground : _hudTheme.MpBackground;
        renderer.DrawRect(spriteBatch, bounds, bgColor, Alpha);

        // Fill
        int fillWidth = (int)(bounds.Width * displayPercentage);
        if (fillWidth > 0)
        {
            Rectangle fillBounds = new Rectangle(bounds.X, bounds.Y, fillWidth, bounds.Height);
            Color fillColor = _barType == HudBarType.HP
                ? _hudTheme.GetHpColor(displayPercentage)
                : _hudTheme.GetMpColor(displayPercentage);
            renderer.DrawRect(spriteBatch, fillBounds, fillColor, Alpha);
        }

        // Border
        renderer.DrawRectBorder(spriteBatch, bounds, _hudTheme.Gold, 1, Alpha * 0.6f);

        // Text
        if (ShowText && _hudTheme.HudFont != null)
        {
            string text = $"{(int)_currentValue}/{(int)_maxValue}";
            renderer.DrawTextCentered(spriteBatch, _hudTheme.HudFont, text, bounds, _hudTheme.TextPrimary, true, Alpha);
        }
    }

    public override bool HandleInput(Vector2 mousePosition, bool mouseDown, bool mouseClicked)
    {
        return false; // Progress bars don't handle input
    }
}
