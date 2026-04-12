using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Painters
{
    public class PillTabPainter : BaseTabStripPainter
    {
        public override string Name => "PillTabPainter";
        private const int PillPadV = 4;
        private const int PillRadius = 8;

        public override void PaintTabBackground(Graphics g, BeepDocumentTab tab, TabStripPaintContext context)
        {
            bool active = context.IsTabActive(tab);
            bool hovered = context.IsTabHovered(-1);

            if (active || hovered)
            {
                var pillRect = Rectangle.Inflate(
                    new Rectangle(tab.TabRect.Left, tab.TabRect.Top + context.Scale(PillPadV),
                        tab.TabRect.Width, tab.TabRect.Height - context.Scale(PillPadV) * 2),
                    -context.Scale(4), 0);

                Color pillColor = active
                    ? BeepDocumentTabStrip.Blend(context.Theme.PrimaryColor, context.Theme.BackgroundColor, 0.85f)
                    : BeepDocumentTabStrip.Blend(context.Theme.PanelBackColor, context.Theme.BorderColor, 0.25f);

                using var path = CreateRoundedRect(pillRect, context.Scale(PillRadius));
                using var br = new SolidBrush(pillColor);
                g.FillPath(br, path);
            }
        }
    }
}
