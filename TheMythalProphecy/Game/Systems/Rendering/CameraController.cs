using Microsoft.Xna.Framework;
using System;

namespace TheMythalProphecy.Game.Systems.Rendering
{
    /// <summary>
    /// Controls camera behavior including following targets, smooth movement, and screen shake
    /// </summary>
    public class CameraController
    {
        private readonly Camera2D _camera;
        private Vector2? _followTarget;
        private float _followSpeed;
        private Vector2 _shakeOffset;
        private float _shakeIntensity;
        private float _shakeDuration;
        private float _shakeElapsed;
        private Random _random;

        /// <summary>
        /// Whether camera is currently following a target
        /// </summary>
        public bool IsFollowing => _followTarget.HasValue;

        /// <summary>
        /// Whether screen shake is active
        /// </summary>
        public bool IsShaking => _shakeElapsed < _shakeDuration;

        /// <summary>
        /// Follow smoothing speed (higher = faster, 0 = instant)
        /// </summary>
        public float FollowSpeed
        {
            get => _followSpeed;
            set => _followSpeed = MathHelper.Max(0, value);
        }

        /// <summary>
        /// Dead zone radius for camera following (camera won't move if target is within this radius)
        /// </summary>
        public float DeadZone { get; set; }

        public CameraController(Camera2D camera)
        {
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));
            _followSpeed = 5.0f;
            _random = new Random();
            DeadZone = 0f;
        }

        /// <summary>
        /// Update camera controller
        /// </summary>
        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update screen shake
            if (IsShaking)
            {
                UpdateScreenShake(deltaTime);
            }
            else
            {
                _shakeOffset = Vector2.Zero;
            }

            // Update follow target
            if (IsFollowing)
            {
                UpdateFollowTarget(deltaTime);
            }

            // Apply shake offset to camera position
            _camera.Position += _shakeOffset;

            // Clamp camera to bounds if set
            if (_camera.Bounds.HasValue)
            {
                _camera.ClampToBounds();
            }
        }

        /// <summary>
        /// Set target position for camera to follow
        /// </summary>
        public void Follow(Vector2 targetPosition)
        {
            _followTarget = targetPosition;
        }

        /// <summary>
        /// Stop following current target
        /// </summary>
        public void StopFollowing()
        {
            _followTarget = null;
        }

        /// <summary>
        /// Instantly move camera to target position
        /// </summary>
        public void SnapToTarget()
        {
            if (_followTarget.HasValue)
            {
                _camera.Position = _followTarget.Value;
            }
        }

        /// <summary>
        /// Start screen shake effect
        /// </summary>
        /// <param name="intensity">Shake intensity (pixel offset)</param>
        /// <param name="duration">Shake duration in seconds</param>
        public void Shake(float intensity, float duration)
        {
            _shakeIntensity = intensity;
            _shakeDuration = duration;
            _shakeElapsed = 0f;
        }

        /// <summary>
        /// Stop screen shake immediately
        /// </summary>
        public void StopShake()
        {
            _shakeElapsed = _shakeDuration;
            _shakeOffset = Vector2.Zero;
        }

        /// <summary>
        /// Move camera smoothly to a position over time
        /// </summary>
        public void MoveTo(Vector2 targetPosition, float speed)
        {
            var previousSpeed = _followSpeed;
            _followSpeed = speed;
            Follow(targetPosition);

            // Will automatically stop following once reached (handled in UpdateFollowTarget)
        }

        private void UpdateFollowTarget(float deltaTime)
        {
            if (!_followTarget.HasValue)
                return;

            Vector2 targetPosition = _followTarget.Value;
            Vector2 currentPosition = _camera.Position;

            // Calculate distance to target
            float distance = Vector2.Distance(currentPosition, targetPosition);

            // Check if within dead zone
            if (distance <= DeadZone)
                return;

            if (_followSpeed <= 0)
            {
                // Instant follow
                _camera.Position = targetPosition;
            }
            else
            {
                // Smooth follow using lerp
                float t = MathHelper.Clamp(_followSpeed * deltaTime, 0f, 1f);
                Vector2 newPosition = Vector2.Lerp(currentPosition, targetPosition, t);
                _camera.Position = newPosition;

                // Stop following if very close (to prevent oscillation)
                if (Vector2.Distance(newPosition, targetPosition) < 0.5f)
                {
                    _camera.Position = targetPosition;
                }
            }
        }

        private void UpdateScreenShake(float deltaTime)
        {
            _shakeElapsed += deltaTime;

            if (_shakeElapsed >= _shakeDuration)
            {
                _shakeOffset = Vector2.Zero;
                return;
            }

            // Calculate shake with decay over time
            float progress = _shakeElapsed / _shakeDuration;
            float decayFactor = 1.0f - progress;
            float currentIntensity = _shakeIntensity * decayFactor;

            // Generate random offset
            float offsetX = ((float)_random.NextDouble() * 2 - 1) * currentIntensity;
            float offsetY = ((float)_random.NextDouble() * 2 - 1) * currentIntensity;

            _shakeOffset = new Vector2(offsetX, offsetY);
        }

        /// <summary>
        /// Set camera bounds for limiting movement
        /// </summary>
        public void SetBounds(Rectangle bounds)
        {
            _camera.Bounds = bounds;
        }

        /// <summary>
        /// Clear camera bounds
        /// </summary>
        public void ClearBounds()
        {
            _camera.Bounds = null;
        }

        /// <summary>
        /// Set camera zoom with optional smooth transition
        /// </summary>
        public void SetZoom(float zoom, bool smooth = false)
        {
            if (smooth)
            {
                // TODO: Implement smooth zoom using tween
                _camera.Zoom = zoom;
            }
            else
            {
                _camera.Zoom = zoom;
            }
        }
    }
}
