using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.Scenes;

/// <summary>Inventory UI shell — slots, selection, empty/rarity states. No item economy logic.</summary>
public sealed class SceneInventory : SceneBase
{
    private Rectangle _btnBack;
    private int _selected = -1;
    private readonly string[] _tabs = { "ALL", "GEAR", "CONSUME", "QUEST" };
    private int _tab;
    private readonly (string Name, int Qty, int Rarity)[] _demo =
    {
        ("CRYSTAL SHARD", 12, 1), ("EMBER CORE", 3, 2), ("SILK THREAD", 40, 0),
        ("VOID KEY", 1, 3), ("", 0, 0), ("", 0, 0), ("MAP FRAGMENT", 2, 1), ("", 0, 0),
        ("LANTERN OIL", 8, 0), ("", 0, 0), ("RELIC DUST", 5, 2), ("", 0, 0)
    };

    public SceneInventory(VoxelionGame game) : base(game) { }

    public override void OnEnter()
    {
        base.OnEnter();
        _selected = -1;
        _tab = 0;
        Layout();
    }

    private void Layout()
    {
        var vp = Game.GraphicsDevice.Viewport;
        float m = SafeLayout.Margin(vp);
        _btnBack = new Rectangle((int)m, (int)m, 100, 40);
    }

    public override void Update(GameTime gameTime, InputState input)
    {
        base.Update(gameTime, input);
        Layout();
        if (!input.IsPointerReleased) return;
        var p = input.PointerPosition;
        if (_btnBack.Contains(p))
        {
            if (Game.StateMachine.CanGoBack) Game.GoBack();
            else Game.TransitionTo(ApplicationState.Hub);
            return;
        }
        var vp = Game.GraphicsDevice.Viewport;
        float m = SafeLayout.Margin(vp);
        for (int i = 0; i < _tabs.Length; i++)
        {
            var tr = new Rectangle((int)(m + 120 + i * 100), (int)m, 92, 40);
            if (tr.Contains(p)) { _tab = i; return; }
        }
        int cols = 4;
        float slot = 64f;
        float gridX = m + 20;
        float gridY = m + 70;
        for (int i = 0; i < _demo.Length; i++)
        {
            int c = i % cols, r = i / cols;
            var sr = new Rectangle((int)(gridX + c * (slot + 10)), (int)(gridY + r * (slot + 10)), (int)slot, (int)slot);
            if (sr.Contains(p)) { _selected = i; return; }
        }
    }

    public override void Draw(SpriteBatch sb, GameTime gameTime)
    {
        var vp = Game.GraphicsDevice.Viewport;
        float w = vp.Width, h = vp.Height;
        float m = SafeLayout.Margin(vp);
        Game.DrawRect(sb, 0, 0, w, h, DesignTokens.Color.VoidBlack);
        Game.DrawRect(sb, 0, 0, w, 56, DesignTokens.Color.PanelBase);
        DrawBtn(sb, _btnBack, "BACK", DesignTokens.Color.PanelElevated);
        UiKit.CenterLabel(Game, sb, "INVENTORY", 16, DesignTokens.Color.TextPrimary, 2.2f, w);

        for (int i = 0; i < _tabs.Length; i++)
        {
            var tr = new Rectangle((int)(m + 120 + i * 100), (int)m, 92, 40);
            DrawBtn(sb, tr, _tabs[i], i == _tab ? DesignTokens.Color.AccentPrimary : DesignTokens.Color.PanelElevated);
        }

        int cols = 4;
        float slot = 64f;
        float gridX = m + 20;
        float gridY = m + 70;
        Color[] rarity = {
            DesignTokens.Color.RarityCommon, DesignTokens.Color.RarityUncommon,
            DesignTokens.Color.RarityRare, DesignTokens.Color.RarityEpic
        };
        for (int i = 0; i < _demo.Length; i++)
        {
            int c = i % cols, r = i / cols;
            var sr = new Rectangle((int)(gridX + c * (slot + 10)), (int)(gridY + r * (slot + 10)), (int)slot, (int)slot);
            bool empty = string.IsNullOrEmpty(_demo[i].Name);
            Game.DrawRect(sb, sr, DesignTokens.Color.PanelElevated);
            Game.DrawBorder(sb, sr, i == _selected ? DesignTokens.Color.AccentPrimary : DesignTokens.Color.BorderSubtle, i == _selected ? 3 : 1);
            if (!empty)
            {
                Game.DrawRect(sb, sr.X + 12, sr.Y + 12, 40, 40, rarity[Math.Clamp(_demo[i].Rarity, 0, 3)] * 0.7f);
                if (_demo[i].Qty > 1)
                    Game.DrawText(sb, _demo[i].Qty.ToString(), new Vector2(sr.X + 4, sr.Bottom - 18), DesignTokens.Color.TextPrimary, 1.2f);
            }
        }

        // Details panel
        float dx = w * 0.55f;
        var detail = new Rectangle((int)dx, (int)(m + 70), (int)(w - dx - m), (int)(h - m * 2 - 80));
        UiKit.Panel(Game, sb, detail, DesignTokens.Color.PanelElevated, DesignTokens.Color.BorderSubtle, 1);
        if (_selected >= 0 && _selected < _demo.Length && !string.IsNullOrEmpty(_demo[_selected].Name))
        {
            var item = _demo[_selected];
            Game.DrawText(sb, item.Name, new Vector2(detail.X + 16, detail.Y + 20), DesignTokens.Color.TextPrimary, 1.8f);
            Game.DrawText(sb, "QTY " + item.Qty, new Vector2(detail.X + 16, detail.Y + 50), DesignTokens.Color.TextSecondary, 1.4f);
            Game.DrawText(sb, "UI PROTOTYPE SLOT", new Vector2(detail.X + 16, detail.Y + 80), DesignTokens.Color.TextMuted, 1.2f);
        }
        else
            Game.DrawText(sb, "SELECT A SLOT", new Vector2(detail.X + 16, detail.Y + 20), DesignTokens.Color.TextMuted, 1.5f);
    }

    private void DrawBtn(SpriteBatch sb, Rectangle r, string label, Color fill)
    {
        Game.DrawRect(sb, r, fill);
        Game.DrawBorder(sb, r, DesignTokens.Color.BorderFocus, 1);
        var size = Game.MeasureText(label, 1.3f);
        Game.DrawText(sb, label, new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + (r.Height - size.Y) * 0.5f),
            DesignTokens.Color.TextPrimary, 1.3f);
    }
}
