using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneTransition : SceneBase
{
    public SceneTransition(VoxelionGame game) : base(game) { }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        if (EnterTime >= 1.8f)
            Game.TransitionTo(ApplicationState.Hub);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height, t = SceneTime;
        float progress = MathHelper.Clamp(EnterTime / 1.8f, 0, 1);

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);

        // Expanding light
        float radius = progress * Math.Max(w, h) * 1.2f;
        float cx = w * 0.5f, cy = h * 0.5f;
        for (int i = 8; i >= 0; i--)
        {
            float r = radius * (1f - i * 0.08f);
            float a = (0.15f - i * 0.015f) * (1 - progress * 0.5f);
            Game.DrawRect(sb, cx - r, cy - r, r * 2, r * 2, DesignTokens.Color.AccentPrimary * a);
        }

        // Dissolving particles
        for (int i = 0; i < 40; i++)
        {
            float angle = i * 0.4f + t;
            float dist = progress * 300 + i * 8;
            float px = cx + MathF.Cos(angle) * dist;
            float py = cy + MathF.Sin(angle) * dist * 0.6f;
            Game.DrawRect(sb, px, py, 3, 3, DesignTokens.Color.AccentSecondary * (1 - progress));
        }
    }
}
