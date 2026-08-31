using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI;
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
        _btnEnter = new Rectangle((int)(vp.Width * 0.5f - bw * 0.5f), (int)(vp.Height * 0.68f), (int)bw, 54);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();
        if (EnterTime >= 0.5f && input.IsPointerReleased && _btnEnter.Contains(input.PointerPosition))
        {
            Game.TransitionTo(ApplicationState.Transition);
            return;
        }
        if (EnterTime >= 4f)
            Game.TransitionTo(ApplicationState.Transition);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height, cx = w * 0.5f;
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);
        UiKit.CenterLabel(Game, sb, "YOU ARE READY", h * 0.28f, DesignTokens.Color.TextPrimary, 3f, w);
        string name = (Game.Profile.DisplayName ?? "WANDERER").ToUpperInvariant();
        UiKit.CenterLabel(Game, sb, name, h * 0.40f, DesignTokens.Color.AccentSecondary, 2.2f, w);
        UiKit.CenterLabel(Game, sb, "WELCOME TO VOXELION", h * 0.50f, DesignTokens.Color.TextSecondary, 1.6f, w);
        Game.DrawRect(sb, _btnEnter, DesignTokens.Color.AccentPrimary);
        Game.DrawBorder(sb, _btnEnter, DesignTokens.Color.BorderFocus, 2);
        var es = Game.MeasureText("ENTER", 2.4f);
        Game.DrawText(sb, "ENTER", new Vector2(_btnEnter.X + (_btnEnter.Width - es.X) * 0.5f, _btnEnter.Y + 14),
            DesignTokens.Color.TextPrimary, 2.4f);
    }
}
