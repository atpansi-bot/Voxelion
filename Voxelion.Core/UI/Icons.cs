using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Voxelion.Core.UI;

/// <summary>Geometric icon primitives drawn with the 1x1 pixel texture — no external assets.</summary>
public static class Icons
{
    public static void Draw(SpriteBatch sb, Texture2D px, string id, Rectangle bounds, Color color)
    {
        float x = bounds.X, y = bounds.Y, w = bounds.Width, h = bounds.Height;
        float s = Math.Min(w, h);
        float cx = x + w * 0.5f, cy = y + h * 0.5f;
        float u = s / 8f;

        void R(float rx, float ry, float rw, float rh) =>
            sb.Draw(px, new Rectangle((int)rx, (int)ry, Math.Max(1, (int)rw), Math.Max(1, (int)rh)), color);

        switch (id.ToLowerInvariant())
        {
            case "play":
                R(cx - u * 1.2f, cy - u * 2, u, u * 4);
                R(cx - u * 0.2f, cy - u * 1.5f, u, u * 3);
                R(cx + u * 0.8f, cy - u, u, u * 2);
                break;
            case "user":
            case "account":
                R(cx - u, cy - u * 2.5f, u * 2, u * 2);
                R(cx - u * 1.8f, cy, u * 3.6f, u * 2.2f);
                break;
            case "gear":
            case "settings":
                R(cx - u * 0.7f, cy - u * 0.7f, u * 1.4f, u * 1.4f);
                R(cx - u * 0.3f, cy - u * 2.4f, u * 0.6f, u * 1.2f);
                R(cx - u * 0.3f, cy + u * 1.2f, u * 0.6f, u * 1.2f);
                R(cx - u * 2.4f, cy - u * 0.3f, u * 1.2f, u * 0.6f);
                R(cx + u * 1.2f, cy - u * 0.3f, u * 1.2f, u * 0.6f);
                break;
            case "world":
            case "globe":
                R(cx - u * 2, cy - u * 2, u * 4, u * 4);
                R(cx - u * 0.3f, cy - u * 2, u * 0.6f, u * 4);
                R(cx - u * 2, cy - u * 0.3f, u * 4, u * 0.6f);
                break;
            case "bag":
            case "inventory":
                R(cx - u * 2, cy - u, u * 4, u * 3);
                R(cx - u, cy - u * 2, u * 2, u);
                break;
            case "social":
            case "friends":
                R(cx - u * 2.5f, cy - u, u * 1.8f, u * 2.2f);
                R(cx + u * 0.5f, cy - u, u * 1.8f, u * 2.2f);
                R(cx - u * 1.8f, cy - u * 2.2f, u * 1.2f, u * 1.2f);
                R(cx + u * 0.8f, cy - u * 2.2f, u * 1.2f, u * 1.2f);
                break;
            case "chat":
                R(cx - u * 2.2f, cy - u * 1.8f, u * 4.4f, u * 3);
                R(cx - u, cy + u * 1.2f, u, u * 1.2f);
                break;
            case "back":
                R(cx - u * 0.5f, cy - u * 0.4f, u * 2.5f, u * 0.8f);
                R(cx - u * 1.5f, cy - u * 1.5f, u * 0.8f, u * 3);
                break;
            case "star":
                R(cx - u * 0.4f, cy - u * 2.2f, u * 0.8f, u * 4.4f);
                R(cx - u * 2.2f, cy - u * 0.4f, u * 4.4f, u * 0.8f);
                break;
            case "menu":
                R(x + u, y + u * 1.5f, s - u * 2, u * 0.7f);
                R(x + u, y + u * 3.5f, s - u * 2, u * 0.7f);
                R(x + u, y + u * 5.5f, s - u * 2, u * 0.7f);
                break;
            case "close":
            case "x":
                R(cx - u * 2, cy - u * 0.35f, u * 4, u * 0.7f);
                R(cx - u * 0.35f, cy - u * 2, u * 0.7f, u * 4);
                break;
            case "lang":
            case "globe_lang":
                R(cx - u * 2, cy - u * 2, u * 4, u * 4);
                R(cx - u * 2, cy - u * 0.3f, u * 4, u * 0.6f);
                break;
            case "heart":
                R(cx - u * 2, cy - u, u * 1.6f, u * 1.6f);
                R(cx + u * 0.4f, cy - u, u * 1.6f, u * 1.6f);
                R(cx - u * 1.5f, cy + u * 0.4f, u * 3, u * 1.8f);
                break;
            default:
                R(cx - u, cy - u, u * 2, u * 2);
                break;
        }
    }
}
