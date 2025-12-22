using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI.Gleam;

/// <summary>
/// Grid layout container for items like inventory slots.
/// Supports selection and keyboard navigation.
/// </summary>
public class GleamGrid : GleamElement
{
    private readonly List<GleamElement> _items = new();
    private int _selectedIndex = -1;

    public int Columns { get; set; } = 4;
    public float CellWidth { get; set; } = 64f;
    public float CellHeight { get; set; } = 64f;
    public float Spacing { get; set; } = 8f;
    public int Padding { get; set; } = 4;
    public bool AllowSelection { get; set; } = true;
    public bool DrawBackground { get; set; } = true;

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (AllowSelection && _items.Count > 0)
            {
                int newIndex = Math.Clamp(value, 0, _items.Count - 1);
                if (_selectedIndex != newIndex)
                {
                    _selectedIndex = newIndex;
                    OnSelectionChanged?.Invoke(this, _selectedIndex);
                }
            }
        }
    }

    public GleamElement SelectedItem => AllowSelection && _selectedIndex >= 0 && _selectedIndex < _items.Count
        ? _items[_selectedIndex]
        : null;

    public int ItemCount => _items.Count;
    public int Rows => _items.Count > 0 ? (_items.Count + Columns - 1) / Columns : 0;

    public event Action<GleamGrid, int> OnSelectionChanged;
    public event Action<GleamGrid, int> OnItemActivated;

    public GleamGrid(Vector2 position, Vector2 size)
    {
        Position = position;
        Size = size;
    }

    public void AddItem(GleamElement item)
    {
        _items.Add(item);
        AddChild(item);
        LayoutItems();

        if (AllowSelection && _selectedIndex < 0 && _items.Count > 0)
        {
            _selectedIndex = 0;
        }
    }

    public void RemoveItem(GleamElement item)
    {
        int index = _items.IndexOf(item);
        if (index >= 0)
        {
            _items.RemoveAt(index);
            RemoveChild(item);
            LayoutItems();

            if (_selectedIndex >= _items.Count)
            {
                _selectedIndex = Math.Max(-1, _items.Count - 1);
            }
        }
    }

    public void ClearItems()
    {
        foreach (var item in _items)
        {
            RemoveChild(item);
        }
        _items.Clear();
        _selectedIndex = -1;
    }

    public GleamElement GetItem(int index)
    {
        return index >= 0 && index < _items.Count ? _items[index] : null;
    }

    private void LayoutItems()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            int row = i / Columns;
            int col = i % Columns;

            float itemX = Padding + col * (CellWidth + Spacing);
            float itemY = Padding + row * (CellHeight + Spacing);

            var item = _items[i];
            item.Position = new Vector2(itemX, itemY);
            item.Size = new Vector2(CellWidth, CellHeight);
        }

        // Auto-size to fit content
        if (_items.Count > 0)
        {
            int rows = Rows;
            float totalWidth = Padding * 2 + Columns * CellWidth + (Columns - 1) * Spacing;
            float totalHeight = Padding * 2 + rows * CellHeight + (rows - 1) * Spacing;
            Size = new Vector2(Math.Max(Size.X, totalWidth), Math.Max(Size.Y, totalHeight));
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, GleamRenderer renderer)
    {
        var theme = renderer.Theme;
        Rectangle bounds = Bounds;

        // Background
        if (DrawBackground)
        {
            renderer.DrawRect(spriteBatch, bounds, theme.DeepPurple, Alpha * 0.5f);
            renderer.DrawRectBorder(spriteBatch, bounds, theme.Gold, 2, Alpha * 0.5f);
        }

        // Selection highlight
        if (AllowSelection && _selectedIndex >= 0 && _selectedIndex < _items.Count)
        {
            var selectedItem = _items[_selectedIndex];
            Rectangle highlight = new Rectangle(
                selectedItem.Bounds.X - 2,
                selectedItem.Bounds.Y - 2,
                selectedItem.Bounds.Width + 4,
                selectedItem.Bounds.Height + 4
            );
            renderer.DrawRectBorder(spriteBatch, highlight, theme.GoldBright, 2, Alpha);
        }
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

        // Check if clicking on an item
        if (mouseClicked && AllowSelection)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Bounds.Contains(mousePosition))
                {
                    if (i == _selectedIndex)
                    {
                        OnItemActivated?.Invoke(this, i);
                    }
                    else
                    {
                        SelectedIndex = i;
                    }
                    return true;
                }
            }
        }

        return false;
    }

    // Navigation helpers
    public void NavigateUp()
    {
        if (_selectedIndex >= Columns)
            SelectedIndex -= Columns;
    }

    public void NavigateDown()
    {
        if (_selectedIndex + Columns < _items.Count)
            SelectedIndex += Columns;
    }

    public void NavigateLeft()
    {
        if (_selectedIndex > 0)
            SelectedIndex--;
    }

    public void NavigateRight()
    {
        if (_selectedIndex < _items.Count - 1)
            SelectedIndex++;
    }

    public void ActivateSelected()
    {
        if (_selectedIndex >= 0)
            OnItemActivated?.Invoke(this, _selectedIndex);
    }
}
