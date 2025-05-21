using System;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Miscellaneous, Utility, and General Properties
        public string FontFamily { get; set; } = "Courier New";
        public float FontSizeBlockHeader { get; set; } = 18f;
        public float FontSizeBlockText { get; set; } = 12f;
        public float FontSizeQuestion { get; set; } = 14f;
        public float FontSizeAnswer { get; set; } = 12f;
        public float FontSizeCaption { get; set; } = 10f;
        public float FontSizeButton { get; set; } = 12f;
        public FontStyle FontStyleRegular { get; set; } = FontStyle.Regular;
        public FontStyle FontStyleBold { get; set; } = FontStyle.Bold;
        public FontStyle FontStyleItalic { get; set; } = FontStyle.Italic;
        public Color PrimaryTextColor { get; set; } = Color.White;
        public Color SecondaryTextColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color AccentTextColor { get; set; } = Color.FromArgb(255, 165, 0);
        public int PaddingSmall { get; set; } = 4;
        public int PaddingMedium { get; set; } = 8;
        public int PaddingLarge { get; set; } = 16;
        public int BorderRadius { get; set; } = 4;
        public int BorderSize { get; set; } = 1;
        public string IconSet { get; set; } = "RetroIcons";
        public bool ApplyThemeToIcons { get; set; } = true;
        public Color ShadowColor { get; set; } = Color.FromArgb(128, 0, 0, 0);
        public float ShadowOpacity { get; set; } = 0.5f;
        public double AnimationDurationShort { get; set; } = 0.2;
        public double AnimationDurationMedium { get; set; } = 0.4;
        public double AnimationDurationLong { get; set; } = 0.8;
        public string AnimationEasingFunction { get; set; } = "ease-in-out";
        public bool HighContrastMode { get; set; } = false;
        public Color FocusIndicatorColor { get; set; } = Color.FromArgb(255, 165, 0);
        public bool IsDarkTheme { get; set; } = true;
    }
}