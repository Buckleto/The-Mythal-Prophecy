using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheMythalProphecy.Game.States;

namespace TheMythalProphecy.Game.Core;

public class MythalGame : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private GameStateManager _stateManager;

    public MythalGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        // Set window title
        Window.Title = "The Mythal Prophecy";

        // Set default resolution (1280x720)
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Initialize game services
        GameServices.Initialize(Content, GraphicsDevice);

        // Initialize state manager
        _stateManager = new GameStateManager();

        // Set initial state to title screen
        var titleState = new TitleScreenState(Content, _stateManager);
        _stateManager.ChangeState(titleState);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Update game services (input, etc.)
        GameServices.Update();

        // Update current state
        _stateManager?.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Draw current state
        _stateManager?.Draw(_spriteBatch, gameTime);

        base.Draw(gameTime);
    }
}
