using Microsoft.Xna.Framework;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace Voxelion.Core.UI.Theme;

/// <summary>
/// VOXELION Global Design System — single source of truth for all UI visuals.
/// Uses XnaColor alias so nested static class Color does not collide with XNA Color.
/// </summary>
public static class DesignTokens
{
    // -------------------------------------------------------------------------
    // RAW / PRIMITIVE COLORS
    // -------------------------------------------------------------------------
    public static class Palette
    {
        public static readonly XnaColor Void = new(8, 6, 14);
        public static readonly XnaColor Night = new(14, 12, 28);
        public static readonly XnaColor Indigo = new(28, 24, 52);
        public static readonly XnaColor Violet = new(148, 92, 255);
        public static readonly XnaColor Cyan = new(72, 196, 255);
        public static readonly XnaColor Gold = new(255, 168, 72);
        public static readonly XnaColor Ember = new(255, 72, 96);
        public static readonly XnaColor Mint = new(72, 220, 140);
        public static readonly XnaColor Amber = new(255, 196, 64);
        public static readonly XnaColor Snow = new(245, 242, 255);
        public static readonly XnaColor Mist = new(180, 172, 210);
        public static readonly XnaColor Fog = new(120, 112, 150);
        public static readonly XnaColor Ash = new(80, 74, 100);
        public static readonly XnaColor Black = new(0, 0, 0);
        public static readonly XnaColor White = new(255, 255, 255);
    }

    // -------------------------------------------------------------------------
    // SEMANTIC COLORS
    // -------------------------------------------------------------------------
    public static class Semantic
    {
        public static readonly XnaColor Background = Palette.Void;
        public static readonly XnaColor BackgroundAlt = Palette.Night;
        public static readonly XnaColor Surface = new(22, 20, 42, 230);
        public static readonly XnaColor SurfaceElevated = new(32, 28, 58, 240);
        public static readonly XnaColor SurfaceGlass = new(40, 36, 72, 180);
        public static readonly XnaColor SurfaceSunken = Palette.Indigo;

        public static readonly XnaColor Primary = Palette.Violet;
        public static readonly XnaColor Secondary = Palette.Cyan;
        public static readonly XnaColor Accent = Palette.Gold;

        public static readonly XnaColor Success = Palette.Mint;
        public static readonly XnaColor Warning = Palette.Amber;
        public static readonly XnaColor Error = Palette.Ember;
        public static readonly XnaColor Info = Palette.Cyan;

        public static readonly XnaColor Disabled = Palette.Ash;
        public static readonly XnaColor Focus = new(148, 92, 255, 200);
        public static readonly XnaColor Selection = new(148, 92, 255, 90);
        public static readonly XnaColor Hover = new(148, 92, 255, 40);
        public static readonly XnaColor Pressed = new(148, 92, 255, 120);

        public static readonly XnaColor TextPrimary = Palette.Snow;
        public static readonly XnaColor TextSecondary = Palette.Mist;
        public static readonly XnaColor TextMuted = Palette.Fog;
        public static readonly XnaColor TextDisabled = Palette.Ash;
        public static readonly XnaColor TextOnPrimary = Palette.Snow;
        public static readonly XnaColor TextDanger = Palette.Ember;

        public static readonly XnaColor Border = new(80, 70, 120, 120);
        public static readonly XnaColor BorderStrong = new(148, 92, 255, 200);
        public static readonly XnaColor BorderDanger = new(255, 72, 96, 180);
        public static readonly XnaColor Overlay = new(0, 0, 0, 160);
        public static readonly XnaColor OverlayHeavy = new(0, 0, 0, 210);
        public static readonly XnaColor Scrim = new(0, 0, 0, 180);

        public static readonly XnaColor RarityCommon = new(160, 160, 170);
        public static readonly XnaColor RarityUncommon = new(72, 200, 120);
        public static readonly XnaColor RarityRare = new(72, 140, 255);
        public static readonly XnaColor RarityEpic = new(180, 80, 255);
        public static readonly XnaColor RarityLegendary = new(255, 168, 48);
    }

    // -------------------------------------------------------------------------
    // LEGACY Color aliases — existing scenes (DesignTokens.Color.*)
    // -------------------------------------------------------------------------
    public static class Color
    {
        public static readonly XnaColor VoidBlack = Semantic.Background;
        public static readonly XnaColor DeepNight = Semantic.BackgroundAlt;
        public static readonly XnaColor ShadowIndigo = Semantic.SurfaceSunken;
        public static readonly XnaColor PanelBase = Semantic.Surface;
        public static readonly XnaColor PanelElevated = Semantic.SurfaceElevated;
        public static readonly XnaColor PanelGlass = Semantic.SurfaceGlass;

        public static readonly XnaColor AccentPrimary = Semantic.Primary;
        public static readonly XnaColor AccentSecondary = Semantic.Secondary;
        public static readonly XnaColor AccentTertiary = Semantic.Accent;
        public static readonly XnaColor AccentDanger = Semantic.Error;
        public static readonly XnaColor AccentSuccess = Semantic.Success;
        public static readonly XnaColor AccentWarning = Semantic.Warning;

        public static readonly XnaColor TextPrimary = Semantic.TextPrimary;
        public static readonly XnaColor TextSecondary = Semantic.TextSecondary;
        public static readonly XnaColor TextMuted = Semantic.TextMuted;
        public static readonly XnaColor TextDisabled = Semantic.TextDisabled;

        public static readonly XnaColor GlowPrimary = new(148, 92, 255, 80);
        public static readonly XnaColor GlowSecondary = new(72, 196, 255, 60);
        public static readonly XnaColor BorderSubtle = Semantic.Border;
        public static readonly XnaColor BorderFocus = Semantic.BorderStrong;
        public static readonly XnaColor OverlayDim = Semantic.Overlay;
        public static readonly XnaColor OverlayHeavy = Semantic.OverlayHeavy;

        public static readonly XnaColor RarityCommon = Semantic.RarityCommon;
        public static readonly XnaColor RarityUncommon = Semantic.RarityUncommon;
        public static readonly XnaColor RarityRare = Semantic.RarityRare;
        public static readonly XnaColor RarityEpic = Semantic.RarityEpic;
        public static readonly XnaColor RarityLegendary = Semantic.RarityLegendary;
    }

    // -------------------------------------------------------------------------
    // TYPOGRAPHY
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
    // SPACING
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
    // RADIUS
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
    // BORDER
    // -------------------------------------------------------------------------
    public static class Border
    {
        public const int Thin = 1;
        public const int Default = 2;
        public const int Thick = 3;
        public const int Focus = 2;
        public static readonly XnaColor Subtle = Semantic.Border;
        public static readonly XnaColor Strong = Semantic.BorderStrong;
        public static readonly XnaColor Danger = Semantic.BorderDanger;
    }

    // -------------------------------------------------------------------------
    // SHADOW / GLOW / OPACITY
    // -------------------------------------------------------------------------
    public static class Shadow
    {
        public static readonly XnaColor Soft = new(0, 0, 0, 80);
        public static readonly XnaColor Medium = new(0, 0, 0, 120);
        public static readonly XnaColor Hard = new(0, 0, 0, 180);
        public const float OffsetY = 4f;
        public const float Spread = 8f;
    }

    public static class Glow
    {
        public static readonly XnaColor Primary = new(148, 92, 255, 80);
        public static readonly XnaColor Secondary = new(72, 196, 255, 60);
        public static readonly XnaColor Accent = new(255, 168, 72, 70);
        public static readonly XnaColor Danger = new(255, 72, 96, 70);
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
    // DEPTH
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
    // MOTION
    // -------------------------------------------------------------------------
    public static class Motion
    {
        public const float Instant = 0.05f;
        public const float Fast = 0.12f;
        public const float Normal = 0.22f;
        public const float Slow = 0.38f;
        public const float Cinematic = 0.65f;
        public const float Epic = 1.15f;

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
    // LAYOUT
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
    // COMPONENT DEFAULTS
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
