using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI;
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
        float dt = Math.Max((float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 60f);
        _progress += dt * 0.7f;
        if (_progress >= 1f || EnterTime >= 2.2f)
            Game.TransitionTo(ApplicationState.Splash);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height, cx = w * 0.5f, cy = h * 0.42f;
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);
        float t = SceneTime;
        for (int i = 0; i < 36; i++)
        {
            float px = (MathF.Sin(t * 0.2f + i * 1.7f) * 0.5f + 0.5f) * w;
            float py = (MathF.Cos(t * 0.14f + i * 2.1f) * 0.5f + 0.5f) * h;
            Game.DrawRect(sb, px, py, 2 + i % 3, 2 + i % 3, DesignTokens.Color.AccentPrimary * 0.2f);
        }
        float a = EaseOutCubic(MathHelper.Clamp(_progress * 2f, 0, 1));
        float s = 56f;
        Game.DrawRect(sb, cx - s * 1.3f, cy - s * 1.3f, s * 2.6f, s * 2.6f, DesignTokens.Color.GlowPrimary * a * 0.4f);
        Game.DrawRect(sb, cx - s * 0.55f, cy - s * 0.55f, s * 1.1f, s * 1.1f, DesignTokens.Color.AccentPrimary * a);
        Game.DrawRect(sb, cx - s * 0.28f, cy - s * 0.28f, s * 0.56f, s * 0.56f, DesignTokens.Color.AccentSecondary * a);
        UiKit.CenterLabel(Game, sb, "VOXELION", cy + s + 18, DesignTokens.Color.TextPrimary * a, 3f, w);
        int pi = Math.Min((int)(_progress * _phases.Length), _phases.Length - 1);
        UiKit.CenterLabel(Game, sb, _phases[Math.Max(0, pi)], h * 0.72f, DesignTokens.Color.TextMuted, 1.4f, w);
        var bar = new Rectangle((int)(cx - 160), (int)(h * 0.80f), 320, 10);
        UiKit.ProgressBar(Game, sb, bar, _progress, DesignTokens.Color.AccentPrimary);
    }
}
