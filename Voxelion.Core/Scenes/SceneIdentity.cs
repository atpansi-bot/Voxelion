using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneIdentity : SceneBase
{
    private string _name = "";
    private string _status = "";
    private bool _valid;
    private float _checkTimer;
    private bool _checking;

    public SceneIdentity(VoxelionGame game) : base(game)
    {
        _name = game.Profile.DisplayName;
        if (string.IsNullOrEmpty(_name))
            _name = $"Wanderer_{Random.Shared.Next(1000, 9999)}";
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Simulated typing via keyboard for PC
        var keys = input.Keyboard.GetPressedKeys();
        foreach (var k in keys)
        {
            if (input.IsKeyPressed(k))
            {
                if (k >= Microsoft.Xna.Framework.Input.Keys.A && k <= Microsoft.Xna.Framework.Input.Keys.Z && _name.Length < 16)
                {
                    char c = (char)('A' + (k - Microsoft.Xna.Framework.Input.Keys.A));
                    if (!input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) && !input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift))
                        c = char.ToLower(c);
                    _name += c;
                    _checking = true;
                    _checkTimer = 0;
                    _valid = false;
                    _status = Game.Loc["identity.checking"];
                }
                else if (k == Microsoft.Xna.Framework.Input.Keys.Back && _name.Length > 0)
                {
                    _name = _name[..^1];
                    _checking = true;
                    _checkTimer = 0;
                    _valid = false;
                }
            }
        }

        if (_checking)
        {
            _checkTimer += dt;
            if (_checkTimer > 0.6f)
            {
                _checking = false;
                if (_name.Length < 3) { _status = Game.Loc["identity.short"]; _valid = false; }
                else if (_name.Length > 16) { _status = Game.Loc["identity.long"]; _valid = false; }
                else if (_name.Any(c => !char.IsLetterOrDigit(c) && c != '_')) { _status = Game.Loc["identity.invalid"]; _valid = false; }
                else { _status = Game.Loc["identity.available"]; _valid = true; }
            }
        }

        var confirmR = new Rectangle((int)(Game.GraphicsDevice.Viewport.Width * 0.5f - 120), (int)(Game.GraphicsDevice.Viewport.Height * 0.62f), 240, 48);
        if (input.IsPointerPressed && confirmR.Contains(input.PointerPosition) && _valid)
        {
            Game.Profile.DisplayName = _name;
            Game.TransitionTo(ApplicationState.Welcome);
        }

        if (input.CancelPressed) Game.GoBack();
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height, cx = w * 0.5f;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.DeepNight);

        string title = Game.Loc["identity.choose"];
        var ts = Game.MeasureText(title, 1.4f);
        Game.DrawText(sb, title, new Vector2(cx - ts.X * 0.5f, h * 0.28f), DesignTokens.Color.TextPrimary, 1.4f);

        // Input field
        var field = new Rectangle((int)(cx - 160), (int)(h * 0.42f), 320, 48);
        Game.DrawRect(sb, field, DesignTokens.Color.PanelBase);
        Game.DrawBorder(sb, field, _valid ? DesignTokens.Color.AccentSuccess : DesignTokens.Color.BorderFocus, 2);
        Game.DrawText(sb, _name + (_checking ? "" : "|"), new Vector2(field.X + 16, field.Y + 14), DesignTokens.Color.TextPrimary, 1.1f);

        // Status
        Color sc = _valid ? DesignTokens.Color.AccentSuccess : DesignTokens.Color.TextMuted;
        var ss = Game.MeasureText(_status, 0.85f);
        Game.DrawText(sb, _status, new Vector2(cx - ss.X * 0.5f, h * 0.52f), sc, 0.85f);

        // Confirm
        var confirmR = new Rectangle((int)(cx - 120), (int)(h * 0.62f), 240, 48);
        Color bg = _valid ? DesignTokens.Color.AccentPrimary * 0.8f : DesignTokens.Color.PanelBase;
        Game.DrawRect(sb, confirmR, bg);
        Game.DrawBorder(sb, confirmR, DesignTokens.Color.BorderSubtle, 1);
        string conf = Game.Loc["identity.confirm"];
        var cs = Game.MeasureText(conf, 1.0f);
        Game.DrawText(sb, conf, new Vector2(confirmR.X + (confirmR.Width - cs.X) * 0.5f, confirmR.Y + 14),
            _valid ? DesignTokens.Color.TextPrimary : DesignTokens.Color.TextDisabled, 1.0f);
    }
}
