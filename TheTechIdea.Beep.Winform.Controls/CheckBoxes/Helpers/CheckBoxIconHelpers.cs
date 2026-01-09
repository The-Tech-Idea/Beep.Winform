using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers
{
    /// <summary>
    /// Helper class for managing icons in checkbox controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class CheckBoxIconHelpers
    {
        /// <summary>
        /// Gets the check icon path
        /// </summary>
        public static string GetCheckIconPath()
        {
            // Try to resolve from SvgsUI using reflection
            var iconProperty = typeof(SvgsUI).GetProperty(
                "Check",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
            
            if (iconProperty != null)
            {
                var iconPath = iconProperty.GetValue(null) as string;
                if (!string.IsNullOrEmpty(iconPath))
                    return iconPath;
            }

            // Fallback
            return SvgsUI.Check ?? SvgsUI.CheckCircle ?? SvgsUI.Circle;
        }

        /// <summary>
        /// Gets the indeterminate icon path (minus or dash)
        /// </summary>
        public static string GetIndeterminateIconPath()
        {
            // Try to resolve from SvgsUI using reflection
            var iconProperty = typeof(SvgsUI).GetProperty(
                "Minus",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
            
            if (iconProperty != null)
            {
                var iconPath = iconProperty.GetValue(null) as string;
                if (!string.IsNullOrEmpty(iconPath))
                    return iconPath;
            }

            // Fallback
            return SvgsUI.Minus ?? SvgsUI.Circle;
        }

        /// <summary>
        /// Gets the icon color based on state and theme
        /// </summary>
        public static Color GetIconColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isChecked = false,
            bool isIndeterminate = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isChecked)
                {
                    if (theme.CheckBoxCheckedForeColor != Color.Empty)
                        return theme.CheckBoxCheckedForeColor;
                    if (theme.SurfaceColor != Color.Empty)
                        return theme.SurfaceColor;
                }
                if (isIndeterminate)
                {
                    if (theme.CheckBoxForeColor != Color.Empty)
                        return theme.CheckBoxForeColor;
                }
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return isChecked ? Color.White : Color.FromArgb(128, 128, 128);
        }

        /// <summary>
        /// Calculates the appropriate icon size for checkbox
        /// </summary>
        public static Size GetIconSize(
            int checkBoxSize,
            float sizeRatio = 0.6f)
        {
            // Icon size as percentage of checkbox size
            int iconSize = (int)(checkBoxSize * sizeRatio);
            
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
            bool isChecked = false,
            bool isIndeterminate = false,
            BeepControlStyle controlStyle = BeepControlStyle.Material3,
            float opacity = 1.0f,
            int cornerRadius = 0)
        {
            if (iconBounds.IsEmpty || string.IsNullOrEmpty(iconPath))
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Get icon color
            Color tintColor = GetIconColor(theme, useThemeColors, isChecked, isIndeterminate);

            // Create GraphicsPath for icon bounds
            using (var iconPathShape = CreateIconPath(iconBounds, controlStyle, cornerRadius))
            {
                if (tintColor != Color.Empty)
                {
                    // Paint icon with tinting using StyledImagePainter
                    StyledImagePainter.PaintWithTint(
                        g,
                        iconPathShape,
                        iconPath,
                        tintColor,
                        opacity,
                        cornerRadius);
                }
                else
                {
                    // Paint icon without tinting
                    StyledImagePainter.Paint(
                        g,
                        iconPathShape,
                        iconPath);
                }
            }
        }

        /// <summary>
        /// Creates a GraphicsPath for icon bounds based on ControlStyle
        /// </summary>
        private static GraphicsPath CreateIconPath(Rectangle bounds, BeepControlStyle controlStyle, int cornerRadius)
        {
            var path = new GraphicsPath();
            
            if (cornerRadius > 0)
            {
                // Rounded rectangle
                int diameter = cornerRadius * 2;
                path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
                path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
                path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
            }
            else
            {
                // Rectangle
                path.AddRectangle(bounds);
            }
            
            return path;
        }

        /// <summary>
        /// Calculates icon bounds within checkbox bounds
        /// </summary>
        public static Rectangle CalculateCheckBoxIconBounds(
            Rectangle checkBoxBounds,
            int checkBoxSize,
            float sizeRatio = 0.6f)
        {
            if (checkBoxBounds.IsEmpty)
                return Rectangle.Empty;

            Size iconSize = GetIconSize(checkBoxSize, sizeRatio);
            
            int x = checkBoxBounds.Left + (checkBoxBounds.Width - iconSize.Width) / 2;
            int y = checkBoxBounds.Top + (checkBoxBounds.Height - iconSize.Height) / 2;
            
            return new Rectangle(x, y, iconSize.Width, iconSize.Height);
        }
    }
}
