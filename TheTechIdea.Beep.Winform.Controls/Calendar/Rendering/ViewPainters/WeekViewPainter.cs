using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.ViewPainters
{
    /// <summary>
    /// Per-view painter for <see cref="CalendarViewMode.Week"/>. 7-day timed
    /// grid. Delegates the actual paint + hit-test to
    /// <see cref="TimedWeekPaintLogic"/>.
    /// </summary>
    public sealed class WeekViewPainter : ICalendarViewPainter
    {
        public CalendarViewMode ViewMode => CalendarViewMode.Week;
        public void Layout(ViewPaintArgs args) { }
        public void Paint(Graphics g, ViewPaintArgs args) => TimedWeekPaintLogic.Paint(g, args, 7);
        public CalendarInteractionHitTestResult HitTest(Point location, ViewPaintArgs args)
            => TimedWeekPaintLogic.HitTest(location, args, 7);
    }
}
