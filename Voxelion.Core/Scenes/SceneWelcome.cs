using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneWelcome : SceneBase
{
    public SceneWelcome(VoxelionGame game) : base(game) { }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        var vp = Game.GraphicsDevice.Viewport;
        var enterR = new Rectangle((int)(vp.Width * 0.5f - 100), (int)(vp.Height * 0.68f), 200, 48);
        if ((input.IsPointerPressed && enterR.Contains(input.PointerPosition)) || input.ConfirmPressed)
            Game.TransitionTo(ApplicationState.Transition);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height, cx = w * 0.5f, t = SceneTime;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);

        float alpha = EaseOutCubic(MathHelper.Clamp(EnterTime / 0.6f, 0, 1));

        string ready = Game.Loc["welcome.ready"];
        var rs = Game.MeasureText(ready, 1.6f);
        Game.DrawText(sb, ready, new Vector2(cx - rs.X * 0.5f, h * 0.25f), DesignTokens.Color.TextPrimary * alpha, 1.6f);

        // Character preview
        float px = cx, py = h * 0.42f;
        Game.DrawRect(sb, px - 40, py - 30, 80, 100, DesignTokens.Color.AccentPrimary * alpha * 0.9f);
        Game.DrawRect(sb, px - 28, py - 60, 56, 40, DesignTokens.Color.AccentSecondary * alpha);

        string name = Game.Profile.DisplayName;
        var ns = Game.MeasureText(name, 1.2f);
        Game.DrawText(sb, name, new Vector2(cx - ns.X * 0.5f, py + 90), DesignTokens.Color.AccentTertiary * alpha, 1.2f);

        string welcome = Game.Loc["welcome.to"];
        var ws = Game.MeasureText(welcome, 0.95f);
        Game.DrawText(sb, welcome, new Vector2(cx - ws.X * 0.5f, py + 120), DesignTokens.Color.TextSecondary * alpha, 0.95f);

        var enterR = new Rectangle((int)(cx - 100), (int)(h * 0.68f), 200, 48);
        Game.DrawRect(sb, enterR, DesignTokens.Color.AccentPrimary * 0.85f * alpha);
        Game.DrawBorder(sb, enterR, DesignTokens.Color.BorderFocus, 2);
        string enter = Game.Loc["welcome.enter"];
        var es = Game.MeasureText(enter, 1.1f);
        Game.DrawText(sb, enter, new Vector2(enterR.X + (enterR.Width - es.X) * 0.5f, enterR.Y + 14),
            DesignTokens.Color.TextPrimary * alpha, 1.1f);
    }
}
