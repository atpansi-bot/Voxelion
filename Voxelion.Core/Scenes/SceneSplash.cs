using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneSplash : SceneBase
{
    private const float MinDuration = 1.6f;
    private bool _canSkip;

    public SceneSplash(VoxelionGame game) : base(game) { }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        if (EnterTime >= MinDuration) _canSkip = true;

        if ((_canSkip && (input.IsPointerPressed || input.ConfirmPressed || input.CancelPressed)) || EnterTime >= 3.2f)
            Game.TransitionTo(ApplicationState.Loading);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float t = SceneTime;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.DeepNight);

        // Particles
        for (int i = 0; i < 60; i++)
        {
            float px = (MathF.Sin(t * 0.2f + i * 0.9f) * 0.5f + 0.5f) * w;
            float py = (MathF.Cos(t * 0.13f + i * 1.4f) * 0.5f + 0.5f) * h;
            float a = 0.1f + 0.15f * MathF.Sin(t * 2f + i);
            Game.DrawRect(sb, px, py, 2, 2, DesignTokens.Color.AccentSecondary * a);
        }

        float alpha = EaseOutCubic(MathHelper.Clamp(t / 0.8f, 0, 1));
        float cx = w * 0.5f, cy = h * 0.45f;

        // Emblem
        float size = 80f;
        Game.DrawRect(sb, cx - size, cy - size, size * 2, size * 2, DesignTokens.Color.GlowPrimary * alpha * 0.5f);
        Game.DrawRect(sb, cx - size * 0.55f, cy - size * 0.55f, size * 1.1f, size * 1.1f, DesignTokens.Color.AccentPrimary * alpha);
        Game.DrawRect(sb, cx - size * 0.28f, cy - size * 0.28f, size * 0.56f, size * 0.56f, DesignTokens.Color.AccentSecondary * alpha);

        // Light sweep
        float sweep = MathHelper.Clamp((t - 0.4f) / 0.6f, 0, 1);
        if (sweep > 0 && sweep < 1)
        {
            float sx = cx - 120 + sweep * 240;
            Game.DrawRect(sb, sx, cy - 90, 12, 180, Color.White * (0.3f * (1 - MathF.Abs(sweep - 0.5f) * 2)));
        }

        string title = Game.Loc["app.name"];
        var ts = Game.MeasureText(title, 2.0f);
        Game.DrawText(sb, title, new Vector2(cx - ts.X * 0.5f, cy + size + 30), DesignTokens.Color.TextPrimary * alpha, 2.0f);

        if (_canSkip)
        {
            string skip = "TAP TO CONTINUE";
            var ss = Game.MeasureText(skip, 0.7f);
            float sa = 0.4f + 0.3f * MathF.Sin(t * 3f);
            Game.DrawText(sb, skip, new Vector2(cx - ss.X * 0.5f, h * 0.88f), DesignTokens.Color.TextMuted * sa, 0.7f);
        }
    }
}
