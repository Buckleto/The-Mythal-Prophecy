using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TheMythalProphecy.Game.UI.Components
{
    /// <summary>
    /// Grid layout container for items like inventory slots.
    /// Supports fixed column count, selection, and keyboard/gamepad navigation.
    /// </summary>
    public class UIGrid : UIElement
    {
        private List<UIElement> _items;
        private int _columns;
        private float _cellWidth;
        private float _cellHeight;
        private float _spacing;
        private int _selectedIndex;
        private bool _allowSelection;

        public int Columns
        {
            get => _columns;
            set
            {
                if (_columns != value && value > 0)
                {
                    _columns = value;
                    LayoutItems();
                }
            }
        }

        public float CellWidth
        {
            get => _cellWidth;
            set
            {
                if (_cellWidth != value && value > 0)
                {
                    _cellWidth = value;
                    LayoutItems();
                }
            }
        }

        public float CellHeight
        {
            get => _cellHeight;
            set
            {
                if (_cellHeight != value && value > 0)
                {
                    _cellHeight = value;
                    LayoutItems();
                }
            }
        }

        public float Spacing
        {
            get => _spacing;
            set
            {
                if (_spacing != value)
                {
                    _spacing = value;
                    LayoutItems();
                }
            }
        }

        public bool AllowSelection
        {
            get => _allowSelection;
            set => _allowSelection = value;
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_allowSelection && _items.Count > 0)
                {
                    _selectedIndex = Math.Clamp(value, 0, _items.Count - 1);
                    OnSelectionChanged?.Invoke(_selectedIndex);
                }
            }
        }

        public UIElement SelectedItem => _allowSelection && _selectedIndex >= 0 && _selectedIndex < _items.Count
            ? _items[_selectedIndex]
            : null;

        public int ItemCount => _items.Count;
        public int Rows => (_items.Count + _columns - 1) / _columns;

        public event Action<int> OnSelectionChanged;
        public event Action<int> OnItemActivated;

        public UIGrid()
        {
            _items = new List<UIElement>();
            _columns = 4;
            _cellWidth = 64f;
            _cellHeight = 64f;
            _spacing = 8f;
            _selectedIndex = -1;
            _allowSelection = true;
            SetPadding(4);
        }

        public UIGrid(Vector2 position, Vector2 size, int columns = 4, float cellWidth = 64f, float cellHeight = 64f) : this()
        {
            Position = position;
            Size = size;
            _columns = columns;
            _cellWidth = cellWidth;
            _cellHeight = cellHeight;
        }

        public void AddItem(UIElement item)
        {
            _items.Add(item);
            AddChild(item);
            LayoutItems();

            if (_allowSelection && _selectedIndex < 0 && _items.Count > 0)
            {
                _selectedIndex = 0;
            }
        }

        public void RemoveItem(UIElement item)
        {
            int index = _items.IndexOf(item);
            if (index >= 0)
            {
                _items.RemoveAt(index);
                RemoveChild(item);
                LayoutItems();

                // Adjust selection
                if (_selectedIndex >= _items.Count)
                {
                    _selectedIndex = Math.Max(0, _items.Count - 1);
                }
            }
        }

        public void RemoveItemAt(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                var item = _items[index];
                RemoveItem(item);
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
            LayoutItems();
        }

        public UIElement GetItem(int index)
        {
            return index >= 0 && index < _items.Count ? _items[index] : null;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_allowSelection && Enabled && _items.Count > 0)
            {
                HandleInput();
            }
        }

        private void HandleInput()
        {
            var input = Core.GameServices.Input;

            // Keyboard/Gamepad navigation
            if (input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                NavigateUp();
            }
            else if (input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                NavigateDown();
            }
            else if (input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                NavigateLeft();
            }
            else if (input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                NavigateRight();
            }

            // Activation (Enter, A button)
            if (input.IsAcceptPressed())
            {
                OnItemActivated?.Invoke(_selectedIndex);
            }

            // TODO: Mouse input handled by base class
        }

        public void NavigateUp()
        {
            if (_selectedIndex >= _columns)
            {
                SelectedIndex -= _columns;
            }
        }

        public void NavigateDown()
        {
            if (_selectedIndex + _columns < _items.Count)
            {
                SelectedIndex += _columns;
            }
        }

        public void NavigateLeft()
        {
            if (_selectedIndex > 0)
            {
                SelectedIndex--;
            }
        }

        public void NavigateRight()
        {
            if (_selectedIndex < _items.Count - 1)
            {
                SelectedIndex++;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch, UITheme theme)
        {
            // Get pixel texture
            Texture2D pixel = Core.GameServices.UI?.PixelTexture;
            if (pixel == null) return;

            // Draw background
            if (theme.PanelTexture != null)
            {
                spriteBatch.Draw(theme.PanelTexture, Bounds, Color.White * 0.5f);
            }
            else
            {
                spriteBatch.Draw(pixel, Bounds, theme.PrimaryColor * 0.5f);
            }

            // Draw selection highlight
            if (_allowSelection && _selectedIndex >= 0 && _selectedIndex < _items.Count)
            {
                var selectedItem = _items[_selectedIndex];
                Rectangle highlightRect = new Rectangle(
                    selectedItem.Bounds.X - 2,
                    selectedItem.Bounds.Y - 2,
                    selectedItem.Bounds.Width + 4,
                    selectedItem.Bounds.Height + 4
                );
                spriteBatch.Draw(pixel, highlightRect, theme.AccentColor);
            }
        }

        private void LayoutItems()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                int row = i / _columns;
                int col = i % _columns;

                float itemX = col * (_cellWidth + _spacing);
                float itemY = row * (_cellHeight + _spacing);

                var item = _items[i];
                item.Position = new Vector2(itemX, itemY);
                item.Size = new Vector2(_cellWidth, _cellHeight);
            }

            // Update grid bounds to fit all items
            if (_items.Count > 0)
            {
                int rows = Rows;
                float totalWidth = _columns * _cellWidth + (_columns - 1) * _spacing;
                float totalHeight = rows * _cellHeight + (rows - 1) * _spacing;

                // Don't shrink the grid, only expand
                Size = new Vector2(Math.Max(Size.X, totalWidth), Math.Max(Size.Y, totalHeight));
            }
        }
    }
}
