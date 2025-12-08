using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

/*<summary>
    // functionalities for Keyboard
    Triggered - new click (1st tick)
    Hold/Pressed - if it's being pressed
    Released - 1st tick after no longer pressed

    // functionalities for Mouse
    track mouse position
    Click Triggered
    Click Released
    Hold
    Scroll up/down/clicked
    Drag
   
   keep in mind most of the times there will be 1 tick delay 
</summary*/

namespace Gui;
public static class InputManager
{
    private static KeyboardState _lastKey;
    private static KeyboardState _currentKey;

    private static MouseState MouseState { get; set; }
    private static MouseState LastMouseState { get; set; }
    
    public static Rectangle MouseCursor { get; private set; }
    public static Vector2 MousePos {get; private set;}
    public static bool MouseClicked {get; private set;} // test

    public static bool WasKeyTriggered(Keys key) // Trigger
    {
        return _currentKey.IsKeyDown(key) && _lastKey.IsKeyUp(key);
    }

    public static bool WasKeyReleased(Keys key) // Release
    {
        return _currentKey.IsKeyUp(key) && _lastKey.IsKeyDown(key);
    }

    public static bool IsKeyPressed(Keys key) // Hold
    {
        return _currentKey.IsKeyDown(key);
    }


    public static void Update()
    {
        _lastKey = _currentKey;
        _currentKey = Keyboard.GetState();

        LastMouseState = MouseState;
        MouseState = Mouse.GetState();

        MousePos = new Vector2(MouseState.Position.X,MouseState.Position.Y); // It might broke when moving camera *maybe*

        MouseClicked = (MouseState.LeftButton == ButtonState.Released) && (LastMouseState.LeftButton == ButtonState.Pressed);

        // Create super small rectangle at mousePos so i can use MouseCursor.Intersects to handle "hovering" 
        MouseCursor = new(MouseState.Position, new(1, 1));
    }
}