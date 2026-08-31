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
        "Graphics", "Input", "Audio", "Fonts", "Localization", "UI Theme", "Essential Assets", "Saved Session", "Network State"
    };
    private int _phaseIndex;

    public SceneBoot(VoxelionGame game) : base(game) { }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _progress += dt * 0.55f;
        _phaseIndex = Math.Min((int)(_progress * _phases.Length), _phases.Length - 1);

        if (_progress >= 1.05f)
            Game.TransitionTo(ApplicationState.Splash);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;

        // Dark cinematic background with subtle particles
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);

        // Atmospheric layers
        float t = SceneTime;
        for (int i = 0; i < 40; i++)
        {
            float px = (MathF.Sin(t * 0.15f + i * 1.7f) * 0.5f + 0.5f) * w;
            float py = (MathF.Cos(t * 0.11f + i * 2.3f) * 0.5f + 0.5f) * h;
            float size = 1.5f + (i % 3);
            Color pc = DesignTokens.Color.AccentPrimary * (0.15f + 0.1f * MathF.Sin(t + i));
            Game.DrawRect(sb, px, py, size, size, pc);
        }

        // Emblem reveal
        float emblemAlpha = EaseOutCubic(MathHelper.Clamp((_progress - 0.2f) * 2f, 0, 1));
        float cx = w * 0.5f, cy = h * 0.42f;
        float emblemSize = 64f * (0.8f + 0.2f * emblemAlpha);

        // Outer glow
        Game.DrawRect(sb, cx - emblemSize * 1.4f, cy - emblemSize * 1.4f, emblemSize * 2.8f, emblemSize * 2.8f,
            DesignTokens.Color.GlowPrimary * emblemAlpha * 0.4f);
        // Core diamond
        Game.DrawRect(sb, cx - emblemSize * 0.5f, cy - emblemSize * 0.5f, emblemSize, emblemSize,
            DesignTokens.Color.AccentPrimary * emblemAlpha);
        // Inner accent
        Game.DrawRect(sb, cx - emblemSize * 0.25f, cy - emblemSize * 0.25f, emblemSize * 0.5f, emblemSize * 0.5f,
            DesignTokens.Color.AccentSecondary * emblemAlpha);

        // Title
        string title = Game.Loc["app.name"];
        var titleSize = Game.MeasureText(title, 1.6f);
        Game.DrawText(sb, title, new Vector2(cx - titleSize.X * 0.5f, cy + emblemSize + 24),
            DesignTokens.Color.TextPrimary * emblemAlpha, 1.6f);

        // Phase text
        if (_phaseIndex < _phases.Length)
        {
            string phase = _phases[_phaseIndex];
            var ps = Game.MeasureText(phase, 0.85f);
            Game.DrawText(sb, phase, new Vector2(cx - ps.X * 0.5f, h * 0.78f),
                DesignTokens.Color.TextMuted * 0.8f, 0.85f);
        }

        // Progress bar
        float barW = 280f, barH = 4f;
        float barX = cx - barW * 0.5f, barY = h * 0.84f;
        Game.DrawRect(sb, barX, barY, barW, barH, DesignTokens.Color.ShadowIndigo);
        Game.DrawRect(sb, barX, barY, barW * MathHelper.Clamp(_progress, 0, 1), barH, DesignTokens.Color.AccentPrimary);
    }
}
