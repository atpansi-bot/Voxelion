using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneAuth : SceneBase
{
    private Rectangle _btnGuest;
    private Rectangle _btnSignIn;
    private Rectangle _btnCreate;
    private Rectangle _btnBack;

    public SceneAuth(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        Layout();
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        float cx = vp.Width * 0.5f;
        float bw = Math.Min(300f, vp.Width * 0.6f);
        float bh = 50f;
        float y = vp.Height * 0.40f;
        float gap = 14f;

        _btnGuest = new Rectangle((int)(cx - bw * 0.5f), (int)y, (int)bw, (int)bh);
        _btnSignIn = new Rectangle((int)(cx - bw * 0.5f), (int)(y + bh + gap), (int)bw, (int)bh);
        _btnCreate = new Rectangle((int)(cx - bw * 0.5f), (int)(y + 2 * (bh + gap)), (int)bw, (int)bh);
        _btnBack = new Rectangle((int)(cx - bw * 0.5f), (int)(y + 3 * (bh + gap) + 8), (int)bw, (int)(bh * 0.85f));
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();

        if (!input.IsPointerReleased) return;

        if (_btnGuest.Contains(input.PointerPosition))
        {
            Game.Session.CreateGuestSession(Game.Profile);
            Game.TransitionTo(ApplicationState.CharacterCreation);
            return;
        }
        if (_btnSignIn.Contains(input.PointerPosition) || _btnCreate.Contains(input.PointerPosition))
        {
            // Offline prototype: treat as guest then character
            Game.Session.CreateGuestSession(Game.Profile);
            Game.TransitionTo(ApplicationState.CharacterCreation);
            return;
        }
        if (_btnBack.Contains(input.PointerPosition))
        {
            Game.TransitionTo(ApplicationState.Title);
        }
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float cx = w * 0.5f;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.DeepNight);

        string title = "ACCOUNT";
        var ts = Game.MeasureText(title, 3f);
        Game.DrawText(sb, title, new Vector2(cx - ts.X * 0.5f, h * 0.18f),
            DesignTokens.Color.TextPrimary, 3f);

        string sub = "PLAY INSTANTLY OR LINK AN ACCOUNT";
        var ss = Game.MeasureText(sub, 1.3f);
        Game.DrawText(sb, sub, new Vector2(cx - ss.X * 0.5f, h * 0.28f),
            DesignTokens.Color.TextSecondary, 1.3f);

        DrawBtn(sb, _btnGuest, "CONTINUE AS GUEST", DesignTokens.Color.AccentPrimary);
        DrawBtn(sb, _btnSignIn, "SIGN IN", DesignTokens.Color.PanelElevated);
        DrawBtn(sb, _btnCreate, "CREATE ACCOUNT", DesignTokens.Color.PanelElevated);
        DrawBtn(sb, _btnBack, "BACK", DesignTokens.Color.PanelBase);
    }

    private void DrawBtn(SpriteBatch sb, Rectangle r, string label, Color fill)
    {
        Game.DrawRect(sb, r, fill);
        Game.DrawBorder(sb, r, DesignTokens.Color.BorderFocus, 2);
        float scale = label.Length > 14 ? 1.6f : 2f;
        var size = Game.MeasureText(label, scale);
        Game.DrawText(sb, label,
            new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + (r.Height - size.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, scale);
    }
}
