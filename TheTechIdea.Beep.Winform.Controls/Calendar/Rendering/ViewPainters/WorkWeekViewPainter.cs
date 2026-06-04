using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.ViewPainters
{
    /// <summary>
    /// Per-view painter for <see cref="CalendarViewMode.WorkWeek"/>. 5-day
    /// timed grid (Mon-Fri). Delegates the actual paint + hit-test to
    /// <see cref="TimedWeekPaintLogic"/>.
    /// </summary>
    public sealed class WorkWeekViewPainter : ICalendarViewPainter
    {
        public CalendarViewMode ViewMode => CalendarViewMode.WorkWeek;
        public void Layout(ViewPaintArgs args) { }
        public void Paint(Graphics g, ViewPaintArgs args) => TimedWeekPaintLogic.Paint(g, args, 5);
        public CalendarInteractionHitTestResult HitTest(Point location, ViewPaintArgs args)
            => TimedWeekPaintLogic.HitTest(location, args, 5);
    }
}
