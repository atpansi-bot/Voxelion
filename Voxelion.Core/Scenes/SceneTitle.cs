using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.Localization;
using Voxelion.Core.UI;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneTitle : SceneBase
{
    private Rectangle _btnPlay, _btnAccount, _btnSettings, _btnLang;
    private readonly Language[] _langs = { Language.English, Language.BahasaIndonesia, Language.Japanese, Language.Chinese, Language.Korean };
    private readonly string[] _langCodes = { "EN", "ID", "JA", "ZH", "KO" };

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
        float bw = Math.Min(280f, SafeLayout.SafeWidth(vp) * 0.55f);
        float bh = Math.Max(SafeLayout.TouchMin, 52f);
        float y = vp.Height * 0.52f;
        float gap = 14f;
        _btnPlay = new Rectangle((int)(cx - bw * 0.5f), (int)y, (int)bw, (int)bh);
        _btnAccount = new Rectangle((int)(cx - bw * 0.5f), (int)(y + bh + gap), (int)bw, (int)bh);
        _btnSettings = new Rectangle((int)(cx - bw * 0.5f), (int)(y + 2 * (bh + gap)), (int)bw, (int)bh);
        float m = SafeLayout.Margin(vp);
        _btnLang = new Rectangle((int)(vp.Width - m - 72), (int)m, 72, 40);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();
        if (!input.IsPointerReleased) return;
        var p = input.PointerPosition;

        if (_btnLang.Contains(p))
        {
            int idx = Array.IndexOf(_langs, Game.Loc.Current);
            idx = (idx + 1) % _langs.Length;
            Game.Loc.Current = _langs[idx];
            Game.Toasts.Push("LANGUAGE " + _langCodes[idx], ToastKind.Info);
            return;
        }
        if (_btnPlay.Contains(p))
        {
            Game.Session.Evaluate();
            if (!Game.Session.HasValidSession)
            {
                Game.TransitionTo(ApplicationState.Authentication);
                return;
            }
            if (!Game.Profile.HasCharacter)
                Game.TransitionTo(ApplicationState.CharacterCreation);
            else
                Game.TransitionTo(ApplicationState.Hub);
            return;
        }
        if (_btnAccount.Contains(p))
        {
            Game.TransitionTo(ApplicationState.Authentication);
            return;
        }
        if (_btnSettings.Contains(p))
            Game.TransitionTo(ApplicationState.Settings);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height, cx = w * 0.5f, t = SceneTime;
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);
        for (int i = 0; i < 28; i++)
        {
            float px = (MathF.Sin(t * 0.12f + i) * 0.5f + 0.5f) * w;
            float py = (MathF.Cos(t * 0.09f + i * 1.5f) * 0.5f + 0.5f) * h;
            Game.DrawRect(sb, px, py, 2, 2, DesignTokens.Color.AccentPrimary * 0.15f);
        }

        float logoY = h * 0.18f;
        float size = 48f;
        Game.DrawRect(sb, cx - size * 0.55f, logoY, size * 1.1f, size * 1.1f, DesignTokens.Color.AccentPrimary);
        Game.DrawRect(sb, cx - size * 0.28f, logoY + size * 0.27f, size * 0.56f, size * 0.56f, DesignTokens.Color.AccentSecondary);
        UiKit.CenterLabel(Game, sb, "VOXELION", logoY + size + 16, DesignTokens.Color.TextPrimary, 4f, w);
        UiKit.CenterLabel(Game, sb, "ENTER THE FRONTIER", logoY + size + 52, DesignTokens.Color.TextSecondary, 1.6f, w);

        DrawBtn(sb, _btnPlay, "PLAY", DesignTokens.Color.AccentPrimary, "play");
        DrawBtn(sb, _btnAccount, "ACCOUNT", DesignTokens.Color.PanelElevated, "account");
        DrawBtn(sb, _btnSettings, "SETTINGS", DesignTokens.Color.PanelBase, "settings");

        // Language chip
        UiKit.Panel(Game, sb, _btnLang, DesignTokens.Color.PanelElevated, DesignTokens.Color.BorderFocus, 1);
        int li = Math.Max(0, Array.IndexOf(_langs, Game.Loc.Current));
        string code = _langCodes[li];
        var cs = Game.MeasureText(code, 1.5f);
        Game.DrawText(sb, code, new Vector2(_btnLang.X + (_btnLang.Width - cs.X) * 0.5f, _btnLang.Y + 12), DesignTokens.Color.TextPrimary, 1.5f);

        UiKit.CenterLabel(Game, sb, "V1.0.0", h * 0.94f, DesignTokens.Color.TextMuted, 1.2f, w);
    }

    private void DrawBtn(SpriteBatch sb, Rectangle r, string label, Color fill, string icon)
    {
        Game.DrawRect(sb, r, fill);
        Game.DrawBorder(sb, r, DesignTokens.Color.BorderFocus, 2);
        var iconR = new Rectangle(r.X + 16, r.Y + (r.Height - 28) / 2, 28, 28);
        Game.DrawIcon(sb, icon, iconR, DesignTokens.Color.TextPrimary);
        var size = Game.MeasureText(label, 2.1f);
        Game.DrawText(sb, label, new Vector2(r.X + (r.Width - size.X) * 0.5f + 8, r.Y + (r.Height - size.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, 2.1f);
    }
}
