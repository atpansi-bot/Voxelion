using System;
using Microsoft.Xna.Framework;

namespace Voxelion.Core.DesignSystem
{
    /// <summary>
    /// Unified motion language for VOXELION.
    /// All animations communicate hierarchy and state.
    /// Reduced-motion accessibility path is mandatory.
    /// </summary>
    public static class MotionSystem
    {
        public static bool ReducedMotion { get; set; } = false;

        // Durations in seconds
        public static float Instant => ReducedMotion ? 0f : 0.05f;
        public static float Fast => ReducedMotion ? 0f : 0.12f;
        public static float Normal => ReducedMotion ? 0f : 0.22f;
        public static float Slow => ReducedMotion ? 0f : 0.35f;
        public static float Dramatic => ReducedMotion ? 0.05f : 0.55f;
        public static float WorldTransition => ReducedMotion ? 0.1f : 1.2f;

        // Easing functions (pure math, no allocation)
        public static float EaseOutCubic(float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            float inv = 1f - t;
            return 1f - inv * inv * inv;
        }

        public static float EaseInOutCubic(float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            return t < 0.5f
                ? 4f * t * t * t
                : 1f - MathF.Pow(-2f * t + 2f, 3f) / 2f;
        }

        public static float EaseOutBack(float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * MathF.Pow(t - 1f, 3f) + c1 * MathF.Pow(t - 1f, 2f);
        }

        public static float EaseOutExpo(float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            return t >= 1f ? 1f : 1f - MathF.Pow(2f, -10f * t);
        }

        public static float Lerp(float a, float b, float t) => a + (b - a) * t;

        public static Color LerpColor(Color a, Color b, float t)
        {
            return new Color(
                (byte)Lerp(a.R, b.R, t),
                (byte)Lerp(a.G, b.G, t),
                (byte)Lerp(a.B, b.B, t),
                (byte)Lerp(a.A, b.A, t));
        }

        public static Vector2 LerpVec(Vector2 a, Vector2 b, float t) => Vector2.Lerp(a, b, t);
    }
}
