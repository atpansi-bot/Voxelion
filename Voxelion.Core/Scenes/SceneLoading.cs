using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneLoading : SceneBase
{
    private float _progress;
    public SceneLoading(VoxelionGame game) : base(game) { }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        float dt = Math.Max((float)gameTime.ElapsedGameTime.TotalSeconds, 1f / 60f);
        _progress += dt * 0.85f;
        if (_progress >= 1f || EnterTime >= 1.8f)
            Game.TransitionTo(ApplicationState.Title);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height, cx = w * 0.5f;
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);
        UiKit.CenterLabel(Game, sb, "LOADING", h * 0.38f, DesignTokens.Color.TextPrimary, 2.5f, w);
        UiKit.CenterLabel(Game, sb, "PREPARING YOUR JOURNEY", h * 0.46f, DesignTokens.Color.TextMuted, 1.3f, w);
        var bar = new Rectangle((int)(cx - 180), (int)(h * 0.55f), 360, 12);
        UiKit.ProgressBar(Game, sb, bar, _progress, DesignTokens.Color.AccentSecondary);
        UiKit.CenterLabel(Game, sb, ((int)(MathHelper.Clamp(_progress, 0, 1) * 100)) + "%", h * 0.60f, DesignTokens.Color.TextSecondary, 1.8f, w);
        UiKit.CenterLabel(Game, sb, "V1.0.0", h * 0.92f, DesignTokens.Color.TextMuted, 1.2f, w);
    }
}
