using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneWelcome : SceneBase
{
    private Rectangle _btnEnter;

    public SceneWelcome(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        Layout();
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        float bw = Math.Min(260f, vp.Width * 0.5f);
        float bh = 54f;
        _btnEnter = new Rectangle(
            (int)(vp.Width * 0.5f - bw * 0.5f),
            (int)(vp.Height * 0.68f),
            (int)bw, (int)bh);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();

        if (EnterTime >= 0.6f && input.IsPointerReleased && _btnEnter.Contains(input.PointerPosition))
        {
            Game.TransitionTo(ApplicationState.Hub);
            return;
        }
        if (EnterTime >= 4f)
            Game.TransitionTo(ApplicationState.Hub);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float cx = w * 0.5f;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);

        string ready = "YOU ARE READY";
        var rs = Game.MeasureText(ready, 3f);
        Game.DrawText(sb, ready, new Vector2(cx - rs.X * 0.5f, h * 0.28f),
            DesignTokens.Color.TextPrimary, 3f);

        string name = Game.Profile.DisplayName;
        if (string.IsNullOrEmpty(name)) name = "WANDERER";
        name = name.ToUpperInvariant();
        var ns = Game.MeasureText(name, 2.2f);
        Game.DrawText(sb, name, new Vector2(cx - ns.X * 0.5f, h * 0.40f),
            DesignTokens.Color.AccentSecondary, 2.2f);

        string welcome = "WELCOME TO VOXELION";
        var ws = Game.MeasureText(welcome, 1.6f);
        Game.DrawText(sb, welcome, new Vector2(cx - ws.X * 0.5f, h * 0.50f),
            DesignTokens.Color.TextSecondary, 1.6f);

        Game.DrawRect(sb, _btnEnter, DesignTokens.Color.AccentPrimary);
        Game.DrawBorder(sb, _btnEnter, DesignTokens.Color.BorderFocus, 2);
        string enter = "ENTER";
        var es = Game.MeasureText(enter, 2.4f);
        Game.DrawText(sb, enter,
            new Vector2(_btnEnter.X + (_btnEnter.Width - es.X) * 0.5f,
                _btnEnter.Y + (_btnEnter.Height - es.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, 2.4f);
    }
}
