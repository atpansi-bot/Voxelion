using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Voxelion.Core.UI;

/// <summary>
/// Runtime bitmap font fallback (5×7). Latin + digits + punctuation.
/// CJK / unsupported codepoints are drawn as readable full-width placeholders
/// (no ToUpper on non-Latin; no false glyph assumptions).
/// Replace with SpriteFont Content assets for production CJK quality.
/// </summary>
public sealed class PixelFont
{
    public const int GlyphW = 5;
    public const int GlyphH = 7;
    public const int Advance = 6;
    public const int CjkAdvance = 8;

    private readonly Texture2D _pixel;
    private static readonly Dictionary<char, byte[]> Glyphs = BuildGlyphs();

    public PixelFont(Texture2D pixel) => _pixel = pixel;

    public static bool IsSupported(char c)
    {
        if (c == ' ' || c == '\n' || c == '\t') return true;
        if (c >= 'a' && c <= 'z') c = (char)(c - 32);
        return Glyphs.ContainsKey(c);
    }

    public static bool IsMostlySupported(string text)
    {
        if (string.IsNullOrEmpty(text)) return true;
        int ok = 0, total = 0;
        foreach (var ch in text)
        {
            if (char.IsWhiteSpace(ch)) continue;
            total++;
            if (IsSupported(ch)) ok++;
        }
        return total == 0 || ok * 2 >= total; // ≥50% supported
    }

    public Vector2 Measure(string text, float scale = 1f)
    {
        if (string.IsNullOrEmpty(text)) return Vector2.Zero;
        float w = 0, lineW = 0, h = GlyphH * scale;
        foreach (char raw in text)
        {
            if (raw == '\n')
            {
                w = Math.Max(w, lineW);
                lineW = 0;
                h += GlyphH * scale * 1.35f;
                continue;
            }
            lineW += AdvanceFor(raw) * scale;
        }
        w = Math.Max(w, lineW);
        return new Vector2(w, h);
    }

    public void Draw(SpriteBatch sb, string text, Vector2 pos, Color color, float scale = 1f)
    {
        if (string.IsNullOrEmpty(text) || scale <= 0) return;
        float x = pos.X;
        float y = pos.Y;
        float px = Math.Max(1f, scale);

        foreach (char raw in text)
        {
            if (raw == '\n')
            {
                x = pos.X;
                y += GlyphH * scale * 1.35f;
                continue;
            }
            if (raw == ' ')
            {
                x += Advance * scale;
                continue;
            }

            char key = raw;
            if (key >= 'a' && key <= 'z') key = (char)(key - 32);

            if (Glyphs.TryGetValue(key, out var rows))
            {
                for (int row = 0; row < GlyphH; row++)
                {
                    byte bits = rows[row];
                    for (int col = 0; col < GlyphW; col++)
                    {
                        if ((bits & (1 << (GlyphW - 1 - col))) == 0) continue;
                        sb.Draw(_pixel,
                            new Rectangle(
                                (int)(x + col * px),
                                (int)(y + row * px),
                                (int)Math.Ceiling(px),
                                (int)Math.Ceiling(px)),
                            color);
                    }
                }
                x += Advance * scale;
            }
            else
            {
                // Unsupported (incl. CJK): full-width placeholder cell — honest, not a fake glyph
                float cw = CjkAdvance * scale;
                float ch = GlyphH * scale;
                var cell = new Rectangle((int)x, (int)y, (int)cw - 1, (int)ch);
                sb.Draw(_pixel, cell, color * 0.22f);
                sb.Draw(_pixel, new Rectangle(cell.X, cell.Y, cell.Width, 1), color * 0.55f);
                sb.Draw(_pixel, new Rectangle(cell.X, cell.Bottom - 1, cell.Width, 1), color * 0.55f);
                sb.Draw(_pixel, new Rectangle(cell.X, cell.Y, 1, cell.Height), color * 0.55f);
                sb.Draw(_pixel, new Rectangle(cell.Right - 1, cell.Y, 1, cell.Height), color * 0.55f);
                // center dot marks "character present"
                sb.Draw(_pixel,
                    new Rectangle((int)(x + cw * 0.35f), (int)(y + ch * 0.35f), (int)(cw * 0.3f), (int)(ch * 0.3f)),
                    color * 0.7f);
                x += cw;
            }
        }
    }

    private static float AdvanceFor(char c)
    {
        if (c == ' ') return Advance;
        char key = c;
        if (key >= 'a' && key <= 'z') key = (char)(key - 32);
        return Glyphs.ContainsKey(key) ? Advance : CjkAdvance;
    }

    private static Dictionary<char, byte[]> BuildGlyphs()
    {
        var g = new Dictionary<char, byte[]>();
        void A(char ch, params byte[] rows) => g[ch] = rows;

        A('A', 0b01110, 0b10001, 0b10001, 0b11111, 0b10001, 0b10001, 0b10001);
        A('B', 0b11110, 0b10001, 0b10001, 0b11110, 0b10001, 0b10001, 0b11110);
        A('C', 0b01110, 0b10001, 0b10000, 0b10000, 0b10000, 0b10001, 0b01110);
        A('D', 0b11110, 0b10001, 0b10001, 0b10001, 0b10001, 0b10001, 0b11110);
        A('E', 0b11111, 0b10000, 0b10000, 0b11110, 0b10000, 0b10000, 0b11111);
        A('F', 0b11111, 0b10000, 0b10000, 0b11110, 0b10000, 0b10000, 0b10000);
        A('G', 0b01110, 0b10001, 0b10000, 0b10111, 0b10001, 0b10001, 0b01110);
        A('H', 0b10001, 0b10001, 0b10001, 0b11111, 0b10001, 0b10001, 0b10001);
        A('I', 0b01110, 0b00100, 0b00100, 0b00100, 0b00100, 0b00100, 0b01110);
        A('J', 0b00111, 0b00010, 0b00010, 0b00010, 0b00010, 0b10010, 0b01100);
        A('K', 0b10001, 0b10010, 0b10100, 0b11000, 0b10100, 0b10010, 0b10001);
        A('L', 0b10000, 0b10000, 0b10000, 0b10000, 0b10000, 0b10000, 0b11111);
        A('M', 0b10001, 0b11011, 0b10101, 0b10001, 0b10001, 0b10001, 0b10001);
        A('N', 0b10001, 0b11001, 0b10101, 0b10011, 0b10001, 0b10001, 0b10001);
        A('O', 0b01110, 0b10001, 0b10001, 0b10001, 0b10001, 0b10001, 0b01110);
        A('P', 0b11110, 0b10001, 0b10001, 0b11110, 0b10000, 0b10000, 0b10000);
        A('Q', 0b01110, 0b10001, 0b10001, 0b10001, 0b10101, 0b10010, 0b01101);
        A('R', 0b11110, 0b10001, 0b10001, 0b11110, 0b10100, 0b10010, 0b10001);
        A('S', 0b01111, 0b10000, 0b10000, 0b01110, 0b00001, 0b00001, 0b11110);
        A('T', 0b11111, 0b00100, 0b00100, 0b00100, 0b00100, 0b00100, 0b00100);
        A('U', 0b10001, 0b10001, 0b10001, 0b10001, 0b10001, 0b10001, 0b01110);
        A('V', 0b10001, 0b10001, 0b10001, 0b10001, 0b10001, 0b01010, 0b00100);
        A('W', 0b10001, 0b10001, 0b10001, 0b10101, 0b10101, 0b10101, 0b01010);
        A('X', 0b10001, 0b10001, 0b01010, 0b00100, 0b01010, 0b10001, 0b10001);
        A('Y', 0b10001, 0b10001, 0b01010, 0b00100, 0b00100, 0b00100, 0b00100);
        A('Z', 0b11111, 0b00001, 0b00010, 0b00100, 0b01000, 0b10000, 0b11111);

        A('0', 0b01110, 0b10001, 0b10011, 0b10101, 0b11001, 0b10001, 0b01110);
        A('1', 0b00100, 0b01100, 0b00100, 0b00100, 0b00100, 0b00100, 0b01110);
        A('2', 0b01110, 0b10001, 0b00001, 0b00110, 0b01000, 0b10000, 0b11111);
        A('3', 0b01110, 0b10001, 0b00001, 0b00110, 0b00001, 0b10001, 0b01110);
        A('4', 0b00010, 0b00110, 0b01010, 0b10010, 0b11111, 0b00010, 0b00010);
        A('5', 0b11111, 0b10000, 0b11110, 0b00001, 0b00001, 0b10001, 0b01110);
        A('6', 0b01110, 0b10000, 0b10000, 0b11110, 0b10001, 0b10001, 0b01110);
        A('7', 0b11111, 0b00001, 0b00010, 0b00100, 0b01000, 0b01000, 0b01000);
        A('8', 0b01110, 0b10001, 0b10001, 0b01110, 0b10001, 0b10001, 0b01110);
        A('9', 0b01110, 0b10001, 0b10001, 0b01111, 0b00001, 0b00001, 0b01110);

        A('.', 0b00000, 0b00000, 0b00000, 0b00000, 0b00000, 0b00100, 0b00100);
        A(',', 0b00000, 0b00000, 0b00000, 0b00000, 0b00100, 0b00100, 0b01000);
        A(':', 0b00000, 0b00100, 0b00100, 0b00000, 0b00100, 0b00100, 0b00000);
        A('!', 0b00100, 0b00100, 0b00100, 0b00100, 0b00100, 0b00000, 0b00100);
        A('?', 0b01110, 0b10001, 0b00001, 0b00110, 0b00100, 0b00000, 0b00100);
        A('-', 0b00000, 0b00000, 0b00000, 0b11111, 0b00000, 0b00000, 0b00000);
        A('+', 0b00000, 0b00100, 0b00100, 0b11111, 0b00100, 0b00100, 0b00000);
        A('*', 0b00000, 0b00100, 0b10101, 0b01110, 0b10101, 0b00100, 0b00000);
        A('/', 0b00001, 0b00010, 0b00100, 0b00100, 0b01000, 0b10000, 0b10000);
        A('%', 0b11001, 0b11010, 0b00100, 0b01000, 0b01011, 0b10011, 0b00000);
        A('(', 0b00010, 0b00100, 0b01000, 0b01000, 0b01000, 0b00100, 0b00010);
        A(')', 0b01000, 0b00100, 0b00010, 0b00010, 0b00010, 0b00100, 0b01000);
        A('[', 0b01110, 0b01000, 0b01000, 0b01000, 0b01000, 0b01000, 0b01110);
        A(']', 0b01110, 0b00010, 0b00010, 0b00010, 0b00010, 0b00010, 0b01110);
        A('_', 0b00000, 0b00000, 0b00000, 0b00000, 0b00000, 0b00000, 0b11111);
        A('=', 0b00000, 0b00000, 0b11111, 0b00000, 0b11111, 0b00000, 0b00000);
        A('\'', 0b00100, 0b00100, 0b01000, 0b00000, 0b00000, 0b00000, 0b00000);
        A('"', 0b01010, 0b01010, 0b00000, 0b00000, 0b00000, 0b00000, 0b00000);
        A('#', 0b01010, 0b01010, 0b11111, 0b01010, 0b11111, 0b01010, 0b01010);
        A('@', 0b01110, 0b10001, 0b10111, 0b10101, 0b10110, 0b10000, 0b01110);
        A('&', 0b01100, 0b10010, 0b10100, 0b01000, 0b10101, 0b10010, 0b01101);
        A('<', 0b00010, 0b00100, 0b01000, 0b10000, 0b01000, 0b00100, 0b00010);
        A('>', 0b01000, 0b00100, 0b00010, 0b00001, 0b00010, 0b00100, 0b01000);
        A('v', 0b00000, 0b00000, 0b10001, 0b10001, 0b01010, 0b01010, 0b00100); // dropdown caret (also as V lower pattern stored under letter already)

        return g;
    }
}
