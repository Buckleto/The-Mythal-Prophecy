using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheMythalProphecy.Game.Systems.Animation
{
    /// <summary>
    /// Defines animation data (frames, timing, looping)
    /// Can be shared across multiple Animation instances
    /// </summary>
    public class AnimationDefinition
    {
        public string Name { get; set; }
        public Texture2D SpriteSheet { get; set; }
        public int FrameCount { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public float FrameDuration { get; set; } // seconds per frame
        public bool Loop { get; set; }
        public int FramesPerRow { get; set; }
        public Point StartFrame { get; set; } // Starting position (column, row) on sprite sheet

        /// <summary>
        /// Create an animation definition
        /// </summary>
        /// <param name="name">Animation name/identifier</param>
        /// <param name="spriteSheet">Texture containing animation frames</param>
        /// <param name="frameWidth">Width of each frame in pixels</param>
        /// <param name="frameHeight">Height of each frame in pixels</param>
        /// <param name="frameCount">Total number of frames</param>
        /// <param name="frameDuration">Time each frame displays (in seconds)</param>
        /// <param name="loop">Whether animation should loop</param>
        /// <param name="framesPerRow">Number of frames per row in sprite sheet (0 = auto-calculate)</param>
        /// <param name="startFrame">Starting frame position on sprite sheet</param>
        public AnimationDefinition(
            string name,
            Texture2D spriteSheet,
            int frameWidth,
            int frameHeight,
            int frameCount,
            float frameDuration = 0.1f,
            bool loop = true,
            int framesPerRow = 0,
            Point? startFrame = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (spriteSheet == null)
                throw new ArgumentNullException(nameof(spriteSheet));
            if (frameCount <= 0)
                throw new ArgumentException("Frame count must be greater than 0", nameof(frameCount));
            if (frameWidth <= 0)
                throw new ArgumentException("Frame width must be greater than 0", nameof(frameWidth));
            if (frameHeight <= 0)
                throw new ArgumentException("Frame height must be greater than 0", nameof(frameHeight));

            Name = name;
            SpriteSheet = spriteSheet;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            FrameCount = frameCount;
            FrameDuration = frameDuration;
            Loop = loop;
            StartFrame = startFrame ?? Point.Zero;

            // Auto-calculate frames per row if not specified
            if (framesPerRow == 0)
            {
                FramesPerRow = spriteSheet.Width / frameWidth;
            }
            else
            {
                FramesPerRow = framesPerRow;
            }
        }

        /// <summary>
        /// Get the source rectangle for a specific frame
        /// </summary>
        public Rectangle GetFrameRectangle(int frameIndex)
        {
            if (frameIndex < 0 || frameIndex >= FrameCount)
                throw new ArgumentOutOfRangeException(nameof(frameIndex));

            // Calculate position on sprite sheet
            int totalFrameIndex = (StartFrame.Y * FramesPerRow) + StartFrame.X + frameIndex;
            int column = totalFrameIndex % FramesPerRow;
            int row = totalFrameIndex / FramesPerRow;

            return new Rectangle(
                column * FrameWidth,
                row * FrameHeight,
                FrameWidth,
                FrameHeight
            );
        }

        /// <summary>
        /// Get total animation duration in seconds
        /// </summary>
        public float GetTotalDuration()
        {
            return FrameCount * FrameDuration;
        }

        /// <summary>
        /// Create a simple horizontal strip animation
        /// </summary>
        public static AnimationDefinition CreateHorizontalStrip(
            string name,
            Texture2D spriteSheet,
            int frameCount,
            float frameDuration = 0.1f,
            bool loop = true)
        {
            int frameWidth = spriteSheet.Width / frameCount;
            int frameHeight = spriteSheet.Height;

            return new AnimationDefinition(
                name,
                spriteSheet,
                frameWidth,
                frameHeight,
                frameCount,
                frameDuration,
                loop,
                frameCount
            );
        }

        /// <summary>
        /// Create a simple vertical strip animation
        /// </summary>
        public static AnimationDefinition CreateVerticalStrip(
            string name,
            Texture2D spriteSheet,
            int frameCount,
            float frameDuration = 0.1f,
            bool loop = true)
        {
            int frameWidth = spriteSheet.Width;
            int frameHeight = spriteSheet.Height / frameCount;

            return new AnimationDefinition(
                name,
                spriteSheet,
                frameWidth,
                frameHeight,
                frameCount,
                frameDuration,
                loop,
                1 // One frame per row (vertical strip)
            );
        }

        /// <summary>
        /// Create a grid-based animation
        /// </summary>
        public static AnimationDefinition CreateGrid(
            string name,
            Texture2D spriteSheet,
            int frameWidth,
            int frameHeight,
            int frameCount,
            float frameDuration = 0.1f,
            bool loop = true,
            Point? startFrame = null)
        {
            return new AnimationDefinition(
                name,
                spriteSheet,
                frameWidth,
                frameHeight,
                frameCount,
                frameDuration,
                loop,
                0, // Auto-calculate frames per row
                startFrame
            );
        }
    }
}
