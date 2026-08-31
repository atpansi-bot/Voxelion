using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

/// <summary>
/// Celestial Hub — multiplayer social environment with bottom navigation.
/// World remains visible; panels open as overlays.
/// </summary>
public sealed class SceneHub : SceneBase
{
    private readonly string[] _navKeys = { "hub.world", "hub.inventory", "hub.social", "hub.discover", "hub.menu" };
    private Rectangle[] _navRects = Array.Empty<Rectangle>();
    private int _hoveredNav = -1;
    private float _hudAlpha;

    public SceneHub(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        _hudAlpha = 0f;
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _hudAlpha = MathHelper.Clamp(_hudAlpha + dt * 2.5f, 0f, 1f);

        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float navH = 56f;
        float btnW = w / _navKeys.Length;
        _navRects = new Rectangle[_navKeys.Length];
        _hoveredNav = -1;

        for (int i = 0; i < _navKeys.Length; i++)
        {
            _navRects[i] = new Rectangle((int)(i * btnW), (int)(h - navH), (int)btnW, (int)navH);
            if (_navRects[i].Contains(input.PointerPosition))
                _hoveredNav = i;
        }

        if (input.IsPointerPressed && _hoveredNav >= 0)
        {
            switch (_hoveredNav)
            {
                case 0: // WORLD / portal
                case 3: // DISCOVER
                    Game.TransitionTo(ApplicationState.WorldDiscovery);
                    break;
                case 1: // INVENTORY
                    Game.StateMachine.SetOverlay(OverlayState.Inventory);
                    break;
                case 2: // SOCIAL
                    Game.StateMachine.SetOverlay(OverlayState.Social);
                    break;
                case 4: // MENU
                    Game.StateMachine.SetOverlay(OverlayState.Settings);
                    break;
            }
        }

        if (input.CancelPressed || input.MenuPressed)
            Game.StateMachine.SetOverlay(OverlayState.None);
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        byte a = (byte)(_hudAlpha * 255);

        // Atmospheric hub background
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.DeepNight);
        // Soft gradient bands
        Game.DrawRect(sb, 0, 0, w, h * 0.35f, DesignTokens.Color.ShadowIndigo * 0.6f);

        // Top HUD bar
        Game.DrawRect(sb, 0, 0, w, 52, DesignTokens.Color.PanelBase * _hudAlpha);
        string name = string.IsNullOrEmpty(Game.Profile.DisplayName) ? "Traveler" : Game.Profile.DisplayName;
        Game.DrawText(sb, $"{name}  Lv.{Game.Profile.Level}", new Vector2(20, 16), DesignTokens.Color.TextPrimary * _hudAlpha, 1.05f);
        Game.DrawText(sb, Game.Loc["hub.friends"], new Vector2(w - 160, 16), DesignTokens.Color.TextSecondary * _hudAlpha, 0.9f);
        Game.DrawText(sb, Game.Loc["hub.mail"], new Vector2(w - 80, 16), DesignTokens.Color.TextSecondary * _hudAlpha, 0.9f);

        // Center presence hint
        string hubLabel = "CELESTIAL HUB";
        var m = Game.MeasureText(hubLabel, 1.4f);
        Game.DrawText(sb, hubLabel, new Vector2((w - m.X) * 0.5f, h * 0.42f), DesignTokens.Color.AccentSecondary * _hudAlpha, 1.4f);

        // Bottom navigation
        float navH = 56f;
        Game.DrawRect(sb, 0, h - navH, w, navH, DesignTokens.Color.PanelElevated * _hudAlpha);
        for (int i = 0; i < _navKeys.Length; i++)
        {
            var r = _navRects[i];
            bool hot = i == _hoveredNav;
            if (hot)
                Game.DrawRect(sb, r, DesignTokens.Color.AccentPrimary * 0.25f * _hudAlpha);
            string label = Game.Loc[_navKeys[i]];
            var sz = Game.MeasureText(label, 0.85f);
            Game.DrawText(sb, label,
                new Vector2(r.X + (r.Width - sz.X) * 0.5f, r.Y + (r.Height - sz.Y) * 0.5f),
                (hot ? DesignTokens.Color.AccentSecondary : DesignTokens.Color.TextSecondary) * _hudAlpha, 0.85f);
        }
    }
}
