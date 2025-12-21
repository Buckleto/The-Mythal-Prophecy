using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TheMythalProphecy.Game.UI.Components;

namespace TheMythalProphecy.Game.UI.HUD
{
    /// <summary>
    /// Scrolling message log for displaying combat messages, events, and notifications.
    /// </summary>
    public class MessageLog : UIElement
    {
        private Queue<LogMessage> _messages;
        private List<LogMessage> _visibleMessages;
        private int _maxMessages;
        private int _maxVisibleLines;
        private float _messageLifetime; // Seconds before message fades
        private float _fadeDuration; // Fade-out duration in seconds

        public int MaxMessages
        {
            get => _maxMessages;
            set => _maxMessages = Math.Max(1, value);
        }

        public float MessageLifetime
        {
            get => _messageLifetime;
            set => _messageLifetime = Math.Max(0, value);
        }

        public bool AutoFade { get; set; }

        public MessageLog()
        {
            _messages = new Queue<LogMessage>();
            _visibleMessages = new List<LogMessage>();
            _maxMessages = 50;
            _maxVisibleLines = 5;
            _messageLifetime = 5f;
            _fadeDuration = 1f;
            AutoFade = true;
            SetPadding(5);
        }

        public MessageLog(Vector2 position, Vector2 size) : this()
        {
            Position = position;
            Size = size;
        }

        public void AddMessage(string text, Color? color = null)
        {
            var message = new LogMessage
            {
                Text = text,
                Color = color ?? Color.White,
                TimeAdded = 0f,
                Alpha = 1f
            };

            _messages.Enqueue(message);

            // Maintain max message count
            while (_messages.Count > _maxMessages)
            {
                _messages.Dequeue();
            }

            UpdateVisibleMessages();
        }

        public void AddCombatMessage(string text)
        {
            AddMessage(text, Color.Yellow);
        }

        public void AddDamageMessage(string text)
        {
            AddMessage(text, Color.OrangeRed);
        }

        public void AddHealMessage(string text)
        {
            AddMessage(text, Color.LightGreen);
        }

        public void AddSystemMessage(string text)
        {
            AddMessage(text, Color.Cyan);
        }

        public void Clear()
        {
            _messages.Clear();
            _visibleMessages.Clear();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!AutoFade) return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update message ages and fade
            foreach (var message in _visibleMessages)
            {
                message.TimeAdded += deltaTime;

                // Calculate fade
                if (message.TimeAdded > _messageLifetime)
                {
                    float fadeProgress = (message.TimeAdded - _messageLifetime) / _fadeDuration;
                    message.Alpha = Math.Max(0, 1f - fadeProgress);
                }
            }

            UpdateVisibleMessages();
        }

        private void UpdateVisibleMessages()
        {
            _visibleMessages.Clear();

            // Take the most recent messages
            var messageArray = _messages.ToArray();
            int startIndex = Math.Max(0, messageArray.Length - _maxVisibleLines);

            for (int i = startIndex; i < messageArray.Length; i++)
            {
                _visibleMessages.Add(messageArray[i]);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch, UITheme theme)
        {
            if (_visibleMessages.Count == 0) return;

            var font = theme.DefaultFont;
            Texture2D pixel = Core.GameServices.UI?.PixelTexture;

            if (font == null)
            {
                Console.WriteLine("[MessageLog] ERROR: Theme.DefaultFont is null, cannot draw messages");
                return;
            }

            if (pixel == null)
            {
                Console.WriteLine("[MessageLog] ERROR: PixelTexture is null, cannot draw background");
                return;
            }

            // Draw semi-transparent background
            spriteBatch.Draw(pixel, Bounds, Color.Black * 0.5f);

            // Draw messages from bottom to top
            float lineHeight = font.LineSpacing + 2f;
            float y = Bounds.Bottom - theme.PaddingMedium - lineHeight;

            for (int i = _visibleMessages.Count - 1; i >= 0; i--)
            {
                var message = _visibleMessages[i];
                Vector2 position = new Vector2(Bounds.X + theme.PaddingMedium, y);

                // Apply fade
                Color color = message.Color * message.Alpha;

                // Draw text with shadow for readability
                Vector2 shadowOffset = new Vector2(1, 1);
                spriteBatch.DrawString(font, message.Text, position + shadowOffset, Color.Black * message.Alpha);
                spriteBatch.DrawString(font, message.Text, position, color);

                y -= lineHeight;

                // Stop if we've gone above the bounds
                if (y < Bounds.Y)
                    break;
            }
        }

        private class LogMessage
        {
            public string Text { get; set; }
            public Color Color { get; set; }
            public float TimeAdded { get; set; }
            public float Alpha { get; set; }
        }
    }
}
