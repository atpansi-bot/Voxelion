using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Voxelion.Core.Core;
using Voxelion.Core.UI.Theme;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace Voxelion.Core.UI;

/// <summary>
/// Original VOXELION visual chrome — layered panels, frames, glow, badges,
/// loading rings, world-card frames. Drawn only with the 1×1 pixel texture.
/// Does not imitate third-party game UIs.
/// </summary>
public static class VisualChrome
{
    // ---- Surfaces ----------------------------------------------------------

    /// <summary>Layered translucent panel with outer glow rim and inner edge.</summary>
    public static void Panel(VoxelionGame g, SpriteBatch sb, Rectangle r, bool elevated = false, bool glow = false)
    {
        if (r.Width < 2 || r.Height < 2) return;

        if (glow)
        {
            var glowR = Inflate(r, 4);
            g.DrawRect(sb, glowR, DesignTokens.Glow.Primary * 0.35f);
        }

        // Drop shadow
        g.DrawRect(sb, r.X + 2, r.Y + 3, r.Width, r.Height, DesignTokens.Shadow.Soft);

        // Body
        var body = elevated ? DesignTokens.Semantic.SurfaceElevated : DesignTokens.Semantic.Surface;
        g.DrawRect(sb, r, body);

        // Inner highlight strip (top)
        g.DrawRect(sb, r.X + 1, r.Y + 1, r.Width - 2, 2, DesignTokens.Semantic.Primary * 0.25f);

        // Outer border
        g.DrawBorder(sb, r, DesignTokens.Semantic.Border, DesignTokens.Border.Default);

        // Corner accents (original motif — small L-brackets)
        DrawCornerBrackets(g, sb, r, DesignTokens.Semantic.Primary * 0.7f, 8);
    }

    /// <summary>Modal / dialog frame with stronger glow and double border.</summary>
    public static void ModalFrame(VoxelionGame g, SpriteBatch sb, Rectangle r)
    {
        var outer = Inflate(r, 6);
        g.DrawRect(sb, outer, DesignTokens.Glow.Primary * 0.4f);
        g.DrawRect(sb, r.X + 3, r.Y + 4, r.Width, r.Height, DesignTokens.Shadow.Medium);
        g.DrawRect(sb, r, DesignTokens.Semantic.SurfaceElevated);
        g.DrawBorder(sb, r, DesignTokens.Semantic.BorderStrong, DesignTokens.Border.Default);
        var inner = Deflate(r, 3);
        g.DrawBorder(sb, inner, DesignTokens.Semantic.Border * 0.6f, 1);
        DrawCornerBrackets(g, sb, r, DesignTokens.Semantic.Secondary * 0.8f, 12);
        // Top crystal bar
        g.DrawRect(sb, r.X + 12, r.Y + 4, r.Width - 24, 3, DesignTokens.Semantic.Primary * 0.55f);
    }

    // ---- Buttons -----------------------------------------------------------

    public static void Button(
        VoxelionGame g, SpriteBatch sb, Rectangle r, string label,
        bool primary, bool hover, bool pressed, float textScale = 1.7f)
    {
        if (r.Width < 2 || r.Height < 2) return;

        XnaColor fill = primary ? DesignTokens.Semantic.Primary : DesignTokens.Semantic.SurfaceElevated;
        if (pressed) fill *= 0.72f;
        else if (hover) fill = primary ? fill * 0.92f : DesignTokens.Semantic.Hover * 2f + fill * 0.85f;

        // Soft outer glow for primary
        if (primary && !pressed)
            g.DrawRect(sb, Inflate(r, 2), DesignTokens.Glow.Primary * 0.45f);

        g.DrawRect(sb, r.X + 1, r.Y + 2, r.Width, r.Height, DesignTokens.Shadow.Soft);
        g.DrawRect(sb, r, fill);

        // Top sheen
        g.DrawRect(sb, r.X + 2, r.Y + 2, r.Width - 4, Math.Max(1, r.Height / 5), XnaColor.White * 0.12f);

        // Border
        var border = primary ? DesignTokens.Semantic.BorderStrong : DesignTokens.Semantic.Border;
        g.DrawBorder(sb, r, hover ? DesignTokens.Semantic.Focus : border, DesignTokens.Border.Default);

        // Micro corner ticks
        int t = 5;
        g.DrawRect(sb, r.X, r.Y, t, 2, DesignTokens.Semantic.Secondary * 0.7f);
        g.DrawRect(sb, r.Right - t, r.Y, t, 2, DesignTokens.Semantic.Secondary * 0.7f);

        if (!string.IsNullOrEmpty(label))
        {
            var size = g.MeasureText(label, textScale);
            g.DrawText(sb, label,
                new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + (r.Height - size.Y) * 0.5f),
                DesignTokens.Semantic.TextPrimary, textScale);
        }
    }

    // ---- Tabs --------------------------------------------------------------

    public static void Tab(VoxelionGame g, SpriteBatch sb, Rectangle r, string label, bool selected)
    {
        var fill = selected ? DesignTokens.Semantic.Primary : DesignTokens.Semantic.Surface;
        g.DrawRect(sb, r, fill);
        if (selected)
            g.DrawRect(sb, r.X, r.Bottom - 3, r.Width, 3, DesignTokens.Semantic.Secondary);
        g.DrawBorder(sb, r, selected ? DesignTokens.Semantic.BorderStrong : DesignTokens.Semantic.Border, 1);
        float scale = label.Length > 10 ? 1.1f : 1.3f;
        var size = g.MeasureText(label, scale);
        g.DrawText(sb, label,
            new Vector2(r.X + (r.Width - size.X) * 0.5f, r.Y + (r.Height - size.Y) * 0.5f),
            DesignTokens.Semantic.TextPrimary, scale);
    }

    // ---- Status badge ------------------------------------------------------

    public static void Badge(VoxelionGame g, SpriteBatch sb, Vector2 pos, string text, XnaColor accent)
    {
        var size = g.MeasureText(text, 1.15f);
        var r = new Rectangle((int)pos.X, (int)pos.Y, (int)size.X + 16, (int)size.Y + 10);
        g.DrawRect(sb, r, accent * 0.35f);
        g.DrawRect(sb, r.X, r.Y, 3, r.Height, accent);
        g.DrawBorder(sb, r, accent * 0.8f, 1);
        g.DrawText(sb, text, new Vector2(r.X + 8, r.Y + 5), DesignTokens.Semantic.TextPrimary, 1.15f);
    }

    // ---- Loading indicator -------------------------------------------------

    /// <summary>Crystal-bar progress with glow tip.</summary>
    public static void ProgressCrystal(VoxelionGame g, SpriteBatch sb, Rectangle track, float progress)
    {
        progress = MathHelper.Clamp(progress, 0f, 1f);
        g.DrawRect(sb, track, DesignTokens.Semantic.SurfaceSunken);
        g.DrawBorder(sb, track, DesignTokens.Semantic.Border, 1);

        int fillW = (int)(track.Width * progress);
        if (fillW > 0)
        {
            var fill = new Rectangle(track.X, track.Y, fillW, track.Height);
            g.DrawRect(sb, fill, DesignTokens.Semantic.Primary);
            // Tip glow
            int tip = Math.Min(8, fillW);
            g.DrawRect(sb, track.X + fillW - tip, track.Y - 1, tip, track.Height + 2, DesignTokens.Semantic.Secondary * 0.7f);
            // Sheen
            g.DrawRect(sb, track.X, track.Y, fillW, Math.Max(1, track.Height / 3), XnaColor.White * 0.2f);
        }
    }

    /// <summary>Orbital spinner using rect segments (no textures).</summary>
    public static void Spinner(VoxelionGame g, SpriteBatch sb, Vector2 center, float radius, float time)
    {
        const int segments = 8;
        for (int i = 0; i < segments; i++)
        {
            float a = time * 3f + i * (MathHelper.TwoPi / segments);
            float fade = (i / (float)segments);
            var c = DesignTokens.Semantic.Secondary * (0.2f + 0.8f * fade);
            float x = center.X + MathF.Cos(a) * radius;
            float y = center.Y + MathF.Sin(a) * radius;
            g.DrawRect(sb, x - 3, y - 3, 6, 6, c);
        }
    }

    // ---- World card --------------------------------------------------------

    public static void WorldCard(VoxelionGame g, SpriteBatch sb, Rectangle r, string name, string meta, XnaColor accent, bool selected, bool favorite)
    {
        Panel(g, sb, r, elevated: true, glow: selected);

        // Preview gem
        var gem = new Rectangle(r.X + 10, r.Y + 10, 56, r.Height - 20);
        g.DrawRect(sb, gem, accent * 0.45f);
        g.DrawRect(sb, gem.X + 12, gem.Y + gem.Height / 2 - 16, 32, 32, accent);
        g.DrawBorder(sb, gem, accent * 0.8f, 1);

        float tx = r.X + 78;
        g.DrawText(sb, name, new Vector2(tx, r.Y + 14), DesignTokens.Semantic.TextPrimary, 1.5f);
        g.DrawText(sb, meta, new Vector2(tx, r.Y + 40), DesignTokens.Semantic.TextMuted, 1.15f);

        if (favorite)
            g.DrawText(sb, "*", new Vector2(r.Right - 28, r.Y + 12), DesignTokens.Semantic.Accent, 2f);
    }

    // ---- Inventory slot ----------------------------------------------------

    public static void InventorySlot(VoxelionGame g, SpriteBatch sb, Rectangle r, bool empty, bool selected, XnaColor? rarity = null)
    {
        g.DrawRect(sb, r, DesignTokens.Semantic.SurfaceElevated);
        g.DrawBorder(sb, r,
            selected ? DesignTokens.Semantic.Primary : DesignTokens.Semantic.Border,
            selected ? DesignTokens.Border.Thick : DesignTokens.Border.Thin);

        if (!empty && rarity.HasValue)
        {
            g.DrawRect(sb, r.X + 8, r.Y + 8, r.Width - 16, r.Height - 16, rarity.Value * 0.55f);
            // Rarity edge
            g.DrawRect(sb, r.X, r.Bottom - 3, r.Width, 3, rarity.Value);
        }
        else if (empty)
        {
            // Empty cross motif
            int cx = r.Center.X, cy = r.Center.Y;
            g.DrawRect(sb, cx - 8, cy - 1, 16, 2, DesignTokens.Semantic.Border * 0.5f);
            g.DrawRect(sb, cx - 1, cy - 8, 2, 16, DesignTokens.Semantic.Border * 0.5f);
        }
    }

    // ---- Avatar ------------------------------------------------------------

    public static void Avatar(VoxelionGame g, SpriteBatch sb, Vector2 center, float size, XnaColor primary, XnaColor secondary)
    {
        float s = size;
        // Glow halo
        g.DrawRect(sb, center.X - s * 0.65f, center.Y - s * 0.65f, s * 1.3f, s * 1.3f, primary * 0.25f);
        // Body block
        g.DrawRect(sb, center.X - s * 0.35f, center.Y - s * 0.1f, s * 0.7f, s * 0.7f, primary * 0.85f);
        // Head
        g.DrawRect(sb, center.X - s * 0.28f, center.Y - s * 0.55f, s * 0.56f, s * 0.45f, primary);
        // Face plate
        g.DrawRect(sb, center.X - s * 0.18f, center.Y - s * 0.42f, s * 0.36f, s * 0.28f, secondary);
        // Silhouette outline
        var box = new Rectangle((int)(center.X - s * 0.4f), (int)(center.Y - s * 0.6f), (int)(s * 0.8f), (int)(s * 1.15f));
        g.DrawBorder(sb, box, DesignTokens.Semantic.BorderStrong * 0.5f, 1);
    }

    // ---- Emblem (boot / splash / hub) --------------------------------------

    public static void Emblem(VoxelionGame g, SpriteBatch sb, Vector2 center, float size, float alpha = 1f)
    {
        // Outer glow diamond field
        g.DrawRect(sb, center.X - size * 1.25f, center.Y - size * 1.25f,
            size * 2.5f, size * 2.5f, DesignTokens.Glow.Primary * (0.4f * alpha));
        // Outer square
        g.DrawRect(sb, center.X - size * 0.55f, center.Y - size * 0.55f,
            size * 1.1f, size * 1.1f, DesignTokens.Semantic.Primary * alpha);
        // Inner core
        g.DrawRect(sb, center.X - size * 0.28f, center.Y - size * 0.28f,
            size * 0.56f, size * 0.56f, DesignTokens.Semantic.Secondary * alpha);
        // Corner brackets
        float s = size * 0.7f;
        var r = new Rectangle((int)(center.X - s), (int)(center.Y - s), (int)(s * 2), (int)(s * 2));
        DrawCornerBrackets(g, sb, r, DesignTokens.Semantic.Secondary * (0.8f * alpha), (int)(size * 0.25f));
    }

    // ---- Ambient particles -------------------------------------------------

    public static void AmbientDust(VoxelionGame g, SpriteBatch sb, Viewport vp, float time, int count = 28)
    {
        for (int i = 0; i < count; i++)
        {
            float px = (MathF.Sin(time * 0.11f + i * 1.7f) * 0.5f + 0.5f) * vp.Width;
            float py = (MathF.Cos(time * 0.08f + i * 1.3f) * 0.5f + 0.5f) * vp.Height;
            float a = 0.08f + 0.1f * MathF.Sin(time * 1.5f + i);
            float sz = 1.5f + (i % 3);
            g.DrawRect(sb, px, py, sz, sz, DesignTokens.Semantic.Primary * a);
        }
    }

    // ---- Helpers -----------------------------------------------------------

    private static Rectangle Inflate(Rectangle r, int a) =>
        new(r.X - a, r.Y - a, r.Width + a * 2, r.Height + a * 2);

    private static Rectangle Deflate(Rectangle r, int a) =>
        new(r.X + a, r.Y + a, Math.Max(1, r.Width - a * 2), Math.Max(1, r.Height - a * 2));

    private static void DrawCornerBrackets(VoxelionGame g, SpriteBatch sb, Rectangle r, XnaColor c, int len)
    {
        // TL
        g.DrawRect(sb, r.X, r.Y, len, 2, c);
        g.DrawRect(sb, r.X, r.Y, 2, len, c);
        // TR
        g.DrawRect(sb, r.Right - len, r.Y, len, 2, c);
        g.DrawRect(sb, r.Right - 2, r.Y, 2, len, c);
        // BL
        g.DrawRect(sb, r.X, r.Bottom - 2, len, 2, c);
        g.DrawRect(sb, r.X, r.Bottom - len, 2, len, c);
        // BR
        g.DrawRect(sb, r.Right - len, r.Bottom - 2, len, 2, c);
        g.DrawRect(sb, r.Right - 2, r.Bottom - len, 2, len, c);
    }
}
