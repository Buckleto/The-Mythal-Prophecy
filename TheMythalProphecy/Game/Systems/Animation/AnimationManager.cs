using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TheMythalProphecy.Game.Systems.Animation
{
    /// <summary>
    /// Global manager for animation definitions and playback
    /// Handles loading, caching, and creating animations
    /// </summary>
    public class AnimationManager
    {
        private readonly ContentManager _content;
        private readonly Dictionary<string, AnimationDefinition> _animationDefinitions;
        private readonly Dictionary<string, Texture2D> _spriteSheetCache;

        public AnimationManager(ContentManager content)
        {
            _content = content ?? throw new ArgumentNullException(nameof(content));
            _animationDefinitions = new Dictionary<string, AnimationDefinition>();
            _spriteSheetCache = new Dictionary<string, Texture2D>();
        }

        /// <summary>
        /// Register an animation definition
        /// </summary>
        public void RegisterAnimation(AnimationDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            _animationDefinitions[definition.Name] = definition;
        }

        /// <summary>
        /// Create an animation definition from a sprite sheet
        /// </summary>
        public AnimationDefinition CreateAnimation(
            string name,
            string spriteSheetPath,
            int frameWidth,
            int frameHeight,
            int frameCount,
            float frameDuration = 0.1f,
            bool loop = true,
            int framesPerRow = 0,
            Point? startFrame = null)
        {
            var spriteSheet = LoadSpriteSheet(spriteSheetPath);

            var definition = new AnimationDefinition(
                name,
                spriteSheet,
                frameWidth,
                frameHeight,
                frameCount,
                frameDuration,
                loop,
                framesPerRow,
                startFrame
            );

            RegisterAnimation(definition);
            return definition;
        }

        /// <summary>
        /// Create a horizontal strip animation
        /// </summary>
        public AnimationDefinition CreateHorizontalStripAnimation(
            string name,
            string spriteSheetPath,
            int frameCount,
            float frameDuration = 0.1f,
            bool loop = true)
        {
            var spriteSheet = LoadSpriteSheet(spriteSheetPath);
            var definition = AnimationDefinition.CreateHorizontalStrip(
                name,
                spriteSheet,
                frameCount,
                frameDuration,
                loop
            );

            RegisterAnimation(definition);
            return definition;
        }

        /// <summary>
        /// Create a vertical strip animation
        /// </summary>
        public AnimationDefinition CreateVerticalStripAnimation(
            string name,
            string spriteSheetPath,
            int frameCount,
            float frameDuration = 0.1f,
            bool loop = true)
        {
            var spriteSheet = LoadSpriteSheet(spriteSheetPath);
            var definition = AnimationDefinition.CreateVerticalStrip(
                name,
                spriteSheet,
                frameCount,
                frameDuration,
                loop
            );

            RegisterAnimation(definition);
            return definition;
        }

        /// <summary>
        /// Create a grid-based animation
        /// </summary>
        public AnimationDefinition CreateGridAnimation(
            string name,
            string spriteSheetPath,
            int frameWidth,
            int frameHeight,
            int frameCount,
            float frameDuration = 0.1f,
            bool loop = true,
            Point? startFrame = null)
        {
            var spriteSheet = LoadSpriteSheet(spriteSheetPath);
            var definition = AnimationDefinition.CreateGrid(
                name,
                spriteSheet,
                frameWidth,
                frameHeight,
                frameCount,
                frameDuration,
                loop,
                startFrame
            );

            RegisterAnimation(definition);
            return definition;
        }

        /// <summary>
        /// Get a registered animation definition by name
        /// </summary>
        public AnimationDefinition GetDefinition(string name)
        {
            return _animationDefinitions.TryGetValue(name, out var definition) ? definition : null;
        }

        /// <summary>
        /// Create a new animation instance from a registered definition
        /// </summary>
        public Animation CreateAnimationInstance(string definitionName)
        {
            var definition = GetDefinition(definitionName);
            if (definition == null)
                throw new InvalidOperationException($"Animation definition '{definitionName}' not found");

            return new Animation(definition);
        }

        /// <summary>
        /// Check if an animation definition exists
        /// </summary>
        public bool HasDefinition(string name)
        {
            return _animationDefinitions.ContainsKey(name);
        }

        /// <summary>
        /// Load a sprite sheet texture (cached)
        /// </summary>
        private Texture2D LoadSpriteSheet(string path)
        {
            if (_spriteSheetCache.TryGetValue(path, out var cachedTexture))
                return cachedTexture;

            var texture = _content.Load<Texture2D>(path);
            _spriteSheetCache[path] = texture;
            return texture;
        }

        /// <summary>
        /// Clear all cached sprite sheets
        /// </summary>
        public void ClearCache()
        {
            _spriteSheetCache.Clear();
        }

        /// <summary>
        /// Remove a specific animation definition
        /// </summary>
        public bool RemoveDefinition(string name)
        {
            return _animationDefinitions.Remove(name);
        }

        /// <summary>
        /// Get all registered animation definition names
        /// </summary>
        public IEnumerable<string> GetAllDefinitionNames()
        {
            return _animationDefinitions.Keys;
        }

        /// <summary>
        /// Clear all animation definitions
        /// </summary>
        public void ClearDefinitions()
        {
            _animationDefinitions.Clear();
        }
    }
}
