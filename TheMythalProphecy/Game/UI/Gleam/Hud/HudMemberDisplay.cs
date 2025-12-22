using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI.Gleam;

/// <summary>
/// Displays a single party member's name, HP, and MP bars.
/// </summary>
public class HudMemberDisplay : GleamElement
{
    private readonly HudTheme _hudTheme;
    private readonly GleamLabel _nameLabel;
    private readonly HudProgressBar _hpBar;
    private readonly HudProgressBar _mpBar;

    private const int Padding = 5;
    private const int BarHeight = 16;
    private const int LabelHeight = 18;
    private const int Spacing = 2;

    public HudMemberDisplay(Vector2 position, Vector2 size, HudTheme hudTheme)
    {
        Position = position;
        Size = size;
        _hudTheme = hudTheme;

        float barWidth = size.X - Padding * 2;
        float currentY = Padding;

        // Name label
        _nameLabel = new GleamLabel("", new Vector2(Padding, currentY), new Vector2(barWidth, LabelHeight))
        {
            TextColor = hudTheme.GoldBright,
            Alignment = TextAlignment.Center
        };
        AddChild(_nameLabel);
        currentY += LabelHeight + Spacing;

        // HP bar
        _hpBar = new HudProgressBar(new Vector2(Padding, currentY), new Vector2(barWidth, BarHeight), hudTheme, HudBarType.HP);
        AddChild(_hpBar);
        currentY += BarHeight + Spacing;

        // MP bar
        _mpBar = new HudProgressBar(new Vector2(Padding, currentY), new Vector2(barWidth, BarHeight), hudTheme, HudBarType.MP);
        AddChild(_mpBar);
    }

    public void UpdateInfo(string name, float currentHp, float maxHp, float currentMp, float maxMp)
    {
        _nameLabel.Text = name;
        _hpBar.MaxValue = maxHp;
        _hpBar.CurrentValue = currentHp;
        _mpBar.MaxValue = maxMp;
        _mpBar.CurrentValue = currentMp;
        Visible = true;
    }

    public void Clear()
    {
        _nameLabel.Text = "";
        _hpBar.CurrentValue = 0;
        _mpBar.CurrentValue = 0;
        Visible = false;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, GleamRenderer renderer)
    {
        Rectangle bounds = Bounds;

        // Background
        renderer.DrawRect(spriteBatch, bounds, _hudTheme.DarkPurple, Alpha * 0.6f);

        // Border
        renderer.DrawRectBorder(spriteBatch, bounds, _hudTheme.Gold, 1, Alpha * 0.4f);
    }

    public override bool HandleInput(Vector2 mousePosition, bool mouseDown, bool mouseClicked)
    {
        return false; // Display only
    }
}
