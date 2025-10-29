using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class CyberpunkBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors && theme != null ? theme.BackgroundColor : Color.FromArgb(10,10,12);
            Color neon = useThemeColors && theme != null ? theme.AccentColor : Color.FromArgb(0,255,200);

            var fill = BackgroundPainterHelpers.ApplyState(baseColor, state);
            var brush = PaintersFactory.GetSolidBrush(fill);
            g.FillPath(brush, path);

            var bounds = path.GetBounds();
            var leftGrad = PaintersFactory.GetLinearGradientBrush(new RectangleF(bounds.Left, bounds.Top, Math.Min(120,bounds.Width), bounds.Height), Color.FromArgb(60,255,0,150), Color.Transparent, LinearGradientMode.Horizontal);
            g.FillRectangle(leftGrad, new RectangleF(bounds.Left, bounds.Top, Math.Min(120,bounds.Width), bounds.Height));

            var pen = PaintersFactory.GetPen(Color.FromArgb(120, neon),2f);
            g.DrawPath(pen, path);
        }
    }
}
