using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Voxelion.Core.Input;

public enum InputDeviceKind
{
    Touch,
    Mouse,
    Keyboard,
    Controller
}

public sealed class InputState
{
    public Point PointerPosition { get; set; }
    public bool IsPointerDown { get; set; }
    public bool IsPointerPressed { get; set; }
    public bool IsPointerReleased { get; set; }
    public bool IsPointerMoved { get; set; }

    public bool IsLongPress { get; set; }
    public float PointerHoldTime { get; set; }
    public Vector2 SwipeDelta { get; set; }
    public Vector2 ScrollDelta { get; set; }

    public Vector2 MoveAxis { get; set; }
    public bool JumpPressed { get; set; }
    public bool PrimaryActionPressed { get; set; }
    public bool InteractPressed { get; set; }
    public bool MenuPressed { get; set; }
    public bool CancelPressed { get; set; }
    public bool ConfirmPressed { get; set; }

    /// <summary>D-pad / arrow / stick digital UI navigation pulses.</summary>
    public bool NavUp { get; set; }
    public bool NavDown { get; set; }
    public bool NavLeft { get; set; }
    public bool NavRight { get; set; }
    public bool FocusNext { get; set; } // Tab
    public bool FocusPrev { get; set; } // Shift+Tab

    public InputDeviceKind LastDevice { get; set; } = InputDeviceKind.Touch;
    public bool PreferLargeTargets => LastDevice == InputDeviceKind.Touch || LastDevice == InputDeviceKind.Controller;
    public bool ShowHover => LastDevice == InputDeviceKind.Mouse;
    public bool ShowFocusRing => LastDevice == InputDeviceKind.Keyboard || LastDevice == InputDeviceKind.Controller;
    public bool ShowShortcutHints => LastDevice == InputDeviceKind.Keyboard || LastDevice == InputDeviceKind.Controller;

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
        NavUp = NavDown = NavLeft = NavRight = false;
        FocusNext = FocusPrev = false;
        IsLongPress = false;
        SwipeDelta = Vector2.Zero;
        ScrollDelta = Vector2.Zero;
    }
}

public sealed class InputManager
{
    private readonly InputState _state = new();
    private Vector2 _lastPointer;
    private Vector2 _pressOrigin;
    private float _holdTimer;
    private bool _holding;
    private bool _touchReady;
    private float _navCooldown;
    private const float LongPressSeconds = 0.45f;
    private const float NavRepeatSeconds = 0.18f;
    private const float StickDead = 0.55f;

    public InputState Current => _state;

    public void Update()
    {
        float dt = 1f / 60f; // scenes pass real dt separately for hold if needed; stable step OK here
        _state.BeginFrame();
        if (_navCooldown > 0) _navCooldown -= dt;

        try { _state.Keyboard = Keyboard.GetState(); } catch { _state.Keyboard = default; }
        try { _state.Mouse = Mouse.GetState(); } catch { _state.Mouse = default; }
        try { _state.GamePad = GamePad.GetState(PlayerIndex.One); } catch { _state.GamePad = default; }

        TouchCollection touches = default;
        try
        {
            touches = TouchPanel.GetState();
            _touchReady = true;
        }
        catch { _touchReady = false; }
        _state.Touches = touches;

        bool usedTouch = false;
        if (_touchReady && touches.Count > 0)
        {
            var t = touches[0];
            _state.PointerPosition = t.Position.ToPoint();
            _state.IsPointerDown = t.State == TouchLocationState.Pressed || t.State == TouchLocationState.Moved;
            _state.IsPointerPressed = t.State == TouchLocationState.Pressed;
            _state.IsPointerReleased = t.State == TouchLocationState.Released;
            usedTouch = true;
            _state.LastDevice = InputDeviceKind.Touch;
        }
        else
        {
            _state.PointerPosition = _state.Mouse.Position;
            _state.IsPointerDown = _state.Mouse.LeftButton == ButtonState.Pressed;
            _state.IsPointerPressed = _state.Mouse.LeftButton == ButtonState.Pressed &&
                                      _state.PreviousMouse.LeftButton == ButtonState.Released;
            _state.IsPointerReleased = _state.Mouse.LeftButton == ButtonState.Released &&
                                       _state.PreviousMouse.LeftButton == ButtonState.Pressed;

            int scroll = _state.Mouse.ScrollWheelValue - _state.PreviousMouse.ScrollWheelValue;
            if (scroll != 0)
            {
                _state.ScrollDelta = new Vector2(0, scroll / 120f);
                _state.LastDevice = InputDeviceKind.Mouse;
            }
            if (_state.IsPointerMoved || _state.IsPointerPressed || _state.IsPointerReleased)
                _state.LastDevice = InputDeviceKind.Mouse;
        }

        _state.IsPointerMoved = _state.PointerPosition != _lastPointer.ToPoint();

        // Long press + swipe
        if (_state.IsPointerPressed)
        {
            _holding = true;
            _holdTimer = 0;
            _pressOrigin = _state.PointerPosition.ToVector2();
        }
        if (_holding && _state.IsPointerDown)
        {
            _holdTimer += dt;
            if (_holdTimer >= LongPressSeconds)
                _state.IsLongPress = true;
            var delta = _state.PointerPosition.ToVector2() - _pressOrigin;
            if (delta.LengthSquared() > 24 * 24)
                _state.SwipeDelta = delta;
        }
        if (_state.IsPointerReleased || !_state.IsPointerDown)
            _holding = false;

        _lastPointer = _state.PointerPosition.ToVector2();

        // Move axis
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
        catch { /* */ }
        _state.MoveAxis = new Vector2(MathHelper.Clamp(x, -1, 1), MathHelper.Clamp(y, -1, 1));

        // Gameplay buttons
        bool padA = false, padB = false, padStart = false;
        try
        {
            padA = _state.GamePad.Buttons.A == ButtonState.Pressed && _state.PreviousGamePad.Buttons.A == ButtonState.Released;
            padB = _state.GamePad.Buttons.B == ButtonState.Pressed && _state.PreviousGamePad.Buttons.B == ButtonState.Released;
            padStart = _state.GamePad.Buttons.Start == ButtonState.Pressed && _state.PreviousGamePad.Buttons.Start == ButtonState.Released;
            if (padA || padB || padStart || Math.Abs(_state.GamePad.ThumbSticks.Left.X) > StickDead)
                _state.LastDevice = InputDeviceKind.Controller;
        }
        catch { /* */ }

        _state.JumpPressed = _state.IsKeyPressed(Keys.Space) || padA;
        _state.PrimaryActionPressed = _state.IsKeyPressed(Keys.E) || _state.IsPointerPressed;
        _state.InteractPressed = _state.IsKeyPressed(Keys.F);
        _state.MenuPressed = _state.IsKeyPressed(Keys.Escape) || padStart;
        _state.CancelPressed = _state.IsKeyPressed(Keys.Escape) || padB;
        _state.ConfirmPressed = _state.IsKeyPressed(Keys.Enter) || padA;

        if (_state.IsKeyPressed(Keys.Escape) || _state.IsKeyPressed(Keys.Enter) ||
            _state.IsKeyPressed(Keys.Tab) || _state.IsKeyPressed(Keys.Up) || _state.IsKeyPressed(Keys.Down))
            _state.LastDevice = InputDeviceKind.Keyboard;

        // Focus / nav pulses
        PulseNavFromKeyboard();
        PulseNavFromPad();
        if (_state.IsKeyPressed(Keys.Tab))
        {
            if (_state.IsKeyDown(Keys.LeftShift) || _state.IsKeyDown(Keys.RightShift))
                _state.FocusPrev = true;
            else
                _state.FocusNext = true;
        }
    }

    private void PulseNavFromKeyboard()
    {
        if (_state.IsKeyPressed(Keys.Up)) { _state.NavUp = true; _state.LastDevice = InputDeviceKind.Keyboard; }
        if (_state.IsKeyPressed(Keys.Down)) { _state.NavDown = true; _state.LastDevice = InputDeviceKind.Keyboard; }
        if (_state.IsKeyPressed(Keys.Left)) { _state.NavLeft = true; _state.LastDevice = InputDeviceKind.Keyboard; }
        if (_state.IsKeyPressed(Keys.Right)) { _state.NavRight = true; _state.LastDevice = InputDeviceKind.Keyboard; }
    }

    private void PulseNavFromPad()
    {
        if (_navCooldown > 0) return;
        try
        {
            var gp = _state.GamePad;
            var prev = _state.PreviousGamePad;
            bool up = (gp.DPad.Up == ButtonState.Pressed && prev.DPad.Up == ButtonState.Released) ||
                      (gp.ThumbSticks.Left.Y > StickDead && prev.ThumbSticks.Left.Y <= StickDead);
            bool down = (gp.DPad.Down == ButtonState.Pressed && prev.DPad.Down == ButtonState.Released) ||
                        (gp.ThumbSticks.Left.Y < -StickDead && prev.ThumbSticks.Left.Y >= -StickDead);
            bool left = (gp.DPad.Left == ButtonState.Pressed && prev.DPad.Left == ButtonState.Released) ||
                        (gp.ThumbSticks.Left.X < -StickDead && prev.ThumbSticks.Left.X >= -StickDead);
            bool right = (gp.DPad.Right == ButtonState.Pressed && prev.DPad.Right == ButtonState.Released) ||
                         (gp.ThumbSticks.Left.X > StickDead && prev.ThumbSticks.Left.X <= StickDead);
            if (up) { _state.NavUp = true; _state.LastDevice = InputDeviceKind.Controller; _navCooldown = NavRepeatSeconds; }
            if (down) { _state.NavDown = true; _state.LastDevice = InputDeviceKind.Controller; _navCooldown = NavRepeatSeconds; }
            if (left) { _state.NavLeft = true; _state.LastDevice = InputDeviceKind.Controller; _navCooldown = NavRepeatSeconds; }
            if (right) { _state.NavRight = true; _state.LastDevice = InputDeviceKind.Controller; _navCooldown = NavRepeatSeconds; }
        }
        catch { /* */ }
    }
}
