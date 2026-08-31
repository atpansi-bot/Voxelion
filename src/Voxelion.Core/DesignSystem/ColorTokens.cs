using Microsoft.Xna.Framework;

namespace Voxelion.Core.DesignSystem
{
    /// <summary>
    /// VOXELION Design Language — Color Tokens.
    /// Original cinematic fantasy palette with magical light accents.
    /// Never imitate existing commercial game UI identities.
    /// </summary>
    public static class ColorTokens
    {
        // Primary Brand
        public static readonly Color EmblemCore = new Color(0xE8, 0xD5, 0xA3);       // Warm celestial gold
        public static readonly Color EmblemGlow = new Color(0xF4, 0xE8, 0xC1, 180);
        public static readonly Color BrandPrimary = new Color(0xC9, 0xA2, 0x27);     // Deep amber gold
        public static readonly Color BrandSecondary = new Color(0x6B, 0x4E, 0x71);   // Mystic violet

        // Backgrounds — dark cinematic
        public static readonly Color BgDeep = new Color(0x0A, 0x0C, 0x12);           // Near black with blue undertone
        public static readonly Color BgPanel = new Color(0x12, 0x16, 0x22, 220);     // Translucent layered panel
        public static readonly Color BgPanelSolid = new Color(0x14, 0x18, 0x24);
        public static readonly Color BgOverlay = new Color(0x08, 0x0A, 0x10, 200);
        public static readonly Color BgHub = new Color(0x0D, 0x11, 0x1A);

        // Text
        public static readonly Color TextPrimary = new Color(0xF5, 0xF0, 0xE6);      // Warm off-white
        public static readonly Color TextSecondary = new Color(0xB8, 0xB0, 0xA0);
        public static readonly Color TextMuted = new Color(0x7A, 0x72, 0x68);
        public static readonly Color TextAccent = new Color(0xE8, 0xD5, 0xA3);
        public static readonly Color TextError = new Color(0xE0, 0x6C, 0x75);
        public static readonly Color TextSuccess = new Color(0x7E, 0xC8, 0xA0);
        public static readonly Color TextWarning = new Color(0xE8, 0xB8, 0x6D);

        // Interactive
        public static readonly Color ButtonPrimary = new Color(0xC9, 0xA2, 0x27);
        public static readonly Color ButtonPrimaryHover = new Color(0xE0, 0xB8, 0x3A);
        public static readonly Color ButtonPrimaryPressed = new Color(0xA8, 0x85, 0x1C);
        public static readonly Color ButtonPrimaryDisabled = new Color(0x5A, 0x4A, 0x20, 160);
        public static readonly Color ButtonSecondary = new Color(0x2A, 0x30, 0x42, 220);
        public static readonly Color ButtonSecondaryHover = new Color(0x3A, 0x42, 0x58, 240);
        public static readonly Color ButtonDanger = new Color(0xA0, 0x3C, 0x48);
        public static readonly Color ButtonDangerHover = new Color(0xC0, 0x4C, 0x58);

        // Borders & Accents
        public static readonly Color BorderSubtle = new Color(0x3A, 0x42, 0x58, 120);
        public static readonly Color BorderFocus = new Color(0xE8, 0xD5, 0xA3, 200);
        public static readonly Color BorderActive = new Color(0xC9, 0xA2, 0x27);
        public static readonly Color GlowSoft = new Color(0xE8, 0xD5, 0xA3, 40);
        public static readonly Color GlowStrong = new Color(0xE8, 0xD5, 0xA3, 90);
        public static readonly Color ParticleLight = new Color(0xF4, 0xE8, 0xC1, 180);
        public static readonly Color CrystalProgress = new Color(0xA8, 0xD4, 0xE8);
        public static readonly Color CrystalProgressFill = new Color(0x7E, 0xC8, 0xE0);

        // Status
        public static readonly Color StatusOnline = new Color(0x7E, 0xC8, 0xA0);
        public static readonly Color StatusAway = new Color(0xE8, 0xB8, 0x6D);
        public static readonly Color StatusOffline = new Color(0x7A, 0x72, 0x68);
        public static readonly Color StatusInWorld = new Color(0xA8, 0xD4, 0xE8);
        public static readonly Color StatusInHub = new Color(0xC9, 0xA2, 0x27);

        // HUD
        public static readonly Color HudHealth = new Color(0xC0, 0x5A, 0x5A);
        public static readonly Color HudHealthBg = new Color(0x2A, 0x18, 0x18, 180);
        public static readonly Color HudMana = new Color(0x5A, 0x7A, 0xC0);
        public static readonly Color HudSlot = new Color(0x1A, 0x1E, 0x28, 200);
        public static readonly Color HudSlotSelected = new Color(0xC9, 0xA2, 0x27, 80);

        // Rarity (for inventory)
        public static readonly Color RarityCommon = new Color(0xB8, 0xB0, 0xA0);
        public static readonly Color RarityUncommon = new Color(0x7E, 0xC8, 0xA0);
        public static readonly Color RarityRare = new Color(0x5A, 0x9A, 0xE0);
        public static readonly Color RarityEpic = new Color(0xA0, 0x6A, 0xC8);
        public static readonly Color RarityLegendary = new Color(0xE8, 0xB8, 0x4A);

        // Safe defaults
        public static Color WithAlpha(Color c, byte a) => new Color(c.R, c.G, c.B, a);
    }
}
