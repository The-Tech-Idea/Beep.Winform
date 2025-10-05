using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.Borders
{
    /// <summary>
    /// Border radius and width definitions for all design systems
    /// Each style has distinct border characteristics
    /// </summary>
    public static class StyleBorders
    {
        /// <summary>
        /// Get border radius for a specific style
        /// </summary>
        public static int GetRadius(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => 28,               // Material 3 extra-large
                BeepControlStyle.iOS15 => 10,                   // iOS gentle curve
                BeepControlStyle.Fluent2 => 4,                  // Fluent subtle
                BeepControlStyle.Minimal => 0,                  // No radius
                BeepControlStyle.AntDesign => 2,                // Ant subtle
                BeepControlStyle.MaterialYou => 28,             // Material You large
                BeepControlStyle.Windows11Mica => 4,            // Windows subtle
                BeepControlStyle.MacOSBigSur => 6,              // macOS subtle
                BeepControlStyle.ChakraUI => 6,                 // Chakra standard
                BeepControlStyle.TailwindCard => 8,             // Tailwind rounded-lg
                BeepControlStyle.NotionMinimal => 3,            // Notion very subtle
                BeepControlStyle.VercelClean => 5,              // Vercel subtle
                BeepControlStyle.StripeDashboard => 8,          // Stripe comfortable
                BeepControlStyle.DarkGlow => 12,                // Dark rounded
                BeepControlStyle.DiscordStyle => 8,             // Discord rounded
                BeepControlStyle.GradientModern => 16,          // Modern very rounded
                BeepControlStyle.GlassAcrylic => 12,            // Glass rounded
                BeepControlStyle.Neumorphism => 20,             // Neo very rounded
                BeepControlStyle.Bootstrap => 4,                // Bootstrap subtle
                BeepControlStyle.FigmaCard => 6,                // Figma subtle
                BeepControlStyle.PillRail => 20,                // Pill very rounded
                _ => 4
            };
        }

        /// <summary>
        /// Get selection indicator radius for a specific style
        /// </summary>
        public static int GetSelectionRadius(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => 28,               // Full rounded pill
                BeepControlStyle.iOS15 => 10,                   // iOS rounded
                BeepControlStyle.Fluent2 => 4,                  // Fluent subtle
                BeepControlStyle.Minimal => 0,                  // Square
                BeepControlStyle.AntDesign => 2,                // Very subtle
                BeepControlStyle.MaterialYou => 28,             // Full pill
                BeepControlStyle.Windows11Mica => 4,            // Subtle
                BeepControlStyle.MacOSBigSur => 8,              // Gentle
                BeepControlStyle.ChakraUI => 6,                 // Standard
                BeepControlStyle.TailwindCard => 6,             // Subtle
                BeepControlStyle.NotionMinimal => 3,            // Very subtle
                BeepControlStyle.VercelClean => 0,              // Square
                BeepControlStyle.StripeDashboard => 6,          // Subtle
                BeepControlStyle.DarkGlow => 12,                // Rounded
                BeepControlStyle.DiscordStyle => 8,             // Rounded
                BeepControlStyle.GradientModern => 16,          // Very rounded
                BeepControlStyle.GlassAcrylic => 12,            // Rounded
                BeepControlStyle.Neumorphism => 20,             // Very rounded
                BeepControlStyle.Bootstrap => 4,                // Subtle
                BeepControlStyle.FigmaCard => 6,                // Subtle
                BeepControlStyle.PillRail => 100,               // Full pill
                _ => 4
            };
        }

        /// <summary>
        /// Get border width for a specific style
        /// </summary>
        public static float GetBorderWidth(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => 1.0f,             // Material standard
                BeepControlStyle.iOS15 => 0.5f,                 // iOS thin
                BeepControlStyle.Fluent2 => 1.0f,               // Fluent standard
                BeepControlStyle.Minimal => 1.0f,               // Standard
                BeepControlStyle.AntDesign => 1.0f,             // Standard
                BeepControlStyle.MaterialYou => 0.0f,           // No border (filled)
                BeepControlStyle.Windows11Mica => 1.0f,         // Standard
                BeepControlStyle.MacOSBigSur => 0.5f,           // Thin
                BeepControlStyle.ChakraUI => 1.0f,              // Standard
                BeepControlStyle.TailwindCard => 1.0f,          // Standard
                BeepControlStyle.NotionMinimal => 1.0f,         // Standard
                BeepControlStyle.VercelClean => 1.0f,           // Standard
                BeepControlStyle.StripeDashboard => 1.5f,       // Slightly thicker
                BeepControlStyle.DarkGlow => 1.0f,              // Standard
                BeepControlStyle.DiscordStyle => 0.0f,          // No border
                BeepControlStyle.GradientModern => 0.0f,        // No border (gradient)
                BeepControlStyle.GlassAcrylic => 1.0f,          // Thin glass edge
                BeepControlStyle.Neumorphism => 0.0f,           // No border (shadow defines)
                BeepControlStyle.Bootstrap => 1.0f,             // Standard
                BeepControlStyle.FigmaCard => 1.0f,             // Standard
                BeepControlStyle.PillRail => 0.0f,              // No border
                _ => 1.0f
            };
        }

        /// <summary>
        /// Check if style uses filled/solid backgrounds (vs outlined)
        /// </summary>
        public static bool IsFilled(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => false,            // Outlined by default
                BeepControlStyle.iOS15 => true,                 // Filled
                BeepControlStyle.Fluent2 => true,               // Filled subtle
                BeepControlStyle.Minimal => false,              // Outlined
                BeepControlStyle.AntDesign => false,            // Outlined
                BeepControlStyle.MaterialYou => true,           // Filled containers
                BeepControlStyle.Windows11Mica => true,         // Filled
                BeepControlStyle.MacOSBigSur => true,           // Filled
                BeepControlStyle.ChakraUI => false,             // Outlined
                BeepControlStyle.TailwindCard => false,         // Outlined
                BeepControlStyle.NotionMinimal => false,        // Outlined
                BeepControlStyle.VercelClean => false,          // Outlined
                BeepControlStyle.StripeDashboard => true,       // Filled
                BeepControlStyle.DarkGlow => true,              // Filled dark
                BeepControlStyle.DiscordStyle => true,          // Filled
                BeepControlStyle.GradientModern => true,        // Gradient filled
                BeepControlStyle.GlassAcrylic => true,          // Translucent filled
                BeepControlStyle.Neumorphism => true,           // Soft filled
                BeepControlStyle.Bootstrap => false,            // Outlined
                BeepControlStyle.FigmaCard => false,            // Outlined
                BeepControlStyle.PillRail => true,              // Filled pills
                _ => false
            };
        }

        /// <summary>
        /// Get accent bar width (for Fluent-style left indicators)
        /// </summary>
        public static int GetAccentBarWidth(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Fluent2 => 3,                  // Fluent signature bar
                BeepControlStyle.Windows11Mica => 3,            // Windows bar
                BeepControlStyle.StripeDashboard => 4,          // Stripe thicker
                BeepControlStyle.Bootstrap => 4,                // Bootstrap bar
                _ => 0                                          // No accent bar
            };
        }
    }
}
