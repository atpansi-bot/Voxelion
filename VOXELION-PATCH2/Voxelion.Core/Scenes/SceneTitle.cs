using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneTitle : SceneBase
{
    private Rectangle _btnPlay;
    private Rectangle _btnAccount;
    private Rectangle _btnSettings;

    public SceneTitle(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        Layout();
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        float cx = vp.Width * 0.5f;
        float bw = Math.Min(280f, vp.Width * 0.55f);
        float bh = 52f;
        float startY = vp.Height * 0.52f;
        float gap = 16f;

        _btnPlay = new Rectangle((int)(cx - bw * 0.5f), (int)startY, (int)bw, (int)bh);
        _btnAccount = new Rectangle((int)(cx - bw * 0.5f), (int)(startY + bh + gap), (int)bw, (int)bh);
        _btnSettings = new Rectangle((int)(cx - bw * 0.5f), (int)(startY + 2 * (bh + gap)), (int)bw, (int)bh);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();

        if (!input.IsPointerReleased) return;

        if (_btnPlay.Contains(input.PointerPosition))
        {
            if (!Game.Session.HasValidSession)
                Game.Session.CreateGuestSession(Game.Profile);
            if (!Game.Profile.HasCharacter)
                Game.TransitionTo(ApplicationState.CharacterCreation);
            else
                Game.TransitionTo(ApplicationState.Hub);
            return;
        }
        if (_btnAccount.Contains(input.PointerPosition))
        {
            Game.TransitionTo(ApplicationState.Authentication);
            return;
        }
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float cx = w * 0.5f;
        float t = SceneTime;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);

        // ambient particles
        for (int i = 0; i < 28; i++)
        {
            float px = (MathF.Sin(t * 0.12f + i) * 0.5f + 0.5f) * w;
            float py = (MathF.Cos(t * 0.09f + i * 1.5f) * 0.5f + 0.5f) * h;
            Game.DrawRect(sb, px, py, 2, 2, DesignTokens.Color.AccentPrimary * 0.15f);
        }

        // Logo
        float logoY = h * 0.22f;
        float size = 48f;
        Game.DrawRect(sb, cx - size * 0.55f, logoY, size * 1.1f, size * 1.1f, DesignTokens.Color.AccentPrimary);
        Game.DrawRect(sb, cx - size * 0.28f, logoY + size * 0.27f, size * 0.56f, size * 0.56f, DesignTokens.Color.AccentSecondary);

        string title = "VOXELION";
        var ts = Game.MeasureText(title, 4f);
        Game.DrawText(sb, title, new Vector2(cx - ts.X * 0.5f, logoY + size + 16),
            DesignTokens.Color.TextPrimary, 4f);

        string tag = "ENTER THE FRONTIER";
        var tgs = Game.MeasureText(tag, 1.6f);
        Game.DrawText(sb, tag, new Vector2(cx - tgs.X * 0.5f, logoY + size + 52),
            DesignTokens.Color.TextSecondary, 1.6f);

        // Buttons
        DrawBtn(sb, _btnPlay, "PLAY", DesignTokens.Color.AccentPrimary);
        DrawBtn(sb, _btnAccount, "ACCOUNT", DesignTokens.Color.PanelElevated);
        DrawBtn(sb, _btnSettings, "SETTINGS", DesignTokens.Color.PanelBase);

        string ver = "V1.0.0";
        var vs = Game.MeasureText(ver, 1.2f);
        Game.DrawText(sb, ver, new Vector2(cx - vs.X * 0.5f, h * 0.94f),
            DesignTokens.Color.TextMuted, 1.2f);
    }

    private void DrawBtn(SpriteBatch sb, Rectangle r, string label, Color fill)
    {
        Game.DrawRect(sb, r, fill);
        Game.DrawBorder(sb, r, DesignTokens.Color.BorderFocus, 2);
        var size = Game.MeasureText(label, 2.2f);
        Game.DrawText(sb, label,
            new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + (r.Height - size.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, 2.2f);
    }
}
