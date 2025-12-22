using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI.Gleam;

/// <summary>
/// Base class for all GleamUI elements.
/// Provides hierarchy, positioning, input handling, and built-in hover animation.
/// </summary>
public abstract class GleamElement
{
    // Hierarchy
    public GleamElement Parent { get; set; }
    public List<GleamElement> Children { get; } = new();

    // Transform
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }

    // State
    public bool Visible { get; set; } = true;
    public bool Enabled { get; set; } = true;
    public bool IsFocused { get; set; }
    public bool IsHovered { get; protected set; }
    public bool IsPressed { get; protected set; }

    // Styling
    public float Alpha { get; set; } = 1f;

    // Animation state
    protected float _hoverProgress; // 0 = normal, 1 = fully hovered
    protected float _elapsedTime;

    // Events
    public event Action<GleamElement> OnClick;
    public event Action<GleamElement> OnHoverStart;
    public event Action<GleamElement> OnHoverEnd;

    /// <summary>
    /// Absolute position in screen space (accounts for parent hierarchy).
    /// </summary>
    public Vector2 AbsolutePosition
    {
        get
        {
            Vector2 pos = Position;
            if (Parent != null)
            {
                pos += Parent.AbsolutePosition;
            }
            return pos;
        }
    }

    /// <summary>
    /// Bounding rectangle in screen space.
    /// </summary>
    public virtual Rectangle Bounds
    {
        get
        {
            Vector2 absPos = AbsolutePosition;
            return new Rectangle((int)absPos.X, (int)absPos.Y, (int)Size.X, (int)Size.Y);
        }
    }

    /// <summary>
    /// Update element and children.
    /// </summary>
    public virtual void Update(GameTime gameTime, GleamRenderer renderer)
    {
        if (!Visible) return;

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _elapsedTime += deltaTime;

        // Animate hover state
        float transitionSpeed = 1f / renderer.Theme.HoverTransitionDuration;
        if (IsHovered)
        {
            _hoverProgress = MathHelper.Min(1f, _hoverProgress + deltaTime * transitionSpeed);
        }
        else
        {
            _hoverProgress = MathHelper.Max(0f, _hoverProgress - deltaTime * transitionSpeed);
        }

        // Update children
        foreach (var child in Children)
        {
            child.Update(gameTime, renderer);
        }
    }

    /// <summary>
    /// Draw element and children.
    /// </summary>
    public virtual void Draw(SpriteBatch spriteBatch, GleamRenderer renderer)
    {
        if (!Visible) return;

        DrawSelf(spriteBatch, renderer);

        foreach (var child in Children)
        {
            child.Draw(spriteBatch, renderer);
        }
    }

    /// <summary>
    /// Override in derived classes to draw the element.
    /// </summary>
    protected abstract void DrawSelf(SpriteBatch spriteBatch, GleamRenderer renderer);

    /// <summary>
    /// Handle mouse input. Returns true if input was consumed.
    /// </summary>
    public virtual bool HandleInput(Vector2 mousePosition, bool mouseDown, bool mouseClicked)
    {
        if (!Enabled || !Visible) return false;

        // Check children first (front to back)
        for (int i = Children.Count - 1; i >= 0; i--)
        {
            if (Children[i].HandleInput(mousePosition, mouseDown, mouseClicked))
                return true;
        }

        // Check if mouse is over this element
        bool wasHovered = IsHovered;
        IsHovered = ContainsPoint(mousePosition);

        if (IsHovered && !wasHovered)
        {
            OnHoverStart?.Invoke(this);
        }
        else if (!IsHovered && wasHovered)
        {
            OnHoverEnd?.Invoke(this);
        }

        // Track pressed state
        IsPressed = IsHovered && mouseDown;

        // Handle click
        if (IsHovered && mouseClicked)
        {
            OnClick?.Invoke(this);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if a point is inside this element.
    /// Override for non-rectangular shapes.
    /// </summary>
    public virtual bool ContainsPoint(Vector2 point)
    {
        return Bounds.Contains(point);
    }

    /// <summary>
    /// Interpolate between two colors based on hover progress.
    /// </summary>
    protected Color LerpColor(Color normal, Color hovered)
    {
        return Color.Lerp(normal, hovered, _hoverProgress);
    }

    /// <summary>
    /// Interpolate between normal, hovered, and pressed colors.
    /// </summary>
    protected Color GetStateColor(Color normal, Color hovered, Color pressed)
    {
        if (IsPressed)
        {
            return Color.Lerp(hovered, pressed, 0.5f);
        }
        return LerpColor(normal, hovered);
    }

    // Child management
    public void AddChild(GleamElement child)
    {
        if (child.Parent != null)
        {
            child.Parent.RemoveChild(child);
        }
        child.Parent = this;
        Children.Add(child);
    }

    public void RemoveChild(GleamElement child)
    {
        if (Children.Remove(child))
        {
            child.Parent = null;
        }
    }

    public void ClearChildren()
    {
        foreach (var child in Children)
        {
            child.Parent = null;
        }
        Children.Clear();
    }

    // Event invocation helpers for derived classes
    protected void InvokeClick() => OnClick?.Invoke(this);
}
