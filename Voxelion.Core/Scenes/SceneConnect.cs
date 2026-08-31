using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

/// <summary>
/// World connection phases with meaningful progress and recovery actions.
/// </summary>
public sealed class SceneConnect : SceneBase
{
    private enum Phase { Connecting, Authenticating, LoadingRegion, SyncingPlayer, Spawning, Failed }
    private Phase _phase = Phase.Connecting;
    private float _phaseTimer;
    private float _overall;
    private bool _failed;
    private Rectangle _retryRect;
    private Rectangle _cancelRect;

    private static readonly (Phase phase, string key, float duration)[] Pipeline =
    {
        (Phase.Connecting, "connect.connecting", 0.7f),
        (Phase.Authenticating, "connect.auth", 0.55f),
        (Phase.LoadingRegion, "connect.region", 0.9f),
        (Phase.SyncingPlayer, "connect.sync", 0.5f),
        (Phase.Spawning, "connect.spawn", 0.4f)
    };

    public SceneConnect(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        _phase = Phase.Connecting;
        _phaseTimer = 0f;
        _overall = 0f;
        _failed = false;
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var vp = Game.GraphicsDevice.Viewport;

        _retryRect = new Rectangle(vp.Width / 2 - 140, vp.Height / 2 + 80, 120, 40);
        _cancelRect = new Rectangle(vp.Width / 2 + 20, vp.Height / 2 + 80, 120, 40);

        if (_failed)
        {
            if (input.IsPointerPressed)
            {
                if (_retryRect.Contains(input.PointerPosition))
                {
                    _failed = false;
                    _phase = Phase.Connecting;
                    _phaseTimer = 0f;
                    _overall = 0f;
                }
                else if (_cancelRect.Contains(input.PointerPosition))
                    Game.TransitionTo(ApplicationState.Hub);
            }
            return;
        }

        _phaseTimer += dt;
        int idx = (int)_phase;
        if (idx < Pipeline.Length)
        {
            float dur = Pipeline[idx].duration;
            float local = MathHelper.Clamp(_phaseTimer / dur, 0f, 1f);
            _overall = (idx + local) / Pipeline.Length;

            if (_phaseTimer >= dur)
            {
                _phaseTimer = 0f;
                if (idx + 1 >= Pipeline.Length)
                {
                    Game.TransitionTo(ApplicationState.WorldLoading);
                    return;
                }
                _phase = Pipeline[idx + 1].phase;
            }
        }

        // Simulate rare failure for recovery UX (disabled by default for clean flow)
        // if (_overall > 0.3f && Random.Shared.NextDouble() < 0.0001) { _failed = true; _phase = Phase.Failed; }

        if (input.CancelPressed)
            Game.TransitionTo(ApplicationState.Hub);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);

        string worldName = Game.Profile.LastWorldName ?? "World";
        var titleSz = Game.MeasureText(worldName, 1.4f);
        Game.DrawText(sb, worldName, new Vector2((w - titleSz.X) * 0.5f, h * 0.28f), DesignTokens.Color.TextPrimary, 1.4f);

        if (_failed)
        {
            Game.DrawText(sb, Game.Loc["error.connection_lost"], new Vector2(w * 0.5f - 90, h * 0.45f), DesignTokens.Color.AccentDanger, 1.1f);
            Game.DrawRect(sb, _retryRect, DesignTokens.Color.AccentPrimary);
            Game.DrawText(sb, Game.Loc["error.retry"], new Vector2(_retryRect.X + 28, _retryRect.Y + 10), DesignTokens.Color.TextPrimary, 0.9f);
            Game.DrawBorder(sb, _cancelRect, DesignTokens.Color.BorderSubtle);
            Game.DrawText(sb, Game.Loc["error.return_hub"], new Vector2(_cancelRect.X + 8, _cancelRect.Y + 10), DesignTokens.Color.TextSecondary, 0.85f);
            return;
        }

        int idx = Math.Min((int)_phase, Pipeline.Length - 1);
        string status = Game.Loc[Pipeline[idx].key];
        var stSz = Game.MeasureText(status, 1f);
        Game.DrawText(sb, status, new Vector2((w - stSz.X) * 0.5f, h * 0.48f), DesignTokens.Color.TextSecondary, 1f);

        // Progress bar
        int barW = (int)(w * 0.45f);
        int barX = (int)((w - barW) * 0.5f);
        int barY = (int)(h * 0.55f);
        Game.DrawRect(sb, barX, barY, barW, 10, DesignTokens.Color.ShadowIndigo);
        Game.DrawRect(sb, barX, barY, (int)(barW * _overall), 10, DesignTokens.Color.AccentSecondary);
    }
}
