using Microsoft.Xna.Framework;

namespace Voxelion.Core.UI;

/// <summary>Responsive landscape layout helpers — safe margins, columns, max content width.</summary>
public static class SafeLayout
{
    public const float MaxContentWidth = 1280f;
    public const float MinMargin = 12f;

    public static float Margin(Viewport vp) =>
        Math.Max(MinMargin, Math.Min(vp.Width, vp.Height) * 0.02f);

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

    public static Rectangle CenterRect(Viewport vp, float width, float height)
    {
        return new Rectangle(
            (int)(vp.Width * 0.5f - width * 0.5f),
            (int)(vp.Height * 0.5f - height * 0.5f),
            (int)width,
            (int)height);
    }

    public static int Columns(Viewport vp) =>
        vp.Width >= 1100 ? 3 : vp.Width >= 700 ? 2 : 1;

    public static float TouchMin => 48f;
}
