using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    /// <summary>
    /// Per-view painter. One implementation per <see cref="CalendarViewMode"/>;
    /// each owns its own layout, paint, and hit-test. Style variations
    /// (Material3 vs Minimal) are produced by switching on the
    /// <see cref="ViewPaintArgs.ControlStyle"/> inside each implementation —
    /// there is no separate style painter hierarchy.
    ///
    /// Resolved by <see cref="ViewPainterFactory"/> from the current view mode.
    /// </summary>
    public interface ICalendarViewPainter
    {
        /// <summary>View mode this painter handles.</summary>
        CalendarViewMode ViewMode { get; }

        /// <summary>
        /// Compute and cache per-view layout for the current paint cycle. Called
        /// once per <c>BeepCalendar.UpdateLayout</c> before <see cref="Paint"/>
        /// and <see cref="HitTest"/>. Implementations populate any internal
        /// rectangle / event caches they need.
        /// </summary>
        void Layout(ViewPaintArgs args);

        /// <summary>Draw the view using the cached layout and the supplied args (theme + control style).</summary>
        void Paint(Graphics g, ViewPaintArgs args);

        /// <summary>
        /// Hit-test a point against the cached layout. Returns
        /// <see cref="CalendarInteractionTargetKind.EmptySurface"/> if nothing
        /// matches.
        /// </summary>
        CalendarInteractionHitTestResult HitTest(Point location, ViewPaintArgs args);
    }
}
