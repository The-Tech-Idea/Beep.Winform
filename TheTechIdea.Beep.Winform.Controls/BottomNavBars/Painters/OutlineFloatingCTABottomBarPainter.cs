using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    internal class OutlineFloatingCTABottomBarPainter : BaseBottomBarPainter
    {
        public override string Name => "OutlineFloatingCTA";

        /// <summary>As the plain CTA, plus the 1.35x halo ring this style draws around it.</summary>
        public override int GetTopOverhang(int contentHeight)
        {
            int radius = (int)((contentHeight / 2 + 6) * 1.35f);
            return Math.Max(0, radius - (contentHeight / 2 - 10));
        }
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
            using (var br = new SolidBrush(ResolveBarBack(context)))
                g.FillRectangle(br, barRect);
            // draw indicator base if needed
            var indicator = _layoutHelper.GetIndicatorRect();
            float iX = indicator.Left, iW = indicator.Width;
            if (context.AnimatedIndicatorWidth > 0f) { iX = context.AnimatedIndicatorX; iW = context.AnimatedIndicatorWidth; }
            var iRect = new RectangleF(iX, indicator.Top, iW, indicator.Height);

            // draw outline CTA if CTA present
            if (context.CTAIndex >= 0 && context.CTAIndex < context.Items.Count)
            {
                var r = _layoutHelper.GetItemRect(context.CTAIndex);
                var center = new Point(r.Left + r.Width / 2, r.Top + r.Height / 2 - 10);
                int radius = Math.Min(r.Width, r.Height) / 2 + 6;
                // outer halo
                using (var halo = new SolidBrush(Color.FromArgb(HaloAlpha, ResolveAccent(context))))
                {
                    var haloRect = new Rectangle(center.X - (int)(radius * HaloScale), center.Y - (int)(radius * HaloScale), (int)(radius * 2 * HaloScale), (int)(radius * 2 * HaloScale));
                    g.FillEllipse(halo, haloRect);
                }
                // inner shadow / subtle inner fill
                using (var fill = new SolidBrush(Color.FromArgb(InnerAlpha, ResolveAccent(context))))
                {
                    var fillRect = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);
                    g.FillEllipse(fill, fillRect);
                }
                // Outline ring in the ACCENT, not the on-accent. On-accent is the colour for content
                // drawn *on top of* a filled accent shape - white by default - and this circle is not
                // filled with accent, it is an outline with the bar showing through. A white ring on
                // a white bar is invisible, which is exactly how this style rendered.
                using (var pen = new Pen(ResolveAccent(context), RingStrokeWidth))
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
                // Accent too: the ring is an outline, so the icon sits on the bar, not on a filled
                // accent disc. On-accent white would have made it invisible alongside the ring.
                context.ImagePainter.FillColor = ResolveAccent(context);
                context.ImagePainter.DrawImage(g, iconRect);
                context.ImagePainter.FillColor = prev;
            }

            var rects = _layoutHelper.GetItemRectangles();
            for (int i = 0, n = PaintableCount(rects, context); i < n; i++)
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
            context.BarHitTest?.SetItemHitArea(context.CTAIndex, circleRect);
        }
    }
}
