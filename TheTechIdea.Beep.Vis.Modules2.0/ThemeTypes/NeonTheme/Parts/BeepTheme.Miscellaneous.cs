using System;
using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Miscellaneous, Utility, and General Properties
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public string FontFamily { get; set; } = "Roboto"; // Default font for theme
        public float FontSizeBlockHeader { get; set; } = 24f; // Large headers
        public float FontSizeBlockText { get; set; } = 14f; // Body text
        public float FontSizeQuestion { get; set; } = 16f; // Questions in dialogs
        public float FontSizeAnswer { get; set; } = 14f; // Answers in dialogs
        public float FontSizeCaption { get; set; } = 12f; // Captions
        public float FontSizeButton { get; set; } = 14f; // Button text
        public FontStyle FontStyleRegular { get; set; } = FontStyle.Regular;
        public FontStyle FontStyleBold { get; set; } = FontStyle.Bold;
        public FontStyle FontStyleItalic { get; set; } = FontStyle.Italic;
        public Color PrimaryTextColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for primary text
        public Color SecondaryTextColor { get; set; } = Color.FromArgb(100, 100, 120); // Muted gray-blue for secondary text
        public Color AccentTextColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for accents
        public int PaddingSmall { get; set; } = 4; // Small padding in pixels
        public int PaddingMedium { get; set; } = 8; // Medium padding in pixels
        public int PaddingLarge { get; set; } = 16; // Large padding in pixels
        public int BorderRadius { get; set; } = 4; // Rounded corners in pixels
        public int BorderSize { get; set; } = 1; // Border thickness in pixels
        public string IconSet { get; set; } = "Material Icons"; // Default icon set
        public bool ApplyThemeToIcons { get; set; } = true; // Apply neon colors to icons
        public Color ShadowColor { get; set; } = Color.FromArgb(120, 26, 188, 156); // Semi-transparent neon turquoise for shadows
        public float ShadowOpacity { get; set; } = 0.3f; // 30% opacity for shadows
        public double AnimationDurationShort { get; set; } = 0.2; // 200ms for short animations
        public double AnimationDurationMedium { get; set; } = 0.4; // 400ms for medium animations
        public double AnimationDurationLong { get; set; } = 0.6; // 600ms for long animations
        public string AnimationEasingFunction { get; set; } = "EaseInOut"; // Smooth easing for animations
        public bool HighContrastMode { get; set; } = false; // Disabled by default
        public Color FocusIndicatorColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for focus indicators
        public bool IsDarkTheme { get; set; } = true; // Neon theme is dark
    }
}