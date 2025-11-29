using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Minimal background painter - ultra-clean design with barely perceptible changes
    /// True minimalism: whisper-light transitions, almost invisible state feedback
    /// </summary>
    public static class MinimalBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Minimal: clean white/light gray background
            Color backgroundColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.Minimal);

            // Use subtle intensity for true minimalism
            BackgroundPainterHelpers.PaintSolidBackground(g, path, backgroundColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            // Whisper-light vertical highlight for understated depth
            var bounds = path.GetBounds();
            if (bounds.Width > 0 && bounds.Height > 0)
            {
                var gradient = PaintersFactory.GetLinearGradientBrush(
                    bounds, 
                    Color.FromArgb(8, 255, 255, 255), 
                    Color.FromArgb(0, 255, 255, 255), 
                    LinearGradientMode.Vertical);
                g.FillPath(gradient, path);
            }
        }
    }
}
