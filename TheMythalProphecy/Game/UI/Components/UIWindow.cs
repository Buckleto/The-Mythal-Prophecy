using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheMythalProphecy.Game.UI.Components
{
    /// <summary>
    /// Window with title bar and content area. Supports modal and non-modal modes.
    /// Can be used for dialogs, menus, and information panels.
    /// </summary>
    public class UIWindow : UIElement
    {
        private string _title;
        private UIPanel _contentPanel;
        private bool _isModal;
        private bool _showCloseButton;
        private Rectangle _titleBarBounds;
        private Rectangle _closeButtonBounds;
        private bool _isCloseButtonHovered;

        public string Title
        {
            get => _title;
            set => _title = value ?? string.Empty;
        }

        public UIPanel ContentPanel => _contentPanel;

        public bool IsModal
        {
            get => _isModal;
            set => _isModal = value;
        }

        public bool ShowCloseButton
        {
            get => _showCloseButton;
            set => _showCloseButton = value;
        }

        public float TitleBarHeight { get; set; } = 40f;
        public Color TitleBarColor { get; set; } = new Color(30, 30, 30);
        public Color TitleTextColor { get; set; } = Color.White;

        public event Action OnClose;

        public UIWindow()
        {
            _title = string.Empty;
            _isModal = false;
            _showCloseButton = true;
            SetPadding(0);
        }

        public UIWindow(Vector2 position, Vector2 size, string title = "") : this()
        {
            Position = position;
            Size = size;
            _title = title ?? string.Empty;

            // Create content panel below title bar
            float contentY = TitleBarHeight;
            float contentHeight = size.Y - TitleBarHeight;

            _contentPanel = new UIPanel(new Vector2(0, contentY), new Vector2(size.X, contentHeight));
            _contentPanel.Layout = PanelLayout.Vertical;
            AddChild(_contentPanel);

            UpdateBounds();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update bounds for title bar and close button
            UpdateBounds();

            // Close button hover and click handled by HandleInput in base class
            _isCloseButtonHovered = false; // TODO: Implement mouse handling
        }

        protected override void DrawSelf(SpriteBatch spriteBatch, UITheme theme)
        {
            // Get pixel texture
            Texture2D pixel = Core.GameServices.UI?.PixelTexture;
            if (pixel == null) return;

            // Draw modal overlay if modal
            if (_isModal)
            {
                var viewport = spriteBatch.GraphicsDevice.Viewport;
                Rectangle screenRect = new Rectangle(0, 0, viewport.Width, viewport.Height);
                spriteBatch.Draw(pixel, screenRect, Color.Black * 0.5f);
            }

            // Draw window background
            if (theme.PanelTexture != null)
            {
                spriteBatch.Draw(theme.PanelTexture, Bounds, Color.White);
            }
            else
            {
                spriteBatch.Draw(pixel, Bounds, theme.PrimaryColor);
            }

            // Draw title bar
            spriteBatch.Draw(pixel, _titleBarBounds, TitleBarColor);

            // Draw title text
            if (theme.DefaultFont != null && !string.IsNullOrEmpty(_title))
            {
                Vector2 titleSize = theme.DefaultFont.MeasureString(_title);
                Vector2 titlePos = new Vector2(
                    _titleBarBounds.X + theme.PaddingMedium,
                    _titleBarBounds.Y + (_titleBarBounds.Height - titleSize.Y) / 2
                );
                spriteBatch.DrawString(theme.DefaultFont, _title, titlePos, TitleTextColor);
            }

            // Draw close button
            if (_showCloseButton)
            {
                Color closeButtonColor = _isCloseButtonHovered ? Color.Red : Color.Gray;
                spriteBatch.Draw(pixel, _closeButtonBounds, closeButtonColor);

                // Draw X
                if (theme.DefaultFont != null)
                {
                    Vector2 xSize = theme.DefaultFont.MeasureString("X");
                    Vector2 xPos = new Vector2(
                        _closeButtonBounds.X + (_closeButtonBounds.Width - xSize.X) / 2,
                        _closeButtonBounds.Y + (_closeButtonBounds.Height - xSize.Y) / 2
                    );
                    spriteBatch.DrawString(theme.DefaultFont, "X", xPos, Color.White);
                }
            }

            // Draw border
            DrawBorder(spriteBatch, pixel, theme);
        }

        private void DrawBorder(SpriteBatch spriteBatch, Texture2D pixel, UITheme theme)
        {
            int borderThickness = theme.BorderThickness;
            Color borderColor = theme.BorderColor;

            // Top
            spriteBatch.Draw(pixel,
                new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, borderThickness),
                borderColor);
            // Bottom
            spriteBatch.Draw(pixel,
                new Rectangle(Bounds.X, Bounds.Bottom - borderThickness, Bounds.Width, borderThickness),
                borderColor);
            // Left
            spriteBatch.Draw(pixel,
                new Rectangle(Bounds.X, Bounds.Y, borderThickness, Bounds.Height),
                borderColor);
            // Right
            spriteBatch.Draw(pixel,
                new Rectangle(Bounds.Right - borderThickness, Bounds.Y, borderThickness, Bounds.Height),
                borderColor);
        }

        private void UpdateBounds()
        {
            _titleBarBounds = new Rectangle(
                Bounds.X,
                Bounds.Y,
                Bounds.Width,
                (int)TitleBarHeight
            );

            if (_showCloseButton)
            {
                int buttonSize = (int)(TitleBarHeight * 0.7f);
                int buttonMargin = (int)((TitleBarHeight - buttonSize) / 2);
                _closeButtonBounds = new Rectangle(
                    Bounds.Right - buttonSize - buttonMargin,
                    Bounds.Y + buttonMargin,
                    buttonSize,
                    buttonSize
                );
            }
        }

        public void Close()
        {
            OnClose?.Invoke();
            Visible = false;
        }

        public void Open()
        {
            Visible = true;
        }

        public void Center(int screenWidth, int screenHeight)
        {
            Position = new Vector2((screenWidth - Size.X) / 2, (screenHeight - Size.Y) / 2);
        }
    }
}
