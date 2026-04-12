using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Painters
{
    public class VSCodeTabPainter : BaseTabStripPainter
    {
        public override string Name => "VSCodeTabPainter";

        public override void PaintTabBackground(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
            Color fill = context.GetTabBackground(tab, -1);
            using var br = new SolidBrush(fill);
            g.FillRectangle(br, tab.TabRect);

            using var pen = new Pen(context.Theme.BorderColor, 1f);
            g.DrawLine(pen, tab.TabRect.Right - 1, tab.TabRect.Top + 4, tab.TabRect.Right - 1, tab.TabRect.Bottom - 4);
        }

        public override void PaintAccentBar(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
            Color accent = context.GetAccentColor(tab);
            int barH = context.Scale(3);
            using var pen = new Pen(accent, barH) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawLine(pen,
                tab.TabRect.Left + context.TabRadius, tab.TabRect.Top + barH / 2,
                tab.TabRect.Right - context.TabRadius, tab.TabRect.Top + barH / 2);
        }
    }
}
