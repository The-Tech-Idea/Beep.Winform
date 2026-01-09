using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Helpers
{
    /// <summary>
    /// Helper class for managing icons in dock controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class DockIconHelpers
    {
        /// <summary>
        /// Gets the icon path for a dock item
        /// Resolves icons from icon name or fallback icons
        /// </summary>
        public static string GetDockItemIconPath(
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
            bool applyThemeToIcons = true,
            bool isHovered = false,
            bool isSelected = false)
        {
            if (!applyThemeToIcons)
                return Color.Empty; // No tinting

            if (useThemeColors && theme != null)
            {
                if (isSelected)
                {
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                }
                if (isHovered)
                {
                    if (theme.SurfaceColor != Color.Empty)
                        return ControlPaint.Light(theme.SurfaceColor, 0.1f);
                }
                if (theme.ForeColor != Color.Empty)
                    return theme.ForeColor;
            }

            return Color.Empty; // No tinting
        }

        /// <summary>
        /// Calculates the appropriate icon size for a dock item
        /// </summary>
        public static Size GetDockItemIconSize(
            int itemSize,
            float sizeRatio = 0.8f)
        {
            // Icon size as percentage of item size
            int iconSize = (int)(itemSize * sizeRatio);
            
            // Ensure minimum and maximum sizes
            iconSize = Math.Max(24, Math.Min(iconSize, 128));
            
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
            bool applyThemeToIcons = true,
            BeepControlStyle controlStyle = BeepControlStyle.Material3,
            float opacity = 1.0f,
            int cornerRadius = 0)
        {
            if (iconBounds.IsEmpty || string.IsNullOrEmpty(iconPath))
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Create GraphicsPath for icon bounds
            using (var iconPathShape = CreateIconPath(iconBounds, controlStyle, cornerRadius))
            {
                if (applyThemeToIcons && iconColor != Color.Empty)
                {
                    // Paint icon with tinting using StyledImagePainter
                    StyledImagePainter.PaintWithTint(
                        g,
                        iconPathShape,
                        iconPath,
                        iconColor,
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
        /// Calculates icon bounds within dock item bounds
        /// Centers the icon and applies appropriate sizing
        /// </summary>
        public static Rectangle CalculateDockItemIconBounds(
            Rectangle itemBounds,
            int itemSize,
            float sizeRatio = 0.8f)
        {
            if (itemBounds.IsEmpty)
                return Rectangle.Empty;

            Size iconSize = GetDockItemIconSize(itemSize, sizeRatio);
            
            int x = itemBounds.X + (itemBounds.Width - iconSize.Width) / 2;
            int y = itemBounds.Y + (itemBounds.Height - iconSize.Height) / 2;
            
            return new Rectangle(x, y, iconSize.Width, iconSize.Height);
        }
    }
}
