using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneAuth : SceneBase
{
    private int _hovered = -1;
    private Rectangle[] _rects = Array.Empty<Rectangle>();
    private readonly string[] _keys = { "auth.guest", "auth.signin", "auth.create" };

    public SceneAuth(VoxelionGame game) : base(game) { }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height, cx = w * 0.5f;
        float startY = h * 0.48f, btnW = 300f, btnH = 52f, gap = 14f;

        _rects = new Rectangle[_keys.Length];
        _hovered = -1;
        for (int i = 0; i < _keys.Length; i++)
        {
            _rects[i] = new Rectangle((int)(cx - btnW * 0.5f), (int)(startY + i * (btnH + gap)), (int)btnW, (int)btnH);
            if (_rects[i].Contains(input.PointerPosition)) _hovered = i;
        }

        if (input.IsPointerPressed && _hovered >= 0)
        {
            switch (_hovered)
            {
                case 0: // Guest
                    Game.Session.CreateGuestSession(Game.Profile);
                    Game.TransitionTo(ApplicationState.CharacterCreation);
                    break;
                case 1: // Sign in — for pure UI we treat as guest for flow
                    Game.Session.CreateGuestSession(Game.Profile);
                    Game.TransitionTo(ApplicationState.CharacterCreation);
                    break;
                case 2: // Create
                    Game.TransitionTo(ApplicationState.Registration);
                    break;
            }
        }

        if (input.CancelPressed) Game.GoBack();
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height, t = SceneTime, cx = w * 0.5f;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);

        // Gateway environment
        for (int i = 0; i < 8; i++)
        {
            float y = h * 0.3f + i * 35;
            Game.DrawRect(sb, w * 0.2f, y, w * 0.6f, 3, DesignTokens.Color.AccentPrimary * (0.05f + i * 0.02f));
        }

        // Particles
        for (int i = 0; i < 20; i++)
        {
            float px = cx + MathF.Sin(t * 0.5f + i) * 180;
            float py = h * 0.25f + MathF.Cos(t * 0.3f + i * 1.2f) * 60;
            Game.DrawRect(sb, px, py, 2, 2, DesignTokens.Color.AccentSecondary * 0.4f);
        }

        string title = Game.Loc["app.name"];
        var ts = Game.MeasureText(title, 1.8f);
        Game.DrawText(sb, title, new Vector2(cx - ts.X * 0.5f, h * 0.18f), DesignTokens.Color.TextPrimary, 1.8f);

        string sub = "Gateway to the Frontier";
        var ss = Game.MeasureText(sub, 0.85f);
        Game.DrawText(sb, sub, new Vector2(cx - ss.X * 0.5f, h * 0.26f), DesignTokens.Color.TextMuted, 0.85f);

        for (int i = 0; i < _keys.Length; i++)
        {
            var r = _rects[i];
            bool hover = i == _hovered;
            Color bg = i == 0 ? (hover ? DesignTokens.Color.AccentPrimary * 0.85f : DesignTokens.Color.AccentPrimary * 0.65f)
                              : (hover ? DesignTokens.Color.PanelElevated : DesignTokens.Color.PanelBase);
            Color border = hover ? DesignTokens.Color.BorderFocus : DesignTokens.Color.BorderSubtle;
            Game.DrawRect(sb, r, bg);
            Game.DrawBorder(sb, r, border, 2);
            string label = Game.Loc[_keys[i]];
            var ls = Game.MeasureText(label, 1.0f);
            Game.DrawText(sb, label, new Vector2(r.X + (r.Width - ls.X) * 0.5f, r.Y + (r.Height - ls.Y) * 0.5f),
                DesignTokens.Color.TextPrimary, 1.0f);
        }

        // Back hint
        Game.DrawText(sb, Game.Loc["common.back"] + " (Esc)", new Vector2(DesignTokens.Spacing.L, h - 36),
            DesignTokens.Color.TextMuted, 0.75f);
    }
}
