using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TheMythalProphecy.Game.Systems.Input;

/// <summary>
/// Centralized input management for keyboard and gamepad
/// </summary>
public class InputManager
{
    private KeyboardState _currentKeyboardState;
    private KeyboardState _previousKeyboardState;

    private GamePadState _currentGamePadState;
    private GamePadState _previousGamePadState;

    public InputManager()
    {
        _currentKeyboardState = Keyboard.GetState();
        _previousKeyboardState = _currentKeyboardState;

        _currentGamePadState = GamePad.GetState(PlayerIndex.One);
        _previousGamePadState = _currentGamePadState;
    }

    /// <summary>
    /// Update input states - call this once per frame before checking input
    /// </summary>
    public void Update()
    {
        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = Keyboard.GetState();

        _previousGamePadState = _currentGamePadState;
        _currentGamePadState = GamePad.GetState(PlayerIndex.One);
    }

    /// <summary>
    /// Check if a key was just pressed this frame (not held from previous frame)
    /// </summary>
    public bool IsKeyPressed(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
    }

    /// <summary>
    /// Check if a key is currently held down
    /// </summary>
    public bool IsKeyDown(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key);
    }

    /// <summary>
    /// Check if a key was just released this frame
    /// </summary>
    public bool IsKeyReleased(Keys key)
    {
        return !_currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyDown(key);
    }

    /// <summary>
    /// Check if a gamepad button was just pressed this frame
    /// </summary>
    public bool IsButtonPressed(Buttons button)
    {
        return _currentGamePadState.IsButtonDown(button) && !_previousGamePadState.IsButtonDown(button);
    }

    /// <summary>
    /// Check if a gamepad button is currently held down
    /// </summary>
    public bool IsButtonDown(Buttons button)
    {
        return _currentGamePadState.IsButtonDown(button);
    }

    /// <summary>
    /// Check if a gamepad button was just released this frame
    /// </summary>
    public bool IsButtonReleased(Buttons button)
    {
        return !_currentGamePadState.IsButtonDown(button) && _previousGamePadState.IsButtonDown(button);
    }

    /// <summary>
    /// Get directional input as a Vector2 (normalized)
    /// Combines keyboard (WASD/Arrows) and left thumbstick input
    /// </summary>
    public Vector2 GetMovementInput()
    {
        var movement = Vector2.Zero;

        // Keyboard input
        if (_currentKeyboardState.IsKeyDown(Keys.W) || _currentKeyboardState.IsKeyDown(Keys.Up))
            movement.Y -= 1;
        if (_currentKeyboardState.IsKeyDown(Keys.S) || _currentKeyboardState.IsKeyDown(Keys.Down))
            movement.Y += 1;
        if (_currentKeyboardState.IsKeyDown(Keys.A) || _currentKeyboardState.IsKeyDown(Keys.Left))
            movement.X -= 1;
        if (_currentKeyboardState.IsKeyDown(Keys.D) || _currentKeyboardState.IsKeyDown(Keys.Right))
            movement.X += 1;

        // Gamepad left thumbstick
        var thumbstick = _currentGamePadState.ThumbSticks.Left;
        if (thumbstick.LengthSquared() > 0.1f) // Dead zone
        {
            movement.X += thumbstick.X;
            movement.Y -= thumbstick.Y; // Y is inverted on thumbstick
        }

        // Normalize if length > 1 to prevent faster diagonal movement
        if (movement.LengthSquared() > 1.0f)
            movement.Normalize();

        return movement;
    }

    /// <summary>
    /// Check if the accept/confirm action was pressed (Enter, Space, or A button)
    /// </summary>
    public bool IsAcceptPressed()
    {
        return IsKeyPressed(Keys.Enter) ||
               IsKeyPressed(Keys.Space) ||
               IsButtonPressed(Buttons.A);
    }

    /// <summary>
    /// Check if the cancel/back action was pressed (Escape, Backspace, or B button)
    /// </summary>
    public bool IsCancelPressed()
    {
        return IsKeyPressed(Keys.Escape) ||
               IsKeyPressed(Keys.Back) ||
               IsButtonPressed(Buttons.B);
    }

    /// <summary>
    /// Check if the menu action was pressed (Tab or Start button)
    /// </summary>
    public bool IsMenuPressed()
    {
        return IsKeyPressed(Keys.Tab) ||
               IsButtonPressed(Buttons.Start);
    }
}
