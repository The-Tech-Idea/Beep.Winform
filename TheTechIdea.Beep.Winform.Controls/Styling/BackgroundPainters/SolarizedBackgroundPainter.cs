using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Solarized theme background painter - Ethan Schoonover's precision color scheme
    /// Clean, scientific color palette with subtle depth
    /// </summary>
    public static class SolarizedBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Solarized Dark base03 (or light base3 if using light theme)
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0x00, 0x2B, 0x36);
            
            // Solarized's accent color (cyan)
            Color accent = useThemeColors && theme != null 
                ? theme.AccentColor 
                : Color.FromArgb(0x2A, 0xA1, 0x98);

            // Paint solid background with subtle state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            // Add subtle top accent (very minimal - Solarized is clean)
            var bounds = path.GetBounds();
            if (bounds.Width > 0 && bounds.Height > 5)
            {
                BackgroundPainterHelpers.PaintTopHighlight(g, path, 8);
            }
        }
    }
}
