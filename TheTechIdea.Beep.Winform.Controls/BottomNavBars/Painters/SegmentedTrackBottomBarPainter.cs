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

        public override void Paint(BottomBarPainterContext context)
        {
            base.CalculateLayout(context);
            var g = context.Graphics;
            var barRect = context.Bounds;
            using (var b = new SolidBrush(context.BarBackColor == Color.Empty ? Color.White : context.BarBackColor))
                g.FillRectangle(b, barRect);

            // draw the small track across the bar, slightly inset from the bottom
            var trackRect = new Rectangle(barRect.Left + 16, barRect.Top + barRect.Height - TrackHeight - 10, barRect.Width - 32, TrackHeight);
            using (var tr = new SolidBrush(context.BarHoverBackColor == Color.Empty ? Color.FromArgb(240,240,240) : context.BarHoverBackColor))
                g.FillRectangle(tr, trackRect);

            // indicator is a small rounded capsule track
            var indicatorRect = _layoutHelper.GetIndicatorRect();
            float iX = indicatorRect.Left + (indicatorRect.Width - IndicatorWidth) / 2f; // center under item
            try { if (context.AnimatedIndicatorWidth > 0f) { iX = context.AnimatedIndicatorX + (context.AnimatedIndicatorWidth - IndicatorWidth)/2f; } } catch { }
            var indRect = new RectangleF(iX, trackRect.Top - (IndicatorHeight - TrackHeight)/2f, IndicatorWidth, IndicatorHeight);
            using (var br = new SolidBrush(context.AccentColor))
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
