using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class GlassmorphismBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors
                ? BackgroundPainterHelpers.WithAlpha(theme.BackgroundColor,210)
                : Color.FromArgb(210,30,40,60);

            Color fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            var fillBrush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(fillBrush, path);

            using var clip = new BackgroundPainterHelpers.ClipScope(g, path);
            // HatchBrushs are not currently cached; create small transient hatch with low cost
            using (var frostBrush = new HatchBrush(HatchStyle.DottedGrid, Color.FromArgb(20, Color.White), Color.Transparent))
            {
                g.FillRectangle(frostBrush, path.GetBounds());
            }

            var bounds = path.GetBounds();
            var topRect = new RectangleF(bounds.Left, bounds.Top, bounds.Width, bounds.Height /3f);
            var overlay = PaintersFactory.GetLinearGradientBrush(topRect, Color.FromArgb(40, Color.White), Color.Transparent, LinearGradientMode.Vertical);
            g.FillRectangle(overlay, topRect);
        }
    }
}
