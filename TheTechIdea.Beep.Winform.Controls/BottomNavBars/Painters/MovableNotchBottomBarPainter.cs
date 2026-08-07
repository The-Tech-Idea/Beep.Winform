using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    internal class MovableNotchBottomBarPainter : BaseBottomBarPainter
    {
        public override string Name => "MovableNotch";

        /// <summary>This style draws its own raised CTA, so it does not take the shared disc.</summary>
        protected override bool DrawsOwnCta => true;

        /// <summary>
        /// The circle is centred 10px above the cell's middle with a radius of half the cell plus 6,
        /// so a little over half of it sits above the band. Derived rather than guessed, so it stays
        /// correct when the bar is made taller or shorter.
        /// </summary>
        /// <remarks>
        /// The tallest thing this style draws above the band is whichever is deeper: the notch cut-out
        /// (<see cref="NotchDepth"/> above the band's top edge) or the CTA disc. Reserving for only one
        /// of them left the notch arc sliced flat, because NotchDepth is 22 and the old formula
        /// reserved 17.
        /// </remarks>
        public override int GetTopOverhang(int contentHeight)
        {
            var disc = CtaDiscGeometry(contentHeight, 1f);
            return Math.Max((int)NotchDepth, Math.Max(0, disc.Radius - disc.CentreOffset));
        }
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
            int barRadius = barRect.Height / 2;
            var barPath = new GraphicsPath();
            barPath.AddArc(barRect.Left, barRect.Top, barRadius, barRadius, 180, 90);
            barPath.AddArc(barRect.Right - barRadius, barRect.Top, barRadius, barRadius, 270, 90);
            barPath.AddArc(barRect.Right - barRadius, barRect.Bottom - barRadius, barRadius, barRadius, 0, 90);
            barPath.AddArc(barRect.Left, barRect.Bottom - barRadius, barRadius, barRadius, 90, 90);
            barPath.CloseFigure();

            // The notch exists to make room for the raised CTA button. Without a CTA there is nothing
            // to make room for, and cutting one anyway takes a bite out of the bar with nothing in it -
            // on the leftmost cell (where the selection starts) it also ate the bar's rounded corner.
            // It used to fall back to the selected item, so every notch bar with no CTA looked damaged.
            int anchorIdx = context.CTAIndex;
            if (anchorIdx >= 0 && anchorIdx < context.Items.Count)
            {
                var r = _layoutHelper.GetItemRect(anchorIdx);
                float cx = r.Left + r.Width / 2f;

                // Follow the animated indicator only when the notch is anchored to the SELECTION.
                // The indicator tracks the selected item, so when a CTA is configured this moved the
                // notch to the selected cell while the CTA circle below was drawn at CTAIndex - the
                // cut-out sat over one item and the button over another, and on the leftmost cell the
                // stray notch clipped the bar's rounded corner.
                if (context.CTAIndex < 0 && context.AnimatedIndicatorWidth > 0f)
                    cx = context.AnimatedIndicatorX + context.AnimatedIndicatorWidth / 2f;
                // Unchanged geometry - GetTopOverhang now reserves for NotchDepth, which is what this
                // draws, instead of a separate formula that came up 5px short.
                var disc = CtaDiscGeometry(barRect.Height, 1f);
                int baseRadius = disc.Radius;
                int notchW = (int)(baseRadius * NotchWidthFactor * 1.4f);
                int notchH = (int)NotchDepth;

                var notchPath = new GraphicsPath();
                notchPath.AddEllipse((int)(cx - notchW / 2f), barRect.Top - notchH, notchW, notchH * 2);

                using (var region = new Region(barPath))
                {
                    region.Exclude(notchPath);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    using (var br = new SolidBrush(ResolveBarBack(context)))
                    {
                        g.FillRegion(br, region);
                    }
                    g.SmoothingMode = SmoothingMode.Default;
                }

                var notchBorderColor = context.NavigationBorderColor == Color.Empty ? Color.FromArgb(30, ResolveBarFore(context)) : context.NavigationBorderColor;
                using (var pen = new Pen(notchBorderColor, 1f))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawPath(pen, notchPath);
                    g.SmoothingMode = SmoothingMode.Default;
                }

                if (context.CTAIndex >= 0 && context.CTAIndex < context.Items.Count)
                {
                    var rect = _layoutHelper.GetItemRect(context.CTAIndex);
                    var center = new Point(rect.Left + rect.Width / 2, barRect.Top + disc.CentreOffset);
                    int cRadius = disc.Radius;
                    using (var sh = new SolidBrush(context.NavigationShadowColor == Color.Empty ? Color.FromArgb(60, ResolveBarFore(context)) : context.NavigationShadowColor))
                    {
                        var shRect = new Rectangle(center.X - cRadius, center.Y - cRadius + context.CTAShadowYOffset, cRadius*2, cRadius*2);
                        g.FillEllipse(sh, shRect);
                    }
                    if (!OutlineCTA)
                    {
                        using (var fill = new SolidBrush(ResolveAccent(context)))
                        using (var pen = new Pen(ResolveOnAccent(context), 2f))
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
                        using (var pen = new Pen(ResolveOnAccent(context), OutlineStroke))
                        {
                            var circleRect = new Rectangle(center.X - cRadius, center.Y - cRadius, cRadius * 2, cRadius * 2);
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            g.DrawEllipse(pen, circleRect);
                            g.SmoothingMode = SmoothingMode.Default;
                        }
                    }
                    var iconRect = new Rectangle(center.X - 12, center.Y - 12, 24, 24);
                    // On-accent when the disc is filled, accent when it is an outline ring - otherwise
                    // the outline variant paints a white glyph onto the bar's white background.
                    PaintTintedIcon(g, string.IsNullOrEmpty(context.Items[context.CTAIndex].ImagePath)
                                           ? context.DefaultImagePath : context.Items[context.CTAIndex].ImagePath,
                                    iconRect,
                                    OutlineCTA ? ResolveAccent(context) : ResolveOnAccent(context), context);
                }
            }
            else
            {
                using (var br = new SolidBrush(ResolveBarBack(context)))
                {
                    g.FillPath(br, barPath);
                }
            }

            var rects = _layoutHelper.GetItemRectangles();
            for (int i = 0, n = PaintableCount(rects, context); i < n; i++)
            {
                // The CTA is drawn above as a raised disc carrying its own icon. Without this skip the
                // same item was also painted as an ordinary cell, so its icon and label rendered a
                // second time underneath the disc - two draws of one item. Every other CTA style
                // already skipped it; this one did not.
                if (i == context.CTAIndex) continue;

                PaintMenuItem(context.Graphics, context.Items[i], rects[i], i, context);
            }
        }

        public override void RegisterHitAreas(BottomBarPainterContext context)
        {
            if (context == null || context.HitTest == null) return;
            var idx = context.CTAIndex >= 0 ? context.CTAIndex : context.SelectedIndex;
            if (idx < 0 || idx >= context.Items.Count) return;
            var r = _layoutHelper.GetItemRect(idx);
            var hitRect = new Rectangle(r.Left - 6, r.Top - 8, r.Width + 12, r.Height + 16);
            context.BarHitTest?.SetItemHitArea(idx, hitRect);
        }
    }
}
