using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Metro path painter - Windows Modern UI flat, sharp-edged paths
    /// 0px radius (sharp edges), flat colors, bold design
    /// Metro signature: Flat UI with no curves or gradients
    /// </summary>
    public static class MetroPathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ControlState state = ControlState.Normal)
        {
            // Metro: Sharp edges (0px radius), flat primary color
            Color fillColor = useThemeColors && theme != null ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Metro);
            int metroRadius = StyleBorders.GetRadius(BeepControlStyle.Metro); // 0px - sharp edges!

            // Create sharp-edged rectangle path (Metro signature)
            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, metroRadius))
            {
                PathPainterHelpers.PaintSolidPath(g, path, fillColor, state);
            }
        }
    }
}
