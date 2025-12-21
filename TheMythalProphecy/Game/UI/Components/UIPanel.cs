using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI.Components;

/// <summary>
/// Layout options for panel children
/// </summary>
public enum PanelLayout
{
    None,           // Manual positioning
    Vertical,       // Stack vertically
    Horizontal,     // Stack horizontally
    Grid            // Grid layout
}

/// <summary>
/// Container panel for organizing UI elements
/// </summary>
public class UIPanel : UIElement
{
    public Color BackgroundColor { get; set; } = new Color(40, 40, 60, 230);
    public Color BorderColor { get; set; } = new Color(200, 200, 220);
    public int BorderThickness { get; set; } = 2;
    public bool DrawBackground { get; set; } = true;
    public bool DrawBorder { get; set; } = true;

    public PanelLayout Layout { get; set; } = PanelLayout.None;
    public int Spacing { get; set; } = 4; // Spacing between children in auto-layout
    public int GridColumns { get; set; } = 2; // For grid layout

    public UIPanel()
    {
        SetPadding(8);
    }

    public UIPanel(Vector2 position, Vector2 size) : this()
    {
        Position = position;
        Size = size;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, UITheme theme)
    {
        Rectangle bounds = Bounds;

        // Get pixel texture from theme or create a simple one
        Texture2D pixel = Core.GameServices.UI?.PixelTexture;
        if (pixel == null) return;

        // Draw background
        if (DrawBackground)
        {
            spriteBatch.Draw(pixel, bounds, BackgroundColor * Alpha);
        }

        // Draw border
        if (DrawBorder && BorderThickness > 0)
        {
            // Top
            spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, BorderThickness), BorderColor * Alpha);
            // Bottom
            spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Bottom - BorderThickness, bounds.Width, BorderThickness), BorderColor * Alpha);
            // Left
            spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, BorderThickness, bounds.Height), BorderColor * Alpha);
            // Right
            spriteBatch.Draw(pixel, new Rectangle(bounds.Right - BorderThickness, bounds.Y, BorderThickness, bounds.Height), BorderColor * Alpha);
        }
    }

    public override void Update(GameTime gameTime)
    {
        // Apply auto-layout if needed
        ApplyLayout();

        base.Update(gameTime);
    }

    /// <summary>
    /// Apply automatic layout to children
    /// </summary>
    private void ApplyLayout()
    {
        if (Layout == PanelLayout.None || Children.Count == 0)
            return;

        Rectangle contentArea = ContentBounds;
        int currentX = contentArea.X;
        int currentY = contentArea.Y;

        switch (Layout)
        {
            case PanelLayout.Vertical:
                foreach (var child in Children)
                {
                    if (!child.Visible) continue;

                    child.Position = new Vector2(currentX - AbsolutePosition.X, currentY - AbsolutePosition.Y);
                    currentY += (int)child.Size.Y + Spacing;
                }
                break;

            case PanelLayout.Horizontal:
                foreach (var child in Children)
                {
                    if (!child.Visible) continue;

                    child.Position = new Vector2(currentX - AbsolutePosition.X, currentY - AbsolutePosition.Y);
                    currentX += (int)child.Size.X + Spacing;
                }
                break;

            case PanelLayout.Grid:
                int column = 0;
                int maxHeightInRow = 0;

                foreach (var child in Children)
                {
                    if (!child.Visible) continue;

                    child.Position = new Vector2(currentX - AbsolutePosition.X, currentY - AbsolutePosition.Y);

                    maxHeightInRow = (int)MathHelper.Max(maxHeightInRow, child.Size.Y);

                    column++;
                    if (column >= GridColumns)
                    {
                        column = 0;
                        currentX = contentArea.X;
                        currentY += maxHeightInRow + Spacing;
                        maxHeightInRow = 0;
                    }
                    else
                    {
                        currentX += (int)child.Size.X + Spacing;
                    }
                }
                break;
        }
    }
}
