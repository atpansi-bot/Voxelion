using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneHub : SceneBase
{
    private Rectangle _btnWorld, _btnDiscover, _btnInv, _btnSocial, _btnMenu;

    public SceneHub(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        Layout();
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        float m = SafeLayout.Margin(vp);
        float bw = Math.Min(160f, (SafeLayout.SafeWidth(vp) - m * 4) / 5f);
        float bh = Math.Max(SafeLayout.TouchMin, 52f);
        float y = vp.Height - bh - m - 4;
        float total = bw * 5 + m * 4;
        float x = (vp.Width - total) * 0.5f;
        _btnWorld = new Rectangle((int)x, (int)y, (int)bw, (int)bh);
        _btnDiscover = new Rectangle((int)(x + bw + m), (int)y, (int)bw, (int)bh);
        _btnInv = new Rectangle((int)(x + 2 * (bw + m)), (int)y, (int)bw, (int)bh);
        _btnSocial = new Rectangle((int)(x + 3 * (bw + m)), (int)y, (int)bw, (int)bh);
        _btnMenu = new Rectangle((int)(x + 4 * (bw + m)), (int)y, (int)bw, (int)bh);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();
        if (!input.IsPointerReleased) return;
        var p = input.PointerPosition;
        if (_btnDiscover.Contains(p) || _btnWorld.Contains(p))
        { Game.TransitionTo(ApplicationState.WorldDiscovery); return; }
        if (_btnInv.Contains(p))
        { Game.TransitionTo(ApplicationState.Inventory); return; }
        if (_btnSocial.Contains(p))
        { Game.TransitionTo(ApplicationState.Social); return; }
        if (_btnMenu.Contains(p))
            Game.TransitionTo(ApplicationState.Settings);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height, cx = w * 0.5f, t = SceneTime;
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.DeepNight);
        for (int i = 0; i < 20; i++)
        {
            float px = (MathF.Sin(t * 0.1f + i) * 0.5f + 0.5f) * w;
            float py = (MathF.Cos(t * 0.08f + i) * 0.5f + 0.5f) * h * 0.65f;
            Game.DrawRect(sb, px, py, 2, 2, DesignTokens.Color.AccentPrimary * 0.12f);
        }

        Game.DrawRect(sb, 0, 0, w, 48, DesignTokens.Color.PanelBase);
        Game.DrawIcon(sb, "user", new Rectangle(12, 8, 32, 32), DesignTokens.Color.AccentPrimary);
        string name = (Game.Profile.DisplayName ?? "WANDERER").ToUpperInvariant();
        if (name.Length > 14) name = name[..14];
        Game.DrawText(sb, name, new Vector2(52, 10), DesignTokens.Color.TextPrimary, 1.5f);
        Game.DrawText(sb, "LV." + Math.Max(1, Game.Profile.Level), new Vector2(52, 28), DesignTokens.Color.TextMuted, 1.1f);
        UiKit.CenterLabel(Game, sb, "CELESTIAL HUB", 14, DesignTokens.Color.TextSecondary, 1.6f, w);

        float size = Math.Min(88f, h * 0.18f);
        float cy = h * 0.36f;
        float pulse = 0.9f + 0.1f * MathF.Sin(t * 2f);
        Game.DrawRect(sb, cx - size * 0.55f * pulse, cy - size * 0.55f * pulse, size * 1.1f * pulse, size * 1.1f * pulse, DesignTokens.Color.AccentPrimary);
        Game.DrawRect(sb, cx - size * 0.28f, cy - size * 0.28f, size * 0.56f, size * 0.56f, DesignTokens.Color.AccentSecondary);
        UiKit.CenterLabel(Game, sb, "WELCOME BACK", cy + size * 0.7f, DesignTokens.Color.TextSecondary, 1.7f, w);
        UiKit.CenterLabel(Game, sb, "TAP DISCOVER TO FIND WORLDS", cy + size * 0.7f + 28, DesignTokens.Color.TextMuted, 1.3f, w);

        DrawNav(sb, _btnWorld, "WORLD", "world", DesignTokens.Color.AccentPrimary);
        DrawNav(sb, _btnDiscover, "DISCOVER", "globe", DesignTokens.Color.PanelElevated);
        DrawNav(sb, _btnInv, "BAG", "bag", DesignTokens.Color.PanelElevated);
        DrawNav(sb, _btnSocial, "SOCIAL", "social", DesignTokens.Color.PanelElevated);
        DrawNav(sb, _btnMenu, "MENU", "menu", DesignTokens.Color.PanelBase);
    }

    private void DrawNav(SpriteBatch sb, Rectangle r, string label, string icon, Color fill)
    {
        Game.DrawRect(sb, r, fill);
        Game.DrawBorder(sb, r, DesignTokens.Color.BorderFocus, 2);
        Game.DrawIcon(sb, icon, new Rectangle(r.X + 8, r.Y + 6, 22, 22), DesignTokens.Color.TextPrimary);
        float scale = label.Length > 7 ? 1.2f : 1.4f;
        var size = Game.MeasureText(label, scale);
        Game.DrawText(sb, label, new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + r.Height - size.Y - 6),
            DesignTokens.Color.TextPrimary, scale);
    }
}
