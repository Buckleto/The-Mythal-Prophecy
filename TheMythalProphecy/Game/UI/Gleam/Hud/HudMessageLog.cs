using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI.Gleam;

public enum HudMessageType { Default, System, Combat, Damage, Heal }

/// <summary>
/// Scrolling message log for combat events and notifications.
/// Messages auto-fade after a configurable lifetime.
/// </summary>
public class HudMessageLog : GleamElement
{
    private readonly HudTheme _hudTheme;
    private readonly Queue<HudLogMessage> _messages;
    private readonly List<HudLogMessage> _visibleMessages;

    private const int MaxMessages = 50;
    private const int Padding = 8;

    public int MaxVisibleLines { get; set; } = 5;
    public float MessageLifetime { get; set; } = 5f;
    public float FadeDuration { get; set; } = 1f;
    public bool AutoFade { get; set; } = true;

    public HudMessageLog(Vector2 position, Vector2 size, HudTheme hudTheme)
    {
        Position = position;
        Size = size;
        _hudTheme = hudTheme;
        _messages = new Queue<HudLogMessage>();
        _visibleMessages = new List<HudLogMessage>();
    }

    public void AddMessage(string text, HudMessageType type = HudMessageType.Default)
    {
        Color color = type switch
        {
            HudMessageType.System => _hudTheme.MessageSystem,
            HudMessageType.Combat => _hudTheme.MessageCombat,
            HudMessageType.Damage => _hudTheme.MessageDamage,
            HudMessageType.Heal => _hudTheme.MessageHeal,
            _ => _hudTheme.MessageDefault
        };

        var message = new HudLogMessage
        {
            Text = text,
            Color = color,
            TimeAdded = _elapsedTime,
            Alpha = 1f
        };

        _messages.Enqueue(message);

        // Remove oldest if over limit
        while (_messages.Count > MaxMessages)
        {
            _messages.Dequeue();
        }

        UpdateVisibleMessages();
    }

    // Convenience methods
    public void AddSystemMessage(string text) => AddMessage(text, HudMessageType.System);
    public void AddCombatMessage(string text) => AddMessage(text, HudMessageType.Combat);
    public void AddDamageMessage(string text) => AddMessage(text, HudMessageType.Damage);
    public void AddHealMessage(string text) => AddMessage(text, HudMessageType.Heal);

    private void UpdateVisibleMessages()
    {
        _visibleMessages.Clear();

        // Get the most recent messages
        var allMessages = _messages.ToArray();
        int startIndex = Math.Max(0, allMessages.Length - MaxVisibleLines);

        for (int i = startIndex; i < allMessages.Length; i++)
        {
            _visibleMessages.Add(allMessages[i]);
        }
    }

    public override void Update(GameTime gameTime, GleamRenderer renderer)
    {
        base.Update(gameTime, renderer);

        if (!AutoFade) return;

        // Update message alpha for fading
        foreach (var message in _visibleMessages)
        {
            float age = _elapsedTime - message.TimeAdded;
            float fadeStart = MessageLifetime - FadeDuration;

            if (age > fadeStart)
            {
                float fadeProgress = (age - fadeStart) / FadeDuration;
                message.Alpha = MathHelper.Clamp(1f - fadeProgress, 0f, 1f);
            }
        }

        // Remove fully faded messages from queue
        while (_messages.Count > 0)
        {
            var oldest = _messages.Peek();
            float age = _elapsedTime - oldest.TimeAdded;
            if (age > MessageLifetime)
            {
                _messages.Dequeue();
                UpdateVisibleMessages();
            }
            else
            {
                break;
            }
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, GleamRenderer renderer)
    {
        Rectangle bounds = Bounds;

        // Background (subtle)
        renderer.DrawRect(spriteBatch, bounds, _hudTheme.PanelBackground, Alpha * 0.5f);

        // Draw messages from bottom to top
        var font = _hudTheme.HudFont ?? _hudTheme.DefaultFont;
        if (font == null || _visibleMessages.Count == 0) return;

        float lineHeight = font.LineSpacing;
        float y = bounds.Bottom - Padding - lineHeight;

        // Draw in reverse order (newest at bottom)
        for (int i = _visibleMessages.Count - 1; i >= 0; i--)
        {
            var message = _visibleMessages[i];
            if (message.Alpha <= 0) continue;

            Vector2 pos = new Vector2(bounds.X + Padding, y);
            renderer.DrawText(spriteBatch, font, message.Text, pos, message.Color, true, Alpha * message.Alpha);

            y -= lineHeight;
            if (y < bounds.Y + Padding) break;
        }
    }

    public override bool HandleInput(Vector2 mousePosition, bool mouseDown, bool mouseClicked)
    {
        return false; // Display only
    }

    public void Clear()
    {
        _messages.Clear();
        _visibleMessages.Clear();
    }
}

internal class HudLogMessage
{
    public string Text { get; set; }
    public Color Color { get; set; }
    public float TimeAdded { get; set; }
    public float Alpha { get; set; } = 1f;
}
