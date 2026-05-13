using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Painters
{
    public class TrapezoidTabPainter : BaseTabStripPainter
    {
        public override string Name => "TrapezoidTabPainter";

        public override void PaintTabBackground(Graphics g, BeepDocumentTab tab, int index, TabStripPaintContext context)
        {
            Color fill = context.GetTabBackground(tab, index);
            int slant = context.Scale(8);
            var r = tab.TabRect;

            using var path = new GraphicsPath();
            path.AddPolygon(new PointF[]
            {
                new(r.Left, r.Bottom),
                new(r.Left + slant, r.Top),
                new(r.Right - slant, r.Top),
                new(r.Right, r.Bottom)
            });

            using (var br = new SolidBrush(fill))
                g.FillPath(br, path);
            using (var pen = new Pen(context.Theme.BorderColor, 1f))
                g.DrawPath(pen, path);
        }

        public override GraphicsPath CreateTabPath(Rectangle bounds, TabStripPaintContext context)
        {
            int slant = context.Scale(8);
            var path = new GraphicsPath();
            path.AddPolygon(new PointF[]
            {
                new(bounds.Left, bounds.Bottom),
                new(bounds.Left + slant, bounds.Top),
                new(bounds.Right - slant, bounds.Top),
                new(bounds.Right, bounds.Bottom)
            });
            return path;
        }
    }
}
