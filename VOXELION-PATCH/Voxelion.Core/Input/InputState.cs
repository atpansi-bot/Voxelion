using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Voxelion.Core.Input;

public sealed class InputState
{
    public Point PointerPosition { get; set; }
    public bool IsPointerDown { get; set; }
    public bool IsPointerPressed { get; set; }
    public bool IsPointerReleased { get; set; }
    public bool IsPointerMoved { get; set; }

    public Vector2 MoveAxis { get; set; }
    public bool JumpPressed { get; set; }
    public bool PrimaryActionPressed { get; set; }
    public bool InteractPressed { get; set; }
    public bool MenuPressed { get; set; }
    public bool CancelPressed { get; set; }
    public bool ConfirmPressed { get; set; }

    public KeyboardState Keyboard { get; set; }
    public KeyboardState PreviousKeyboard { get; set; }
    public MouseState Mouse { get; set; }
    public MouseState PreviousMouse { get; set; }
    public GamePadState GamePad { get; set; }
    public GamePadState PreviousGamePad { get; set; }
    public TouchCollection Touches { get; set; }

    public bool IsKeyDown(Keys key) => Keyboard.IsKeyDown(key);
    public bool IsKeyPressed(Keys key) => Keyboard.IsKeyDown(key) && !PreviousKeyboard.IsKeyDown(key);
    public bool IsKeyReleased(Keys key) => !Keyboard.IsKeyDown(key) && PreviousKeyboard.IsKeyDown(key);

    public void BeginFrame()
    {
        PreviousKeyboard = Keyboard;
        PreviousMouse = Mouse;
        PreviousGamePad = GamePad;
    }
}

public sealed class InputManager
{
    private readonly InputState _state = new();
    private Vector2 _lastPointer;
    private bool _touchReady;

    public InputState Current => _state;

    public void Update()
    {
        _state.BeginFrame();

        try { _state.Keyboard = Keyboard.GetState(); }
        catch { _state.Keyboard = default; }

        try { _state.Mouse = Mouse.GetState(); }
        catch { _state.Mouse = default; }

        try { _state.GamePad = GamePad.GetState(PlayerIndex.One); }
        catch { _state.GamePad = default; }

        TouchCollection touches = default;
        try
        {
            touches = TouchPanel.GetState();
            _touchReady = true;
        }
        catch
        {
            _touchReady = false;
        }
        _state.Touches = touches;

        if (_touchReady && touches.Count > 0)
        {
            var t = touches[0];
            _state.PointerPosition = t.Position.ToPoint();
            _state.IsPointerDown = t.State == TouchLocationState.Pressed || t.State == TouchLocationState.Moved;
            _state.IsPointerPressed = t.State == TouchLocationState.Pressed;
            _state.IsPointerReleased = t.State == TouchLocationState.Released;
        }
        else
        {
            _state.PointerPosition = _state.Mouse.Position;
            _state.IsPointerDown = _state.Mouse.LeftButton == ButtonState.Pressed;
            _state.IsPointerPressed = _state.Mouse.LeftButton == ButtonState.Pressed &&
                                      _state.PreviousMouse.LeftButton == ButtonState.Released;
            _state.IsPointerReleased = _state.Mouse.LeftButton == ButtonState.Released &&
                                       _state.PreviousMouse.LeftButton == ButtonState.Pressed;
        }

        _state.IsPointerMoved = _state.PointerPosition != _lastPointer.ToPoint();
        _lastPointer = _state.PointerPosition.ToVector2();

        float x = 0, y = 0;
        if (_state.IsKeyDown(Keys.A) || _state.IsKeyDown(Keys.Left)) x -= 1;
        if (_state.IsKeyDown(Keys.D) || _state.IsKeyDown(Keys.Right)) x += 1;
        if (_state.IsKeyDown(Keys.W) || _state.IsKeyDown(Keys.Up)) y -= 1;
        if (_state.IsKeyDown(Keys.S) || _state.IsKeyDown(Keys.Down)) y += 1;
        try
        {
            x += _state.GamePad.ThumbSticks.Left.X;
            y -= _state.GamePad.ThumbSticks.Left.Y;
        }
        catch { /* ignore */ }
        _state.MoveAxis = new Vector2(MathHelper.Clamp(x, -1, 1), MathHelper.Clamp(y, -1, 1));

        _state.JumpPressed = _state.IsKeyPressed(Keys.Space);
        _state.PrimaryActionPressed = _state.IsKeyPressed(Keys.E) || _state.IsPointerPressed;
        _state.InteractPressed = _state.IsKeyPressed(Keys.F);
        _state.MenuPressed = _state.IsKeyPressed(Keys.Escape);
        _state.CancelPressed = _state.IsKeyPressed(Keys.Escape);
        _state.ConfirmPressed = _state.IsKeyPressed(Keys.Enter);
    }
}
