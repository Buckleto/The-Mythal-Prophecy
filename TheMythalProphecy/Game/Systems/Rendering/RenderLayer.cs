namespace TheMythalProphecy.Game.Systems.Rendering
{
    /// <summary>
    /// Defines rendering layer depths for proper draw order
    /// Lower values are drawn first (behind), higher values drawn last (front)
    /// </summary>
    public enum RenderLayer
    {
        /// <summary>
        /// Background elements (sky, distant parallax layers)
        /// </summary>
        Background = 0,

        /// <summary>
        /// Far background (mountains, clouds)
        /// </summary>
        BackgroundFar = 100,

        /// <summary>
        /// Mid background (trees, ruins)
        /// </summary>
        BackgroundMid = 200,

        /// <summary>
        /// Near background (foreground decorations)
        /// </summary>
        BackgroundNear = 300,

        /// <summary>
        /// Ground/terrain tiles
        /// </summary>
        Ground = 400,

        /// <summary>
        /// Shadows and ground effects
        /// </summary>
        Shadows = 450,

        /// <summary>
        /// Game entities (characters, enemies, objects)
        /// </summary>
        Entities = 500,

        /// <summary>
        /// Effects and particles above entities
        /// </summary>
        Effects = 600,

        /// <summary>
        /// Foreground decorations (grass, bushes in front)
        /// </summary>
        Foreground = 700,

        /// <summary>
        /// UI elements (HUD, health bars)
        /// </summary>
        UI = 800,

        /// <summary>
        /// Overlays (menus, dialogs)
        /// </summary>
        UIOverlay = 900,

        /// <summary>
        /// Debug rendering (collision boxes, grid, etc.)
        /// </summary>
        Debug = 1000
    }

    /// <summary>
    /// Helper class for working with render layers
    /// </summary>
    public static class RenderLayerHelper
    {
        /// <summary>
        /// Convert layer to normalized depth (0.0 to 1.0) for SpriteBatch
        /// </summary>
        public static float ToDepth(RenderLayer layer)
        {
            // Normalize to 0.0-1.0 range
            return (int)layer / 1000f;
        }

        /// <summary>
        /// Convert layer with offset to normalized depth
        /// </summary>
        public static float ToDepth(RenderLayer layer, int offset)
        {
            return ((int)layer + offset) / 1000f;
        }

        /// <summary>
        /// Get depth value for isometric rendering based on Y position
        /// Entities with higher Y values are drawn behind those with lower Y values
        /// </summary>
        public static float GetIsometricDepth(RenderLayer baseLayer, float yPosition, float maxY = 10000f)
        {
            // Get base depth for the layer
            float baseDepth = ToDepth(baseLayer);

            // Add Y-based offset (normalized to 0.0-0.09 range to stay within layer)
            float yOffset = (yPosition / maxY) * 0.09f;

            return baseDepth + yOffset;
        }
    }
}
