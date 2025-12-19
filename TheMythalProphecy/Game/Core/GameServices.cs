using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheMythalProphecy.Game.Systems.Input;

namespace TheMythalProphecy.Game.Core;

/// <summary>
/// Service locator for global game systems
/// Provides centralized access to managers and services
/// </summary>
public static class GameServices
{
    public static InputManager Input { get; private set; }
    public static ContentManager Content { get; private set; }
    public static GraphicsDevice GraphicsDevice { get; private set; }

    /// <summary>
    /// Initialize the game services
    /// </summary>
    public static void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
    {
        Content = content;
        GraphicsDevice = graphicsDevice;
        Input = new InputManager();
    }

    /// <summary>
    /// Update all services that require per-frame updates
    /// </summary>
    public static void Update()
    {
        Input?.Update();
    }
}
