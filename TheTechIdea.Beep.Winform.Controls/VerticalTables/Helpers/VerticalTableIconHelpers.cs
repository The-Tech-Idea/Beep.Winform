using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.VerticalTables.Helpers
{
    /// <summary>
    /// Helper class for managing icons in vertical table controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class VerticalTableIconHelpers
    {
        /// <summary>
        /// Gets the icon path for a column header
        /// Resolves icons from SimpleItem.ImagePath or fallback icons
        /// </summary>
        public static string GetColumnIconPath(
            string? imagePath,
            string? fallbackIcon = null)
        {
            // Priority 1: Custom image path from SimpleItem
            if (!string.IsNullOrEmpty(imagePath))
                return imagePath;

            // Priority 2: Fallback icon (if provided)
            if (!string.IsNullOrEmpty(fallbackIcon))
                return fallbackIcon;

            // Priority 3: Default icon from SvgsUI
            return SvgsUI.Package ?? SvgsUI.Box ?? SvgsUI.Check;
        }

        /// <summary>
        /// Gets the icon color based on state and theme
        /// </summary>
        public static Color GetIconColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false,
            bool isFeatured = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isFeatured || isSelected)
                {
                    // Use white icons on colored backgrounds
                    return Color.White;
                }
                else
                {
                    if (theme.ForeColor != Color.Empty)
                        return theme.ForeColor;
                }
            }

            return isFeatured || isSelected
                ? Color.White
                : Color.FromArgb(73, 80, 87); // Medium gray
        }

        /// <summary>
        /// Calculates the appropriate icon size for a column header
        /// </summary>
        public static Size GetHeaderIconSize(
            int headerHeight,
            int headerWidth)
        {
            // Icon size as percentage of header height
            int iconSize = (int)(headerHeight * 0.4f);
            
            // Ensure minimum and maximum sizes
            iconSize = Math.Max(16, Math.Min(iconSize, 64));
            
            return new Size(iconSize, iconSize);
        }

        /// <summary>
        /// Calculates the appropriate icon size for a cell
        /// </summary>
        public static Size GetCellIconSize(
            int rowHeight)
        {
            // Icon size as percentage of row height
            int iconSize = (int)(rowHeight * 0.6f);
            
            // Ensure minimum and maximum sizes
            iconSize = Math.Max(12, Math.Min(iconSize, 32));
            
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
            BeepControlStyle controlStyle = BeepControlStyle.Material3)
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
            string iconPath,
            Color iconColor)
        {
            if (string.IsNullOrEmpty(iconPath) || radius <= 0)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Paint icon in circle using StyledImagePainter
            StyledImagePainter.PaintInCircle(
                g,
                centerX,
                centerY,
                radius,
                iconPath,
                iconColor,
                1f);
        }

        /// <summary>
        /// Creates a GraphicsPath for icon bounds based on ControlStyle
        /// </summary>
        private static GraphicsPath CreateIconPath(Rectangle bounds, BeepControlStyle controlStyle)
        {
            var path = new GraphicsPath();
            
            // Most icons are circular or square
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
        /// Calculates icon bounds within header bounds
        /// Centers the icon and applies appropriate sizing
        /// </summary>
        public static Rectangle CalculateHeaderIconBounds(
            Rectangle headerBounds,
            int headerHeight)
        {
            if (headerBounds.IsEmpty)
                return Rectangle.Empty;

            Size iconSize = GetHeaderIconSize(headerHeight, headerBounds.Width);
            
            int x = headerBounds.X + (headerBounds.Width - iconSize.Width) / 2;
            int y = headerBounds.Y + (headerBounds.Height - iconSize.Height) / 2;
            
            return new Rectangle(x, y, iconSize.Width, iconSize.Height);
        }

        /// <summary>
        /// Calculates icon bounds within cell bounds
        /// </summary>
        public static Rectangle CalculateCellIconBounds(
            Rectangle cellBounds,
            int rowHeight)
        {
            if (cellBounds.IsEmpty)
                return Rectangle.Empty;

            Size iconSize = GetCellIconSize(rowHeight);
            
            int x = cellBounds.X + 4; // Left padding
            int y = cellBounds.Y + (cellBounds.Height - iconSize.Height) / 2;
            
            return new Rectangle(x, y, iconSize.Width, iconSize.Height);
        }
    }
}
