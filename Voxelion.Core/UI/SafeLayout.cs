using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.UI;

/// <summary>Responsive landscape layout — driven by DesignTokens.Layout.</summary>
public static class SafeLayout
{
    public static float MaxContentWidth => DesignTokens.Layout.MaxContentWidth;
    public static float TouchMin => DesignTokens.Layout.MinTouchTarget;

    public static float Margin(Viewport vp) =>
        Math.Max(DesignTokens.Layout.SafeAreaMin * 0.5f,
            Math.Min(vp.Width, vp.Height) * 0.02f);

    public static float SafeLeft(Viewport vp) => Margin(vp);
    public static float SafeRight(Viewport vp) => vp.Width - Margin(vp);
    public static float SafeTop(Viewport vp) => Margin(vp);
    public static float SafeBottom(Viewport vp) => vp.Height - Margin(vp);
    public static float SafeWidth(Viewport vp) => SafeRight(vp) - SafeLeft(vp);
    public static float SafeHeight(Viewport vp) => SafeBottom(vp) - SafeTop(vp);

    public static Rectangle ContentBand(Viewport vp, float heightFraction = 1f)
    {
        float m = Margin(vp);
        float maxW = Math.Min(MaxContentWidth, vp.Width - m * 2);
        float x = (vp.Width - maxW) * 0.5f;
        float h = vp.Height * heightFraction - m * 2;
        return new Rectangle((int)x, (int)m, (int)maxW, (int)Math.Max(1, h));
    }

    public static Rectangle CenterRect(Viewport vp, float width, float height) =>
        new(
            (int)(vp.Width * 0.5f - width * 0.5f),
            (int)(vp.Height * 0.5f - height * 0.5f),
            (int)width,
            (int)height);

    public static int Columns(Viewport vp) =>
        vp.Width >= 1100 ? 3 : vp.Width >= 700 ? 2 : 1;
}
