using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    internal class FloatingCTABottomBarPainter : BaseBottomBarPainter
    {
        public override string Name => "FloatingCTA";
        public override void Paint(BottomBarPainterContext context)
        {
            // Ensure layout is calculated
            base.CalculateLayout(context);
            // draw base background
            using (var b = new SolidBrush(Color.FromArgb(245, 245, 245)))
            {
                context.Graphics.FillRectangle(b, context.Bounds);
            }

            // draw CTA if set
            if (context.CTAIndex >= 0 && context.CTAIndex < context.Items.Count)
            {
                var rect = _layoutHelper.GetItemRect(context.CTAIndex);
                var center = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2 - 10);
                int baseRadius = Math.Min(rect.Width, rect.Height) / 2 + 6;
                // pulsing scale from context animation phase -> 1.0 .. 1.08
                float scale = 1.0f + 0.06f * context.AnimationPhase;
                int radius = (int)(baseRadius * scale);

                // shadow
                using (var shadow = new SolidBrush(Color.FromArgb(72, 0, 0, 0)))
                {
                    var shadowRect = new Rectangle(center.X - radius, center.Y - radius + 6, radius * 2, radius * 2);
                    context.Graphics.FillEllipse(shadow, shadowRect);
                }

                // cta circle with slight glow
                using (var fill = new SolidBrush(context.AccentColor))
                using (var pen = new Pen(Color.White, 2f))
                {
                    var circleRect = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);
                    context.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    // soft halo
                    using (var halo = new SolidBrush(Color.FromArgb(32, context.AccentColor)))
                    {
                        var haloRect = new Rectangle(center.X - (int)(radius * 1.35), center.Y - (int)(radius * 1.35), (int)(radius * 2.7), (int)(radius * 2.7));
                        context.Graphics.FillEllipse(halo, haloRect);
                    }
                    context.Graphics.FillEllipse(fill, circleRect);
                    context.Graphics.DrawEllipse(pen, circleRect);
                    context.Graphics.SmoothingMode = SmoothingMode.Default;
                }

                // draw CTA icon
                var iconRect = new Rectangle(center.X - 12, center.Y - 12, 24, 24);
                // slightly scale the icon with pulse
                var iconScale = 1.0f + 0.06f * context.AnimationPhase;
                var scaledIconRect = new Rectangle(
                    center.X - (int)(12 * iconScale),
                    center.Y - (int)(12 * iconScale),
                    (int)(24 * iconScale),
                    (int)(24 * iconScale));
                context.ImagePainter.ImagePath = string.IsNullOrEmpty(context.Items[context.CTAIndex].ImagePath) ? context.DefaultImagePath : context.Items[context.CTAIndex].ImagePath;
                context.ImagePainter.ImageEmbededin = BaseImage.ImageEmbededin.Button;
                var prevFill = context.ImagePainter.FillColor;
                context.ImagePainter.FillColor = Color.White;
                context.ImagePainter.DrawImage(context.Graphics, scaledIconRect);
                context.ImagePainter.FillColor = prevFill;
            }

            // handle indicator for normal items
            var indicator = _layoutHelper.GetIndicatorRect();
            using (var ib = new SolidBrush(Color.FromArgb(25, context.AccentColor)))
            {
                context.Graphics.FillRectangle(ib, indicator);
            }

            // Draw remaining items
            var rects = _layoutHelper.GetItemRectangles();
            for (int i = 0; i < rects.Count; i++)
            {
                if (i == context.CTAIndex) continue; // CTA drawn separately
                var item = context.Items[i];
                PaintMenuItem(context.Graphics, item, rects[i], context);
            }
        }

        public override void RegisterHitAreas(BottomBarPainterContext context)
        {
            if (context == null || context.HitTest == null) return;
            if (context.CTAIndex < 0 || context.CTAIndex >= context.Items.Count) return;
            var rect = _layoutHelper.GetItemRect(context.CTAIndex);
            var center = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2 - 10);
            int radius = Math.Min(rect.Width, rect.Height) / 2 + 6;
            var hitRect = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            context.HitTest.AddHitArea($"BottomBarItem_{context.CTAIndex}", hitRect, null, () => context.OnItemClicked?.Invoke(context.CTAIndex, MouseButtons.Left));
        }

        public override void RegisterHitAreas(BottomBarPainterContext context)
        {
            if (context == null || context.HitTest == null) return;
            var cta = context.CTAIndex;
            if (cta < 0 || cta >= context.Items.Count) return;
            var rect = _layoutHelper.GetItemRect(cta);
            var center = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2 - 10);
            int baseRadius = Math.Min(rect.Width, rect.Height) / 2 + 6;
            int radius = (int)(baseRadius * (1.0f + 0.06f * context.AnimationPhase));
            var circleRect = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            // Register as same BottomBarItem_{cta} name so click action remains consistent
            context.HitTest.AddHitArea($"BottomBarItem_{cta}", circleRect, null, () => context.OnItemClicked?.Invoke(cta, MouseButtons.Left));
        }
    }
}
