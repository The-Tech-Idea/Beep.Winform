using System;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Miscellaneous, Utility, and General Properties
        public string FontFamily { get; set; } = "Segoe UI";
        public float FontSizeBlockHeader { get; set; } = 24f;
        public float FontSizeBlockText { get; set; } = 14f;
        public float FontSizeQuestion { get; set; } = 16f;
        public float FontSizeAnswer { get; set; } = 14f;
        public float FontSizeCaption { get; set; } = 12f;
        public float FontSizeButton { get; set; } = 14f;
        public FontStyle FontStyleRegular { get; set; } = FontStyle.Regular;
        public FontStyle FontStyleBold { get; set; } = FontStyle.Bold;
        public FontStyle FontStyleItalic { get; set; } = FontStyle.Italic;
        public Color PrimaryTextColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark gray
        public Color SecondaryTextColor { get; set; } = Color.FromArgb(117, 117, 117); // Medium gray
        public Color AccentTextColor { get; set; } = Color.FromArgb(0, 120, 215); // Bright blue
        public int PaddingSmall { get; set; } = 4;
        public int PaddingMedium { get; set; } = 8;
        public int PaddingLarge { get; set; } = 16;
        public int BorderRadius { get; set; } = 4;
        public int BorderSize { get; set; } = 1;
        public string IconSet { get; set; } = "FlatIcons";
        public bool ApplyThemeToIcons { get; set; } = true;
        public Color ShadowColor { get; set; } = Color.FromArgb(64, 0, 0, 0); // Semi-transparent black
        public float ShadowOpacity { get; set; } = 0.25f;
        public double AnimationDurationShort { get; set; } = 0.15;
        public double AnimationDurationMedium { get; set; } = 0.3;
        public double AnimationDurationLong { get; set; } = 0.5;
        public string AnimationEasingFunction { get; set; } = "ease-in-out";
        public bool HighContrastMode { get; set; } = false;
        public Color FocusIndicatorColor { get; set; } = Color.FromArgb(0, 120, 215);
        public bool IsDarkTheme { get; set; } = false;
    }
}
