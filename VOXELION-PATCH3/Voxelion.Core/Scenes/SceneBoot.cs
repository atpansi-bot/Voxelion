using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneBoot : SceneBase
{
    private float _progress;
    private readonly string[] _phases =
    {
        "GRAPHICS", "INPUT", "AUDIO", "FONTS", "LOCALIZATION",
        "UI THEME", "ASSETS", "SESSION", "NETWORK"
    };

    public SceneBoot(VoxelionGame game) : base(game) { }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (dt <= 0) dt = 1f / 60f;
        _progress += dt * 0.7f;

        // Hard advance by wall time so we never stick
        if (_progress >= 1f || EnterTime >= 2.2f)
            Game.TransitionTo(ApplicationState.Splash);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float cx = w * 0.5f, cy = h * 0.42f;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);

        float t = SceneTime;
        for (int i = 0; i < 36; i++)
        {
            float px = (MathF.Sin(t * 0.2f + i * 1.7f) * 0.5f + 0.5f) * w;
            float py = (MathF.Cos(t * 0.14f + i * 2.1f) * 0.5f + 0.5f) * h;
            float size = 2f + (i % 3);
            Color pc = DesignTokens.Color.AccentPrimary * (0.12f + 0.1f * MathF.Sin(t + i));
            Game.DrawRect(sb, px, py, size, size, pc);
        }

        float emblemAlpha = EaseOutCubic(MathHelper.Clamp(_progress * 2f, 0, 1));
        float emblemSize = 56f;

        Game.DrawRect(sb, cx - emblemSize * 1.35f, cy - emblemSize * 1.35f,
            emblemSize * 2.7f, emblemSize * 2.7f, DesignTokens.Color.GlowPrimary * emblemAlpha * 0.45f);
        Game.DrawRect(sb, cx - emblemSize * 0.55f, cy - emblemSize * 0.55f,
            emblemSize * 1.1f, emblemSize * 1.1f, DesignTokens.Color.AccentPrimary * emblemAlpha);
        Game.DrawRect(sb, cx - emblemSize * 0.28f, cy - emblemSize * 0.28f,
            emblemSize * 0.56f, emblemSize * 0.56f, DesignTokens.Color.AccentSecondary * emblemAlpha);

        string title = "VOXELION";
        var titleSize = Game.MeasureText(title, 3f);
        Game.DrawText(sb, title, new Vector2(cx - titleSize.X * 0.5f, cy + emblemSize + 18),
            DesignTokens.Color.TextPrimary * emblemAlpha, 3f);

        int phaseIndex = Math.Min((int)(_progress * _phases.Length), _phases.Length - 1);
        string phase = _phases[Math.Max(0, phaseIndex)];
        var ps = Game.MeasureText(phase, 1.4f);
        Game.DrawText(sb, phase, new Vector2(cx - ps.X * 0.5f, h * 0.72f),
            DesignTokens.Color.TextMuted, 1.4f);

        // Thick visible progress bar
        float barW = Math.Min(320f, w * 0.5f);
        float barH = 10f;
        float barX = cx - barW * 0.5f;
        float barY = h * 0.80f;
        Game.DrawRect(sb, barX, barY, barW, barH, DesignTokens.Color.ShadowIndigo);
        Game.DrawRect(sb, barX, barY, barW * MathHelper.Clamp(_progress, 0, 1), barH, DesignTokens.Color.AccentPrimary);
        Game.DrawBorder(sb, new Rectangle((int)barX, (int)barY, (int)barW, (int)barH),
            DesignTokens.Color.BorderSubtle, 1);
    }
}
