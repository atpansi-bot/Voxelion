namespace Voxelion.Core.DesignSystem
{
    /// <summary>
    /// VOXELION Spacing Scale — 4pt base system for consistent rhythm.
    /// All layout distances derive from these tokens.
    /// </summary>
    public static class Spacing
    {
        public const float Unit = 4f;

        public static float Xs => Unit * 1f;      // 4
        public static float Sm => Unit * 2f;      // 8
        public static float Md => Unit * 3f;      // 12
        public static float Lg => Unit * 4f;      // 16
        public static float Xl => Unit * 6f;      // 24
        public static float Xxl => Unit * 8f;     // 32
        public static float Xxxl => Unit * 12f;   // 48
        public static float Huge => Unit * 16f;   // 64
        public static float Massive => Unit * 24f; // 96

        // Semantic aliases
        public static float PanelPadding => Xl;
        public static float ButtonPaddingX => Xl;
        public static float ButtonPaddingY => Md;
        public static float SectionGap => Xxl;
        public static float CardGap => Lg;
        public static float SafeAreaMin => Xl;
        public static float TouchTargetMin => 48f;
        public static float TouchTargetComfort => 56f;
    }
}
