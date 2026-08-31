using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneIdentity : SceneBase
{
    private Rectangle _btnConfirm;
    private Rectangle _btnBack;
    private string _name = "WANDERER";

    public SceneIdentity(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        if (!string.IsNullOrEmpty(Game.Profile.DisplayName))
            _name = Game.Profile.DisplayName.ToUpperInvariant();
        else
            _name = "WANDERER" + Random.Shared.Next(10, 99);
        Layout();
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        float cx = vp.Width * 0.5f;
        float bw = Math.Min(240f, vp.Width * 0.4f);
        float bh = 48f;
        float y = vp.Height * 0.72f;
        _btnBack = new Rectangle((int)(cx - bw - 12), (int)y, (int)bw, (int)bh);
        _btnConfirm = new Rectangle((int)(cx + 12), (int)y, (int)bw, (int)bh);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();

        if (!input.IsPointerReleased) return;

        if (_btnConfirm.Contains(input.PointerPosition))
        {
            Game.Profile.DisplayName = _name;
            Game.Profile.HasCharacter = true;
            Game.TransitionTo(ApplicationState.Welcome);
            return;
        }
        if (_btnBack.Contains(input.PointerPosition))
        {
            Game.TransitionTo(ApplicationState.CharacterCreation);
        }
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float cx = w * 0.5f;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.DeepNight);

        string title = "CHOOSE YOUR NAME";
        var ts = Game.MeasureText(title, 2.4f);
        Game.DrawText(sb, title, new Vector2(cx - ts.X * 0.5f, h * 0.18f),
            DesignTokens.Color.TextPrimary, 2.4f);

        // Name panel
        float nw = Math.Min(360f, w * 0.6f);
        float nh = 56f;
        var panel = new Rectangle((int)(cx - nw * 0.5f), (int)(h * 0.42f), (int)nw, (int)nh);
        Game.DrawRect(sb, panel, DesignTokens.Color.PanelElevated);
        Game.DrawBorder(sb, panel, DesignTokens.Color.AccentPrimary, 2);

        var ns = Game.MeasureText(_name, 2.5f);
        Game.DrawText(sb, _name,
            new Vector2(panel.X + (panel.Width - ns.X) * 0.5f, panel.Y + (panel.Height - ns.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, 2.5f);

        string ok = "NAME AVAILABLE";
        var os = Game.MeasureText(ok, 1.4f);
        Game.DrawText(sb, ok, new Vector2(cx - os.X * 0.5f, h * 0.55f),
            DesignTokens.Color.AccentSuccess, 1.4f);

        DrawBtn(sb, _btnBack, "BACK", DesignTokens.Color.PanelBase);
        DrawBtn(sb, _btnConfirm, "CONFIRM", DesignTokens.Color.AccentPrimary);
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
