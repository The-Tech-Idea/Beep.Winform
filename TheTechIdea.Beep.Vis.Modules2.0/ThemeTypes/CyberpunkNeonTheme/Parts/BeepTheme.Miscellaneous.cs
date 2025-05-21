using System;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Miscellaneous, Utility, and General Properties

        public string FontFamily { get; set; } = "Consolas";           // Digital/monospace
        public float FontSizeBlockHeader { get; set; } = 24f;
        public float FontSizeBlockText { get; set; } = 14f;
        public float FontSizeQuestion { get; set; } = 16f;
        public float FontSizeAnswer { get; set; } = 14f;
        public float FontSizeCaption { get; set; } = 12f;
        public float FontSizeButton { get; set; } = 14f;

        public FontStyle FontStyleRegular { get; set; } = FontStyle.Regular;
        public FontStyle FontStyleBold { get; set; } = FontStyle.Bold;
        public FontStyle FontStyleItalic { get; set; } = FontStyle.Italic;

        public Color PrimaryTextColor { get; set; } = Color.FromArgb(0, 255, 255);          // Neon Cyan
        public Color SecondaryTextColor { get; set; } = Color.FromArgb(255, 0, 255);        // Neon Magenta
        public Color AccentTextColor { get; set; } = Color.FromArgb(255, 255, 0);           // Neon Yellow

        public int PaddingSmall { get; set; } = 6;
        public int PaddingMedium { get; set; } = 12;
        public int PaddingLarge { get; set; } = 20;
        public int BorderRadius { get; set; } = 8;
        public int BorderSize { get; set; } = 2;

        public string IconSet { get; set; } = "Fluent";
        public bool ApplyThemeToIcons { get; set; } = true;

        public Color ShadowColor { get; set; } = Color.FromArgb(0, 255, 255);               // Neon Cyan shadow
        public float ShadowOpacity { get; set; } = 0.6f;

        public double AnimationDurationShort { get; set; } = 0.15;
        public double AnimationDurationMedium { get; set; } = 0.30;
        public double AnimationDurationLong { get; set; } = 0.65;
        public string AnimationEasingFunction { get; set; } = "cubic-bezier(0.68, -0.55, 0.27, 1.55)"; // Snappy cyber feel

        public bool HighContrastMode { get; set; } = true;

        public Color FocusIndicatorColor { get; set; } = Color.FromArgb(0, 255, 128);       // Neon Green for focus
        public bool IsDarkTheme { get; set; } = true;
    }
}
