using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Factory for creating tree painters based on TreeStyle.
    /// Each TreeStyle represents a distinct visual design pattern.
    /// </summary>
    public static class BeepTreePainterFactory
    {
        private static readonly Dictionary<TreeStyle, ITreePainter> _painterCache = new();

        /// <summary>
        /// Create or retrieve a cached painter for the specified TreeStyle.
        /// </summary>
        public static ITreePainter CreatePainter(TreeStyle style, BeepTree owner, IBeepTheme theme)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));

            if (_painterCache.TryGetValue(style, out var cached))
            {
                // Re-initialize with the new owner and theme
                try { cached.Initialize(owner, theme); } catch { }
                return cached;
            }

            ITreePainter painter = style switch
            {
                // Application-specific styles (from reference images)
                TreeStyle.InfrastructureTree => new InfrastructureTreePainter(),
                TreeStyle.PortfolioTree => new PortfolioTreePainter(),
                TreeStyle.FileManagerTree => new FileManagerTreePainter(),
                TreeStyle.ActivityLogTree => new ActivityLogTreePainter(),
                TreeStyle.ComponentTree => new ComponentTreePainter(),
                TreeStyle.FileBrowserTree => new FileBrowserTreePainter(),
                TreeStyle.DocumentTree => new DocumentTreePainter(),

                // Modern framework styles
                TreeStyle.Material3 => new Material3TreePainter(),
                TreeStyle.Fluent2 => new Fluent2TreePainter(),
                TreeStyle.iOS15 => new iOS15TreePainter(),
                TreeStyle.MacOSBigSur => new MacOSBigSurTreePainter(),
                TreeStyle.NotionMinimal => new NotionMinimalTreePainter(),
                TreeStyle.VercelClean => new VercelCleanTreePainter(),
                TreeStyle.Discord => new DiscordTreePainter(),
                TreeStyle.AntDesign => new AntDesignTreePainter(),
                TreeStyle.ChakraUI => new ChakraUITreePainter(),
                TreeStyle.Bootstrap => new BootstrapTreePainter(),
                TreeStyle.TailwindCard => new TailwindCardTreePainter(),

                // Enterprise component library styles
                TreeStyle.DevExpress => new DevExpressTreePainter(),
                TreeStyle.Syncfusion => new SyncfusionTreePainter(),
                TreeStyle.Telerik => new TelerikTreePainter(),

                // Layout-specific styles
                TreeStyle.PillRail => new PillRailTreePainter(),
                TreeStyle.StripeDashboard => new StripeDashboardTreePainter(),
                TreeStyle.FigmaCard => new FigmaCardTreePainter(),

                // Default
                _ => new StandardTreePainter()
            };

            try { painter.Initialize(owner, theme); } catch { }
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

