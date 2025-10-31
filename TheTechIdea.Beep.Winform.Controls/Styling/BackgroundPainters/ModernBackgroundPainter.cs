using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class ModernBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors && theme != null ? theme.BackgroundColor : Color.FromArgb(0xF5,0xF5,0xF7);
            Color fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            var fillBrush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(fillBrush, path);

            var bounds = path.GetBounds();
            var overlay = PaintersFactory.GetLinearGradientBrush(bounds, Color.FromArgb(8, Color.White), Color.FromArgb(0, Color.White), LinearGradientMode.Vertical);
            g.FillPath(overlay, path);
        }
    }
}
