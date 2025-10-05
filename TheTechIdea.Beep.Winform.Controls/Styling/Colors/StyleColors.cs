using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.Colors
{
    /// <summary>
    /// Centralized color definitions for all BeepControlStyle design systems
    /// Each style has distinct, recognizable colors
    /// </summary>
    public static class StyleColors
    {
        /// <summary>
        /// Get background color for a specific style
        /// </summary>
        public static Color GetBackground(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => Color.FromArgb(255, 251, 254),        // Soft lavender tonal surface
                BeepControlStyle.iOS15 => Color.FromArgb(242, 242, 247),            // Light gray with blue tint
                BeepControlStyle.Fluent2 => Color.FromArgb(243, 242, 241),          // Warm neutral gray
                BeepControlStyle.Minimal => Color.FromArgb(250, 250, 250),          // Pure light gray
                BeepControlStyle.AntDesign => Color.FromArgb(250, 250, 250),        // Slightly warm white
                BeepControlStyle.MaterialYou => Color.FromArgb(255, 248, 250),      // Pink-tinted surface
                BeepControlStyle.Windows11Mica => Color.FromArgb(248, 248, 248),    // Cool gray
                BeepControlStyle.MacOSBigSur => Color.FromArgb(252, 252, 252),      // Clean white with warmth
                BeepControlStyle.ChakraUI => Color.FromArgb(247, 250, 252),         // Soft blue-gray
                BeepControlStyle.TailwindCard => Color.FromArgb(255, 255, 255),     // Pure white
                BeepControlStyle.NotionMinimal => Color.FromArgb(251, 251, 250),    // Off-white with warmth
                BeepControlStyle.VercelClean => Color.FromArgb(255, 255, 255),      // Pure white
                BeepControlStyle.StripeDashboard => Color.FromArgb(248, 250, 252),  // Light blue-gray
                BeepControlStyle.DarkGlow => Color.FromArgb(24, 24, 27),            // Deep charcoal
                BeepControlStyle.DiscordStyle => Color.FromArgb(47, 49, 54),        // Dark slate gray
                BeepControlStyle.GradientModern => Color.FromArgb(58, 123, 213),    // Blue (gradient start)
                BeepControlStyle.GlassAcrylic => Color.FromArgb(240, 255, 255, 255), // Translucent white
                BeepControlStyle.Neumorphism => Color.FromArgb(225, 227, 230),      // Soft gray
                BeepControlStyle.Bootstrap => Color.FromArgb(255, 255, 255),        // White
                BeepControlStyle.FigmaCard => Color.FromArgb(255, 255, 255),        // Pure white
                BeepControlStyle.PillRail => Color.FromArgb(245, 245, 247),         // Light gray-blue
                _ => Color.FromArgb(250, 250, 250)
            };
        }

        /// <summary>
        /// Get primary/accent color for a specific style
        /// </summary>
        public static Color GetPrimary(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => Color.FromArgb(103, 80, 164),         // Material Purple
                BeepControlStyle.iOS15 => Color.FromArgb(0, 122, 255),              // iOS Blue
                BeepControlStyle.Fluent2 => Color.FromArgb(0, 120, 212),            // Fluent Blue
                BeepControlStyle.Minimal => Color.FromArgb(64, 64, 64),             // Dark Gray
                BeepControlStyle.AntDesign => Color.FromArgb(24, 144, 255),         // Ant Blue
                BeepControlStyle.MaterialYou => Color.FromArgb(208, 188, 255),      // Dynamic Purple
                BeepControlStyle.Windows11Mica => Color.FromArgb(0, 120, 212),      // Windows Blue
                BeepControlStyle.MacOSBigSur => Color.FromArgb(0, 122, 255),        // macOS Blue
                BeepControlStyle.ChakraUI => Color.FromArgb(56, 178, 172),          // Teal
                BeepControlStyle.TailwindCard => Color.FromArgb(59, 130, 246),      // Tailwind Blue
                BeepControlStyle.NotionMinimal => Color.FromArgb(55, 53, 47),       // Notion Dark Gray
                BeepControlStyle.VercelClean => Color.FromArgb(0, 0, 0),            // Black
                BeepControlStyle.StripeDashboard => Color.FromArgb(99, 91, 255),    // Stripe Purple
                BeepControlStyle.DarkGlow => Color.FromArgb(139, 92, 246),          // Neon Purple
                BeepControlStyle.DiscordStyle => Color.FromArgb(88, 101, 242),      // Discord Blurple
                BeepControlStyle.GradientModern => Color.FromArgb(0, 210, 255),     // Cyan
                BeepControlStyle.GlassAcrylic => Color.FromArgb(0, 120, 215),       // Blue
                BeepControlStyle.Neumorphism => Color.FromArgb(100, 100, 150),      // Muted Blue-Gray
                BeepControlStyle.Bootstrap => Color.FromArgb(13, 110, 253),         // Bootstrap Primary Blue
                BeepControlStyle.FigmaCard => Color.FromArgb(24, 160, 251),         // Figma Blue
                BeepControlStyle.PillRail => Color.FromArgb(0, 122, 255),           // iOS Blue
                _ => Color.FromArgb(0, 120, 215)
            };
        }

        /// <summary>
        /// Get secondary/surface color for a specific style
        /// </summary>
        public static Color GetSecondary(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => Color.FromArgb(234, 221, 255),        // Light purple container
                BeepControlStyle.iOS15 => Color.FromArgb(229, 229, 234),            // Light gray
                BeepControlStyle.Fluent2 => Color.FromArgb(237, 235, 233),          // Warm gray
                BeepControlStyle.Minimal => Color.FromArgb(240, 240, 240),          // Light gray
                BeepControlStyle.AntDesign => Color.FromArgb(230, 247, 255),        // Light blue
                BeepControlStyle.MaterialYou => Color.FromArgb(255, 216, 228),      // Light pink
                BeepControlStyle.Windows11Mica => Color.FromArgb(243, 242, 241),    // Mica gray
                BeepControlStyle.MacOSBigSur => Color.FromArgb(242, 242, 247),      // Light gray
                BeepControlStyle.ChakraUI => Color.FromArgb(237, 242, 247),         // Light blue-gray
                BeepControlStyle.TailwindCard => Color.FromArgb(249, 250, 251),     // Very light gray
                BeepControlStyle.NotionMinimal => Color.FromArgb(247, 247, 245),    // Cream
                BeepControlStyle.VercelClean => Color.FromArgb(250, 250, 250),      // Off-white
                BeepControlStyle.StripeDashboard => Color.FromArgb(242, 244, 248),  // Very light blue
                BeepControlStyle.DarkGlow => Color.FromArgb(39, 39, 42),            // Dark gray
                BeepControlStyle.DiscordStyle => Color.FromArgb(32, 34, 37),        // Darker gray
                BeepControlStyle.GradientModern => Color.FromArgb(99, 179, 237),    // Light blue
                BeepControlStyle.GlassAcrylic => Color.FromArgb(200, 255, 255, 255), // Translucent white
                BeepControlStyle.Neumorphism => Color.FromArgb(235, 237, 240),      // Very light gray
                BeepControlStyle.Bootstrap => Color.FromArgb(108, 117, 125),        // Bootstrap Secondary
                BeepControlStyle.FigmaCard => Color.FromArgb(242, 242, 242),        // Light gray
                BeepControlStyle.PillRail => Color.FromArgb(209, 213, 219),         // Medium gray
                _ => Color.FromArgb(240, 240, 240)
            };
        }

        /// <summary>
        /// Get text/foreground color for a specific style
        /// </summary>
        public static Color GetForeground(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => Color.FromArgb(28, 27, 31),           // On-surface
                BeepControlStyle.iOS15 => Color.FromArgb(60, 60, 67),               // Label
                BeepControlStyle.Fluent2 => Color.FromArgb(50, 49, 48),             // Neutral primary
                BeepControlStyle.Minimal => Color.FromArgb(64, 64, 64),             // Dark gray
                BeepControlStyle.AntDesign => Color.FromArgb(0, 0, 0, 85),          // 85% black
                BeepControlStyle.MaterialYou => Color.FromArgb(73, 69, 79),         // On-primary-container
                BeepControlStyle.Windows11Mica => Color.FromArgb(50, 49, 48),       // Text primary
                BeepControlStyle.MacOSBigSur => Color.FromArgb(0, 0, 0),            // Label
                BeepControlStyle.ChakraUI => Color.FromArgb(26, 32, 44),            // Gray 800
                BeepControlStyle.TailwindCard => Color.FromArgb(17, 24, 39),        // Gray 900
                BeepControlStyle.NotionMinimal => Color.FromArgb(55, 53, 47),       // Default
                BeepControlStyle.VercelClean => Color.FromArgb(0, 0, 0),            // Black
                BeepControlStyle.StripeDashboard => Color.FromArgb(30, 35, 48),     // Midnight blue
                BeepControlStyle.DarkGlow => Color.FromArgb(248, 250, 252),         // Light text
                BeepControlStyle.DiscordStyle => Color.FromArgb(220, 221, 222),     // Light gray text
                BeepControlStyle.GradientModern => Color.FromArgb(255, 255, 255),   // White
                BeepControlStyle.GlassAcrylic => Color.FromArgb(0, 0, 0),           // Black
                BeepControlStyle.Neumorphism => Color.FromArgb(80, 80, 80),         // Dark gray
                BeepControlStyle.Bootstrap => Color.FromArgb(33, 37, 41),           // Dark
                BeepControlStyle.FigmaCard => Color.FromArgb(0, 0, 0),              // Black
                BeepControlStyle.PillRail => Color.FromArgb(55, 65, 81),            // Gray 700
                _ => Color.FromArgb(50, 50, 50)
            };
        }

        /// <summary>
        /// Get border color for a specific style
        /// </summary>
        public static Color GetBorder(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => Color.FromArgb(121, 116, 126),        // Outline
                BeepControlStyle.iOS15 => Color.FromArgb(60, 60, 67, 60),           // Separator (opacity)
                BeepControlStyle.Fluent2 => Color.FromArgb(237, 235, 233),          // Divider
                BeepControlStyle.Minimal => Color.FromArgb(224, 224, 224),          // Light gray
                BeepControlStyle.AntDesign => Color.FromArgb(217, 217, 217),        // Border base
                BeepControlStyle.MaterialYou => Color.FromArgb(121, 116, 126),      // Outline variant
                BeepControlStyle.Windows11Mica => Color.FromArgb(229, 229, 229),    // Stroke subtle
                BeepControlStyle.MacOSBigSur => Color.FromArgb(216, 216, 216),      // Separator
                BeepControlStyle.ChakraUI => Color.FromArgb(226, 232, 240),         // Gray 300
                BeepControlStyle.TailwindCard => Color.FromArgb(229, 231, 235),     // Gray 200
                BeepControlStyle.NotionMinimal => Color.FromArgb(233, 233, 231),    // Border
                BeepControlStyle.VercelClean => Color.FromArgb(234, 234, 234),      // Gray border
                BeepControlStyle.StripeDashboard => Color.FromArgb(229, 231, 235),  // Border
                BeepControlStyle.DarkGlow => Color.FromArgb(63, 63, 70),            // Zinc 700
                BeepControlStyle.DiscordStyle => Color.FromArgb(32, 34, 37),        // Background tertiary
                BeepControlStyle.GradientModern => Color.FromArgb(255, 255, 255, 30), // Translucent white
                BeepControlStyle.GlassAcrylic => Color.FromArgb(100, 255, 255, 255), // Translucent white
                BeepControlStyle.Neumorphism => Color.FromArgb(210, 212, 215),      // Shadow border
                BeepControlStyle.Bootstrap => Color.FromArgb(222, 226, 230),        // Border color
                BeepControlStyle.FigmaCard => Color.FromArgb(229, 229, 229),        // Border
                BeepControlStyle.PillRail => Color.FromArgb(209, 213, 219),         // Gray 300
                _ => Color.FromArgb(200, 200, 200)
            };
        }

        /// <summary>
        /// Get hover/interaction color for a specific style
        /// </summary>
        public static Color GetHover(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => Color.FromArgb(8, 103, 80, 164),      // State overlay
                BeepControlStyle.iOS15 => Color.FromArgb(20, 0, 0, 0),              // 20% black
                BeepControlStyle.Fluent2 => Color.FromArgb(10, 0, 120, 212),        // Subtle blue
                BeepControlStyle.Minimal => Color.FromArgb(245, 245, 245),          // Very light gray
                BeepControlStyle.AntDesign => Color.FromArgb(230, 247, 255),        // Light blue
                BeepControlStyle.MaterialYou => Color.FromArgb(10, 208, 188, 255),  // Transparent purple
                BeepControlStyle.Windows11Mica => Color.FromArgb(243, 242, 241),    // Subtle fill
                BeepControlStyle.MacOSBigSur => Color.FromArgb(0, 0, 0, 10),        // 10% black
                BeepControlStyle.ChakraUI => Color.FromArgb(247, 250, 252),         // Gray 50
                BeepControlStyle.TailwindCard => Color.FromArgb(249, 250, 251),     // Gray 50
                BeepControlStyle.NotionMinimal => Color.FromArgb(247, 247, 245),    // Hover
                BeepControlStyle.VercelClean => Color.FromArgb(250, 250, 250),      // Subtle gray
                BeepControlStyle.StripeDashboard => Color.FromArgb(249, 250, 251),  // Gray 50
                BeepControlStyle.DarkGlow => Color.FromArgb(39, 39, 42),            // Zinc 800
                BeepControlStyle.DiscordStyle => Color.FromArgb(54, 57, 63),        // Background secondary
                BeepControlStyle.GradientModern => Color.FromArgb(30, 255, 255, 255), // Translucent white
                BeepControlStyle.GlassAcrylic => Color.FromArgb(50, 255, 255, 255), // Translucent white
                BeepControlStyle.Neumorphism => Color.FromArgb(230, 232, 235),      // Lighter
                BeepControlStyle.Bootstrap => Color.FromArgb(233, 236, 239),        // Light
                BeepControlStyle.FigmaCard => Color.FromArgb(248, 248, 248),        // Very light gray
                BeepControlStyle.PillRail => Color.FromArgb(229, 231, 235),         // Gray 200
                _ => Color.FromArgb(240, 240, 240)
            };
        }

        /// <summary>
        /// Get selection/active color for a specific style
        /// </summary>
        public static Color GetSelection(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => Color.FromArgb(234, 221, 255),        // Primary container
                BeepControlStyle.iOS15 => Color.FromArgb(200, 0, 122, 255),         // iOS blue semi-transparent
                BeepControlStyle.Fluent2 => Color.FromArgb(243, 242, 241),          // Subtle background
                BeepControlStyle.Minimal => Color.FromArgb(240, 240, 240),          // Light gray
                BeepControlStyle.AntDesign => Color.FromArgb(230, 247, 255),        // Primary 1
                BeepControlStyle.MaterialYou => Color.FromArgb(208, 188, 255),      // Primary container
                BeepControlStyle.Windows11Mica => Color.FromArgb(243, 242, 241),    // Selected fill
                BeepControlStyle.MacOSBigSur => Color.FromArgb(229, 229, 234),      // Selected content background
                BeepControlStyle.ChakraUI => Color.FromArgb(237, 242, 247),         // Blue 50
                BeepControlStyle.TailwindCard => Color.FromArgb(239, 246, 255),     // Blue 50
                BeepControlStyle.NotionMinimal => Color.FromArgb(55, 53, 47, 8),    // Subtle
                BeepControlStyle.VercelClean => Color.FromArgb(0, 0, 0),            // Black
                BeepControlStyle.StripeDashboard => Color.FromArgb(237, 233, 254),  // Purple 100
                BeepControlStyle.DarkGlow => Color.FromArgb(88, 28, 135),           // Purple 900
                BeepControlStyle.DiscordStyle => Color.FromArgb(71, 82, 196),       // Selected blurple
                BeepControlStyle.GradientModern => Color.FromArgb(0, 210, 255),     // Bright cyan
                BeepControlStyle.GlassAcrylic => Color.FromArgb(150, 0, 120, 215),  // Blue transparent
                BeepControlStyle.Neumorphism => Color.FromArgb(220, 222, 225),      // Pressed state
                BeepControlStyle.Bootstrap => Color.FromArgb(13, 110, 253),         // Primary
                BeepControlStyle.FigmaCard => Color.FromArgb(24, 160, 251),         // Figma blue
                BeepControlStyle.PillRail => Color.FromArgb(59, 130, 246),          // Blue 500
                _ => Color.FromArgb(220, 220, 220)
            };
        }
    }
}
