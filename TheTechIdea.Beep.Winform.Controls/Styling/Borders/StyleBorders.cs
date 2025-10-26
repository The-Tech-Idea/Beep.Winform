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
                BeepControlStyle.Metro => 0,                    // Metro sharp edges (no radius)
                BeepControlStyle.Office => 4,                   // Office subtle rounded
                BeepControlStyle.Gnome => 6,                    // Gnome gentle (Adwaita)
                BeepControlStyle.Kde => 5,                      // KDE moderate (Breeze)
                BeepControlStyle.Cinnamon => 8,                 // Cinnamon rounded
                BeepControlStyle.Elementary => 6,               // Elementary subtle
                BeepControlStyle.NeoBrutalist => 0,             // Neo-Brutalist sharp (no radius)
                BeepControlStyle.Gaming => 0,                   // Gaming angular (no radius)
                BeepControlStyle.HighContrast => 0,             // High contrast sharp for clarity
                BeepControlStyle.Neon => 12,                    // Neon rounded for glow
                BeepControlStyle.ArcLinux => 4,
                BeepControlStyle.Brutalist => 0,
                BeepControlStyle.Cartoon => 12,
                BeepControlStyle.ChatBubble => 16,
                BeepControlStyle.Cyberpunk => 4,
                BeepControlStyle.Dracula => 7,
                BeepControlStyle.Glassmorphism => 14,
                BeepControlStyle.Holographic => 12,
                BeepControlStyle.GruvBox => 6,
                BeepControlStyle.Metro2 => 0,
                BeepControlStyle.Modern => 8,
                BeepControlStyle.Nord => 6,
                BeepControlStyle.Nordic => 8,
                BeepControlStyle.OneDark => 6,
                BeepControlStyle.Paper => 8,
                BeepControlStyle.Retro => 0,
                BeepControlStyle.Solarized => 4,
                BeepControlStyle.Terminal => 0,
                BeepControlStyle.Tokyo => 8,
                BeepControlStyle.Ubuntu => 7,
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
                BeepControlStyle.Metro => 2.0f,                 // Metro thick flat borders
                BeepControlStyle.Office => 1.0f,                // Office standard
                BeepControlStyle.Gnome => 1.0f,                 // Gnome standard
                BeepControlStyle.Kde => 1.0f,                   // KDE standard
                BeepControlStyle.Cinnamon => 1.0f,              // Cinnamon standard
                BeepControlStyle.Elementary => 1.0f,            // Elementary standard
                BeepControlStyle.NeoBrutalist => 4.0f,          // Neo-Brutalist very thick (signature)
                BeepControlStyle.Gaming => 2.0f,                // Gaming thick borders
                BeepControlStyle.HighContrast => 2.0f,          // High contrast thick for visibility
                BeepControlStyle.Neon => 2.0f,                  // Neon thick for glow effect
                BeepControlStyle.ArcLinux => 1.0f,
                BeepControlStyle.Brutalist => 5.0f,
                BeepControlStyle.Cartoon => 3.0f,
                BeepControlStyle.ChatBubble => 2.0f,
                BeepControlStyle.Cyberpunk => 2.0f,
                BeepControlStyle.Dracula => 1.0f,
                BeepControlStyle.Glassmorphism => 1.0f,
                BeepControlStyle.Holographic => 2.0f,
                BeepControlStyle.GruvBox => 2.0f,
                BeepControlStyle.Metro2 => 1.0f,
                BeepControlStyle.Modern => 1.0f,
                BeepControlStyle.Nord => 1.0f,
                BeepControlStyle.Nordic => 1.0f,
                BeepControlStyle.OneDark => 1.0f,
                BeepControlStyle.Paper => 1.0f,
                BeepControlStyle.Retro => 2.0f,
                BeepControlStyle.Solarized => 1.0f,
                BeepControlStyle.Terminal => 2.0f,
                BeepControlStyle.Tokyo => 2.0f,
                BeepControlStyle.Ubuntu => 1.0f,
                _ => 1.0f
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
                BeepControlStyle.Retro => true,
                BeepControlStyle.Solarized => true,
                BeepControlStyle.Terminal => true,
                BeepControlStyle.Tokyo => true,
                BeepControlStyle.Ubuntu => true,
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
                BeepControlStyle.TailwindCard => 3.0f,          // Tailwind ring
                BeepControlStyle.ChakraUI => 2.0f,              // Chakra ring
                BeepControlStyle.HighContrast => 3.0f,          // High contrast focus ring
                _ => 0.0f                                       // No ring
            };
        }

        /// <summary>
        /// Get ring effect offset (for Tailwind-Style focus rings)
        /// </summary>
        public static float GetRingOffset(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.TailwindCard => 2.0f,          // Tailwind offset
                BeepControlStyle.ChakraUI => 1.5f,              // Chakra offset
                BeepControlStyle.HighContrast => 2.0f,          // High contrast ring offset
                _ => 0.0f                                       // No offset
            };
        }
    }
}
