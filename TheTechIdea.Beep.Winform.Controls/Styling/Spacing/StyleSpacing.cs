using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.Spacing
{
    /// <summary>
    /// Spacing and padding definitions for all design systems
    /// Each style has distinct spacing patterns
    /// </summary>
    public static class StyleSpacing
    {
        /// <summary>
        /// Get standard padding for a specific style
        /// </summary>
        public static int GetPadding(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => 16,               // Material Design 8dp grid
                BeepControlStyle.iOS15 => 12,                   // iOS comfortable spacing
                BeepControlStyle.Fluent2 => 8,                  // Fluent compact
                BeepControlStyle.Minimal => 8,                  // Minimal tight spacing
                BeepControlStyle.AntDesign => 16,               // Ant spacious
                BeepControlStyle.MaterialYou => 16,             // Material 8dp grid
                BeepControlStyle.Windows11Mica => 12,           // Windows balanced
                BeepControlStyle.MacOSBigSur => 12,             // macOS standard
                BeepControlStyle.ChakraUI => 16,                // Chakra 4-based
                BeepControlStyle.TailwindCard => 16,            // Tailwind 4-unit
                BeepControlStyle.NotionMinimal => 12,           // Notion comfortable
                BeepControlStyle.VercelClean => 16,             // Vercel spacious
                BeepControlStyle.StripeDashboard => 20,         // Stripe generous
                BeepControlStyle.DarkGlow => 16,                // Dark mode spacing
                BeepControlStyle.DiscordStyle => 12,            // Discord compact
                BeepControlStyle.GradientModern => 20,          // Modern spacious
                BeepControlStyle.GlassAcrylic => 16,            // Glass generous
                BeepControlStyle.Neumorphism => 20,             // Neo needs space for shadows
                BeepControlStyle.Bootstrap => 12,               // Bootstrap standard
                BeepControlStyle.FigmaCard => 16,               // Figma 8px grid
                BeepControlStyle.PillRail => 8,                 // Pill compact
                BeepControlStyle.Apple => 12,                   // Apple comfortable
                BeepControlStyle.Fluent => 8,                   // Fluent compact
                BeepControlStyle.Material => 16,                // Material 8dp grid
                BeepControlStyle.WebFramework => 16,            // Web standard
                BeepControlStyle.Effect => 20,                  // Effect generous
                _ => 12
            };
        }

        /// <summary>
        /// Get item spacing/gap for a specific style
        /// </summary>
        public static int GetItemSpacing(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => 4,                // Tight item spacing
                BeepControlStyle.iOS15 => 4,                    // iOS close items
                BeepControlStyle.Fluent2 => 4,                  // Fluent tight
                BeepControlStyle.Minimal => 2,                  // Minimal very tight
                BeepControlStyle.AntDesign => 8,                // Ant comfortable
                BeepControlStyle.MaterialYou => 4,              // Material tight
                BeepControlStyle.Windows11Mica => 4,            // Windows balanced
                BeepControlStyle.MacOSBigSur => 2,              // macOS very tight
                BeepControlStyle.ChakraUI => 8,                 // Chakra generous
                BeepControlStyle.TailwindCard => 4,             // Tailwind standard
                BeepControlStyle.NotionMinimal => 2,            // Notion minimal
                BeepControlStyle.VercelClean => 8,              // Vercel spacious
                BeepControlStyle.StripeDashboard => 8,          // Stripe comfortable
                BeepControlStyle.DarkGlow => 4,                 // Dark spacing
                BeepControlStyle.DiscordStyle => 2,             // Discord compact
                BeepControlStyle.GradientModern => 8,           // Modern spacious
                BeepControlStyle.GlassAcrylic => 8,             // Glass generous
                BeepControlStyle.Neumorphism => 12,             // Neo extra space
                BeepControlStyle.Bootstrap => 8,                // Bootstrap standard
                BeepControlStyle.FigmaCard => 8,                // Figma comfortable
                BeepControlStyle.PillRail => 4,                 // Pill tight
                BeepControlStyle.Apple => 2,                    // Apple minimal spacing
                BeepControlStyle.Fluent => 4,                   // Fluent tight
                BeepControlStyle.Material => 4,                 // Material tight
                BeepControlStyle.WebFramework => 8,             // Web comfortable
                BeepControlStyle.Effect => 8,                   // Effect spacious
                _ => 4
            };
        }

        /// <summary>
        /// Get icon size for a specific style
        /// </summary>
        public static int GetIconSize(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => 24,               // Material standard
                BeepControlStyle.iOS15 => 28,                   // iOS larger icons
                BeepControlStyle.Fluent2 => 20,                 // Fluent compact
                BeepControlStyle.Minimal => 18,                 // Minimal small
                BeepControlStyle.AntDesign => 24,               // Ant standard
                BeepControlStyle.MaterialYou => 24,             // Material standard
                BeepControlStyle.Windows11Mica => 20,           // Windows standard
                BeepControlStyle.MacOSBigSur => 22,             // macOS standard
                BeepControlStyle.ChakraUI => 20,                // Chakra standard
                BeepControlStyle.TailwindCard => 20,            // Tailwind standard
                BeepControlStyle.NotionMinimal => 18,           // Notion small
                BeepControlStyle.VercelClean => 20,             // Vercel standard
                BeepControlStyle.StripeDashboard => 22,         // Stripe comfortable
                BeepControlStyle.DarkGlow => 24,                // Dark larger
                BeepControlStyle.DiscordStyle => 20,            // Discord standard
                BeepControlStyle.GradientModern => 24,          // Modern standard
                BeepControlStyle.GlassAcrylic => 22,            // Glass standard
                BeepControlStyle.Neumorphism => 24,             // Neo standard
                BeepControlStyle.Bootstrap => 20,               // Bootstrap standard
                BeepControlStyle.FigmaCard => 20,               // Figma standard
                BeepControlStyle.PillRail => 20,                // Pill standard
                BeepControlStyle.Apple => 22,                   // Apple SF Symbols size
                BeepControlStyle.Fluent => 20,                  // Fluent standard
                BeepControlStyle.Material => 24,                // Material standard
                BeepControlStyle.WebFramework => 20,            // Web standard
                BeepControlStyle.Effect => 24,                  // Effect larger
                _ => 20
            };
        }

        /// <summary>
        /// Get indentation width for hierarchical items (sidebars, trees)
        /// </summary>
        public static int GetIndentationWidth(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => 24,               // Material generous
                BeepControlStyle.iOS15 => 20,                   // iOS comfortable
                BeepControlStyle.Fluent2 => 16,                 // Fluent compact
                BeepControlStyle.Minimal => 12,                 // Minimal tight
                BeepControlStyle.AntDesign => 24,               // Ant spacious
                BeepControlStyle.MaterialYou => 24,             // Material generous
                BeepControlStyle.Windows11Mica => 20,           // Windows standard
                BeepControlStyle.MacOSBigSur => 16,             // macOS tight
                BeepControlStyle.ChakraUI => 20,                // Chakra standard
                BeepControlStyle.TailwindCard => 16,            // Tailwind standard
                BeepControlStyle.NotionMinimal => 16,           // Notion standard
                BeepControlStyle.VercelClean => 20,             // Vercel spacious
                BeepControlStyle.StripeDashboard => 24,         // Stripe generous
                BeepControlStyle.DarkGlow => 20,                // Dark standard
                BeepControlStyle.DiscordStyle => 16,            // Discord compact
                BeepControlStyle.GradientModern => 24,          // Modern spacious
                BeepControlStyle.GlassAcrylic => 20,            // Glass standard
                BeepControlStyle.Neumorphism => 24,             // Neo generous
                BeepControlStyle.Bootstrap => 20,               // Bootstrap standard
                BeepControlStyle.FigmaCard => 20,               // Figma standard
                BeepControlStyle.PillRail => 16,                // Pill compact
                BeepControlStyle.Apple => 16,                   // Apple tight hierarchy
                BeepControlStyle.Fluent => 16,                  // Fluent compact
                BeepControlStyle.Material => 24,                // Material generous
                BeepControlStyle.WebFramework => 20,            // Web standard
                BeepControlStyle.Effect => 24,                  // Effect spacious
                _ => 20
            };
        }

        /// <summary>
        /// Get item height for list/menu items
        /// </summary>
        public static int GetItemHeight(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => 48,               // Material touch target
                BeepControlStyle.iOS15 => 44,                   // iOS standard
                BeepControlStyle.Fluent2 => 40,                 // Fluent compact
                BeepControlStyle.Minimal => 36,                 // Minimal tight
                BeepControlStyle.AntDesign => 40,               // Ant comfortable
                BeepControlStyle.MaterialYou => 48,             // Material touch target
                BeepControlStyle.Windows11Mica => 40,           // Windows standard
                BeepControlStyle.MacOSBigSur => 44,             // macOS standard
                BeepControlStyle.ChakraUI => 40,                // Chakra standard
                BeepControlStyle.TailwindCard => 40,            // Tailwind standard
                BeepControlStyle.NotionMinimal => 32,           // Notion compact
                BeepControlStyle.VercelClean => 44,             // Vercel spacious
                BeepControlStyle.StripeDashboard => 48,         // Stripe generous
                BeepControlStyle.DarkGlow => 48,                // Dark comfortable
                BeepControlStyle.DiscordStyle => 36,            // Discord compact
                BeepControlStyle.GradientModern => 48,          // Modern spacious
                BeepControlStyle.GlassAcrylic => 44,            // Glass standard
                BeepControlStyle.Neumorphism => 52,             // Neo extra height for effect
                BeepControlStyle.Bootstrap => 40,               // Bootstrap standard
                BeepControlStyle.FigmaCard => 40,               // Figma standard
                BeepControlStyle.PillRail => 40,                // Pill standard
                BeepControlStyle.Apple => 44,                   // Apple iOS/macOS standard
                BeepControlStyle.Fluent => 40,                  // Fluent standard
                BeepControlStyle.Material => 48,                // Material touch target
                BeepControlStyle.WebFramework => 40,            // Web standard
                BeepControlStyle.Effect => 48,                  // Effect generous
                _ => 40
            };
        }
    }
}
