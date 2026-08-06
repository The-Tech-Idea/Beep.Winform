using System;
using System.Drawing;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Testimonials.Helpers
{
    /// <summary>
    /// Centralized icon management for Testimonial controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class TestimonialIconHelpers
    {
        #region Icon Path Resolution

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
        /// Get recommended company logo icon path
        /// </summary>
        public static string GetRecommendedCompanyLogoIcon()
        {
            // Try common company logo icon paths
            string[] logoPaths = {
                SvgsUI.BrandTabler ?? "brand.svg",
                SvgsUI.BrandTabler ?? "logo.svg",
                SvgsUI.BuildingBroadcastTower ?? "company.svg",
                "company-logo.svg"
            };

            foreach (var path in logoPaths)
            {
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            return "company-logo.svg";
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
            string iconType,
            object viewType = null)
        {
            string viewTypeStr = viewType?.ToString() ?? "";

            return iconType switch
            {
                "avatar" => viewTypeStr.Contains("Compact") 
                    ? new Size(Math.Min(40, cardSize.Height / 4), Math.Min(40, cardSize.Height / 4))
                    : new Size(Math.Min(50, cardSize.Height / 4), Math.Min(50, cardSize.Height / 4)),
                "company" => new Size(Math.Min(60, cardSize.Width / 5), Math.Min(30, cardSize.Height / 10)),
                _ => new Size(50, 50)
            };
        }

        #endregion

        #region Icon Painting



        #endregion
    }
}


