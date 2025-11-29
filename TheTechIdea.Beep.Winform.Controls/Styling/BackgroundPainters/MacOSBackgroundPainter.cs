using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// macOS background painter - classic Mac design language
    /// Clean surface with subtle gradient depth
    /// </summary>
    public static class MacOSBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // macOS: system gray/white surface
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.MacOSBigSur);

            // Solid background with subtle state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            // macOS subtle gradient overlay for depth
            RectangleF bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            var gradientBrush = PaintersFactory.GetLinearGradientBrush(
                bounds,
                Color.FromArgb(10, 255, 255, 255),  // Subtle top highlight
                Color.FromArgb(5, 0, 0, 0),         // Subtle bottom shadow
                LinearGradientMode.Vertical);
            g.FillPath(gradientBrush, path);
        }

        /// <summary>
        /// Legacy overload without state
        /// </summary>
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors)
        {
            Paint(g, path, style, theme, useThemeColors, ControlState.Normal);
        }
    }
}
