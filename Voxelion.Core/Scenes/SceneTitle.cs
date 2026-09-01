using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.Localization;
using Voxelion.Core.UI;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

/// <summary>Title — FocusNav + InputAdaptive for all devices.</summary>
public sealed class SceneTitle : SceneBase
{
    private Rectangle _btnPlay, _btnAccount, _btnSettings, _btnLang;
    private readonly FocusNav _focus = new();
    private readonly Language[] _langs =
    {
        Language.English, Language.BahasaIndonesia, Language.Japanese, Language.Chinese, Language.Korean
    };
    private readonly string[] _langCodes = { "EN", "ID", "JA", "ZH", "KO" };

    public SceneTitle(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        Relayout();
    }

    private void Relayout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        UI.Layout.Update(vp);

        float bw = Math.Min(280f, UI.Layout.Safe.Width * 0.55f);
        float bh = Math.Max(DesignTokens.Layout.MinTouchTarget, DesignTokens.Component.ButtonHeight);

        var play = UI.Layout.Box(LayoutBox.Default
            .WithAnchor(Anchor.Center)
            .WithSize(bw, bh)
            .WithRelative(0.5f, 0.58f));
        _btnPlay = play;
        _btnAccount = new Rectangle(play.X, play.Y + play.Height + (int)DesignTokens.Spacing.M, play.Width, play.Height);
        _btnSettings = new Rectangle(play.X, _btnAccount.Y + play.Height + (int)DesignTokens.Spacing.M, play.Width, play.Height);
        _btnLang = UI.Layout.Box(LayoutBox.Default
            .WithAnchor(Anchor.TopRight)
            .WithSize(72, 40)
            .WithMargin(new Thickness(DesignTokens.Spacing.M)));
        _btnLang = UI.Layout.ClampToSafe(_btnLang);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Relayout();

        float bh = InputAdaptive.TargetHeight(input);
        if (Math.Abs(_btnPlay.Height - bh) > 1)
        {
            _btnPlay = new Rectangle(_btnPlay.X, _btnPlay.Y, _btnPlay.Width, (int)bh);
            _btnAccount = new Rectangle(_btnAccount.X, _btnPlay.Bottom + (int)DesignTokens.Spacing.M, _btnAccount.Width, (int)bh);
            _btnSettings = new Rectangle(_btnSettings.X, _btnAccount.Bottom + (int)DesignTokens.Spacing.M, _btnSettings.Width, (int)bh);
        }

        _focus.Clear();
        _focus.Register("play", _btnPlay, 0);
        _focus.Register("account", _btnAccount, 1);
        _focus.Register("settings", _btnSettings, 2);
        _focus.Register("lang", _btnLang, 3);
        _focus.EndRegister();
        _focus.Update(input);

        if (input.CancelPressed)
            return;

        if (_focus.Activated(input, "lang", _btnLang))
        {
            int idx = Array.IndexOf(_langs, Game.Loc.Current);
            idx = (idx + 1) % _langs.Length;
            Game.Loc.Current = _langs[idx];
            Game.Toasts.Push("LANGUAGE " + _langCodes[idx], ToastKind.Info);
            return;
        }
        if (_focus.Activated(input, "play", _btnPlay))
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
        if (_focus.Activated(input, "account", _btnAccount))
        {
            Game.TransitionTo(ApplicationState.Authentication);
            return;
        }
        if (_focus.Activated(input, "settings", _btnSettings))
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

        VisualChrome.Button(Game, sb, _btnPlay, "PLAY", true, _focus.IsFocused("play"), false, DesignTokens.Typography.ButtonLarge);
        VisualChrome.Button(Game, sb, _btnAccount, "ACCOUNT", false, _focus.IsFocused("account"), false, DesignTokens.Typography.Button);
        VisualChrome.Button(Game, sb, _btnSettings, "SETTINGS", false, _focus.IsFocused("settings"), false, DesignTokens.Typography.Button);

        VisualChrome.Panel(Game, sb, _btnLang, elevated: true);
        int li = Math.Max(0, Array.IndexOf(_langs, Game.Loc.Current));
        var cs = Game.MeasureText(_langCodes[li], 1.5f);
        Game.DrawText(sb, _langCodes[li],
            new Vector2(_btnLang.X + (_btnLang.Width - cs.X) * 0.5f, _btnLang.Y + 12),
            DesignTokens.Semantic.TextPrimary, 1.5f);

        var ringInput = new InputState { LastDevice = InputDeviceKind.Controller };
        _focus.DrawFocus(Game, sb, ringInput, t);
        _focus.DrawShortcut(Game, sb, new InputState { LastDevice = InputDeviceKind.Keyboard }, _btnPlay, "ENTER");

        UiKit.CenterLabel(Game, sb, "V1.0.0", h * 0.94f, DesignTokens.Semantic.TextMuted, DesignTokens.Typography.Caption, w);
    }
}
