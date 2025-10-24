using System;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Factory for creating navigation painter instances based on Style
    /// </summary>
    public static class NavigationPainterFactory
    {
        /// <summary>
        /// Creates a navigation painter instance for the specified Style
        /// </summary>
        /// <param name="style">The navigation Style to create a painter for</param>
        /// <returns>An INavigationPainter implementation</returns>
        /// <exception cref="ArgumentException">Thrown when an unknown Style is specified</exception>
        public static INavigationPainter CreatePainter(navigationStyle style)
        {
            return style switch
            {
                navigationStyle.None => new StandardNavigationPainter(), // Fallback - should not be called for None
                navigationStyle.Standard => new StandardNavigationPainter(),
                navigationStyle.Material => new MaterialNavigationPainter(),
                navigationStyle.Compact => new CompactNavigationPainter(),
                navigationStyle.Minimal => new MinimalNavigationPainter(),
                navigationStyle.Bootstrap => new BootstrapNavigationPainter(),
                navigationStyle.Fluent => new FluentNavigationPainter(),
                navigationStyle.AntDesign => new AntDesignNavigationPainter(),
                navigationStyle.Telerik => new TelerikNavigationPainter(),
                navigationStyle.AGGrid => new AGGridNavigationPainter(),
                navigationStyle.DataTables => new DataTablesNavigationPainter(),
                navigationStyle.Card => new CardNavigationPainter(),
                navigationStyle.Tailwind => new TailwindNavigationPainter(),
                _ => throw new ArgumentException($"Unknown navigation Style: {style}", nameof(style))
            };
        }

        /// <summary>
        /// Gets the recommended height for a navigation Style
        /// </summary>
        public static int GetRecommendedHeight(navigationStyle style)
        {
            if (style == navigationStyle.None)
                return 0; // No height needed for None Style
                
            var painter = CreatePainter(style);
            return painter.RecommendedHeight;
        }

        /// <summary>
        /// Gets the recommended minimum width for a navigation Style
        /// </summary>
        public static int GetRecommendedMinWidth(navigationStyle style)
        {
            var painter = CreatePainter(style);
            return painter.RecommendedMinWidth;
        }
    }
}
