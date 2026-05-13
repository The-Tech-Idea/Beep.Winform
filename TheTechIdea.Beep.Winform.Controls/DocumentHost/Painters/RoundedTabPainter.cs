using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Painters
{
    public class RoundedTabPainter : BaseTabStripPainter
    {
        public override string Name => "RoundedTabPainter";

        public override void PaintTabBackground(Graphics g, BeepDocumentTab tab, int index, TabStripPaintContext context)
        {
            if (tab.TabRect.Width < 6 || tab.TabRect.Height < 6) return;
            Color fill = context.GetTabBackground(tab, index);
            var r = Rectangle.Inflate(tab.TabRect, -2, -2);
            int radius = System.Math.Min(r.Height / 2, context.Scale(10));

            using var path = CreateRoundedRect(r, radius);
            using var br = new SolidBrush(fill);
            g.FillPath(br, path);

            if (context.IsTabActive(tab))
            {
                Color accent = context.GetAccentColor(tab);
                using var pen = new Pen(accent, 2f);
                g.DrawPath(pen, path);
            }
        }
    }
}
