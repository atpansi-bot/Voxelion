using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

/// <summary>World connection pipeline with staged progress and recovery actions.</summary>
public sealed class SceneConnect : SceneBase
{
    private static readonly string[] Stages =
    {
        "CONNECTING",
        "AUTHENTICATING",
        "LOADING REGION",
        "SYNCING PLAYER",
        "SPAWNING"
    };

    private int _stage;
    private float _stageTimer;
    private bool _failed;
    private Rectangle _btnRetry, _btnCancel, _btnHub;

    public SceneConnect(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        _stage = 0;
        _stageTimer = 0;
        _failed = false;
        Layout();
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        float cx = vp.Width * 0.5f;
        float bw = Math.Min(160f, vp.Width * 0.28f);
        float bh = 44f;
        float y = vp.Height * 0.78f;
        _btnRetry = new Rectangle((int)(cx - bw * 1.6f), (int)y, (int)bw, (int)bh);
        _btnCancel = new Rectangle((int)(cx - bw * 0.5f), (int)y, (int)bw, (int)bh);
        _btnHub = new Rectangle((int)(cx + bw * 0.6f), (int)y, (int)bw, (int)bh);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (dt <= 0) dt = 1f / 60f;

        if (_failed)
        {
            if (!input.IsPointerReleased) return;
            if (_btnRetry.Contains(input.PointerPosition))
            {
                _failed = false;
                _stage = 0;
                _stageTimer = 0;
                return;
            }
            if (_btnCancel.Contains(input.PointerPosition) || _btnHub.Contains(input.PointerPosition))
            {
                Game.TransitionTo(ApplicationState.Hub);
            }
            return;
        }

        _stageTimer += dt;
        // Simulate rare failure only if forced offline — otherwise advance
        if (!Game.Session.IsNetworkAvailable && _stage >= 1)
        {
            _failed = true;
            return;
        }

        float stageDuration = 0.55f;
        if (_stageTimer >= stageDuration)
        {
            _stageTimer = 0;
            _stage++;
            if (_stage >= Stages.Length)
                Game.TransitionTo(ApplicationState.WorldLoading);
        }
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float cx = w * 0.5f;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.DeepNight);

        var world = Game.Profile.SelectedWorld;
        string worldName = world?.Name ?? "UNKNOWN WORLD";
        var ns = Game.MeasureText(worldName, 2.4f);
        Game.DrawText(sb, worldName, new Vector2(cx - ns.X * 0.5f, h * 0.18f), DesignTokens.Color.TextPrimary, 2.4f);

        if (world != null)
        {
            string meta = world.Category + "  ·  " + world.PlayerCount + " PLAYERS";
            var ms = Game.MeasureText(meta, 1.3f);
            Game.DrawText(sb, meta, new Vector2(cx - ms.X * 0.5f, h * 0.26f), DesignTokens.Color.TextMuted, 1.3f);
        }

        if (_failed)
        {
            string err = "CONNECTION LOST";
            var es = Game.MeasureText(err, 2.2f);
            Game.DrawText(sb, err, new Vector2(cx - es.X * 0.5f, h * 0.42f), DesignTokens.Color.AccentDanger, 2.2f);
            string sub = "SESSION COULD NOT BE SYNCHRONIZED";
            var ss = Game.MeasureText(sub, 1.3f);
            Game.DrawText(sb, sub, new Vector2(cx - ss.X * 0.5f, h * 0.50f), DesignTokens.Color.TextSecondary, 1.3f);

            DrawBtn(sb, _btnRetry, "RETRY", DesignTokens.Color.AccentPrimary);
            DrawBtn(sb, _btnCancel, "CANCEL", DesignTokens.Color.PanelElevated);
            DrawBtn(sb, _btnHub, "HUB", DesignTokens.Color.PanelBase);
            return;
        }

        int displayStage = Math.Min(_stage, Stages.Length - 1);
        string stage = Stages[displayStage];
        var st = Game.MeasureText(stage, 2f);
        Game.DrawText(sb, stage, new Vector2(cx - st.X * 0.5f, h * 0.42f), DesignTokens.Color.AccentSecondary, 2f);

        // Stage dots
        float dotY = h * 0.55f;
        float totalW = Stages.Length * 28f;
        float startX = cx - totalW * 0.5f;
        for (int i = 0; i < Stages.Length; i++)
        {
            Color c = i < _stage ? DesignTokens.Color.AccentSuccess
                : i == _stage ? DesignTokens.Color.AccentPrimary
                : DesignTokens.Color.ShadowIndigo;
            Game.DrawRect(sb, startX + i * 28, dotY, 16, 16, c);
        }

        float progress = (_stage + _stageTimer / 0.55f) / Stages.Length;
        float barW = Math.Min(360f, w * 0.5f);
        float barX = cx - barW * 0.5f;
        float barY = h * 0.62f;
        Game.DrawRect(sb, barX, barY, barW, 12, DesignTokens.Color.ShadowIndigo);
        Game.DrawRect(sb, barX, barY, barW * MathHelper.Clamp(progress, 0, 1), 12, DesignTokens.Color.AccentPrimary);
    }

    private void DrawBtn(SpriteBatch sb, Rectangle r, string label, Color fill)
    {
        Game.DrawRect(sb, r, fill);
        Game.DrawBorder(sb, r, DesignTokens.Color.BorderFocus, 2);
        var size = Game.MeasureText(label, 1.5f);
        Game.DrawText(sb, label,
            new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + (r.Height - size.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, 1.5f);
    }
}
