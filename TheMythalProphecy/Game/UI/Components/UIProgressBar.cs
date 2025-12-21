using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheMythalProphecy.Game.UI.Components;

/// <summary>
/// Progress bar orientation
/// </summary>
public enum ProgressBarOrientation
{
    Horizontal,
    Vertical
}

/// <summary>
/// Progress bar for displaying values like HP/MP
/// </summary>
public class UIProgressBar : UIElement
{
    private float _currentValue;
    private float _maxValue;
    private float _displayValue; // For smooth transitions

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
    public Color BackgroundColor { get; set; } = new Color(40, 40, 40);
    public Color BorderColor { get; set; } = new Color(200, 200, 220);
    public int BorderThickness { get; set; } = 2;

    public ProgressBarOrientation Orientation { get; set; } = ProgressBarOrientation.Horizontal;
    public bool ShowText { get; set; } = true;
    public string TextFormat { get; set; } = "{0}/{1}"; // {0} = current, {1} = max
    public float LowThreshold { get; set; } = 0.25f; // Switch to low color below this percentage
    public float TransitionSpeed { get; set; } = 5.0f; // How fast the bar animates

    private UILabel _textLabel;

    public UIProgressBar()
    {
        _textLabel = new UILabel
        {
            Alignment = TextAlignment.Center,
            Scale = 0.8f
        };
        AddChild(_textLabel);

        _maxValue = 100;
        _currentValue = 100;
        _displayValue = 100;
    }

    public UIProgressBar(Vector2 position, Vector2 size, float maxValue = 100) : this()
    {
        Position = position;
        Size = size;
        MaxValue = maxValue;
        CurrentValue = maxValue;
    }

    public override void Update(GameTime gameTime)
    {
        // Smooth transition for display value
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_displayValue != _currentValue)
        {
            float difference = _currentValue - _displayValue;
            float change = difference * TransitionSpeed * delta;

            if (Math.Abs(change) < 0.1f)
            {
                _displayValue = _currentValue;
            }
            else
            {
                _displayValue += change;
            }
        }

        // Update text
        if (ShowText)
        {
            _textLabel.Text = string.Format(TextFormat, (int)_currentValue, (int)_maxValue);
            _textLabel.Size = Size;
            _textLabel.Visible = true;
        }
        else
        {
            _textLabel.Visible = false;
        }

        base.Update(gameTime);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, UITheme theme)
    {
        Rectangle bounds = Bounds;
        Texture2D pixel = Core.GameServices.UI?.PixelTexture;
        if (pixel == null) return;

        // Draw background
        spriteBatch.Draw(pixel, bounds, BackgroundColor * Alpha);

        // Calculate fill area
        float fillPercentage = _maxValue > 0 ? MathHelper.Clamp(_displayValue / _maxValue, 0, 1) : 0;
        Rectangle fillBounds = bounds;

        if (Orientation == ProgressBarOrientation.Horizontal)
        {
            fillBounds.Width = (int)(bounds.Width * fillPercentage);
        }
        else
        {
            int fillHeight = (int)(bounds.Height * fillPercentage);
            fillBounds.Y = bounds.Bottom - fillHeight;
            fillBounds.Height = fillHeight;
        }

        // Determine fill color based on percentage
        Color currentFillColor = fillPercentage <= LowThreshold ? LowFillColor : FillColor;

        // Draw fill
        if (fillBounds.Width > 0 && fillBounds.Height > 0)
        {
            spriteBatch.Draw(pixel, fillBounds, currentFillColor * Alpha);
        }

        // Draw border
        if (BorderThickness > 0)
        {
            // Top
            spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, BorderThickness), BorderColor * Alpha);
            // Bottom
            spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Bottom - BorderThickness, bounds.Width, BorderThickness), BorderColor * Alpha);
            // Left
            spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, BorderThickness, bounds.Height), BorderColor * Alpha);
            // Right
            spriteBatch.Draw(pixel, new Rectangle(bounds.Right - BorderThickness, bounds.Y, BorderThickness, bounds.Height), BorderColor * Alpha);
        }
    }

    /// <summary>
    /// Set value as a percentage (0-1)
    /// </summary>
    public void SetPercentage(float percentage)
    {
        CurrentValue = _maxValue * MathHelper.Clamp(percentage, 0, 1);
    }

    /// <summary>
    /// Create an HP bar with default styling
    /// </summary>
    public static UIProgressBar CreateHPBar(Vector2 position, Vector2 size, float maxHP)
    {
        return new UIProgressBar(position, size, maxHP)
        {
            FillColor = new Color(100, 220, 100),
            LowFillColor = new Color(220, 60, 60),
            ShowText = true
        };
    }

    /// <summary>
    /// Create an MP bar with default styling
    /// </summary>
    public static UIProgressBar CreateMPBar(Vector2 position, Vector2 size, float maxMP)
    {
        return new UIProgressBar(position, size, maxMP)
        {
            FillColor = new Color(100, 150, 255),
            LowFillColor = new Color(60, 90, 180),
            ShowText = true
        };
    }
}
