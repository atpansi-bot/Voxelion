using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Data;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

/// <summary>World Discovery browser — cards, tabs, detail panel, enter action.</summary>
public sealed class SceneDiscover : SceneBase
{
    private readonly List<WorldInfo> _worlds = new();
    private readonly string[] _tabs = { "RECOMMENDED", "TRENDING", "NEW", "FRIENDS" };
    private int _tabIndex;
    private int _selected = -1;
    private bool _showDetail;
    private Rectangle _btnBack, _btnEnter, _btnFavorite, _btnCloseDetail;
    private readonly List<Rectangle> _tabRects = new();
    private readonly List<Rectangle> _cardRects = new();
    private float _scrollY;

    public SceneDiscover(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        SeedWorlds();
        _selected = -1;
        _showDetail = false;
        _tabIndex = 0;
        _scrollY = 0;
        Layout();
    }

    private void SeedWorlds()
    {
        _worlds.Clear();
        _worlds.Add(new WorldInfo
        {
            Id = "a1b2c3d4", Name = "AETHER REACH", Creator = "LUNA", PlayerCount = 42,
            Category = "ADVENTURE", Tags = new[] { "EXPLORE", "QUEST" },
            Description = "FLOATING ISLANDS AND CRYSTAL PATHS.", ConnectionQuality = 0.95f
        });
        _worlds.Add(new WorldInfo
        {
            Id = "e5f6g7h8", Name = "EMBER BAZAAR", Creator = "KAEL", PlayerCount = 128,
            Category = "SOCIAL", Tags = new[] { "MARKET", "TRADE" },
            Description = "NIGHT MARKET UNDER VOLCANIC SKY.", ConnectionQuality = 0.88f, IsFavorite = true
        });
        _worlds.Add(new WorldInfo
        {
            Id = "i9j0k1l2", Name = "SILK HOLLOW", Creator = "MIRA", PlayerCount = 17,
            Category = "CREATIVE", Tags = new[] { "BUILD", "ART" },
            Description = "QUIET VALLEY FOR BUILDERS.", ConnectionQuality = 0.99f
        });
        _worlds.Add(new WorldInfo
        {
            Id = "m3n4o5p6", Name = "STORM SPIRE", Creator = "REX", PlayerCount = 64,
            Category = "COMPETITIVE", Tags = new[] { "ARENA", "PVP" },
            Description = "HIGH TOWER DUELS AND RACES.", ConnectionQuality = 0.72f
        });
        _worlds.Add(new WorldInfo
        {
            Id = "q7r8s9t0", Name = "DEW GARDEN", Creator = "AYA", PlayerCount = 9,
            Category = "ADVENTURE", Tags = new[] { "PEACEFUL", "STORY" },
            Description = "GENTLE RAIN AND HIDDEN SHRINES.", ConnectionQuality = 1f
        });
        _worlds.Add(new WorldInfo
        {
            Id = "u1v2w3x4", Name = "NIGHT DOCKS", Creator = "JON", PlayerCount = 33,
            Category = "SOCIAL", Tags = new[] { "CHAT", "MUSIC" },
            Description = "LANTERNS ON THE WATERFRONT.", ConnectionQuality = 0.91f
        });
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        float pad = Math.Max(12f, vp.Width * 0.015f);
        _btnBack = new Rectangle((int)pad, (int)pad, 100, 40);

        _tabRects.Clear();
        float tabY = pad + 48;
        float tabH = 36;
        float tabW = Math.Min(140f, (vp.Width - pad * 2 - 12 * 3) / 4f);
        float tx = pad;
        for (int i = 0; i < _tabs.Length; i++)
        {
            _tabRects.Add(new Rectangle((int)tx, (int)tabY, (int)tabW, (int)tabH));
            tx += tabW + 12;
        }

        _cardRects.Clear();
        float cardTop = tabY + tabH + 16;
        float cardW = Math.Min(300f, (vp.Width - pad * 3) / 2f);
        float cardH = 110f;
        float gap = 12f;
        int cols = vp.Width >= 900 ? 3 : 2;
        cardW = (vp.Width - pad * (cols + 1) - gap * (cols - 1)) / cols;
        for (int i = 0; i < _worlds.Count; i++)
        {
            int col = i % cols;
            int row = i / cols;
            float x = pad + col * (cardW + gap);
            float y = cardTop + row * (cardH + gap) - _scrollY;
            _cardRects.Add(new Rectangle((int)x, (int)y, (int)cardW, (int)cardH));
        }

        float dw = Math.Min(420f, vp.Width * 0.55f);
        float dh = Math.Min(360f, vp.Height * 0.75f);
        var detailPanel = new Rectangle((int)(vp.Width * 0.5f - dw * 0.5f), (int)(vp.Height * 0.5f - dh * 0.5f), (int)dw, (int)dh);
        _btnCloseDetail = new Rectangle(detailPanel.Right - 48, detailPanel.Y + 12, 36, 36);
        _btnFavorite = new Rectangle(detailPanel.X + 24, detailPanel.Bottom - 64, 120, 44);
        _btnEnter = new Rectangle(detailPanel.Right - 160, detailPanel.Bottom - 64, 136, 44);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();

        if (!input.IsPointerReleased) return;
        var p = input.PointerPosition;

        if (_showDetail)
        {
            if (_btnCloseDetail.Contains(p))
            {
                _showDetail = false;
                return;
            }
            if (_selected >= 0 && _selected < _worlds.Count)
            {
                if (_btnFavorite.Contains(p))
                {
                    _worlds[_selected].IsFavorite = !_worlds[_selected].IsFavorite;
                    return;
                }
                if (_btnEnter.Contains(p))
                {
                    Game.Profile.SelectedWorld = _worlds[_selected];
                    Game.Profile.LastWorldId = _worlds[_selected].Id;
                    Game.Profile.LastWorldName = _worlds[_selected].Name;
                    Game.TransitionTo(ApplicationState.WorldConnecting);
                    return;
                }
            }
            return;
        }

        if (_btnBack.Contains(p))
        {
            Game.TransitionTo(ApplicationState.Hub);
            return;
        }

        for (int i = 0; i < _tabRects.Count; i++)
        {
            if (_tabRects[i].Contains(p))
            {
                _tabIndex = i;
                return;
            }
        }

        for (int i = 0; i < _cardRects.Count; i++)
        {
            if (_cardRects[i].Contains(p) && _cardRects[i].Y > 80)
            {
                _selected = i;
                _showDetail = true;
                return;
            }
        }
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;

        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);

        // Header
        Game.DrawRect(sb, 0, 0, w, 56, DesignTokens.Color.PanelBase);
        DrawBtn(sb, _btnBack, "BACK", DesignTokens.Color.PanelElevated, 1.4f);
        string title = "DISCOVER";
        var ts = Game.MeasureText(title, 2.2f);
        Game.DrawText(sb, title, new Vector2(w * 0.5f - ts.X * 0.5f, 16), DesignTokens.Color.TextPrimary, 2.2f);

        // Tabs
        for (int i = 0; i < _tabs.Length && i < _tabRects.Count; i++)
        {
            var fill = i == _tabIndex ? DesignTokens.Color.AccentPrimary : DesignTokens.Color.PanelElevated;
            DrawBtn(sb, _tabRects[i], _tabs[i], fill, 1.1f);
        }

        // Cards
        for (int i = 0; i < _worlds.Count && i < _cardRects.Count; i++)
        {
            var r = _cardRects[i];
            if (r.Bottom < 70 || r.Y > h) continue;
            DrawWorldCard(sb, r, _worlds[i], i == _selected);
        }

        if (_showDetail && _selected >= 0 && _selected < _worlds.Count)
            DrawDetail(sb, _worlds[_selected]);
    }

    private void DrawWorldCard(SpriteBatch sb, Rectangle r, WorldInfo world, bool selected)
    {
        Game.DrawRect(sb, r, DesignTokens.Color.PanelElevated);
        Game.DrawBorder(sb, r, selected ? DesignTokens.Color.AccentPrimary : DesignTokens.Color.BorderSubtle, selected ? 3 : 1);

        // Preview strip
        var preview = new Rectangle(r.X + 8, r.Y + 8, 72, r.Height - 16);
        Color catColor = world.Category switch
        {
            "SOCIAL" => DesignTokens.Color.AccentSecondary,
            "CREATIVE" => DesignTokens.Color.AccentSuccess,
            "COMPETITIVE" => DesignTokens.Color.AccentDanger,
            _ => DesignTokens.Color.AccentPrimary
        };
        Game.DrawRect(sb, preview, catColor * 0.55f);
        Game.DrawRect(sb, preview.X + 16, preview.Y + 20, 40, 40, catColor);

        float tx = r.X + 92;
        Game.DrawText(sb, world.Name, new Vector2(tx, r.Y + 12), DesignTokens.Color.TextPrimary, 1.5f);
        Game.DrawText(sb, world.Creator + "  ·  " + world.PlayerCount + " PLAYERS",
            new Vector2(tx, r.Y + 36), DesignTokens.Color.TextMuted, 1.15f);
        Game.DrawText(sb, world.Category, new Vector2(tx, r.Y + 58), catColor, 1.2f);
        if (world.IsFavorite)
            Game.DrawText(sb, "*", new Vector2(r.Right - 28, r.Y + 12), DesignTokens.Color.AccentTertiary, 2f);
    }

    private void DrawDetail(SpriteBatch sb, WorldInfo world)
    {
        var vp = Game.GraphicsDevice.Viewport;
        Game.DrawRect(sb, 0, 0, vp.Width, vp.Height, DesignTokens.Color.OverlayDim);

        float dw = Math.Min(420f, vp.Width * 0.55f);
        float dh = Math.Min(360f, vp.Height * 0.75f);
        var panel = new Rectangle((int)(vp.Width * 0.5f - dw * 0.5f), (int)(vp.Height * 0.5f - dh * 0.5f), (int)dw, (int)dh);
        Game.DrawRect(sb, panel, DesignTokens.Color.PanelElevated);
        Game.DrawBorder(sb, panel, DesignTokens.Color.AccentPrimary, 2);

        // Preview block
        Game.DrawRect(sb, panel.X + 20, panel.Y + 20, panel.Width - 40, 100, DesignTokens.Color.ShadowIndigo);
        Game.DrawRect(sb, panel.X + panel.Width * 0.5f - 30, panel.Y + 45, 60, 50, DesignTokens.Color.AccentPrimary);

        Game.DrawText(sb, world.Name, new Vector2(panel.X + 24, panel.Y + 132), DesignTokens.Color.TextPrimary, 2f);
        Game.DrawText(sb, "BY " + world.Creator, new Vector2(panel.X + 24, panel.Y + 162), DesignTokens.Color.TextSecondary, 1.3f);
        Game.DrawText(sb, world.PlayerCount + " PLAYERS  ·  " + world.Category,
            new Vector2(panel.X + 24, panel.Y + 186), DesignTokens.Color.TextMuted, 1.2f);

        string desc = world.Description;
        if (desc.Length > 48) desc = desc[..48] + "...";
        Game.DrawText(sb, desc, new Vector2(panel.X + 24, panel.Y + 220), DesignTokens.Color.TextSecondary, 1.2f);

        string q = "SIGNAL " + ((int)(world.ConnectionQuality * 100)) + "%";
        Game.DrawText(sb, q, new Vector2(panel.X + 24, panel.Y + 250), DesignTokens.Color.AccentSuccess, 1.2f);

        DrawBtn(sb, _btnCloseDetail, "X", DesignTokens.Color.PanelBase, 1.6f);
        DrawBtn(sb, _btnFavorite, world.IsFavorite ? "FAVED" : "FAVORITE", DesignTokens.Color.PanelBase, 1.3f);
        DrawBtn(sb, _btnEnter, "ENTER", DesignTokens.Color.AccentPrimary, 1.8f);
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
