using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Painters
{
    public class UnderlineTabPainter : BaseTabStripPainter
    {
        public override string Name => "UnderlineTabPainter";

        public override void PaintTabBackground(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
            if (context.IsTabHovered(-1) && !context.IsTabActive(tab))
            {
                using var br = new SolidBrush(BeepDocumentTabStrip.Blend(context.Theme.PanelBackColor, context.Theme.BorderColor, 0.15f));
                g.FillRectangle(br, tab.TabRect);
            }
        }

        public override void PaintAccentBar(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
        }
    }
}
