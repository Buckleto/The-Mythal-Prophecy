using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TheMythalProphecy.Game.UI.Components;

namespace TheMythalProphecy.Game.UI.HUD
{
    /// <summary>
    /// HUD element showing HP/MP bars for active party members on the world map.
    /// </summary>
    public class PartyStatusHUD : UIElement
    {
        private List<PartyMemberDisplay> _memberDisplays;
        private int _maxPartySize;

        public PartyStatusHUD()
        {
            _maxPartySize = 4;
            _memberDisplays = new List<PartyMemberDisplay>();
            SetPadding(5);
        }

        public PartyStatusHUD(Vector2 position, Vector2 size, int maxPartySize = 4) : this()
        {
            Position = position;
            Size = size;
            _maxPartySize = maxPartySize;

            // Create displays for max party members
            float memberWidth = (size.X - 10f * (_maxPartySize - 1)) / _maxPartySize;
            for (int i = 0; i < _maxPartySize; i++)
            {
                float memberX = i * (memberWidth + 10f);
                var display = new PartyMemberDisplay(new Vector2(memberX, 0), new Vector2(memberWidth, size.Y));
                display.Visible = false; // Hidden until populated
                _memberDisplays.Add(display);
                AddChild(display);
            }
        }

        public void UpdateMember(int index, string name, int currentHp, int maxHp, int currentMp, int maxMp)
        {
            if (index >= 0 && index < _memberDisplays.Count)
            {
                var display = _memberDisplays[index];
                display.UpdateInfo(name, currentHp, maxHp, currentMp, maxMp);
                display.Visible = true;
            }
        }

        public void HideMember(int index)
        {
            if (index >= 0 && index < _memberDisplays.Count)
            {
                _memberDisplays[index].Visible = false;
            }
        }

        public void ClearAll()
        {
            foreach (var display in _memberDisplays)
            {
                display.Visible = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch, UITheme theme)
        {
            // Draw semi-transparent background
            Texture2D pixel = Core.GameServices.UI?.PixelTexture;
            if (pixel != null)
            {
                spriteBatch.Draw(pixel, Bounds, Color.Black * 0.6f);
            }
            else
            {
                Console.WriteLine("[PartyStatusHUD] WARNING: PixelTexture is null, cannot draw background");
            }
        }
    }

    /// <summary>
    /// Individual party member display showing name and HP/MP bars.
    /// </summary>
    public class PartyMemberDisplay : UIElement
    {
        private UILabel _nameLabel;
        private UIProgressBar _hpBar;
        private UIProgressBar _mpBar;
        private UILabel _hpLabel;
        private UILabel _mpLabel;

        public PartyMemberDisplay()
        {
            SetPadding(2);
        }

        public PartyMemberDisplay(Vector2 position, Vector2 size) : this()
        {
            Position = position;
            Size = size;
            BuildUI();
        }

        private void BuildUI()
        {
            float padding = 5f;
            float labelHeight = 20f;
            float barHeight = 15f;

            // Name
            _nameLabel = new UILabel
            {
                Position = new Vector2(padding, padding),
                Size = new Vector2(Size.X - padding * 2, labelHeight),
                Text = "",
                TextColor = Color.Yellow
            };
            AddChild(_nameLabel);

            // HP Bar
            _hpBar = new UIProgressBar(
                new Vector2(padding, padding + labelHeight + 2),
                new Vector2(Size.X - padding * 2, barHeight)
            );
            _hpBar.FillColor = Color.LimeGreen;
            _hpBar.BackgroundColor = Color.DarkRed;
            AddChild(_hpBar);

            // HP Label
            _hpLabel = new UILabel
            {
                Position = new Vector2(padding, padding + labelHeight + barHeight + 4),
                Size = new Vector2(Size.X - padding * 2, 15f),
                Text = "",
                Alignment = TextAlignment.Left
            };
            AddChild(_hpLabel);

            // MP Bar
            _mpBar = new UIProgressBar(
                new Vector2(padding, padding + labelHeight + barHeight + 20),
                new Vector2(Size.X - padding * 2, barHeight)
            );
            _mpBar.FillColor = Color.CornflowerBlue;
            _mpBar.BackgroundColor = Color.DarkBlue;
            AddChild(_mpBar);

            // MP Label
            _mpLabel = new UILabel
            {
                Position = new Vector2(padding, padding + labelHeight + barHeight * 2 + 22),
                Size = new Vector2(Size.X - padding * 2, 15f),
                Text = "",
                Alignment = TextAlignment.Left
            };
            AddChild(_mpLabel);
        }

        public void UpdateInfo(string name, int currentHp, int maxHp, int currentMp, int maxMp)
        {
            _nameLabel.Text = name;
            _hpBar.MaxValue = maxHp;
            _hpBar.CurrentValue = currentHp;
            _mpBar.MaxValue = maxMp;
            _mpBar.CurrentValue = currentMp;
            _hpLabel.Text = $"HP: {currentHp}/{maxHp}";
            _mpLabel.Text = $"MP: {currentMp}/{maxMp}";

            // Change HP bar color based on health percentage
            float hpPercent = maxHp > 0 ? (float)currentHp / maxHp : 0f;
            if (hpPercent <= 0.25f)
                _hpBar.FillColor = Color.Red;
            else if (hpPercent <= 0.5f)
                _hpBar.FillColor = Color.Orange;
            else
                _hpBar.FillColor = Color.LimeGreen;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch, UITheme theme)
        {
            // Draw background for member slot
            Texture2D pixel = Core.GameServices.UI?.PixelTexture;
            if (pixel != null)
            {
                spriteBatch.Draw(pixel, Bounds, Color.Black * 0.4f);
            }
            else
            {
                Console.WriteLine("[PartyMemberDisplay] WARNING: PixelTexture is null, cannot draw background");
            }
        }
    }
}
