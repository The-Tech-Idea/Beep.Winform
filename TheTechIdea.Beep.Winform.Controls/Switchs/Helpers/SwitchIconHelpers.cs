using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Switchs.Helpers
{
    /// <summary>
    /// Helper class for managing icons in switch controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class SwitchIconHelpers
    {
        /// <summary>
        /// Gets the icon path for a switch state
        /// Resolves icons from icon name or fallback icons
        /// </summary>
        public static string GetSwitchIconPath(
            string? iconName,
            bool isOn = false,
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

            // Priority 3: Default icons based on state
            return isOn
                ? (SvgsUI.Check ?? SvgsUI.Circle ?? SvgsUI.Box)
                : (SvgsUI.Circle ?? SvgsUI.Box);
        }

        /// <summary>
        /// Gets the icon color based on state and theme
        /// </summary>
        public static Color GetIconColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isOn = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isOn)
                {
                    if (theme.SuccessColor != Color.Empty)
                        return theme.SuccessColor;
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                }
                else
                {
                    if (theme.ForeColor != Color.Empty)
                        return theme.ForeColor;
                }
            }

            return isOn
                ? Color.FromArgb(76, 175, 80) // Material Green
                : Color.FromArgb(158, 158, 158); // Material Gray
        }

        /// <summary>
        /// Calculates the appropriate icon size for a switch thumb
        /// </summary>
        public static Size GetThumbIconSize(
            int thumbSize,
            float sizeRatio = 0.55f)
        {
            // Icon size as percentage of thumb size
            int iconSize = (int)(thumbSize * sizeRatio);
            
            // Ensure minimum and maximum sizes
            iconSize = Math.Max(12, Math.Min(iconSize, 24));
            
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

            // Create GraphicsPath for icon bounds (circular for thumb icons)
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
            
            // Thumb icons are typically circular
            path.AddEllipse(bounds);
            return path;
        }

        /// <summary>
        /// Calculates icon bounds within thumb bounds
        /// Centers the icon and applies appropriate sizing
        /// </summary>
        public static Rectangle CalculateThumbIconBounds(
            Rectangle thumbBounds,
            int thumbSize,
            float sizeRatio = 0.55f)
        {
            if (thumbBounds.IsEmpty)
                return Rectangle.Empty;

            Size iconSize = GetThumbIconSize(thumbSize, sizeRatio);
            
            int x = thumbBounds.X + (thumbBounds.Width - iconSize.Width) / 2;
            int y = thumbBounds.Y + (thumbBounds.Height - iconSize.Height) / 2;
            
            return new Rectangle(x, y, iconSize.Width, iconSize.Height);
        }
    }
}
