using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

/// <summary>
/// In-world presentation: staged HUD fade-in, mobile landscape controls (visual),
/// interaction prompt, quick menu shell. No gameplay simulation.
/// </summary>
public sealed class SceneWorld : SceneBase
{
    private float _hudAlpha;
    private bool _showQuickMenu;
    private bool _tutorialMoveDone;
    private bool _tutorialJumpDone;
    private Rectangle _pad, _btnJump, _btnAct, _btnInteract, _btnMenu;
    private Rectangle _qmInv, _qmChar, _qmWorlds, _qmSocial, _qmSettings, _qmClose;
    private Vector2 _playerPos;
    private string _interactLabel = "";

    public SceneWorld(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        _hudAlpha = 0;
        _showQuickMenu = false;
        _tutorialMoveDone = false;
        _tutorialJumpDone = false;
        _interactLabel = "";
        var vp = Game.GraphicsDevice.Viewport;
        _playerPos = new Vector2(vp.Width * 0.5f, vp.Height * 0.55f);
        Layout();
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        float margin = Math.Max(16f, vp.Width * 0.02f);
        float padSize = Math.Min(120f, vp.Height * 0.28f);
        _pad = new Rectangle((int)margin, (int)(vp.Height - padSize - margin), (int)padSize, (int)padSize);

        float btn = Math.Min(56f, vp.Height * 0.12f);
        float right = vp.Width - margin - btn;
        float by = vp.Height - margin - btn;
        _btnJump = new Rectangle((int)right, (int)(by - btn - 12), (int)btn, (int)btn);
        _btnAct = new Rectangle((int)(right - btn - 12), (int)by, (int)btn, (int)btn);
        _btnInteract = new Rectangle((int)(right - btn - 12), (int)(by - btn - 12), (int)btn, (int)btn);
        _btnMenu = new Rectangle((int)(vp.Width - margin - 48), (int)margin, 48, 40);

        // Quick menu panel
        float qw = Math.Min(280f, vp.Width * 0.4f);
        float qh = 320f;
        float qx = vp.Width * 0.5f - qw * 0.5f;
        float qy = vp.Height * 0.5f - qh * 0.5f;
        float rowH = 44f;
        float gap = 8f;
        float ry = qy + 48;
        _qmInv = new Rectangle((int)(qx + 20), (int)ry, (int)(qw - 40), (int)rowH);
        _qmChar = new Rectangle((int)(qx + 20), (int)(ry + (rowH + gap)), (int)(qw - 40), (int)rowH);
        _qmWorlds = new Rectangle((int)(qx + 20), (int)(ry + 2 * (rowH + gap)), (int)(qw - 40), (int)rowH);
        _qmSocial = new Rectangle((int)(qx + 20), (int)(ry + 3 * (rowH + gap)), (int)(qw - 40), (int)rowH);
        _qmSettings = new Rectangle((int)(qx + 20), (int)(ry + 4 * (rowH + gap)), (int)(qw - 40), (int)rowH);
        _qmClose = new Rectangle((int)(qx + 20), (int)(ry + 5 * (rowH + gap)), (int)(qw - 40), (int)rowH);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (dt <= 0) dt = 1f / 60f;
        _hudAlpha = MathHelper.Clamp(_hudAlpha + dt * 0.8f, 0, 1);

        // Movement from axis or virtual pad
        Vector2 move = input.MoveAxis;
        if (input.IsPointerDown && _pad.Contains(input.PointerPosition))
        {
            var center = new Vector2(_pad.Center.X, _pad.Center.Y);
            var delta = input.PointerPosition.ToVector2() - center;
            if (delta.LengthSquared() > 1)
            {
                delta.Normalize();
                move += delta;
            }
            _tutorialMoveDone = true;
        }

        if (move.LengthSquared() > 0.01f)
        {
            move.Normalize();
            _playerPos += move * 160f * dt;
            var vp = Game.GraphicsDevice.Viewport;
            _playerPos.X = MathHelper.Clamp(_playerPos.X, 40, vp.Width - 40);
            _playerPos.Y = MathHelper.Clamp(_playerPos.Y, 60, vp.Height - 60);
            _tutorialMoveDone = true;
        }

        // Proximity interaction demo marker
        var marker = new Vector2(Game.GraphicsDevice.Viewport.Width * 0.72f, Game.GraphicsDevice.Viewport.Height * 0.45f);
        if (Vector2.Distance(_playerPos, marker) < 70f)
            _interactLabel = "TAP  OPEN CHEST";
        else
            _interactLabel = "";

        if (!input.IsPointerReleased && !input.JumpPressed && !input.InteractPressed && !input.MenuPressed)
            return;

        if (_showQuickMenu)
        {
            if (input.IsPointerReleased)
            {
                if (_qmInv.Contains(input.PointerPosition))
                { Game.TransitionTo(ApplicationState.Inventory); return; }
                if (_qmWorlds.Contains(input.PointerPosition))
                { Game.TransitionTo(ApplicationState.WorldDiscovery); return; }
                if (_qmSocial.Contains(input.PointerPosition))
                { Game.TransitionTo(ApplicationState.Social); return; }
                if (_qmSettings.Contains(input.PointerPosition))
                { Game.TransitionTo(ApplicationState.Settings); return; }
                if (_qmClose.Contains(input.PointerPosition) || _btnMenu.Contains(input.PointerPosition))
                    _showQuickMenu = false;
            }
            return;
        }

        if ((input.IsPointerReleased && _btnMenu.Contains(input.PointerPosition)) || input.MenuPressed)
        {
            Game.TransitionTo(ApplicationState.PauseMenu);
            return;
        }

        if (input.JumpPressed || (input.IsPointerReleased && _btnJump.Contains(input.PointerPosition)))
            _tutorialJumpDone = true;

        if (input.IsPointerReleased && _btnInteract.Contains(input.PointerPosition) && _interactLabel.Length > 0)
        {
            // UI-only feedback: clear label briefly
            _interactLabel = "OPENED";
        }
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float a = _hudAlpha;

        // World backdrop (UI placeholder terrain bands)
        Game.DrawRect(sb, 0, 0, w, h, new Color(12, 18, 28));
        Game.DrawRect(sb, 0, h * 0.62f, w, h * 0.38f, new Color(18, 28, 22));
        for (int i = 0; i < 8; i++)
        {
            float bx = (i * 140 + SceneTime * 12) % (w + 80) - 40;
            Game.DrawRect(sb, bx, h * 0.55f - (i % 3) * 20, 60, 40 + (i % 3) * 15, new Color(30, 42, 36));
        }

        // Interactable marker
        var marker = new Vector2(w * 0.72f, h * 0.45f);
        Game.DrawRect(sb, marker.X - 18, marker.Y - 18, 36, 36, DesignTokens.Color.AccentTertiary * 0.7f);

        // Player avatar
        Game.DrawRect(sb, _playerPos.X - 16, _playerPos.Y - 28, 32, 48, DesignTokens.Color.AccentPrimary);
        Game.DrawRect(sb, _playerPos.X - 10, _playerPos.Y - 40, 20, 16, DesignTokens.Color.AccentSecondary);

        if (a < 0.05f) return;

        // Top HUD
        Game.DrawRect(sb, 12, 12, 140, 36, DesignTokens.Color.PanelGlass * a);
        Game.DrawRect(sb, 18, 20, 80 * a, 10, DesignTokens.Color.AccentSuccess * a); // HP bar visual
        Game.DrawText(sb, "HP", new Vector2(105, 18), DesignTokens.Color.TextPrimary * a, 1.2f);

        string loc = Game.Profile.SelectedWorld?.Name ?? "WORLD";
        if (loc.Length > 16) loc = loc[..16];
        var ls = Game.MeasureText(loc, 1.3f);
        Game.DrawText(sb, loc, new Vector2(w - ls.X - 70, 18), DesignTokens.Color.TextSecondary * a, 1.3f);

        DrawBtn(sb, _btnMenu, "=", DesignTokens.Color.PanelElevated * a, 2f);

        // Interaction prompt
        if (_interactLabel.Length > 0)
        {
            var isz = Game.MeasureText(_interactLabel, 1.5f);
            float ix = _playerPos.X - isz.X * 0.5f;
            float iy = _playerPos.Y - 64;
            Game.DrawRect(sb, ix - 8, iy - 4, isz.X + 16, isz.Y + 10, DesignTokens.Color.PanelBase * a);
            Game.DrawText(sb, _interactLabel, new Vector2(ix, iy), DesignTokens.Color.TextPrimary * a, 1.5f);
        }

        // Mobile controls
        Game.DrawRect(sb, _pad, DesignTokens.Color.PanelGlass * (0.45f * a));
        Game.DrawBorder(sb, _pad, DesignTokens.Color.BorderSubtle * a, 2);
        Game.DrawRect(sb, _pad.Center.X - 12, _pad.Center.Y - 12, 24, 24, DesignTokens.Color.AccentSecondary * a);

        DrawRoundBtn(sb, _btnJump, "J", a);
        DrawRoundBtn(sb, _btnAct, "A", a);
        DrawRoundBtn(sb, _btnInteract, "E", a);

        // Tutorial chips
        if (!_tutorialMoveDone && EnterTime > 0.8f)
        {
            string tip = "MOVE WITH PAD OR WASD";
            var tz = Game.MeasureText(tip, 1.4f);
            Game.DrawText(sb, tip, new Vector2(w * 0.5f - tz.X * 0.5f, h * 0.22f), DesignTokens.Color.TextPrimary * a, 1.4f);
        }
        else if (!_tutorialJumpDone && _tutorialMoveDone)
        {
            string tip = "PRESS J TO JUMP";
            var tz = Game.MeasureText(tip, 1.4f);
            Game.DrawText(sb, tip, new Vector2(w * 0.5f - tz.X * 0.5f, h * 0.22f), DesignTokens.Color.TextPrimary * a, 1.4f);
        }

        if (_showQuickMenu)
            DrawQuickMenu(sb, a);
    }

    private void DrawQuickMenu(SpriteBatch sb, float a)
    {
        var vp = Game.GraphicsDevice.Viewport;
        Game.DrawRect(sb, 0, 0, vp.Width, vp.Height, DesignTokens.Color.OverlayDim * a);

        float qw = Math.Min(280f, vp.Width * 0.4f);
        float qh = 320f;
        float qx = vp.Width * 0.5f - qw * 0.5f;
        float qy = vp.Height * 0.5f - qh * 0.5f;
        var panel = new Rectangle((int)qx, (int)qy, (int)qw, (int)qh);
        Game.DrawRect(sb, panel, DesignTokens.Color.PanelElevated);
        Game.DrawBorder(sb, panel, DesignTokens.Color.AccentPrimary, 2);

        string title = "QUICK MENU";
        var ts = Game.MeasureText(title, 1.8f);
        Game.DrawText(sb, title, new Vector2(qx + (qw - ts.X) * 0.5f, qy + 16), DesignTokens.Color.TextPrimary, 1.8f);

        DrawBtn(sb, _qmInv, "INVENTORY", DesignTokens.Color.PanelBase, 1.5f);
        DrawBtn(sb, _qmChar, "CHARACTER", DesignTokens.Color.PanelBase, 1.5f);
        DrawBtn(sb, _qmWorlds, "WORLDS", DesignTokens.Color.AccentPrimary, 1.5f);
        DrawBtn(sb, _qmSocial, "SOCIAL", DesignTokens.Color.PanelBase, 1.5f);
        DrawBtn(sb, _qmSettings, "SETTINGS", DesignTokens.Color.PanelBase, 1.5f);
        DrawBtn(sb, _qmClose, "RESUME", DesignTokens.Color.PanelElevated, 1.5f);
    }

    private void DrawRoundBtn(SpriteBatch sb, Rectangle r, string label, float a)
    {
        Game.DrawRect(sb, r, DesignTokens.Color.PanelElevated * (0.7f * a));
        Game.DrawBorder(sb, r, DesignTokens.Color.BorderFocus * a, 2);
        var size = Game.MeasureText(label, 1.8f);
        Game.DrawText(sb, label,
            new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + (r.Height - size.Y) * 0.5f),
            DesignTokens.Color.TextPrimary * a, 1.8f);
    }

    private void DrawBtn(SpriteBatch sb, Rectangle r, string label, Color fill, float scale)
    {
        Game.DrawRect(sb, r, fill);
        Game.DrawBorder(sb, r, DesignTokens.Color.BorderFocus, 1);
        var size = Game.MeasureText(label, scale);
        Game.DrawText(sb, label,
            new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + (r.Height - size.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, scale);
    }
}
