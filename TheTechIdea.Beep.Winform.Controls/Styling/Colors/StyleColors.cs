using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.Colors
{
    /// <summary>
    /// Centralized color definitions for all BeepControlStyle design systems
    /// Each Style has distinct, recognizable colors
    /// </summary>
    public static class StyleColors
    {
        /// <summary>
        /// Get background color for a specific Style
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
                BeepControlStyle.Metro => Color.FromArgb(240, 240, 240),            // Flat gray
                BeepControlStyle.Office => Color.FromArgb(255, 255, 255),           // Clean white
                BeepControlStyle.Gnome => Color.FromArgb(246, 245, 244),            // Warm light gray (Adwaita)
                BeepControlStyle.Kde => Color.FromArgb(239, 240, 241),              // Cool light gray (Breeze)
                BeepControlStyle.Cinnamon => Color.FromArgb(245, 245, 245),         // Light gray
                BeepControlStyle.Elementary => Color.FromArgb(250, 250, 250),       // Off-white
                BeepControlStyle.NeoBrutalist => Color.FromArgb(255, 255, 0),       // Bold yellow
                BeepControlStyle.Gaming => Color.FromArgb(18, 18, 18),              // Dark charcoal
                BeepControlStyle.HighContrast => Color.FromArgb(255, 255, 255),     // Pure white (WCAG AAA)
                BeepControlStyle.Neon => Color.FromArgb(10, 10, 20),                // Deep dark blue
                // Additional modern styles
                BeepControlStyle.Modern => Color.FromArgb(245, 245, 247),           // Light gray-white (Apple-inspired)
                BeepControlStyle.ArcLinux => Color.FromArgb(56, 60, 74),            // Arc dark background
                BeepControlStyle.Brutalist => Color.FromArgb(242, 242, 242),        // Clean light gray
                BeepControlStyle.Cartoon => Color.FromArgb(255, 248, 220),          // Warm cream
                BeepControlStyle.ChatBubble => Color.FromArgb(240, 242, 245),       // Chat light gray
                BeepControlStyle.Cyberpunk => Color.FromArgb(15, 15, 35),           // Deep dark blue-purple
                BeepControlStyle.Dracula => Color.FromArgb(40, 42, 54),             // Dracula background
                BeepControlStyle.Glassmorphism => Color.FromArgb(180, 255, 255, 255), // Translucent white
                BeepControlStyle.Holographic => Color.FromArgb(20, 20, 40),         // Deep dark
                BeepControlStyle.GruvBox => Color.FromArgb(40, 40, 40),             // Gruvbox dark
                BeepControlStyle.Metro2 => Color.FromArgb(37, 37, 38),              // VS Code dark
                BeepControlStyle.Nord => Color.FromArgb(46, 52, 64),                // Nord dark
                BeepControlStyle.Nordic => Color.FromArgb(236, 239, 244),           // Nord light
                BeepControlStyle.OneDark => Color.FromArgb(40, 44, 52),             // One Dark background
                BeepControlStyle.Paper => Color.FromArgb(253, 251, 247),            // Paper cream
                BeepControlStyle.Solarized => Color.FromArgb(253, 246, 227),        // Solarized light
                BeepControlStyle.Terminal => Color.FromArgb(30, 30, 30),            // Terminal black
                BeepControlStyle.Tokyo => Color.FromArgb(26, 27, 38),               // Tokyo Night dark
                BeepControlStyle.Ubuntu => Color.FromArgb(48, 48, 48),              // Ubuntu dark
                BeepControlStyle.Retro => Color.FromArgb(255, 251, 235),            // Retro cream
                BeepControlStyle.Apple => Color.FromArgb(255, 255, 255),            // Apple clean white
                BeepControlStyle.Fluent => Color.FromArgb(249, 249, 249),           // Fluent light
                BeepControlStyle.Material => Color.FromArgb(255, 255, 255),         // Material white
                BeepControlStyle.WebFramework => Color.FromArgb(250, 250, 250),     // Web light
                BeepControlStyle.Effect => Color.FromArgb(28, 28, 35),              // Effect dark
                _ => Color.FromArgb(250, 250, 250)
            };
        }

        /// <summary>
        /// Get primary/accent color for a specific Style
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
                BeepControlStyle.Metro => Color.FromArgb(0, 120, 212),              // Windows Blue
                BeepControlStyle.Office => Color.FromArgb(0, 120, 212),             // Office Blue
                BeepControlStyle.Gnome => Color.FromArgb(53, 132, 228),             // Gnome Blue (Adwaita)
                BeepControlStyle.Kde => Color.FromArgb(61, 174, 233),               // KDE Blue (Breeze)
                BeepControlStyle.Cinnamon => Color.FromArgb(115, 210, 22),          // Cinnamon Green
                BeepControlStyle.Elementary => Color.FromArgb(100, 125, 255),       // Elementary Blue
                BeepControlStyle.NeoBrutalist => Color.FromArgb(0, 0, 0),           // Black
                BeepControlStyle.Gaming => Color.FromArgb(0, 255, 127),             // Bright green
                BeepControlStyle.HighContrast => Color.FromArgb(0, 0, 0),           // Pure black (WCAG AAA)
                BeepControlStyle.Neon => Color.FromArgb(0, 255, 255),               // Cyan neon
                // Additional modern styles
                BeepControlStyle.Modern => Color.FromArgb(0, 122, 255),             // Modern blue accent
                BeepControlStyle.ArcLinux => Color.FromArgb(82, 148, 226),          // Arc blue
                BeepControlStyle.Brutalist => Color.FromArgb(0, 0, 0),              // Brutalist black
                BeepControlStyle.Cartoon => Color.FromArgb(255, 99, 71),            // Cartoon red
                BeepControlStyle.ChatBubble => Color.FromArgb(0, 132, 255),         // Messenger blue
                BeepControlStyle.Cyberpunk => Color.FromArgb(255, 0, 128),          // Cyberpunk magenta
                BeepControlStyle.Dracula => Color.FromArgb(189, 147, 249),          // Dracula purple
                BeepControlStyle.Glassmorphism => Color.FromArgb(99, 102, 241),     // Glass purple
                BeepControlStyle.Holographic => Color.FromArgb(139, 92, 246),       // Holographic purple
                BeepControlStyle.GruvBox => Color.FromArgb(214, 93, 14),            // Gruvbox orange
                BeepControlStyle.Metro2 => Color.FromArgb(0, 122, 204),             // VS Code blue
                BeepControlStyle.Nord => Color.FromArgb(136, 192, 208),             // Nord frost blue
                BeepControlStyle.Nordic => Color.FromArgb(94, 129, 172),            // Nordic blue
                BeepControlStyle.OneDark => Color.FromArgb(97, 175, 239),           // One Dark blue
                BeepControlStyle.Paper => Color.FromArgb(66, 133, 244),             // Paper blue
                BeepControlStyle.Solarized => Color.FromArgb(38, 139, 210),         // Solarized blue
                BeepControlStyle.Terminal => Color.FromArgb(0, 255, 0),             // Terminal green
                BeepControlStyle.Tokyo => Color.FromArgb(122, 162, 247),            // Tokyo Night blue
                BeepControlStyle.Ubuntu => Color.FromArgb(233, 84, 32),             // Ubuntu orange
                BeepControlStyle.Retro => Color.FromArgb(64, 64, 64),               // Retro dark
                BeepControlStyle.Apple => Color.FromArgb(0, 122, 255),              // Apple blue
                BeepControlStyle.Fluent => Color.FromArgb(0, 120, 212),             // Fluent blue
                BeepControlStyle.Material => Color.FromArgb(98, 0, 238),            // Material purple
                BeepControlStyle.WebFramework => Color.FromArgb(59, 130, 246),      // Web blue
                BeepControlStyle.Effect => Color.FromArgb(139, 92, 246),            // Effect purple
                _ => Color.FromArgb(0, 120, 215)
            };
        }

        /// <summary>
        /// Get secondary/surface color for a specific Style
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
                BeepControlStyle.Metro => Color.FromArgb(230, 230, 230),            // Light flat gray
                BeepControlStyle.Office => Color.FromArgb(243, 242, 241),           // Office gray
                BeepControlStyle.Gnome => Color.FromArgb(220, 218, 214),            // Warm gray
                BeepControlStyle.Kde => Color.FromArgb(239, 240, 241),              // Breeze gray
                BeepControlStyle.Cinnamon => Color.FromArgb(235, 235, 235),         // Light gray
                BeepControlStyle.Elementary => Color.FromArgb(245, 245, 245),       // Light gray
                BeepControlStyle.NeoBrutalist => Color.FromArgb(255, 0, 255),       // Magenta
                BeepControlStyle.Gaming => Color.FromArgb(30, 30, 30),              // Dark gray
                BeepControlStyle.HighContrast => Color.FromArgb(240, 240, 240),     // Light gray
                BeepControlStyle.Neon => Color.FromArgb(255, 0, 255),               // Magenta neon
                // Additional modern styles
                BeepControlStyle.Modern => Color.FromArgb(229, 231, 235),           // Modern secondary gray
                BeepControlStyle.ArcLinux => Color.FromArgb(64, 69, 82),            // Arc secondary
                BeepControlStyle.Brutalist => Color.FromArgb(220, 220, 220),        // Brutalist gray
                BeepControlStyle.Cartoon => Color.FromArgb(255, 218, 185),          // Cartoon peach
                BeepControlStyle.ChatBubble => Color.FromArgb(225, 230, 235),       // Chat secondary
                BeepControlStyle.Cyberpunk => Color.FromArgb(25, 25, 50),           // Cyberpunk secondary
                BeepControlStyle.Dracula => Color.FromArgb(68, 71, 90),             // Dracula selection
                BeepControlStyle.Glassmorphism => Color.FromArgb(150, 255, 255, 255), // Glass secondary
                BeepControlStyle.Holographic => Color.FromArgb(30, 30, 55),         // Holographic secondary
                BeepControlStyle.GruvBox => Color.FromArgb(60, 56, 54),             // Gruvbox secondary
                BeepControlStyle.Metro2 => Color.FromArgb(51, 51, 51),              // VS Code secondary
                BeepControlStyle.Nord => Color.FromArgb(59, 66, 82),                // Nord secondary
                BeepControlStyle.Nordic => Color.FromArgb(216, 222, 233),           // Nordic secondary
                BeepControlStyle.OneDark => Color.FromArgb(55, 60, 70),             // One Dark secondary
                BeepControlStyle.Paper => Color.FromArgb(232, 234, 237),            // Paper secondary
                BeepControlStyle.Solarized => Color.FromArgb(238, 232, 213),        // Solarized secondary
                BeepControlStyle.Terminal => Color.FromArgb(50, 50, 50),            // Terminal secondary
                BeepControlStyle.Tokyo => Color.FromArgb(36, 40, 59),               // Tokyo Night secondary
                BeepControlStyle.Ubuntu => Color.FromArgb(77, 77, 77),              // Ubuntu secondary
                BeepControlStyle.Retro => Color.FromArgb(238, 232, 213),            // Retro secondary
                BeepControlStyle.Apple => Color.FromArgb(242, 242, 247),            // Apple secondary
                BeepControlStyle.Fluent => Color.FromArgb(239, 239, 239),           // Fluent secondary
                BeepControlStyle.Material => Color.FromArgb(187, 134, 252),         // Material secondary
                BeepControlStyle.WebFramework => Color.FromArgb(243, 244, 246),     // Web secondary
                BeepControlStyle.Effect => Color.FromArgb(45, 45, 55),              // Effect secondary
                _ => Color.FromArgb(240, 240, 240)
            };
        }

        /// <summary>
        /// Get text/foreground color for a specific Style
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
                BeepControlStyle.Metro => Color.FromArgb(0, 0, 0),                  // Black
                BeepControlStyle.Office => Color.FromArgb(68, 68, 68),              // Dark gray
                BeepControlStyle.Gnome => Color.FromArgb(36, 31, 49),               // Dark (Adwaita)
                BeepControlStyle.Kde => Color.FromArgb(35, 38, 41),                 // Dark (Breeze)
                BeepControlStyle.Cinnamon => Color.FromArgb(50, 50, 50),            // Dark gray
                BeepControlStyle.Elementary => Color.FromArgb(51, 51, 51),          // Dark gray
                BeepControlStyle.NeoBrutalist => Color.FromArgb(0, 0, 0),           // Black
                BeepControlStyle.Gaming => Color.FromArgb(255, 255, 255),           // White
                BeepControlStyle.HighContrast => Color.FromArgb(0, 0, 0),           // Pure black (WCAG AAA)
                BeepControlStyle.Neon => Color.FromArgb(255, 255, 255),             // White
                // Additional modern styles - DARK TEXT for light backgrounds, LIGHT TEXT for dark backgrounds
                BeepControlStyle.Modern => Color.FromArgb(28, 28, 30),              // Modern dark text (excellent contrast)
                BeepControlStyle.ArcLinux => Color.FromArgb(211, 218, 227),         // Arc light text
                BeepControlStyle.Brutalist => Color.FromArgb(0, 0, 0),              // Brutalist black text
                BeepControlStyle.Cartoon => Color.FromArgb(45, 45, 45),             // Cartoon dark text
                BeepControlStyle.ChatBubble => Color.FromArgb(28, 28, 30),          // Chat dark text
                BeepControlStyle.Cyberpunk => Color.FromArgb(255, 255, 255),        // Cyberpunk white text
                BeepControlStyle.Dracula => Color.FromArgb(248, 248, 242),          // Dracula foreground
                BeepControlStyle.Glassmorphism => Color.FromArgb(28, 28, 30),       // Glass dark text
                BeepControlStyle.Holographic => Color.FromArgb(255, 255, 255),      // Holographic white text
                BeepControlStyle.GruvBox => Color.FromArgb(235, 219, 178),          // Gruvbox foreground
                BeepControlStyle.Metro2 => Color.FromArgb(212, 212, 212),           // VS Code light text
                BeepControlStyle.Nord => Color.FromArgb(216, 222, 233),             // Nord snow
                BeepControlStyle.Nordic => Color.FromArgb(46, 52, 64),              // Nordic dark text
                BeepControlStyle.OneDark => Color.FromArgb(171, 178, 191),          // One Dark foreground
                BeepControlStyle.Paper => Color.FromArgb(32, 33, 36),               // Paper dark text
                BeepControlStyle.Solarized => Color.FromArgb(101, 123, 131),        // Solarized base00
                BeepControlStyle.Terminal => Color.FromArgb(0, 255, 0),             // Terminal green text
                BeepControlStyle.Tokyo => Color.FromArgb(169, 177, 214),            // Tokyo Night foreground
                BeepControlStyle.Ubuntu => Color.FromArgb(255, 255, 255),           // Ubuntu white text
                BeepControlStyle.Retro => Color.FromArgb(51, 51, 51),               // Retro dark text
                BeepControlStyle.Apple => Color.FromArgb(28, 28, 30),               // Apple dark text
                BeepControlStyle.Fluent => Color.FromArgb(28, 28, 30),              // Fluent dark text
                BeepControlStyle.Material => Color.FromArgb(33, 33, 33),            // Material dark text
                BeepControlStyle.WebFramework => Color.FromArgb(17, 24, 39),        // Web dark text
                BeepControlStyle.Effect => Color.FromArgb(255, 255, 255),           // Effect white text
                _ => Color.FromArgb(50, 50, 50)
            };
        }

        /// <summary>
        /// Get border color for a specific Style
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
                BeepControlStyle.Metro => Color.FromArgb(200, 200, 200),            // Medium gray
                BeepControlStyle.Office => Color.FromArgb(0, 120, 212),             // Office blue
                BeepControlStyle.Gnome => Color.FromArgb(205, 199, 194),            // Warm border
                BeepControlStyle.Kde => Color.FromArgb(189, 195, 199),              // Breeze border
                BeepControlStyle.Cinnamon => Color.FromArgb(210, 210, 210),         // Medium gray
                BeepControlStyle.Elementary => Color.FromArgb(220, 220, 220),       // Light gray
                BeepControlStyle.NeoBrutalist => Color.FromArgb(0, 0, 0),           // Black (thick)
                BeepControlStyle.Gaming => Color.FromArgb(0, 255, 127),             // Bright green
                BeepControlStyle.HighContrast => Color.FromArgb(0, 0, 0),           // Pure black (WCAG AAA)
                BeepControlStyle.Neon => Color.FromArgb(0, 255, 255),               // Cyan neon
                // Additional modern styles
                BeepControlStyle.Modern => Color.FromArgb(209, 213, 219),           // Modern subtle border
                BeepControlStyle.ArcLinux => Color.FromArgb(75, 81, 98),            // Arc border
                BeepControlStyle.Brutalist => Color.FromArgb(0, 0, 0),              // Brutalist black border
                BeepControlStyle.Cartoon => Color.FromArgb(45, 45, 45),             // Cartoon dark border
                BeepControlStyle.ChatBubble => Color.FromArgb(209, 213, 219),       // Chat border
                BeepControlStyle.Cyberpunk => Color.FromArgb(0, 255, 255),          // Cyberpunk cyan
                BeepControlStyle.Dracula => Color.FromArgb(68, 71, 90),             // Dracula comment
                BeepControlStyle.Glassmorphism => Color.FromArgb(100, 255, 255, 255), // Glass border
                BeepControlStyle.Holographic => Color.FromArgb(139, 92, 246),       // Holographic border
                BeepControlStyle.GruvBox => Color.FromArgb(146, 131, 116),          // Gruvbox border
                BeepControlStyle.Metro2 => Color.FromArgb(69, 69, 69),              // VS Code border
                BeepControlStyle.Nord => Color.FromArgb(76, 86, 106),               // Nord border
                BeepControlStyle.Nordic => Color.FromArgb(216, 222, 233),           // Nordic border
                BeepControlStyle.OneDark => Color.FromArgb(62, 68, 81),             // One Dark border
                BeepControlStyle.Paper => Color.FromArgb(218, 220, 224),            // Paper border
                BeepControlStyle.Solarized => Color.FromArgb(147, 161, 161),        // Solarized border
                BeepControlStyle.Terminal => Color.FromArgb(0, 255, 0),             // Terminal green
                BeepControlStyle.Tokyo => Color.FromArgb(61, 66, 91),               // Tokyo Night border
                BeepControlStyle.Ubuntu => Color.FromArgb(233, 84, 32),             // Ubuntu orange
                BeepControlStyle.Retro => Color.FromArgb(180, 180, 180),            // Retro border
                BeepControlStyle.Apple => Color.FromArgb(209, 209, 214),            // Apple border
                BeepControlStyle.Fluent => Color.FromArgb(229, 229, 229),           // Fluent border
                BeepControlStyle.Material => Color.FromArgb(189, 189, 189),         // Material border
                BeepControlStyle.WebFramework => Color.FromArgb(229, 231, 235),     // Web border
                BeepControlStyle.Effect => Color.FromArgb(80, 80, 100),             // Effect border
                _ => Color.FromArgb(200, 200, 200)
            };
        }

        /// <summary>
        /// Get hover/interaction color for a specific Style
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
                BeepControlStyle.Metro => Color.FromArgb(0, 103, 192),              // Darker Windows blue
                BeepControlStyle.Office => Color.FromArgb(0, 103, 192),             // Darker Office blue
                BeepControlStyle.Gnome => Color.FromArgb(26, 95, 180),              // Darker Gnome blue
                BeepControlStyle.Kde => Color.FromArgb(41, 128, 185),               // Darker KDE blue
                BeepControlStyle.Cinnamon => Color.FromArgb(78, 154, 6),            // Darker green
                BeepControlStyle.Elementary => Color.FromArgb(64, 78, 237),         // Darker elementary blue
                BeepControlStyle.NeoBrutalist => Color.FromArgb(230, 230, 0),       // Darker yellow
                BeepControlStyle.Gaming => Color.FromArgb(0, 200, 100),             // Darker green
                BeepControlStyle.HighContrast => Color.FromArgb(200, 200, 200),     // Light gray
                BeepControlStyle.Neon => Color.FromArgb(0, 200, 200),               // Darker cyan
                // Additional modern styles
                BeepControlStyle.Modern => Color.FromArgb(229, 231, 235),           // Modern hover
                BeepControlStyle.ArcLinux => Color.FromArgb(75, 81, 98),            // Arc hover
                BeepControlStyle.Brutalist => Color.FromArgb(200, 200, 200),        // Brutalist hover
                BeepControlStyle.Cartoon => Color.FromArgb(255, 200, 150),          // Cartoon hover
                BeepControlStyle.ChatBubble => Color.FromArgb(220, 225, 230),       // Chat hover
                BeepControlStyle.Cyberpunk => Color.FromArgb(35, 35, 60),           // Cyberpunk hover
                BeepControlStyle.Dracula => Color.FromArgb(68, 71, 90),             // Dracula hover
                BeepControlStyle.Glassmorphism => Color.FromArgb(50, 255, 255, 255), // Glass hover
                BeepControlStyle.Holographic => Color.FromArgb(40, 40, 70),         // Holographic hover
                BeepControlStyle.GruvBox => Color.FromArgb(80, 73, 69),             // Gruvbox hover
                BeepControlStyle.Metro2 => Color.FromArgb(60, 60, 60),              // VS Code hover
                BeepControlStyle.Nord => Color.FromArgb(67, 76, 94),                // Nord hover
                BeepControlStyle.Nordic => Color.FromArgb(229, 233, 240),           // Nordic hover
                BeepControlStyle.OneDark => Color.FromArgb(50, 55, 65),             // One Dark hover
                BeepControlStyle.Paper => Color.FromArgb(241, 243, 244),            // Paper hover
                BeepControlStyle.Solarized => Color.FromArgb(238, 232, 213),        // Solarized hover
                BeepControlStyle.Terminal => Color.FromArgb(50, 50, 50),            // Terminal hover
                BeepControlStyle.Tokyo => Color.FromArgb(42, 46, 66),               // Tokyo Night hover
                BeepControlStyle.Ubuntu => Color.FromArgb(90, 90, 90),              // Ubuntu hover
                BeepControlStyle.Retro => Color.FromArgb(245, 240, 225),            // Retro hover
                BeepControlStyle.Apple => Color.FromArgb(229, 229, 234),            // Apple hover
                BeepControlStyle.Fluent => Color.FromArgb(233, 233, 233),           // Fluent hover
                BeepControlStyle.Material => Color.FromArgb(245, 245, 245),         // Material hover
                BeepControlStyle.WebFramework => Color.FromArgb(243, 244, 246),     // Web hover
                BeepControlStyle.Effect => Color.FromArgb(40, 40, 50),              // Effect hover
                _ => Color.FromArgb(240, 240, 240)
            };
        }

        /// <summary>
        /// Get selection/active color for a specific Style
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
                BeepControlStyle.Metro => Color.FromArgb(0, 120, 212),              // Windows blue
                BeepControlStyle.Office => Color.FromArgb(200, 0, 120, 212),        // Semi-transparent Office blue
                BeepControlStyle.Gnome => Color.FromArgb(200, 53, 132, 228),        // Semi-transparent Gnome blue
                BeepControlStyle.Kde => Color.FromArgb(200, 61, 174, 233),          // Semi-transparent KDE blue
                BeepControlStyle.Cinnamon => Color.FromArgb(200, 115, 210, 22),     // Semi-transparent green
                BeepControlStyle.Elementary => Color.FromArgb(200, 100, 125, 255),  // Semi-transparent elementary blue
                BeepControlStyle.NeoBrutalist => Color.FromArgb(255, 0, 255),       // Magenta
                BeepControlStyle.Gaming => Color.FromArgb(0, 255, 127),             // Bright green
                BeepControlStyle.HighContrast => Color.FromArgb(255, 255, 0),       // Yellow (WCAG AAA)
                BeepControlStyle.Neon => Color.FromArgb(0, 255, 255),               // Cyan neon
                // Additional modern styles
                BeepControlStyle.Modern => Color.FromArgb(200, 0, 122, 255),        // Modern selection
                BeepControlStyle.ArcLinux => Color.FromArgb(82, 148, 226),          // Arc selection
                BeepControlStyle.Brutalist => Color.FromArgb(180, 180, 180),        // Brutalist selection
                BeepControlStyle.Cartoon => Color.FromArgb(255, 150, 100),          // Cartoon selection
                BeepControlStyle.ChatBubble => Color.FromArgb(200, 0, 132, 255),    // Chat selection
                BeepControlStyle.Cyberpunk => Color.FromArgb(255, 0, 128),          // Cyberpunk magenta
                BeepControlStyle.Dracula => Color.FromArgb(68, 71, 90),             // Dracula selection
                BeepControlStyle.Glassmorphism => Color.FromArgb(100, 99, 102, 241), // Glass selection
                BeepControlStyle.Holographic => Color.FromArgb(139, 92, 246),       // Holographic selection
                BeepControlStyle.GruvBox => Color.FromArgb(131, 165, 152),          // Gruvbox selection
                BeepControlStyle.Metro2 => Color.FromArgb(0, 122, 204),             // VS Code selection
                BeepControlStyle.Nord => Color.FromArgb(136, 192, 208),             // Nord selection
                BeepControlStyle.Nordic => Color.FromArgb(94, 129, 172),            // Nordic selection
                BeepControlStyle.OneDark => Color.FromArgb(97, 175, 239),           // One Dark selection
                BeepControlStyle.Paper => Color.FromArgb(66, 133, 244),             // Paper selection
                BeepControlStyle.Solarized => Color.FromArgb(38, 139, 210),         // Solarized selection
                BeepControlStyle.Terminal => Color.FromArgb(0, 200, 0),             // Terminal selection
                BeepControlStyle.Tokyo => Color.FromArgb(122, 162, 247),            // Tokyo Night selection
                BeepControlStyle.Ubuntu => Color.FromArgb(233, 84, 32),             // Ubuntu selection
                BeepControlStyle.Retro => Color.FromArgb(200, 200, 200),            // Retro selection
                BeepControlStyle.Apple => Color.FromArgb(0, 122, 255),              // Apple selection
                BeepControlStyle.Fluent => Color.FromArgb(0, 120, 212),             // Fluent selection
                BeepControlStyle.Material => Color.FromArgb(98, 0, 238),            // Material selection
                BeepControlStyle.WebFramework => Color.FromArgb(59, 130, 246),      // Web selection
                BeepControlStyle.Effect => Color.FromArgb(139, 92, 246),            // Effect selection
                _ => Color.FromArgb(220, 220, 220)
            };
        }

        internal static Color GetPressed(BeepControlStyle style)
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
                BeepControlStyle.Metro => Color.FromArgb(0, 90, 158),               // Darkest Windows blue
                BeepControlStyle.Office => Color.FromArgb(0, 90, 158),              // Darkest Office blue
                BeepControlStyle.Gnome => Color.FromArgb(30, 80, 150),              // Darkest Gnome blue
                BeepControlStyle.Kde => Color.FromArgb(30, 100, 140),               // Darkest KDE blue
                BeepControlStyle.Cinnamon => Color.FromArgb(55, 110, 6),            // Darkest green
                BeepControlStyle.Elementary => Color.FromArgb(50, 60, 180),         // Darkest elementary blue
                BeepControlStyle.NeoBrutalist => Color.FromArgb(200, 0, 200),       // Darker magenta
                BeepControlStyle.Gaming => Color.FromArgb(0, 150, 75),              // Darkest green
                BeepControlStyle.HighContrast => Color.FromArgb(180, 180, 0),       // Darker yellow
                BeepControlStyle.Neon => Color.FromArgb(0, 180, 180),               // Darkest cyan
                // Additional modern styles
                BeepControlStyle.Modern => Color.FromArgb(0, 90, 200),              // Modern pressed
                BeepControlStyle.ArcLinux => Color.FromArgb(60, 120, 200),          // Arc pressed
                BeepControlStyle.Brutalist => Color.FromArgb(160, 160, 160),        // Brutalist pressed
                BeepControlStyle.Cartoon => Color.FromArgb(255, 100, 50),           // Cartoon pressed
                BeepControlStyle.ChatBubble => Color.FromArgb(0, 100, 200),         // Chat pressed
                BeepControlStyle.Cyberpunk => Color.FromArgb(200, 0, 100),          // Cyberpunk pressed
                BeepControlStyle.Dracula => Color.FromArgb(50, 52, 70),             // Dracula pressed
                BeepControlStyle.Glassmorphism => Color.FromArgb(80, 80, 90, 220),  // Glass pressed
                BeepControlStyle.Holographic => Color.FromArgb(100, 60, 200),       // Holographic pressed
                BeepControlStyle.GruvBox => Color.FromArgb(100, 90, 80),            // Gruvbox pressed
                BeepControlStyle.Metro2 => Color.FromArgb(0, 90, 160),              // VS Code pressed
                BeepControlStyle.Nord => Color.FromArgb(100, 150, 180),             // Nord pressed
                BeepControlStyle.Nordic => Color.FromArgb(70, 100, 140),            // Nordic pressed
                BeepControlStyle.OneDark => Color.FromArgb(70, 130, 200),           // One Dark pressed
                BeepControlStyle.Paper => Color.FromArgb(50, 100, 200),             // Paper pressed
                BeepControlStyle.Solarized => Color.FromArgb(30, 110, 180),         // Solarized pressed
                BeepControlStyle.Terminal => Color.FromArgb(0, 150, 0),             // Terminal pressed
                BeepControlStyle.Tokyo => Color.FromArgb(90, 130, 220),             // Tokyo Night pressed
                BeepControlStyle.Ubuntu => Color.FromArgb(200, 60, 20),             // Ubuntu pressed
                BeepControlStyle.Retro => Color.FromArgb(180, 180, 180),            // Retro pressed
                BeepControlStyle.Apple => Color.FromArgb(0, 90, 200),               // Apple pressed
                BeepControlStyle.Fluent => Color.FromArgb(0, 90, 180),              // Fluent pressed
                BeepControlStyle.Material => Color.FromArgb(80, 0, 200),            // Material pressed
                BeepControlStyle.WebFramework => Color.FromArgb(40, 100, 200),      // Web pressed
                BeepControlStyle.Effect => Color.FromArgb(100, 60, 200),            // Effect pressed
                _ => Color.FromArgb(220, 220, 220)
            };
        }

        internal static Color GetSurface(BeepControlStyle style)
        {
            // Surface colors are aligned with the "secondary" / card-surface values
            // so that backgrounds, paths, and borders share a coherent base.
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
                BeepControlStyle.TailwindCard => Color.FromArgb(249, 250, 251),     // Card surface (matches secondary)
                BeepControlStyle.NotionMinimal => Color.FromArgb(247, 247, 245),    // Block surface (matches secondary)
                BeepControlStyle.VercelClean => Color.FromArgb(255, 255, 255),      // Pure white
                BeepControlStyle.StripeDashboard => Color.FromArgb(242, 244, 248),  // Card surface (matches secondary)
                BeepControlStyle.DarkGlow => Color.FromArgb(24, 24, 27),            // Deep charcoal
                BeepControlStyle.DiscordStyle => Color.FromArgb(47, 49, 54),        // Dark slate gray
                BeepControlStyle.GradientModern => Color.FromArgb(58, 123, 213),    // Blue (gradient start)
                BeepControlStyle.GlassAcrylic => Color.FromArgb(240, 255, 255, 255), // Translucent white
                BeepControlStyle.Neumorphism => Color.FromArgb(225, 227, 230),      // Soft gray
                BeepControlStyle.Bootstrap => Color.FromArgb(255, 255, 255),        // White
                BeepControlStyle.FigmaCard => Color.FromArgb(255, 255, 255),        // Pure white
                BeepControlStyle.PillRail => Color.FromArgb(245, 245, 247),         // Light gray-blue
                // Additional modern styles
                BeepControlStyle.Modern => Color.FromArgb(250, 250, 252),           // Modern surface
                BeepControlStyle.ArcLinux => Color.FromArgb(60, 64, 78),            // Arc surface
                BeepControlStyle.Brutalist => Color.FromArgb(242, 242, 242),        // Brutalist surface
                BeepControlStyle.Cartoon => Color.FromArgb(255, 252, 240),          // Cartoon surface
                BeepControlStyle.ChatBubble => Color.FromArgb(245, 247, 250),       // Chat surface
                BeepControlStyle.Cyberpunk => Color.FromArgb(20, 20, 40),           // Cyberpunk surface
                BeepControlStyle.Dracula => Color.FromArgb(44, 44, 58),             // Dracula surface
                BeepControlStyle.Glassmorphism => Color.FromArgb(200, 255, 255, 255), // Glass surface
                BeepControlStyle.Holographic => Color.FromArgb(25, 25, 45),         // Holographic surface
                BeepControlStyle.GruvBox => Color.FromArgb(50, 48, 47),             // Gruvbox surface
                BeepControlStyle.Metro2 => Color.FromArgb(45, 45, 45),              // VS Code surface
                BeepControlStyle.Nord => Color.FromArgb(52, 60, 78),                // Nord surface
                BeepControlStyle.Nordic => Color.FromArgb(242, 244, 248),           // Nordic surface
                BeepControlStyle.OneDark => Color.FromArgb(45, 48, 58),             // One Dark surface
                BeepControlStyle.Paper => Color.FromArgb(255, 252, 248),            // Paper surface
                BeepControlStyle.Solarized => Color.FromArgb(253, 246, 227),        // Solarized surface
                BeepControlStyle.Terminal => Color.FromArgb(35, 35, 35),            // Terminal surface
                BeepControlStyle.Tokyo => Color.FromArgb(30, 32, 48),               // Tokyo Night surface
                BeepControlStyle.Ubuntu => Color.FromArgb(55, 55, 55),              // Ubuntu surface
                BeepControlStyle.Retro => Color.FromArgb(252, 248, 235),            // Retro surface
                BeepControlStyle.Apple => Color.FromArgb(255, 255, 255),            // Apple surface
                BeepControlStyle.Fluent => Color.FromArgb(252, 252, 252),           // Fluent surface
                BeepControlStyle.Material => Color.FromArgb(255, 255, 255),         // Material surface
                BeepControlStyle.WebFramework => Color.FromArgb(252, 252, 252),     // Web surface
                BeepControlStyle.Effect => Color.FromArgb(32, 32, 40),              // Effect surface
                BeepControlStyle.Metro => Color.FromArgb(240, 240, 240),            // Metro surface
                BeepControlStyle.Office => Color.FromArgb(255, 255, 255),           // Office surface
                BeepControlStyle.NeoBrutalist => Color.FromArgb(255, 255, 0),       // NeoBrutalist surface
                BeepControlStyle.Gaming => Color.FromArgb(20, 20, 20),              // Gaming surface
                BeepControlStyle.HighContrast => Color.FromArgb(255, 255, 255),     // HighContrast surface
                BeepControlStyle.Neon => Color.FromArgb(15, 15, 25),                // Neon surface
                _ => Color.FromArgb(250, 250, 250)
            };
        }

    }
}
