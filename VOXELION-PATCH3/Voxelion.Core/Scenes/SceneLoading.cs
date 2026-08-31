using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneLoading : SceneBase
{
    private float _progress;

    public SceneLoading(VoxelionGame game) : base(game) { }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (dt <= 0) dt = 1f / 60f;
        _progress += dt * 0.85f;

        if (_progress >= 1f || EnterTime >= 1.8f)
            Game.TransitionTo(ApplicationState.Title);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float cx = w * 0.5f;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);

        string title = "LOADING";
        var ts = Game.MeasureText(title, 2.5f);
        Game.DrawText(sb, title, new Vector2(cx - ts.X * 0.5f, h * 0.38f),
            DesignTokens.Color.TextPrimary, 2.5f);

        float barW = Math.Min(360f, w * 0.55f);
        float barH = 12f;
        float barX = cx - barW * 0.5f;
        float barY = h * 0.52f;
        Game.DrawRect(sb, barX, barY, barW, barH, DesignTokens.Color.ShadowIndigo);
        Game.DrawRect(sb, barX, barY, barW * MathHelper.Clamp(_progress, 0, 1), barH,
            DesignTokens.Color.AccentSecondary);
        Game.DrawBorder(sb, new Rectangle((int)barX, (int)barY, (int)barW, (int)barH),
            DesignTokens.Color.BorderFocus, 1);

        string pct = ((int)(MathHelper.Clamp(_progress, 0, 1) * 100)).ToString() + "%";
        var ps = Game.MeasureText(pct, 1.8f);
        Game.DrawText(sb, pct, new Vector2(cx - ps.X * 0.5f, barY + 22),
            DesignTokens.Color.TextSecondary, 1.8f);
    }
}
