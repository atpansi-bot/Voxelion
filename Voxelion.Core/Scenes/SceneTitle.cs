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
        float bh = Math.Max(DesignTokens.Layout.MinTouchTarget, DesignTokens.Component.ButtonHeight);
        float y = vp.Height * 0.52f;
        float gap = DesignTokens.Spacing.M;
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
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Semantic.Background);
        VisualChrome.AmbientDust(Game, sb, vp, t, 32);

        float logoY = h * 0.18f;
        VisualChrome.Emblem(Game, sb, new Vector2(cx, logoY + 40), 48f);
        UiKit.CenterLabel(Game, sb, "VOXELION", logoY + 96, DesignTokens.Semantic.TextPrimary, DesignTokens.Typography.Display, w);
        UiKit.CenterLabel(Game, sb, "ENTER THE FRONTIER", logoY + 132, DesignTokens.Semantic.TextSecondary, DesignTokens.Typography.Body, w);

        var input = new InputState(); // draw-only hover approx via pointer not needed for static
        VisualChrome.Button(Game, sb, _btnPlay, "PLAY", true, false, false, DesignTokens.Typography.ButtonLarge);
        VisualChrome.Button(Game, sb, _btnAccount, "ACCOUNT", false, false, false, DesignTokens.Typography.Button);
        VisualChrome.Button(Game, sb, _btnSettings, "SETTINGS", false, false, false, DesignTokens.Typography.Button);

        VisualChrome.Panel(Game, sb, _btnLang, elevated: true);
        int li = Math.Max(0, Array.IndexOf(_langs, Game.Loc.Current));
        var cs = Game.MeasureText(_langCodes[li], 1.5f);
        Game.DrawText(sb, _langCodes[li],
            new Vector2(_btnLang.X + (_btnLang.Width - cs.X) * 0.5f, _btnLang.Y + 12),
            DesignTokens.Semantic.TextPrimary, 1.5f);

        UiKit.CenterLabel(Game, sb, "V1.0.0", h * 0.94f, DesignTokens.Semantic.TextMuted, DesignTokens.Typography.Caption, w);
    }
}
