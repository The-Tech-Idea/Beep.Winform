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
                    return SvgsUI.Eye ?? SvgsUI.Chart ?? "views.svg";
                if (lowerType.Contains("user") || lowerType.Contains("member"))
                    return SvgsUI.User ?? SvgsUI.Users ?? "users.svg";
                if (lowerType.Contains("revenue") || lowerType.Contains("money"))
                    return SvgsUI.Dollar ?? SvgsUI.Currency ?? "revenue.svg";
                if (lowerType.Contains("sale"))
                    return SvgsUI.Shopping ?? SvgsUI.Cart ?? "sales.svg";
            }

            // Default metric icon
            string[] metricIconPaths = {
                SvgsUI.Chart ?? "chart.svg",
                SvgsUI.BarChart ?? "bar-chart.svg",
                SvgsUI.TrendingUp ?? "trending-up.svg",
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
                    return SvgsUI.Dollar ?? "revenue-silhouette.svg";
            }

            // Default silhouette icon
            string[] silhouettePaths = {
                SvgsUI.Chart ?? "chart-silhouette.svg",
                SvgsUI.BarChart ?? "bar-chart-silhouette.svg",
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
        /// Paint icon using StyledImagePainter
        /// Supports SVG, caching, and theme-aware tinting
        /// </summary>
        public static void PaintIcon(
            Graphics g,
            Rectangle bounds,
            string iconPath,
            IBeepTheme theme,
            bool useThemeColors,
            Color? tintColor = null,
            float rotation = 0f)
        {
            if (g == null || bounds.IsEmpty || string.IsNullOrEmpty(iconPath))
                return;

            Color iconColor = tintColor ?? GetIconColor(theme, useThemeColors, null);

            try
            {
                StyledImagePainter.PaintWithTint(
                    g,
                    bounds,
                    iconPath,
                    iconColor);
            }
            catch
            {
                // Fallback: draw a simple shape if icon fails to load
                PaintFallbackIcon(g, bounds, iconColor);
            }
        }

        /// <summary>
        /// Paint silhouette image with semi-transparency
        /// </summary>
        public static void PaintSilhouette(
            Graphics g,
            Rectangle bounds,
            Image silhouette,
            float opacity = 0.2f)
        {
            if (g == null || bounds.IsEmpty || silhouette == null)
                return;

            try
            {
                using (ImageAttributes ia = new ImageAttributes())
                {
                    ColorMatrix cm = new ColorMatrix();
                    cm.Matrix33 = Math.Max(0f, Math.Min(1f, opacity)); // Alpha channel
                    ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    g.DrawImage(
                        silhouette,
                        bounds,
                        0, 0,
                        silhouette.Width,
                        silhouette.Height,
                        GraphicsUnit.Pixel,
                        ia);
                }
            }
            catch
            {
                // Fallback: draw a simple shape if silhouette fails to load
                PaintFallbackSilhouette(g, bounds, opacity);
            }
        }

        /// <summary>
        /// Paint fallback icon when icon path fails
        /// </summary>
        private static void PaintFallbackIcon(Graphics g, Rectangle bounds, Color color)
        {
            using (SolidBrush brush = new SolidBrush(color))
            {
                g.FillEllipse(brush, bounds);
            }
        }

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

