using System;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Miscellaneous, Utility, and General Properties with defaults
        public string FontFamily { get; set; } = "Roboto";
        public float FontSizeBlockHeader { get; set; } = 24f;
        public float FontSizeBlockText { get; set; } = 14f;
        public float FontSizeQuestion { get; set; } = 16f;
        public float FontSizeAnswer { get; set; } = 14f;
        public float FontSizeCaption { get; set; } = 12f;
        public float FontSizeButton { get; set; } = 14f;
        public FontStyle FontStyleRegular { get; set; } = FontStyle.Regular;
        public FontStyle FontStyleBold { get; set; } = FontStyle.Bold;
        public FontStyle FontStyleItalic { get; set; } = FontStyle.Italic;

        public Color PrimaryTextColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Gray 900
        public Color SecondaryTextColor { get; set; } = Color.FromArgb(117, 117, 117); // Gray 600
        public Color AccentTextColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500

        public int PaddingSmall { get; set; } = 4;
        public int PaddingMedium { get; set; } = 8;
        public int PaddingLarge { get; set; } = 16;
        public int BorderRadius { get; set; } = 4;
        public int BorderSize { get; set; } = 1;

        public string IconSet { get; set; } = "MaterialIcons";
        public bool ApplyThemeToIcons { get; set; } = true;

        public Color ShadowColor { get; set; } = Color.FromArgb(0, 0, 0); // Black shadow
        public float ShadowOpacity { get; set; } = 0.2f;

        public double AnimationDurationShort { get; set; } = 0.15;
        public double AnimationDurationMedium { get; set; } = 0.3;
        public double AnimationDurationLong { get; set; } = 0.5;

        public string AnimationEasingFunction { get; set; } = "ease-in-out";

        public bool HighContrastMode { get; set; } = false;

        public Color FocusIndicatorColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500

        public bool IsDarkTheme { get; set; } = false;
    }
}
