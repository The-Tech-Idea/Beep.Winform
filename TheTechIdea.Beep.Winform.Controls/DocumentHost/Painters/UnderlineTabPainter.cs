using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Painters
{
    public class UnderlineTabPainter : BaseTabStripPainter
    {
        public override string Name => "UnderlineTabPainter";

        public override void PaintTabBackground(Graphics g, BeepDocumentTab tab, int index, TabStripPaintContext context)
        {
            // Hover tint on inactive tabs
            if (context.IsTabHovered(index) && !context.IsTabActive(tab))
            {
                using var br = new SolidBrush(BeepDocumentTabStrip.Blend(context.Theme.PanelBackColor, context.Theme.BorderColor, 0.15f));
                g.FillRectangle(br, tab.TabRect);
            }

            // Active tab: draw the underline that defines this style
            if (context.IsTabActive(tab))
            {
                Color accent = context.GetAccentColor(tab);
                int barH = context.Scale(3);
                int barY = tab.TabRect.Bottom - barH;
                using var pen = new Pen(accent, barH) { StartCap = LineCap.Round, EndCap = LineCap.Round };
                g.DrawLine(pen,
                    tab.TabRect.Left + 2, barY,
                    tab.TabRect.Right - 2, barY);
            }
        }

        public override void PaintAccentBar(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
            // Underline is drawn in PaintTabBackground to ensure correct Z-order.
        }
    }
}
