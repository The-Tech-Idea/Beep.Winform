using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Bootstrap background painter - clean utility-first design
    /// Solid white backgrounds with noticeable state feedback
    /// </summary>
    public static class BootstrapBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Bootstrap: clean white
            Color backgroundColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.Bootstrap);

            // Bootstrap uses normal/strong state feedback (utilitarian)
            BackgroundPainterHelpers.PaintSolidBackground(g, path, backgroundColor, state,
                BackgroundPainterHelpers.StateIntensity.Normal);
        }
    }
}
