using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.AccordionMenus.Helpers
{
    /// <summary>
    /// Helper class for managing icons in accordion menu controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class AccordionIconHelpers
    {
        /// <summary>
        /// Gets the expand icon path (chevron right/down)
        /// </summary>
        public static string GetExpandIconPath()
        {
            // Try to resolve from SvgsUI using reflection
            var iconProperty = typeof(SvgsUI).GetProperty(
                "ChevronRight",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
            
            if (iconProperty != null)
            {
                var iconPath = iconProperty.GetValue(null) as string;
                if (!string.IsNullOrEmpty(iconPath))
                    return iconPath;
            }

            // Fallback
            return SvgsUI.ChevronRight ?? SvgsUI.ArrowRight ?? SvgsUI.Circle;
        }

        /// <summary>
        /// Gets the collapse icon path (chevron down)
        /// </summary>
        public static string GetCollapseIconPath()
        {
            // Try to resolve from SvgsUI using reflection
            var iconProperty = typeof(SvgsUI).GetProperty(
                "ChevronDown",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
            
            if (iconProperty != null)
            {
                var iconPath = iconProperty.GetValue(null) as string;
                if (!string.IsNullOrEmpty(iconPath))
                    return iconPath;
            }

            // Fallback
            return SvgsUI.ChevronDown ?? SvgsUI.ArrowDown ?? SvgsUI.Circle;
        }

        /// <summary>
        /// Gets the hamburger menu icon path
        /// </summary>
        public static string GetHamburgerIconPath()
        {
            // Try to resolve from SvgsUI using reflection
            var iconProperty = typeof(SvgsUI).GetProperty(
                "Menu",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
            
            if (iconProperty != null)
            {
                var iconPath = iconProperty.GetValue(null) as string;
                if (!string.IsNullOrEmpty(iconPath))
                    return iconPath;
            }

            // Fallback
            return SvgsUI.Menu ?? SvgsUI.Box;
        }

        /// <summary>
        /// Gets the icon color based on state and theme
        /// </summary>
        public static Color GetIconColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false,
            bool isHovered = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isSelected)
                {
                    if (theme.MenuMainItemSelectedForeColor != Color.Empty)
                        return theme.MenuMainItemSelectedForeColor;
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                }
                if (isHovered)
                {
                    if (theme.MenuMainItemHoverForeColor != Color.Empty)
                        return theme.MenuMainItemHoverForeColor;
                }
                if (theme.SideMenuForeColor != Color.Empty)
                    return theme.SideMenuForeColor;
                if (theme.MenuForeColor != Color.Empty)
                    return theme.MenuForeColor;
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return Color.FromArgb(100, 100, 100);
        }

        /// <summary>
        /// Calculates the appropriate icon size for accordion items
        /// </summary>
        public static Size GetIconSize(
            int itemHeight,
            float sizeRatio = 0.6f)
        {
            // Icon size as percentage of item height
            int iconSize = (int)(itemHeight * sizeRatio);
            
            // Ensure minimum and maximum sizes
            iconSize = Math.Max(16, Math.Min(iconSize, 32));
            
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
            bool isSelected = false,
            bool isHovered = false,
            BeepControlStyle controlStyle = BeepControlStyle.Material3,
            float opacity = 1.0f,
            int cornerRadius = 0)
        {
            if (iconBounds.IsEmpty || string.IsNullOrEmpty(iconPath))
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Get icon color
            Color tintColor = GetIconColor(theme, useThemeColors, isSelected, isHovered);

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
        /// Calculates icon bounds within accordion item bounds
        /// </summary>
        public static Rectangle CalculateItemIconBounds(
            Rectangle itemBounds,
            int itemHeight,
            float sizeRatio = 0.6f,
            int padding = 8)
        {
            if (itemBounds.IsEmpty)
                return Rectangle.Empty;

            Size iconSize = GetIconSize(itemHeight, sizeRatio);
            
            int x = itemBounds.Left + padding;
            int y = itemBounds.Top + (itemBounds.Height - iconSize.Height) / 2;
            
            return new Rectangle(x, y, iconSize.Width, iconSize.Height);
        }
    }
}
