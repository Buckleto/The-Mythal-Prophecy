using Microsoft.Xna.Framework;

namespace TheMythalProphecy.Game.Entities.Components;

/// <summary>
/// Component that handles entity movement behavior
/// </summary>
public class MovementComponent : IComponent
{
    public Entity Owner { get; set; }
    public bool Enabled { get; set; } = true;

    public float Speed { get; set; }
    public Vector2 Velocity { get; set; }
    public bool IsMoving => Velocity.LengthSquared() > 0.01f;

    private TransformComponent _transform;

    public MovementComponent(float speed = 100f)
    {
        Speed = speed;
        Velocity = Vector2.Zero;
    }

    public void Initialize()
    {
        _transform = Owner.GetComponent<TransformComponent>();
    }

    public void Update(GameTime gameTime)
    {
        if (!Enabled || _transform == null) return;

        // Apply velocity to position
        if (Velocity.LengthSquared() > 0.01f)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 movement = Velocity * deltaTime;
            _transform.Move(movement);

            // Update facing direction based on movement
            _transform.SetFacingFromMovement(Velocity);
        }
    }

    /// <summary>
    /// Move in a direction (will be normalized)
    /// </summary>
    public void Move(Vector2 direction)
    {
        if (direction.LengthSquared() > 0.01f)
        {
            direction.Normalize();
            Velocity = direction * Speed;
        }
        else
        {
            Velocity = Vector2.Zero;
        }
    }

    /// <summary>
    /// Stop all movement
    /// </summary>
    public void Stop()
    {
        Velocity = Vector2.Zero;
    }

    /// <summary>
    /// Move towards a target position
    /// </summary>
    public void MoveTowards(Vector2 targetPosition, float stopDistance = 1f)
    {
        if (_transform == null) return;

        Vector2 direction = targetPosition - _transform.Position;
        float distance = direction.Length();

        if (distance > stopDistance)
        {
            direction.Normalize();
            Velocity = direction * Speed;
        }
        else
        {
            Stop();
        }
    }
}
