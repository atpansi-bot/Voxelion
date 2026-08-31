using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Voxelion.Core.UI;

/// <summary>
/// Runtime 5x7 bitmap font — no Content pipeline required.
/// Readable labels on Android without SpriteFont.
/// </summary>
public sealed class PixelFont
{
    private readonly Texture2D _pixel;
    private static readonly Dictionary<char, byte[]> Glyphs = BuildGlyphs();

    public const int GlyphW = 5;
    public const int GlyphH = 7;
    public const int Advance = 6;

    public PixelFont(Texture2D pixel) => _pixel = pixel;

    public Vector2 Measure(string text, float scale = 1f)
    {
        if (string.IsNullOrEmpty(text)) return Vector2.Zero;
        return new Vector2(text.Length * Advance * scale, GlyphH * scale);
    }

    public void Draw(SpriteBatch sb, string text, Vector2 pos, Color color, float scale = 1f)
    {
        if (string.IsNullOrEmpty(text) || scale <= 0) return;
        float x = pos.X;
        float y = pos.Y;
        float px = Math.Max(1f, scale);

        foreach (char raw in text)
        {
            char c = char.ToUpperInvariant(raw);
            if (c == ' ')
            {
                x += Advance * scale;
                continue;
            }

            if (!Glyphs.TryGetValue(c, out var rows))
            {
                // unknown → small block
                sb.Draw(_pixel, new Rectangle((int)x, (int)y, (int)(3 * scale), (int)(5 * scale)), color * 0.6f);
                x += Advance * scale;
                continue;
            }

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
    }

    private static Dictionary<char, byte[]> BuildGlyphs()
    {
        // 5 columns bit patterns, top→bottom. Bit4 = leftmost.
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
        A('2', 0b01110, 0b10001, 0b00001, 0b00010, 0b00100, 0b01000, 0b11111);
        A('3', 0b01110, 0b10001, 0b00001, 0b00110, 0b00001, 0b10001, 0b01110);
        A('4', 0b00010, 0b00110, 0b01010, 0b10010, 0b11111, 0b00010, 0b00010);
        A('5', 0b11111, 0b10000, 0b11110, 0b00001, 0b00001, 0b10001, 0b01110);
        A('6', 0b01110, 0b10000, 0b10000, 0b11110, 0b10001, 0b10001, 0b01110);
        A('7', 0b11111, 0b00001, 0b00010, 0b00100, 0b01000, 0b01000, 0b01000);
        A('8', 0b01110, 0b10001, 0b10001, 0b01110, 0b10001, 0b10001, 0b01110);
        A('9', 0b01110, 0b10001, 0b10001, 0b01111, 0b00001, 0b00001, 0b01110);

        A('-', 0b00000, 0b00000, 0b00000, 0b11111, 0b00000, 0b00000, 0b00000);
        A('_', 0b00000, 0b00000, 0b00000, 0b00000, 0b00000, 0b00000, 0b11111);
        A('.', 0b00000, 0b00000, 0b00000, 0b00000, 0b00000, 0b01100, 0b01100);
        A(':', 0b00000, 0b01100, 0b01100, 0b00000, 0b01100, 0b01100, 0b00000);
        A('!', 0b00100, 0b00100, 0b00100, 0b00100, 0b00100, 0b00000, 0b00100);
        A('?', 0b01110, 0b10001, 0b00001, 0b00010, 0b00100, 0b00000, 0b00100);
        A('/', 0b00001, 0b00010, 0b00100, 0b00100, 0b01000, 0b10000, 0b10000);
        A('\'', 0b00100, 0b00100, 0b01000, 0b00000, 0b00000, 0b00000, 0b00000);
        A('+', 0b00000, 0b00100, 0b00100, 0b11111, 0b00100, 0b00100, 0b00000);

        return g;
    }
}
