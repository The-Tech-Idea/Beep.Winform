using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    internal class FloatingCTABottomBarPainter : BaseBottomBarPainter
    {
        public override string Name => "FloatingCTA";

        /// <summary>This style draws its own raised CTA, so it does not take the shared disc.</summary>
        protected override bool DrawsOwnCta => true;

        /// <summary>This style breathes its selected decoration, so it needs the ticker.</summary>
        public override bool WantsContinuousAnimation => true;

        /// <summary>Scale of the soft halo drawn around the CTA disc.</summary>
        public float HaloScale { get; set; } = 1.35f;

        /// <summary>Room for the halo, taken from the same geometry the paint uses.</summary>
        public override int GetTopOverhang(int contentHeight)
        {
            var disc = CtaDiscGeometry(contentHeight, HaloScale);
            return Math.Max(0, disc.OuterRadius - disc.CentreOffset);
        }
        public float NotchRadiusFactor { get; set; } = 1.05f;
        public override void Paint(BottomBarPainterContext context)
        {
            // Ensure layout is calculated
            base.CalculateLayout(context);
            // draw base background (with optional notch if CTA present)
            var barRect = context.Bounds;
            var barFill = ResolveBarBack(context);
            using (var barBrush = new SolidBrush(barFill))
                if (context.CTAIndex >= 0 && context.CTAIndex < context.Items.Count)
                {
                    var rect = _layoutHelper.GetItemRect(context.CTAIndex);
                    var center = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2 - 10);
                    int baseRadius = Math.Min(rect.Width, rect.Height) / 2 + 6;
                    int radius = (int)(baseRadius * (1.0f + 0.06f * context.AnimationPhase));
                    int notchRadius = (int)(radius * NotchRadiusFactor);

                    var barRadius = barRect.Height / 2;
                    var barPath = new GraphicsPath();
                    barPath.AddArc(barRect.Left, barRect.Top, barRadius, barRadius, 180, 90);
                    barPath.AddArc(barRect.Right - barRadius, barRect.Top, barRadius, barRadius, 270, 90);
                    barPath.AddArc(barRect.Right - barRadius, barRect.Bottom - barRadius, barRadius, barRadius, 0, 90);
                    barPath.AddArc(barRect.Left, barRect.Bottom - barRadius, barRadius, barRadius, 90, 90);
                    barPath.CloseFigure();

                    var notchPath = new GraphicsPath();
                    notchPath.AddEllipse(center.X - notchRadius, center.Y - notchRadius, notchRadius * 2, notchRadius * 2);

                    using (var region = new System.Drawing.Region(barPath))
                    {
                        region.Exclude(notchPath);
                        context.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        using (var themeBrush = new SolidBrush(barFill))
                        {
                            context.Graphics.FillRegion(themeBrush, region);
                        }
                        context.Graphics.SmoothingMode = SmoothingMode.Default;
                    }
                }
                else
                {
                    context.Graphics.FillRectangle(barBrush, barRect);
                }


            // draw CTA if set
            if (context.CTAIndex >= 0 && context.CTAIndex < context.Items.Count)
            {
                var rect = _layoutHelper.GetItemRect(context.CTAIndex);
                var disc = CtaDiscGeometry(context.Bounds.Height, HaloScale);
                var center = new Point(rect.Left + rect.Width / 2, context.Bounds.Top + disc.CentreOffset);
                int baseRadius = disc.Radius;
                // pulsing scale from context animation phase -> 1.0 .. 1.06. The pulse grows the disc,
                // so the overhang has to cover the disc at full pulse, not at rest - CtaDiscGeometry's
                // outer radius already accounts for it through HaloScale.
                float scale = 1.0f + 0.06f * context.AnimationPhase;
                int radius = (int)(baseRadius * scale);

                // small decorative notch circle above the bar center
                var decorativeNotchRadius = Math.Min(8, rect.Width / 8);
                var decorativeNotchCenter = new Point(center.X, context.Bounds.Top);
                using (var notchFill = new SolidBrush(barFill))
                {
                    var notchPenBase = context.NavigationBorderColor == Color.Empty ? ResolveBarFore(context) : context.NavigationBorderColor;
                    var notchPenColor = Color.FromArgb(200, notchPenBase.R, notchPenBase.G, notchPenBase.B);
                    using (var notchPen = new Pen(notchPenColor, 1f))
                    {
                        var notchRect = new Rectangle(decorativeNotchCenter.X - decorativeNotchRadius, decorativeNotchCenter.Y - decorativeNotchRadius / 2 - 2, decorativeNotchRadius * 2, decorativeNotchRadius * 2);
                        context.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        context.Graphics.FillEllipse(notchFill, notchRect);
                        context.Graphics.DrawEllipse(notchPen, notchRect);
                        context.Graphics.SmoothingMode = SmoothingMode.Default;
                    }
                    // Use themed accent / fore color for notch stroke/outline (if desired)

                    // shadow - use theme hover color with alpha for better dark-mode handling
                    // layered shadow for CTA - use NavigationShadowColor RGB as base but render two fills
                    var baseShadow = context.NavigationShadowColor == Color.Empty ? Color.FromArgb(100, ResolveBarFore(context)) : context.NavigationShadowColor;
                    // outer soft halo: larger ellipse + lower alpha
                    var outerAlpha = Math.Max(24, baseShadow.A / 5); // softer outer
                    using (var outer = new SolidBrush(Color.FromArgb(outerAlpha, baseShadow.R, baseShadow.G, baseShadow.B)))
                    {
                        var outerRect = new Rectangle(center.X - disc.OuterRadius, center.Y - disc.OuterRadius + context.CTAShadowYOffset, disc.OuterRadius * 2, disc.OuterRadius * 2);
                        context.Graphics.FillEllipse(outer, outerRect);
                    }
                    // inner shadow: more concentrated and slightly offset
                    var innerAlpha = Math.Min(160, baseShadow.A + 40);
                    using (var inner = new SolidBrush(Color.FromArgb(innerAlpha, baseShadow.R, baseShadow.G, baseShadow.B)))
                    {
                        var innerRect = new Rectangle(center.X - radius, center.Y - radius + context.CTAShadowYOffset, radius * 2, radius * 2);
                        context.Graphics.FillEllipse(inner, innerRect);
                    }

                    // cta circle with slight glow
                    using (var fill = new SolidBrush(ResolveAccent(context)))
                    using (var pen = new Pen(ResolveOnAccent(context), 2f))
                    {
                        var circleRect = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);
                        context.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        // soft halo
                        using (var halo = new SolidBrush(Color.FromArgb(32, ResolveAccent(context))))
                        {
                            var haloRect = new Rectangle(center.X - disc.OuterRadius, center.Y - disc.OuterRadius, disc.OuterRadius * 2, disc.OuterRadius * 2);
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
                    PaintTintedIcon(context.Graphics,
                                    string.IsNullOrEmpty(context.Items[context.CTAIndex].ImagePath)
                                        ? context.DefaultImagePath : context.Items[context.CTAIndex].ImagePath,
                                    scaledIconRect, ResolveOnAccent(context), context);
                }

                // handle indicator for normal items - rounded pill using animated values if provided
                var indicatorRect = _layoutHelper.GetIndicatorRect();
                float iX = indicatorRect.Left;
                float iW = indicatorRect.Width;
                if (context.AnimatedIndicatorWidth > 0f)
                {
                    iX = context.AnimatedIndicatorX;
                    iW = context.AnimatedIndicatorWidth;
                }
                var iRect = new RectangleF(iX, indicatorRect.Top, iW, indicatorRect.Height);
                using (var ib = new SolidBrush(Color.FromArgb(25, ResolveAccent(context))))
                using (var gp = new GraphicsPath())
                {
                    int adjustedRadius = Math.Max(2, (int)Math.Round(iRect.Height / 2f));
                    var adjustedRect = Rectangle.Round(iRect);
                    gp.AddArc(adjustedRect.Left, adjustedRect.Top, adjustedRadius * 2, adjustedRadius * 2, 180, 90);
                    gp.AddArc(adjustedRect.Right - adjustedRadius * 2, adjustedRect.Top, adjustedRadius * 2, adjustedRadius * 2, 270, 90);
                    gp.AddArc(adjustedRect.Right - adjustedRadius * 2, adjustedRect.Bottom - adjustedRadius * 2, adjustedRadius * 2, adjustedRadius * 2, 0, 90);
                    gp.AddArc(adjustedRect.Left, adjustedRect.Bottom - adjustedRadius * 2, adjustedRadius * 2, adjustedRadius * 2, 90, 90);
                    gp.CloseFigure();
                    context.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    context.Graphics.FillPath(ib, gp);
                    context.Graphics.SmoothingMode = SmoothingMode.Default;
                }

            }

            // Items, whether or not a CTA is configured. This loop used to sit inside the
            // `if (CTAIndex >= 0)` above, so a bar without a CTA painted its background and stopped:
            // every item invisible, the whole style rendering as an empty white strip. Having no CTA
            // is a configuration, not a reason to draw nothing. Both sibling styles,
            // OutlineFloatingCTA and MovableNotch, already had their loop outside the conditional.
            var rects = _layoutHelper.GetItemRectangles();
            for (int i = 0, n = PaintableCount(rects, context); i < n; i++)
            {
                if (i == context.CTAIndex) continue; // CTA drawn separately
                var item = context.Items[i];
                PaintMenuItem(context.Graphics, item, rects[i], context);
            }
        }
        // Hit area registration is handled by the more detailed animated method below

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
            context.BarHitTest?.SetItemHitArea(cta, circleRect);
        }
    }
}
