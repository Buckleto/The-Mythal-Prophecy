using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheMythalProphecy.Game.UI.Gleam;

/// <summary>
/// Horizontal slider with cosmic styling for volume and other value selection.
/// </summary>
public class GleamSlider : GleamElement
{
    private float _value;
    private float _minValue;
    private float _maxValue;
    private bool _isDragging;
    private MouseState _previousMouseState;

    public float Value
    {
        get => _value;
        set
        {
            float newValue = MathHelper.Clamp(value, _minValue, _maxValue);
            if (Math.Abs(_value - newValue) > 0.0001f)
            {
                _value = newValue;
                OnValueChanged?.Invoke(this, _value);
            }
        }
    }

    public float MinValue
    {
        get => _minValue;
        set
        {
            _minValue = value;
            Value = _value; // Re-clamp
        }
    }

    public float MaxValue
    {
        get => _maxValue;
        set
        {
            _maxValue = value;
            Value = _value; // Re-clamp
        }
    }

    public float Percentage => _maxValue > _minValue ? (_value - _minValue) / (_maxValue - _minValue) : 0;

    public int ThumbWidth { get; set; } = 12;
    public int TrackHeight { get; set; } = 8;

    public event Action<GleamSlider, float> OnValueChanged;

    public GleamSlider(Vector2 position, Vector2 size, float minValue = 0, float maxValue = 1, float initialValue = 0.5f)
    {
        Position = position;
        Size = size;
        _minValue = minValue;
        _maxValue = maxValue;
        _value = MathHelper.Clamp(initialValue, minValue, maxValue);
    }

    public override void Update(GameTime gameTime, GleamRenderer renderer)
    {
        base.Update(gameTime, renderer);

        if (!Enabled) return;

        MouseState mouseState = Mouse.GetState();
        Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);

        bool wasPressed = _previousMouseState.LeftButton == ButtonState.Pressed;
        bool isPressed = mouseState.LeftButton == ButtonState.Pressed;
        bool justPressed = isPressed && !wasPressed;

        // Start dragging if clicked on slider
        if (justPressed && Bounds.Contains(mousePos))
        {
            _isDragging = true;
            UpdateValueFromMouse(mousePos);
        }

        // Continue dragging
        if (_isDragging && isPressed)
        {
            UpdateValueFromMouse(mousePos);
        }

        // Stop dragging
        if (!isPressed)
        {
            _isDragging = false;
        }

        _previousMouseState = mouseState;
    }

    private void UpdateValueFromMouse(Vector2 mousePos)
    {
        Rectangle bounds = Bounds;
        float relativeX = mousePos.X - bounds.X - ThumbWidth / 2f;
        float usableWidth = bounds.Width - ThumbWidth;
        float percentage = MathHelper.Clamp(relativeX / usableWidth, 0, 1);
        Value = _minValue + percentage * (_maxValue - _minValue);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, GleamRenderer renderer)
    {
        var theme = renderer.Theme;
        Rectangle bounds = Bounds;

        // Track is vertically centered in the bounds
        int trackY = bounds.Y + (bounds.Height - TrackHeight) / 2;
        Rectangle trackBounds = new Rectangle(bounds.X, trackY, bounds.Width, TrackHeight);

        // Draw track background
        renderer.DrawRect(spriteBatch, trackBounds, theme.DarkPurple, Alpha);

        // Draw track border
        renderer.DrawRectBorder(spriteBatch, trackBounds, theme.Gold, 1, Alpha * 0.5f);

        // Draw fill (gold)
        int fillWidth = (int)((bounds.Width - ThumbWidth) * Percentage);
        if (fillWidth > 0)
        {
            Rectangle fillRect = new Rectangle(bounds.X + ThumbWidth / 2, trackY, fillWidth, TrackHeight);
            Color fillColor = _isDragging ? theme.GoldBright : theme.Gold;
            renderer.DrawRect(spriteBatch, fillRect, fillColor, Alpha);
        }

        // Draw thumb
        int thumbX = bounds.X + (int)((bounds.Width - ThumbWidth) * Percentage);
        Rectangle thumbRect = new Rectangle(thumbX, bounds.Y, ThumbWidth, bounds.Height);

        // Thumb background
        Color thumbBg = IsHovered || _isDragging ? theme.MidPurple : theme.DeepPurple;
        renderer.DrawRect(spriteBatch, thumbRect, thumbBg, Alpha);

        // Thumb border
        Color thumbBorder = _isDragging ? theme.GoldBright : theme.Gold;
        renderer.DrawRectBorder(spriteBatch, thumbRect, thumbBorder, 2, Alpha);
    }

    public override bool HandleInput(Vector2 mousePosition, bool mouseDown, bool mouseClicked)
    {
        if (!Enabled || !Visible) return false;

        IsHovered = Bounds.Contains(mousePosition);

        if (IsHovered && mouseClicked)
        {
            _isDragging = true;
            UpdateValueFromMouse(mousePosition);
            return true;
        }

        return false;
    }
}
