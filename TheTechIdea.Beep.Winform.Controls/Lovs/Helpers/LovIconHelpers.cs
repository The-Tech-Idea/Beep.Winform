using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Lovs.Helpers
{
    /// <summary>
    /// Helper class for managing icons in LOV controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class LovIconHelpers
    {
        /// <summary>
        /// Gets the icon path for dropdown button
        /// </summary>
        public static string GetDropdownIconPath(string? customIcon = null)
        {
            if (!string.IsNullOrEmpty(customIcon))
                return customIcon;

            return SvgsUI.ChevronDown ?? SvgsUI.ArrowDown ?? SvgsUI.Circle;
        }

        /// <summary>
        /// Gets the icon color based on state and theme
        /// </summary>
        public static Color GetIconColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isPressed = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isPressed || isHovered)
                {
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                }
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return Color.FromArgb(100, 100, 100);
        }

        /// <summary>
        /// Calculates the appropriate icon size for a button
        /// </summary>
        public static Size GetButtonIconSize(
            int buttonSize,
            float sizeRatio = 0.6f)
        {
            // Icon size as percentage of button size
            int iconSize = (int)(buttonSize * sizeRatio);
            
            // Ensure minimum and maximum sizes
            iconSize = Math.Max(12, Math.Min(iconSize, 20));
            
            return new Size(iconSize, iconSize);
        }

        /// <summary>
        /// Paints an icon in a rectangle using StyledImagePainter
        /// </summary>
        public static void PaintIcon(
            Graphics g,
            Rectangle iconBounds,
            string iconPath,
            Color iconColor,
            IBeepTheme theme = null,
            bool useThemeColors = false,
            BeepControlStyle controlStyle = BeepControlStyle.Material3,
            float opacity = 1.0f)
        {
            if (iconBounds.IsEmpty || string.IsNullOrEmpty(iconPath))
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Create GraphicsPath for icon bounds
            using (var iconPathShape = CreateIconPath(iconBounds, controlStyle))
            {
                // Paint icon with tinting using StyledImagePainter
                StyledImagePainter.PaintWithTint(
                    g,
                    iconPathShape,
                    iconPath,
                    iconColor,
                    opacity,
                    0);
            }
        }

        /// <summary>
        /// Creates a GraphicsPath for icon bounds based on ControlStyle
        /// </summary>
        private static GraphicsPath CreateIconPath(Rectangle bounds, BeepControlStyle controlStyle)
        {
            var path = new GraphicsPath();
            
            // Button icons are typically rectangular
            path.AddRectangle(bounds);
            return path;
        }

        /// <summary>
        /// Calculates icon bounds within button bounds
        /// Centers the icon and applies appropriate sizing
        /// </summary>
        public static Rectangle CalculateButtonIconBounds(
            Rectangle buttonBounds,
            int buttonSize,
            float sizeRatio = 0.6f)
        {
            if (buttonBounds.IsEmpty)
                return Rectangle.Empty;

            Size iconSize = GetButtonIconSize(buttonSize, sizeRatio);
            
            int x = buttonBounds.X + (buttonBounds.Width - iconSize.Width) / 2;
            int y = buttonBounds.Y + (buttonBounds.Height - iconSize.Height) / 2;
            
            return new Rectangle(x, y, iconSize.Width, iconSize.Height);
        }
    }
}
