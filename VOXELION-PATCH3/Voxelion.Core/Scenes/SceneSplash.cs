using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneSplash : SceneBase
{
    public SceneSplash(VoxelionGame game) : base(game) { }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);

        bool skip = EnterTime >= 0.8f &&
                    (input.IsPointerPressed || input.IsPointerReleased || input.ConfirmPressed);
        bool timeout = EnterTime >= 2.5f;

        if (skip || timeout)
            Game.TransitionTo(ApplicationState.Loading);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float t = SceneTime;
        float cx = w * 0.5f, cy = h * 0.42f;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.DeepNight);

        for (int i = 0; i < 50; i++)
        {
            float px = (MathF.Sin(t * 0.25f + i * 0.9f) * 0.5f + 0.5f) * w;
            float py = (MathF.Cos(t * 0.16f + i * 1.3f) * 0.5f + 0.5f) * h;
            float a = 0.12f + 0.12f * MathF.Sin(t * 2f + i);
            Game.DrawRect(sb, px, py, 2, 2, DesignTokens.Color.AccentSecondary * a);
        }

        float alpha = EaseOutCubic(MathHelper.Clamp(t / 0.6f, 0, 1));
        float size = 64f;

        Game.DrawRect(sb, cx - size * 1.3f, cy - size * 1.3f, size * 2.6f, size * 2.6f,
            DesignTokens.Color.GlowPrimary * alpha * 0.5f);
        Game.DrawRect(sb, cx - size * 0.55f, cy - size * 0.55f, size * 1.1f, size * 1.1f,
            DesignTokens.Color.AccentPrimary * alpha);
        Game.DrawRect(sb, cx - size * 0.28f, cy - size * 0.28f, size * 0.56f, size * 0.56f,
            DesignTokens.Color.AccentSecondary * alpha);

        string title = "VOXELION";
        var ts = Game.MeasureText(title, 3.5f);
        Game.DrawText(sb, title, new Vector2(cx - ts.X * 0.5f, cy + size + 24),
            DesignTokens.Color.TextPrimary * alpha, 3.5f);

        string tag = "ENTER THE FRONTIER";
        var tgs = Game.MeasureText(tag, 1.5f);
        Game.DrawText(sb, tag, new Vector2(cx - tgs.X * 0.5f, cy + size + 56),
            DesignTokens.Color.TextSecondary * alpha, 1.5f);

        if (EnterTime >= 0.8f)
        {
            string skip = "TAP TO CONTINUE";
            var ss = Game.MeasureText(skip, 1.6f);
            float sa = 0.45f + 0.35f * MathF.Sin(t * 3.5f);
            Game.DrawText(sb, skip, new Vector2(cx - ss.X * 0.5f, h * 0.86f),
                DesignTokens.Color.TextMuted * sa, 1.6f);
        }
    }
}
