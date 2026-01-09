using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    /// <summary>
    /// Helper class for managing icons in tab controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class TabIconHelpers
    {
        /// <summary>
        /// Gets the close button icon path
        /// </summary>
        public static string GetCloseIconPath(string? customPath = null)
        {
            // Priority 1: Custom path
            if (!string.IsNullOrEmpty(customPath))
                return customPath;

            // Priority 2: Default close icon
            return "TheTechIdea.Beep.Winform.Controls.GFX.SVG.close.svg";
        }

        /// <summary>
        /// Gets the icon color for close button based on state and theme
        /// </summary>
        public static Color GetCloseIconColor(
            IBeepTheme theme,
            bool useThemeColors,
            bool isHovered = false)
        {
            if (useThemeColors && theme != null)
            {
                if (isHovered)
                {
                    if (theme.PrimaryColor != Color.Empty)
                        return theme.PrimaryColor;
                    if (theme.AccentColor != Color.Empty)
                        return theme.AccentColor;
                }
                else
                {
                    if (theme.ForeColor != Color.Empty)
                        return theme.ForeColor;
                }
            }

            return isHovered
                ? Color.FromArgb(33, 150, 243) // Material Blue
                : Color.FromArgb(158, 158, 158); // Material Gray
        }

        /// <summary>
        /// Calculates the appropriate icon size for a tab
        /// </summary>
        public static Size GetTabIconSize(
            int tabHeight,
            int maxIconSize = 24)
        {
            // Icon size as percentage of tab height
            int iconSize = (int)(tabHeight * 0.6f);
            
            // Ensure minimum and maximum sizes
            iconSize = Math.Max(16, Math.Min(iconSize, maxIconSize));
            
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
            
            // Most icons are square or circular
            bool useCircle = controlStyle switch
            {
                BeepControlStyle.Material3 => false, // Square for Material
                BeepControlStyle.iOS15 => true,
                BeepControlStyle.MacOSBigSur => true,
                BeepControlStyle.NeoBrutalist => false, // Square for brutalist
                BeepControlStyle.HighContrast => false, // Square for high contrast
                _ => false // Default to square
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
        /// Calculates close button bounds within tab bounds
        /// </summary>
        public static Rectangle CalculateCloseButtonBounds(
            Rectangle tabBounds,
            int tabHeight,
            bool vertical,
            int padding = 8)
        {
            if (tabBounds.IsEmpty)
                return Rectangle.Empty;

            Size iconSize = GetTabIconSize(tabHeight);

            if (vertical)
            {
                int x = tabBounds.X + (tabBounds.Width - iconSize.Width) / 2;
                int y = tabBounds.Bottom - iconSize.Height - padding;
                return new Rectangle(x, y, iconSize.Width, iconSize.Height);
            }
            else
            {
                int x = tabBounds.Right - iconSize.Width - padding;
                int y = tabBounds.Y + (tabBounds.Height - iconSize.Height) / 2;
                return new Rectangle(x, y, iconSize.Width, iconSize.Height);
            }
        }
    }
}
