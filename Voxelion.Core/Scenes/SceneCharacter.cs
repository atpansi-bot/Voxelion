using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneCharacter : SceneBase
{
    private int _category;
    private int _hovered = -1;
    private readonly string[] _cats = { "char.body", "char.hair", "char.face", "char.eyes", "char.outfit", "char.accessories" };
    private readonly int[] _indices = new int[6];

    public SceneCharacter(VoxelionGame game) : base(game) { }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;

        // Category list
        for (int i = 0; i < _cats.Length; i++)
        {
            var r = new Rectangle(40, 100 + i * 48, 160, 40);
            if (r.Contains(input.PointerPosition) && input.IsPointerPressed)
                _category = i;
        }

        // Randomize
        var randR = new Rectangle(40, (int)(h - 80), 140, 40);
        if (randR.Contains(input.PointerPosition) && input.IsPointerPressed)
        {
            for (int i = 0; i < _indices.Length; i++)
                _indices[i] = Random.Shared.Next(0, 8);
            ApplyToProfile();
        }

        // Next / Back
        var nextR = new Rectangle((int)(w - 180), 30, 140, 40);
        var backR = new Rectangle(40, 30, 100, 40);
        _hovered = -1;
        if (nextR.Contains(input.PointerPosition)) _hovered = 0;
        if (backR.Contains(input.PointerPosition)) _hovered = 1;

        if (input.IsPointerPressed)
        {
            if (_hovered == 0)
            {
                ApplyToProfile();
                Game.Profile.HasCharacter = true;
                Game.TransitionTo(ApplicationState.Identity);
            }
            else if (_hovered == 1) Game.GoBack();
        }

        // Cycle options for current category
        var prevR = new Rectangle((int)(w * 0.55f), (int)(h - 80), 80, 40);
        var nextOptR = new Rectangle((int)(w * 0.65f), (int)(h - 80), 80, 40);
        if (prevR.Contains(input.PointerPosition) && input.IsPointerPressed)
        {
            _indices[_category] = (_indices[_category] + 7) % 8;
            ApplyToProfile();
        }
        if (nextOptR.Contains(input.PointerPosition) && input.IsPointerPressed)
        {
            _indices[_category] = (_indices[_category] + 1) % 8;
            ApplyToProfile();
        }
    }

    private void ApplyToProfile()
    {
        var a = Game.Profile.Appearance;
        a.BodyIndex = _indices[0];
        a.HairIndex = _indices[1];
        a.FaceIndex = _indices[2];
        a.EyesIndex = _indices[3];
        a.OutfitIndex = _indices[4];
        a.AccessoryIndex = _indices[5];
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height, t = SceneTime;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);

        // Header
        Game.DrawText(sb, Game.Loc["char.create"], new Vector2(w * 0.5f - 120, 36), DesignTokens.Color.TextPrimary, 1.2f);

        // Categories
        for (int i = 0; i < _cats.Length; i++)
        {
            var r = new Rectangle(40, 100 + i * 48, 160, 40);
            bool sel = i == _category;
            Game.DrawRect(sb, r, sel ? DesignTokens.Color.PanelElevated : DesignTokens.Color.PanelBase);
            Game.DrawBorder(sb, r, sel ? DesignTokens.Color.BorderFocus : DesignTokens.Color.BorderSubtle, 1);
            Game.DrawText(sb, Game.Loc[_cats[i]], new Vector2(r.X + 12, r.Y + 12), DesignTokens.Color.TextPrimary, 0.9f);
        }

        // Character preview chamber
        float px = w * 0.55f, py = h * 0.35f, size = 160f;
        Game.DrawRect(sb, px - size * 0.7f, py - size * 0.9f, size * 1.4f, size * 1.8f, DesignTokens.Color.PanelGlass);
        Game.DrawBorder(sb, new Rectangle((int)(px - size * 0.7f), (int)(py - size * 0.9f), (int)(size * 1.4f), (int)(size * 1.8f)),
            DesignTokens.Color.BorderSubtle, 1);

        // Animated avatar representation
        float pulse = 0.9f + 0.1f * MathF.Sin(t * 2f);
        Color bodyC = DesignTokens.Color.AccentPrimary * pulse;
        Game.DrawRect(sb, px - 30, py - 20, 60, 90, bodyC); // body
        Game.DrawRect(sb, px - 22, py - 55, 44, 40, DesignTokens.Color.AccentSecondary); // head
        Game.DrawRect(sb, px - 18, py - 70, 36, 18, DesignTokens.Color.AccentTertiary * 0.8f); // hair

        // Index indicators
        Game.DrawText(sb, $"#{_indices[_category] + 1}", new Vector2(px - 10, py + 90), DesignTokens.Color.TextMuted, 0.85f);

        // Controls
        DrawBtn(sb, new Rectangle(40, (int)(h - 80), 140, 40), Game.Loc["char.randomize"], false);
        DrawBtn(sb, new Rectangle((int)(w * 0.55f), (int)(h - 80), 80, 40), "<", false);
        DrawBtn(sb, new Rectangle((int)(w * 0.65f), (int)(h - 80), 80, 40), ">", false);
        DrawBtn(sb, new Rectangle((int)(w - 180), 30, 140, 40), Game.Loc["char.next"], _hovered == 0);
        DrawBtn(sb, new Rectangle(40, 30, 100, 40), Game.Loc["char.back"], _hovered == 1);
    }

    private void DrawBtn(SpriteBatch sb, Rectangle r, string label, bool hover)
    {
        Game.DrawRect(sb, r, hover ? DesignTokens.Color.PanelElevated : DesignTokens.Color.PanelBase);
        Game.DrawBorder(sb, r, hover ? DesignTokens.Color.BorderFocus : DesignTokens.Color.BorderSubtle, 1);
        var ls = Game.MeasureText(label, 0.9f);
        Game.DrawText(sb, label, new Vector2(r.X + (r.Width - ls.X) * 0.5f, r.Y + (r.Height - ls.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, 0.9f);
    }
}
