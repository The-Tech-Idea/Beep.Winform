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
    /// NeoBrutalist path painter - Sharp edges, bold colors, THICK black borders
    /// 0px radius (sharp!), flat fills (yellow/magenta), NO gradients
    /// Neo-Brutalism signature: Raw, aggressive, brutalist aesthetic
    /// </summary>
    public static class NeoBrutalistPathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ControlState state = ControlState.Normal)
        {
            // NeoBrutalist: BOLD flat colors (yellow/magenta), sharp edges
            Color fillColor = useThemeColors && theme != null ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.NeoBrutalist);
            Color secondaryColor = useThemeColors && theme != null ? theme.SecondaryColor : StyleColors.GetSecondary(BeepControlStyle.NeoBrutalist);
            int brutalistRadius = StyleBorders.GetRadius(BeepControlStyle.NeoBrutalist); // 0px - SHARP!

            // NeoBrutalist: Use secondary (magenta) for paths (bold!)
            Color pathColor = secondaryColor;

            // Create sharp-edged rectangle path (NeoBrutalist signature)
            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, brutalistRadius))
            {
                var brush = PaintersFactory.GetSolidBrush(pathColor);
                g.FillPath(brush, path);
            }
        }
    }
}
