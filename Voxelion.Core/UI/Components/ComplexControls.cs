using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace Voxelion.Core.UI.Components;

public sealed class Tab : UIElement
{
    public string Label { get; set; } = "";

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        VisualChrome.Tab(game, sb, Bounds, Label, Is(UIState.Selected) || Is(UIState.Focused));
    }
}

public sealed class TabBar : UIElement
{
    public List<string> Tabs { get; } = new();
    public int SelectedIndex { get; set; }
    private readonly List<Rectangle> _tabRects = new();

    public override void Update(InputState input, bool focused)
    {
        base.Update(input, focused);
        Rebuild();
        if (!Enabled) return;
        if (input.IsPointerReleased)
        {
            for (int i = 0; i < _tabRects.Count; i++)
                if (_tabRects[i].Contains(input.PointerPosition))
                    SelectedIndex = i;
        }
        if (focused)
        {
            if (input.NavLeft) SelectedIndex = Math.Max(0, SelectedIndex - 1);
            if (input.NavRight) SelectedIndex = Math.Min(Tabs.Count - 1, SelectedIndex + 1);
        }
    }

    private void Rebuild()
    {
        _tabRects.Clear();
        if (Tabs.Count == 0) return;
        int w = Bounds.Width / Tabs.Count;
        for (int i = 0; i < Tabs.Count; i++)
            _tabRects.Add(new Rectangle(Bounds.X + i * w, Bounds.Y, w, Bounds.Height));
    }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        Rebuild();
        for (int i = 0; i < Tabs.Count; i++)
            VisualChrome.Tab(game, sb, _tabRects[i], Tabs[i], i == SelectedIndex);
    }
}

public sealed class Dropdown : UIElement
{
    public List<string> Options { get; } = new();
    public int SelectedIndex { get; set; }
    public bool Open { get; set; }

    public string Current => Options.Count > 0 && SelectedIndex >= 0 && SelectedIndex < Options.Count
        ? Options[SelectedIndex] : "";

    public override void Update(InputState input, bool focused)
    {
        base.Update(input, focused);
        if (!Enabled) return;
        if (WasActivated(input, focused)) Open = !Open;
        if (Open && input.IsPointerReleased)
        {
            int rowH = Bounds.Height;
            for (int i = 0; i < Options.Count; i++)
            {
                var row = new Rectangle(Bounds.X, Bounds.Bottom + i * rowH, Bounds.Width, rowH);
                if (row.Contains(input.PointerPosition))
                {
                    SelectedIndex = i;
                    Open = false;
                    break;
                }
            }
        }
        if (input.CancelPressed) Open = false;
    }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        game.DrawRect(sb, Bounds, DesignTokens.Semantic.SurfaceElevated);
        game.DrawBorder(sb, Bounds, Is(UIState.Focused) ? DesignTokens.Semantic.Focus : DesignTokens.Semantic.Border, 2);
        game.DrawText(sb, Current, new Vector2(Bounds.X + 10, Bounds.Y + 14),
            DesignTokens.Semantic.TextPrimary, DesignTokens.Typography.Body);
        game.DrawText(sb, "v", new Vector2(Bounds.Right - 24, Bounds.Y + 14),
            DesignTokens.Semantic.TextMuted, DesignTokens.Typography.Body);
        if (Open)
        {
            for (int i = 0; i < Options.Count; i++)
            {
                var row = new Rectangle(Bounds.X, Bounds.Bottom + i * Bounds.Height, Bounds.Width, Bounds.Height);
                game.DrawRect(sb, row, i == SelectedIndex ? DesignTokens.Semantic.Primary * 0.35f : DesignTokens.Semantic.Surface);
                game.DrawBorder(sb, row, DesignTokens.Semantic.Border, 1);
                game.DrawText(sb, Options[i], new Vector2(row.X + 10, row.Y + 12),
                    DesignTokens.Semantic.TextPrimary, DesignTokens.Typography.Body);
            }
        }
    }
}

public sealed class ScrollView : UIElement
{
    public float ContentHeight { get; set; }
    public float ScrollY { get; set; }
    public Rectangle Viewport => Bounds;

    public override void Update(InputState input, bool focused)
    {
        base.Update(input, focused);
        float max = Math.Max(0, ContentHeight - Bounds.Height);
        if (Bounds.Contains(input.PointerPosition))
        {
            if (input.ScrollDelta.Y != 0)
                ScrollY = MathHelper.Clamp(ScrollY - input.ScrollDelta.Y * 24f, 0, max);
            if (input.SwipeDelta.Y != 0 && input.IsPointerDown)
                ScrollY = MathHelper.Clamp(ScrollY - input.SwipeDelta.Y * 0.15f, 0, max);
        }
        if (focused)
        {
            if (input.NavUp) ScrollY = MathHelper.Clamp(ScrollY - 40, 0, max);
            if (input.NavDown) ScrollY = MathHelper.Clamp(ScrollY + 40, 0, max);
        }
    }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        game.DrawRect(sb, Bounds, DesignTokens.Semantic.Surface * 0.5f);
        game.DrawBorder(sb, Bounds, DesignTokens.Semantic.Border, 1);
        // scrollbar
        if (ContentHeight > Bounds.Height)
        {
            float ratio = Bounds.Height / ContentHeight;
            float thumbH = Math.Max(20, Bounds.Height * ratio);
            float thumbY = Bounds.Y + (Bounds.Height - thumbH) * (ScrollY / Math.Max(1, ContentHeight - Bounds.Height));
            game.DrawRect(sb, Bounds.Right - 6, thumbY, 4, thumbH, DesignTokens.Semantic.Primary * 0.6f);
        }
    }

    public Rectangle ItemRect(float localY, float height) =>
        new(Bounds.X, (int)(Bounds.Y + localY - ScrollY), Bounds.Width, (int)height);
}

public sealed class ListView : UIElement
{
    public List<string> Items { get; } = new();
    public int SelectedIndex { get; set; } = -1;
    public float RowHeight { get; set; } = 48;
    private readonly ScrollView _scroll = new();

    public override void Update(InputState input, bool focused)
    {
        Bounds = Bounds;
        _scroll.Bounds = Bounds;
        _scroll.ContentHeight = Items.Count * RowHeight;
        _scroll.Update(input, focused);
        base.Update(input, focused);
        if (!Enabled) return;
        if (input.IsPointerReleased && Bounds.Contains(input.PointerPosition))
        {
            float ly = input.PointerPosition.Y - Bounds.Y + _scroll.ScrollY;
            int idx = (int)(ly / RowHeight);
            if (idx >= 0 && idx < Items.Count) SelectedIndex = idx;
        }
        if (focused)
        {
            if (input.NavUp) SelectedIndex = Math.Max(0, SelectedIndex - 1);
            if (input.NavDown) SelectedIndex = Math.Min(Items.Count - 1, SelectedIndex + 1);
        }
    }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        _scroll.Bounds = Bounds;
        _scroll.ContentHeight = Items.Count * RowHeight;
        _scroll.Draw(game, sb, time);
        for (int i = 0; i < Items.Count; i++)
        {
            var row = _scroll.ItemRect(i * RowHeight, RowHeight);
            if (row.Bottom < Bounds.Y || row.Y > Bounds.Bottom) continue;
            if (i == SelectedIndex)
                game.DrawRect(sb, row, DesignTokens.Semantic.Selection);
            game.DrawText(sb, Items[i], new Vector2(row.X + 12, row.Y + 14),
                DesignTokens.Semantic.TextPrimary, DesignTokens.Typography.Body);
        }
    }
}

public sealed class GridView : UIElement
{
    public int Columns { get; set; } = 3;
    public float CellHeight { get; set; } = 100;
    public float Gap { get; set; } = 12;
    public int ItemCount { get; set; }
    public int SelectedIndex { get; set; } = -1;
    public Action<VoxelionGame, SpriteBatch, int, Rectangle, bool>? DrawCell { get; set; }

    public Rectangle CellBounds(int index)
    {
        int col = index % Math.Max(1, Columns);
        int row = index / Math.Max(1, Columns);
        float cellW = (Bounds.Width - Gap * (Columns - 1)) / Columns;
        return new Rectangle(
            (int)(Bounds.X + col * (cellW + Gap)),
            (int)(Bounds.Y + row * (CellHeight + Gap)),
            (int)cellW,
            (int)CellHeight);
    }

    public override void Update(InputState input, bool focused)
    {
        base.Update(input, focused);
        if (!Enabled) return;
        if (input.IsPointerReleased)
        {
            for (int i = 0; i < ItemCount; i++)
                if (CellBounds(i).Contains(input.PointerPosition))
                    SelectedIndex = i;
        }
    }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        for (int i = 0; i < ItemCount; i++)
        {
            var r = CellBounds(i);
            bool sel = i == SelectedIndex;
            if (DrawCell != null) DrawCell(game, sb, i, r, sel);
            else
            {
                VisualChrome.Panel(game, sb, r, elevated: sel);
            }
        }
    }
}

public sealed class Tooltip : UIElement
{
    public string Text { get; set; } = "";
    public float ShowDelay { get; set; } = 0.35f;
    private float _hoverTime;

    public override void Update(InputState input, bool focused)
    {
        base.Update(input, focused);
        if (Is(UIState.Hover) || focused) _hoverTime += 1f / 60f;
        else _hoverTime = 0;
        Visible = _hoverTime >= ShowDelay && !string.IsNullOrEmpty(Text);
    }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        var sz = game.MeasureText(Text, DesignTokens.Typography.Caption);
        var r = new Rectangle(Bounds.X, Bounds.Y - (int)sz.Y - 16, (int)sz.X + 16, (int)sz.Y + 12);
        game.DrawRect(sb, r, DesignTokens.Semantic.SurfaceElevated);
        game.DrawBorder(sb, r, DesignTokens.Semantic.Border, 1);
        game.DrawText(sb, Text, new Vector2(r.X + 8, r.Y + 6), DesignTokens.Semantic.TextPrimary, DesignTokens.Typography.Caption);
    }
}

public class Modal : UIElement
{
    public string Title { get; set; } = "";
    public bool Open { get; set; }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible || !Open) return;
        var vp = game.GraphicsDevice.Viewport;
        game.DrawRect(sb, 0, 0, vp.Width, vp.Height, DesignTokens.Semantic.Scrim);
        VisualChrome.ModalFrame(game, sb, Bounds);
        if (!string.IsNullOrEmpty(Title))
            game.DrawText(sb, Title, new Vector2(Bounds.X + 20, Bounds.Y + 16),
                DesignTokens.Semantic.TextPrimary, DesignTokens.Typography.Heading);
    }
}

public class Dialog : Modal
{
    public string Message { get; set; } = "";
    public string ConfirmLabel { get; set; } = "OK";
    public string CancelLabel { get; set; } = "CANCEL";
    public Rectangle ConfirmBounds { get; set; }
    public Rectangle CancelBounds { get; set; }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible || !Open) return;
        base.Draw(game, sb, time);
        game.DrawText(sb, Message, new Vector2(Bounds.X + 20, Bounds.Y + 56),
            DesignTokens.Semantic.TextSecondary, DesignTokens.Typography.Body);
        ConfirmBounds = new Rectangle(Bounds.Right - 140, Bounds.Bottom - 56, 120, 40);
        CancelBounds = new Rectangle(Bounds.Right - 280, Bounds.Bottom - 56, 120, 40);
        VisualChrome.Button(game, sb, ConfirmBounds, ConfirmLabel, true, false, false, DesignTokens.Typography.Button);
        VisualChrome.Button(game, sb, CancelBounds, CancelLabel, false, false, false, DesignTokens.Typography.Button);
    }
}

public sealed class ConfirmDialog : Dialog
{
    public ConfirmDialog()
    {
        ConfirmLabel = "CONFIRM";
        CancelLabel = "CANCEL";
    }
}

public sealed class LoadingOverlay : UIElement
{
    public string Message { get; set; } = "LOADING";
    public float Progress { get; set; } = -1; // <0 = indeterminate

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        var vp = game.GraphicsDevice.Viewport;
        game.DrawRect(sb, 0, 0, vp.Width, vp.Height, DesignTokens.Semantic.OverlayHeavy);
        VisualChrome.Spinner(game, sb, new Vector2(vp.Width * 0.5f, vp.Height * 0.45f), 18, time);
        var sz = game.MeasureText(Message, DesignTokens.Typography.Body);
        game.DrawText(sb, Message, new Vector2(vp.Width * 0.5f - sz.X * 0.5f, vp.Height * 0.55f),
            DesignTokens.Semantic.TextPrimary, DesignTokens.Typography.Body);
        if (Progress >= 0)
        {
            var bar = new Rectangle(vp.Width / 2 - 120, (int)(vp.Height * 0.62f), 240, 12);
            VisualChrome.ProgressCrystal(game, sb, bar, Progress);
        }
    }
}

public sealed class StatusBadge : UIElement
{
    public string Text { get; set; } = "";
    public XnaColor Accent { get; set; } = DesignTokens.Semantic.Info;

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible) return;
        VisualChrome.Badge(game, sb, new Vector2(Bounds.X, Bounds.Y), Text, Accent);
    }
}

public sealed class ContextMenu : UIElement
{
    public List<string> Actions { get; } = new();
    public int SelectedIndex { get; set; } = -1;
    public bool Open { get; set; }
    public float RowHeight { get; set; } = 40;

    public override void Update(InputState input, bool focused)
    {
        base.Update(input, focused);
        if (!Open) return;
        if (input.IsPointerReleased)
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                var row = new Rectangle(Bounds.X, Bounds.Y + (int)(i * RowHeight), Bounds.Width, (int)RowHeight);
                if (row.Contains(input.PointerPosition)) SelectedIndex = i;
            }
        }
        if (input.CancelPressed) Open = false;
    }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible || !Open) return;
        var h = (int)(Actions.Count * RowHeight);
        var panel = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, h);
        VisualChrome.Panel(game, sb, panel, elevated: true, glow: true);
        for (int i = 0; i < Actions.Count; i++)
        {
            var row = new Rectangle(panel.X, panel.Y + (int)(i * RowHeight), panel.Width, (int)RowHeight);
            if (i == SelectedIndex) game.DrawRect(sb, row, DesignTokens.Semantic.Selection);
            game.DrawText(sb, Actions[i], new Vector2(row.X + 12, row.Y + 12),
                DesignTokens.Semantic.TextPrimary, DesignTokens.Typography.Body);
        }
    }
}

public sealed class ContextActionBar : UIElement
{
    public List<(string Id, string Label)> Actions { get; } = new();
    public string? PressedId { get; private set; }

    public override void Update(InputState input, bool focused)
    {
        base.Update(input, focused);
        PressedId = null;
        if (!Enabled || Actions.Count == 0) return;
        int w = Bounds.Width / Actions.Count;
        if (input.IsPointerReleased)
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                var r = new Rectangle(Bounds.X + i * w, Bounds.Y, w, Bounds.Height);
                if (r.Contains(input.PointerPosition)) PressedId = Actions[i].Id;
            }
        }
    }

    public override void Draw(VoxelionGame game, SpriteBatch sb, float time)
    {
        if (!Visible || Actions.Count == 0) return;
        VisualChrome.Panel(game, sb, Bounds, elevated: true);
        int w = Bounds.Width / Actions.Count;
        for (int i = 0; i < Actions.Count; i++)
        {
            var r = new Rectangle(Bounds.X + i * w, Bounds.Y, w, Bounds.Height);
            VisualChrome.Button(game, sb, r, Actions[i].Label, false, false, false, DesignTokens.Typography.Caption);
        }
    }
}
