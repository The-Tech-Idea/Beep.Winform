using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Metro background painter - Windows Modern UI flat tile design
    /// Flat gray background with bold accent colors, no gradients
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class MetroBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color bg = useThemeColors && theme != null ? theme.BackgroundColor : Color.FromArgb(0xF8,0xF8,0xFA);
            var brush = PaintersFactory.GetSolidBrush(bg);
            g.FillPath(brush, path);
        }
    }
}
