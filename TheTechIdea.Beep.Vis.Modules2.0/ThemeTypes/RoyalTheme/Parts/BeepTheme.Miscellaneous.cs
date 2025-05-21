using System;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Miscellaneous, Utility, and General Properties
        public string FontFamily { get; set; } = "Times New Roman";
        public float FontSizeBlockHeader { get; set; } = 24f;
        public float FontSizeBlockText { get; set; } = 14f;
        public float FontSizeQuestion { get; set; } = 16f;
        public float FontSizeAnswer { get; set; } = 14f;
        public float FontSizeCaption { get; set; } = 12f;
        public float FontSizeButton { get; set; } = 14f;
        public FontStyle FontStyleRegular { get; set; } = FontStyle.Regular;
        public FontStyle FontStyleBold { get; set; } = FontStyle.Bold;
        public FontStyle FontStyleItalic { get; set; } = FontStyle.Italic;
        public Color PrimaryTextColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color SecondaryTextColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color AccentTextColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public int PaddingSmall { get; set; } = 8;
        public int PaddingMedium { get; set; } = 16;
        public int PaddingLarge { get; set; } = 24;
        public int BorderRadius { get; set; } = 6;
        public int BorderSize { get; set; } = 1;
        public string IconSet { get; set; } = "RoyalIcons";
        public bool ApplyThemeToIcons { get; set; } = true;
        public Color ShadowColor { get; set; } = Color.FromArgb(128, 0, 0, 0); // Black with opacity
        public float ShadowOpacity { get; set; } = 0.3f;
        public double AnimationDurationShort { get; set; } = 0.2;
        public double AnimationDurationMedium { get; set; } = 0.4;
        public double AnimationDurationLong { get; set; } = 0.6;
        public string AnimationEasingFunction { get; set; } = "ease-in-out";
        public bool HighContrastMode { get; set; } = false;
        public Color FocusIndicatorColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public bool IsDarkTheme { get; set; } = false;
    }
}