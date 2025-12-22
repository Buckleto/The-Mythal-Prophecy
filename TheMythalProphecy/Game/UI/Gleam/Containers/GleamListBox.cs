using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI.Gleam;

/// <summary>
/// Scrollable list box for displaying and selecting from string items.
/// </summary>
public class GleamListBox : GleamElement
{
    private readonly List<string> _items = new();
    private int _selectedIndex = -1;
    private int _scrollOffset;
    private int _visibleItemCount;
    private int _hoveredIndex = -1;

    public int ItemHeight { get; set; } = 30;
    public int Padding { get; set; } = 4;
    public SpriteFont Font { get; set; }

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (_selectedIndex != value && value >= -1 && value < _items.Count)
            {
                _selectedIndex = value;
                OnSelectionChanged?.Invoke(this, _selectedIndex);
                EnsureVisible(_selectedIndex);
            }
        }
    }

    public string SelectedItem => _selectedIndex >= 0 && _selectedIndex < _items.Count ? _items[_selectedIndex] : null;
    public IReadOnlyList<string> Items => _items.AsReadOnly();
    public int ItemCount => _items.Count;

    public event Action<GleamListBox, int> OnSelectionChanged;
    public event Action<GleamListBox, int> OnItemActivated;

    public GleamListBox(Vector2 position, Vector2 size)
    {
        Position = position;
        Size = size;
    }

    public override void Update(GameTime gameTime, GleamRenderer renderer)
    {
        base.Update(gameTime, renderer);

        // Calculate visible items
        Rectangle contentArea = GetContentArea();
        _visibleItemCount = Math.Max(1, contentArea.Height / ItemHeight);
    }

    private Rectangle GetContentArea()
    {
        Rectangle bounds = Bounds;
        return new Rectangle(
            bounds.X + Padding,
            bounds.Y + Padding,
            bounds.Width - Padding * 2,
            bounds.Height - Padding * 2
        );
    }

    private void EnsureVisible(int index)
    {
        if (index < 0) return;

        if (index < _scrollOffset)
        {
            _scrollOffset = index;
        }
        else if (index >= _scrollOffset + _visibleItemCount)
        {
            _scrollOffset = index - _visibleItemCount + 1;
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, GleamRenderer renderer)
    {
        var theme = renderer.Theme;
        Rectangle bounds = Bounds;
        Rectangle contentArea = GetContentArea();

        // Background
        renderer.DrawRect(spriteBatch, bounds, theme.DeepPurple, Alpha);

        // Items
        var font = Font ?? theme.DefaultFont;
        if (font != null && _items.Count > 0)
        {
            int y = contentArea.Y;
            int endIndex = Math.Min(_scrollOffset + _visibleItemCount, _items.Count);

            for (int i = _scrollOffset; i < endIndex; i++)
            {
                Rectangle itemBounds = new Rectangle(contentArea.X, y, contentArea.Width, ItemHeight);

                // Item background
                if (i == _selectedIndex)
                {
                    renderer.DrawRect(spriteBatch, itemBounds, theme.MidPurple, Alpha);
                }
                else if (i == _hoveredIndex)
                {
                    renderer.DrawRect(spriteBatch, itemBounds, theme.MutedPurple, Alpha);
                }

                // Item text
                Vector2 textPos = new Vector2(itemBounds.X + Padding, itemBounds.Y + (ItemHeight - font.LineSpacing) / 2f);
                Color textColor = i == _selectedIndex ? theme.GoldBright : theme.TextPrimary;
                renderer.DrawText(spriteBatch, font, _items[i], textPos, textColor, true, Alpha);

                y += ItemHeight;
            }
        }

        // Border
        Color borderColor = IsFocused ? theme.GoldBright : theme.Gold;
        renderer.DrawRectBorder(spriteBatch, bounds, borderColor, 2, Alpha);

        // Scrollbar
        if (_items.Count > _visibleItemCount)
        {
            DrawScrollbar(spriteBatch, renderer, bounds);
        }
    }

    private void DrawScrollbar(SpriteBatch spriteBatch, GleamRenderer renderer, Rectangle bounds)
    {
        var theme = renderer.Theme;
        int scrollbarWidth = 6;
        int scrollbarX = bounds.Right - scrollbarWidth - Padding;
        int scrollbarY = bounds.Y + Padding;
        int scrollbarHeight = bounds.Height - Padding * 2;

        // Track
        Rectangle track = new Rectangle(scrollbarX, scrollbarY, scrollbarWidth, scrollbarHeight);
        renderer.DrawRect(spriteBatch, track, theme.DarkPurple, Alpha);

        // Thumb
        float thumbHeight = Math.Max(20f, scrollbarHeight * ((float)_visibleItemCount / _items.Count));
        int maxScroll = Math.Max(1, _items.Count - _visibleItemCount);
        float thumbY = scrollbarY + (scrollbarHeight - thumbHeight) * (_scrollOffset / (float)maxScroll);

        Rectangle thumb = new Rectangle(scrollbarX, (int)thumbY, scrollbarWidth, (int)thumbHeight);
        renderer.DrawRect(spriteBatch, thumb, theme.Gold, Alpha);
    }

    public override bool HandleInput(Vector2 mousePosition, bool mouseDown, bool mouseClicked)
    {
        if (!Enabled || !Visible) return false;

        Rectangle bounds = Bounds;
        Rectangle contentArea = GetContentArea();

        IsHovered = bounds.Contains(mousePosition);
        _hoveredIndex = -1;

        if (IsHovered && contentArea.Contains(mousePosition))
        {
            // Calculate hovered item
            int relativeY = (int)mousePosition.Y - contentArea.Y;
            int hoverIndex = _scrollOffset + (relativeY / ItemHeight);

            if (hoverIndex >= 0 && hoverIndex < _items.Count)
            {
                _hoveredIndex = hoverIndex;

                if (mouseClicked)
                {
                    if (hoverIndex == _selectedIndex)
                    {
                        OnItemActivated?.Invoke(this, hoverIndex);
                    }
                    else
                    {
                        SelectedIndex = hoverIndex;
                    }
                    return true;
                }
            }
        }

        return false;
    }

    // Item management
    public void AddItem(string item) => _items.Add(item);

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

    // Navigation
    public void SelectNext()
    {
        if (_items.Count > 0)
            SelectedIndex = (_selectedIndex + 1) % _items.Count;
    }

    public void SelectPrevious()
    {
        if (_items.Count > 0)
            SelectedIndex = _selectedIndex <= 0 ? _items.Count - 1 : _selectedIndex - 1;
    }

    public void ScrollUp() => _scrollOffset = Math.Max(0, _scrollOffset - 1);
    public void ScrollDown() => _scrollOffset = Math.Min(Math.Max(0, _items.Count - _visibleItemCount), _scrollOffset + 1);
}
