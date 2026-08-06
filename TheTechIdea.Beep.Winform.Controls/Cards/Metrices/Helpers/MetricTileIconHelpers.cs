using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Metrices.Helpers
{
    /// <summary>
    /// Centralized icon management for MetricTile controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class MetricTileIconHelpers
    {
        #region Icon Path Resolution

        /// <summary>
        /// Get recommended metric icon path based on metric type
        /// </summary>
        public static string GetRecommendedMetricIcon(string metricType = null)
        {
            if (!string.IsNullOrEmpty(metricType))
            {
                // Try to match metric type to icon
                string lowerType = metricType.ToLowerInvariant();
                if (lowerType.Contains("view") || lowerType.Contains("visit"))
                    return SvgsUI.Eye ?? SvgsUI.ChartDots ?? "views.svg";
                if (lowerType.Contains("user") || lowerType.Contains("member"))
                    return SvgsUI.User ?? SvgsUI.User ?? "users.svg";
                if (lowerType.Contains("revenue") || lowerType.Contains("money"))
                    return SvgsUI.CashBanknote ?? SvgsUI.CashBanknote ?? "revenue.svg";
                if (lowerType.Contains("sale"))
                    return SvgsUI.ShoppingCart ?? SvgsUI.ShoppingCart ?? "sales.svg";
            }

            // Default metric icon
            string[] metricIconPaths = {
                SvgsUI.ChartDots ?? "chart.svg",
                SvgsUI.ChartDots ?? "bar-chart.svg",
                SvgsUI.ArrowBadgeUp ?? "trending-up.svg",
                "metric.svg"
            };

            foreach (var path in metricIconPaths)
            {
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            return "metric.svg";
        }

        /// <summary>
        /// Get recommended silhouette icon path based on metric type
        /// </summary>
        public static string GetRecommendedSilhouetteIcon(string metricType = null)
        {
            if (!string.IsNullOrEmpty(metricType))
            {
                // Try to match metric type to silhouette
                string lowerType = metricType.ToLowerInvariant();
                if (lowerType.Contains("view") || lowerType.Contains("visit"))
                    return SvgsUI.Eye ?? "views-silhouette.svg";
                if (lowerType.Contains("user") || lowerType.Contains("member"))
                    return SvgsUI.User ?? "users-silhouette.svg";
                if (lowerType.Contains("revenue") || lowerType.Contains("money"))
                    return SvgsUI.CashBanknote ?? "revenue-silhouette.svg";
            }

            // Default silhouette icon
            string[] silhouettePaths = {
                SvgsUI.ChartDots ?? "chart-silhouette.svg",
                SvgsUI.ChartDots ?? "bar-chart-silhouette.svg",
                "silhouette.svg"
            };

            foreach (var path in silhouettePaths)
            {
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            return "silhouette.svg";
        }

        /// <summary>
        /// Resolve icon path from multiple sources
        /// </summary>
        public static string ResolveIconPath(
            string iconPath,
            string defaultIcon)
        {
            if (!string.IsNullOrEmpty(iconPath))
                return iconPath;

            return defaultIcon;
        }

        #endregion

        #region Icon Color Management

        /// <summary>
        /// Get icon color based on theme
        /// </summary>
        public static Color GetIconColor(
            IBeepTheme theme,
            bool useThemeColors,
            Color? customColor = null)
        {
            if (customColor.HasValue)
                return customColor.Value;

            if (useThemeColors && theme != null)
            {
                if (theme.PrimaryTextColor != Color.Empty)
                    return theme.PrimaryTextColor;

                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;

                if (theme.CardTextForeColor != Color.Empty)
                    return theme.CardTextForeColor;
            }

            return Color.Black;
        }

        #endregion

        #region Icon Sizing

        /// <summary>
        /// Get icon size based on tile size and icon type
        /// </summary>
        public static Size GetIconSize(
            Size tileSize,
            string iconType)
        {
            return iconType switch
            {
                "metric" => new Size(Math.Min(24, tileSize.Height / 6), Math.Min(24, tileSize.Height / 6)),
                "silhouette" => new Size(
                    (int)(tileSize.Width * 0.60),
                    (int)(tileSize.Height * 0.60)),
                _ => new Size(24, 24)
            };
        }

        #endregion

        #region Icon Painting




        /// <summary>
        /// Paint fallback silhouette when image fails
        /// </summary>
        private static void PaintFallbackSilhouette(Graphics g, Rectangle bounds, float opacity)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb((int)(255 * opacity), Color.Gray)))
            {
                g.FillEllipse(brush, bounds);
            }
        }

        #endregion
    }
}


