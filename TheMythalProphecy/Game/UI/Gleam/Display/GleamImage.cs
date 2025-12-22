using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI.Gleam;

/// <summary>
/// Image display element with optional aspect ratio preservation and tinting.
/// </summary>
public class GleamImage : GleamElement
{
    public Texture2D Texture { get; set; }
    public Rectangle? SourceRectangle { get; set; }
    public Color Tint { get; set; } = Color.White;
    public bool PreserveAspectRatio { get; set; } = true;
    public float Rotation { get; set; }

    public GleamImage(Vector2 position, Vector2 size)
    {
        Position = position;
        Size = size;
    }

    public GleamImage(Texture2D texture, Vector2 position) : this(position, Vector2.Zero)
    {
        Texture = texture;
        if (texture != null)
        {
            Size = new Vector2(texture.Width, texture.Height);
        }
    }

    public GleamImage(Texture2D texture, Vector2 position, Vector2 size) : this(position, size)
    {
        Texture = texture;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, GleamRenderer renderer)
    {
        if (Texture == null) return;

        Rectangle bounds = Bounds;
        Rectangle sourceRect = SourceRectangle ?? new Rectangle(0, 0, Texture.Width, Texture.Height);
        Rectangle destRect = bounds;

        // Preserve aspect ratio if requested
        if (PreserveAspectRatio && Size != Vector2.Zero)
        {
            float sourceAspect = (float)sourceRect.Width / sourceRect.Height;
            float destAspect = bounds.Width / (float)bounds.Height;

            if (sourceAspect > destAspect)
            {
                int newHeight = (int)(bounds.Width / sourceAspect);
                destRect.Y += (bounds.Height - newHeight) / 2;
                destRect.Height = newHeight;
            }
            else
            {
                int newWidth = (int)(bounds.Height * sourceAspect);
                destRect.X += (bounds.Width - newWidth) / 2;
                destRect.Width = newWidth;
            }
        }

        Color finalTint = Tint * Alpha;

        if (Rotation != 0f)
        {
            Vector2 position = new Vector2(destRect.X + destRect.Width / 2f, destRect.Y + destRect.Height / 2f);
            Vector2 scale = new Vector2((float)destRect.Width / sourceRect.Width, (float)destRect.Height / sourceRect.Height);
            Vector2 origin = new Vector2(sourceRect.Width / 2f, sourceRect.Height / 2f);

            spriteBatch.Draw(Texture, position, sourceRect, finalTint, Rotation, origin, scale, SpriteEffects.None, 0f);
        }
        else
        {
            spriteBatch.Draw(Texture, destRect, sourceRect, finalTint);
        }
    }

    public override bool HandleInput(Vector2 mousePosition, bool mouseDown, bool mouseClicked)
    {
        // Images don't handle input by default
        return false;
    }
}
