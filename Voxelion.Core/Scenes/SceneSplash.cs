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
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Semantic.BackgroundAlt);
        VisualChrome.AmbientDust(Game, sb, vp, t, 44);

        float a = DesignTokens.Motion.EaseOut(MathHelper.Clamp(t / 0.6f, 0, 1));
        VisualChrome.Emblem(Game, sb, new Vector2(cx, cy), 64f, a);

        UiKit.CenterLabel(Game, sb, Game.Loc.T("app.name"), cy + 80,
            DesignTokens.Semantic.TextPrimary * a, TypeScale.Display, w);
        UiKit.CenterLabel(Game, sb, Game.Loc.T("app.tagline"), cy + 118,
            DesignTokens.Semantic.TextSecondary * a, TypeScale.Body, w);

        if (EnterTime >= 0.8f)
        {
            float sa = 0.45f + 0.35f * MathF.Sin(t * 3.5f);
            UiKit.CenterLabel(Game, sb, Game.Loc.T("splash.continue"), h * 0.86f,
                DesignTokens.Semantic.TextMuted * sa, TypeScale.Body, w);
        }
    }
}
