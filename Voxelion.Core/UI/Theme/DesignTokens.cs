namespace Voxelion.Core.UI.Theme;

/// <summary>
/// VOXELION Design System — Immutable color, typography, spacing, motion tokens.
/// Pure mathematical identity. No external references.
/// </summary>
public static class DesignTokens
{
    public static class Color
    {
        // Core palette — cinematic fantasy
        public static readonly Microsoft.Xna.Framework.Color VoidBlack = new(8, 6, 14);
        public static readonly Microsoft.Xna.Framework.Color DeepNight = new(14, 12, 28);
        public static readonly Microsoft.Xna.Framework.Color ShadowIndigo = new(28, 24, 52);
        public static readonly Microsoft.Xna.Framework.Color PanelBase = new(22, 20, 42, 230);
        public static readonly Microsoft.Xna.Framework.Color PanelElevated = new(32, 28, 58, 240);
        public static readonly Microsoft.Xna.Framework.Color PanelGlass = new(40, 36, 72, 180);

        public static readonly Microsoft.Xna.Framework.Color AccentPrimary = new(148, 92, 255);      // Magical violet
        public static readonly Microsoft.Xna.Framework.Color AccentSecondary = new(72, 196, 255);    // Celestial cyan
        public static readonly Microsoft.Xna.Framework.Color AccentTertiary = new(255, 168, 72);     // Ember gold
        public static readonly Microsoft.Xna.Framework.Color AccentDanger = new(255, 72, 96);
        public static readonly Microsoft.Xna.Framework.Color AccentSuccess = new(72, 220, 140);
        public static readonly Microsoft.Xna.Framework.Color AccentWarning = new(255, 196, 64);

        public static readonly Microsoft.Xna.Framework.Color TextPrimary = new(245, 242, 255);
        public static readonly Microsoft.Xna.Framework.Color TextSecondary = new(180, 172, 210);
        public static readonly Microsoft.Xna.Framework.Color TextMuted = new(120, 112, 150);
        public static readonly Microsoft.Xna.Framework.Color TextDisabled = new(80, 74, 100);

        public static readonly Microsoft.Xna.Framework.Color GlowPrimary = new(148, 92, 255, 80);
        public static readonly Microsoft.Xna.Framework.Color GlowSecondary = new(72, 196, 255, 60);
        public static readonly Microsoft.Xna.Framework.Color BorderSubtle = new(80, 70, 120, 120);
        public static readonly Microsoft.Xna.Framework.Color BorderFocus = new(148, 92, 255, 200);
        public static readonly Microsoft.Xna.Framework.Color OverlayDim = new(0, 0, 0, 160);
        public static readonly Microsoft.Xna.Framework.Color OverlayHeavy = new(0, 0, 0, 210);

        public static readonly Microsoft.Xna.Framework.Color RarityCommon = new(180, 180, 190);
        public static readonly Microsoft.Xna.Framework.Color RarityUncommon = new(72, 200, 120);
        public static readonly Microsoft.Xna.Framework.Color RarityRare = new(72, 140, 255);
        public static readonly Microsoft.Xna.Framework.Color RarityEpic = new(180, 80, 255);
        public static readonly Microsoft.Xna.Framework.Color RarityLegendary = new(255, 168, 48);
    }

    public static class Typography
    {
        public const float ScaleDisplay = 1.85f;
        public const float ScaleTitle = 1.45f;
        public const float ScaleHeading = 1.20f;
        public const float ScaleBody = 1.00f;
        public const float ScaleCaption = 0.85f;
        public const float ScaleMicro = 0.72f;

        public const float LineHeightTight = 1.15f;
        public const float LineHeightNormal = 1.35f;
        public const float LineHeightRelaxed = 1.55f;
    }

    public static class Spacing
    {
        public const float Unit = 8f;
        public const float XXS = Unit * 0.25f; // 2
        public const float XS = Unit * 0.5f;  // 4
        public const float S = Unit;          // 8
        public const float M = Unit * 1.5f;   // 12
        public const float L = Unit * 2f;     // 16
        public const float XL = Unit * 3f;    // 24
        public const float XXL = Unit * 4f;   // 32
        public const float XXXL = Unit * 6f;  // 48
        public const float Huge = Unit * 8f;  // 64
    }

    public static class Motion
    {
        public const float DurationInstant = 0.05f;
        public const float DurationFast = 0.12f;
        public const float DurationNormal = 0.22f;
        public const float DurationSlow = 0.38f;
        public const float DurationCinematic = 0.65f;
        public const float DurationEpic = 1.15f;

        public const float EaseOutCubic = 0.33f; // parameter for cubic ease
        public const float SpringStiffness = 180f;
        public const float SpringDamping = 18f;
    }

    public static class Layout
    {
        public const float SafeAreaMin = 24f;
        public const float PanelCornerRadius = 12f;
        public const float ButtonCornerRadius = 8f;
        public const float InputCornerRadius = 6f;
        public const float MaxContentWidth = 1280f;
        public const float MinTouchTarget = 48f;
        public const float MaxTouchTarget = 72f;
    }

    public static class ZIndex
    {
        public const int World = 0;
        public const int Hud = 100;
        public const int Overlay = 200;
        public const int Modal = 300;
        public const int Dialog = 400;
        public const int Toast = 500;
        public const int Critical = 600;
        public const int Debug = 999;
    }
}
