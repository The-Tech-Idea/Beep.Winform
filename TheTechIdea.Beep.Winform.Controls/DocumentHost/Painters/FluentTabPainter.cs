using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Painters
{
    public class FluentTabPainter : BaseTabStripPainter
    {
        public override string Name => "FluentTabPainter";

        public override void PaintTabBackground(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
            Color fill = context.GetTabBackground(tab, -1);
            using var br = new SolidBrush(fill);
            g.FillRectangle(br, tab.TabRect);

            if (context.IsTabActive(tab))
            {
                Color accent = context.GetAccentColor(tab);
                using var pen = new Pen(accent, context.Scale(4));
                g.DrawLine(pen,
                    tab.TabRect.Left + 1, tab.TabRect.Bottom - 2,
                    tab.TabRect.Right - 1, tab.TabRect.Bottom - 2);
            }

            if (context.IsTabActive(tab) || context.IsTabHovered(-1))
            {
                using var topPen = new Pen(Color.FromArgb(60, context.Theme.BorderColor), 1f);
                g.DrawLine(topPen, tab.TabRect.Left, tab.TabRect.Top, tab.TabRect.Right, tab.TabRect.Top);
            }
        }

        public override void PaintAccentBar(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
        }
    }
}
