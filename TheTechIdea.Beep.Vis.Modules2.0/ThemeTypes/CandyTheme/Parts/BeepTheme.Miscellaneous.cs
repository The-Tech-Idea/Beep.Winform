using System;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Miscellaneous, Utility, and General Properties

        public string FontFamily { get; set; } = "Comic Sans MS";
        public float FontSizeBlockHeader { get; set; } = 24f;
        public float FontSizeBlockText { get; set; } = 14f;
        public float FontSizeQuestion { get; set; } = 16f;
        public float FontSizeAnswer { get; set; } = 14f;
        public float FontSizeCaption { get; set; } = 12f;
        public float FontSizeButton { get; set; } = 14f;

        public FontStyle FontStyleRegular { get; set; } = FontStyle.Regular;
        public FontStyle FontStyleBold { get; set; } = FontStyle.Bold;
        public FontStyle FontStyleItalic { get; set; } = FontStyle.Italic;

        // Candy theme text colors
        public Color PrimaryTextColor { get; set; } = Color.FromArgb(44, 62, 80);        // Navy
        public Color SecondaryTextColor { get; set; } = Color.FromArgb(127, 255, 212);   // Mint
        public Color AccentTextColor { get; set; } = Color.FromArgb(240, 100, 180);      // Candy Pink

        // Modern spacings and radii for "soft" candy look
        public int PaddingSmall { get; set; } = 4;
        public int PaddingMedium { get; set; } = 8;
        public int PaddingLarge { get; set; } = 16;
        public int BorderRadius { get; set; } = 12;
        public int BorderSize { get; set; } = 1;

        public string IconSet { get; set; } = "Candy"; // e.g., a custom pastel icon set name
        public bool ApplyThemeToIcons { get; set; } = true;

        public Color ShadowColor { get; set; } = Color.FromArgb(90, 240, 100, 180); // Semi-transparent Candy Pink
        public float ShadowOpacity { get; set; } = 0.35f;

        // Animations: modern, snappy, but playful
        public double AnimationDurationShort { get; set; } = 0.15;
        public double AnimationDurationMedium { get; set; } = 0.3;
        public double AnimationDurationLong { get; set; } = 0.6;
        public string AnimationEasingFunction { get; set; } = "easeOutCubic";

        public bool HighContrastMode { get; set; } = false;
        public Color FocusIndicatorColor { get; set; } = Color.FromArgb(255, 223, 93); // Lemon (bright and visible)
        public bool IsDarkTheme { get; set; } = false;
    }
}
