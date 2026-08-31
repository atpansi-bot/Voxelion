using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Data;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

/// <summary>
/// World Discovery browser — integrated into the game, not a web page.
/// </summary>
public sealed class SceneDiscover : SceneBase
{
    private readonly List<WorldInfo> _worlds = new();
    private int _selected = -1;
    private int _hoveredCard = -1;
    private Rectangle[] _cardRects = Array.Empty<Rectangle>();
    private Rectangle _backRect;
    private Rectangle _enterRect;

    public SceneDiscover(VoxelionGame game) : base(game)
    {
        // Seed sample worlds for UI prototype (no EXAMPLE labels — real names)
        _worlds.Add(new WorldInfo { Name = "Aether Spire", Creator = "Luminara", PlayerCount = 42, Category = "Adventure", Description = "Floating citadel among crystal clouds." });
        _worlds.Add(new WorldInfo { Name = "Obsidian Reach", Creator = "Vex", PlayerCount = 18, Category = "Competitive", Description = "Dark arena carved from volcanic glass." });
        _worlds.Add(new WorldInfo { Name = "Verdant Hollow", Creator = "Sylva", PlayerCount = 67, Category = "Social", Description = "Living forest hub with quiet glades." });
        _worlds.Add(new WorldInfo { Name = "Null Market", Creator = "Cipher", PlayerCount = 31, Category = "Market", Description = "Trade district under neon runes." });
        _worlds.Add(new WorldInfo { Name = "Dawn Forge", Creator = "Kael", PlayerCount = 9, Category = "Creative", Description = "Workshop realm for builders." });
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;

        _backRect = new Rectangle(16, 16, 100, 36);
        _enterRect = new Rectangle((int)(w - 160), (int)(h - 56), 140, 40);

        // Cards grid 2 columns
        float cardW = (w - 64) * 0.5f - 8;
        float cardH = 110;
        float startY = 70;
        _cardRects = new Rectangle[_worlds.Count];
        _hoveredCard = -1;

        for (int i = 0; i < _worlds.Count; i++)
        {
            int col = i % 2;
            int row = i / 2;
            float x = 24 + col * (cardW + 16);
            float y = startY + row * (cardH + 12);
            _cardRects[i] = new Rectangle((int)x, (int)y, (int)cardW, (int)cardH);
            if (_cardRects[i].Contains(input.PointerPosition))
                _hoveredCard = i;
        }

        if (input.IsPointerPressed)
        {
            if (_backRect.Contains(input.PointerPosition))
            {
                Game.GoBack();
                return;
            }
            if (_hoveredCard >= 0)
                _selected = _hoveredCard;
            if (_enterRect.Contains(input.PointerPosition) && _selected >= 0)
            {
                var world = _worlds[_selected];
                Game.Profile.LastWorldId = world.Id;
                Game.Profile.LastWorldName = world.Name;
                Game.TransitionTo(ApplicationState.WorldConnecting);
            }
        }

        if (input.CancelPressed)
            Game.GoBack();
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);
        Game.DrawText(sb, Game.Loc["discover.title"], new Vector2(w * 0.5f - 60, 20), DesignTokens.Color.TextPrimary, 1.3f);
        Game.DrawText(sb, Game.Loc["common.back"], new Vector2(_backRect.X + 12, _backRect.Y + 8), DesignTokens.Color.TextSecondary, 0.9f);
        Game.DrawBorder(sb, _backRect, DesignTokens.Color.BorderSubtle);

        for (int i = 0; i < _worlds.Count; i++)
        {
            var r = _cardRects[i];
            var world = _worlds[i];
            bool sel = i == _selected;
            bool hot = i == _hoveredCard;
            var bg = sel ? DesignTokens.Color.PanelElevated : DesignTokens.Color.PanelBase;
            if (hot) bg = DesignTokens.Color.PanelGlass;
            Game.DrawRect(sb, r, bg);
            Game.DrawBorder(sb, r, sel ? DesignTokens.Color.AccentPrimary : DesignTokens.Color.BorderSubtle);

            Game.DrawText(sb, world.Name, new Vector2(r.X + 14, r.Y + 12), DesignTokens.Color.TextPrimary, 1.05f);
            Game.DrawText(sb, world.Creator + "  ·  " + world.PlayerCount + " players", new Vector2(r.X + 14, r.Y + 38), DesignTokens.Color.TextSecondary, 0.8f);
            Game.DrawText(sb, world.Category, new Vector2(r.X + 14, r.Y + 60), DesignTokens.Color.AccentSecondary, 0.75f);
            Game.DrawText(sb, world.Description, new Vector2(r.X + 14, r.Y + 82), DesignTokens.Color.TextMuted, 0.7f);
        }

        if (_selected >= 0)
        {
            Game.DrawRect(sb, _enterRect, DesignTokens.Color.AccentPrimary);
            var label = Game.Loc["discover.enter"];
            var sz = Game.MeasureText(label, 0.95f);
            Game.DrawText(sb, label, new Vector2(_enterRect.X + (_enterRect.Width - sz.X) * 0.5f, _enterRect.Y + 10), DesignTokens.Color.TextPrimary, 0.95f);
        }
    }
}
