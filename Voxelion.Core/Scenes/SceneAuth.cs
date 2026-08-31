using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneAuth : SceneBase
{
    private Rectangle _btnGuest, _btnSignIn, _btnCreate, _btnBack;
    private string _status = "";
    private float _statusTimer;

    public SceneAuth(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        _status = "";
        Layout();
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        float cx = vp.Width * 0.5f;
        float bw = Math.Min(300f, SafeLayout.SafeWidth(vp) * 0.6f);
        float bh = Math.Max(SafeLayout.TouchMin, 50f);
        float y = vp.Height * 0.38f;
        float gap = 12f;
        _btnGuest = new Rectangle((int)(cx - bw * 0.5f), (int)y, (int)bw, (int)bh);
        _btnSignIn = new Rectangle((int)(cx - bw * 0.5f), (int)(y + bh + gap), (int)bw, (int)bh);
        _btnCreate = new Rectangle((int)(cx - bw * 0.5f), (int)(y + 2 * (bh + gap)), (int)bw, (int)bh);
        _btnBack = new Rectangle((int)(cx - bw * 0.5f), (int)(y + 3 * (bh + gap) + 8), (int)bw, (int)(bh * 0.85f));
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();
        if (_statusTimer > 0) _statusTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (!input.IsPointerReleased) return;
        var p = input.PointerPosition;

        if (_btnGuest.Contains(p))
        {
            if (!Game.Session.IsNetworkAvailable)
            {
                _status = "NETWORK UNAVAILABLE";
                _statusTimer = 2f;
                Game.Toasts.Push("OFFLINE MODE", ToastKind.Warning);
            }
            Game.Session.CreateGuestSession(Game.Profile);
            Game.Toasts.Push("GUEST SESSION", ToastKind.Success);
            Game.TransitionTo(ApplicationState.CharacterCreation);
            return;
        }
        if (_btnSignIn.Contains(p))
        {
            // Offline prototype: guest-equivalent
            Game.Session.CreateGuestSession(Game.Profile);
            Game.Toasts.Push("SIGNED IN", ToastKind.Success);
            Game.TransitionTo(Game.Profile.HasCharacter ? ApplicationState.Hub : ApplicationState.CharacterCreation);
            return;
        }
        if (_btnCreate.Contains(p))
        {
            Game.TransitionTo(ApplicationState.Registration);
            return;
        }
        if (_btnBack.Contains(p))
            Game.TransitionTo(ApplicationState.Title);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height, cx = w * 0.5f;
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.DeepNight);
        UiKit.CenterLabel(Game, sb, "ACCOUNT", h * 0.14f, DesignTokens.Color.TextPrimary, 3f, w);
        UiKit.CenterLabel(Game, sb, "PLAY INSTANTLY OR LINK AN ACCOUNT", h * 0.24f, DesignTokens.Color.TextSecondary, 1.3f, w);
        DrawBtn(sb, _btnGuest, "CONTINUE AS GUEST", DesignTokens.Color.AccentPrimary);
        DrawBtn(sb, _btnSignIn, "SIGN IN", DesignTokens.Color.PanelElevated);
        DrawBtn(sb, _btnCreate, "CREATE ACCOUNT", DesignTokens.Color.PanelElevated);
        DrawBtn(sb, _btnBack, "BACK", DesignTokens.Color.PanelBase);
        if (_statusTimer > 0 && _status.Length > 0)
            UiKit.CenterLabel(Game, sb, _status, h * 0.72f, DesignTokens.Color.AccentWarning, 1.4f, w);
        UiKit.CenterLabel(Game, sb, "GUEST PROGRESS CAN BE LINKED LATER", h * 0.90f, DesignTokens.Color.TextMuted, 1.2f, w);
    }

    private void DrawBtn(SpriteBatch sb, Rectangle r, string label, Color fill)
    {
        Game.DrawRect(sb, r, fill);
        Game.DrawBorder(sb, r, DesignTokens.Color.BorderFocus, 2);
        float scale = label.Length > 16 ? 1.5f : 1.8f;
        var size = Game.MeasureText(label, scale);
        Game.DrawText(sb, label, new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + (r.Height - size.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, scale);
    }
}
