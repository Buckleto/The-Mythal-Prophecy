using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TheMythalProphecy.Game.UI.Components;

/// <summary>
/// Abstract base class for all UI elements
/// </summary>
public abstract class UIElement
{
    // Hierarchy
    public UIElement Parent { get; set; }
    public List<UIElement> Children { get; } = new();

    // Transform
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Anchor { get; set; } = Vector2.Zero; // 0,0 = top-left, 0.5,0.5 = center, 1,1 = bottom-right

    // State
    public bool Visible { get; set; } = true;
    public bool Enabled { get; set; } = true;
    public bool IsFocused { get; set; }
    public bool IsHovered { get; set; }

    // Styling
    public Color Tint { get; set; } = Color.White;
    public float Alpha { get; set; } = 1.0f;
    public int ZIndex { get; set; } = 0;

    // Padding
    public int PaddingLeft { get; set; }
    public int PaddingRight { get; set; }
    public int PaddingTop { get; set; }
    public int PaddingBottom { get; set; }

    // Events
    public event Action<UIElement> OnClick;
    public event Action<UIElement> OnHover;
    public event Action<UIElement> OnFocusGained;
    public event Action<UIElement> OnFocusLost;

    /// <summary>
    /// Get absolute position in screen space
    /// </summary>
    public Vector2 AbsolutePosition
    {
        get
        {
            Vector2 pos = Position;
            if (Parent != null)
            {
                pos += Parent.AbsolutePosition;
                pos += new Vector2(Parent.PaddingLeft, Parent.PaddingTop);
            }
            return pos;
        }
    }

    /// <summary>
    /// Get bounding rectangle in screen space
    /// </summary>
    public Rectangle Bounds
    {
        get
        {
            Vector2 absPos = AbsolutePosition;
            Vector2 offset = new Vector2(Size.X * Anchor.X, Size.Y * Anchor.Y);
            return new Rectangle(
                (int)(absPos.X - offset.X),
                (int)(absPos.Y - offset.Y),
                (int)Size.X,
                (int)Size.Y
            );
        }
    }

    /// <summary>
    /// Get content area (bounds minus padding)
    /// </summary>
    public Rectangle ContentBounds
    {
        get
        {
            Rectangle bounds = Bounds;
            return new Rectangle(
                bounds.X + PaddingLeft,
                bounds.Y + PaddingTop,
                bounds.Width - PaddingLeft - PaddingRight,
                bounds.Height - PaddingTop - PaddingBottom
            );
        }
    }

    /// <summary>
    /// Update the UI element
    /// </summary>
    public virtual void Update(GameTime gameTime)
    {
        if (!Enabled) return;

        foreach (var child in Children)
        {
            child.Update(gameTime);
        }
    }

    /// <summary>
    /// Draw the UI element
    /// </summary>
    public virtual void Draw(SpriteBatch spriteBatch, UITheme theme)
    {
        if (!Visible) return;

        // Draw self
        DrawSelf(spriteBatch, theme);

        // Draw children
        foreach (var child in Children)
        {
            child.Draw(spriteBatch, theme);
        }
    }

    /// <summary>
    /// Draw the element itself (override in derived classes)
    /// </summary>
    protected abstract void DrawSelf(SpriteBatch spriteBatch, UITheme theme);

    /// <summary>
    /// Handle input for this element
    /// </summary>
    public virtual bool HandleInput(Vector2 mousePosition, bool mouseClicked)
    {
        if (!Enabled || !Visible) return false;

        // Check children first (from front to back)
        for (int i = Children.Count - 1; i >= 0; i--)
        {
            if (Children[i].HandleInput(mousePosition, mouseClicked))
                return true;
        }

        // Check if mouse is over this element
        bool wasHovered = IsHovered;
        IsHovered = Bounds.Contains(mousePosition);

        if (IsHovered && !wasHovered)
        {
            OnHover?.Invoke(this);
        }

        if (IsHovered && mouseClicked)
        {
            OnClick?.Invoke(this);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Set focus on this element
    /// </summary>
    public virtual void SetFocus(bool focused)
    {
        if (IsFocused == focused) return;

        IsFocused = focused;

        if (focused)
            OnFocusGained?.Invoke(this);
        else
            OnFocusLost?.Invoke(this);
    }

    /// <summary>
    /// Add a child element
    /// </summary>
    public void AddChild(UIElement child)
    {
        if (child.Parent != null)
        {
            child.Parent.RemoveChild(child);
        }

        child.Parent = this;
        Children.Add(child);
    }

    /// <summary>
    /// Remove a child element
    /// </summary>
    public void RemoveChild(UIElement child)
    {
        if (Children.Remove(child))
        {
            child.Parent = null;
        }
    }

    /// <summary>
    /// Remove all children
    /// </summary>
    public void ClearChildren()
    {
        foreach (var child in Children)
        {
            child.Parent = null;
        }
        Children.Clear();
    }

    /// <summary>
    /// Set padding for all sides
    /// </summary>
    public void SetPadding(int padding)
    {
        PaddingLeft = PaddingRight = PaddingTop = PaddingBottom = padding;
    }

    /// <summary>
    /// Set padding for horizontal and vertical
    /// </summary>
    public void SetPadding(int horizontal, int vertical)
    {
        PaddingLeft = PaddingRight = horizontal;
        PaddingTop = PaddingBottom = vertical;
    }

    /// <summary>
    /// Set padding for all sides individually
    /// </summary>
    public void SetPadding(int left, int right, int top, int bottom)
    {
        PaddingLeft = left;
        PaddingRight = right;
        PaddingTop = top;
        PaddingBottom = bottom;
    }
}
