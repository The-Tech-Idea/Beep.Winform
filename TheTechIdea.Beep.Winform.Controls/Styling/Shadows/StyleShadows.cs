using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.Shadows
{
    /// <summary>
    /// Shadow and elevation definitions for all design systems
    /// Each Style has distinct shadow characteristics
    /// </summary>
    public static class StyleShadows
    {
        /// <summary>
        /// Check if Style uses shadows/elevation
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
                BeepControlStyle.Metro => false,                // Metro flat (no shadow)
                BeepControlStyle.Office => true,                // Office subtle elevation
                BeepControlStyle.Gnome => true,                 // Gnome subtle shadow
                BeepControlStyle.Kde => true,                   // KDE glow + shadow
                BeepControlStyle.Cinnamon => true,              // Cinnamon soft shadow
                BeepControlStyle.Elementary => true,            // Elementary subtle shadow
                BeepControlStyle.NeoBrutalist => false,         // Neo-Brutalist NO shadows (signature)
                BeepControlStyle.Gaming => true,                // Gaming dramatic glow
                BeepControlStyle.HighContrast => false,         // High contrast flat (no shadow)
                BeepControlStyle.Neon => true,                  // Neon strong glow
                BeepControlStyle.ArcLinux => true,
                BeepControlStyle.Brutalist => true,
                BeepControlStyle.Cartoon => true,
                BeepControlStyle.ChatBubble => true,
                BeepControlStyle.Cyberpunk => true,
                BeepControlStyle.Dracula => true,
                BeepControlStyle.Glassmorphism => true,
                BeepControlStyle.Holographic => true,
                BeepControlStyle.GruvBox => true,
                BeepControlStyle.Metro2 => true,
                BeepControlStyle.Modern => true,
                BeepControlStyle.Nord => true,
                BeepControlStyle.Nordic => true,
                BeepControlStyle.OneDark => true,
                BeepControlStyle.Paper => true,
               
                BeepControlStyle.Solarized => true,
                BeepControlStyle.Terminal => false,
                BeepControlStyle.Tokyo => true,
                BeepControlStyle.Ubuntu => true,
                _ => false
            };
        }

        /// <summary>
        /// Get shadow blur radius for a specific Style
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
                BeepControlStyle.Office => 8,                   // Office subtle
                BeepControlStyle.Gnome => 6,                    // Gnome very subtle
                BeepControlStyle.Kde => 10,                     // KDE soft glow
                BeepControlStyle.Cinnamon => 10,                // Cinnamon moderate
                BeepControlStyle.Elementary => 8,               // Elementary subtle
                BeepControlStyle.Gaming => 20,                  // Gaming dramatic glow
                BeepControlStyle.Neon => 24,                    // Neon large glow
                BeepControlStyle.ArcLinux => 6,
                BeepControlStyle.Brutalist => 6,
                BeepControlStyle.Cartoon => 15,
                BeepControlStyle.ChatBubble => 12,
                BeepControlStyle.Cyberpunk => 24,
                BeepControlStyle.Dracula => 14,
                BeepControlStyle.Glassmorphism => 15,
                BeepControlStyle.Holographic => 16,
                BeepControlStyle.GruvBox => 8,
                BeepControlStyle.Metro2 => 10,
                BeepControlStyle.Modern => 10,
                BeepControlStyle.Nord => 6,
                BeepControlStyle.Nordic => 10,
                BeepControlStyle.OneDark => 10,
                BeepControlStyle.Paper => 12,
               
                BeepControlStyle.Solarized => 7,
                BeepControlStyle.Terminal => 0,
                BeepControlStyle.Tokyo => 14,
                BeepControlStyle.Ubuntu => 12,
                _ => 8
            };
        }

        /// <summary>
        /// Get shadow spread radius for a specific Style
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
                BeepControlStyle.Office => 0,                   // No spread
                BeepControlStyle.Gnome => 0,                    // No spread
                BeepControlStyle.Kde => 2,                      // KDE glow spreads
                BeepControlStyle.Cinnamon => 0,                 // No spread
                BeepControlStyle.Elementary => 0,               // No spread
                BeepControlStyle.Gaming => 6,                   // Gaming glow expands
                BeepControlStyle.Neon => 8,                     // Neon glow expands
                _ => 0
            };
        }

        /// <summary>
        /// Get shadow offset Y for a specific Style
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
                BeepControlStyle.Office => 2,                   // Office subtle below
                BeepControlStyle.Gnome => 2,                    // Gnome subtle below
                BeepControlStyle.Kde => 0,                      // KDE glow all around
                BeepControlStyle.Cinnamon => 3,                 // Cinnamon below
                BeepControlStyle.Elementary => 2,               // Elementary subtle below
                BeepControlStyle.Gaming => 0,                   // Gaming glow all around
                BeepControlStyle.Neon => 0,                     // Neon glow all around
                BeepControlStyle.ArcLinux => 3,
                BeepControlStyle.Brutalist => 6,
                BeepControlStyle.Cartoon => 8,
                BeepControlStyle.ChatBubble => 6,
                BeepControlStyle.Cyberpunk => 8,
                BeepControlStyle.Dracula => 6,
                BeepControlStyle.Glassmorphism => 6,
                BeepControlStyle.Holographic => 8,
                BeepControlStyle.GruvBox => 4,
                BeepControlStyle.Metro2 => 3,
                BeepControlStyle.Modern => 4,
                BeepControlStyle.Nord => 3,
                BeepControlStyle.Nordic => 4,
                BeepControlStyle.OneDark => 4,
                BeepControlStyle.Paper => 6,
              
                BeepControlStyle.Solarized => 3,
                BeepControlStyle.Terminal => 0,
                BeepControlStyle.Tokyo => 7,
                BeepControlStyle.Ubuntu => 5,
                _ => 2
            };
        }

        /// <summary>
        /// Get shadow offset X for a specific Style
        /// </summary>
        public static int GetShadowOffsetX(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Neumorphism => 5,              // Neo offset right
                BeepControlStyle.Brutalist => 6,
                _ => 0                                          // Most centered
            };
        }

        /// <summary>
        /// Get shadow color for a specific Style
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
                BeepControlStyle.Office => Color.FromArgb(30, 0, 0, 0),         // Office subtle black
                BeepControlStyle.Gnome => Color.FromArgb(35, 0, 0, 0),          // Gnome soft black
                BeepControlStyle.Kde => Color.FromArgb(100, 61, 174, 233),      // KDE blue glow
                BeepControlStyle.Cinnamon => Color.FromArgb(40, 0, 0, 0),       // Cinnamon soft black
                BeepControlStyle.Elementary => Color.FromArgb(30, 0, 0, 0),     // Elementary subtle black
                BeepControlStyle.Gaming => Color.FromArgb(150, 0, 255, 127),    // Gaming green glow
                BeepControlStyle.Neon => Color.FromArgb(180, 0, 255, 255),      // Neon cyan glow
                BeepControlStyle.ArcLinux => Color.FromArgb(40, 0, 0, 0),
                BeepControlStyle.Brutalist => Color.FromArgb(120, 0, 0, 0),
                BeepControlStyle.Cartoon => Color.FromArgb(60, 0, 0, 0),
                BeepControlStyle.ChatBubble => Color.FromArgb(60, 100, 149, 237),
                BeepControlStyle.Cyberpunk => Color.FromArgb(140, 0, 255, 255),
                BeepControlStyle.Dracula => Color.FromArgb(110, 150, 100, 200),
                BeepControlStyle.Glassmorphism => Color.FromArgb(50, 0, 0, 0),
                BeepControlStyle.Holographic => Color.FromArgb(130, 100, 150, 255),
                BeepControlStyle.GruvBox => Color.FromArgb(90, 40, 30, 20),
                BeepControlStyle.Metro2 => Color.FromArgb(70, 0, 0, 0),
                BeepControlStyle.Modern => Color.FromArgb(70, 0, 0, 0),
                BeepControlStyle.Nord => Color.FromArgb(70, 94, 129, 156),
                BeepControlStyle.Nordic => Color.FromArgb(50, 163, 177, 198),
                BeepControlStyle.OneDark => Color.FromArgb(90, 0, 0, 0),
                BeepControlStyle.Paper => Color.FromArgb(70, 0, 0, 0),
               
                BeepControlStyle.Solarized => Color.FromArgb(80, 0, 43, 54),
                BeepControlStyle.Tokyo => Color.FromArgb(130, 122, 162, 247),
                BeepControlStyle.Ubuntu => Color.FromArgb(120, 233, 84, 32),
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
        /// Check if Style uses dual shadows (neumorphism pattern)
        /// </summary>
        public static bool UsesDualShadows(BeepControlStyle style)
        {
            return style == BeepControlStyle.Neumorphism;
        }

        /// <summary>
        /// Check if Style uses glow effect instead of shadow
        /// </summary>
        public static bool UsesGlow(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.DarkGlow => true,
                BeepControlStyle.Kde => true,                   // KDE Breeze glow
                BeepControlStyle.Gaming => true,                // Gaming RGB glow
                BeepControlStyle.Neon => true,                  // Neon glow
                BeepControlStyle.Cyberpunk => true,
                BeepControlStyle.Holographic => true,
                BeepControlStyle.Tokyo => true,
                _ => false
            };
        }
    }
}
