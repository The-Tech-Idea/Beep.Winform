using System;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Miscellaneous, Utility, and General Properties
        public string FontFamily { get; set; } = "Segoe UI";
        public float FontSizeBlockHeader { get; set; } = 26f;
        public float FontSizeBlockText { get; set; } = 15f;
        public float FontSizeQuestion { get; set; } = 18f;
        public float FontSizeAnswer { get; set; } = 15f;
        public float FontSizeCaption { get; set; } = 13f;
        public float FontSizeButton { get; set; } = 15f;
        public FontStyle FontStyleRegular { get; set; } = FontStyle.Regular;
        public FontStyle FontStyleBold { get; set; } = FontStyle.Bold;
        public FontStyle FontStyleItalic { get; set; } = FontStyle.Italic;
        public Color PrimaryTextColor { get; set; } = Color.FromArgb(34, 49, 34); // dark forest green
        public Color SecondaryTextColor { get; set; } = Color.FromArgb(102, 123, 102); // muted green-gray
        public Color AccentTextColor { get; set; } = Color.FromArgb(76, 175, 80); // medium green accent
        public int PaddingSmall { get; set; } = 4;
        public int PaddingMedium { get; set; } = 8;
        public int PaddingLarge { get; set; } = 16;
        public int BorderRadius { get; set; } = 6;
        public int BorderSize { get; set; } = 2;
        public string IconSet { get; set; } = "ForestIcons";
        public bool ApplyThemeToIcons { get; set; } = true;
        public Color ShadowColor { get; set; } = Color.FromArgb(80, 0, 0, 0);
        public float ShadowOpacity { get; set; } = 0.4f;
        public double AnimationDurationShort { get; set; } = 0.2;
        public double AnimationDurationMedium { get; set; } = 0.5;
        public double AnimationDurationLong { get; set; } = 0.8;
        public string AnimationEasingFunction { get; set; } = "ease-in-out";
        public bool HighContrastMode { get; set; } = false;
        public Color FocusIndicatorColor { get; set; } = Color.FromArgb(102, 187, 106); // light green focus
        public bool IsDarkTheme { get; set; } = true;
    }
}
