using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Metrices.Helpers
{
    /// <summary>
    /// Centralized layout management for MetricTile controls
    /// Provides responsive layout calculations
    /// </summary>
    public static class MetricTileLayoutHelpers
    {
        #region Layout Calculations

        /// <summary>
        /// Calculate title bounds at top-left
        /// </summary>
        public static Rectangle CalculateTitleBounds(
            Rectangle tileBounds,
            Padding padding)
        {
            return new Rectangle(
                tileBounds.Left + padding.Left,
                tileBounds.Top + padding.Top,
                tileBounds.Width - padding.Horizontal - 40, // Leave space for icon
                24);
        }

        /// <summary>
        /// Calculate icon bounds at top-right
        /// </summary>
        public static Rectangle CalculateIconBounds(
            Rectangle tileBounds,
            Size iconSize,
            Padding padding)
        {
            return new Rectangle(
                tileBounds.Right - padding.Right - iconSize.Width,
                tileBounds.Top + padding.Top,
                iconSize.Width,
                iconSize.Height);
        }

        /// <summary>
        /// Calculate metric value bounds at bottom-left
        /// </summary>
        public static Rectangle CalculateMetricValueBounds(
            Rectangle tileBounds,
            Size titleSize,
            Padding padding)
        {
            return new Rectangle(
                tileBounds.Left + padding.Left,
                tileBounds.Bottom - 50,
                tileBounds.Width / 2,
                40);
        }

        /// <summary>
        /// Calculate delta bounds to the right of metric value
        /// </summary>
        public static Rectangle CalculateDeltaBounds(
            Rectangle tileBounds,
            Size metricSize,
            Padding padding)
        {
            return new Rectangle(
                tileBounds.Right + 5,
                tileBounds.Top + 10,
                tileBounds.Width - tileBounds.Right - padding.Right - 15,
                24);
        }

        /// <summary>
        /// Calculate silhouette bounds in center
        /// </summary>
        public static Rectangle CalculateSilhouetteBounds(
            Rectangle tileBounds,
            float scale = 0.6f)
        {
            int silhouetteWidth = (int)(tileBounds.Width * scale);
            int silhouetteHeight = (int)(tileBounds.Height * scale);
            int x = tileBounds.Left + (tileBounds.Width - silhouetteWidth) / 2;
            int y = tileBounds.Top + (tileBounds.Height - silhouetteHeight) / 2;

            return new Rectangle(x, y, silhouetteWidth, silhouetteHeight);
        }

        /// <summary>
        /// Get optimal tile size based on content
        /// </summary>
        public static Size GetOptimalTileSize(Padding padding)
        {
            return new Size(150, 150); // Default metric tile size
        }

        /// <summary>
        /// Calculate layout for all elements
        /// Returns a layout structure with all element bounds
        /// </summary>
        public static MetricTileLayout CalculateLayout(
            Rectangle tileBounds,
            Size iconSize,
            Padding padding)
        {
            var titleBounds = CalculateTitleBounds(tileBounds, padding);
            var iconBounds = CalculateIconBounds(tileBounds, iconSize, padding);
            var metricBounds = CalculateMetricValueBounds(tileBounds, titleBounds.Size, padding);
            var deltaBounds = CalculateDeltaBounds(tileBounds, metricBounds.Size, padding);
            var silhouetteBounds = CalculateSilhouetteBounds(tileBounds, 0.6f);

            return new MetricTileLayout
            {
                TitleBounds = titleBounds,
                IconBounds = iconBounds,
                MetricValueBounds = metricBounds,
                DeltaBounds = deltaBounds,
                SilhouetteBounds = silhouetteBounds
            };
        }

        #endregion
    }

    /// <summary>
    /// Layout structure for metric tile elements
    /// </summary>
    public class MetricTileLayout
    {
        public Rectangle TitleBounds { get; set; }
        public Rectangle IconBounds { get; set; }
        public Rectangle MetricValueBounds { get; set; }
        public Rectangle DeltaBounds { get; set; }
        public Rectangle SilhouetteBounds { get; set; }
    }
}

