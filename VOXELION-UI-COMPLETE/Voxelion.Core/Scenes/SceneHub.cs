using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

/// <summary>Celestial Hub — persistent multiplayer lobby presentation (UI only).</summary>
public sealed class SceneHub : SceneBase
{
    private Rectangle _btnWorld, _btnDiscover, _btnInv, _btnSocial, _btnMenu;
    private Rectangle _avatarHit;

    public SceneHub(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        Layout();
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        float margin = Math.Max(16f, vp.Width * 0.02f);
        float bw = Math.Min(168f, (vp.Width - margin * 6) / 5f);
        float bh = 52f;
        float y = vp.Height - bh - margin - 8f;
        float total = bw * 5 + margin * 4;
        float x = (vp.Width - total) * 0.5f;

        _btnWorld = new Rectangle((int)x, (int)y, (int)bw, (int)bh);
        _btnDiscover = new Rectangle((int)(x + (bw + margin)), (int)y, (int)bw, (int)bh);
        _btnInv = new Rectangle((int)(x + 2 * (bw + margin)), (int)y, (int)bw, (int)bh);
        _btnSocial = new Rectangle((int)(x + 3 * (bw + margin)), (int)y, (int)bw, (int)bh);
        _btnMenu = new Rectangle((int)(x + 4 * (bw + margin)), (int)y, (int)bw, (int)bh);
        _avatarHit = new Rectangle(16, 8, 200, 40);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();
        if (!input.IsPointerReleased) return;

        if (_btnDiscover.Contains(input.PointerPosition) || _btnWorld.Contains(input.PointerPosition))
        {
            Game.TransitionTo(ApplicationState.WorldDiscovery);
            return;
        }
        // Inventory / Social / Menu — UI shells reserved; stay in hub for now
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float cx = w * 0.5f;
        float t = SceneTime;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.DeepNight);

        // Soft ambient
        for (int i = 0; i < 24; i++)
        {
            float px = (MathF.Sin(t * 0.1f + i * 1.3f) * 0.5f + 0.5f) * w;
            float py = (MathF.Cos(t * 0.08f + i * 0.9f) * 0.5f + 0.5f) * h * 0.7f;
            Game.DrawRect(sb, px, py, 2, 2, DesignTokens.Color.AccentPrimary * 0.12f);
        }

        // Top bar
        Game.DrawRect(sb, 0, 0, w, 48, DesignTokens.Color.PanelBase * 0.95f);
        Game.DrawRect(sb, 0, 48, w, 1, DesignTokens.Color.BorderSubtle);

        // Avatar chip
        Game.DrawRect(sb, 12, 8, 32, 32, DesignTokens.Color.AccentPrimary);
        Game.DrawRect(sb, 18, 14, 20, 20, DesignTokens.Color.AccentSecondary);
        string name = (Game.Profile.DisplayName ?? "WANDERER").ToUpperInvariant();
        if (name.Length > 14) name = name[..14];
        Game.DrawText(sb, name, new Vector2(52, 12), DesignTokens.Color.TextPrimary, 1.5f);
        Game.DrawText(sb, "LV." + Math.Max(1, Game.Profile.Level), new Vector2(52, 28), DesignTokens.Color.TextMuted, 1.1f);

        string hubTitle = "CELESTIAL HUB";
        var ht = Game.MeasureText(hubTitle, 1.6f);
        Game.DrawText(sb, hubTitle, new Vector2(cx - ht.X * 0.5f, 16), DesignTokens.Color.TextSecondary, 1.6f);

        // Center portal motif
        float size = Math.Min(88f, h * 0.18f);
        float cy = h * 0.36f;
        float pulse = 0.85f + 0.15f * MathF.Sin(t * 2f);
        Game.DrawRect(sb, cx - size * 1.2f * pulse, cy - size * 1.2f * pulse,
            size * 2.4f * pulse, size * 2.4f * pulse, DesignTokens.Color.GlowPrimary * 0.35f);
        Game.DrawRect(sb, cx - size * 0.55f, cy - size * 0.55f, size * 1.1f, size * 1.1f, DesignTokens.Color.AccentPrimary);
        Game.DrawRect(sb, cx - size * 0.28f, cy - size * 0.28f, size * 0.56f, size * 0.56f, DesignTokens.Color.AccentSecondary);

        string welcome = "WELCOME BACK";
        var ws = Game.MeasureText(welcome, 1.7f);
        Game.DrawText(sb, welcome, new Vector2(cx - ws.X * 0.5f, cy + size * 0.75f), DesignTokens.Color.TextSecondary, 1.7f);

        string hint = "TAP DISCOVER TO FIND WORLDS";
        var hs = Game.MeasureText(hint, 1.3f);
        Game.DrawText(sb, hint, new Vector2(cx - hs.X * 0.5f, cy + size * 0.75f + 28), DesignTokens.Color.TextMuted, 1.3f);

        DrawBtn(sb, _btnWorld, "WORLD", DesignTokens.Color.AccentPrimary);
        DrawBtn(sb, _btnDiscover, "DISCOVER", DesignTokens.Color.PanelElevated);
        DrawBtn(sb, _btnInv, "BAG", DesignTokens.Color.PanelElevated);
        DrawBtn(sb, _btnSocial, "SOCIAL", DesignTokens.Color.PanelElevated);
        DrawBtn(sb, _btnMenu, "MENU", DesignTokens.Color.PanelBase);
    }

    private void DrawBtn(SpriteBatch sb, Rectangle r, string label, Color fill)
    {
        Game.DrawRect(sb, r, fill);
        Game.DrawBorder(sb, r, DesignTokens.Color.BorderFocus, 2);
        float scale = label.Length > 7 ? 1.3f : 1.6f;
        var size = Game.MeasureText(label, scale);
        Game.DrawText(sb, label,
            new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + (r.Height - size.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, scale);
    }
}
