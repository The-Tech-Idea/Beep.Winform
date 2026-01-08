using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ColorSystems;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Material You dynamic color system path painter
    /// Uses adaptive primary color with dynamic theming
    /// </summary>
    public static class MaterialYouPathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            // Get base color
            Color baseColor = PathPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(103, 80, 164));
            
            // Use MaterialYouColorSystem for dynamic palette generation
            bool isDarkMode = theme?.IsDarkMode ?? false;
            var palette = MaterialYouColorSystem.GenerateMaterialYouPalette(baseColor, isDarkMode, ensureAccessibility: true);

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                // Apply tonal colors based on state (Material You uses tonal colors)
                Color fillColor = state switch
                {
                    ControlState.Hovered => palette.Tonal300,
                    ControlState.Pressed => palette.Tonal700,
                    ControlState.Focused => palette.Tonal400,
                    ControlState.Selected => palette.Tonal400,
                    ControlState.Disabled => palette.Tonal100,
                    _ => palette.Primary
                };

                // Ensure accessibility compliance
                if (theme != null && theme.BackColor != Color.Empty)
                {
                    fillColor = ColorAccessibilityHelper.EnsureContrastRatio(fillColor, theme.BackColor, ColorAccessibilityHelper.WCAG_AA_Normal);
                }

                PathPainterHelpers.PaintSolidPath(g, path, fillColor, ControlState.Normal);
            }
        }
    }
}

