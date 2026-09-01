using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.Localization;
using Voxelion.Core.UI;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneSettings : SceneBase
{
    private Rectangle _btnBack;
    private readonly string[] _cats =
    {
        "GRAPHICS", "AUDIO", "CONTROLS", "INTERFACE", "ACCESSIBILITY",
        "LANGUAGE", "NOTIFICATIONS", "NETWORK", "PRIVACY", "ACCOUNT"
    };
    private int _cat;
    private float _uiScale = 1f;
    private bool _reduceMotion;
    private bool _highReadability;
    private readonly Language[] _langs = { Language.English, Language.BahasaIndonesia, Language.Japanese, Language.Chinese, Language.Korean };
    private readonly string[] _langCodes = { "EN", "ID", "JA", "ZH", "KO" };

    public SceneSettings(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        Layout();
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        float m = SafeLayout.Margin(vp);
        _btnBack = new Rectangle((int)m, (int)m, 100, 40);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();
        if (!input.IsPointerReleased) return;
        var p = input.PointerPosition;
        var vp = Game.GraphicsDevice.Viewport;
        float m = SafeLayout.Margin(vp);

        if (_btnBack.Contains(p))
        {
            if (Game.StateMachine.CanGoBack) Game.GoBack();
            else Game.TransitionTo(ApplicationState.Title);
            return;
        }
        for (int i = 0; i < _cats.Length; i++)
        {
            var r = new Rectangle((int)m, (int)(m + 56 + i * 36), 180, 34);
            if (r.Contains(p)) { _cat = i; return; }
        }

        // Right panel interactions for selected category
        float px = m + 200;
        if (_cat == 3) // INTERFACE
        {
            var up = new Rectangle((int)px, (int)(m + 80), 120, 40);
            var dn = new Rectangle((int)(px + 130), (int)(m + 80), 120, 40);
            if (up.Contains(p)) { _uiScale = Math.Min(1.4f, _uiScale + 0.1f); Game.Toasts.Push("UI SCALE " + (int)(_uiScale * 100), ToastKind.Info); }
            if (dn.Contains(p)) { _uiScale = Math.Max(0.8f, _uiScale - 0.1f); Game.Toasts.Push("UI SCALE " + (int)(_uiScale * 100), ToastKind.Info); }
        }
        if (_cat == 4) // ACCESSIBILITY
        {
            var rm = new Rectangle((int)px, (int)(m + 80), 200, 40);
            var hr = new Rectangle((int)px, (int)(m + 130), 200, 40);
            if (rm.Contains(p)) { _reduceMotion = !_reduceMotion; Game.Toasts.Push(_reduceMotion ? "REDUCED MOTION ON" : "REDUCED MOTION OFF", ToastKind.Info); }
            if (hr.Contains(p)) { _highReadability = !_highReadability; Game.Toasts.Push(_highReadability ? "HIGH READABILITY ON" : "HIGH READABILITY OFF", ToastKind.Info); }
        }
        if (_cat == 5) // LANGUAGE
        {
            for (int i = 0; i < _langCodes.Length; i++)
            {
                var r = new Rectangle((int)px, (int)(m + 80 + i * 44), 100, 40);
                if (r.Contains(p))
                {
                    Game.Loc.Current = _langs[i];
                    Game.Toasts.Push("LANGUAGE " + _langCodes[i], ToastKind.Success);
                }
            }
        }
        if (_cat == 7) // NETWORK
        {
            var tog = new Rectangle((int)px, (int)(m + 80), 200, 40);
            if (tog.Contains(p))
            {
                Game.Session.SetNetwork(!Game.Session.IsNetworkAvailable);
                Game.Toasts.Push(Game.Session.IsNetworkAvailable ? "NETWORK ON" : "OFFLINE", ToastKind.Warning);
            }
        }
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float m = SafeLayout.Margin(vp);
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);
        Game.DrawRect(sb, 0, 0, w, 56, DesignTokens.Color.PanelBase);
        DrawBtn(sb, _btnBack, "BACK", DesignTokens.Color.PanelElevated);
        UiKit.CenterLabel(Game, sb, "SETTINGS", 16, DesignTokens.Color.TextPrimary, 2.2f, w);

        for (int i = 0; i < _cats.Length; i++)
        {
            var r = new Rectangle((int)m, (int)(m + 56 + i * 36), 180, 34);
            var fill = i == _cat ? DesignTokens.Color.AccentPrimary : DesignTokens.Color.PanelElevated;
            Game.DrawRect(sb, r, fill);
            Game.DrawText(sb, _cats[i], new Vector2(r.X + 8, r.Y + 8), DesignTokens.Color.TextPrimary, 1.2f);
        }

        float px = m + 200;
        float py = m + 70;
        Game.DrawText(sb, _cats[_cat], new Vector2(px, py), DesignTokens.Color.AccentSecondary, 2f);

        if (_cat == 3)
        {
            Game.DrawText(sb, "UI SCALE " + (int)(_uiScale * 100) + "%", new Vector2(px, py + 40), DesignTokens.Color.TextSecondary, 1.4f);
            DrawBtn(sb, new Rectangle((int)px, (int)(py + 70), 120, 40), "PLUS", DesignTokens.Color.PanelElevated);
            DrawBtn(sb, new Rectangle((int)(px + 130), (int)(py + 70), 120, 40), "MINUS", DesignTokens.Color.PanelElevated);
        }
        else if (_cat == 4)
        {
            DrawBtn(sb, new Rectangle((int)px, (int)(py + 50), 220, 40), _reduceMotion ? "MOTION REDUCED" : "REDUCE MOTION", DesignTokens.Color.PanelElevated);
            DrawBtn(sb, new Rectangle((int)px, (int)(py + 100), 220, 40), _highReadability ? "READABILITY ON" : "HIGH READABILITY", DesignTokens.Color.PanelElevated);
        }
        else if (_cat == 5)
        {
            for (int i = 0; i < _langCodes.Length; i++)
            {
                bool sel = Game.Loc.Current == _langs[i];
                DrawBtn(sb, new Rectangle((int)px, (int)(py + 50 + i * 44), 100, 40), _langCodes[i],
                    sel ? DesignTokens.Color.AccentPrimary : DesignTokens.Color.PanelElevated);
            }
        }
        else if (_cat == 7)
        {
            string net = Game.Session.IsNetworkAvailable ? "ONLINE" : "OFFLINE";
            Game.DrawText(sb, "STATUS " + net, new Vector2(px, py + 40), DesignTokens.Color.TextSecondary, 1.4f);
            DrawBtn(sb, new Rectangle((int)px, (int)(py + 70), 200, 40), "TOGGLE NETWORK", DesignTokens.Color.PanelElevated);
        }
        else
            Game.DrawText(sb, "OPTIONS FOR THIS CATEGORY ARE UI SHELLS", new Vector2(px, py + 50), DesignTokens.Color.TextMuted, 1.3f);
    }

    private void DrawBtn(SpriteBatch sb, Rectangle r, string label, Color fill)
    {
        Game.DrawRect(sb, r, fill);
        Game.DrawBorder(sb, r, DesignTokens.Color.BorderFocus, 1);
        var size = Game.MeasureText(label, 1.3f);
        Game.DrawText(sb, label, new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + (r.Height - size.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, 1.3f);
    }
}
