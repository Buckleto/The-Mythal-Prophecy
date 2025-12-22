using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMythalProphecy.Game.UI.Gleam;

/// <summary>
/// Container for party member status displays.
/// Shows HP/MP for up to 4 active party members.
/// </summary>
public class HudPartyStatus : GleamElement
{
    private readonly HudTheme _hudTheme;
    private readonly List<HudMemberDisplay> _memberDisplays;
    private readonly int _maxPartySize;
    private const int Spacing = 10;
    private const int Padding = 5;

    public HudPartyStatus(Vector2 position, Vector2 size, HudTheme hudTheme, int maxPartySize = 4)
    {
        Position = position;
        Size = size;
        _hudTheme = hudTheme;
        _maxPartySize = maxPartySize;
        _memberDisplays = new List<HudMemberDisplay>();

        CreateMemberDisplays();
    }

    private void CreateMemberDisplays()
    {
        float availableWidth = Size.X - Padding * 2 - Spacing * (_maxPartySize - 1);
        float memberWidth = availableWidth / _maxPartySize;
        float memberHeight = Size.Y - Padding * 2;

        for (int i = 0; i < _maxPartySize; i++)
        {
            float x = Padding + i * (memberWidth + Spacing);
            var display = new HudMemberDisplay(
                new Vector2(x, Padding),
                new Vector2(memberWidth, memberHeight),
                _hudTheme
            );
            display.Visible = false;
            _memberDisplays.Add(display);
            AddChild(display);
        }
    }

    /// <summary>
    /// Updates a party member's display at the given index.
    /// </summary>
    public void UpdateMember(int index, string name, float currentHp, float maxHp, float currentMp, float maxMp)
    {
        if (index >= 0 && index < _memberDisplays.Count)
        {
            _memberDisplays[index].UpdateInfo(name, currentHp, maxHp, currentMp, maxMp);
        }
    }

    /// <summary>
    /// Clears all member displays.
    /// </summary>
    public void ClearAll()
    {
        foreach (var display in _memberDisplays)
        {
            display.Clear();
        }
    }

    /// <summary>
    /// Sets the number of visible member slots.
    /// </summary>
    public void SetActiveCount(int count)
    {
        for (int i = 0; i < _memberDisplays.Count; i++)
        {
            _memberDisplays[i].Visible = i < count;
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch, GleamRenderer renderer)
    {
        Rectangle bounds = Bounds;

        // Background
        renderer.DrawRect(spriteBatch, bounds, _hudTheme.PanelBackground, Alpha * _hudTheme.PanelAlpha);

        // Border
        renderer.DrawRectBorder(spriteBatch, bounds, _hudTheme.Gold, _hudTheme.BorderThickness, Alpha * 0.5f);
    }

    public override bool HandleInput(Vector2 mousePosition, bool mouseDown, bool mouseClicked)
    {
        return false; // Display only
    }
}
