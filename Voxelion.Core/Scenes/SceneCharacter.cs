using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneCharacter : SceneBase
{
    private Rectangle _btnNext, _btnBack, _btnRandom;
    private readonly string[] _cats = { "BODY", "HAIR", "FACE", "EYES", "OUTFIT", "ACCESSORY" };
    private int _cat;
    private readonly int[] _vals = new int[6];

    public SceneCharacter(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        var a = Game.Profile.Appearance;
        _vals[0] = a.BodyIndex; _vals[1] = a.HairIndex; _vals[2] = a.FaceIndex;
        _vals[3] = a.EyesIndex; _vals[4] = a.OutfitIndex; _vals[5] = a.AccessoryIndex;
        Layout();
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        float bw = Math.Min(180f, vp.Width * 0.26f);
        float bh = 48f;
        float y = vp.Height * 0.84f;
        float gap = 12f;
        float total = bw * 3 + gap * 2;
        float x = (vp.Width - total) * 0.5f;
        _btnBack = new Rectangle((int)x, (int)y, (int)bw, (int)bh);
        _btnRandom = new Rectangle((int)(x + bw + gap), (int)y, (int)bw, (int)bh);
        _btnNext = new Rectangle((int)(x + 2 * (bw + gap)), (int)y, (int)bw, (int)bh);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();
        if (!input.IsPointerReleased) return;
        var p = input.PointerPosition;
        var vp = Game.GraphicsDevice.Viewport;

        // Category list hits
        float listX = SafeLayout.Margin(vp) + 8;
        float listY = vp.Height * 0.22f;
        for (int i = 0; i < _cats.Length; i++)
        {
            var r = new Rectangle((int)listX, (int)(listY + i * 40), 140, 36);
            if (r.Contains(p)) { _cat = i; return; }
        }
        // Cycle value on preview tap
        var preview = new Rectangle((int)(vp.Width * 0.55f - 50), (int)(vp.Height * 0.28f), 100, 160);
        if (preview.Contains(p))
        {
            _vals[_cat] = (_vals[_cat] + 1) % 8;
            return;
        }
        if (_btnRandom.Contains(p))
        {
            for (int i = 0; i < _vals.Length; i++) _vals[i] = Random.Shared.Next(0, 8);
            return;
        }
        if (_btnBack.Contains(p))
        {
            Game.TransitionTo(ApplicationState.Title);
            return;
        }
        if (_btnNext.Contains(p))
        {
            var a = Game.Profile.Appearance;
            a.BodyIndex = _vals[0]; a.HairIndex = _vals[1]; a.FaceIndex = _vals[2];
            a.EyesIndex = _vals[3]; a.OutfitIndex = _vals[4]; a.AccessoryIndex = _vals[5];
            Game.TransitionTo(ApplicationState.Identity);
        }
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height, cx = w * 0.5f;
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);
        UiKit.CenterLabel(Game, sb, "CREATE YOUR CHARACTER", h * 0.08f, DesignTokens.Color.TextPrimary, 2.2f, w);

        float listX = SafeLayout.Margin(vp) + 8;
        float listY = h * 0.22f;
        for (int i = 0; i < _cats.Length; i++)
        {
            var r = new Rectangle((int)listX, (int)(listY + i * 40), 140, 36);
            var fill = i == _cat ? DesignTokens.Color.AccentPrimary : DesignTokens.Color.PanelElevated;
            Game.DrawRect(sb, r, fill);
            Game.DrawText(sb, _cats[i], new Vector2(r.X + 10, r.Y + 10), DesignTokens.Color.TextPrimary, 1.3f);
        }

        float ax = w * 0.55f, ay = h * 0.28f;
        Game.DrawRect(sb, ax - 50, ay, 100, 160, DesignTokens.Color.PanelElevated);
        Game.DrawRect(sb, ax - 28, ay - 36, 56, 56, Game.Profile.Appearance.PrimaryColor);
        Game.DrawRect(sb, ax - 18, ay - 26, 36, 36, Game.Profile.Appearance.SecondaryColor);
        Game.DrawRect(sb, ax - 40, ay + 20, 80, 100, DesignTokens.Color.AccentPrimary * 0.7f);

        string info = _cats[_cat] + "  " + _vals[_cat];
        UiKit.CenterLabel(Game, sb, info, ay + 180, DesignTokens.Color.TextSecondary, 1.5f, w);
        UiKit.CenterLabel(Game, sb, "TAP PREVIEW TO CYCLE", ay + 210, DesignTokens.Color.TextMuted, 1.2f, w);

        DrawBtn(sb, _btnBack, "BACK", DesignTokens.Color.PanelBase);
        DrawBtn(sb, _btnRandom, "RANDOM", DesignTokens.Color.PanelElevated);
        DrawBtn(sb, _btnNext, "NEXT", DesignTokens.Color.AccentPrimary);
    }

    private void DrawBtn(SpriteBatch sb, Rectangle r, string label, Color fill)
    {
        Game.DrawRect(sb, r, fill);
        Game.DrawBorder(sb, r, DesignTokens.Color.BorderFocus, 2);
        var size = Game.MeasureText(label, 1.7f);
        Game.DrawText(sb, label, new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + (r.Height - size.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, 1.7f);
    }
}
