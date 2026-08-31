using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneWorldLoading : SceneBase
{
    private float _progress;
    private const float Duration = 2.2f;

    public SceneWorldLoading(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        _progress = 0f;
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        _progress = MathHelper.Clamp(SceneTime / Duration, 0f, 1f);
        if (_progress >= 1f)
            Game.TransitionTo(ApplicationState.World);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.DeepNight);

        string name = Game.Profile.LastWorldName ?? "World";
        var sz = Game.MeasureText(name, 1.5f);
        Game.DrawText(sb, name, new Vector2((w - sz.X) * 0.5f, h * 0.32f), DesignTokens.Color.TextPrimary, 1.5f);

        string status = Game.Loc["connect.spawn"];
        var st = Game.MeasureText(status, 0.95f);
        Game.DrawText(sb, status, new Vector2((w - st.X) * 0.5f, h * 0.48f), DesignTokens.Color.TextSecondary, 0.95f);

        int barW = (int)(w * 0.4f);
        int barX = (int)((w - barW) * 0.5f);
        int barY = (int)(h * 0.56f);
        Game.DrawRect(sb, barX, barY, barW, 12, DesignTokens.Color.ShadowIndigo);
        Game.DrawRect(sb, barX, barY, (int)(barW * _progress), 12, DesignTokens.Color.AccentTertiary);

        string meta = (Game.Profile.DisplayName ?? "Traveler") + "  ·  entering";
        var mz = Game.MeasureText(meta, 0.8f);
        Game.DrawText(sb, meta, new Vector2((w - mz.X) * 0.5f, h * 0.62f), DesignTokens.Color.TextMuted, 0.8f);
    }
}
