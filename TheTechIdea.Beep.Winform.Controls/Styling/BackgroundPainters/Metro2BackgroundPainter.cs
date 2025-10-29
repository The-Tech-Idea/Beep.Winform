using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class Metro2BackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;
            Color bg = useThemeColors && theme != null ? theme.BackgroundColor : Color.FromArgb(0xF7, 0xF7, 0xF9);
            var fill = BackgroundPainterHelpers.ApplyState(bg, state);
            var brush = PaintersFactory.GetSolidBrush(fill);
            g.FillPath(brush, path);
        }
    }
}
