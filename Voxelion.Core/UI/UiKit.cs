using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.UI;

/// <summary>Shared UI helpers — routes through VisualChrome for premium look.</summary>
public static class UiKit
{
    public static void Panel(VoxelionGame g, SpriteBatch sb, Rectangle r, Color? fill = null, Color? border = null, int borderW = 2)
    {
        // Prefer chrome panel; fall back tint if custom fill requested
        if (fill == null && border == null)
        {
            VisualChrome.Panel(g, sb, r, elevated: true, glow: false);
            return;
        }
        g.DrawRect(sb, r, fill ?? DesignTokens.Semantic.SurfaceElevated);
        if (borderW > 0)
            g.DrawBorder(sb, r, border ?? DesignTokens.Semantic.Border, borderW);
    }

    public static void Dim(VoxelionGame g, SpriteBatch sb, Viewport vp, float alpha = 0.65f) =>
        g.DrawRect(sb, 0, 0, vp.Width, vp.Height, DesignTokens.Semantic.Overlay * alpha);

    public static bool Button(
        VoxelionGame g, SpriteBatch sb, InputState input, Rectangle r, string label,
        Color fill, float textScale = 1.6f, bool draw = true)
    {
        bool hover = r.Contains(input.PointerPosition);
        bool down = hover && input.IsPointerDown;
        if (draw)
        {
            bool primary = fill.R > 100 && fill.B > 100; // rough: violet-ish
            VisualChrome.Button(g, sb, r, label, primary, hover, down, textScale);
        }
        return hover && input.IsPointerReleased;
    }

    public static void ProgressBar(VoxelionGame g, SpriteBatch sb, Rectangle track, float progress, Color fill) =>
        VisualChrome.ProgressCrystal(g, sb, track, progress);

    public static void Label(VoxelionGame g, SpriteBatch sb, string text, Vector2 pos, Color color, float scale = 1.5f) =>
        g.DrawText(sb, text, pos, color, scale);

    public static void CenterLabel(VoxelionGame g, SpriteBatch sb, string text, float y, Color color, float scale, float screenW)
    {
        var size = g.MeasureText(text, scale);
        g.DrawText(sb, text, new Vector2(screenW * 0.5f - size.X * 0.5f, y), color, scale);
    }
}
