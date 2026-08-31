using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Voxelion.Core.Input
{
    public struct InputState
    {
        public Vector2 PointerPosition;
        public bool PointerDown;
        public bool PointerPressed;
        public bool PointerReleased;
        public bool PrimaryAction;
        public bool SecondaryAction;
        public bool Back;
        public bool Confirm;
        public bool Cancel;
        public Vector2 Move;
        public bool Jump;
        public int ScrollDelta;
        public bool IsTouch;
        public bool IsMouse;
        public bool IsKeyboard;
        public bool IsGamepad;
        public PlayerIndex ActivePad;
    }

    /// <summary>
    /// Platform-agnostic input abstraction.
    /// Touch, mouse, keyboard, controller unified.
    /// </summary>
    public class InputManager
    {
        private MouseState _prevMouse;
        private KeyboardState _prevKey;
        private GamePadState _prevPad;
        private TouchCollection _prevTouch;
        public InputState Current { get; private set; }

        public void Initialize()
        {
            TouchPanel.EnabledGestures = GestureType.Tap | GestureType.Hold | GestureType.FreeDrag;
        }

        public void Update(GameTime gameTime)
        {
            var mouse = Mouse.GetState();
            var key = Keyboard.GetState();
            var pad = GamePad.GetState(PlayerIndex.One);
            var touch = TouchPanel.GetState();

            var state = new InputState();

            // Pointer priority: touch > mouse
            if (touch.Count > 0)
            {
                state.IsTouch = true;
                var t = touch[0];
                state.PointerPosition = t.Position;
                state.PointerDown = t.State == TouchLocationState.Pressed || t.State == TouchLocationState.Moved;
                state.PointerPressed = t.State == TouchLocationState.Pressed;
                state.PointerReleased = t.State == TouchLocationState.Released;
            }
            else
            {
                state.IsMouse = true;
                state.PointerPosition = mouse.Position.ToVector2();
                state.PointerDown = mouse.LeftButton == ButtonState.Pressed;
                state.PointerPressed = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;
                state.PointerReleased = mouse.LeftButton == ButtonState.Released && _prevMouse.LeftButton == ButtonState.Pressed;
                state.ScrollDelta = mouse.ScrollWheelValue - _prevMouse.ScrollWheelValue;
            }

            // Keyboard / Gamepad actions
            state.IsKeyboard = true;
            state.Confirm = key.IsKeyDown(Keys.Enter) || key.IsKeyDown(Keys.Space) ||
                            pad.Buttons.A == ButtonState.Pressed;
            state.Cancel = key.IsKeyDown(Keys.Escape) || pad.Buttons.B == ButtonState.Pressed;
            state.Back = key.IsKeyDown(Keys.Escape) || pad.Buttons.Back == ButtonState.Pressed ||
                         (state.IsTouch == false && key.IsKeyDown(Keys.Back));
            state.PrimaryAction = key.IsKeyDown(Keys.E) || pad.Buttons.X == ButtonState.Pressed;
            state.Jump = key.IsKeyDown(Keys.Space) || pad.Buttons.A == ButtonState.Pressed;
            state.Move = new Vector2(
                (key.IsKeyDown(Keys.D) || key.IsKeyDown(Keys.Right) ? 1f : 0f) -
                (key.IsKeyDown(Keys.A) || key.IsKeyDown(Keys.Left) ? 1f : 0f) +
                pad.ThumbSticks.Left.X,
                (key.IsKeyDown(Keys.S) || key.IsKeyDown(Keys.Down) ? 1f : 0f) -
                (key.IsKeyDown(Keys.W) || key.IsKeyDown(Keys.Up) ? 1f : 0f) -
                pad.ThumbSticks.Left.Y);

            if (pad.IsConnected) state.IsGamepad = true;

            Current = state;
            _prevMouse = mouse;
            _prevKey = key;
            _prevPad = pad;
            _prevTouch = touch;
        }
    }
}
