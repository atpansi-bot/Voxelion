using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneSplash : SceneBase
{
    public SceneSplash(VoxelionGame game) : base(game) { }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        bool skip = EnterTime >= 0.8f && (input.IsPointerPressed || input.IsPointerReleased || input.ConfirmPressed);
        if (skip || EnterTime >= 2.5f)
            Game.TransitionTo(ApplicationState.Loading);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height, cx = w * 0.5f, cy = h * 0.42f, t = SceneTime;
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.DeepNight);
        for (int i = 0; i < 40; i++)
        {
            float px = (MathF.Sin(t * 0.25f + i) * 0.5f + 0.5f) * w;
            float py = (MathF.Cos(t * 0.16f + i * 1.2f) * 0.5f + 0.5f) * h;
            Game.DrawRect(sb, px, py, 2, 2, DesignTokens.Color.AccentSecondary * 0.2f);
        }
        float a = EaseOutCubic(MathHelper.Clamp(t / 0.6f, 0, 1));
        float s = 64f;
        Game.DrawRect(sb, cx - s * 0.55f, cy - s * 0.55f, s * 1.1f, s * 1.1f, DesignTokens.Color.AccentPrimary * a);
        Game.DrawRect(sb, cx - s * 0.28f, cy - s * 0.28f, s * 0.56f, s * 0.56f, DesignTokens.Color.AccentSecondary * a);
        UiKit.CenterLabel(Game, sb, "VOXELION", cy + s + 20, DesignTokens.Color.TextPrimary * a, 3.5f, w);
        UiKit.CenterLabel(Game, sb, "ENTER THE FRONTIER", cy + s + 56, DesignTokens.Color.TextSecondary * a, 1.5f, w);
        if (EnterTime >= 0.8f)
        {
            float sa = 0.45f + 0.35f * MathF.Sin(t * 3.5f);
            UiKit.CenterLabel(Game, sb, "TAP TO CONTINUE", h * 0.86f, DesignTokens.Color.TextMuted * sa, 1.6f, w);
        }
    }
}
