using Microsoft.Xna.Framework;
using System;

namespace TheMythalProphecy.Game.Entities.Components;

/// <summary>
/// Component that handles entity position, rotation, and facing direction
/// </summary>
public class TransformComponent : IComponent
{
    public Entity Owner { get; set; }
    public bool Enabled { get; set; } = true;

    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public Direction Facing { get; set; } = Direction.Down;

    public TransformComponent(Vector2 position)
    {
        Position = position;
        Rotation = 0f;
    }

    public TransformComponent(float x, float y) : this(new Vector2(x, y))
    {
    }

    public void Initialize()
    {
        // Nothing to initialize
    }

    public void Update(GameTime gameTime)
    {
        // Transform doesn't need per-frame updates
    }

    /// <summary>
    /// Move the entity by a delta amount
    /// </summary>
    public void Move(Vector2 delta)
    {
        Position += delta;
    }

    /// <summary>
    /// Set facing direction based on movement vector
    /// </summary>
    public void SetFacingFromMovement(Vector2 movement)
    {
        if (movement.LengthSquared() < 0.01f) return;

        // Determine primary direction
        if (Math.Abs(movement.X) > Math.Abs(movement.Y))
        {
            Facing = movement.X > 0 ? Direction.Right : Direction.Left;
        }
        else
        {
            Facing = movement.Y > 0 ? Direction.Down : Direction.Up;
        }
    }
}

/// <summary>
/// Cardinal directions for entity facing
/// </summary>
public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
