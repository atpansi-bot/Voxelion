using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

/// <summary>Cinematic dissolve into hub — UI elements fade, light expands.</summary>
public sealed class SceneTransition : SceneBase
{
    public SceneTransition(VoxelionGame game) : base(game) { }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        if (EnterTime >= 1.4f)
            Game.TransitionTo(ApplicationState.Hub);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height, cx = w * 0.5f, cy = h * 0.5f;
        float t = MathHelper.Clamp(EnterTime / 1.4f, 0, 1);
        float ease = EaseInOut(t);

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);

        float radius = 40 + ease * Math.Max(w, h);
        // Expanding light as filled rects approximating bloom
        for (int i = 4; i >= 0; i--)
        {
            float s = radius * (0.4f + i * 0.15f);
            float a = (1f - i * 0.18f) * (0.3f + 0.4f * ease);
            Game.DrawRect(sb, cx - s * 0.5f, cy - s * 0.5f, s, s, DesignTokens.Color.AccentPrimary * a);
        }
        float emblem = 48 * (1f - ease * 0.5f);
        Game.DrawRect(sb, cx - emblem * 0.5f, cy - emblem * 0.5f, emblem, emblem,
            DesignTokens.Color.AccentSecondary * (1f - ease));
    }
}
