using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneTitle : SceneBase
{
    private int _hovered = -1;
    private readonly string[] _buttons = { "title.play", "title.account", "title.settings", "title.credits" };
    private Rectangle[] _btnRects = Array.Empty<Rectangle>();
    private bool _returning;

    public SceneTitle(VoxelionGame game) : base(game)
    {
        _returning = game.Session.HasValidSession && game.Profile.HasCharacter;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _returning = Game.Session.HasValidSession && Game.Profile.HasCharacter;
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float cx = w * 0.5f;
        float startY = h * 0.52f;
        float btnW = 260f, btnH = 48f, gap = 12f;

        _btnRects = new Rectangle[_buttons.Length];
        _hovered = -1;

        for (int i = 0; i < _buttons.Length; i++)
        {
            float by = startY + i * (btnH + gap);
            _btnRects[i] = new Rectangle((int)(cx - btnW * 0.5f), (int)by, (int)btnW, (int)btnH);
            if (_btnRects[i].Contains(input.PointerPosition))
                _hovered = i;
        }

        if (input.IsPointerPressed && _hovered >= 0)
        {
            switch (_hovered)
            {
                case 0: // PLAY / CONTINUE
                    Game.Session.Evaluate();
                    if (Game.Session.HasValidSession && Game.Profile.HasCharacter)
                        Game.TransitionTo(ApplicationState.Hub);
                    else if (Game.Session.HasValidSession)
                        Game.TransitionTo(ApplicationState.CharacterCreation);
                    else
                        Game.TransitionTo(ApplicationState.Authentication);
                    break;
                case 1:
                    Game.TransitionTo(ApplicationState.Authentication);
                    break;
                case 2:
                    // Settings overlay would open here
                    break;
                case 3:
                    // Credits
                    break;
            }
        }

        // Language cycle on corner click (demo)
        var langRect = new Rectangle((int)(w - 90), 16, 70, 28);
        if (input.IsPointerPressed && langRect.Contains(input.PointerPosition))
        {
            Game.Loc.Current = Game.Loc.Current switch
            {
                Localization.Language.English => Localization.Language.BahasaIndonesia,
                Localization.Language.BahasaIndonesia => Localization.Language.Japanese,
                Localization.Language.Japanese => Localization.Language.Chinese,
                Localization.Language.Chinese => Localization.Language.Korean,
                _ => Localization.Language.English
            };
        }
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float t = SceneTime;
        float cx = w * 0.5f;

        // Animated fantasy landscape
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.DeepNight);

        // Terrain silhouettes
        for (int i = 0; i < 6; i++)
        {
            float baseY = h * 0.62f + i * 22;
            float wave = MathF.Sin(t * 0.2f + i * 0.7f) * 8;
            Color c = DesignTokens.Color.ShadowIndigo * (0.25f + i * 0.08f);
            Game.DrawRect(sb, 0, baseY + wave, w, h - baseY, c);
        }

        // Clouds / particles
        for (int i = 0; i < 25; i++)
        {
            float px = ((t * 8 + i * 53) % (w + 100)) - 50;
            float py = 40 + (i % 7) * 28 + MathF.Sin(t * 0.4f + i) * 6;
            Game.DrawRect(sb, px, py, 40 + (i % 4) * 12, 10, DesignTokens.Color.PanelGlass * 0.15f);
        }

        // Title
        float titleAlpha = EaseOutCubic(MathHelper.Clamp(EnterTime / 0.5f, 0, 1));
        string title = Game.Loc["app.name"];
        var ts = Game.MeasureText(title, 2.4f);
        Game.DrawText(sb, title, new Vector2(cx - ts.X * 0.5f, h * 0.22f), DesignTokens.Color.TextPrimary * titleAlpha, 2.4f);

        string tag = Game.Loc["app.tagline"];
        var tags = Game.MeasureText(tag, 0.95f);
        Game.DrawText(sb, tag, new Vector2(cx - tags.X * 0.5f, h * 0.30f), DesignTokens.Color.AccentSecondary * titleAlpha, 0.95f);

        // Buttons
        for (int i = 0; i < _buttons.Length; i++)
        {
            var r = _btnRects[i];
            bool hover = i == _hovered;
            Color bg = hover ? DesignTokens.Color.PanelElevated : DesignTokens.Color.PanelBase;
            Color border = hover ? DesignTokens.Color.BorderFocus : DesignTokens.Color.BorderSubtle;
            Color textC = hover ? DesignTokens.Color.TextPrimary : DesignTokens.Color.TextSecondary;

            Game.DrawRect(sb, r, bg);
            Game.DrawBorder(sb, r, border, hover ? 2 : 1);

            string label = Game.Loc[_buttons[i]];
            if (i == 0 && _returning) label = Game.Loc["title.continue_btn"];
            var ls = Game.MeasureText(label, 1.0f);
            Game.DrawText(sb, label, new Vector2(r.X + (r.Width - ls.X) * 0.5f, r.Y + (r.Height - ls.Y) * 0.5f), textC, 1.0f);
        }

        // Returning card
        if (_returning)
        {
            string cont = Game.Loc["title.continue"];
            var cs = Game.MeasureText(cont, 0.85f);
            Game.DrawText(sb, cont, new Vector2(cx - cs.X * 0.5f, h * 0.42f), DesignTokens.Color.TextMuted, 0.85f);
            string name = Game.Profile.DisplayName;
            var ns = Game.MeasureText(name, 1.0f);
            Game.DrawText(sb, name, new Vector2(cx - ns.X * 0.5f, h * 0.46f), DesignTokens.Color.AccentTertiary, 1.0f);
        }

        // Language
        string lang = Game.Loc.Current switch
        {
            Localization.Language.BahasaIndonesia => "ID",
            Localization.Language.Japanese => "JA",
            Localization.Language.Chinese => "ZH",
            Localization.Language.Korean => "KO",
            _ => "EN"
        };
        Game.DrawText(sb, $"[ 🌐 {lang} ]", new Vector2(w - 90, 18), DesignTokens.Color.TextSecondary, 0.8f);

        // Version
        Game.DrawText(sb, "v1.0.0", new Vector2(w * 0.5f - 30, h - 28), DesignTokens.Color.TextMuted * 0.5f, 0.7f);
    }
}
