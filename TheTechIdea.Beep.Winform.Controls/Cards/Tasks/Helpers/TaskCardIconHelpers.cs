using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Tasks.Helpers
{
    /// <summary>
    /// Centralized icon management for TaskCard controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class TaskCardIconHelpers
    {
        #region Icon Path Resolution

        /// <summary>
        /// Get recommended more icon path
        /// </summary>
        public static string GetRecommendedMoreIcon()
        {
            // Try common more icon paths
            string[] moreIconPaths = {
                SvgsUI.Dots ?? "more.svg",
                SvgsUI.MenuN2 ?? "menu.svg",
                SvgsUI.DotsVertical ?? "dots-vertical.svg",
                SvgsUI.Dots ?? "ellipsis.svg",
                "more.svg"
            };

            foreach (var path in moreIconPaths)
            {
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            return "more.svg";
        }

        /// <summary>
        /// Get recommended avatar icon path
        /// </summary>
        public static string GetRecommendedAvatarIcon()
        {
            // Try common avatar icon paths
            string[] avatarPaths = {
                SvgsUI.User ?? "user.svg",
                SvgsUI.User ?? "users.svg",
                SvgsUI.User ?? "person.svg",
                "avatar.svg"
            };

            foreach (var path in avatarPaths)
            {
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            return "avatar.svg";
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
                "more" => new Size(Math.Min(24, cardSize.Height / 10), Math.Min(24, cardSize.Height / 10)),
                "avatar" => new Size(Math.Min(32, cardSize.Height / 7), Math.Min(32, cardSize.Height / 7)),
                _ => new Size(24, 24)
            };
        }

        #endregion

        #region Icon Painting




        /// <summary>
        /// Paint fallback avatar when image fails
        /// </summary>
        private static void PaintFallbackAvatar(Graphics g, Rectangle bounds, Color borderColor, int borderThickness)
        {
            using (Pen borderPen = new Pen(borderColor, borderThickness))
            {
                g.DrawEllipse(borderPen, bounds);
            }
            using (SolidBrush brush = new SolidBrush(Color.LightGray))
            {
                g.FillEllipse(brush, bounds);
            }
        }

        #endregion
    }
}


