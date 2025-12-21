using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TheMythalProphecy.Game.UI.Components;

/// <summary>
/// Scrollable list box for displaying selectable items
/// </summary>
public class UIListBox : UIElement
{
    private readonly List<string> _items = new();
    private int _selectedIndex = -1;
    private int _scrollOffset = 0;
    private int _visibleItemCount;

    public Color BackgroundColor { get; set; } = new Color(40, 40, 60, 230);
    public Color SelectedItemColor { get; set; } = new Color(100, 150, 200);
    public Color HoverItemColor { get; set; } = new Color(80, 120, 160);
    public Color BorderColor { get; set; } = new Color(200, 200, 220);
    public int BorderThickness { get; set; } = 2;
    public int ItemHeight { get; set; } = 30;
    public int ItemPadding { get; set; } = 4;

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (_selectedIndex != value && value >= -1 && value < _items.Count)
            {
                _selectedIndex = value;
                OnSelectionChanged?.Invoke(this, _selectedIndex);

                // Auto-scroll to keep selected item visible
                if (_selectedIndex >= 0)
                {
                    if (_selectedIndex < _scrollOffset)
                    {
                        _scrollOffset = _selectedIndex;
                    }
                    else if (_selectedIndex >= _scrollOffset + _visibleItemCount)
                    {
                        _scrollOffset = _selectedIndex - _visibleItemCount + 1;
                    }
                }
            }
        }
    }

    public string SelectedItem => _selectedIndex >= 0 && _selectedIndex < _items.Count ? _items[_selectedIndex] : null;

    public IReadOnlyList<string> Items => _items.AsReadOnly();

    public event Action<UIListBox, int> OnSelectionChanged;
    public event Action<UIListBox, int> OnItemActivated;

    private KeyboardState _previousKeyState;

    public UIListBox()
    {
        SetPadding(4);
    }

    public UIListBox(Vector2 position, Vector2 size) : this()
    {
        Position = position;
        Size = size;
    }

    public override void Update(GameTime gameTime)
    {
        // Calculate visible item count
        Rectangle contentArea = ContentBounds;
        _visibleItemCount = (int)MathHelper.Max(1, contentArea.Height / ItemHeight);

        // Handle keyboard navigation if focused
        if (IsFocused)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Down) && !_previousKeyState.IsKeyDown(Keys.Down))
            {
                SelectNext();
            }
            else if (keyState.IsKeyDown(Keys.Up) && !_previousKeyState.IsKeyDown(Keys.Up))
            {
                SelectPrevious();
            }
            else if (keyState.IsKeyDown(Keys.PageDown) && !_previousKeyState.IsKeyDown(Keys.PageDown))
            {
                SelectedIndex = Math.Min(_selectedIndex + _visibleItemCount, _items.Count - 1);
            }
            else if (keyState.IsKeyDown(Keys.PageUp) && !_previousKeyState.IsKeyDown(Keys.PageUp))
            {
                SelectedIndex = Math.Max(_selectedIndex - _visibleItemCount, 0);
            }
            else if ((keyState.IsKeyDown(Keys.Enter) || keyState.IsKeyDown(Keys.Space)) &&
                     (!_previousKeyState.IsKeyDown(Keys.Enter) && !_previousKeyState.IsKeyDown(Keys.Space)))
            {
                if (_selectedIndex >= 0)
                {
                    OnItemActivated?.Invoke(this, _selectedIndex);
                }
            }

            _previousKeyState = keyState;
        }

        base.Update(gameTime);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, UITheme theme)
    {
        Rectangle bounds = Bounds;
        Rectangle contentArea = ContentBounds;
        Texture2D pixel = Core.GameServices.UI?.PixelTexture;
        if (pixel == null) return;

        // Draw background
        spriteBatch.Draw(pixel, bounds, BackgroundColor * Alpha);

        // Draw items
        SpriteFont font = theme.DefaultFont;
        if (font != null)
        {
            int y = contentArea.Y;
            int endIndex = Math.Min(_scrollOffset + _visibleItemCount, _items.Count);

            for (int i = _scrollOffset; i < endIndex; i++)
            {
                Rectangle itemBounds = new Rectangle(contentArea.X, y, contentArea.Width, ItemHeight);

                // Draw item background if selected or hovered
                if (i == _selectedIndex)
                {
                    spriteBatch.Draw(pixel, itemBounds, SelectedItemColor * Alpha);
                }
                else if (IsHovered && itemBounds.Contains(Mouse.GetState().Position))
                {
                    spriteBatch.Draw(pixel, itemBounds, HoverItemColor * Alpha);
                }

                // Draw item text
                Vector2 textPos = new Vector2(itemBounds.X + ItemPadding, itemBounds.Y + ItemPadding);
                spriteBatch.DrawString(font, _items[i], textPos, theme.TextColor * Alpha);

                y += ItemHeight;
            }
        }

        // Draw border
        if (BorderThickness > 0)
        {
            Color border = IsFocused ? theme.HighlightColor : BorderColor;
            // Top
            spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, BorderThickness), border * Alpha);
            // Bottom
            spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Bottom - BorderThickness, bounds.Width, BorderThickness), border * Alpha);
            // Left
            spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, BorderThickness, bounds.Height), border * Alpha);
            // Right
            spriteBatch.Draw(pixel, new Rectangle(bounds.Right - BorderThickness, bounds.Y, BorderThickness, bounds.Height), border * Alpha);
        }

        // Draw scrollbar if needed
        if (_items.Count > _visibleItemCount)
        {
            DrawScrollbar(spriteBatch, pixel, theme);
        }
    }

    private void DrawScrollbar(SpriteBatch spriteBatch, Texture2D pixel, UITheme theme)
    {
        Rectangle bounds = Bounds;
        int scrollbarWidth = 8;
        int scrollbarX = bounds.Right - scrollbarWidth - BorderThickness;
        int scrollbarY = bounds.Y + BorderThickness;
        int scrollbarHeight = bounds.Height - BorderThickness * 2;

        // Draw scrollbar track
        Rectangle trackRect = new Rectangle(scrollbarX, scrollbarY, scrollbarWidth, scrollbarHeight);
        spriteBatch.Draw(pixel, trackRect, new Color(60, 60, 60) * Alpha);

        // Draw scrollbar thumb
        float thumbHeight = scrollbarHeight * ((float)_visibleItemCount / _items.Count);
        float thumbPosition = scrollbarHeight * ((float)_scrollOffset / _items.Count);

        Rectangle thumbRect = new Rectangle(
            scrollbarX,
            scrollbarY + (int)thumbPosition,
            scrollbarWidth,
            (int)MathHelper.Max(20, thumbHeight)
        );

        spriteBatch.Draw(pixel, thumbRect, theme.AccentColor * Alpha);
    }

    public override bool HandleInput(Vector2 mousePosition, bool mouseClicked)
    {
        if (!Enabled || !Visible) return false;

        Rectangle contentArea = ContentBounds;
        bool wasHovered = IsHovered;
        IsHovered = Bounds.Contains(mousePosition);

        if (IsHovered)
        {
            // Check if clicking on an item
            if (mouseClicked && contentArea.Contains(mousePosition))
            {
                int relativeY = (int)mousePosition.Y - contentArea.Y;
                int clickedIndex = _scrollOffset + (relativeY / ItemHeight);

                if (clickedIndex >= 0 && clickedIndex < _items.Count)
                {
                    if (clickedIndex == _selectedIndex)
                    {
                        // Double-click or activate
                        OnItemActivated?.Invoke(this, clickedIndex);
                    }
                    else
                    {
                        SelectedIndex = clickedIndex;
                    }
                    return true;
                }
            }
        }

        return false;
    }

    // List management methods
    public void AddItem(string item)
    {
        _items.Add(item);
    }

    public void InsertItem(int index, string item)
    {
        _items.Insert(index, item);
    }

    public void RemoveItem(int index)
    {
        if (index >= 0 && index < _items.Count)
        {
            _items.RemoveAt(index);

            if (_selectedIndex >= _items.Count)
            {
                _selectedIndex = _items.Count - 1;
            }
        }
    }

    public void ClearItems()
    {
        _items.Clear();
        _selectedIndex = -1;
        _scrollOffset = 0;
    }

    public void SetItems(IEnumerable<string> items)
    {
        _items.Clear();
        _items.AddRange(items);
        _selectedIndex = -1;
        _scrollOffset = 0;
    }

    // Navigation methods
    public void SelectNext()
    {
        if (_items.Count > 0)
        {
            SelectedIndex = (_selectedIndex + 1) % _items.Count;
        }
    }

    public void SelectPrevious()
    {
        if (_items.Count > 0)
        {
            SelectedIndex = _selectedIndex <= 0 ? _items.Count - 1 : _selectedIndex - 1;
        }
    }

    public void ScrollUp()
    {
        _scrollOffset = Math.Max(0, _scrollOffset - 1);
    }

    public void ScrollDown()
    {
        _scrollOffset = Math.Min(_items.Count - _visibleItemCount, _scrollOffset + 1);
    }
}
