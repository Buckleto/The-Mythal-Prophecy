using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI.Gleam;

/// <summary>
/// Window with title bar and content area. Supports modal overlay and close button.
/// </summary>
public class GleamWindow : GleamElement
{
    private string _title = string.Empty;
    private Rectangle _titleBarBounds;
    private Rectangle _closeButtonBounds;
    private bool _closeButtonHovered;

    public string Title
    {
        get => _title;
        set => _title = value ?? string.Empty;
    }

    public float TitleBarHeight { get; set; } = 40f;
    public bool ShowCloseButton { get; set; } = true;
    public bool IsModal { get; set; }
    public SpriteFont TitleFont { get; set; }

    public event Action<GleamWindow> OnClose;

    public GleamWindow(Vector2 position, Vector2 size, string title = "")
    {
        Position = position;
        Size = size;
        _title = title ?? string.Empty;
    }

    public Rectangle ContentBounds
    {
        get
        {
            Rectangle bounds = Bounds;
            return new Rectangle(
                bounds.X,
                bounds.Y + (int)TitleBarHeight,
                bounds.Width,
                bounds.Height - (int)TitleBarHeight
            );
        }
    }

    public override void Update(GameTime gameTime, GleamRenderer renderer)
    {
        base.Update(gameTime, renderer);
        UpdateBounds();
    }

    private void UpdateBounds()
    {
        Rectangle bounds = Bounds;

        _titleBarBounds = new Rectangle(
            bounds.X,
            bounds.Y,
            bounds.Width,
            (int)TitleBarHeight
        );

        if (ShowCloseButton)
        {
            int buttonSize = (int)(TitleBarHeight * 0.6f);
            int buttonMargin = (int)((TitleBarHeight - buttonSize) / 2);
            _closeButtonBounds = new Rectangle(
                bounds.Right - buttonSize - buttonMargin,
                bounds.Y + buttonMargin,
                buttonSize,
                buttonSize
            );
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, GleamRenderer renderer)
    {
        var theme = renderer.Theme;
        Rectangle bounds = Bounds;

        // Modal overlay
        if (IsModal)
        {
            var viewport = spriteBatch.GraphicsDevice.Viewport;
            Rectangle screenRect = new Rectangle(0, 0, viewport.Width, viewport.Height);
            renderer.DrawRect(spriteBatch, screenRect, Color.Black, 0.5f);
        }

        // Window background
        renderer.DrawRect(spriteBatch, bounds, theme.DeepPurple, Alpha);

        // Title bar
        renderer.DrawRect(spriteBatch, _titleBarBounds, theme.DarkPurple, Alpha);

        // Title text
        var font = TitleFont ?? theme.MenuFont ?? theme.DefaultFont;
        if (font != null && !string.IsNullOrEmpty(_title))
        {
            Vector2 titleSize = font.MeasureString(_title);
            Vector2 titlePos = new Vector2(
                _titleBarBounds.X + theme.PaddingMedium,
                _titleBarBounds.Y + (_titleBarBounds.Height - titleSize.Y) / 2f
            );
            renderer.DrawText(spriteBatch, font, _title, titlePos, theme.GoldBright, true, Alpha);
        }

        // Close button
        if (ShowCloseButton)
        {
            Color buttonBg = _closeButtonHovered ? theme.MidPurple : theme.DarkPurple;
            renderer.DrawRect(spriteBatch, _closeButtonBounds, buttonBg, Alpha);
            renderer.DrawRectBorder(spriteBatch, _closeButtonBounds, _closeButtonHovered ? theme.GoldBright : theme.Gold, 1, Alpha);

            // Draw X
            if (font != null)
            {
                renderer.DrawTextCentered(spriteBatch, font, "X", _closeButtonBounds,
                    _closeButtonHovered ? theme.GoldBright : theme.TextPrimary, false, Alpha);
            }
        }

        // Window border
        renderer.DrawRectBorder(spriteBatch, bounds, theme.Gold, 2, Alpha);

        // Title bar separator
        renderer.DrawLine(spriteBatch,
            new Vector2(bounds.X, _titleBarBounds.Bottom),
            new Vector2(bounds.Right, _titleBarBounds.Bottom),
            2, theme.Gold * Alpha);
    }

    public override bool HandleInput(Vector2 mousePosition, bool mouseDown, bool mouseClicked)
    {
        if (!Enabled || !Visible) return false;

        // Check children first
        for (int i = Children.Count - 1; i >= 0; i--)
        {
            if (Children[i].HandleInput(mousePosition, mouseDown, mouseClicked))
                return true;
        }

        // Check close button
        _closeButtonHovered = ShowCloseButton && _closeButtonBounds.Contains(mousePosition);

        if (_closeButtonHovered && mouseClicked)
        {
            Close();
            return true;
        }

        // Modal windows consume all input within their bounds
        if (IsModal)
        {
            return Bounds.Contains(mousePosition);
        }

        return false;
    }

    public void Close()
    {
        OnClose?.Invoke(this);
        Visible = false;
    }

    public void Open()
    {
        Visible = true;
    }

    public void Center(int screenWidth, int screenHeight)
    {
        Position = new Vector2(
            (screenWidth - Size.X) / 2f,
            (screenHeight - Size.Y) / 2f
        );
    }
}
