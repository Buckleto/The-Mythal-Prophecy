using Microsoft.Xna.Framework;

namespace TheMythalProphecy.Game.Entities.Components;

/// <summary>
/// Interface for all entity components
/// Components add functionality to entities through composition
/// </summary>
public interface IComponent
{
    /// <summary>
    /// The entity this component is attached to
    /// </summary>
    Entity Owner { get; set; }

    /// <summary>
    /// Whether this component is currently active
    /// </summary>
    bool Enabled { get; set; }

    /// <summary>
    /// Initialize the component (called when added to entity)
    /// </summary>
    void Initialize();

    /// <summary>
    /// Update the component each frame
    /// </summary>
    void Update(GameTime gameTime);
}
