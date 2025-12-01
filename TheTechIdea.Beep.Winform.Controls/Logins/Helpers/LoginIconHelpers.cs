using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Logins.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Helpers
{
    /// <summary>
    /// Helper class for managing icons, logos, and SVG assets in login controls
    /// </summary>
    public static class LoginIconHelpers
    {
        /// <summary>
        /// Gets the default logo path for a view type
        /// </summary>
        public static string GetDefaultLogoPath(LoginViewType viewType)
        {
            // Use SVG from IconsManagement
            return SvgsUI.Logo ?? SvgsUI.Image ?? SvgsUI.Home;
        }

        /// <summary>
        /// Gets the default avatar path
        /// </summary>
        public static string GetDefaultAvatarPath()
        {
            return SvgsUI.User ?? SvgsUI.User ?? SvgsUI.Users;
        }

        /// <summary>
        /// Gets the icon path for a social login provider
        /// </summary>
        public static string GetSocialIconPath(string provider)
        {
            return provider.ToLowerInvariant() switch
            {
                "google" => SvgsUI.Google ?? SvgsUI.Globe,
                "facebook" => SvgsUI.Facebook ?? SvgsUI.Users,
                "twitter" => SvgsUI.Twitter ?? SvgsUI.MessageCircle,
                "github" => SvgsUI.Github ?? SvgsUI.Code,
                "microsoft" => SvgsUI.Microsoft ?? SvgsUI.Monitor,
                _ => SvgsUI.LogIn
            };
        }

        /// <summary>
        /// Paints an icon using StyledImagePainter
        /// </summary>
        public static void PaintIcon(
            Graphics g,
            GraphicsPath path,
            string iconPath,
            BeepControlStyle style)
        {
            if (string.IsNullOrEmpty(iconPath) || path == null) return;
            StyledImagePainter.Paint(g, path, iconPath, style);
        }

        /// <summary>
        /// Paints an icon in a circle using StyledImagePainter
        /// </summary>
        public static void PaintIconInCircle(
            Graphics g,
            float centerX,
            float centerY,
            float radius,
            string iconPath,
            Color? tint = null,
            float opacity = 1f)
        {
            if (string.IsNullOrEmpty(iconPath)) return;
            StyledImagePainter.PaintInCircle(g, centerX, centerY, radius, iconPath, tint, opacity);
        }

        /// <summary>
        /// Paints a logo using StyledImagePainter in a circular shape
        /// </summary>
        public static void PaintLogo(
            Graphics g,
            float centerX,
            float centerY,
            float radius,
            string logoPath,
            BeepControlStyle style,
            Color? tint = null,
            float opacity = 1f)
        {
            if (string.IsNullOrEmpty(logoPath)) return;
            StyledImagePainter.PaintInCircle(g, centerX, centerY, radius, logoPath, tint, opacity);
        }

        /// <summary>
        /// Paints an avatar using StyledImagePainter in a circular shape
        /// </summary>
        public static void PaintAvatar(
            Graphics g,
            float centerX,
            float centerY,
            float radius,
            string avatarPath,
            BeepControlStyle style,
            Color? tint = null,
            float opacity = 1f)
        {
            if (string.IsNullOrEmpty(avatarPath)) return;
            StyledImagePainter.PaintInCircle(g, centerX, centerY, radius, avatarPath, tint, opacity);
        }

        /// <summary>
        /// Paints an icon with a GraphicsPath using StyledImagePainter
        /// </summary>
        public static void PaintIconWithPath(
            Graphics g,
            GraphicsPath path,
            string iconPath,
            BeepControlStyle style,
            Color? tint = null,
            float opacity = 1f)
        {
            if (string.IsNullOrEmpty(iconPath) || path == null) return;
            
            if (tint.HasValue)
            {
                StyledImagePainter.PaintWithTint(g, path, iconPath, tint.Value, opacity);
            }
            else
            {
                StyledImagePainter.Paint(g, path, iconPath, style);
            }
        }

        /// <summary>
        /// Gets the recommended image path for a logo
        /// </summary>
        public static string GetRecommendedLogoPath()
        {
            return SvgsUI.Logo ?? SvgsUI.Image ?? SvgsUI.Home;
        }

        /// <summary>
        /// Gets the recommended image path for an avatar
        /// </summary>
        public static string GetRecommendedAvatarPath()
        {
            return SvgsUI.User ?? SvgsUI.UserCircle ?? SvgsUI.Users;
        }

        /// <summary>
        /// Resizes an icon size maintaining aspect ratio
        /// </summary>
        public static Size ResizeIcon(Size originalSize, Size maxSize)
        {
            if (originalSize.Width <= maxSize.Width && originalSize.Height <= maxSize.Height)
                return originalSize;

            float ratio = Math.Min(
                (float)maxSize.Width / originalSize.Width,
                (float)maxSize.Height / originalSize.Height);

            return new Size(
                (int)(originalSize.Width * ratio),
                (int)(originalSize.Height * ratio));
        }

        /// <summary>
        /// Gets the recommended icon size for a control type
        /// </summary>
        public static Size GetIconSize(string controlType)
        {
            return controlType.ToLowerInvariant() switch
            {
                "logo" => new Size(60, 60),
                "avatar" => new Size(60, 60),
                "social" => new Size(24, 24),
                "button" => new Size(20, 20),
                _ => new Size(32, 32)
            };
        }
    }
}

