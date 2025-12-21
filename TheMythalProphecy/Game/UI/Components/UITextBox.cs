using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TheMythalProphecy.Game.UI.Components
{
    /// <summary>
    /// Multi-line scrollable text box for displaying large amounts of text.
    /// Supports word wrapping, scrolling, and auto-scroll modes.
    /// </summary>
    public class UITextBox : UIElement
    {
        private string _text;
        private List<string> _wrappedLines;
        private int _scrollOffset;
        private int _maxVisibleLines;
        private bool _autoScroll; // Auto-scroll to bottom when new text is added
        private bool _needsReflow;

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

        public Color TextColor { get; set; } = Color.White;
        public float LineSpacing { get; set; } = 1.2f;
        public bool AutoScroll
        {
            get => _autoScroll;
            set => _autoScroll = value;
        }

        public int ScrollOffset
        {
            get => _scrollOffset;
            set => _scrollOffset = Math.Clamp(value, 0, MaxScrollOffset);
        }

        public int MaxScrollOffset => Math.Max(0, _wrappedLines.Count - _maxVisibleLines);
        public bool CanScrollUp => _scrollOffset > 0;
        public bool CanScrollDown => _scrollOffset < MaxScrollOffset;

        public UITextBox()
        {
            _text = string.Empty;
            _wrappedLines = new List<string>();
            _scrollOffset = 0;
            _autoScroll = false;
            _needsReflow = true;
            SetPadding(4);
        }

        public UITextBox(Vector2 position, Vector2 size) : this()
        {
            Position = position;
            Size = size;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_needsReflow)
            {
                ReflowText();
                _needsReflow = false;

                if (_autoScroll)
                {
                    ScrollToBottom();
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch, UITheme theme)
        {
            // Get pixel texture
            Texture2D pixel = Core.GameServices.UI?.PixelTexture;
            if (pixel == null) return;

            var font = theme.DefaultFont;

            // Draw background
            if (theme.PanelTexture != null)
            {
                spriteBatch.Draw(theme.PanelTexture, Bounds, Color.White);
            }
            else
            {
                spriteBatch.Draw(pixel, Bounds, theme.PrimaryColor);
            }

            // Draw text with scrolling
            if (font != null && _wrappedLines.Count > 0)
            {
                float lineHeight = font.LineSpacing * LineSpacing;
                Vector2 textPosition = new Vector2(
                    Bounds.X + theme.PaddingMedium,
                    Bounds.Y + theme.PaddingMedium
                );

                // Calculate visible range
                int startLine = _scrollOffset;
                int endLine = Math.Min(_scrollOffset + _maxVisibleLines, _wrappedLines.Count);

                // Draw visible lines
                for (int i = startLine; i < endLine; i++)
                {
                    spriteBatch.DrawString(
                        font,
                        _wrappedLines[i],
                        textPosition,
                        TextColor
                    );
                    textPosition.Y += lineHeight;
                }
            }

            // Draw scroll indicator if needed
            if (_wrappedLines.Count > _maxVisibleLines)
            {
                DrawScrollIndicator(spriteBatch, pixel, theme);
            }
        }

        private void DrawScrollIndicator(SpriteBatch spriteBatch, Texture2D pixel, UITheme theme)
        {
            // Draw scrollbar on the right side
            float scrollbarWidth = 8f;
            float scrollbarX = Bounds.Right - scrollbarWidth - theme.PaddingMedium;
            float scrollbarY = Bounds.Y + theme.PaddingMedium;
            float scrollbarHeight = Bounds.Height - theme.PaddingMedium * 2;

            // Background track
            Rectangle trackRect = new Rectangle(
                (int)scrollbarX,
                (int)scrollbarY,
                (int)scrollbarWidth,
                (int)scrollbarHeight
            );
            spriteBatch.Draw(pixel, trackRect, Color.Black * 0.3f);

            // Thumb
            if (_wrappedLines.Count > 0)
            {
                float thumbHeight = Math.Max(20f, scrollbarHeight * (_maxVisibleLines / (float)_wrappedLines.Count));
                float thumbY = scrollbarY + (scrollbarHeight - thumbHeight) * (_scrollOffset / (float)MaxScrollOffset);

                Rectangle thumbRect = new Rectangle(
                    (int)scrollbarX,
                    (int)thumbY,
                    (int)scrollbarWidth,
                    (int)thumbHeight
                );
                spriteBatch.Draw(pixel, thumbRect, theme.AccentColor);
            }
        }

        private void ReflowText()
        {
            _wrappedLines.Clear();

            var theme = Core.GameServices.UI?.Theme;
            if (theme == null) return;

            var font = theme.DefaultFont;

            if (font == null || string.IsNullOrEmpty(_text))
            {
                _maxVisibleLines = 0;
                return;
            }

            // Calculate available width for text
            float availableWidth = Bounds.Width - theme.PaddingMedium * 2;

            // Account for scrollbar if needed
            if (_wrappedLines.Count > _maxVisibleLines)
            {
                availableWidth -= 8f + theme.PaddingMedium;
            }

            // Wrap text
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

            // Calculate max visible lines
            float lineHeight = font.LineSpacing * LineSpacing;
            float availableHeight = Bounds.Height - theme.PaddingMedium * 2;
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
                Vector2 size = font.MeasureString(testLine);

                if (size.X > maxWidth && !string.IsNullOrEmpty(currentLine))
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

        public void ScrollUp(int lines = 1)
        {
            ScrollOffset -= lines;
        }

        public void ScrollDown(int lines = 1)
        {
            ScrollOffset += lines;
        }

        public void ScrollToTop()
        {
            ScrollOffset = 0;
        }

        public void ScrollToBottom()
        {
            ScrollOffset = MaxScrollOffset;
        }

        public void AppendText(string text)
        {
            Text += text;
            if (_autoScroll)
            {
                ScrollToBottom();
            }
        }

        public void Clear()
        {
            Text = string.Empty;
            ScrollOffset = 0;
        }
    }
}
