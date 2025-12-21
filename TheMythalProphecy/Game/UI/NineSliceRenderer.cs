using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI;

/// <summary>
/// Utility for rendering scalable UI borders and backgrounds using 9-slice technique
/// </summary>
public static class NineSliceRenderer
{
    /// <summary>
    /// Draw a 9-slice sprite (scalable border with corners)
    /// </summary>
    /// <param name="spriteBatch">SpriteBatch to draw with</param>
    /// <param name="texture">Texture containing the 9-slice sprite</param>
    /// <param name="destinationRect">Rectangle to draw to</param>
    /// <param name="sourceRect">Source rectangle in texture (the 9-slice template)</param>
    /// <param name="cornerSize">Size of the corner pieces</param>
    /// <param name="color">Tint color</param>
    public static void Draw(
        SpriteBatch spriteBatch,
        Texture2D texture,
        Rectangle destinationRect,
        Rectangle sourceRect,
        int cornerSize,
        Color color)
    {
        // Define source rectangles for each of the 9 slices
        int x = sourceRect.X;
        int y = sourceRect.Y;
        int w = sourceRect.Width;
        int h = sourceRect.Height;

        // Corners
        Rectangle srcTopLeft = new Rectangle(x, y, cornerSize, cornerSize);
        Rectangle srcTopRight = new Rectangle(x + w - cornerSize, y, cornerSize, cornerSize);
        Rectangle srcBottomLeft = new Rectangle(x, y + h - cornerSize, cornerSize, cornerSize);
        Rectangle srcBottomRight = new Rectangle(x + w - cornerSize, y + h - cornerSize, cornerSize, cornerSize);

        // Edges
        Rectangle srcTop = new Rectangle(x + cornerSize, y, w - cornerSize * 2, cornerSize);
        Rectangle srcBottom = new Rectangle(x + cornerSize, y + h - cornerSize, w - cornerSize * 2, cornerSize);
        Rectangle srcLeft = new Rectangle(x, y + cornerSize, cornerSize, h - cornerSize * 2);
        Rectangle srcRight = new Rectangle(x + w - cornerSize, y + cornerSize, cornerSize, h - cornerSize * 2);

        // Center
        Rectangle srcCenter = new Rectangle(x + cornerSize, y + cornerSize, w - cornerSize * 2, h - cornerSize * 2);

        // Define destination rectangles
        int dx = destinationRect.X;
        int dy = destinationRect.Y;
        int dw = destinationRect.Width;
        int dh = destinationRect.Height;

        // Corners (fixed size)
        Rectangle dstTopLeft = new Rectangle(dx, dy, cornerSize, cornerSize);
        Rectangle dstTopRight = new Rectangle(dx + dw - cornerSize, dy, cornerSize, cornerSize);
        Rectangle dstBottomLeft = new Rectangle(dx, dy + dh - cornerSize, cornerSize, cornerSize);
        Rectangle dstBottomRight = new Rectangle(dx + dw - cornerSize, dy + dh - cornerSize, cornerSize, cornerSize);

        // Edges (stretched)
        Rectangle dstTop = new Rectangle(dx + cornerSize, dy, dw - cornerSize * 2, cornerSize);
        Rectangle dstBottom = new Rectangle(dx + cornerSize, dy + dh - cornerSize, dw - cornerSize * 2, cornerSize);
        Rectangle dstLeft = new Rectangle(dx, dy + cornerSize, cornerSize, dh - cornerSize * 2);
        Rectangle dstRight = new Rectangle(dx + dw - cornerSize, dy + cornerSize, cornerSize, dh - cornerSize * 2);

        // Center (stretched)
        Rectangle dstCenter = new Rectangle(dx + cornerSize, dy + cornerSize, dw - cornerSize * 2, dh - cornerSize * 2);

        // Draw all 9 slices
        spriteBatch.Draw(texture, dstTopLeft, srcTopLeft, color);
        spriteBatch.Draw(texture, dstTopRight, srcTopRight, color);
        spriteBatch.Draw(texture, dstBottomLeft, srcBottomLeft, color);
        spriteBatch.Draw(texture, dstBottomRight, srcBottomRight, color);
        spriteBatch.Draw(texture, dstTop, srcTop, color);
        spriteBatch.Draw(texture, dstBottom, srcBottom, color);
        spriteBatch.Draw(texture, dstLeft, srcLeft, color);
        spriteBatch.Draw(texture, dstRight, srcRight, color);
        spriteBatch.Draw(texture, dstCenter, srcCenter, color);
    }

    /// <summary>
    /// Draw a simple filled rectangle with border
    /// </summary>
    public static void DrawPanel(
        SpriteBatch spriteBatch,
        Texture2D pixel,
        Rectangle bounds,
        Color fillColor,
        Color borderColor,
        int borderThickness = 2)
    {
        // Draw fill
        spriteBatch.Draw(pixel, bounds, fillColor);

        // Draw borders
        // Top
        spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, borderThickness), borderColor);
        // Bottom
        spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Bottom - borderThickness, bounds.Width, borderThickness), borderColor);
        // Left
        spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, borderThickness, bounds.Height), borderColor);
        // Right
        spriteBatch.Draw(pixel, new Rectangle(bounds.Right - borderThickness, bounds.Y, borderThickness, bounds.Height), borderColor);
    }
}
