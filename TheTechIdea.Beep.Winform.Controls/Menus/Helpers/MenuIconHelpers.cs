using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Menus.Helpers
{
    /// <summary>
    /// Helper class for managing icons in menu bar controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class MenuIconHelpers
    {
        /// <summary>
        /// Gets the icon path for a menu item
        /// Resolves icons from icon name or fallback icons
        /// </summary>
        public static string GetMenuItemIconPath(
            string? iconName,
            string? fallbackIcon = null)
        {
            // Priority 1: Custom icon name from property
            if (!string.IsNullOrEmpty(iconName))
            {
                // Try to resolve from SvgsUI using reflection
                var iconProperty = typeof(SvgsUI).GetProperty(
                    iconName.Replace("-", "").Replace("_", ""),
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
                
                if (iconProperty != null)
                {
                    var iconPath = iconProperty.GetValue(null) as string;
                    if (!string.IsNullOrEmpty(iconPath))
                        return iconPath;
                }
            }

            // Priority 2: Fallback icon (if provided)
            if (!string.IsNullOrEmpty(fallbackIcon))
                return fallbackIcon;

            // Priority 3: Default icon
            return SvgsUI.Circle ?? SvgsUI.Box;
        }

        /// <summary>
        /// Gets the icon color based on state and theme
        /// </summary>
        public static Color GetIconColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false,
            bool isSelected = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isSelected)
                {
                    if (theme.MenuItemSelectedForeColor != Color.Empty)
                        return theme.MenuItemSelectedForeColor;
                }
                if (isHovered)
                {
                    if (theme.MenuItemHoverForeColor != Color.Empty)
                        return theme.MenuItemHoverForeColor;
                }
                if (theme.MenuItemForeColor != Color.Empty)
                    return theme.MenuItemForeColor;
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            if (isSelected)
                return Color.White;
            return Color.FromArgb(100, 100, 100);
        }

        /// <summary>
        /// Calculates the appropriate icon size for a menu item
        /// </summary>
        public static Size GetMenuItemIconSize(
            int menuItemHeight,
            float sizeRatio = 0.6f)
        {
            // Icon size as percentage of menu item height
            int iconSize = (int)(menuItemHeight * sizeRatio);
            
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
            
            // Menu item icons are typically rectangular
            path.AddRectangle(bounds);
            return path;
        }

        /// <summary>
        /// Calculates icon bounds within menu item bounds
        /// Centers the icon and applies appropriate sizing
        /// </summary>
        public static Rectangle CalculateMenuItemIconBounds(
            Rectangle menuItemBounds,
            int menuItemHeight,
            float sizeRatio = 0.6f)
        {
            if (menuItemBounds.IsEmpty)
                return Rectangle.Empty;

            Size iconSize = GetMenuItemIconSize(menuItemHeight, sizeRatio);
            
            int x = menuItemBounds.X + 8; // Left padding
            int y = menuItemBounds.Y + (menuItemBounds.Height - iconSize.Height) / 2;
            
            return new Rectangle(x, y, iconSize.Width, iconSize.Height);
        }
    }
}
