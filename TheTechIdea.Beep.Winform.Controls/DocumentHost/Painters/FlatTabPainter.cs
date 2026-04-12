using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Painters
{
    public class FlatTabPainter : BaseTabStripPainter
    {
        public override string Name => "FlatTabPainter";

        public override void PaintTabBackground(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
            Color fill = context.GetTabBackground(tab, -1);
            using var br = new SolidBrush(fill);
            g.FillRectangle(br, tab.TabRect);

            if (context.IsTabActive(tab))
            {
                Color accent = context.GetAccentColor(tab);
                using var pen = new Pen(accent, context.Scale(3));
                g.DrawLine(pen, tab.TabRect.Left, tab.TabRect.Top + 1, tab.TabRect.Right, tab.TabRect.Top + 1);
            }
        }

        public override void PaintAccentBar(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
        }
    }
}
