using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.Data;
using Voxelion.Core.Localization;
using Voxelion.Core.UI;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneSettings : SceneBase
{
    private Rectangle _btnBack;
    private readonly string[] _catKeys =
    {
        "settings.cat.graphics", "settings.cat.audio", "settings.cat.controls",
        "settings.cat.interface", "settings.cat.accessibility", "settings.cat.language",
        "settings.cat.notifications", "settings.cat.network", "settings.cat.privacy", "settings.cat.account"
    };
    private int _cat = 5; // default open Language so selector is visible
    private readonly LanguageSelector _lang = new() { Presentation = LanguageSelector.Mode.List };
    private readonly FocusNav _focus = new();

    public SceneSettings(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        Relayout();
    }

    private void Relayout()
    {
        UI.Layout.Update(Game.GraphicsDevice.Viewport);
        _btnBack = UI.Layout.ClampToSafe(UI.Layout.Box(LayoutBox.Default
            .WithAnchor(Anchor.TopLeft)
            .WithSize(120, 40)
            .WithMargin(new Thickness(DesignTokens.Spacing.M))));

        float panelX = UI.Layout.Safe.X + 200;
        float panelW = UI.Layout.Safe.Width - 220;
        _lang.Bounds = new Rectangle((int)panelX, UI.Layout.Safe.Y + 72, (int)Math.Min(panelW, 420), (int)_lang.RowHeight);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Relayout();

        _focus.Clear();
        _focus.Register("back", _btnBack, 0);
        _focus.EndRegister();
        _focus.Update(input);

        if (_focus.Activated(input, "back", _btnBack) || input.CancelPressed)
        {
            if (Game.StateMachine.CanGoBack) Game.GoBack();
            else Game.TransitionTo(ApplicationState.Title);
            return;
        }

        if (!input.IsPointerReleased) return;
        var p = input.PointerPosition;

        for (int i = 0; i < _catKeys.Length; i++)
        {
            var r = CatRect(i);
            if (r.Contains(p)) { _cat = i; return; }
        }

        if (_cat == 3) // interface
        {
            float px = UI.Layout.Safe.X + 200;
            float py = UI.Layout.Safe.Y + 80;
            var up = new Rectangle((int)px, (int)py, 120, 40);
            var dn = new Rectangle((int)(px + 130), (int)py, 120, 40);
            if (up.Contains(p))
            {
                Game.Settings.UiScale = Math.Min(1.4f, Game.Settings.UiScale + 0.1f);
                Game.Settings.Save();
                Game.Toasts.Push(Game.Loc.T("settings.ui_scale", (int)(Game.Settings.UiScale * 100)), ToastKind.Info);
            }
            if (dn.Contains(p))
            {
                Game.Settings.UiScale = Math.Max(0.8f, Game.Settings.UiScale - 0.1f);
                Game.Settings.Save();
                Game.Toasts.Push(Game.Loc.T("settings.ui_scale", (int)(Game.Settings.UiScale * 100)), ToastKind.Info);
            }
        }
        if (_cat == 4)
        {
            float px = UI.Layout.Safe.X + 200;
            float py = UI.Layout.Safe.Y + 80;
            var rm = new Rectangle((int)px, (int)py, 220, 40);
            var hr = new Rectangle((int)px, (int)(py + 50), 220, 40);
            if (rm.Contains(p))
            {
                Game.Settings.ReduceMotion = !Game.Settings.ReduceMotion;
                Game.Settings.Save();
            }
            if (hr.Contains(p))
            {
                Game.Settings.HighReadability = !Game.Settings.HighReadability;
                Game.Settings.Save();
            }
        }
        if (_cat == 5)
        {
            _lang.Update(input, Game.Loc, lang =>
            {
                Game.Settings.Language = lang;
                Game.Settings.Save();
                Game.Toasts.Push(Game.Loc.T("lang.changed", LanguageInfo.Get(lang).Code), ToastKind.Success);
            });
        }
        if (_cat == 7)
        {
            float px = UI.Layout.Safe.X + 200;
            var tog = new Rectangle((int)px, UI.Layout.Safe.Y + 80, 200, 40);
            if (tog.Contains(p))
            {
                Game.Session.SetNetwork(!Game.Session.IsNetworkAvailable);
                Game.Toasts.Push(Game.Session.IsNetworkAvailable
                    ? Game.Loc.T("settings.network_on")
                    : Game.Loc.T("settings.network_off"), ToastKind.Warning);
            }
        }
    }

    private Rectangle CatRect(int i)
    {
        var safe = UI.Layout.Safe;
        return new Rectangle(safe.X + 8, safe.Y + 56 + i * 36, 180, 34);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        Game.DrawRect(sb, 0, 0, vp.Width, vp.Height, DesignTokens.Semantic.Background);
        VisualChrome.AmbientDust(Game, sb, vp, SceneTime, 20);

        VisualChrome.Button(Game, sb, _btnBack, Game.Loc.T("settings.back"), false, false, false, TypeScale.Label);
        UiKit.CenterLabel(Game, sb, Game.Loc.T("settings.title"), UI.Layout.Safe.Y + 8,
            DesignTokens.Semantic.TextPrimary, TypeScale.Title, vp.Width);

        for (int i = 0; i < _catKeys.Length; i++)
        {
            var r = CatRect(i);
            bool sel = i == _cat;
            VisualChrome.Tab(Game, sb, r, Game.Loc.T(_catKeys[i]), sel);
        }

        float px = UI.Layout.Safe.X + 200;
        float py = UI.Layout.Safe.Y + 72;

        if (_cat == 5)
        {
            _lang.Presentation = LanguageSelector.Mode.List;
            _lang.Bounds = new Rectangle((int)px, (int)py, (int)Math.Min(UI.Layout.Safe.Width - 220, 440), (int)_lang.RowHeight);
            _lang.Draw(Game, sb, Game.Loc, null);
        }
        else if (_cat == 3)
        {
            gameLabel(sb, Game.Loc.T("settings.ui_scale", (int)(Game.Settings.UiScale * 100)), px, py);
            VisualChrome.Button(Game, sb, new Rectangle((int)px, (int)(py + 40), 120, 40), "+", false, false, false, TypeScale.Label);
            VisualChrome.Button(Game, sb, new Rectangle((int)(px + 130), (int)(py + 40), 120, 40), "-", false, false, false, TypeScale.Label);
        }
        else if (_cat == 4)
        {
            VisualChrome.Button(Game, sb, new Rectangle((int)px, (int)py, 240, 40),
                Game.Settings.ReduceMotion ? "REDUCED MOTION ON" : "REDUCED MOTION OFF", false, false, false, TypeScale.Caption);
            VisualChrome.Button(Game, sb, new Rectangle((int)px, (int)(py + 50), 240, 40),
                Game.Settings.HighReadability ? "HIGH READABILITY ON" : "HIGH READABILITY OFF", false, false, false, TypeScale.Caption);
        }
        else if (_cat == 7)
        {
            VisualChrome.Button(Game, sb, new Rectangle((int)px, (int)py, 220, 40),
                Game.Session.IsNetworkAvailable ? Game.Loc.T("settings.network_on") : Game.Loc.T("settings.network_off"),
                false, false, false, TypeScale.Label);
        }
        else
        {
            gameLabel(sb, Game.Loc.T(_catKeys[_cat]), px, py);
        }
    }

    private void gameLabel(SpriteBatch sb, string t, float x, float y) =>
        Game.DrawText(sb, t, new Vector2(x, y), DesignTokens.Semantic.TextSecondary, TypeScale.Body);
}
