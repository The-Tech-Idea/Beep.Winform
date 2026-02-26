using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.Borders
{
    
    /// <summary>
    /// Border radius and width definitions for all design systems
    /// Each Style has distinct border characteristics
    /// </summary>
    public static class StyleBorders
    {
        /// <summary>
        /// Get border radius for a specific Style
        /// </summary>
        public static int GetRadius(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => 12,              // Material 3 rounded
                BeepControlStyle.iOS15 => 16,                  // iOS modern curve
                BeepControlStyle.Fluent2 => 4,                  // Fluent subtle
                BeepControlStyle.Minimal => 0,                  // No radius
                BeepControlStyle.AntDesign => 6,                // Ant Design standard
                BeepControlStyle.MaterialYou => 16,             // Material You large
                BeepControlStyle.Windows11Mica => 4,            // Windows subtle
                BeepControlStyle.MacOSBigSur => 10,             // macOS continuous curve
                BeepControlStyle.ChakraUI => 6,                 // Chakra standard
                BeepControlStyle.TailwindCard => 8,             // Tailwind rounded-lg
                BeepControlStyle.FinSet => 8,                   // FinSet compact card radius
                BeepControlStyle.NotionMinimal => 3,            // Notion very subtle
                BeepControlStyle.VercelClean => 4,              // Vercel subtle
                BeepControlStyle.StripeDashboard => 6,          // Stripe comfortable
                BeepControlStyle.DarkGlow => 2,                 // Dark rounded (cyberpunk path)
                BeepControlStyle.DiscordStyle => 8,             // Discord rounded
                BeepControlStyle.GradientModern => 14,          // Modern flowing curves
                BeepControlStyle.GlassAcrylic => 12,            // Glass smooth
                BeepControlStyle.Neumorphism => 4,              // Neo soft rounded
                BeepControlStyle.Bootstrap => 4,                // Bootstrap subtle
                BeepControlStyle.FigmaCard => 8,                // Figma modern rounded
                BeepControlStyle.PillRail => 6,                 // Pill (uses pill path)
                BeepControlStyle.Metro => 0,                    // Metro sharp edges
                BeepControlStyle.Office => 3,                   // Office subtle
                BeepControlStyle.Gnome => 6,                    // Gnome gentle (Adwaita)
                BeepControlStyle.Kde => 5,                      // KDE moderate (Breeze)
                BeepControlStyle.Cinnamon => 8,                 // Cinnamon rounded
                BeepControlStyle.Elementary => 6,               // Elementary subtle
                BeepControlStyle.NeoBrutalist => 0,             // Neo-Brutalist sharp
                BeepControlStyle.Gaming => 0,                   // Gaming angular (gaming path)
                BeepControlStyle.HighContrast => 0,             // High contrast sharp
                BeepControlStyle.Neon => 2,                     // Neon rounded for glow
                BeepControlStyle.ArcLinux => 4,
                BeepControlStyle.Brutalist => 0,                // Brutalist sharp
                BeepControlStyle.Cartoon => 2,
                BeepControlStyle.ChatBubble => 4,
                BeepControlStyle.Cyberpunk => 4,
                BeepControlStyle.Dracula => 7,
                BeepControlStyle.Glassmorphism => 2,
                BeepControlStyle.Holographic => 4,
                BeepControlStyle.GruvBox => 6,
                BeepControlStyle.Metro2 => 0,
                BeepControlStyle.Modern => 8,
                BeepControlStyle.Nord => 6,
                BeepControlStyle.Nordic => 8,
                BeepControlStyle.OneDark => 6,
                BeepControlStyle.Paper => 8,
                BeepControlStyle.Solarized => 4,
                BeepControlStyle.Terminal => 0,
                BeepControlStyle.Tokyo => 8,
                BeepControlStyle.Ubuntu => 7,
                BeepControlStyle.Apple => 12,                   // Apple continuous curves
                BeepControlStyle.Fluent => 4,                   // Fluent standard
                BeepControlStyle.Material => 4,                 // Material standard
                BeepControlStyle.WebFramework => 4,             // WebFramework standard
                BeepControlStyle.Effect => 4,                   // Effect standard
                BeepControlStyle.Retro => 0,                    // Retro sharp edges (retro path)
                BeepControlStyle.NeonGlow => 12,                // NeonGlow (cyberpunk path)
                BeepControlStyle.Shadcn => 6,                   // Shadcn modern radius
                BeepControlStyle.RadixUI => 6,                  // RadixUI modern radius
                BeepControlStyle.NextJS => 8,                   // NextJS smooth radius
                BeepControlStyle.Linear => 8,                   // Linear smooth radius
                _ => 4
            };
        }

        /// <summary>
        /// Get selection indicator radius for a specific Style
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
                BeepControlStyle.Metro => 0,                    // Metro sharp (no radius)
                BeepControlStyle.Office => 4,                   // Office subtle
                BeepControlStyle.Gnome => 6,                    // Gnome gentle
                BeepControlStyle.Kde => 5,                      // KDE moderate
                BeepControlStyle.Cinnamon => 8,                 // Cinnamon rounded
                BeepControlStyle.Elementary => 6,               // Elementary subtle
                BeepControlStyle.NeoBrutalist => 0,             // Neo-Brutalist sharp
                BeepControlStyle.Gaming => 0,                   // Gaming angular
                BeepControlStyle.HighContrast => 0,             // High contrast sharp
                BeepControlStyle.Neon => 12,                    // Neon rounded
                _ => 4
            };
        }

        /// <summary>
        /// Get border width for a specific Style
        /// </summary>
        public static float GetBorderWidth(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => 2.0f,
                BeepControlStyle.iOS15 => 2.0f,
                BeepControlStyle.Fluent2 => 2.0f,
                BeepControlStyle.Minimal => 2.0f,
                BeepControlStyle.AntDesign => 2.0f,
                BeepControlStyle.MaterialYou => 0.0f,           // No border (filled - Material You spec)
                BeepControlStyle.Windows11Mica => 2.0f,
                BeepControlStyle.MacOSBigSur => 2.0f,
                BeepControlStyle.ChakraUI => 2.0f,
                BeepControlStyle.TailwindCard => 2.0f,
                BeepControlStyle.FinSet => 2.0f,
                BeepControlStyle.NotionMinimal => 2.0f,
                BeepControlStyle.VercelClean => 2.0f,
                BeepControlStyle.StripeDashboard => 2.0f,
                BeepControlStyle.DarkGlow => 2.0f,
                BeepControlStyle.DiscordStyle => 0.0f,          // No border
                BeepControlStyle.GradientModern => 0.0f,        // No border (gradient)
                BeepControlStyle.GlassAcrylic => 2.0f,
                BeepControlStyle.Neumorphism => 0.0f,           // No border (shadow defines)
                BeepControlStyle.Bootstrap => 2.0f,
                BeepControlStyle.FigmaCard => 2.0f,
                BeepControlStyle.PillRail => 0.0f,              // No border
                BeepControlStyle.Metro => 2.0f,
                BeepControlStyle.Office => 2.0f,
                BeepControlStyle.Gnome => 2.0f,
                BeepControlStyle.Kde => 2.0f,
                BeepControlStyle.Cinnamon => 2.0f,
                BeepControlStyle.Elementary => 2.0f,
                BeepControlStyle.NeoBrutalist => 3.0f,          // Neo-Brutalist very thick aggressive border
                BeepControlStyle.Gaming => 2.0f,
                BeepControlStyle.HighContrast => 2.0f,
                BeepControlStyle.Neon => 2.0f,
                BeepControlStyle.ArcLinux => 2.0f,
                BeepControlStyle.Brutalist => 3.0f,
                BeepControlStyle.Cartoon => 3.0f,
                BeepControlStyle.ChatBubble => 2.0f,
                BeepControlStyle.Cyberpunk => 2.0f,
                BeepControlStyle.Dracula => 2.0f,
                BeepControlStyle.Glassmorphism => 2.0f,
                BeepControlStyle.Holographic => 2.0f,
                BeepControlStyle.GruvBox => 2.0f,
                BeepControlStyle.Metro2 => 2.0f,
                BeepControlStyle.Modern => 2.0f,
                BeepControlStyle.Nord => 2.0f,
                BeepControlStyle.Nordic => 2.0f,
                BeepControlStyle.OneDark => 2.0f,
                BeepControlStyle.Paper => 2.0f,
                BeepControlStyle.Solarized => 2.0f,
                BeepControlStyle.Terminal => 2.0f,
                BeepControlStyle.Tokyo => 2.0f,
                BeepControlStyle.Ubuntu => 2.0f,
                BeepControlStyle.Apple => 2.0f,
                BeepControlStyle.Fluent => 2.0f,
                BeepControlStyle.Material => 2.0f,
                BeepControlStyle.WebFramework => 2.0f,
                BeepControlStyle.Effect => 0.0f,                // Effect no border (effect-driven)
                BeepControlStyle.Retro => 2.0f,
                BeepControlStyle.NeonGlow => 2.0f,
                BeepControlStyle.Shadcn => 2.0f,
                BeepControlStyle.RadixUI => 2.0f,
                BeepControlStyle.NextJS => 2.0f,
                BeepControlStyle.Linear => 2.0f,
                _ => 2.0f
            };
        }

        /// <summary>
        /// Check if Style uses filled/solid backgrounds (vs outlined)
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
                BeepControlStyle.FinSet => false,               // Outlined
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
                BeepControlStyle.Metro => true,                 // Metro filled tiles
                BeepControlStyle.Office => true,                // Office filled (ribbon)
                BeepControlStyle.Gnome => true,                 // Gnome filled
                BeepControlStyle.Kde => true,                   // KDE filled
                BeepControlStyle.Cinnamon => true,              // Cinnamon filled
                BeepControlStyle.Elementary => true,            // Elementary filled
                BeepControlStyle.NeoBrutalist => true,          // Neo-Brutalist bold filled
                BeepControlStyle.Gaming => true,                // Gaming filled
                BeepControlStyle.HighContrast => false,         // High contrast outlined for clarity
                BeepControlStyle.Neon => true,                  // Neon filled with glow
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
                BeepControlStyle.Terminal => true,
                BeepControlStyle.Tokyo => true,
                BeepControlStyle.Ubuntu => true,
                BeepControlStyle.Apple => true,                 // Apple filled surfaces
                BeepControlStyle.Fluent => true,                // Fluent filled
                BeepControlStyle.Material => true,              // Material filled
                BeepControlStyle.WebFramework => false,         // WebFramework outlined
                BeepControlStyle.Effect => true,                // Effect filled
                BeepControlStyle.Retro => true,                 // Retro filled
                BeepControlStyle.NeonGlow => true,              // NeonGlow filled with glow
                BeepControlStyle.Shadcn => false,               // Shadcn outlined
                BeepControlStyle.RadixUI => false,              // RadixUI outlined
                BeepControlStyle.NextJS => false,               // NextJS outlined
                BeepControlStyle.Linear => false,               // Linear outlined
                _ => false
            };
        }

        /// <summary>
        /// Get accent bar width (for Fluent-Style left indicators)
        /// </summary>
        public static int GetAccentBarWidth(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Fluent2 => 3,                  // Fluent signature bar
                BeepControlStyle.Windows11Mica => 3,            // Windows bar
                BeepControlStyle.StripeDashboard => 4,          // Stripe thicker
                BeepControlStyle.Bootstrap => 4,                // Bootstrap bar
                BeepControlStyle.Office => 3,                   // Office ribbon accent bar
                _ => 0                                          // No accent bar
            };
        }

        /// <summary>
        /// Get glow border width (for glow effect styles)
        /// </summary>
        public static float GetGlowWidth(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.DarkGlow => 2.0f,              // Dark glow signature
                BeepControlStyle.GlassAcrylic => 1.5f,          // Subtle glass glow
                BeepControlStyle.Kde => 1.0f,                   // KDE subtle glow (Breeze)
                BeepControlStyle.Neon => 3.0f,                  // Neon strong glow
                BeepControlStyle.Cyberpunk => 3.5f,
                BeepControlStyle.Dracula => 2.0f,
                BeepControlStyle.Glassmorphism => 1.5f,
                BeepControlStyle.Holographic => 2.5f,
                BeepControlStyle.Tokyo => 2.5f,
                _ => 0.0f                                       // No glow
            };
        }

        /// <summary>
        /// Get ring effect width (for Tailwind-Style focus rings)
        /// </summary>
        public static float GetRingWidth(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => 2.5f,             // Material focus ring
                BeepControlStyle.Material => 2.5f,               // Material focus ring
                BeepControlStyle.MaterialYou => 2.5f,            // Material You focus ring
                BeepControlStyle.Fluent => 2.0f,                 // Fluent focus ring
                BeepControlStyle.Fluent2 => 2.0f,                // Fluent 2 focus ring
                BeepControlStyle.iOS15 => 2.5f,                  // iOS focus ring
                BeepControlStyle.Apple => 2.5f,                  // Apple focus ring
                BeepControlStyle.MacOSBigSur => 2.5f,            // macOS focus ring
                BeepControlStyle.Modern => 2.0f,                 // Modern focus ring
                BeepControlStyle.TailwindCard => 3.0f,           // Tailwind ring
                BeepControlStyle.ChakraUI => 2.0f,               // Chakra ring
                BeepControlStyle.HighContrast => 3.0f,           // High contrast focus ring
                _ => 0.0f                                        // No ring
            };
        }

        /// <summary>
        /// Get ring effect offset (for Tailwind-Style focus rings)
        /// </summary>
        public static float GetRingOffset(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => 2.0f,             // Material ring offset
                BeepControlStyle.Material => 2.0f,               // Material ring offset
                BeepControlStyle.MaterialYou => 2.0f,            // Material You ring offset
                BeepControlStyle.Fluent => 1.0f,                 // Fluent ring offset
                BeepControlStyle.Fluent2 => 1.0f,                // Fluent 2 ring offset
                BeepControlStyle.iOS15 => 1.0f,                  // iOS ring offset
                BeepControlStyle.Apple => 1.0f,                  // Apple ring offset
                BeepControlStyle.MacOSBigSur => 1.0f,            // macOS ring offset
                BeepControlStyle.Modern => 1.0f,                 // Modern ring offset
                BeepControlStyle.TailwindCard => 2.0f,           // Tailwind offset
                BeepControlStyle.ChakraUI => 1.5f,               // Chakra offset
                BeepControlStyle.HighContrast => 2.0f,           // High contrast ring offset
                _ => 0.0f                                        // No offset
            };
        }
    }
}
