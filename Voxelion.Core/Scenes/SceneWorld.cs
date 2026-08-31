using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

/// <summary>
/// In-world gameplay HUD — staged appearance, world priority, touch + PC controls.
/// </summary>
public sealed class SceneWorld : SceneBase
{
    private float _hudAlpha;
    private float _playerX = 0.5f;
    private float _playerY = 0.55f;
    private bool _showInteract;
    private Rectangle _menuRect;
    private Rectangle _inventoryHint;
    private string _tutorialStep = "look";
    private float _tutorialTimer;

    public SceneWorld(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        _hudAlpha = 0f;
        _playerX = 0.5f;
        _playerY = 0.55f;
        _showInteract = false;
        _tutorialStep = "move";
        _tutorialTimer = 0f;
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _hudAlpha = MathHelper.Clamp(_hudAlpha + dt * 1.8f, 0f, 1f);
        _tutorialTimer += dt;

        // Movement
        var move = input.MoveAxis;
        _playerX = MathHelper.Clamp(_playerX + move.X * dt * 0.35f, 0.08f, 0.92f);
        _playerY = MathHelper.Clamp(_playerY + move.Y * dt * 0.35f, 0.2f, 0.85f);

        // Contextual interaction near center-right
        float dx = _playerX - 0.72f;
        float dy = _playerY - 0.5f;
        _showInteract = dx * dx + dy * dy < 0.02f;

        if (_showInteract && (input.InteractPressed || input.PrimaryActionPressed || input.IsPointerPressed))
        {
            // Open chest / interact — visual feedback only in UI phase
            _showInteract = false;
            if (_tutorialStep == "interact") _tutorialStep = "inventory";
        }

        if (input.MenuPressed || input.CancelPressed)
            Game.StateMachine.SetOverlay(OverlayState.QuickMenu);

        var vp = Game.GraphicsDevice.Viewport;
        _menuRect = new Rectangle(vp.Width - 70, 12, 56, 32);
        _inventoryHint = new Rectangle(16, vp.Height - 70, 48, 48);

        if (input.IsPointerPressed && _menuRect.Contains(input.PointerPosition))
            Game.StateMachine.SetOverlay(OverlayState.QuickMenu);

        // Tutorial progression
        if (_tutorialStep == "move" && move.LengthSquared() > 0.1f && _tutorialTimer > 1.5f)
            _tutorialStep = "interact";
        if (_tutorialStep == "inventory" && _tutorialTimer > 8f)
            _tutorialStep = "done";
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;

        // World backdrop (procedural terrain bands)
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.DeepNight);
        Game.DrawRect(sb, 0, h * 0.55f, w, h * 0.45f, DesignTokens.Color.ShadowIndigo * 0.7f);
        // Horizon glow
        Game.DrawRect(sb, 0, h * 0.48f, w, 8, DesignTokens.Color.AccentSecondary * 0.15f);

        // Player silhouette
        float px = _playerX * w;
        float py = _playerY * h;
        Game.DrawRect(sb, px - 12, py - 28, 24, 40, DesignTokens.Color.AccentPrimary * 0.9f);
        Game.DrawRect(sb, px - 8, py - 40, 16, 14, DesignTokens.Color.TextPrimary * 0.85f);

        // Interactable object
        Game.DrawRect(sb, w * 0.72f - 18, h * 0.5f - 14, 36, 28, DesignTokens.Color.AccentTertiary * 0.7f);

        if (_showInteract)
        {
            string prompt = "TAP  OPEN CHEST";
            var psz = Game.MeasureText(prompt, 0.9f);
            Game.DrawRect(sb, px - psz.X * 0.5f - 8, py - 70, psz.X + 16, 28, DesignTokens.Color.PanelElevated);
            Game.DrawText(sb, prompt, new Vector2(px - psz.X * 0.5f, py - 64), DesignTokens.Color.TextPrimary, 0.9f);
        }

        // Staged HUD
        byte a = (byte)(_hudAlpha * 255);
        // HP / status top-left
        Game.DrawRect(sb, 16, 16, 140, 14, new Color(42, 24, 24, 180) * _hudAlpha);
        Game.DrawRect(sb, 16, 16, 110, 14, DesignTokens.Color.AccentDanger * _hudAlpha);

        // Location
        string loc = Game.Profile.LastWorldName ?? "Frontier";
        Game.DrawText(sb, loc, new Vector2(w * 0.5f - 40, 14), DesignTokens.Color.TextSecondary * _hudAlpha, 0.85f);

        // Menu
        Game.DrawBorder(sb, _menuRect, DesignTokens.Color.BorderSubtle * _hudAlpha);
        Game.DrawText(sb, "MENU", new Vector2(_menuRect.X + 8, _menuRect.Y + 8), DesignTokens.Color.TextSecondary * _hudAlpha, 0.8f);

        // Quick slots bottom-left
        for (int i = 0; i < 4; i++)
        {
            var r = new Rectangle(16 + i * 52, (int)h - 64, 44, 44);
            Game.DrawRect(sb, r, DesignTokens.Color.PanelBase * _hudAlpha);
            Game.DrawBorder(sb, r, DesignTokens.Color.BorderSubtle * _hudAlpha);
        }

        // Tutorial contextual
        if (_tutorialStep != "done" && _hudAlpha > 0.8f)
        {
            string tip = _tutorialStep switch
            {
                "move" => Game.Loc["tutorial.move"],
                "interact" => Game.Loc["tutorial.interact"],
                "inventory" => Game.Loc["tutorial.inventory"],
                _ => ""
            };
            if (!string.IsNullOrEmpty(tip))
            {
                var tsz = Game.MeasureText(tip, 0.9f);
                Game.DrawRect(sb, (w - tsz.X) * 0.5f - 12, h * 0.22f, tsz.X + 24, 32, DesignTokens.Color.PanelElevated * 0.9f);
                Game.DrawText(sb, tip, new Vector2((w - tsz.X) * 0.5f, h * 0.22f + 8), DesignTokens.Color.AccentSecondary, 0.9f);
            }
        }
    }
}

