using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Painters
{
    public class OfficeTabPainter : BaseTabStripPainter
    {
        public override string Name => "OfficeTabPainter";

        public override void PaintTabBackground(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
            Color fill = context.GetTabBackground(tab, -1);
            using var br = new SolidBrush(fill);
            g.FillRectangle(br, tab.TabRect);

            if (context.IsTabActive(tab))
            {
                Color accent = context.GetAccentColor(tab);
                using var pen = new Pen(accent, context.Scale(3));
                g.DrawLine(pen, tab.TabRect.Left, tab.TabRect.Bottom - 2, tab.TabRect.Right, tab.TabRect.Bottom - 2);
            }

            if (!context.IsTabActive(tab))
            {
                using var sep = new Pen(Color.FromArgb(40, context.Theme.BorderColor), 1f);
                g.DrawLine(sep, tab.TabRect.Right - 1, tab.TabRect.Top + 4, tab.TabRect.Right - 1, tab.TabRect.Bottom - 4);
            }
        }

        public override void PaintAccentBar(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
        }
    }
}
