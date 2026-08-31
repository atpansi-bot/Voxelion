using Microsoft.Xna.Framework.Graphics;

namespace Voxelion.Core.DesignSystem
{
    /// <summary>
    /// VOXELION Typography System.
    /// Crisp hierarchy for cinematic fantasy UI.
    /// Fonts loaded via Content Pipeline or runtime; scale is resolution-independent.
    /// </summary>
    public static class Typography
    {
        public enum Style
        {
            DisplayHero,      // Title logo / major headlines
            DisplayLarge,     // Screen titles
            Heading1,         // Section headers
            Heading2,         // Subsection
            BodyLarge,        // Primary readable body
            Body,             // Standard body
            BodySmall,        // Secondary / captions
            Label,            // Buttons, tabs, chips
            Micro,            // Tiny metadata, version numbers
            Mono              // Code-like / IDs if needed
        }

        public static float GetScale(Style style)
        {
            return style switch
            {
                Style.DisplayHero => 2.8f,
                Style.DisplayLarge => 2.0f,
                Style.Heading1 => 1.5f,
                Style.Heading2 => 1.25f,
                Style.BodyLarge => 1.1f,
                Style.Body => 1.0f,
                Style.BodySmall => 0.85f,
                Style.Label => 0.95f,
                Style.Micro => 0.7f,
                Style.Mono => 0.9f,
                _ => 1.0f
            };
        }

        public static float GetLineHeight(Style style)
        {
            return style switch
            {
                Style.DisplayHero => 1.15f,
                Style.DisplayLarge => 1.2f,
                Style.Heading1 => 1.25f,
                Style.Heading2 => 1.3f,
                Style.BodyLarge => 1.4f,
                Style.Body => 1.45f,
                Style.BodySmall => 1.4f,
                Style.Label => 1.2f,
                Style.Micro => 1.3f,
                Style.Mono => 1.3f,
                _ => 1.4f
            };
        }

        // Runtime font references (populated by FontManager)
        public static SpriteFont FontRegular { get; set; }
        public static SpriteFont FontBold { get; set; }
        public static SpriteFont FontLight { get; set; }
        public static SpriteFont FontDisplay { get; set; }

        public static SpriteFont Resolve(Style style)
        {
            return style switch
            {
                Style.DisplayHero or Style.DisplayLarge => FontDisplay ?? FontBold ?? FontRegular,
                Style.Heading1 or Style.Heading2 => FontBold ?? FontRegular,
                Style.Label => FontBold ?? FontRegular,
                Style.Mono => FontRegular,
                _ => FontRegular
            };
        }
    }
}
