using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneLoading : SceneBase
{
    private float _progress;
    private string _status = "Initializing world interface";
    private bool _failed;
    private float _retryTimer;

    public SceneLoading(VoxelionGame game) : base(game) { }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_failed)
        {
            _retryTimer += dt;
            if (input.IsPointerPressed || input.ConfirmPressed)
            {
                _failed = false;
                _progress = 0;
                _status = "Retrying...";
            }
            return;
        }

        _progress += dt * 0.42f;
        if (_progress < 0.3f) _status = Game.Loc["boot.preparing"];
        else if (_progress < 0.7f) _status = "Loading essential assets";
        else if (_progress < 0.95f) _status = Game.Loc["boot.initializing"];
        else _status = "Ready";

        // Simulated rare failure for robustness demo (disabled for clean flow)
        // if (_progress > 0.5f && Random.Shared.NextDouble() < 0.0001) { _failed = true; }

        if (_progress >= 1.0f)
            Game.TransitionTo(ApplicationState.Title);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float t = SceneTime;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);

        // Cinematic world art placeholder — layered silhouettes
        for (int i = 0; i < 5; i++)
        {
            float layerY = h * 0.55f + i * 18;
            float layerA = 0.08f + i * 0.03f;
            Game.DrawRect(sb, 0, layerY, w, h - layerY, DesignTokens.Color.ShadowIndigo * layerA);
        }

        // Floating particles
        for (int i = 0; i < 30; i++)
        {
            float px = ((t * 12 + i * 47) % (w + 40)) - 20;
            float py = h * 0.3f + MathF.Sin(t * 0.8f + i) * 40 + i * 8;
            Game.DrawRect(sb, px, py, 2, 2, DesignTokens.Color.AccentPrimary * 0.35f);
        }

        // Header
        Game.DrawText(sb, Game.Loc["app.name"], new Vector2(DesignTokens.Spacing.L, DesignTokens.Spacing.L),
            DesignTokens.Color.TextPrimary, 1.1f);

        // Language indicator
        string lang = Game.Loc.Current switch
        {
            Localization.Language.BahasaIndonesia => "ID",
            Localization.Language.Japanese => "JA",
            Localization.Language.Chinese => "ZH",
            Localization.Language.Korean => "KO",
            _ => "EN"
        };
        var ls = Game.MeasureText($"[ 🌐 {lang} ]", 0.8f);
        Game.DrawText(sb, $"[ 🌐 {lang} ]", new Vector2(w - ls.X - DesignTokens.Spacing.L, DesignTokens.Spacing.L),
            DesignTokens.Color.TextSecondary, 0.8f);

        // Center status
        float cx = w * 0.5f;
        string status = _failed ? Game.Loc["common.error"] : _status;
        var ss = Game.MeasureText(status, 1.0f);
        Game.DrawText(sb, status, new Vector2(cx - ss.X * 0.5f, h * 0.48f), DesignTokens.Color.TextPrimary, 1.0f);

        // Crystal progress bar
        float barW = Math.Min(420, w * 0.5f), barH = 10f;
        float barX = cx - barW * 0.5f, barY = h * 0.56f;
        Game.DrawRect(sb, barX - 2, barY - 2, barW + 4, barH + 4, DesignTokens.Color.BorderSubtle);
        Game.DrawRect(sb, barX, barY, barW, barH, DesignTokens.Color.ShadowIndigo);
        float fill = _failed ? 0 : MathHelper.Clamp(_progress, 0, 1);
        Game.DrawRect(sb, barX, barY, barW * fill, barH, DesignTokens.Color.AccentPrimary);
        // Glow on tip
        if (fill > 0.01f)
            Game.DrawRect(sb, barX + barW * fill - 6, barY - 4, 12, barH + 8, DesignTokens.Color.GlowPrimary);

        // Sub status
        string sub = _failed ? "Tap to retry" : Game.Loc["boot.initializing"];
        var subS = Game.MeasureText(sub, 0.75f);
        Game.DrawText(sb, sub, new Vector2(cx - subS.X * 0.5f, barY + 28), DesignTokens.Color.TextMuted, 0.75f);

        // Version
        Game.DrawText(sb, "1.0.0", new Vector2(w - 60, h - 30), DesignTokens.Color.TextMuted * 0.6f, 0.7f);
    }
}
