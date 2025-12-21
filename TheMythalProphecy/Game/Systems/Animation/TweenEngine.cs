using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace TheMythalProphecy.Game.Systems.Animation
{
    /// <summary>
    /// Easing functions for smooth interpolation
    /// </summary>
    public enum EasingType
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
        EaseInBounce,
        EaseOutBounce,
        EaseInOutBounce
    }

    /// <summary>
    /// Base class for tweens
    /// </summary>
    public abstract class Tween
    {
        protected float _elapsed;
        protected float _duration;
        protected EasingType _easing;
        protected bool _isComplete;

        public bool IsComplete => _isComplete;
        public event Action OnComplete;

        protected Tween(float duration, EasingType easing = EasingType.Linear)
        {
            _duration = duration;
            _easing = easing;
            _elapsed = 0f;
            _isComplete = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (_isComplete)
                return;

            _elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_elapsed >= _duration)
            {
                _elapsed = _duration;
                _isComplete = true;
                OnComplete?.Invoke();
            }
        }

        protected float GetProgress()
        {
            float t = Math.Min(_elapsed / _duration, 1.0f);
            return EasingFunctions.Apply(t, _easing);
        }
    }

    /// <summary>
    /// Tween for float values
    /// </summary>
    public class FloatTween : Tween
    {
        private readonly float _start;
        private readonly float _end;
        private float _current;

        public float Current => _current;

        public FloatTween(float start, float end, float duration, EasingType easing = EasingType.Linear)
            : base(duration, easing)
        {
            _start = start;
            _end = end;
            _current = start;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            float progress = GetProgress();
            _current = MathHelper.Lerp(_start, _end, progress);
        }
    }

    /// <summary>
    /// Tween for Vector2 values
    /// </summary>
    public class Vector2Tween : Tween
    {
        private readonly Vector2 _start;
        private readonly Vector2 _end;
        private Vector2 _current;

        public Vector2 Current => _current;

        public Vector2Tween(Vector2 start, Vector2 end, float duration, EasingType easing = EasingType.Linear)
            : base(duration, easing)
        {
            _start = start;
            _end = end;
            _current = start;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            float progress = GetProgress();
            _current = Vector2.Lerp(_start, _end, progress);
        }
    }

    /// <summary>
    /// Tween for Color values
    /// </summary>
    public class ColorTween : Tween
    {
        private readonly Color _start;
        private readonly Color _end;
        private Color _current;

        public Color Current => _current;

        public ColorTween(Color start, Color end, float duration, EasingType easing = EasingType.Linear)
            : base(duration, easing)
        {
            _start = start;
            _end = end;
            _current = start;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            float progress = GetProgress();
            _current = Color.Lerp(_start, _end, progress);
        }
    }

    /// <summary>
    /// Engine for managing multiple tweens
    /// </summary>
    public class TweenEngine
    {
        private readonly List<Tween> _activeTweens;
        private readonly List<Tween> _tweensToRemove;

        public TweenEngine()
        {
            _activeTweens = new List<Tween>();
            _tweensToRemove = new List<Tween>();
        }

        /// <summary>
        /// Add a tween to be updated
        /// </summary>
        public T AddTween<T>(T tween) where T : Tween
        {
            if (tween == null)
                throw new ArgumentNullException(nameof(tween));

            _activeTweens.Add(tween);
            tween.OnComplete += () => _tweensToRemove.Add(tween);
            return tween;
        }

        /// <summary>
        /// Create and add a float tween
        /// </summary>
        public FloatTween TweenFloat(float start, float end, float duration, EasingType easing = EasingType.Linear)
        {
            var tween = new FloatTween(start, end, duration, easing);
            return AddTween(tween);
        }

        /// <summary>
        /// Create and add a Vector2 tween
        /// </summary>
        public Vector2Tween TweenVector2(Vector2 start, Vector2 end, float duration, EasingType easing = EasingType.Linear)
        {
            var tween = new Vector2Tween(start, end, duration, easing);
            return AddTween(tween);
        }

        /// <summary>
        /// Create and add a Color tween
        /// </summary>
        public ColorTween TweenColor(Color start, Color end, float duration, EasingType easing = EasingType.Linear)
        {
            var tween = new ColorTween(start, end, duration, easing);
            return AddTween(tween);
        }

        /// <summary>
        /// Update all active tweens
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Update all active tweens
            foreach (var tween in _activeTweens)
            {
                tween.Update(gameTime);
            }

            // Remove completed tweens
            if (_tweensToRemove.Count > 0)
            {
                foreach (var tween in _tweensToRemove)
                {
                    _activeTweens.Remove(tween);
                }
                _tweensToRemove.Clear();
            }
        }

        /// <summary>
        /// Clear all tweens
        /// </summary>
        public void Clear()
        {
            _activeTweens.Clear();
            _tweensToRemove.Clear();
        }

        /// <summary>
        /// Get count of active tweens
        /// </summary>
        public int ActiveTweenCount => _activeTweens.Count;
    }

    /// <summary>
    /// Easing function implementations
    /// </summary>
    public static class EasingFunctions
    {
        public static float Apply(float t, EasingType type)
        {
            return type switch
            {
                EasingType.Linear => t,
                EasingType.EaseInQuad => t * t,
                EasingType.EaseOutQuad => t * (2 - t),
                EasingType.EaseInOutQuad => t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t,
                EasingType.EaseInCubic => t * t * t,
                EasingType.EaseOutCubic => (--t) * t * t + 1,
                EasingType.EaseInOutCubic => t < 0.5f ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1,
                EasingType.EaseInQuart => t * t * t * t,
                EasingType.EaseOutQuart => 1 - (--t) * t * t * t,
                EasingType.EaseInOutQuart => t < 0.5f ? 8 * t * t * t * t : 1 - 8 * (--t) * t * t * t,
                EasingType.EaseInExpo => t == 0 ? 0 : MathF.Pow(2, 10 * (t - 1)),
                EasingType.EaseOutExpo => t == 1 ? 1 : 1 - MathF.Pow(2, -10 * t),
                EasingType.EaseInOutExpo => EaseInOutExpo(t),
                EasingType.EaseInElastic => EaseInElastic(t),
                EasingType.EaseOutElastic => EaseOutElastic(t),
                EasingType.EaseInOutElastic => EaseInOutElastic(t),
                EasingType.EaseInBounce => 1 - EaseOutBounce(1 - t),
                EasingType.EaseOutBounce => EaseOutBounce(t),
                EasingType.EaseInOutBounce => t < 0.5f ? (1 - EaseOutBounce(1 - 2 * t)) * 0.5f : (1 + EaseOutBounce(2 * t - 1)) * 0.5f,
                _ => t
            };
        }

        private static float EaseInOutExpo(float t)
        {
            if (t == 0) return 0;
            if (t == 1) return 1;
            if (t < 0.5f)
                return MathF.Pow(2, 20 * t - 10) * 0.5f;
            return (2 - MathF.Pow(2, -20 * t + 10)) * 0.5f;
        }

        private static float EaseInElastic(float t)
        {
            if (t == 0) return 0;
            if (t == 1) return 1;
            return -MathF.Pow(2, 10 * (t - 1)) * MathF.Sin((t - 1.1f) * 5 * MathF.PI);
        }

        private static float EaseOutElastic(float t)
        {
            if (t == 0) return 0;
            if (t == 1) return 1;
            return MathF.Pow(2, -10 * t) * MathF.Sin((t - 0.1f) * 5 * MathF.PI) + 1;
        }

        private static float EaseInOutElastic(float t)
        {
            if (t == 0) return 0;
            if (t == 1) return 1;
            t *= 2;
            if (t < 1)
                return -0.5f * MathF.Pow(2, 10 * (t - 1)) * MathF.Sin((t - 1.1f) * 5 * MathF.PI);
            return MathF.Pow(2, -10 * (t - 1)) * MathF.Sin((t - 1.1f) * 5 * MathF.PI) * 0.5f + 1;
        }

        private static float EaseOutBounce(float t)
        {
            if (t < 1 / 2.75f)
                return 7.5625f * t * t;
            else if (t < 2 / 2.75f)
                return 7.5625f * (t -= 1.5f / 2.75f) * t + 0.75f;
            else if (t < 2.5f / 2.75f)
                return 7.5625f * (t -= 2.25f / 2.75f) * t + 0.9375f;
            else
                return 7.5625f * (t -= 2.625f / 2.75f) * t + 0.984375f;
        }
    }
}
