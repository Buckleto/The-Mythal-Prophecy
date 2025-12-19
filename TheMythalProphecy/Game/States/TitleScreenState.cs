using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TheMythalProphecy.Game.Core;

namespace TheMythalProphecy.Game.States;

/// <summary>
/// Title screen state - displays game title and waits for player input to start
/// </summary>
public class TitleScreenState : IGameState
{
    private readonly ContentManager _content;
    private readonly GameStateManager _stateManager;
    private SpriteFont _font;

    private readonly string _title = "The Mythal Prophecy";
    private readonly string _prompt = "Press Enter to Start";

    private float _promptAlpha;
    private float _promptAlphaDirection = 1.0f;

    public TitleScreenState(ContentManager content, GameStateManager stateManager)
    {
        _content = content;
        _stateManager = stateManager;
        _promptAlpha = 1.0f;
    }

    public void Enter()
    {
        _font = _content.Load<SpriteFont>("Fonts/Default");
    }

    public void Exit()
    {
    }

    public void Update(GameTime gameTime)
    {
        // Animate the prompt text (fade in/out)
        _promptAlpha += _promptAlphaDirection * (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_promptAlpha >= 1.0f)
        {
            _promptAlpha = 1.0f;
            _promptAlphaDirection = -1.0f;
        }
        else if (_promptAlpha <= 0.3f)
        {
            _promptAlpha = 0.3f;
            _promptAlphaDirection = 1.0f;
        }

        // Check for Enter key to start game
        if (GameServices.Input.IsAcceptPressed())
        {
            // TODO: Transition to next state (WorldMapState) when implemented
            // For now, just do nothing
        }
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        spriteBatch.Begin();

        // Calculate positions for centered text
        var titleSize = _font.MeasureString(_title);
        var titlePosition = new Vector2(
            (1280 - titleSize.X) / 2,
            300
        );

        var promptSize = _font.MeasureString(_prompt);
        var promptPosition = new Vector2(
            (1280 - promptSize.X) / 2,
            450
        );

        // Draw title
        spriteBatch.DrawString(_font, _title, titlePosition, Color.White);

        // Draw prompt with fading animation
        var promptColor = Color.White * _promptAlpha;
        spriteBatch.DrawString(_font, _prompt, promptPosition, promptColor);

        spriteBatch.End();
    }
}
