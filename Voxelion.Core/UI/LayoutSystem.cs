using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.UI;

[Flags]
public enum Anchor
{
    None = 0,
    Left = 1,
    Right = 2,
    Top = 4,
    Bottom = 8,
    CenterX = 16,
    CenterY = 32,
    Center = CenterX | CenterY,
    HStretch = Left | Right,
    VStretch = Top | Bottom,
    Stretch = HStretch | VStretch,
    TopLeft = Top | Left,
    TopRight = Top | Right,
    BottomLeft = Bottom | Left,
    BottomRight = Bottom | Right,
    TopStretch = Top | HStretch,
    BottomStretch = Bottom | HStretch,
    LeftStretch = Left | VStretch,
    RightStretch = Right | VStretch
}

public enum Dock
{
    None,
    Left,
    Right,
    Top,
    Bottom,
    Fill
}

public enum LayoutBreakpoint
{
    /// <summary>Phone landscape ~16:9–20:9, width &lt; 900</summary>
    Compact = 0,
    /// <summary>Large phone / small tablet, 900–1279</summary>
    Regular = 1,
    /// <summary>Tablet / desktop 1280–1919</summary>
    Expanded = 2,
    /// <summary>Ultrawide / 1080p+ desktop ≥ 1920</summary>
    Ultrawide = 3
}

/// <summary>
/// Single layout context per frame — safe area, aspect, breakpoints.
/// All scene positioning should go through this instead of raw viewport math.
/// </summary>
public sealed class LayoutContext
{
    public Viewport Viewport { get; private set; }
    public Rectangle Screen { get; private set; }
    public Rectangle Safe { get; private set; }
    public Thickness SafeInsets { get; private set; }
    public float Aspect { get; private set; }
    public LayoutBreakpoint Breakpoint { get; private set; }
    public float Scale { get; private set; }

    public void Update(Viewport vp, Thickness? platformInsets = null)
    {
        Viewport = vp;
        Screen = new Rectangle(0, 0, vp.Width, vp.Height);
        Aspect = vp.Height > 0 ? (float)vp.Width / vp.Height : 1.777f;

        // Base safe margin from design tokens + aspect bias for tall phones (20:9 etc.)
        float baseM = DesignTokens.Layout.SafeAreaMin;
        float sideBias = 0f;
        if (Aspect >= 2.1f) sideBias = baseM * 0.5f;      // ~19.5:9+
        else if (Aspect >= 2.0f) sideBias = baseM * 0.25f; // ~18:9

        var insets = platformInsets ?? new Thickness(
            left: baseM + sideBias,
            top: baseM,
            right: baseM + sideBias,
            bottom: baseM);

        // Extra bottom for gesture bars on very short height
        if (vp.Height < 400)
            insets = new Thickness(insets.Left, insets.Top, insets.Right, insets.Bottom + 8);

        SafeInsets = insets;
        Safe = new Rectangle(
            (int)insets.Left,
            (int)insets.Top,
            Math.Max(1, vp.Width - (int)(insets.Left + insets.Right)),
            Math.Max(1, vp.Height - (int)(insets.Top + insets.Bottom)));

        Breakpoint = vp.Width switch
        {
            >= 1920 => LayoutBreakpoint.Ultrawide,
            >= 1280 => LayoutBreakpoint.Expanded,
            >= 900 => LayoutBreakpoint.Regular,
            _ => LayoutBreakpoint.Compact
        };

        // Reference design width 1280 — scale for typography/touch only, not for anchors
        Scale = MathHelper.Clamp(vp.Width / 1280f, 0.75f, 1.35f);
    }

    public float Touch(float desired) =>
        Math.Max(DesignTokens.Layout.MinTouchTarget, desired * Math.Min(1.1f, Scale));

    public int Columns(int compact = 1, int regular = 2, int expanded = 3, int ultra = 4) =>
        Breakpoint switch
        {
            LayoutBreakpoint.Ultrawide => ultra,
            LayoutBreakpoint.Expanded => expanded,
            LayoutBreakpoint.Regular => regular,
            _ => compact
        };
}

public readonly struct Thickness
{
    public float Left { get; }
    public float Top { get; }
    public float Right { get; }
    public float Bottom { get; }

    public Thickness(float all) : this(all, all, all, all) { }
    public Thickness(float horizontal, float vertical) : this(horizontal, vertical, horizontal, vertical) { }
    public Thickness(float left, float top, float right, float bottom)
    {
        Left = left; Top = top; Right = right; Bottom = bottom;
    }

    public static Thickness Zero => new(0);
    public static Thickness FromToken(float s) => new(s);
}

/// <summary>
/// Declarative box → computed rectangle. No per-resolution coordinate tables.
/// </summary>
public struct LayoutBox
{
    public Anchor Anchor;
    public Dock Dock;
    public Thickness Margin;
    public Thickness Padding;
    public Vector2 PreferredSize;   // 0 = auto / stretch-driven
    public Vector2 MinSize;
    public Vector2 MaxSize;         // 0 = unlimited
    public Vector2 Pivot;           // 0..1, used with Center anchors; default 0.5,0.5
    public Vector2 RelativeOffset;  // fraction of parent (e.g. 0.5,0.5 = center bias)
    public float RelativeX;         // optional 0..1 position of pivot within parent
    public float RelativeY;
    public bool UseRelativePosition;

    public static LayoutBox Default => new()
    {
        Anchor = Anchor.TopLeft,
        Dock = Dock.None,
        Margin = Thickness.Zero,
        Padding = Thickness.Zero,
        PreferredSize = Vector2.Zero,
        MinSize = Vector2.Zero,
        MaxSize = Vector2.Zero,
        Pivot = new Vector2(0.5f, 0.5f)
    };

    public LayoutBox WithAnchor(Anchor a) { Anchor = a; return this; }
    public LayoutBox WithDock(Dock d) { Dock = d; return this; }
    public LayoutBox WithMargin(Thickness m) { Margin = m; return this; }
    public LayoutBox WithMargin(float all) { Margin = new Thickness(all); return this; }
    public LayoutBox WithPadding(Thickness p) { Padding = p; return this; }
    public LayoutBox WithSize(float w, float h) { PreferredSize = new Vector2(w, h); return this; }
    public LayoutBox WithMin(float w, float h) { MinSize = new Vector2(w, h); return this; }
    public LayoutBox WithMax(float w, float h) { MaxSize = new Vector2(w, h); return this; }
    public LayoutBox WithRelative(float x, float y)
    {
        UseRelativePosition = true;
        RelativeX = x;
        RelativeY = y;
        return this;
    }

    /// <summary>Compute outer rectangle inside parent.</summary>
    public Rectangle Compute(Rectangle parent)
    {
        float x = parent.X, y = parent.Y, w = PreferredSize.X, h = PreferredSize.Y;
        float availW = parent.Width - Margin.Left - Margin.Right;
        float availH = parent.Height - Margin.Top - Margin.Bottom;

        // Dock overrides anchor for primary axis
        if (Dock != Dock.None)
        {
            switch (Dock)
            {
                case Dock.Fill:
                    return ApplyMinMax(new Rectangle(
                        (int)(parent.X + Margin.Left),
                        (int)(parent.Y + Margin.Top),
                        (int)Math.Max(1, availW),
                        (int)Math.Max(1, availH)));
                case Dock.Top:
                    h = h > 0 ? h : MinSize.Y > 0 ? MinSize.Y : DesignTokens.Layout.TopBarHeight;
                    return ApplyMinMax(new Rectangle(
                        (int)(parent.X + Margin.Left),
                        (int)(parent.Y + Margin.Top),
                        (int)Math.Max(1, availW),
                        (int)h));
                case Dock.Bottom:
                    h = h > 0 ? h : MinSize.Y > 0 ? MinSize.Y : DesignTokens.Layout.BottomNavHeight;
                    return ApplyMinMax(new Rectangle(
                        (int)(parent.X + Margin.Left),
                        (int)(parent.Bottom - Margin.Bottom - h),
                        (int)Math.Max(1, availW),
                        (int)h));
                case Dock.Left:
                    w = w > 0 ? w : MinSize.X > 0 ? MinSize.X : 120;
                    return ApplyMinMax(new Rectangle(
                        (int)(parent.X + Margin.Left),
                        (int)(parent.Y + Margin.Top),
                        (int)w,
                        (int)Math.Max(1, availH)));
                case Dock.Right:
                    w = w > 0 ? w : MinSize.X > 0 ? MinSize.X : 120;
                    return ApplyMinMax(new Rectangle(
                        (int)(parent.Right - Margin.Right - w),
                        (int)(parent.Y + Margin.Top),
                        (int)w,
                        (int)Math.Max(1, availH)));
            }
        }

        // Stretch axes
        if ((Anchor & Anchor.HStretch) == Anchor.HStretch)
            w = availW;
        if ((Anchor & Anchor.VStretch) == Anchor.VStretch)
            h = availH;

        if (w <= 0) w = MinSize.X > 0 ? MinSize.X : 100;
        if (h <= 0) h = MinSize.Y > 0 ? MinSize.Y : 40;

        // Horizontal position
        if (UseRelativePosition)
        {
            x = parent.X + parent.Width * RelativeX - w * Pivot.X;
            y = parent.Y + parent.Height * RelativeY - h * Pivot.Y;
        }
        else
        {
            bool left = (Anchor & Anchor.Left) != 0;
            bool right = (Anchor & Anchor.Right) != 0;
            bool cx = (Anchor & Anchor.CenterX) != 0;
            bool top = (Anchor & Anchor.Top) != 0;
            bool bottom = (Anchor & Anchor.Bottom) != 0;
            bool cy = (Anchor & Anchor.CenterY) != 0;

            if (left && right)
            {
                x = parent.X + Margin.Left;
                w = availW;
            }
            else if (cx || (!left && !right))
                x = parent.X + (parent.Width - w) * 0.5f + (Margin.Left - Margin.Right) * 0.5f;
            else if (right)
                x = parent.Right - Margin.Right - w;
            else
                x = parent.X + Margin.Left;

            if (top && bottom)
            {
                y = parent.Y + Margin.Top;
                h = availH;
            }
            else if (cy || (!top && !bottom))
                y = parent.Y + (parent.Height - h) * 0.5f + (Margin.Top - Margin.Bottom) * 0.5f;
            else if (bottom)
                y = parent.Bottom - Margin.Bottom - h;
            else
                y = parent.Y + Margin.Top;
        }

        return ApplyMinMax(new Rectangle((int)x, (int)y, (int)Math.Max(1, w), (int)Math.Max(1, h)));
    }

    public Rectangle ContentRect(Rectangle outer) =>
        new(
            (int)(outer.X + Padding.Left),
            (int)(outer.Y + Padding.Top),
            (int)Math.Max(1, outer.Width - Padding.Left - Padding.Right),
            (int)Math.Max(1, outer.Height - Padding.Top - Padding.Bottom));

    private Rectangle ApplyMinMax(Rectangle r)
    {
        float w = r.Width, h = r.Height;
        if (MinSize.X > 0) w = Math.Max(w, MinSize.X);
        if (MinSize.Y > 0) h = Math.Max(h, MinSize.Y);
        if (MaxSize.X > 0) w = Math.Min(w, MaxSize.X);
        if (MaxSize.Y > 0) h = Math.Min(h, MaxSize.Y);
        return new Rectangle(r.X, r.Y, (int)w, (int)h);
    }
}

/// <summary>Static helpers for common landscape patterns.</summary>
public static class Layout
{
    /// <summary>Shared context — call Update once per frame from game/scene.</summary>
    public static LayoutContext Ctx { get; } = new();

    public static void Update(Viewport vp, Thickness? platformInsets = null) =>
        Ctx.Update(vp, platformInsets);

    public static Rectangle Safe => Ctx.Safe;
    public static Rectangle Screen => Ctx.Screen;

    public static Rectangle Box(LayoutBox box, Rectangle? parent = null) =>
        box.Compute(parent ?? Ctx.Safe);

    public static Rectangle TopBar(float height = -1) =>
        Box(LayoutBox.Default
            .WithDock(Dock.Top)
            .WithSize(0, height > 0 ? height : DesignTokens.Layout.TopBarHeight)
            .WithMargin(Thickness.Zero));

    public static Rectangle BottomBar(float height = -1) =>
        Box(LayoutBox.Default
            .WithDock(Dock.Bottom)
            .WithSize(0, height > 0 ? height : DesignTokens.Layout.BottomNavHeight)
            .WithMargin(new Thickness(0, 0, 0, 0)));

    public static Rectangle Centered(float width, float height, float maxWidth = -1)
    {
        float w = width;
        if (maxWidth < 0) maxWidth = DesignTokens.Layout.MaxContentWidth;
        w = Math.Min(w, Math.Min(maxWidth, Ctx.Safe.Width * 0.92f));
        return Box(LayoutBox.Default
            .WithAnchor(Anchor.Center)
            .WithSize(w, height)
            .WithMax(maxWidth, 0));
    }

    public static Rectangle ContentColumn(float maxWidth = -1)
    {
        if (maxWidth < 0) maxWidth = DesignTokens.Layout.MaxContentWidth;
        float w = Math.Min(maxWidth, Ctx.Safe.Width);
        float x = Ctx.Safe.X + (Ctx.Safe.Width - w) * 0.5f;
        return new Rectangle((int)x, Ctx.Safe.Y, (int)w, Ctx.Safe.Height);
    }

    /// <summary>Grid cell in a responsive column count.</summary>
    public static Rectangle GridCell(int index, int columns, float rowHeight, float gap, Rectangle? area = null)
    {
        var parent = area ?? Ctx.Safe;
        columns = Math.Max(1, columns);
        int col = index % columns;
        int row = index / columns;
        float cellW = (parent.Width - gap * (columns - 1)) / columns;
        float x = parent.X + col * (cellW + gap);
        float y = parent.Y + row * (rowHeight + gap);
        return new Rectangle((int)x, (int)y, (int)cellW, (int)rowHeight);
    }

    public static Rectangle ClampToSafe(Rectangle r)
    {
        var s = Ctx.Safe;
        int x = MathHelper.Clamp(r.X, s.X, s.Right - r.Width);
        int y = MathHelper.Clamp(r.Y, s.Y, s.Bottom - r.Height);
        int w = Math.Min(r.Width, s.Width);
        int h = Math.Min(r.Height, s.Height);
        if (x + w > s.Right) x = s.Right - w;
        if (y + h > s.Bottom) y = s.Bottom - h;
        return new Rectangle(Math.Max(s.X, x), Math.Max(s.Y, y), Math.Max(1, w), Math.Max(1, h));
    }
}
