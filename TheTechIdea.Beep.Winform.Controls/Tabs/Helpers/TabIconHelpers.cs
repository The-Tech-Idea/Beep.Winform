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

            // Priority 2: Default close icon.
            // Deliberately x.svg and not close.svg: close.svg is a red rounded-square *badge*
            // (<rect fill="#dd4752">) with a white cross on top, not a glyph. Recolouring it filled
            // the whole box with one colour, and tinting it multiplied the badge to near-black —
            // which is why every tab of every painter showed a solid dark square on the contact
            // sheet. x.svg is a single polygon with no fill attribute, so it recolours to a clean
            // monochrome cross in whatever the theme asks for.
            return "TheTechIdea.Beep.Winform.Controls.GFX.SVG.x.svg";
        }

        /// <summary>
        /// Gets the icon color for close button based on state and theme
        /// </summary>
        public static Color GetCloseIconColor(IBeepTheme theme, bool isHovered = false)
        {
            // Single source for the close glyph colour. TabThemeHelpers used to carry a second,
            // near-identical GetCloseButtonColor that nothing called; it was deleted rather than
            // left to drift out of step with this one.
            if (TabThemeHelpers.IsHighContrast)
            {
                return isHovered ? SystemColors.HighlightText : SystemColors.WindowText;
            }

            var t = theme ?? ThemeManagement.BeepThemesManager.CurrentTheme;
            return isHovered ? t.PrimaryColor : t.TabForeColor;
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
            Color iconColor)
        {
            if (iconBounds.IsEmpty || string.IsNullOrEmpty(iconPath))
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // PaintWithTint multiplies the glyph by the tint colour, so it can only ever darken.
            // The shipped close glyph is near-black, so multiplying rendered it as a solid black
            // square on every tab of every painter — visible on the contact sheet. Recolouring
            // replaces the element colours instead of blending, which is what a themed monochrome
            // icon needs; this is the same fix already applied to the grid toolbar icons.
            StyledImagePainter.PaintSvgRecolored(g, iconBounds, iconPath, iconColor);
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
