using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    internal class OutlineFloatingCTABottomBarPainter : BaseBottomBarPainter
    {
        public override string Name => "OutlineFloatingCTA";
        public int RingStrokeWidth { get; set; } = 4;
        public int HaloAlpha { get; set; } = 36;
        public int InnerAlpha { get; set; } = 12;
        public float HaloScale { get; set; } = 1.4f;

        public override void Paint(BottomBarPainterContext context)
        {
            base.CalculateLayout(context);
            var g = context.Graphics;
            var barRect = context.Bounds;
            // draw base background
            using (var br = new SolidBrush(context.BarBackColor == Color.Empty ? Color.White : context.BarBackColor))
                g.FillRectangle(br, barRect);
            // draw indicator base if needed
            var indicator = _layoutHelper.GetIndicatorRect();
            float iX = indicator.Left, iW = indicator.Width;
            try { if (context.AnimatedIndicatorWidth > 0f) { iX = context.AnimatedIndicatorX; iW = context.AnimatedIndicatorWidth; } } catch { }
            var iRect = new RectangleF(iX, indicator.Top, iW, indicator.Height);

            // draw outline CTA if CTA present
            if (context.CTAIndex >= 0 && context.CTAIndex < context.Items.Count)
            {
                var r = _layoutHelper.GetItemRect(context.CTAIndex);
                var center = new Point(r.Left + r.Width / 2, r.Top + r.Height / 2 - 10);
                int radius = Math.Min(r.Width, r.Height) / 2 + 6;
                // outer halo
                using (var halo = new SolidBrush(Color.FromArgb(HaloAlpha, context.AccentColor)))
                {
                    var haloRect = new Rectangle(center.X - (int)(radius * HaloScale), center.Y - (int)(radius * HaloScale), (int)(radius * 2 * HaloScale), (int)(radius * 2 * HaloScale));
                    g.FillEllipse(halo, haloRect);
                }
                // inner shadow / subtle inner fill
                using (var fill = new SolidBrush(Color.FromArgb(InnerAlpha, context.AccentColor)))
                {
                    var fillRect = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);
                    g.FillEllipse(fill, fillRect);
                }
                // outline ring
                using (var pen = new Pen(context.OnAccentColor == Color.Empty ? Color.White : context.OnAccentColor, RingStrokeWidth))
                {
                    var ringRect = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawEllipse(pen, ringRect);
                    g.SmoothingMode = SmoothingMode.Default;
                }
                // draw CTA icon
                var iconRect = new Rectangle(center.X - 12, center.Y - 12, 24, 24);
                var prev = context.ImagePainter.FillColor;
                context.ImagePainter.ImagePath = string.IsNullOrEmpty(context.Items[context.CTAIndex].ImagePath) ? context.DefaultImagePath : context.Items[context.CTAIndex].ImagePath;
                context.ImagePainter.ImageEmbededin = ImageEmbededin.Button;
                context.ImagePainter.FillColor = context.OnAccentColor == Color.Empty ? Color.White : context.OnAccentColor;
                context.ImagePainter.DrawImage(g, iconRect);
                context.ImagePainter.FillColor = prev;
            }

            var rects = _layoutHelper.GetItemRectangles();
            for (int i = 0; i < rects.Count; i++)
            {
                if (i == context.CTAIndex) continue;
                var item = context.Items[i];
                PaintMenuItem(context.Graphics, item, rects[i], context);
            }
        }

        public override void RegisterHitAreas(BottomBarPainterContext context)
        {
            if (context == null || context.HitTest == null) return;
            if (context.CTAIndex < 0 || context.CTAIndex >= context.Items.Count) return;
            var r = _layoutHelper.GetItemRect(context.CTAIndex);
            var center = new Point(r.Left + r.Width / 2, r.Top + r.Height / 2 - 10);
            int radius = Math.Min(r.Width, r.Height) / 2 + 6;
            var circleRect = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            context.HitTest.AddHitArea($"BottomBarItem_{context.CTAIndex}", circleRect, null, () => context.OnItemClicked?.Invoke(context.CTAIndex, MouseButtons.Left));
        }
    }
}
