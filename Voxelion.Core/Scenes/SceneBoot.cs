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
    private readonly string[] _phaseKeys =
    {
        "boot.phase.graphics", "boot.phase.input", "boot.phase.audio", "boot.phase.fonts",
        "boot.phase.localization", "boot.phase.ui", "boot.phase.assets", "boot.phase.session",
        "boot.phase.network"
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
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Semantic.Background);
        VisualChrome.AmbientDust(Game, sb, vp, SceneTime, 36);

        float a = DesignTokens.Motion.EaseOut(MathHelper.Clamp(_progress * 2f, 0, 1));
        VisualChrome.Emblem(Game, sb, new Vector2(cx, cy), 56f * (0.85f + 0.15f * a), a);

        UiKit.CenterLabel(Game, sb, Game.Loc.T("app.name"), cy + 72,
            DesignTokens.Semantic.TextPrimary * a, TypeScale.Title, w);

        int pi = Math.Min((int)(_progress * _phaseKeys.Length), _phaseKeys.Length - 1);
        UiKit.CenterLabel(Game, sb, Game.Loc.T(_phaseKeys[Math.Max(0, pi)]), h * 0.72f,
            DesignTokens.Semantic.TextMuted, TypeScale.Label, w);

        var bar = new Rectangle((int)(cx - 160), (int)(h * 0.80f), 320, (int)DesignTokens.Component.ProgressHeight);
        VisualChrome.ProgressCrystal(Game, sb, bar, _progress);
    }
}
