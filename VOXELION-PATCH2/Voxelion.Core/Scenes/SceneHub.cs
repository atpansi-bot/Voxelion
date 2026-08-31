using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

public sealed class SceneHub : SceneBase
{
    private Rectangle _btnDiscover;
    private Rectangle _btnWorld;
    private Rectangle _btnSocial;
    private Rectangle _btnInv;

    public SceneHub(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        Layout();
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        float margin = 24f;
        float bw = Math.Min(200f, (vp.Width - margin * 5) / 4f);
        float bh = 56f;
        float y = vp.Height * 0.55f;
        float total = bw * 4 + margin * 3;
        float x = (vp.Width - total) * 0.5f;

        _btnWorld = new Rectangle((int)x, (int)y, (int)bw, (int)bh);
        _btnDiscover = new Rectangle((int)(x + bw + margin), (int)y, (int)bw, (int)bh);
        _btnInv = new Rectangle((int)(x + 2 * (bw + margin)), (int)y, (int)bw, (int)bh);
        _btnSocial = new Rectangle((int)(x + 3 * (bw + margin)), (int)y, (int)bw, (int)bh);
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
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float cx = w * 0.5f;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.DeepNight);

        // Top bar
        Game.DrawRect(sb, 0, 0, w, 48, DesignTokens.Color.PanelBase);
        string hub = "CELESTIAL HUB";
        Game.DrawText(sb, hub, new Vector2(24, 16), DesignTokens.Color.TextPrimary, 2f);

        string name = (Game.Profile.DisplayName ?? "WANDERER").ToUpperInvariant();
        var ns = Game.MeasureText(name, 1.5f);
        Game.DrawText(sb, name, new Vector2(w - ns.X - 24, 18), DesignTokens.Color.AccentSecondary, 1.5f);

        // Center emblem
        float size = 72f;
        float cy = h * 0.32f;
        Game.DrawRect(sb, cx - size * 0.55f, cy - size * 0.55f, size * 1.1f, size * 1.1f, DesignTokens.Color.AccentPrimary);
        Game.DrawRect(sb, cx - size * 0.28f, cy - size * 0.28f, size * 0.56f, size * 0.56f, DesignTokens.Color.AccentSecondary);

        string welcome = "WELCOME BACK";
        var ws = Game.MeasureText(welcome, 1.8f);
        Game.DrawText(sb, welcome, new Vector2(cx - ws.X * 0.5f, cy + size * 0.7f),
            DesignTokens.Color.TextSecondary, 1.8f);

        DrawBtn(sb, _btnWorld, "WORLD", DesignTokens.Color.AccentPrimary);
        DrawBtn(sb, _btnDiscover, "DISCOVER", DesignTokens.Color.PanelElevated);
        DrawBtn(sb, _btnInv, "INVENTORY", DesignTokens.Color.PanelElevated);
        DrawBtn(sb, _btnSocial, "SOCIAL", DesignTokens.Color.PanelElevated);

        string hint = "TAP DISCOVER TO FIND WORLDS";
        var hs = Game.MeasureText(hint, 1.3f);
        Game.DrawText(sb, hint, new Vector2(cx - hs.X * 0.5f, h * 0.78f),
            DesignTokens.Color.TextMuted, 1.3f);
    }

    private void DrawBtn(SpriteBatch sb, Rectangle r, string label, Color fill)
    {
        Game.DrawRect(sb, r, fill);
        Game.DrawBorder(sb, r, DesignTokens.Color.BorderFocus, 2);
        float scale = label.Length > 8 ? 1.4f : 1.7f;
        var size = Game.MeasureText(label, scale);
        Game.DrawText(sb, label,
            new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + (r.Height - size.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, scale);
    }
}
