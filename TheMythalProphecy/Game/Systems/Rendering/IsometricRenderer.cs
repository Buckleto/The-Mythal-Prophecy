using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheMythalProphecy.Game.Systems.Rendering
{
    /// <summary>
    /// Represents a renderable object in isometric space
    /// </summary>
    public interface IIsometricRenderable
    {
        Vector2 Position { get; }
        float GetSortingY();
        void Draw(SpriteBatch spriteBatch, float depth);
    }

    /// <summary>
    /// Handles isometric rendering with Y-based depth sorting
    /// </summary>
    public class IsometricRenderer
    {
        private readonly List<IIsometricRenderable> _renderables;
        private readonly List<IIsometricRenderable> _sortedRenderables;
        private bool _needsSort;

        /// <summary>
        /// Base render layer for isometric objects
        /// </summary>
        public RenderLayer BaseLayer { get; set; }

        /// <summary>
        /// Maximum Y value for depth calculation
        /// </summary>
        public float MaxY { get; set; }

        public IsometricRenderer(RenderLayer baseLayer = RenderLayer.Entities, float maxY = 10000f)
        {
            _renderables = new List<IIsometricRenderable>();
            _sortedRenderables = new List<IIsometricRenderable>();
            BaseLayer = baseLayer;
            MaxY = maxY;
            _needsSort = false;
        }

        /// <summary>
        /// Add a renderable object
        /// </summary>
        public void Add(IIsometricRenderable renderable)
        {
            if (renderable == null)
                throw new ArgumentNullException(nameof(renderable));

            _renderables.Add(renderable);
            _needsSort = true;
        }

        /// <summary>
        /// Remove a renderable object
        /// </summary>
        public bool Remove(IIsometricRenderable renderable)
        {
            bool removed = _renderables.Remove(renderable);
            if (removed)
            {
                _needsSort = true;
            }
            return removed;
        }

        /// <summary>
        /// Clear all renderables
        /// </summary>
        public void Clear()
        {
            _renderables.Clear();
            _sortedRenderables.Clear();
            _needsSort = false;
        }

        /// <summary>
        /// Mark that sorting is needed (call when renderable positions change)
        /// </summary>
        public void MarkDirty()
        {
            _needsSort = true;
        }

        /// <summary>
        /// Draw all renderables with isometric depth sorting
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_renderables.Count == 0)
                return;

            // Sort renderables by Y position if needed
            if (_needsSort)
            {
                SortRenderables();
                _needsSort = false;
            }

            // Draw all renderables in sorted order
            for (int i = 0; i < _sortedRenderables.Count; i++)
            {
                var renderable = _sortedRenderables[i];
                float depth = RenderLayerHelper.GetIsometricDepth(BaseLayer, renderable.GetSortingY(), MaxY);
                renderable.Draw(spriteBatch, depth);
            }
        }

        /// <summary>
        /// Draw renderables within a specific camera view (for culling)
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, Rectangle visibleArea)
        {
            if (_renderables.Count == 0)
                return;

            // Sort renderables by Y position if needed
            if (_needsSort)
            {
                SortRenderables();
                _needsSort = false;
            }

            // Draw only visible renderables
            for (int i = 0; i < _sortedRenderables.Count; i++)
            {
                var renderable = _sortedRenderables[i];

                // Simple visibility check (can be improved with proper bounds)
                if (visibleArea.Contains(renderable.Position))
                {
                    float depth = RenderLayerHelper.GetIsometricDepth(BaseLayer, renderable.GetSortingY(), MaxY);
                    renderable.Draw(spriteBatch, depth);
                }
            }
        }

        private void SortRenderables()
        {
            _sortedRenderables.Clear();
            _sortedRenderables.AddRange(_renderables);

            // Sort by Y position (higher Y = further back = drawn first)
            _sortedRenderables.Sort((a, b) => a.GetSortingY().CompareTo(b.GetSortingY()));
        }

        /// <summary>
        /// Get count of renderables
        /// </summary>
        public int Count => _renderables.Count;
    }

    /// <summary>
    /// Helper class for creating simple isometric renderables
    /// </summary>
    public class SimpleIsometricRenderable : IIsometricRenderable
    {
        private readonly Texture2D _texture;
        private readonly Rectangle? _sourceRectangle;
        private readonly Color _color;
        private readonly float _sortingYOffset;

        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }
        public SpriteEffects SpriteEffects { get; set; }

        public SimpleIsometricRenderable(
            Texture2D texture,
            Vector2 position,
            Rectangle? sourceRectangle = null,
            Color? color = null,
            float sortingYOffset = 0f)
        {
            _texture = texture ?? throw new ArgumentNullException(nameof(texture));
            Position = position;
            _sourceRectangle = sourceRectangle;
            _color = color ?? Color.White;
            _sortingYOffset = sortingYOffset;
            Origin = Vector2.Zero;
            Rotation = 0f;
            Scale = Vector2.One;
            SpriteEffects = SpriteEffects.None;
        }

        public float GetSortingY()
        {
            return Position.Y + _sortingYOffset;
        }

        public void Draw(SpriteBatch spriteBatch, float depth)
        {
            spriteBatch.Draw(
                _texture,
                Position,
                _sourceRectangle,
                _color,
                Rotation,
                Origin,
                Scale,
                SpriteEffects,
                depth
            );
        }
    }
}
