using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.BreadCrumbs.Helpers
{
    /// <summary>
    /// Helper class for managing icons in breadcrumb controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class BreadcrumbIconHelpers
    {
        /// <summary>
        /// Gets the recommended home icon path for breadcrumbs
        /// </summary>
        public static string GetHomeIconPath()
        {
            // Use Home icon from SvgsUI
            return SvgsUI.Home;
        }

        /// <summary>
        /// Gets the default icon path for breadcrumb items without an icon
        /// </summary>
        public static string GetDefaultItemIcon(string itemName = null)
        {
            if (string.IsNullOrEmpty(itemName))
                return SvgsUI.Circle ?? "circle.svg";

            // Try to match item name to appropriate icon
            string lowerName = itemName.ToLowerInvariant();
            return lowerName switch
            {
                var n when n.Contains("home") => GetHomeIconPath(),
                var n when n.Contains("folder") || n.Contains("directory") => SvgsUI.Folder,
                var n when n.Contains("file") || n.Contains("document") => SvgsUI.File,
                var n when n.Contains("image") || n.Contains("picture") => SvgsUI.Image,
                var n when n.Contains("video") => SvgsUI.Video,
                var n when n.Contains("music") || n.Contains("audio") => SvgsUI.Sound,
                var n when n.Contains("settings") || n.Contains("config") => SvgsUI.Settings,
                var n when n.Contains("user") || n.Contains("profile") => SvgsUI.User,
                var n when n.Contains("search") => SvgsUI.Search,
                _ => SvgsUI.Circle
            };
        }

        /// <summary>
        /// Resolves icon path from various sources
        /// Handles SvgsUI properties, file paths, and embedded resources
        /// </summary>
        public static string ResolveIconPath(string iconPath, string itemName = null)
        {
            if (string.IsNullOrEmpty(iconPath))
            {
                // Fallback to default icon based on item name
                return GetDefaultItemIcon(itemName);
            }

            // If it's already a valid path (contains / or \ or has extension), return as-is
            if (iconPath.Contains("/") || iconPath.Contains("\\") || 
                iconPath.Contains(".svg") || iconPath.Contains(".png") || iconPath.Contains(".jpg"))
            {
                return iconPath;
            }

            // Try to resolve from SvgsUI static properties using reflection
            // This handles cases where iconPath is a property name like "Home" or "Folder"
            try
            {
                var svgsUIType = typeof(SvgsUI);
                var property = svgsUIType.GetProperty(iconPath, 
                    System.Reflection.BindingFlags.Public | 
                    System.Reflection.BindingFlags.Static | 
                    System.Reflection.BindingFlags.IgnoreCase);
                
                if (property != null)
                {
                    var value = property.GetValue(null) as string;
                    if (!string.IsNullOrEmpty(value))
                        return value;
                }
            }
            catch
            {
                // Reflection failed, continue with fallback
            }

            // Fallback to default icon
            return GetDefaultItemIcon(itemName);
        }

        /// <summary>
        /// Gets the icon color based on breadcrumb item state and theme
        /// Integrates with BreadcrumbThemeHelpers for theme-aware colors
        /// </summary>
        public static Color GetIconColor(
            BeepBreadcrump breadcrumb,
            bool isLast,
            bool isHovered,
            IBeepTheme theme = null,
            bool useThemeColors = false)
        {
            if (breadcrumb == null)
                return Color.Gray;

            // Use theme colors if available
            if (useThemeColors && theme != null)
            {
                return BreadcrumbThemeHelpers.GetItemTextColor(theme, useThemeColors, isLast, isHovered);
            }

            // Use breadcrumb's foreground color
            return breadcrumb.ForeColor != Color.Empty ? breadcrumb.ForeColor : Color.Gray;
        }

        /// <summary>
        /// Calculates the appropriate icon size for a breadcrumb item
        /// Icons are sized as a percentage of item height
        /// </summary>
        public static Size GetIconSize(
            BeepBreadcrump breadcrumb,
            int itemHeight)
        {
            if (breadcrumb == null || itemHeight <= 0)
                return new Size(16, 16);

            // Icon size is typically 60-70% of item height
            int iconSize = (int)(itemHeight * 0.65f);
            
            // Ensure minimum and maximum sizes
            iconSize = Math.Max(12, Math.Min(iconSize, 32));
            
            return new Size(iconSize, iconSize);
        }

        /// <summary>
        /// Calculates icon bounds within item bounds
        /// Centers the icon and applies appropriate sizing
        /// </summary>
        public static Rectangle CalculateIconBounds(
            Rectangle itemBounds,
            BeepBreadcrump breadcrumb)
        {
            if (itemBounds.IsEmpty || breadcrumb == null)
                return Rectangle.Empty;

            Size iconSize = GetIconSize(breadcrumb, itemBounds.Height);
            
            int x = itemBounds.X + 4; // Left padding
            int y = itemBounds.Y + (itemBounds.Height - iconSize.Height) / 2; // Centered vertically
            
            return new Rectangle(x, y, iconSize.Width, iconSize.Height);
        }

        /// <summary>
        /// Paints an icon in a rectangle using StyledImagePainter
        /// Handles tinting, theme colors, and state-based styling
        /// </summary>
        public static void PaintIcon(
            Graphics g,
            Rectangle iconBounds,
            BeepBreadcrump breadcrumb,
            string iconPath,
            bool isLast,
            bool isHovered,
            IBeepTheme theme = null,
            bool useThemeColors = false,
            BeepControlStyle controlStyle = BeepControlStyle.Material3)
        {
            if (iconBounds.IsEmpty || breadcrumb == null || string.IsNullOrEmpty(iconPath))
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Resolve icon path
            string resolvedPath = ResolveIconPath(iconPath);

            if (string.IsNullOrEmpty(resolvedPath))
                return;

            // Get icon color
            Color iconColor = GetIconColor(breadcrumb, isLast, isHovered, theme, useThemeColors);

            // Create GraphicsPath for icon bounds (square or circle based on style)
            using (var iconPathShape = CreateIconPath(iconBounds, controlStyle))
            {
                // Paint icon with tinting using StyledImagePainter
                StyledImagePainter.PaintWithTint(
                    g,
                    iconPathShape,
                    resolvedPath,
                    iconColor,
                    1f,
                    0);
            }
        }

        /// <summary>
        /// Paints an icon in a circle using StyledImagePainter
        /// </summary>
        public static void PaintIconInCircle(
            Graphics g,
            float centerX,
            float centerY,
            float radius,
            BeepBreadcrump breadcrumb,
            string iconPath,
            bool isLast,
            bool isHovered,
            IBeepTheme theme = null,
            bool useThemeColors = false)
        {
            if (breadcrumb == null || radius <= 0 || string.IsNullOrEmpty(iconPath))
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Resolve icon path
            string resolvedPath = ResolveIconPath(iconPath);

            if (string.IsNullOrEmpty(resolvedPath))
                return;

            // Get icon color
            Color iconColor = GetIconColor(breadcrumb, isLast, isHovered, theme, useThemeColors);

            // Paint icon in circle using StyledImagePainter
            StyledImagePainter.PaintInCircle(
                g,
                centerX,
                centerY,
                radius,
                resolvedPath,
                iconColor,
                1f);
        }

        /// <summary>
        /// Paints an icon with a GraphicsPath using StyledImagePainter
        /// </summary>
        public static void PaintIconWithPath(
            Graphics g,
            GraphicsPath path,
            BeepBreadcrump breadcrumb,
            string iconPath,
            bool isLast,
            bool isHovered,
            IBeepTheme theme = null,
            bool useThemeColors = false)
        {
            if (path == null || breadcrumb == null || string.IsNullOrEmpty(iconPath))
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Resolve icon path
            string resolvedPath = ResolveIconPath(iconPath);

            if (string.IsNullOrEmpty(resolvedPath))
                return;

            // Get icon color
            Color iconColor = GetIconColor(breadcrumb, isLast, isHovered, theme, useThemeColors);

            // Paint icon with path using StyledImagePainter
            StyledImagePainter.PaintWithTint(
                g,
                path,
                resolvedPath,
                iconColor,
                1f,
                0);
        }

        /// <summary>
        /// Paints the home icon for breadcrumbs
        /// </summary>
        public static void PaintHomeIcon(
            Graphics g,
            Rectangle iconBounds,
            BeepBreadcrump breadcrumb,
            IBeepTheme theme = null,
            bool useThemeColors = false,
            BeepControlStyle controlStyle = BeepControlStyle.Material3)
        {
            string homeIconPath = GetHomeIconPath();
            PaintIcon(g, iconBounds, breadcrumb, homeIconPath, false, false, theme, useThemeColors, controlStyle);
        }

        /// <summary>
        /// Creates a GraphicsPath for icon bounds based on ControlStyle
        /// </summary>
        private static GraphicsPath CreateIconPath(Rectangle bounds, BeepControlStyle controlStyle)
        {
            var path = new GraphicsPath();
            
            // Most icons are square or circular
            // Use circle for most styles, square for some
            bool useCircle = controlStyle switch
            {
                BeepControlStyle.Material3 => true,
                BeepControlStyle.iOS15 => true,
                BeepControlStyle.MacOSBigSur => true,
                BeepControlStyle.NeoBrutalist => false, // Square for brutalist
                BeepControlStyle.HighContrast => false, // Square for high contrast
                _ => true // Default to circle
            };

            if (useCircle)
            {
                path.AddEllipse(bounds);
            }
            else
            {
                path.AddRectangle(bounds);
            }

            return path;
        }

        /// <summary>
        /// Gets recommended icon paths for common breadcrumb use cases
        /// </summary>
        public static string GetRecommendedIcon(string useCase)
        {
            return useCase.ToLowerInvariant() switch
            {
                "home" => GetHomeIconPath(),
                "folder" or "directory" => SvgsUI.Folder,
                "file" or "document" => SvgsUI.File,
                "image" or "picture" => SvgsUI.Image,
                "video" => SvgsUI.Video,
                "settings" or "config" => SvgsUI.Settings,
                "user" or "profile" => SvgsUI.User,
                "search" => SvgsUI.Search,
                "trash" or "delete" => SvgsUI.Trash,
                _ => SvgsUI.Circle // Default
            };
        }
    }
}

