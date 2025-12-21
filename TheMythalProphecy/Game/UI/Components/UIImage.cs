using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI.Components;

/// <summary>
/// UI element for displaying textures and sprites
/// </summary>
public class UIImage : UIElement
{
    public Texture2D Texture { get; set; }
    public Rectangle? SourceRectangle { get; set; }
    public Color ImageTint { get; set; } = Color.White;
    public bool PreserveAspectRatio { get; set; } = true;
    public float Rotation { get; set; } = 0f;

    // Animation support
    public int FrameWidth { get; set; }
    public int FrameHeight { get; set; }
    public int CurrentFrame { get; set; }
    public int FrameCount { get; set; }
    public float FrameDuration { get; set; } = 0.1f;
    private float _frameTimer;

    public UIImage()
    {
    }

    public UIImage(Texture2D texture, Vector2 position)
    {
        Texture = texture;
        Position = position;

        if (texture != null)
        {
            Size = new Vector2(texture.Width, texture.Height);
        }
    }

    public UIImage(Texture2D texture, Vector2 position, Vector2 size) : this(texture, position)
    {
        Size = size;
    }

    public override void Update(GameTime gameTime)
    {
        // Update frame animation if applicable
        if (FrameCount > 1)
        {
            _frameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_frameTimer >= FrameDuration)
            {
                _frameTimer -= FrameDuration;
                CurrentFrame = (CurrentFrame + 1) % FrameCount;
            }
        }

        base.Update(gameTime);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, UITheme theme)
    {
        if (Texture == null) return;

        Rectangle bounds = Bounds;

        // Calculate source rectangle
        Rectangle sourceRect;
        if (SourceRectangle.HasValue)
        {
            sourceRect = SourceRectangle.Value;
        }
        else if (FrameWidth > 0 && FrameHeight > 0)
        {
            // Use frame-based source rectangle
            int framesPerRow = Texture.Width / FrameWidth;
            int frameX = (CurrentFrame % framesPerRow) * FrameWidth;
            int frameY = (CurrentFrame / framesPerRow) * FrameHeight;
            sourceRect = new Rectangle(frameX, frameY, FrameWidth, FrameHeight);
        }
        else
        {
            sourceRect = new Rectangle(0, 0, Texture.Width, Texture.Height);
        }

        // Calculate destination rectangle
        Rectangle destRect = bounds;

        if (PreserveAspectRatio && Size != Vector2.Zero)
        {
            float sourceAspect = (float)sourceRect.Width / sourceRect.Height;
            float destAspect = bounds.Width / (float)bounds.Height;

            if (sourceAspect > destAspect)
            {
                // Fit to width
                int newHeight = (int)(bounds.Width / sourceAspect);
                destRect.Y += (bounds.Height - newHeight) / 2;
                destRect.Height = newHeight;
            }
            else
            {
                // Fit to height
                int newWidth = (int)(bounds.Height * sourceAspect);
                destRect.X += (bounds.Width - newWidth) / 2;
                destRect.Width = newWidth;
            }
        }

        // Draw texture
        Color finalTint = ImageTint * Alpha * Tint;
        Vector2 origin = new Vector2(Anchor.X * sourceRect.Width, Anchor.Y * sourceRect.Height);

        if (Rotation != 0f)
        {
            Vector2 position = new Vector2(destRect.X + destRect.Width / 2, destRect.Y + destRect.Height / 2);
            Vector2 scale = new Vector2((float)destRect.Width / sourceRect.Width, (float)destRect.Height / sourceRect.Height);

            spriteBatch.Draw(
                Texture,
                position,
                sourceRect,
                finalTint,
                Rotation,
                new Vector2(sourceRect.Width / 2, sourceRect.Height / 2),
                scale,
                SpriteEffects.None,
                0f
            );
        }
        else
        {
            spriteBatch.Draw(Texture, destRect, sourceRect, finalTint);
        }
    }

    /// <summary>
    /// Setup frame-based animation
    /// </summary>
    public void SetupAnimation(int frameWidth, int frameHeight, int frameCount, float frameDuration = 0.1f)
    {
        FrameWidth = frameWidth;
        FrameHeight = frameHeight;
        FrameCount = frameCount;
        FrameDuration = frameDuration;
        CurrentFrame = 0;
        _frameTimer = 0;
    }
}
