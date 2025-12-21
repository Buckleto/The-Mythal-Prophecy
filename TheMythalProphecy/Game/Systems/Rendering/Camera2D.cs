using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheMythalProphecy.Game.Systems.Rendering
{
    /// <summary>
    /// 2D camera with position, zoom, rotation, and view matrix calculation
    /// </summary>
    public class Camera2D
    {
        private Vector2 _position;
        private float _zoom;
        private float _rotation;
        private Viewport _viewport;
        private Matrix _transform;
        private bool _transformDirty;

        /// <summary>
        /// Camera position in world space
        /// </summary>
        public Vector2 Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    _transformDirty = true;
                }
            }
        }

        /// <summary>
        /// Camera zoom level (1.0 = normal, >1.0 = zoomed in, <1.0 = zoomed out)
        /// </summary>
        public float Zoom
        {
            get => _zoom;
            set
            {
                _zoom = MathHelper.Clamp(value, MinZoom, MaxZoom);
                _transformDirty = true;
            }
        }

        /// <summary>
        /// Camera rotation in radians
        /// </summary>
        public float Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                _transformDirty = true;
            }
        }

        /// <summary>
        /// Viewport the camera renders to
        /// </summary>
        public Viewport Viewport
        {
            get => _viewport;
            set
            {
                _viewport = value;
                _transformDirty = true;
            }
        }

        /// <summary>
        /// Minimum zoom level
        /// </summary>
        public float MinZoom { get; set; } = 0.1f;

        /// <summary>
        /// Maximum zoom level
        /// </summary>
        public float MaxZoom { get; set; } = 10.0f;

        /// <summary>
        /// Camera bounds (optional, for limiting camera movement)
        /// </summary>
        public Rectangle? Bounds { get; set; }

        /// <summary>
        /// Camera center in screen space
        /// </summary>
        public Vector2 Center => new Vector2(_viewport.Width / 2f, _viewport.Height / 2f);

        /// <summary>
        /// Get the transformation matrix for rendering
        /// </summary>
        public Matrix Transform
        {
            get
            {
                if (_transformDirty)
                {
                    RecalculateTransform();
                    _transformDirty = false;
                }
                return _transform;
            }
        }

        public Camera2D(Viewport viewport)
        {
            _viewport = viewport;
            _position = Vector2.Zero;
            _zoom = 1.0f;
            _rotation = 0f;
            _transformDirty = true;
            Bounds = null;
        }

        /// <summary>
        /// Move camera by offset
        /// </summary>
        public void Move(Vector2 offset)
        {
            Position += offset;
        }

        /// <summary>
        /// Look at a specific world position
        /// </summary>
        public void LookAt(Vector2 position)
        {
            Position = position;
        }

        /// <summary>
        /// Convert screen coordinates to world coordinates
        /// </summary>
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(Transform));
        }

        /// <summary>
        /// Convert world coordinates to screen coordinates
        /// </summary>
        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, Transform);
        }

        /// <summary>
        /// Get the visible area in world space
        /// </summary>
        public Rectangle GetVisibleArea()
        {
            var topLeft = ScreenToWorld(Vector2.Zero);
            var bottomRight = ScreenToWorld(new Vector2(_viewport.Width, _viewport.Height));

            return new Rectangle(
                (int)topLeft.X,
                (int)topLeft.Y,
                (int)(bottomRight.X - topLeft.X),
                (int)(bottomRight.Y - topLeft.Y)
            );
        }

        /// <summary>
        /// Check if a world position is visible to the camera
        /// </summary>
        public bool IsInView(Vector2 worldPosition)
        {
            var visibleArea = GetVisibleArea();
            return visibleArea.Contains(worldPosition);
        }

        /// <summary>
        /// Check if a rectangle is visible to the camera
        /// </summary>
        public bool IsInView(Rectangle worldRectangle)
        {
            var visibleArea = GetVisibleArea();
            return visibleArea.Intersects(worldRectangle);
        }

        /// <summary>
        /// Clamp camera position to bounds
        /// </summary>
        public void ClampToBounds()
        {
            if (!Bounds.HasValue)
                return;

            var visibleArea = GetVisibleArea();
            var bounds = Bounds.Value;

            var cameraMax = new Vector2(
                bounds.Right - (visibleArea.Width / 2f),
                bounds.Bottom - (visibleArea.Height / 2f)
            );

            var cameraMin = new Vector2(
                bounds.Left + (visibleArea.Width / 2f),
                bounds.Top + (visibleArea.Height / 2f)
            );

            _position = Vector2.Clamp(_position, cameraMin, cameraMax);
            _transformDirty = true;
        }

        /// <summary>
        /// Reset camera to default state
        /// </summary>
        public void Reset()
        {
            _position = Vector2.Zero;
            _zoom = 1.0f;
            _rotation = 0f;
            _transformDirty = true;
        }

        private void RecalculateTransform()
        {
            // Create transformation matrix
            // Order: Translation -> Rotation -> Scale -> Center offset
            _transform =
                Matrix.CreateTranslation(new Vector3(-_position.X, -_position.Y, 0)) *
                Matrix.CreateRotationZ(_rotation) *
                Matrix.CreateScale(_zoom, _zoom, 1) *
                Matrix.CreateTranslation(new Vector3(Center.X, Center.Y, 0));
        }
    }
}
