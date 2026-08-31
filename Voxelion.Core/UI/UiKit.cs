using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.Input;
using Voxelion.Core.UI.Theme;

namespace Voxelion.Core.UI;

/// <summary>Shared draw/hit helpers for panels, buttons, bars — used by all scenes.</summary>
public static class UiKit
{
    public static void Panel(VoxelionGame g, SpriteBatch sb, Rectangle r, Color? fill = null, Color? border = null, int borderW = 2)
    {
        g.DrawRect(sb, r, fill ?? DesignTokens.Color.PanelElevated);
        if (borderW > 0)
            g.DrawBorder(sb, r, border ?? DesignTokens.Color.BorderSubtle, borderW);
    }

    public static void Dim(VoxelionGame g, SpriteBatch sb, Viewport vp, float alpha = 0.65f) =>
        g.DrawRect(sb, 0, 0, vp.Width, vp.Height, DesignTokens.Color.OverlayDim * alpha);

    public static bool Button(
        VoxelionGame g, SpriteBatch sb, InputState input, Rectangle r, string label,
        Color fill, float textScale = 1.6f, bool draw = true)
    {
        bool hover = r.Contains(input.PointerPosition);
        bool down = hover && input.IsPointerDown;
        if (draw)
        {
            Color bg = down ? fill * 0.75f : hover ? fill * 0.9f : fill;
            g.DrawRect(sb, r, bg);
            g.DrawBorder(sb, r, DesignTokens.Color.BorderFocus, 2);
            var size = g.MeasureText(label, textScale);
            g.DrawText(sb, label,
                new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + (r.Height - size.Y) * 0.5f),
                DesignTokens.Color.TextPrimary, textScale);
        }
        return hover && input.IsPointerReleased;
    }

    public static void ProgressBar(VoxelionGame g, SpriteBatch sb, Rectangle track, float progress, Color fill)
    {
        g.DrawRect(sb, track, DesignTokens.Color.ShadowIndigo);
        float w = track.Width * MathHelper.Clamp(progress, 0, 1);
        if (w >= 1)
            g.DrawRect(sb, track.X, track.Y, w, track.Height, fill);
        g.DrawBorder(sb, track, DesignTokens.Color.BorderSubtle, 1);
    }

    public static void Label(VoxelionGame g, SpriteBatch sb, string text, Vector2 pos, Color color, float scale = 1.5f) =>
        g.DrawText(sb, text, pos, color, scale);

    public static void CenterLabel(VoxelionGame g, SpriteBatch sb, string text, float y, Color color, float scale, float screenW)
    {
        var size = g.MeasureText(text, scale);
        g.DrawText(sb, text, new Vector2(screenW * 0.5f - size.X * 0.5f, y), color, scale);
    }
}
