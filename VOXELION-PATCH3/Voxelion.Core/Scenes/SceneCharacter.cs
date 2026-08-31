using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneCharacter : SceneBase
{
    private Rectangle _btnNext;
    private Rectangle _btnBack;
    private Rectangle _btnRandom;
    private int _body;
    private int _hair;
    private int _outfit;

    public SceneCharacter(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        Layout();
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        float bw = Math.Min(200f, vp.Width * 0.28f);
        float bh = 48f;
        float y = vp.Height * 0.82f;
        float gap = 16f;
        float total = bw * 3 + gap * 2;
        float startX = (vp.Width - total) * 0.5f;

        _btnBack = new Rectangle((int)startX, (int)y, (int)bw, (int)bh);
        _btnRandom = new Rectangle((int)(startX + bw + gap), (int)y, (int)bw, (int)bh);
        _btnNext = new Rectangle((int)(startX + 2 * (bw + gap)), (int)y, (int)bw, (int)bh);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();

        if (!input.IsPointerReleased) return;

        if (_btnNext.Contains(input.PointerPosition))
        {
            Game.Profile.Appearance.BodyIndex = _body;
            Game.Profile.Appearance.HairIndex = _hair;
            Game.Profile.Appearance.OutfitIndex = _outfit;
            Game.Profile.HasCharacter = true;
            Game.TransitionTo(ApplicationState.Identity);
            return;
        }
        if (_btnBack.Contains(input.PointerPosition))
        {
            Game.TransitionTo(ApplicationState.Title);
            return;
        }
        if (_btnRandom.Contains(input.PointerPosition))
        {
            _body = Random.Shared.Next(0, 6);
            _hair = Random.Shared.Next(0, 8);
            _outfit = Random.Shared.Next(0, 6);
        }
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float cx = w * 0.5f;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);

        string title = "CREATE CHARACTER";
        var ts = Game.MeasureText(title, 2.5f);
        Game.DrawText(sb, title, new Vector2(cx - ts.X * 0.5f, h * 0.08f),
            DesignTokens.Color.TextPrimary, 2.5f);

        // Simple avatar silhouette
        float ay = h * 0.28f;
        float aw = 80f, ah = 140f;
        Game.DrawRect(sb, cx - aw * 0.5f, ay, aw, ah, DesignTokens.Color.PanelElevated);
        Game.DrawRect(sb, cx - 28, ay - 36, 56, 56, DesignTokens.Color.AccentPrimary); // head
        Game.DrawRect(sb, cx - 18, ay - 26, 36, 36, DesignTokens.Color.AccentSecondary); // face

        string info = "BODY " + _body + "  HAIR " + _hair + "  OUTFIT " + _outfit;
        var isz = Game.MeasureText(info, 1.5f);
        Game.DrawText(sb, info, new Vector2(cx - isz.X * 0.5f, ay + ah + 16),
            DesignTokens.Color.TextSecondary, 1.5f);

        DrawBtn(sb, _btnBack, "BACK", DesignTokens.Color.PanelBase);
        DrawBtn(sb, _btnRandom, "RANDOM", DesignTokens.Color.PanelElevated);
        DrawBtn(sb, _btnNext, "NEXT", DesignTokens.Color.AccentPrimary);
    }

    private void DrawBtn(SpriteBatch sb, Rectangle r, string label, Color fill)
    {
        Game.DrawRect(sb, r, fill);
        Game.DrawBorder(sb, r, DesignTokens.Color.BorderFocus, 2);
        var size = Game.MeasureText(label, 1.8f);
        Game.DrawText(sb, label,
            new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + (r.Height - size.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, 1.8f);
    }
}
