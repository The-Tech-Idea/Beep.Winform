using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    internal class SegmentedTrackBottomBarPainter : BaseBottomBarPainter
    {
        public override string Name => "SegmentedTrack";
        public int TrackHeight { get; set; } = 6;
        public int IndicatorWidth { get; set; } = 40;
        public int IndicatorHeight { get; set; } = 6;
        public int IndicatorRadius { get; set; } = 3;

        /// <summary>Reserves the track band at the bottom so the labels are laid out above it.</summary>
        protected override Rectangle GetContentBounds(BottomBarPainterContext context)
        {
            var r = context.Bounds;
            r.Height = Math.Max(1, r.Height - (TrackHeight + TrackBottomInset * 2));
            return r;
        }

        /// <summary>Gap between the track and the bottom edge of the bar.</summary>
        private const int TrackBottomInset = 4;

        public override void Paint(BottomBarPainterContext context)
        {
            base.CalculateLayout(context);
            var g = context.Graphics;
            var barRect = context.Bounds;
            using (var b = new SolidBrush(ResolveBarBack(context)))
                g.FillRectangle(b, barRect);

            // The track sits in the band GetContentBounds reserved, below the labels. It used to be
            // placed at `Height - TrackHeight - 10` against a grid laid out over the full height, so
            // it ran through the caption of every item.
            var trackRect = new Rectangle(
                barRect.Left + 16,
                barRect.Bottom - TrackHeight - TrackBottomInset,
                barRect.Width - 32,
                TrackHeight);
            using (var tr = new SolidBrush(context.NavigationBorderColor == Color.Empty ? context.BarHoverBackColor : context.NavigationBorderColor))
                g.FillRectangle(tr, trackRect);

            // indicator is a small rounded capsule track
            var indicatorRect = _layoutHelper.GetIndicatorRect();
            float iX = indicatorRect.Left + (indicatorRect.Width - IndicatorWidth) / 2f; // center under item
            try { if (context.AnimatedIndicatorWidth > 0f) { iX = context.AnimatedIndicatorX + (context.AnimatedIndicatorWidth - IndicatorWidth)/2f; } } catch { }
            var indRect = new RectangleF(iX, trackRect.Top - (IndicatorHeight - TrackHeight)/2f, IndicatorWidth, IndicatorHeight);
            using (var br = new SolidBrush(ResolveAccent(context)))
            using (var gp = new GraphicsPath())
            {
                int r = IndicatorRadius;
                var rect = Rectangle.Round(indRect);
                gp.AddArc(rect.Left, rect.Top, r*2, r*2, 180, 90);
                gp.AddArc(rect.Right-r*2, rect.Top, r*2, r*2, 270, 90);
                gp.AddArc(rect.Right-r*2, rect.Bottom-r*2, r*2, r*2, 0, 90);
                gp.AddArc(rect.Left, rect.Bottom-r*2, r*2, r*2, 90, 90);
                gp.CloseFigure();
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(br, gp);
                g.SmoothingMode = SmoothingMode.Default;
            }

            // draw menu items as usual
            var rects = _layoutHelper.GetItemRectangles();
            for (int i = 0; i < rects.Count; i++)
            {
                var item = context.Items[i];
                PaintMenuItem(g, item, rects[i], context);
            }
        }
    }
}
