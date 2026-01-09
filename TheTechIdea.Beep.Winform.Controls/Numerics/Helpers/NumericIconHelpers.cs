using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Helpers
{
    /// <summary>
    /// Helper class for managing icons in numeric controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class NumericIconHelpers
    {
        /// <summary>
        /// Gets the icon path for up/increment button
        /// </summary>
        public static string GetUpIconPath(string? customIcon = null)
        {
            if (!string.IsNullOrEmpty(customIcon))
                return customIcon;

            return SvgsUI.ChevronUp ?? SvgsUI.ArrowUp ?? SvgsUI.Circle;
        }

        /// <summary>
        /// Gets the icon path for down/decrement button
        /// </summary>
        public static string GetDownIconPath(string? customIcon = null)
        {
            if (!string.IsNullOrEmpty(customIcon))
                return customIcon;

            return SvgsUI.ChevronDown ?? SvgsUI.ArrowDown ?? SvgsUI.Circle;
        }

        /// <summary>
        /// Gets the icon path for plus button
        /// </summary>
        public static string GetPlusIconPath(string? customIcon = null)
        {
            if (!string.IsNullOrEmpty(customIcon))
                return customIcon;

            return Svgs.Plus ?? SvgsUI.Plus ?? SvgsUI.Circle;
        }

        /// <summary>
        /// Gets the icon path for minus button
        /// </summary>
        public static string GetMinusIconPath(string? customIcon = null)
        {
            if (!string.IsNullOrEmpty(customIcon))
                return customIcon;

            return Svgs.Minus ?? SvgsUI.Minus ?? SvgsUI.Circle;
        }

        /// <summary>
        /// Gets the icon color based on state and theme
        /// </summary>
        public static Color GetIconColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isPressed = false,
            bool isHovered = false,
            bool isDisabled = false)
        {
            if (isDisabled)
            {
                if (useThemeColors && theme != null)
                {
                    if (theme.DisabledForeColor != Color.Empty)
                        return theme.DisabledForeColor;
                }
                return Color.FromArgb(180, 180, 180);
            }

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
            float sizeRatio = 0.5f)
        {
            // Icon size as percentage of button size
            int iconSize = (int)(buttonSize * sizeRatio);
            
            // Ensure minimum and maximum sizes
            iconSize = Math.Max(8, Math.Min(iconSize, 16));
            
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
            float sizeRatio = 0.5f)
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
