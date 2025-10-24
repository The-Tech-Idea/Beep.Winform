using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.Typography
{
    /// <summary>
    /// Typography and font definitions for all design systems
    /// Each Style has distinct font characteristics
    /// </summary>
    public static class StyleTypography
    {
        /// <summary>
        /// Get primary font family for a specific Style
        /// </summary>
        public static string GetFontFamily(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => "Roboto, Segoe UI, Arial",
                BeepControlStyle.iOS15 => "SF Pro Display, Segoe UI, Arial",
                BeepControlStyle.Fluent2 => "Segoe UI Variable, Segoe UI, Arial",
                BeepControlStyle.Minimal => "Inter, Segoe UI, Arial",
                BeepControlStyle.AntDesign => "Chinese Quote, Segoe UI, Arial",
                BeepControlStyle.MaterialYou => "Roboto Flex, Roboto, Segoe UI, Arial",
                BeepControlStyle.Windows11Mica => "Segoe UI Variable, Segoe UI, Arial",
                BeepControlStyle.MacOSBigSur => "SF Pro Display, Segoe UI, Arial",
                BeepControlStyle.ChakraUI => "Inter, Segoe UI, Arial",
                BeepControlStyle.TailwindCard => "Inter, Segoe UI, Arial",
                BeepControlStyle.NotionMinimal => "Inter, Segoe UI, Arial",
                BeepControlStyle.VercelClean => "Inter, Segoe UI, Arial",
                BeepControlStyle.StripeDashboard => "Inter, Segoe UI, Arial",
                BeepControlStyle.DarkGlow => "JetBrains Mono, Consolas, Courier New",
                BeepControlStyle.DiscordStyle => "Whitney, Segoe UI, Arial",
                BeepControlStyle.GradientModern => "Inter, Segoe UI, Arial",
                BeepControlStyle.GlassAcrylic => "Segoe UI Variable, Segoe UI, Arial",
                BeepControlStyle.Neumorphism => "Montserrat, Segoe UI, Arial",
                BeepControlStyle.Bootstrap => "System UI, Segoe UI, Arial",
                BeepControlStyle.FigmaCard => "Inter, Segoe UI, Arial",
                BeepControlStyle.PillRail => "Inter, Segoe UI, Arial",
                BeepControlStyle.Metro => "Segoe UI, Arial",                    // Metro modern sans-serif
                BeepControlStyle.Office => "Calibri, Segoe UI, Arial",          // Office signature font
                BeepControlStyle.Gnome => "Cantarell, Segoe UI, Arial",         // GNOME system font
                BeepControlStyle.Kde => "Noto Sans, Segoe UI, Arial",           // KDE default font
                BeepControlStyle.Cinnamon => "Ubuntu, Segoe UI, Arial",         // Cinnamon Ubuntu font
                BeepControlStyle.Elementary => "Open Sans, Segoe UI, Arial",    // elementary OS font
                BeepControlStyle.NeoBrutalist => "Arial, Helvetica, sans-serif",// Neo-Brutalist simple sans
                BeepControlStyle.Gaming => "Orbitron, Rajdhani, Segoe UI",      // Gaming futuristic (fallback to system)
                BeepControlStyle.HighContrast => "Segoe UI, Arial",             // High contrast clear font
                BeepControlStyle.Neon => "Rajdhani, Segoe UI, Arial",           // Neon modern geometric
                _ => "Segoe UI, Arial"
            };
        }

        /// <summary>
        /// Get default font size for regular text
        /// </summary>
        public static float GetFontSize(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => 14f,
                BeepControlStyle.iOS15 => 14f,
                BeepControlStyle.Fluent2 => 14f,
                BeepControlStyle.Minimal => 13f,
                BeepControlStyle.AntDesign => 14f,
                BeepControlStyle.MaterialYou => 14f,
                BeepControlStyle.Windows11Mica => 14f,
                BeepControlStyle.MacOSBigSur => 13f,
                BeepControlStyle.ChakraUI => 14f,
                BeepControlStyle.TailwindCard => 14f,
                BeepControlStyle.NotionMinimal => 14f,
                BeepControlStyle.VercelClean => 13f,
                BeepControlStyle.StripeDashboard => 14f,
                BeepControlStyle.DarkGlow => 13f,
                BeepControlStyle.DiscordStyle => 14f,
                BeepControlStyle.GradientModern => 14f,
                BeepControlStyle.GlassAcrylic => 14f,
                BeepControlStyle.Neumorphism => 14f,
                BeepControlStyle.Bootstrap => 14f,
                BeepControlStyle.FigmaCard => 13f,
                BeepControlStyle.PillRail => 13f,
                BeepControlStyle.Metro => 14f,                  // Metro standard
                BeepControlStyle.Office => 11f,                 // Office smaller (ribbon Style)
                BeepControlStyle.Gnome => 14f,                  // Gnome standard
                BeepControlStyle.Kde => 14f,                    // KDE standard
                BeepControlStyle.Cinnamon => 14f,               // Cinnamon standard
                BeepControlStyle.Elementary => 13f,             // Elementary slightly smaller
                BeepControlStyle.NeoBrutalist => 16f,           // Neo-Brutalist larger, bold
                BeepControlStyle.Gaming => 15f,                 // Gaming slightly larger
                BeepControlStyle.HighContrast => 16f,           // High contrast larger for readability
                BeepControlStyle.Neon => 14f,                   // Neon standard
                _ => 14f
            };
        }

        /// <summary>
        /// Get font weight for regular text
        /// </summary>
        public static FontStyle GetFontStyle(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => FontStyle.Regular,
                BeepControlStyle.iOS15 => FontStyle.Regular,
                BeepControlStyle.Fluent2 => FontStyle.Regular,
                BeepControlStyle.Minimal => FontStyle.Regular,
                BeepControlStyle.AntDesign => FontStyle.Regular,
                BeepControlStyle.MaterialYou => FontStyle.Regular,
                BeepControlStyle.Windows11Mica => FontStyle.Regular,
                BeepControlStyle.MacOSBigSur => FontStyle.Regular,
                BeepControlStyle.ChakraUI => FontStyle.Regular,
                BeepControlStyle.TailwindCard => FontStyle.Regular,
                BeepControlStyle.NotionMinimal => FontStyle.Regular,
                BeepControlStyle.VercelClean => FontStyle.Regular,
                BeepControlStyle.StripeDashboard => FontStyle.Regular,
                BeepControlStyle.DarkGlow => FontStyle.Regular,
                BeepControlStyle.DiscordStyle => FontStyle.Regular,
                BeepControlStyle.GradientModern => FontStyle.Regular,
                BeepControlStyle.GlassAcrylic => FontStyle.Regular,
                BeepControlStyle.Neumorphism => FontStyle.Regular,
                BeepControlStyle.Bootstrap => FontStyle.Regular,
                BeepControlStyle.FigmaCard => FontStyle.Regular,
                BeepControlStyle.PillRail => FontStyle.Regular,
                BeepControlStyle.Metro => FontStyle.Regular,
                BeepControlStyle.Office => FontStyle.Regular,
                BeepControlStyle.Gnome => FontStyle.Regular,
                BeepControlStyle.Kde => FontStyle.Regular,
                BeepControlStyle.Cinnamon => FontStyle.Regular,
                BeepControlStyle.Elementary => FontStyle.Regular,
                BeepControlStyle.NeoBrutalist => FontStyle.Bold,   // Neo-Brutalist always bold
                BeepControlStyle.Gaming => FontStyle.Bold,          // Gaming bold
                BeepControlStyle.HighContrast => FontStyle.Bold,    // High contrast bold for readability
                BeepControlStyle.Neon => FontStyle.Regular,
                _ => FontStyle.Regular
            };
        }

        /// <summary>
        /// Get font weight for selected/active text
        /// </summary>
        public static FontStyle GetActiveFontStyle(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => FontStyle.Bold,
                BeepControlStyle.iOS15 => FontStyle.Bold,
                BeepControlStyle.Fluent2 => FontStyle.Bold,
                BeepControlStyle.Minimal => FontStyle.Bold,
                BeepControlStyle.AntDesign => FontStyle.Bold,
                BeepControlStyle.MaterialYou => FontStyle.Bold,
                BeepControlStyle.Windows11Mica => FontStyle.Bold,
                BeepControlStyle.MacOSBigSur => FontStyle.Bold,
                BeepControlStyle.ChakraUI => FontStyle.Bold,
                BeepControlStyle.TailwindCard => FontStyle.Bold,
                BeepControlStyle.NotionMinimal => FontStyle.Regular,       // Notion minimal weight change
                BeepControlStyle.VercelClean => FontStyle.Regular,          // Vercel minimal weight change
                BeepControlStyle.StripeDashboard => FontStyle.Bold,
                BeepControlStyle.DarkGlow => FontStyle.Bold,
                BeepControlStyle.DiscordStyle => FontStyle.Bold,
                BeepControlStyle.GradientModern => FontStyle.Bold,
                BeepControlStyle.GlassAcrylic => FontStyle.Bold,
                BeepControlStyle.Neumorphism => FontStyle.Bold,
                BeepControlStyle.Bootstrap => FontStyle.Bold,
                BeepControlStyle.FigmaCard => FontStyle.Bold,
                BeepControlStyle.PillRail => FontStyle.Bold,
                BeepControlStyle.Metro => FontStyle.Bold,
                BeepControlStyle.Office => FontStyle.Bold,
                BeepControlStyle.Gnome => FontStyle.Bold,
                BeepControlStyle.Kde => FontStyle.Bold,
                BeepControlStyle.Cinnamon => FontStyle.Bold,
                BeepControlStyle.Elementary => FontStyle.Bold,
                BeepControlStyle.NeoBrutalist => FontStyle.Bold,    // Already bold, stay bold
                BeepControlStyle.Gaming => FontStyle.Bold,           // Already bold, stay bold
                BeepControlStyle.HighContrast => FontStyle.Bold,     // Already bold, stay bold
                BeepControlStyle.Neon => FontStyle.Bold,
                _ => FontStyle.Bold
            };
        }

        /// <summary>
        /// Get line height multiplier for text
        /// </summary>
        public static float GetLineHeight(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => 1.5f,
                BeepControlStyle.iOS15 => 1.4f,
                BeepControlStyle.Fluent2 => 1.428f,
                BeepControlStyle.Minimal => 1.5f,
                BeepControlStyle.AntDesign => 1.5715f,
                BeepControlStyle.MaterialYou => 1.5f,
                BeepControlStyle.Windows11Mica => 1.428f,
                BeepControlStyle.MacOSBigSur => 1.4f,
                BeepControlStyle.ChakraUI => 1.625f,
                BeepControlStyle.TailwindCard => 1.5f,
                BeepControlStyle.NotionMinimal => 1.5f,
                BeepControlStyle.VercelClean => 1.5f,
                BeepControlStyle.StripeDashboard => 1.5f,
                BeepControlStyle.DarkGlow => 1.6f,
                BeepControlStyle.DiscordStyle => 1.375f,
                BeepControlStyle.GradientModern => 1.5f,
                BeepControlStyle.GlassAcrylic => 1.428f,
                BeepControlStyle.Neumorphism => 1.5f,
                BeepControlStyle.Bootstrap => 1.5f,
                BeepControlStyle.FigmaCard => 1.5f,
                BeepControlStyle.PillRail => 1.5f,
                BeepControlStyle.Metro => 1.4f,                 // Metro tight
                BeepControlStyle.Office => 1.3f,                // Office compact
                BeepControlStyle.Gnome => 1.5f,                 // Gnome standard
                BeepControlStyle.Kde => 1.5f,                   // KDE standard
                BeepControlStyle.Cinnamon => 1.6f,              // Cinnamon generous
                BeepControlStyle.Elementary => 1.5f,            // Elementary standard
                BeepControlStyle.NeoBrutalist => 1.3f,          // Neo-Brutalist tight
                BeepControlStyle.Gaming => 1.4f,                // Gaming compact
                BeepControlStyle.HighContrast => 1.6f,          // High contrast spacious for readability
                BeepControlStyle.Neon => 1.5f,                  // Neon standard
                _ => 1.5f
            };
        }

        /// <summary>
        /// Get letter spacing for text (in pixels)
        /// </summary>
        public static float GetLetterSpacing(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 => 0.1f,
                BeepControlStyle.iOS15 => -0.2f,                    // iOS negative tracking
                BeepControlStyle.Fluent2 => 0f,
                BeepControlStyle.Minimal => 0.2f,                   // Minimal wider tracking
                BeepControlStyle.AntDesign => 0f,
                BeepControlStyle.MaterialYou => 0.1f,
                BeepControlStyle.Windows11Mica => 0f,
                BeepControlStyle.MacOSBigSur => -0.2f,              // macOS negative tracking
                BeepControlStyle.ChakraUI => 0f,
                BeepControlStyle.TailwindCard => 0f,
                BeepControlStyle.NotionMinimal => 0.2f,             // Notion wider tracking
                BeepControlStyle.VercelClean => 0.2f,               // Vercel wider tracking
                BeepControlStyle.StripeDashboard => 0f,
                BeepControlStyle.DarkGlow => 0.5f,                  // Monospace wider tracking
                BeepControlStyle.DiscordStyle => 0f,
                BeepControlStyle.GradientModern => 0.3f,
                BeepControlStyle.GlassAcrylic => 0f,
                BeepControlStyle.Neumorphism => 0.5f,
                BeepControlStyle.Bootstrap => 0f,
                BeepControlStyle.FigmaCard => 0.2f,
                BeepControlStyle.PillRail => 0f,
                BeepControlStyle.Metro => 0f,                   // Metro normal tracking
                BeepControlStyle.Office => 0f,                  // Office normal tracking
                BeepControlStyle.Gnome => 0f,                   // Gnome normal tracking
                BeepControlStyle.Kde => 0f,                     // KDE normal tracking
                BeepControlStyle.Cinnamon => 0.1f,              // Cinnamon slightly wider
                BeepControlStyle.Elementary => 0.2f,            // Elementary wider tracking
                BeepControlStyle.NeoBrutalist => 0.3f,          // Neo-Brutalist wider for impact
                BeepControlStyle.Gaming => 0.5f,                // Gaming wide tracking
                BeepControlStyle.HighContrast => 0.3f,          // High contrast wider for readability
                BeepControlStyle.Neon => 0.4f,                  // Neon wide tracking for glow
                _ => 0f
            };
        }

        /// <summary>
        /// Check if Style uses monospace fonts
        /// </summary>
        public static bool IsMonospace(BeepControlStyle style)
        {
            return style == BeepControlStyle.DarkGlow;
        }

        /// <summary>
        /// Get font for a specific Style with size
        /// </summary>
        public static Font GetFont(BeepControlStyle style, float? size = null, FontStyle? fontStyle = null)
        {
            string family = GetFontFamily(style);
            float fontSize = size ?? GetFontSize(style);
            FontStyle style_ = fontStyle ?? GetFontStyle(style);
            
            // Try to use the first font in the family list
            string[] families = family.Split(',');
            foreach (var fam in families)
            {
                string trimmed = fam.Trim();
                try
                {
                    return new Font(trimmed, fontSize, style_);
                }
                catch
                {
                    // Font not available, try next
                    continue;
                }
            }
            
            // Fallback to default
            return new Font("Segoe UI", fontSize, style_);
        }
    }
}
