using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TheMythalProphecy.Game.Systems.Rendering
{
    /// <summary>
    /// Delegate for custom render actions
    /// </summary>
    public delegate void RenderAction(SpriteBatch spriteBatch);

    /// <summary>
    /// Central manager for all rendering operations
    /// Coordinates cameras, layers, and rendering order
    /// </summary>
    public class RenderManager
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;
        private readonly Dictionary<RenderLayer, List<RenderAction>> _renderActions;
        private readonly List<RenderLayer> _sortedLayers;

        /// <summary>
        /// Main camera for world rendering
        /// </summary>
        public Camera2D Camera { get; private set; }

        /// <summary>
        /// Camera controller for following and effects
        /// </summary>
        public CameraController CameraController { get; private set; }

        /// <summary>
        /// Isometric renderer for entities
        /// </summary>
        public IsometricRenderer IsometricRenderer { get; private set; }

        /// <summary>
        /// Background color for clearing the screen
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// Whether to use camera transform for UI layer (usually false)
        /// </summary>
        public bool UseCameraForUI { get; set; }

        public RenderManager(GraphicsDevice graphicsDevice, Viewport viewport)
        {
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            _renderActions = new Dictionary<RenderLayer, List<RenderAction>>();
            _sortedLayers = new List<RenderLayer>();

            // Initialize camera
            Camera = new Camera2D(viewport);
            CameraController = new CameraController(Camera);

            // Initialize isometric renderer
            IsometricRenderer = new IsometricRenderer(RenderLayer.Entities);

            BackgroundColor = Color.Black;
            UseCameraForUI = false;

            // Initialize render action lists for all layers
            foreach (RenderLayer layer in Enum.GetValues(typeof(RenderLayer)))
            {
                _renderActions[layer] = new List<RenderAction>();
            }
        }

        /// <summary>
        /// Update the render manager (camera, effects, etc.)
        /// </summary>
        public void Update(GameTime gameTime)
        {
            CameraController.Update(gameTime);
        }

        /// <summary>
        /// Register a render action for a specific layer
        /// </summary>
        public void RegisterRenderAction(RenderLayer layer, RenderAction action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (!_renderActions.ContainsKey(layer))
            {
                _renderActions[layer] = new List<RenderAction>();
            }

            _renderActions[layer].Add(action);
        }

        /// <summary>
        /// Clear all registered render actions for a specific layer
        /// </summary>
        public void ClearRenderActions(RenderLayer layer)
        {
            if (_renderActions.ContainsKey(layer))
            {
                _renderActions[layer].Clear();
            }
        }

        /// <summary>
        /// Clear all registered render actions
        /// </summary>
        public void ClearAllRenderActions()
        {
            foreach (var layer in _renderActions.Keys)
            {
                _renderActions[layer].Clear();
            }
        }

        /// <summary>
        /// Main render method - draws all registered render actions in layer order
        /// </summary>
        public void Draw()
        {
            // Clear screen
            _graphicsDevice.Clear(BackgroundColor);

            // Get visible area for culling
            var visibleArea = Camera.GetVisibleArea();

            // Draw each layer in order
            foreach (RenderLayer layer in Enum.GetValues(typeof(RenderLayer)))
            {
                if (!_renderActions.ContainsKey(layer) || _renderActions[layer].Count == 0)
                    continue;

                // Determine if we should use camera transform
                bool useCamera = layer < RenderLayer.UI || UseCameraForUI;
                Matrix? transform = useCamera ? Camera.Transform : null;

                // Begin sprite batch for this layer
                _spriteBatch.Begin(
                    sortMode: SpriteSortMode.FrontToBack,
                    blendState: BlendState.AlphaBlend,
                    samplerState: SamplerState.PointClamp,
                    depthStencilState: null,
                    rasterizerState: null,
                    effect: null,
                    transformMatrix: transform
                );

                // Execute all render actions for this layer
                foreach (var action in _renderActions[layer])
                {
                    action(_spriteBatch);
                }

                _spriteBatch.End();
            }
        }

        /// <summary>
        /// Begin a custom sprite batch with camera transform
        /// </summary>
        public void BeginWorld(
            SpriteSortMode sortMode = SpriteSortMode.FrontToBack,
            BlendState blendState = null,
            SamplerState samplerState = null)
        {
            _spriteBatch.Begin(
                sortMode: sortMode,
                blendState: blendState ?? BlendState.AlphaBlend,
                samplerState: samplerState ?? SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: Camera.Transform
            );
        }

        /// <summary>
        /// Begin a custom sprite batch without camera transform (for UI)
        /// </summary>
        public void BeginUI(
            SpriteSortMode sortMode = SpriteSortMode.Deferred,
            BlendState blendState = null,
            SamplerState samplerState = null)
        {
            _spriteBatch.Begin(
                sortMode: sortMode,
                blendState: blendState ?? BlendState.AlphaBlend,
                samplerState: samplerState ?? SamplerState.LinearClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: null
            );
        }

        /// <summary>
        /// End the current sprite batch
        /// </summary>
        public void End()
        {
            _spriteBatch.End();
        }

        /// <summary>
        /// Get the sprite batch for custom rendering
        /// </summary>
        public SpriteBatch GetSpriteBatch()
        {
            return _spriteBatch;
        }

        /// <summary>
        /// Draw a texture at a world position
        /// </summary>
        public void DrawWorld(
            Texture2D texture,
            Vector2 position,
            Rectangle? sourceRectangle,
            Color color,
            RenderLayer layer = RenderLayer.Entities,
            float rotation = 0f,
            Vector2? origin = null,
            Vector2? scale = null,
            SpriteEffects effects = SpriteEffects.None)
        {
            float depth = RenderLayerHelper.ToDepth(layer);

            RegisterRenderAction(layer, (spriteBatch) =>
            {
                spriteBatch.Draw(
                    texture,
                    position,
                    sourceRectangle,
                    color,
                    rotation,
                    origin ?? Vector2.Zero,
                    scale ?? Vector2.One,
                    effects,
                    depth
                );
            });
        }

        /// <summary>
        /// Draw a texture at a screen position (UI)
        /// </summary>
        public void DrawUI(
            Texture2D texture,
            Vector2 position,
            Rectangle? sourceRectangle,
            Color color,
            RenderLayer layer = RenderLayer.UI,
            float rotation = 0f,
            Vector2? origin = null,
            Vector2? scale = null,
            SpriteEffects effects = SpriteEffects.None)
        {
            float depth = RenderLayerHelper.ToDepth(layer);

            RegisterRenderAction(layer, (spriteBatch) =>
            {
                spriteBatch.Draw(
                    texture,
                    position,
                    sourceRectangle,
                    color,
                    rotation,
                    origin ?? Vector2.Zero,
                    scale ?? Vector2.One,
                    effects,
                    depth
                );
            });
        }

        /// <summary>
        /// Update viewport (call when window is resized)
        /// </summary>
        public void UpdateViewport(Viewport viewport)
        {
            Camera.Viewport = viewport;
        }
    }
}
