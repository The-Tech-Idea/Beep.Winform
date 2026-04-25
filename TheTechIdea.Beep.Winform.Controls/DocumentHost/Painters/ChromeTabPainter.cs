using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Painters
{
    public class ChromeTabPainter : BaseTabStripPainter
    {
        public override string Name => "ChromeTabPainter";

        public override void PaintTabBackground(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
            Color fill = context.GetTabBackground(tab, -1);
            using var path = CreateChromeTabPath(tab.TabRect, context.TabRadius);
            using var br = new SolidBrush(fill);
            g.FillPath(br, path);
            using var pen = new Pen(context.Theme.BorderColor, 1f);
            g.DrawPath(pen, path);
        }

        public override void PaintFocusIndicator(Graphics g, BeepDocumentTab tab, bool isFocused, TabStripPaintContext context)
        {
            if (!isFocused) return;
            if (tab.TabRect.IsEmpty) return;

            var focusRect = Rectangle.Inflate(tab.TabRect, -2, -2);
            if (focusRect.Width < 4 || focusRect.Height < 4) return;

            if (SystemInformation.HighContrast)
            {
                using var pen = new Pen(ColorUtils.MapSystemColor(SystemColors.Highlight), 3f);
                int r = System.Math.Min(context.TabRadius, focusRect.Width / 2);
                using var path = CreateRoundedRect(focusRect, r);
                g.DrawPath(pen, path);
            }
            else
            {
                ControlPaint.DrawFocusRectangle(g, focusRect);
            }
        }
    }
}
