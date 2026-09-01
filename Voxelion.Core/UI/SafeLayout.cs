using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.UI;

/// <summary>
/// Compatibility façade over Layout system.
/// Prefer Layout.Update + LayoutBox for new code.
/// </summary>
public static class SafeLayout
{
    public static float MaxContentWidth => DesignTokens.Layout.MaxContentWidth;
    public static float TouchMin => DesignTokens.Layout.MinTouchTarget;

    public static float Margin(Viewport vp)
    {
        Layout.Update(vp);
        return Layout.Ctx.SafeInsets.Left;
    }

    public static float SafeLeft(Viewport vp) { Layout.Update(vp); return Layout.Ctx.Safe.Left; }
    public static float SafeRight(Viewport vp) { Layout.Update(vp); return Layout.Ctx.Safe.Right; }
    public static float SafeTop(Viewport vp) { Layout.Update(vp); return Layout.Ctx.Safe.Top; }
    public static float SafeBottom(Viewport vp) { Layout.Update(vp); return Layout.Ctx.Safe.Bottom; }
    public static float SafeWidth(Viewport vp) { Layout.Update(vp); return Layout.Ctx.Safe.Width; }
    public static float SafeHeight(Viewport vp) { Layout.Update(vp); return Layout.Ctx.Safe.Height; }

    public static Rectangle ContentBand(Viewport vp, float heightFraction = 1f)
    {
        Layout.Update(vp);
        var col = Layout.ContentColumn();
        int h = (int)(Layout.Ctx.Safe.Height * heightFraction);
        return new Rectangle(col.X, Layout.Ctx.Safe.Y, col.Width, Math.Max(1, h));
    }

    public static Rectangle CenterRect(Viewport vp, float width, float height)
    {
        Layout.Update(vp);
        return Layout.Centered(width, height);
    }

    public static int Columns(Viewport vp)
    {
        Layout.Update(vp);
        return Layout.Ctx.Columns();
    }
}
