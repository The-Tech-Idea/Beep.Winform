using System;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
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
        public Color PrimaryTextColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color SecondaryTextColor { get; set; } = Color.FromArgb(100, 100, 100);
        public Color AccentTextColor { get; set; } = Color.FromArgb(60, 179, 113);
        public int PaddingSmall { get; set; } = 4;
        public int PaddingMedium { get; set; } = 8;
        public int PaddingLarge { get; set; } = 16;
        public int BorderRadius { get; set; } = 4;
        public int BorderSize { get; set; } = 1;
        public string IconSet { get; set; } = "Material Icons";
        public bool ApplyThemeToIcons { get; set; } = true;
        public Color ShadowColor { get; set; } = Color.FromArgb(50, 0, 0, 0);
        public float ShadowOpacity { get; set; } = 0.3f;
        public double AnimationDurationShort { get; set; } = 0.2;
        public double AnimationDurationMedium { get; set; } = 0.4;
        public double AnimationDurationLong { get; set; } = 0.6;
        public string AnimationEasingFunction { get; set; } = "ease-in-out";
        public bool HighContrastMode { get; set; } = false;
        public Color FocusIndicatorColor { get; set; } = Color.FromArgb(50, 205, 50);
        public bool IsDarkTheme { get; set; } = false;
    }
}