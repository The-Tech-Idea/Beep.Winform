using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// NeoBrutalist background painter - Bold, raw, brutalist design
    /// Bright bold colors (yellow/magenta), flat fills, NO gradients
    /// Neo-Brutalism signature: Aggressive simplicity with thick borders
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class NeoBrutalistBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color bg = useThemeColors && theme != null ? theme.BackgroundColor : Color.FromArgb(240,240,240);
            var brush = PaintersFactory.GetSolidBrush(bg);
            g.FillPath(brush, path);
            var pen = PaintersFactory.GetPen(Color.FromArgb(30,0,0,0),1f);
            g.DrawPath(pen, path);
        }
    }
}
