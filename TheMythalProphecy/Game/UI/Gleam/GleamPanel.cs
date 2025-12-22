using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI.Gleam;

/// <summary>
/// Container panel with cosmic styling and optional auto-layout.
/// </summary>
public class GleamPanel : GleamElement
{
    public bool DrawBackground { get; set; } = true;
    public bool DrawBorder { get; set; } = true;
    public Color? BackgroundColor { get; set; }
    public Color? BorderColor { get; set; }
    public int BorderThickness { get; set; } = 2;
    public float BackgroundAlpha { get; set; } = 0.9f;

    // Auto-layout
    public GleamLayout Layout { get; set; } = GleamLayout.None;
    public int Spacing { get; set; } = 8;
    public int Padding { get; set; } = 8;

    public GleamPanel(Vector2 position, Vector2 size)
    {
        Position = position;
        Size = size;
    }

    public override void Update(GameTime gameTime, GleamRenderer renderer)
    {
        // Apply layout to children
        if (Layout != GleamLayout.None)
        {
            ApplyLayout();
        }

        base.Update(gameTime, renderer);
    }

    private void ApplyLayout()
    {
        float currentX = Padding;
        float currentY = Padding;

        foreach (var child in Children)
        {
            if (!child.Visible) continue;

            switch (Layout)
            {
                case GleamLayout.Vertical:
                    child.Position = new Vector2(currentX, currentY);
                    currentY += child.Size.Y + Spacing;
                    break;

                case GleamLayout.Horizontal:
                    child.Position = new Vector2(currentX, currentY);
                    currentX += child.Size.X + Spacing;
                    break;
            }
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, GleamRenderer renderer)
    {
        var theme = renderer.Theme;
        Rectangle bounds = Bounds;

        // Background
        if (DrawBackground)
        {
            Color bg = BackgroundColor ?? theme.DeepPurple;
            renderer.DrawRect(spriteBatch, bounds, bg, Alpha * BackgroundAlpha);
        }

        // Border
        if (DrawBorder && BorderThickness > 0)
        {
            Color border = BorderColor ?? theme.Gold;
            renderer.DrawRectBorder(spriteBatch, bounds, border, BorderThickness, Alpha);
        }
    }
}

public enum GleamLayout
{
    None,
    Vertical,
    Horizontal
}
