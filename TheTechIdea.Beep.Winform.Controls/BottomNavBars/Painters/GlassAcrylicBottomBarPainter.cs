using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    internal class GlassAcrylicBottomBarPainter : BaseBottomBarPainter
    {
        public override string Name => "GlassAcrylic";
        public float AcrylicOpacity { get; set; } = 0.6f;
        public Color HighlightColor { get; set; } = Color.FromArgb(60, 255, 255, 255);
        public override void Paint(BottomBarPainterContext context)
        {
            base.CalculateLayout(context);
            var g = context.Graphics;
            var rect = context.Bounds;

            // simulate a glass effect with a translucent gradient and inner highlight
            var baseColor = context.BarBackColor == Color.Empty ? Color.FromArgb(250,250,250) : context.BarBackColor;
            using (var lg = new LinearGradientBrush(rect, Color.FromArgb((int)(AcrylicOpacity*255), baseColor), Color.FromArgb((int)(AcrylicOpacity*255*0.9), baseColor), LinearGradientMode.Vertical))
            {
                g.FillRectangle(lg, rect);
            }

            // inner top highlight
            var topRect = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height / 2);
            using (var br = new SolidBrush(Color.FromArgb(25, 255,255,255)))
            {
                g.FillRectangle(br, topRect);
            }

            // subtle border
            using (var pen = new Pen(context.NavigationBorderColor == Color.Empty ? Color.FromArgb(30, context.BarForeColor) : Color.FromArgb(30, context.NavigationBorderColor)))
            {
                g.DrawRectangle(pen, rect.Left, rect.Top, rect.Width-1, rect.Height-1);
            }

            // draw items
            var rects = _layoutHelper.GetItemRectangles();
            for (int i = 0; i < rects.Count; i++)
            {
                var item = context.Items[i];
                PaintMenuItem(g, item, rects[i], context);
            }
        }
    }
}
