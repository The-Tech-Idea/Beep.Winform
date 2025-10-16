using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.Shadows
{
    /// <summary>
    /// Shadow and elevation definitions for all design systems
    /// Each style has distinct shadow characteristics
    /// </summary>
    public static class StyleShadows
    {
        /// <summary>
        /// Check if style uses shadows/elevation
        /// </summary>
        public static bool HasShadow(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => true,             // Material elevation
                BeepControlStyle.iOS15 => false,                // iOS flat with blur
                BeepControlStyle.Fluent2 => true,               // Subtle elevation
                BeepControlStyle.Minimal => false,              // Flat
                BeepControlStyle.AntDesign => true,             // Subtle shadow
                BeepControlStyle.MaterialYou => true,           // Material elevation
                BeepControlStyle.Windows11Mica => false,        // Mica doesn't need shadow
                BeepControlStyle.MacOSBigSur => true,           // macOS subtle shadow
                BeepControlStyle.ChakraUI => true,              // Chakra shadow
                BeepControlStyle.TailwindCard => true,          // Card shadow
                BeepControlStyle.NotionMinimal => false,        // Flat
                BeepControlStyle.VercelClean => false,          // Clean flat
                BeepControlStyle.StripeDashboard => true,       // Stripe elevation
                BeepControlStyle.DarkGlow => true,              // Glow effect
                BeepControlStyle.DiscordStyle => false,         // Flat dark
                BeepControlStyle.GradientModern => true,        // Modern depth
                BeepControlStyle.GlassAcrylic => false,         // Glass doesn't need shadow
                BeepControlStyle.Neumorphism => true,           // Soft shadows crucial
                BeepControlStyle.Bootstrap => true,             // Card shadow
                BeepControlStyle.FigmaCard => true,             // Figma elevation
                BeepControlStyle.PillRail => false,             // Flat pills
                BeepControlStyle.Apple => true,                 // Apple subtle shadow
                BeepControlStyle.Fluent => true,                // Fluent layered shadow
                BeepControlStyle.Material => true,              // Material elevation
                BeepControlStyle.WebFramework => true,          // Web box-shadow
                BeepControlStyle.Effect => true,                // Effect dramatic shadow
                _ => false
            };
        }

        /// <summary>
        /// Get shadow blur radius for a specific style
        /// </summary>
        public static int GetShadowBlur(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => 12,               // Material soft shadow
                BeepControlStyle.Fluent2 => 8,                  // Fluent subtle
                BeepControlStyle.AntDesign => 6,                // Ant subtle
                BeepControlStyle.MaterialYou => 12,             // Material soft
                BeepControlStyle.MacOSBigSur => 10,             // macOS gentle
                BeepControlStyle.ChakraUI => 10,                // Chakra standard
                BeepControlStyle.TailwindCard => 15,            // Tailwind prominent
                BeepControlStyle.StripeDashboard => 20,         // Stripe generous
                BeepControlStyle.DarkGlow => 24,                // Glow large
                BeepControlStyle.GradientModern => 16,          // Modern soft
                BeepControlStyle.Neumorphism => 20,             // Neo soft large
                BeepControlStyle.Bootstrap => 8,                // Bootstrap subtle
                BeepControlStyle.FigmaCard => 10,               // Figma standard
                BeepControlStyle.Apple => 5,                    // Apple very subtle
                BeepControlStyle.Fluent => 8,                   // Fluent subtle
                BeepControlStyle.Material => 12,                // Material soft
                BeepControlStyle.WebFramework => 8,             // Web standard
                BeepControlStyle.Effect => 20,                  // Effect dramatic
                _ => 8
            };
        }

        /// <summary>
        /// Get shadow spread radius for a specific style
        /// </summary>
        public static int GetShadowSpread(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => -2,               // Inset slightly
                BeepControlStyle.Fluent2 => 0,                  // No spread
                BeepControlStyle.AntDesign => 0,                // No spread
                BeepControlStyle.MaterialYou => -2,             // Inset slightly
                BeepControlStyle.MacOSBigSur => 0,              // No spread
                BeepControlStyle.ChakraUI => -1,                // Slight inset
                BeepControlStyle.TailwindCard => -1,            // Slight inset
                BeepControlStyle.StripeDashboard => 0,          // No spread
                BeepControlStyle.DarkGlow => 4,                 // Glow expands
                BeepControlStyle.GradientModern => 0,           // No spread
                BeepControlStyle.Neumorphism => 0,              // No spread (dual shadows)
                BeepControlStyle.Bootstrap => 0,                // No spread
                BeepControlStyle.FigmaCard => -1,               // Slight inset
                BeepControlStyle.Apple => 0,                    // No spread
                BeepControlStyle.Fluent => 0,                   // No spread
                BeepControlStyle.Material => -2,                // Inset slightly
                BeepControlStyle.WebFramework => 1,             // Slight spread
                BeepControlStyle.Effect => 4,                   // Effect expands
                _ => 0
            };
        }

        /// <summary>
        /// Get shadow offset Y for a specific style
        /// </summary>
        public static int GetShadowOffsetY(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => 4,                // Below
                BeepControlStyle.Fluent2 => 2,                  // Subtle below
                BeepControlStyle.AntDesign => 2,                // Subtle below
                BeepControlStyle.MaterialYou => 4,              // Below
                BeepControlStyle.MacOSBigSur => 3,              // Subtle below
                BeepControlStyle.ChakraUI => 4,                 // Below
                BeepControlStyle.TailwindCard => 4,             // Below
                BeepControlStyle.StripeDashboard => 6,          // Prominent below
                BeepControlStyle.DarkGlow => 0,                 // Glow all around
                BeepControlStyle.GradientModern => 8,           // Deep below
                BeepControlStyle.Neumorphism => 10,             // Neo below
                BeepControlStyle.Bootstrap => 2,                // Subtle below
                BeepControlStyle.FigmaCard => 4,                // Below
                BeepControlStyle.Apple => 1,                    // Very subtle below
                BeepControlStyle.Fluent => 2,                   // Subtle below
                BeepControlStyle.Material => 4,                 // Below
                BeepControlStyle.WebFramework => 2,             // Subtle below
                BeepControlStyle.Effect => 8,                   // Dramatic below
                _ => 2
            };
        }

        /// <summary>
        /// Get shadow offset X for a specific style
        /// </summary>
        public static int GetShadowOffsetX(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Neumorphism => 5,              // Neo offset right
                _ => 0                                          // Most centered
            };
        }

        /// <summary>
        /// Get shadow color for a specific style
        /// </summary>
        public static Color GetShadowColor(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => Color.FromArgb(60, 0, 0, 0),      // Material soft black
                BeepControlStyle.Fluent2 => Color.FromArgb(40, 0, 0, 0),        // Fluent very subtle
                BeepControlStyle.AntDesign => Color.FromArgb(45, 0, 0, 0),      // Ant subtle
                BeepControlStyle.MaterialYou => Color.FromArgb(60, 0, 0, 0),    // Material soft
                BeepControlStyle.MacOSBigSur => Color.FromArgb(30, 0, 0, 0),    // macOS very subtle
                BeepControlStyle.ChakraUI => Color.FromArgb(50, 0, 0, 0),       // Chakra moderate
                BeepControlStyle.TailwindCard => Color.FromArgb(40, 0, 0, 0),   // Tailwind subtle
                BeepControlStyle.StripeDashboard => Color.FromArgb(50, 0, 0, 0), // Stripe moderate
                BeepControlStyle.DarkGlow => Color.FromArgb(150, 139, 92, 246), // Purple glow
                BeepControlStyle.GradientModern => Color.FromArgb(80, 0, 0, 0), // Deep shadow
                BeepControlStyle.Neumorphism => Color.FromArgb(50, 163, 177, 198), // Soft blue-gray
                BeepControlStyle.Bootstrap => Color.FromArgb(40, 0, 0, 0),      // Bootstrap subtle
                BeepControlStyle.FigmaCard => Color.FromArgb(40, 0, 0, 0),      // Figma subtle
                BeepControlStyle.Apple => Color.FromArgb(25, 0, 0, 0),          // Apple very subtle
                BeepControlStyle.Fluent => Color.FromArgb(30, 0, 0, 0),         // Fluent very subtle
                BeepControlStyle.Material => Color.FromArgb(60, 0, 0, 0),       // Material soft
                BeepControlStyle.WebFramework => Color.FromArgb(30, 0, 0, 0),   // Web subtle
                BeepControlStyle.Effect => Color.FromArgb(100, 80, 120, 255),   // Effect blue-purple glow
                _ => Color.FromArgb(40, 0, 0, 0)
            };
        }

        /// <summary>
        /// Get highlight/light shadow for neumorphism (top-left)
        /// </summary>
        public static Color GetNeumorphismHighlight(BeepControlStyle style)
        {
            if (style == BeepControlStyle.Neumorphism)
            {
                return Color.FromArgb(200, 255, 255, 255);      // White highlight
            }
            return Color.Transparent;
        }

        /// <summary>
        /// Check if style uses dual shadows (neumorphism pattern)
        /// </summary>
        public static bool UsesDualShadows(BeepControlStyle style)
        {
            return style == BeepControlStyle.Neumorphism;
        }

        /// <summary>
        /// Check if style uses glow effect instead of shadow
        /// </summary>
        public static bool UsesGlow(BeepControlStyle style)
        {
            return style == BeepControlStyle.DarkGlow;
        }
    }
}
