using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Chips.Helpers
{
    /// <summary>
    /// Helper class for managing icons in chip controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class ChipIconHelpers
    {
        /// <summary>
        /// Gets the icon path for a chip item
        /// Resolves icons from icon name or fallback icons
        /// </summary>
        public static string GetChipIconPath(
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
            ChipVariant variant,
            ChipColor chipColor,
            bool isSelected = false,
            bool isHovered = false)
        {
            if (useThemeColors && theme != null)
            {
                if (variant == ChipVariant.Filled)
                {
                    // For filled chips, icon color should contrast with background
                    Color bgColor = ChipThemeHelpers.GetChipBackgroundColor(theme, useThemeColors, variant, chipColor, isSelected, isHovered);
                    return ChipThemeHelpers.GetChipForegroundColor(theme, useThemeColors, variant, chipColor, isSelected);
                }
                else // Outlined or Text
                {
                    // Use foreground color helper
                    return ChipThemeHelpers.GetChipForegroundColor(theme, useThemeColors, variant, chipColor, isSelected);
                }
            }

            // Default colors
            if (variant == ChipVariant.Filled)
                return isSelected ? Color.White : Color.FromArgb(100, 100, 100);
            return Color.FromArgb(100, 100, 100);
        }

        /// <summary>
        /// Calculates the appropriate icon size for a chip based on chip size
        /// </summary>
        public static Size GetChipIconSize(
            ChipSize chipSize)
        {
            return chipSize switch
            {
                ChipSize.Small => new Size(14, 14),
                ChipSize.Medium => new Size(16, 16),
                ChipSize.Large => new Size(20, 20),
                _ => new Size(16, 16)
            };
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
            ChipVariant variant = ChipVariant.Filled,
            ChipColor chipColor = ChipColor.Default,
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
            Color tintColor = GetIconColor(theme, useThemeColors, variant, chipColor, isSelected, isHovered);

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
        /// Calculates icon bounds within chip bounds
        /// </summary>
        public static Rectangle CalculateChipIconBounds(
            Rectangle chipBounds,
            ChipSize chipSize,
            bool isLeading = true)
        {
            if (chipBounds.IsEmpty)
                return Rectangle.Empty;

            Size iconSize = GetChipIconSize(chipSize);
            int padding = chipSize == ChipSize.Small ? 4 : chipSize == ChipSize.Medium ? 6 : 8;
            
            int x = isLeading 
                ? chipBounds.Left + padding
                : chipBounds.Right - iconSize.Width - padding;
            int y = chipBounds.Top + (chipBounds.Height - iconSize.Height) / 2;
            
            return new Rectangle(x, y, iconSize.Width, iconSize.Height);
        }
    }
}
