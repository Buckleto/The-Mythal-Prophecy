using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI.Gleam;

/// <summary>
/// Toggle button for on/off settings like fullscreen.
/// </summary>
public class GleamToggle : GleamElement
{
    private bool _isOn;
    private float _toggleProgress; // 0 = off, 1 = on

    public bool IsOn
    {
        get => _isOn;
        set
        {
            if (_isOn != value)
            {
                _isOn = value;
                OnToggled?.Invoke(this, _isOn);
            }
        }
    }

    public string OnText { get; set; } = "ON";
    public string OffText { get; set; } = "OFF";
    public SpriteFont Font { get; set; }

    public event Action<GleamToggle, bool> OnToggled;

    public GleamToggle(Vector2 position, Vector2 size, bool initialState = false)
    {
        Position = position;
        Size = size;
        _isOn = initialState;
        _toggleProgress = initialState ? 1f : 0f;
    }

    public override void Update(GameTime gameTime, GleamRenderer renderer)
    {
        base.Update(gameTime, renderer);

        // Animate toggle state
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float speed = 8f;

        if (_isOn)
        {
            _toggleProgress = MathHelper.Min(1f, _toggleProgress + deltaTime * speed);
        }
        else
        {
            _toggleProgress = MathHelper.Max(0f, _toggleProgress - deltaTime * speed);
        }
    }

    public override bool HandleInput(Vector2 mousePosition, bool mouseDown, bool mouseClicked)
    {
        if (!Enabled || !Visible) return false;

        IsHovered = ContainsPoint(mousePosition);

        if (IsHovered && mouseClicked)
        {
            IsOn = !IsOn;
            return true;
        }

        return false;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, GleamRenderer renderer)
    {
        var theme = renderer.Theme;
        Rectangle bounds = Bounds;

        // Background - interpolate color based on toggle state
        Color offColor = theme.DeepPurple;
        Color onColor = theme.MidPurple;
        Color bgColor = Color.Lerp(offColor, onColor, _toggleProgress);

        if (IsHovered)
        {
            bgColor = Color.Lerp(bgColor, theme.MidPurple, 0.3f);
        }

        renderer.DrawRect(spriteBatch, bounds, bgColor, Alpha);

        // Border - gold when on, muted when off
        Color borderColor = Color.Lerp(theme.GoldDim, theme.Gold, _toggleProgress);
        if (IsHovered) borderColor = theme.GoldBright;
        renderer.DrawRectBorder(spriteBatch, bounds, borderColor, 2, Alpha);

        // Text
        var font = Font ?? theme.DefaultFont;
        if (font != null)
        {
            string text = _isOn ? OnText : OffText;
            Color textColor = _isOn ? theme.GoldBright : theme.TextSecondary;
            renderer.DrawTextCentered(spriteBatch, font, text, bounds, textColor, true, Alpha);
        }
    }
}
