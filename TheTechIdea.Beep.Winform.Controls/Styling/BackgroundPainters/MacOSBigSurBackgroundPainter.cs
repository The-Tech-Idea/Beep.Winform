using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// macOS Big Sur background painter - refined vibrancy with vertical gradient
    /// 5% lighter at top, subtle state transitions, vibrancy overlays
    /// </summary>
    public static class MacOSBigSurBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // macOS Big Sur: refined system surface
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.MacOSBigSur);

            // Get state-adjusted color with subtle Apple-style intensity
            Color stateColor = BackgroundPainterHelpers.GetStateAdjustedColor(
                baseColor, state, BackgroundPainterHelpers.StateIntensity.Subtle);

            RectangleF bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Big Sur signature: vertical gradient (5% lighter at top)
            Color topColor = BackgroundPainterHelpers.Lighten(stateColor, 0.05f);
            Color bottomColor = stateColor;

            var brush = PaintersFactory.GetLinearGradientBrush(
                bounds, topColor, bottomColor, LinearGradientMode.Vertical);
            g.FillPath(brush, path);

            // macOS vibrancy overlays for specific states
            if (state == ControlState.Hovered)
            {
                var vibrancyBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(8, Color.White));
                g.FillPath(vibrancyBrush, path);
            }
            else if (state == ControlState.Pressed)
            {
                var pressBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(12, Color.Black));
                g.FillPath(pressBrush, path);
            }
        }
    }
}
