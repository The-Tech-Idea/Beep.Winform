using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Switchs.Helpers.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Switchs.Helpers
{
    /// <summary>
    /// Factory for creating switch painter instances based on BeepControlStyle
    /// Follows the same pattern as BeepTogglePainterFactory
    /// </summary>
    public static class SwitchPainterFactory
    {
        /// <summary>
        /// Creates a switch painter for the specified style
        /// </summary>
        /// <param name="style">The BeepControlStyle to create a painter for</param>
        /// <param name="owner">The BeepSwitch control instance</param>
        /// <returns>ISwitchPainter implementation</returns>
        public static ISwitchPainter CreatePainter(BeepControlStyle style, BeepSwitch owner)
        {
            // Map BeepControlStyle to specific painter implementations
            return style switch
            {
                // Core modern styles
                BeepControlStyle.iOS15 => new iOSSwitchPainter(owner),
                BeepControlStyle.Material3 => new Material3SwitchPainter(owner),
                BeepControlStyle.Fluent2 => new Fluent2SwitchPainter(owner),
                BeepControlStyle.Minimal => new MinimalSwitchPainter(owner),
                
                // Additional modern styles (use iOS as base)
                BeepControlStyle.AntDesign => new iOSSwitchPainter(owner),
                BeepControlStyle.MaterialYou => new Material3SwitchPainter(owner),
                BeepControlStyle.Windows11Mica => new Fluent2SwitchPainter(owner),
                BeepControlStyle.MacOSBigSur => new iOSSwitchPainter(owner),
                BeepControlStyle.ChakraUI => new Material3SwitchPainter(owner),
                BeepControlStyle.TailwindCard => new iOSSwitchPainter(owner),
                BeepControlStyle.NotionMinimal => new MinimalSwitchPainter(owner),
                BeepControlStyle.VercelClean => new MinimalSwitchPainter(owner),
                BeepControlStyle.StripeDashboard => new Material3SwitchPainter(owner),
                BeepControlStyle.DarkGlow => new Material3SwitchPainter(owner),
                BeepControlStyle.DiscordStyle => new iOSSwitchPainter(owner),
                BeepControlStyle.GradientModern => new Material3SwitchPainter(owner),
                BeepControlStyle.GlassAcrylic => new Fluent2SwitchPainter(owner),
                BeepControlStyle.Neumorphism => new Material3SwitchPainter(owner),
                BeepControlStyle.Bootstrap => new iOSSwitchPainter(owner),
                BeepControlStyle.FigmaCard => new iOSSwitchPainter(owner),
                BeepControlStyle.PillRail => new iOSSwitchPainter(owner),
                BeepControlStyle.Apple => new iOSSwitchPainter(owner),
                BeepControlStyle.Fluent => new Fluent2SwitchPainter(owner),
                BeepControlStyle.Material => new Material3SwitchPainter(owner),
                BeepControlStyle.WebFramework => new iOSSwitchPainter(owner),
                BeepControlStyle.Effect => new Material3SwitchPainter(owner),
                BeepControlStyle.Metro => new MinimalSwitchPainter(owner),
                BeepControlStyle.Office => new Fluent2SwitchPainter(owner),
                BeepControlStyle.Gnome => new Material3SwitchPainter(owner),
                BeepControlStyle.Kde => new Material3SwitchPainter(owner),
                BeepControlStyle.Cinnamon => new Material3SwitchPainter(owner),
                BeepControlStyle.Elementary => new iOSSwitchPainter(owner),
                BeepControlStyle.NeoBrutalist => new MinimalSwitchPainter(owner),
                BeepControlStyle.Gaming => new Material3SwitchPainter(owner),
                BeepControlStyle.HighContrast => new MinimalSwitchPainter(owner),
                BeepControlStyle.Neon => new Material3SwitchPainter(owner),
                BeepControlStyle.Terminal => new MinimalSwitchPainter(owner),
                
                // Theme-style mappings
                BeepControlStyle.ArcLinux => new Material3SwitchPainter(owner),
                BeepControlStyle.Brutalist => new MinimalSwitchPainter(owner),
                BeepControlStyle.Cartoon => new iOSSwitchPainter(owner),
                BeepControlStyle.ChatBubble => new iOSSwitchPainter(owner),
                BeepControlStyle.Cyberpunk => new Material3SwitchPainter(owner),
                BeepControlStyle.Dracula => new Material3SwitchPainter(owner),
                BeepControlStyle.Glassmorphism => new Fluent2SwitchPainter(owner),
                BeepControlStyle.Holographic => new Material3SwitchPainter(owner),
                BeepControlStyle.GruvBox => new Material3SwitchPainter(owner),
                BeepControlStyle.Metro2 => new MinimalSwitchPainter(owner),
                BeepControlStyle.Modern => new iOSSwitchPainter(owner),
                BeepControlStyle.Nord => new Material3SwitchPainter(owner),
                BeepControlStyle.Nordic => new Material3SwitchPainter(owner),
                BeepControlStyle.OneDark => new Material3SwitchPainter(owner),
                BeepControlStyle.Paper => new Material3SwitchPainter(owner),
                BeepControlStyle.Solarized => new Material3SwitchPainter(owner),
                BeepControlStyle.Tokyo => new Material3SwitchPainter(owner),
                BeepControlStyle.Ubuntu => new Material3SwitchPainter(owner),
                BeepControlStyle.Retro => new MinimalSwitchPainter(owner),
                BeepControlStyle.NeonGlow => new Material3SwitchPainter(owner),
                
                // Fallback to iOS style (most universal)
                _ => new iOSSwitchPainter(owner)
            };
        }
    }
}

