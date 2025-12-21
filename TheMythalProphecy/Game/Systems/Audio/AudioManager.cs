using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace TheMythalProphecy.Game.Systems.Audio
{
    /// <summary>
    /// Manages all audio playback including background music and sound effects.
    /// Features: music transitions, volume control, crossfading, and SFX pooling.
    /// </summary>
    public class AudioManager
    {
        #region Fields

        // Music state
        private Song _currentMusic;
        private Song _nextMusic;
        private float _musicVolume = 1.0f;
        private float _masterVolume = 1.0f;
        private bool _isMusicEnabled = true;

        // Music transition
        private bool _isTransitioning;
        private float _transitionDuration;
        private float _transitionElapsed;
        private float _fadeOutVolume;
        private float _fadeInVolume;

        // Sound effects
        private Dictionary<string, SoundEffect> _soundEffects;
        private float _sfxVolume = 1.0f;
        private bool _areSfxEnabled = true;

        // SFX instance pooling (for efficient reuse)
        private Dictionary<string, Queue<SoundEffectInstance>> _sfxInstancePools;
        private const int MaxPoolSize = 10;

        #endregion

        #region Properties

        /// <summary>Master volume affecting all audio (0.0 to 1.0)</summary>
        public float MasterVolume
        {
            get => _masterVolume;
            set
            {
                _masterVolume = Math.Clamp(value, 0f, 1f);
                UpdateMusicVolume();
            }
        }

        /// <summary>Music volume (0.0 to 1.0)</summary>
        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = Math.Clamp(value, 0f, 1f);
                UpdateMusicVolume();
            }
        }

        /// <summary>Sound effects volume (0.0 to 1.0)</summary>
        public float SfxVolume
        {
            get => _sfxVolume;
            set => _sfxVolume = Math.Clamp(value, 0f, 1f);
        }

        /// <summary>Enable/disable music playback</summary>
        public bool IsMusicEnabled
        {
            get => _isMusicEnabled;
            set
            {
                _isMusicEnabled = value;
                if (!value && MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.Stop();
                }
            }
        }

        /// <summary>Enable/disable sound effects</summary>
        public bool AreSfxEnabled
        {
            get => _areSfxEnabled;
            set => _areSfxEnabled = value;
        }

        /// <summary>Current music playback state</summary>
        public MediaState MusicState => MediaPlayer.State;

        /// <summary>Is a music transition currently happening?</summary>
        public bool IsTransitioning => _isTransitioning;

        #endregion

        #region Initialization

        public AudioManager()
        {
            _soundEffects = new Dictionary<string, SoundEffect>();
            _sfxInstancePools = new Dictionary<string, Queue<SoundEffectInstance>>();

            // Set MediaPlayer to repeat by default
            MediaPlayer.IsRepeating = true;
        }

        #endregion

        #region Update

        /// <summary>
        /// Update audio transitions. Call this every frame.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame</param>
        public void Update(float deltaTime)
        {
            if (!_isTransitioning)
                return;

            _transitionElapsed += deltaTime;
            float progress = Math.Clamp(_transitionElapsed / _transitionDuration, 0f, 1f);

            if (progress < 0.5f)
            {
                // First half: fade out current music
                float fadeProgress = progress * 2f; // 0 to 1 over first half
                _fadeOutVolume = 1f - fadeProgress;
                MediaPlayer.Volume = _fadeOutVolume * _musicVolume * _masterVolume;
            }
            else if (progress >= 0.5f && _nextMusic != null && _currentMusic != _nextMusic)
            {
                // Switch to next music at midpoint
                _currentMusic = _nextMusic;
                MediaPlayer.Play(_currentMusic);

                // Second half: fade in new music
                float fadeProgress = (progress - 0.5f) * 2f; // 0 to 1 over second half
                _fadeInVolume = fadeProgress;
                MediaPlayer.Volume = _fadeInVolume * _musicVolume * _masterVolume;

                _nextMusic = null;
            }

            // Transition complete
            if (progress >= 1f)
            {
                _isTransitioning = false;
                MediaPlayer.Volume = _musicVolume * _masterVolume;
            }
        }

        #endregion

        #region Music Playback

        /// <summary>
        /// Play background music immediately without transition.
        /// </summary>
        /// <param name="music">The Song to play</param>
        /// <param name="loop">Should the music loop?</param>
        public void PlayMusic(Song music, bool loop = true)
        {
            if (!_isMusicEnabled || music == null)
                return;

            // Stop any transition
            _isTransitioning = false;
            _nextMusic = null;

            _currentMusic = music;
            MediaPlayer.IsRepeating = loop;
            MediaPlayer.Volume = _musicVolume * _masterVolume;
            MediaPlayer.Play(music);
        }

        /// <summary>
        /// Transition to new music with crossfade effect.
        /// </summary>
        /// <param name="music">The Song to transition to</param>
        /// <param name="transitionDuration">Duration of the crossfade in seconds</param>
        /// <param name="loop">Should the new music loop?</param>
        public void TransitionToMusic(Song music, float transitionDuration = 1.5f, bool loop = true)
        {
            if (!_isMusicEnabled || music == null)
                return;

            // If no music is playing, just play it
            if (_currentMusic == null || MediaPlayer.State != MediaState.Playing)
            {
                PlayMusic(music, loop);
                return;
            }

            // Start crossfade transition
            _nextMusic = music;
            _isTransitioning = true;
            _transitionDuration = transitionDuration;
            _transitionElapsed = 0f;
            _fadeOutVolume = 1f;
            _fadeInVolume = 0f;
            MediaPlayer.IsRepeating = loop;
        }

        /// <summary>
        /// Stop music playback.
        /// </summary>
        public void StopMusic()
        {
            MediaPlayer.Stop();
            _currentMusic = null;
            _nextMusic = null;
            _isTransitioning = false;
        }

        /// <summary>
        /// Pause music playback.
        /// </summary>
        public void PauseMusic()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Pause();
            }
        }

        /// <summary>
        /// Resume paused music.
        /// </summary>
        public void ResumeMusic()
        {
            if (MediaPlayer.State == MediaState.Paused)
            {
                MediaPlayer.Resume();
            }
        }

        /// <summary>
        /// Update the music volume based on master volume.
        /// </summary>
        private void UpdateMusicVolume()
        {
            if (!_isTransitioning)
            {
                MediaPlayer.Volume = _musicVolume * _masterVolume;
            }
        }

        #endregion

        #region Sound Effects Management

        /// <summary>
        /// Register a sound effect for later playback.
        /// </summary>
        /// <param name="name">Unique identifier for this SFX</param>
        /// <param name="soundEffect">The SoundEffect asset</param>
        public void RegisterSoundEffect(string name, SoundEffect soundEffect)
        {
            if (string.IsNullOrEmpty(name) || soundEffect == null)
                return;

            _soundEffects[name] = soundEffect;

            // Initialize instance pool for this SFX
            if (!_sfxInstancePools.ContainsKey(name))
            {
                _sfxInstancePools[name] = new Queue<SoundEffectInstance>();
            }
        }

        /// <summary>
        /// Unregister a sound effect.
        /// </summary>
        /// <param name="name">Name of the SFX to remove</param>
        public void UnregisterSoundEffect(string name)
        {
            if (_soundEffects.ContainsKey(name))
            {
                // Clean up pooled instances
                if (_sfxInstancePools.ContainsKey(name))
                {
                    var pool = _sfxInstancePools[name];
                    while (pool.Count > 0)
                    {
                        var instance = pool.Dequeue();
                        instance.Dispose();
                    }
                    _sfxInstancePools.Remove(name);
                }

                _soundEffects.Remove(name);
            }
        }

        /// <summary>
        /// Play a sound effect once.
        /// </summary>
        /// <param name="name">Name of the registered SFX</param>
        /// <param name="volume">Volume override (0.0 to 1.0), or -1 to use default</param>
        /// <param name="pitch">Pitch adjustment (-1.0 to 1.0)</param>
        /// <param name="pan">Stereo pan (-1.0 left to 1.0 right)</param>
        public void PlaySfx(string name, float volume = -1f, float pitch = 0f, float pan = 0f)
        {
            if (!_areSfxEnabled || !_soundEffects.ContainsKey(name))
                return;

            var sfx = _soundEffects[name];
            float finalVolume = (volume >= 0f ? volume : 1f) * _sfxVolume * _masterVolume;

            // Simple one-shot playback
            sfx.Play(finalVolume, pitch, pan);
        }

        /// <summary>
        /// Play a sound effect with full control via SoundEffectInstance.
        /// Returns the instance for advanced control (looping, etc).
        /// </summary>
        /// <param name="name">Name of the registered SFX</param>
        /// <param name="loop">Should the SFX loop?</param>
        /// <param name="volume">Volume override (0.0 to 1.0), or -1 to use default</param>
        /// <returns>The SoundEffectInstance, or null if not found</returns>
        public SoundEffectInstance PlaySfxInstance(string name, bool loop = false, float volume = -1f)
        {
            if (!_areSfxEnabled || !_soundEffects.ContainsKey(name))
                return null;

            var sfx = _soundEffects[name];
            SoundEffectInstance instance = GetPooledInstance(name);

            if (instance == null)
            {
                instance = sfx.CreateInstance();
            }

            float finalVolume = (volume >= 0f ? volume : 1f) * _sfxVolume * _masterVolume;
            instance.Volume = finalVolume;
            instance.IsLooped = loop;
            instance.Play();

            return instance;
        }

        /// <summary>
        /// Stop all playing sound effect instances.
        /// </summary>
        public void StopAllSfx()
        {
            foreach (var pool in _sfxInstancePools.Values)
            {
                foreach (var instance in pool)
                {
                    if (instance.State == SoundState.Playing)
                    {
                        instance.Stop();
                    }
                }
            }
        }

        /// <summary>
        /// Get a pooled SFX instance if available.
        /// </summary>
        private SoundEffectInstance GetPooledInstance(string name)
        {
            if (!_sfxInstancePools.ContainsKey(name))
                return null;

            var pool = _sfxInstancePools[name];

            // Find a stopped instance in the pool
            while (pool.Count > 0)
            {
                var instance = pool.Dequeue();
                if (instance.State == SoundState.Stopped)
                {
                    return instance;
                }
                // Instance is still playing, put it back if pool isn't full
                if (pool.Count < MaxPoolSize)
                {
                    pool.Enqueue(instance);
                }
            }

            return null;
        }

        /// <summary>
        /// Return a SFX instance to the pool for reuse.
        /// </summary>
        /// <param name="name">Name of the SFX</param>
        /// <param name="instance">The instance to return</param>
        public void ReturnToPool(string name, SoundEffectInstance instance)
        {
            if (instance == null || !_sfxInstancePools.ContainsKey(name))
                return;

            var pool = _sfxInstancePools[name];

            // Only pool if not at capacity
            if (pool.Count < MaxPoolSize)
            {
                instance.Stop();
                pool.Enqueue(instance);
            }
            else
            {
                instance.Dispose();
            }
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Clean up all audio resources.
        /// </summary>
        public void Dispose()
        {
            StopMusic();
            StopAllSfx();

            // Dispose all pooled instances
            foreach (var pool in _sfxInstancePools.Values)
            {
                while (pool.Count > 0)
                {
                    var instance = pool.Dequeue();
                    instance.Dispose();
                }
            }

            _sfxInstancePools.Clear();
            _soundEffects.Clear();
        }

        #endregion
    }
}
