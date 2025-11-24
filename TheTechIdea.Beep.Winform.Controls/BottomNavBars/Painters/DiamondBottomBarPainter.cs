using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    internal class DiamondBottomBarPainter : BaseBottomBarPainter
    {
        public override string Name => "Diamond";
        public override void Paint(BottomBarPainterContext context)
        {
            base.CalculateLayout(context);
            using (var b = new SolidBrush(context.BarBackColor == Color.Empty ? Color.FromArgb(250, 250, 250) : context.BarBackColor))
            {
                context.Graphics.FillRectangle(b, context.Bounds);
            }

            var rects = _layoutHelper.GetItemRectangles();
            for (int i = 0; i < rects.Count; i++)
            {
                var r = rects[i];
                if (i == context.SelectedIndex)
                {
                    var center = new Point(r.Left + r.Width / 2, r.Top + r.Height / 2);
                    int size = Math.Min(r.Width, r.Height) / 2;
                    var p = new Point[] {
                        new Point(center.X, center.Y - size),
                        new Point(center.X + size, center.Y),
                        new Point(center.X, center.Y + size),
                        new Point(center.X - size, center.Y)
                    };
                    using (var brush = new SolidBrush(Color.FromArgb(32, context.AccentColor)))
                    {
                        context.Graphics.FillPolygon(brush, p);
                    }
                }

                var item = context.Items[i];
                PaintMenuItem(context.Graphics, item, r, context);
            }
        }
    }
}
