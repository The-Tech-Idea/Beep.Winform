using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Factory for creating tree painters based on control style.
    /// </summary>
    public static class BeepTreePainterFactory
    {
        private static readonly Dictionary<BeepControlStyle, ITreePainter> _painterCache = new();

        /// <summary>
        /// Create or retrieve a cached painter for the specified style.
        /// </summary>
        public static ITreePainter CreatePainter(BeepControlStyle style, BeepTree owner, IBeepTheme theme)
        {
            if (_painterCache.TryGetValue(style, out var cached))
            {
                cached.Initialize(owner, theme);
                return cached;
            }

            ITreePainter painter = style switch
            {
                BeepControlStyle.Material3 => new Material3TreePainter(),
                BeepControlStyle.MaterialYou => new MaterialYouTreePainter(),
                BeepControlStyle.iOS15 => new iOS15TreePainter(),
                BeepControlStyle.MacOSBigSur => new MacOSBigSurTreePainter(),
                BeepControlStyle.Fluent2 => new Fluent2TreePainter(),
                BeepControlStyle.Windows11Mica => new Windows11MicaTreePainter(),
                BeepControlStyle.Minimal => new MinimalTreePainter(),
                BeepControlStyle.NotionMinimal => new NotionMinimalTreePainter(),
                BeepControlStyle.VercelClean => new VercelCleanTreePainter(),
                BeepControlStyle.Neumorphism => new NeumorphismTreePainter(),
                BeepControlStyle.GlassAcrylic => new GlassAcrylicTreePainter(),
                BeepControlStyle.DarkGlow => new DarkGlowTreePainter(),
                BeepControlStyle.GradientModern => new GradientModernTreePainter(),
                BeepControlStyle.Bootstrap => new BootstrapTreePainter(),
                BeepControlStyle.TailwindCard => new TailwindCardTreePainter(),
                BeepControlStyle.StripeDashboard => new StripeDashboardTreePainter(),
                BeepControlStyle.FigmaCard => new FigmaCardTreePainter(),
                BeepControlStyle.DiscordStyle => new DiscordStyleTreePainter(),
                BeepControlStyle.AntDesign => new AntDesignTreePainter(),
                BeepControlStyle.ChakraUI => new ChakraUITreePainter(),
                BeepControlStyle.PillRail => new PillRailTreePainter(),
                _ => new StandardTreePainter()
            };

            painter.Initialize(owner, theme);
            _painterCache[style] = painter;
            return painter;
        }

        /// <summary>
        /// Clear the painter cache (useful when themes change significantly).
        /// </summary>
        public static void ClearCache()
        {
            _painterCache.Clear();
        }
    }
}
