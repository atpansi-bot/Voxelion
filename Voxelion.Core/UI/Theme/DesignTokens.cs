using Microsoft.Xna.Framework;

namespace Voxelion.Core.UI.Theme;

/// <summary>
/// VOXELION Global Design System — single source of truth for all UI visuals.
/// Scenes must consume these tokens; do not hard-code palette values in scene logic.
/// </summary>
public static class DesignTokens
{
    // -------------------------------------------------------------------------
    // RAW / PRIMITIVE COLORS (palette)
    // -------------------------------------------------------------------------
    public static class Palette
    {
        public static readonly Color Void = new(8, 6, 14);
        public static readonly Color Night = new(14, 12, 28);
        public static readonly Color Indigo = new(28, 24, 52);
        public static readonly Color Violet = new(148, 92, 255);
        public static readonly Color Cyan = new(72, 196, 255);
        public static readonly Color Gold = new(255, 168, 72);
        public static readonly Color Ember = new(255, 72, 96);
        public static readonly Color Mint = new(72, 220, 140);
        public static readonly Color Amber = new(255, 196, 64);
        public static readonly Color Snow = new(245, 242, 255);
        public static readonly Color Mist = new(180, 172, 210);
        public static readonly Color Fog = new(120, 112, 150);
        public static readonly Color Ash = new(80, 74, 100);
        public static readonly Color Black = new(0, 0, 0);
        public static readonly Color White = new(255, 255, 255);
    }

    // -------------------------------------------------------------------------
    // SEMANTIC COLORS — preferred for scene code
    // -------------------------------------------------------------------------
    public static class Semantic
    {
        // Surfaces
        public static readonly Color Background = Palette.Void;
        public static readonly Color BackgroundAlt = Palette.Night;
        public static readonly Color Surface = new(22, 20, 42, 230);
        public static readonly Color SurfaceElevated = new(32, 28, 58, 240);
        public static readonly Color SurfaceGlass = new(40, 36, 72, 180);
        public static readonly Color SurfaceSunken = Palette.Indigo;

        // Brand / action
        public static readonly Color Primary = Palette.Violet;
        public static readonly Color Secondary = Palette.Cyan;
        public static readonly Color Accent = Palette.Gold;

        // Feedback
        public static readonly Color Success = Palette.Mint;
        public static readonly Color Warning = Palette.Amber;
        public static readonly Color Error = Palette.Ember;
        public static readonly Color Info = Palette.Cyan;

        // Interactive states
        public static readonly Color Disabled = Palette.Ash;
        public static readonly Color Focus = new(148, 92, 255, 200);
        public static readonly Color Selection = new(148, 92, 255, 90);
        public static readonly Color Hover = new(148, 92, 255, 40);
        public static readonly Color Pressed = new(148, 92, 255, 120);

        // Text
        public static readonly Color TextPrimary = Palette.Snow;
        public static readonly Color TextSecondary = Palette.Mist;
        public static readonly Color TextMuted = Palette.Fog;
        public static readonly Color TextDisabled = Palette.Ash;
        public static readonly Color TextOnPrimary = Palette.Snow;
        public static readonly Color TextDanger = Palette.Ember;

        // Chrome
        public static readonly Color Border = new(80, 70, 120, 120);
        public static readonly Color BorderStrong = new(148, 92, 255, 200);
        public static readonly Color BorderDanger = new(255, 72, 96, 180);
        public static readonly Color Overlay = new(0, 0, 0, 160);
        public static readonly Color OverlayHeavy = new(0, 0, 0, 210);
        public static readonly Color Scrim = new(0, 0, 0, 180);

        // Rarity (inventory)
        public static readonly Color RarityCommon = new(160, 160, 170);
        public static readonly Color RarityUncommon = new(72, 200, 120);
        public static readonly Color RarityRare = new(72, 140, 255);
        public static readonly Color RarityEpic = new(180, 80, 255);
        public static readonly Color RarityLegendary = new(255, 168, 48);
    }

    // -------------------------------------------------------------------------
    // LEGACY Color aliases — keep existing scene code compiling
    // -------------------------------------------------------------------------
    public static class Color
    {
        public static readonly Microsoft.Xna.Framework.Color VoidBlack = Semantic.Background;
        public static readonly Microsoft.Xna.Framework.Color DeepNight = Semantic.BackgroundAlt;
        public static readonly Microsoft.Xna.Framework.Color ShadowIndigo = Semantic.SurfaceSunken;
        public static readonly Microsoft.Xna.Framework.Color PanelBase = Semantic.Surface;
        public static readonly Microsoft.Xna.Framework.Color PanelElevated = Semantic.SurfaceElevated;
        public static readonly Microsoft.Xna.Framework.Color PanelGlass = Semantic.SurfaceGlass;

        public static readonly Microsoft.Xna.Framework.Color AccentPrimary = Semantic.Primary;
        public static readonly Microsoft.Xna.Framework.Color AccentSecondary = Semantic.Secondary;
        public static readonly Microsoft.Xna.Framework.Color AccentTertiary = Semantic.Accent;
        public static readonly Microsoft.Xna.Framework.Color AccentDanger = Semantic.Error;
        public static readonly Microsoft.Xna.Framework.Color AccentSuccess = Semantic.Success;
        public static readonly Microsoft.Xna.Framework.Color AccentWarning = Semantic.Warning;

        public static readonly Microsoft.Xna.Framework.Color TextPrimary = Semantic.TextPrimary;
        public static readonly Microsoft.Xna.Framework.Color TextSecondary = Semantic.TextSecondary;
        public static readonly Microsoft.Xna.Framework.Color TextMuted = Semantic.TextMuted;
        public static readonly Microsoft.Xna.Framework.Color TextDisabled = Semantic.TextDisabled;

        public static readonly Microsoft.Xna.Framework.Color GlowPrimary = new(148, 92, 255, 80);
        public static readonly Microsoft.Xna.Framework.Color GlowSecondary = new(72, 196, 255, 60);
        public static readonly Microsoft.Xna.Framework.Color BorderSubtle = Semantic.Border;
        public static readonly Microsoft.Xna.Framework.Color BorderFocus = Semantic.BorderStrong;
        public static readonly Microsoft.Xna.Framework.Color OverlayDim = Semantic.Overlay;
        public static readonly Microsoft.Xna.Framework.Color OverlayHeavy = Semantic.OverlayHeavy;

        public static readonly Microsoft.Xna.Framework.Color RarityCommon = Semantic.RarityCommon;
        public static readonly Microsoft.Xna.Framework.Color RarityUncommon = Semantic.RarityUncommon;
        public static readonly Microsoft.Xna.Framework.Color RarityRare = Semantic.RarityRare;
        public static readonly Microsoft.Xna.Framework.Color RarityEpic = Semantic.RarityEpic;
        public static readonly Microsoft.Xna.Framework.Color RarityLegendary = Semantic.RarityLegendary;
    }

    // -------------------------------------------------------------------------
    // TYPOGRAPHY (pixel-font scale multipliers)
    // -------------------------------------------------------------------------
    public static class Typography
    {
        public const float Display = 3.5f;
        public const float Title = 2.8f;
        public const float Heading = 2.2f;
        public const float Subheading = 1.8f;
        public const float Body = 1.5f;
        public const float BodySmall = 1.3f;
        public const float Caption = 1.2f;
        public const float Micro = 1.0f;
        public const float Button = 1.7f;
        public const float ButtonLarge = 2.1f;

        // Legacy aliases
        public const float ScaleDisplay = Display;
        public const float ScaleTitle = Title;
        public const float ScaleHeading = Heading;
        public const float ScaleBody = Body;
        public const float ScaleCaption = Caption;
        public const float ScaleMicro = Micro;

        public const float LineHeightTight = 1.15f;
        public const float LineHeightNormal = 1.35f;
        public const float LineHeightRelaxed = 1.55f;
    }

    // -------------------------------------------------------------------------
    // SPACING (8pt grid)
    // -------------------------------------------------------------------------
    public static class Spacing
    {
        public const float Unit = 8f;
        public const float XXS = 2f;
        public const float XS = 4f;
        public const float S = 8f;
        public const float M = 12f;
        public const float L = 16f;
        public const float XL = 24f;
        public const float XXL = 32f;
        public const float XXXL = 48f;
        public const float Huge = 64f;
        public const float Section = 40f;
    }

    // -------------------------------------------------------------------------
    // CORNER RADIUS (logical; MonoGame draws rects — used as layout intent)
    // -------------------------------------------------------------------------
    public static class Radius
    {
        public const float None = 0f;
        public const float XS = 4f;
        public const float S = 6f;
        public const float M = 8f;
        public const float L = 12f;
        public const float XL = 16f;
        public const float Pill = 999f;
        public const float Panel = L;
        public const float Button = M;
        public const float Input = S;
        public const float Chip = Pill;
    }

    // -------------------------------------------------------------------------
    // BORDERS
    // -------------------------------------------------------------------------
    public static class Border
    {
        public const int Thin = 1;
        public const int Default = 2;
        public const int Thick = 3;
        public const int Focus = 2;
        public static readonly Microsoft.Xna.Framework.Color Subtle = Semantic.Border;
        public static readonly Microsoft.Xna.Framework.Color Strong = Semantic.BorderStrong;
        public static readonly Microsoft.Xna.Framework.Color Danger = Semantic.BorderDanger;
    }

    // -------------------------------------------------------------------------
    // SHADOWS / GLOW / OPACITY
    // -------------------------------------------------------------------------
    public static class Shadow
    {
        public static readonly Microsoft.Xna.Framework.Color Soft = new(0, 0, 0, 80);
        public static readonly Microsoft.Xna.Framework.Color Medium = new(0, 0, 0, 120);
        public static readonly Microsoft.Xna.Framework.Color Hard = new(0, 0, 0, 180);
        public const float OffsetY = 4f;
        public const float Spread = 8f;
    }

    public static class Glow
    {
        public static readonly Microsoft.Xna.Framework.Color Primary = new(148, 92, 255, 80);
        public static readonly Microsoft.Xna.Framework.Color Secondary = new(72, 196, 255, 60);
        public static readonly Microsoft.Xna.Framework.Color Accent = new(255, 168, 72, 70);
        public static readonly Microsoft.Xna.Framework.Color Danger = new(255, 72, 96, 70);
        public const float SoftSpread = 1.2f;
        public const float StrongSpread = 1.6f;
    }

    public static class Opacity
    {
        public const float Invisible = 0f;
        public const float Faint = 0.12f;
        public const float Subtle = 0.35f;
        public const float Medium = 0.55f;
        public const float Strong = 0.75f;
        public const float Almost = 0.90f;
        public const float Opaque = 1f;
        public const float Disabled = 0.40f;
        public const float Overlay = 0.65f;
        public const float Scrim = 0.75f;
    }

    // -------------------------------------------------------------------------
    // DEPTH (layer intent)
    // -------------------------------------------------------------------------
    public static class Depth
    {
        public const int Base = 0;
        public const int Raised = 1;
        public const int Floating = 2;
        public const int Overlay = 3;
        public const int Modal = 4;
        public const int Toast = 5;
    }

    // -------------------------------------------------------------------------
    // MOTION — duration + easing helpers
    // -------------------------------------------------------------------------
    public static class Motion
    {
        public const float Instant = 0.05f;
        public const float Fast = 0.12f;
        public const float Normal = 0.22f;
        public const float Slow = 0.38f;
        public const float Cinematic = 0.65f;
        public const float Epic = 1.15f;

        // Legacy names
        public const float DurationInstant = Instant;
        public const float DurationFast = Fast;
        public const float DurationNormal = Normal;
        public const float DurationSlow = Slow;
        public const float DurationCinematic = Cinematic;
        public const float DurationEpic = Epic;
        public const float EaseOutCubic = 0.33f;
        public const float SpringStiffness = 180f;
        public const float SpringDamping = 18f;

        public static float Linear(float t) => MathHelper.Clamp(t, 0f, 1f);

        public static float EaseOut(float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            return 1f - MathF.Pow(1f - t, 3f);
        }

        public static float EaseIn(float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            return t * t * t;
        }

        public static float EaseInOut(float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            return t < 0.5f
                ? 4f * t * t * t
                : 1f - MathF.Pow(-2f * t + 2f, 3f) / 2f;
        }

        public static float SmoothStep(float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            return t * t * (3f - 2f * t);
        }
    }

    // -------------------------------------------------------------------------
    // LAYOUT / SAFE AREA / TOUCH
    // -------------------------------------------------------------------------
    public static class Layout
    {
        public const float SafeAreaMin = 24f;
        public const float SafeAreaPreferred = 32f;
        public const float PanelCornerRadius = Radius.Panel;
        public const float ButtonCornerRadius = Radius.Button;
        public const float InputCornerRadius = Radius.Input;
        public const float MaxContentWidth = 1280f;
        public const float MinTouchTarget = 48f;
        public const float MaxTouchTarget = 72f;
        public const float ComfortTouch = 56f;
        public const float TopBarHeight = 48f;
        public const float BottomNavHeight = 56f;
    }

    // -------------------------------------------------------------------------
    // Z-INDEX
    // -------------------------------------------------------------------------
    public static class ZIndex
    {
        public const int World = 0;
        public const int WorldFx = 50;
        public const int Hud = 100;
        public const int HudControls = 120;
        public const int Overlay = 200;
        public const int Sheet = 250;
        public const int Modal = 300;
        public const int Dialog = 400;
        public const int Toast = 500;
        public const int Tooltip = 550;
        public const int Critical = 600;
        public const int Debug = 999;
    }

    // -------------------------------------------------------------------------
    // COMPONENT DEFAULTS (shared sizes)
    // -------------------------------------------------------------------------
    public static class Component
    {
        public const float ButtonHeight = 52f;
        public const float ButtonHeightSm = 40f;
        public const float ButtonMinWidth = 120f;
        public const float InputHeight = 48f;
        public const float ProgressHeight = 12f;
        public const float IconSm = 20f;
        public const float IconMd = 28f;
        public const float IconLg = 40f;
        public const float ChipHeight = 36f;
        public const float CardMinHeight = 100f;
        public const float AvatarSm = 32f;
        public const float AvatarMd = 48f;
        public const float AvatarLg = 80f;
    }
}
