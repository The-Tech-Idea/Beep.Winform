using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.ViewPainters
{
    /// <summary>
    /// Per-view painter for <see cref="CalendarViewMode.Day"/>. Single day
    /// timed grid. Delegates the actual paint + hit-test to
    /// <see cref="TimedWeekPaintLogic"/>.
    /// </summary>
    public sealed class DayViewPainter : ICalendarViewPainter
    {
        public CalendarViewMode ViewMode => CalendarViewMode.Day;
        public void Layout(ViewPaintArgs args) { }
        public void Paint(Graphics g, ViewPaintArgs args) => TimedWeekPaintLogic.Paint(g, args, 1);
        public CalendarInteractionHitTestResult HitTest(Point location, ViewPaintArgs args)
            => TimedWeekPaintLogic.HitTest(location, args, 1);
    }
}
