using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

/// <summary>World identity loading screen before spawn.</summary>
public sealed class SceneWorldLoading : SceneBase
{
    private float _progress;

    public SceneWorldLoading(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        _progress = 0;
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (dt <= 0) dt = 1f / 60f;
        _progress += dt * 0.65f;

        if (_progress >= 1f || EnterTime >= 2.2f)
            Game.TransitionTo(ApplicationState.World);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float cx = w * 0.5f;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);

        var world = Game.Profile.SelectedWorld;
        string name = world?.Name ?? "WORLD";
        string cat = world?.Category ?? "ADVENTURE";
        string creator = world?.Creator ?? "UNKNOWN";
        int players = world?.PlayerCount ?? 0;

        // Preview frame
        float pw = Math.Min(420f, w * 0.55f);
        float ph = Math.Min(160f, h * 0.28f);
        var preview = new Rectangle((int)(cx - pw * 0.5f), (int)(h * 0.18f), (int)pw, (int)ph);
        Game.DrawRect(sb, preview, DesignTokens.Color.ShadowIndigo);
        Game.DrawBorder(sb, preview, DesignTokens.Color.BorderSubtle, 2);
        Game.DrawRect(sb, cx - 40, preview.Y + ph * 0.5f - 40, 80, 80, DesignTokens.Color.AccentPrimary * 0.8f);
        Game.DrawRect(sb, cx - 20, preview.Y + ph * 0.5f - 20, 40, 40, DesignTokens.Color.AccentSecondary);

        var ns = Game.MeasureText(name, 2.6f);
        Game.DrawText(sb, name, new Vector2(cx - ns.X * 0.5f, preview.Bottom + 20), DesignTokens.Color.TextPrimary, 2.6f);

        Game.DrawText(sb, cat, new Vector2(cx - Game.MeasureText(cat, 1.3f).X * 0.5f, preview.Bottom + 52),
            DesignTokens.Color.AccentTertiary, 1.3f);

        string entering = "ENTERING THE WORLD";
        var es = Game.MeasureText(entering, 1.5f);
        Game.DrawText(sb, entering, new Vector2(cx - es.X * 0.5f, h * 0.62f), DesignTokens.Color.TextSecondary, 1.5f);

        float barW = Math.Min(360f, w * 0.5f);
        float barX = cx - barW * 0.5f;
        float barY = h * 0.70f;
        Game.DrawRect(sb, barX, barY, barW, 12, DesignTokens.Color.ShadowIndigo);
        Game.DrawRect(sb, barX, barY, barW * MathHelper.Clamp(_progress, 0, 1), 12, DesignTokens.Color.AccentSecondary);

        string meta = creator + "  ·  " + players + " PLAYERS";
        var ms = Game.MeasureText(meta, 1.2f);
        Game.DrawText(sb, meta, new Vector2(cx - ms.X * 0.5f, barY + 24), DesignTokens.Color.TextMuted, 1.2f);
    }
}
