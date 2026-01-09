using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers
{
    /// <summary>
    /// Helper class for managing icons in radio group controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class RadioGroupIconHelpers
    {
        /// <summary>
        /// Gets the icon path for a radio item
        /// Resolves icons from SimpleItem.ImagePath or fallback icons
        /// </summary>
        public static string GetItemIconPath(
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
            return SvgsUI.Circle ?? SvgsUI.Check ?? SvgsUI.Box;
        }

        /// <summary>
        /// Gets the icon color based on state and theme
        /// </summary>
        public static Color GetIconColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isSelected = false,
            bool isDisabled = false)
        {
            if (isDisabled)
            {
                if (useThemeColors && theme != null && theme.DisabledForeColor != Color.Empty)
                    return theme.DisabledForeColor;
                return Color.FromArgb(180, 180, 180);
            }

            if (isSelected)
            {
                if (useThemeColors && theme != null)
                {
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                    if (theme.AccentColor != Color.Empty)
                        return theme.AccentColor;
                }
                return Color.FromArgb(33, 150, 243); // Material Blue
            }

            if (useThemeColors && theme != null)
            {
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return Color.FromArgb(97, 97, 97); // Material Gray
        }

        /// <summary>
        /// Calculates the appropriate icon size for a radio item
        /// </summary>
        public static Size GetItemIconSize(
            int itemHeight,
            Size maxImageSize)
        {
            // Icon size as percentage of item height, but respect max size
            int iconSize = (int)(itemHeight * 0.6f);
            
            // Ensure minimum and maximum sizes
            iconSize = Math.Max(16, Math.Min(iconSize, Math.Min(maxImageSize.Width, maxImageSize.Height)));
            
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
        /// Calculates icon bounds within item bounds
        /// Centers the icon and applies appropriate sizing
        /// </summary>
        public static Rectangle CalculateItemIconBounds(
            Rectangle itemBounds,
            int itemHeight,
            Size maxImageSize,
            int padding = 8)
        {
            if (itemBounds.IsEmpty)
                return Rectangle.Empty;

            Size iconSize = GetItemIconSize(itemHeight, maxImageSize);
            
            int x = itemBounds.X + padding;
            int y = itemBounds.Y + (itemBounds.Height - iconSize.Height) / 2;
            
            return new Rectangle(x, y, iconSize.Width, iconSize.Height);
        }
    }
}
