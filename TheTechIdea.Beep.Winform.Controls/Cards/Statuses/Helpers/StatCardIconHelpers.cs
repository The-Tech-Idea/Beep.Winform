using System;
using System.Drawing;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.StatusCards.Helpers
{
    /// <summary>
    /// Centralized icon management for StatCard controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class StatCardIconHelpers
    {
        #region Icon Path Resolution

        /// <summary>
        /// Get recommended trend up icon path
        /// </summary>
        public static string GetRecommendedTrendUpIcon()
        {
            // Try common trend up icon paths
            string[] trendUpPaths = {
                SvgsUI.TrendingUp ?? "trending-up.svg",
                SvgsUI.ArrowUp ?? "arrow-up.svg",
                SvgsUI.ChevronUp ?? "chevron-up.svg",
                "trendup.svg"
            };

            foreach (var path in trendUpPaths)
            {
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            return "trendup.svg";
        }

        /// <summary>
        /// Get recommended trend down icon path
        /// </summary>
        public static string GetRecommendedTrendDownIcon()
        {
            // Try common trend down icon paths
            string[] trendDownPaths = {
                SvgsUI.TrendingDown ?? "trending-down.svg",
                SvgsUI.ArrowDown ?? "arrow-down.svg",
                SvgsUI.ChevronDown ?? "chevron-down.svg",
                "trenddown.svg"
            };

            foreach (var path in trendDownPaths)
            {
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            return "trenddown.svg";
        }

        /// <summary>
        /// Get recommended card icon path
        /// </summary>
        public static string GetRecommendedCardIcon(string cardType = null)
        {
            if (!string.IsNullOrEmpty(cardType))
            {
                // Try to match card type to icon
                string lowerType = cardType.ToLowerInvariant();
                if (lowerType.Contains("revenue") || lowerType.Contains("money"))
                    return SvgsUI.DollarSign ?? SvgsUI.Currency ?? "revenue.svg";
                if (lowerType.Contains("user") || lowerType.Contains("member"))
                    return SvgsUI.User ?? SvgsUI.Users ?? "users.svg";
                if (lowerType.Contains("view") || lowerType.Contains("visit"))
                    return SvgsUI.Eye ?? SvgsUI.BarChart ?? "views.svg";
            }

            // Default card icon
            string[] cardIconPaths = {
                SvgsUI.Apps ?? "apps.svg",
                SvgsUI.Chart ?? "chart.svg",
                SvgsUI.BarChart ?? "bar-chart.svg",
                "card-icon.svg"
            };

            foreach (var path in cardIconPaths)
            {
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            return "card-icon.svg";
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
        /// Get icon size based on card size and icon type
        /// </summary>
        public static Size GetIconSize(
            Size cardSize,
            string iconType)
        {
            return iconType switch
            {
                "trend" => new Size(Math.Min(16, cardSize.Height / 10), Math.Min(16, cardSize.Height / 10)),
                "card" => new Size(Math.Min(32, cardSize.Height / 5), Math.Min(32, cardSize.Height / 5)),
                _ => new Size(16, 16)
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
        /// Paint fallback icon when icon path fails
        /// </summary>
        private static void PaintFallbackIcon(Graphics g, Rectangle bounds, Color color)
        {
            using (SolidBrush brush = new SolidBrush(color))
            {
                g.FillEllipse(brush, bounds);
            }
        }

        #endregion
    }
}

