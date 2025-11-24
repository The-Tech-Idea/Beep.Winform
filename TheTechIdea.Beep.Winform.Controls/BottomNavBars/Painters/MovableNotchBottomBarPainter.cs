using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    internal class MovableNotchBottomBarPainter : BaseBottomBarPainter
    {
        public override string Name => "MovableNotch";
        public float NotchDepth { get; set; } = 22f;
        public float NotchWidthFactor { get; set; } = 1.15f;
        public float NotchRadiusFactor { get; set; } = 1.2f;
        public bool OutlineCTA { get; set; } = false;
        public int OutlineStroke { get; set; } = 4;

        public override void Paint(BottomBarPainterContext context)
        {
            base.CalculateLayout(context);
            var g = context.Graphics;
            var barRect = context.Bounds;
            // draw background rounded bar
            int barRadius = barRect.Height / 2;
            var barPath = new GraphicsPath();
            barPath.AddArc(barRect.Left, barRect.Top, barRadius, barRadius, 180, 90);
            barPath.AddArc(barRect.Right - barRadius, barRect.Top, barRadius, barRadius, 270, 90);
            barPath.AddArc(barRect.Right - barRadius, barRect.Bottom - barRadius, barRadius, barRadius, 0, 90);
            barPath.AddArc(barRect.Left, barRect.Bottom - barRadius, barRadius, barRadius, 90, 90);
            barPath.CloseFigure();

            // If there is a CTAIndex or SelectedIndex, create a notch at that item's center
            int anchorIdx = context.CTAIndex >= 0 ? context.CTAIndex : context.SelectedIndex;
            if (anchorIdx >= 0 && anchorIdx < context.Items.Count)
            {
                var r = _layoutHelper.GetItemRect(anchorIdx);
                // attempt to use animated indicator center if available
                float cx = r.Left + r.Width / 2f;
                try { if (context.AnimatedIndicatorWidth > 0f) cx = context.AnimatedIndicatorX + context.AnimatedIndicatorWidth / 2f; } catch { }
                // notch width and radius
                int baseRadius = Math.Min(r.Width, r.Height) / 2 + 6;
                int notchW = (int)(baseRadius * NotchWidthFactor * 1.4f);
                int notchH = (int)NotchDepth;
                int notchRadius = (int)(notchH * NotchRadiusFactor);

                var notchPath = new GraphicsPath();
                // use an ellipse to create a smooth scoop
                notchPath.AddEllipse((int)(cx - notchW / 2f), barRect.Top - notchH, notchW, notchH * 2);

                using (var region = new Region(barPath))
                {
                    region.Exclude(notchPath);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    using (var br = new SolidBrush(context.BarBackColor == Color.Empty ? Color.White : context.BarBackColor))
                    {
                        g.FillRegion(br, region);
                    }
                    g.SmoothingMode = SmoothingMode.Default;
                }

                // optionally draw notch outline or CTA circle
                using (var pen = new Pen(context.NavigationBorderColor == Color.Empty ? Color.FromArgb(180, context.BarForeColor) : context.NavigationBorderColor, 1f))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawPath(pen, notchPath);
                    g.SmoothingMode = SmoothingMode.Default;
                }

                // Draw CTA icon if CTAIndex is set
                if (context.CTAIndex >= 0 && context.CTAIndex < context.Items.Count)
                {
                    var rect = _layoutHelper.GetItemRect(context.CTAIndex);
                    var center = new Point(rect.Left + rect.Width/2, rect.Top + rect.Height/2 - 10);
                    int cRadius = Math.Min(rect.Width, rect.Height) / 2 + 6;
                    using (var sh = new SolidBrush(context.NavigationShadowColor == Color.Empty ? Color.FromArgb(60, 0,0,0) : context.NavigationShadowColor))
                    {
                        var shRect = new Rectangle(center.X - cRadius, center.Y - cRadius + context.CTAShadowYOffset, cRadius*2, cRadius*2);
                        g.FillEllipse(sh, shRect);
                    }
                    if (!OutlineCTA)
                    {
                        using (var fill = new SolidBrush(context.AccentColor))
                        using (var pen = new Pen(context.OnAccentColor == Color.Empty ? Color.White : context.OnAccentColor, 2f))
                        {
                            var circleRect = new Rectangle(center.X - cRadius, center.Y - cRadius, cRadius * 2, cRadius * 2);
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            g.FillEllipse(fill, circleRect);
                            g.DrawEllipse(pen, circleRect);
                            g.SmoothingMode = SmoothingMode.Default;
                        }
                    }
                    else
                    {
                        using (var pen = new Pen(context.OnAccentColor == Color.Empty ? Color.White : context.OnAccentColor, OutlineStroke))
                        {
                            var circleRect = new Rectangle(center.X - cRadius, center.Y - cRadius, cRadius * 2, cRadius * 2);
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            g.DrawEllipse(pen, circleRect);
                            g.SmoothingMode = SmoothingMode.Default;
                        }
                    }
                    var iconRect = new Rectangle(center.X - 12, center.Y - 12, 24, 24);
                    var prev = context.ImagePainter.FillColor;
                    context.ImagePainter.ImagePath = string.IsNullOrEmpty(context.Items[context.CTAIndex].ImagePath) ? context.DefaultImagePath : context.Items[context.CTAIndex].ImagePath;
                    context.ImagePainter.ImageEmbededin = ImageEmbededin.Button;
                    context.ImagePainter.FillColor = context.OnAccentColor == Color.Empty ? Color.White : context.OnAccentColor;
                    context.ImagePainter.DrawImage(g, iconRect);
                    context.ImagePainter.FillColor = prev;
                }
            }
            else
            {
                // no notch -> draw flat bar
                using (var br = new SolidBrush(context.BarBackColor == Color.Empty ? Color.White : context.BarBackColor))
                {
                    g.FillPath(br, barPath);
                }
            }

            // Draw items
            var rects = _layoutHelper.GetItemRectangles();
            for (int i = 0; i < rects.Count; i++)
            {
                var item = context.Items[i];
                PaintMenuItem(context.Graphics, item, rects[i], context);
            }
        }

        public override void RegisterHitAreas(BottomBarPainterContext context)
        {
            if (context == null || context.HitTest == null) return;
            var idx = context.CTAIndex >= 0 ? context.CTAIndex : context.SelectedIndex;
            if (idx < 0 || idx >= context.Items.Count) return;
            var r = _layoutHelper.GetItemRect(idx);
            var hitRect = new Rectangle(r.Left - 6, r.Top - 8, r.Width + 12, r.Height + 16);
            context.HitTest.AddHitArea($"BottomBarItem_{idx}", hitRect, null, () => context.OnItemClicked?.Invoke(idx, MouseButtons.Left));
        }
    }
}
