using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    /// <summary>
    /// A raised diamond call-to-action, with a soft diamond marking the selected item.
    /// </summary>
    /// <remarks>
    /// The diamond used to be drawn on the <b>selected</b> item at alpha 32 using
    /// <c>context.AccentColor</c> directly - so it was a barely-visible wash in the wrong place, and
    /// when no accent was set it resolved to <c>Color.FromArgb(32, Color.Empty)</c> and vanished
    /// entirely. The style is named for the reference's raised purple diamond CTA, which is a solid
    /// shape carrying its own icon.
    /// </remarks>
    internal class DiamondBottomBarPainter : BaseBottomBarPainter
    {
        public override string Name => "Diamond";

        /// <summary>This style draws its own raised CTA, so it does not take the shared disc.</summary>
        protected override bool DrawsOwnCta => true;

        /// <summary>
        /// The circle is centred 10px above the cell's middle with a radius of half the cell plus 6,
        /// so a little over half of it sits above the band. Derived rather than guessed, so it stays
        /// correct when the bar is made taller or shorter.
        /// </summary>
        public override int GetTopOverhang(int contentHeight)
        {
            var disc = CtaDiscGeometry(contentHeight, 1f);
            return Math.Max(0, DiamondHalfHeight(contentHeight) - disc.CentreOffset);
        }

        /// <summary>Half the diamond's height — the same value <c>PaintCtaDiamond</c> draws with.</summary>
        private static int DiamondHalfHeight(int contentHeight) => Math.Max(10, contentHeight / 2);

        public override void Paint(BottomBarPainterContext context)
        {
            base.CalculateLayout(context);
            var g = context.Graphics;

            using (var b = new SolidBrush(ResolveBarBack(context)))
            {
                g.FillRectangle(b, context.Bounds);
            }

            var accent = ResolveAccent(context);
            var rects = _layoutHelper.GetItemRectangles();
            bool hasCta = context.CTAIndex >= 0 && context.CTAIndex < rects.Count;

            for (int i = 0, n = PaintableCount(rects, context); i < n; i++)
            {
                var r = rects[i];

                if (hasCta && i == context.CTAIndex)
                {
                    PaintCtaDiamond(g, r, context, accent);
                    continue;   // the CTA carries its own icon
                }

                // A soft diamond behind the selected item. Resolved accent, and an alpha that can
                // actually be seen against the bar.
                if (i == context.SelectedIndex)
                {
                    var centre = new Point(r.Left + r.Width / 2, r.Top + r.Height / 2);
                    int size = Math.Min(r.Width, r.Height) / 2;
                    using var brush = new SolidBrush(Color.FromArgb(46, accent));
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.FillPolygon(brush, DiamondPoints(centre, size));
                    g.SmoothingMode = SmoothingMode.Default;
                }

                PaintMenuItem(g, context.Items[i], r, i, context);
            }
        }

        private void PaintCtaDiamond(Graphics g, Rectangle cell, BottomBarPainterContext context, Color accent)
        {
            // Same geometry the overhang reserves. Sizing from the cell instead meant the diamond
            // needed 10px above the band while the control reserved 17, leaving a strip of dead space
            // above every diamond bar - and the two numbers moved independently of each other.
            var disc = CtaDiscGeometry(context.Bounds.Height, 1f);
            var centre = new Point(cell.Left + cell.Width / 2, context.Bounds.Top + disc.CentreOffset);
            int size = DiamondHalfHeight(context.Bounds.Height);
            var points = DiamondPoints(centre, size);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Shadow beneath, so the diamond reads as lifted off the bar.
            var shadow = ResolveShadowColor(context);
            using (var shadowBrush = new SolidBrush(Color.FromArgb(Math.Min(90, shadow.A + 40), shadow.R, shadow.G, shadow.B)))
            {
                g.FillPolygon(shadowBrush, DiamondPoints(new Point(centre.X, centre.Y + 3), size));
            }

            using (var fill = new SolidBrush(accent))
            {
                g.FillPolygon(fill, points);
            }

            // Icon in the on-accent colour, centred in the diamond.
            var item = context.Items[context.CTAIndex];
            var iconRect = new Rectangle(centre.X - 12, centre.Y - 12, 24, 24);
            PaintTintedIcon(g, string.IsNullOrEmpty(item?.ImagePath) ? context.DefaultImagePath : item.ImagePath,
                            iconRect, ResolveOnAccent(context), context);

            g.SmoothingMode = SmoothingMode.Default;
        }

        private static Point[] DiamondPoints(Point centre, int size) => new[]
        {
            new Point(centre.X, centre.Y - size),
            new Point(centre.X + size, centre.Y),
            new Point(centre.X, centre.Y + size),
            new Point(centre.X - size, centre.Y)
        };
    }
}
