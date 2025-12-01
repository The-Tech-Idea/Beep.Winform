using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Features.Helpers
{
    /// <summary>
    /// Centralized icon management for FeatureCard controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class FeatureCardIconHelpers
    {
        #region Icon Path Resolution

        /// <summary>
        /// Get recommended logo icon path
        /// </summary>
        public static string GetRecommendedLogoIcon()
        {
            // Try common logo icon paths
            string[] logoPaths = {
                SvgsUI.Apps ?? "apps.svg",
                SvgsUI.Logo ?? "logo.svg",
                SvgsUI.Brand ?? "brand.svg",
                "logo.svg"
            };

            foreach (var path in logoPaths)
            {
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            return "logo.svg";
        }

        /// <summary>
        /// Get recommended bullet icon path
        /// </summary>
        public static string GetRecommendedBulletIcon()
        {
            // Try common bullet icon paths
            string[] bulletPaths = {
                SvgsUI.Check ?? "check.svg",
                SvgsUI.Circle ?? "circle.svg",
                SvgsUI.Dot ?? "dot.svg",
                "bullet.svg"
            };

            foreach (var path in bulletPaths)
            {
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            return "bullet.svg";
        }

        /// <summary>
        /// Get recommended action icon path for a specific index
        /// </summary>
        public static string GetRecommendedActionIcon(int index)
        {
            return index switch
            {
                0 => SvgsUI.More ?? SvgsUI.Menu ?? "more.svg",
                1 => SvgsUI.Settings ?? SvgsUI.Sliders ?? "settings.svg",
                2 => SvgsUI.Info ?? SvgsUI.HelpCircle ?? "info.svg",
                _ => SvgsUI.More ?? "more.svg"
            };
        }

        /// <summary>
        /// Get recommended card icon path
        /// </summary>
        public static string GetRecommendedCardIcon()
        {
            // Try common card icon paths
            string[] cardIconPaths = {
                SvgsUI.Apps ?? "apps.svg",
                SvgsUI.Grid ?? "grid.svg",
                SvgsUI.Layout ?? "layout.svg",
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
                "logo" => new Size(Math.Min(32, cardSize.Height / 8), Math.Min(32, cardSize.Height / 8)),
                "action" => new Size(Math.Min(24, cardSize.Height / 10), Math.Min(24, cardSize.Height / 10)),
                "card" => new Size(Math.Min(64, cardSize.Height / 4), Math.Min(64, cardSize.Height / 4)),
                "bullet" => new Size(Math.Min(20, cardSize.Height / 12), Math.Min(20, cardSize.Height / 12)),
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

