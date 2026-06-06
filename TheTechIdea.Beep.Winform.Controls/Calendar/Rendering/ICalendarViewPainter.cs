using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    /// <summary>
    /// Per-view painter. One implementation per <see cref="CalendarViewMode"/>;
    /// each owns its own layout, paint, hit-test, navigation, header text, and
    /// toolbar identity. Style variations (Material3 vs Minimal) are produced
    /// by switching on the <see cref="ViewPaintArgs.ControlStyle"/> inside each
    /// implementation — there is no separate style painter hierarchy.
    ///
    /// Resolved by <see cref="ViewPainterFactory"/> from the current view mode.
    /// The painter is the SOLE owner of all view-specific behavior. Central
    /// code in <c>BeepCalendar.*</c> partials calls into the painter instead
    /// of <c>switch (ViewMode)</c> statements.
    /// </summary>
    public interface ICalendarViewPainter
    {
        // ── Identity ────────────────────────────────────────────────────────

        /// <summary>View mode this painter handles.</summary>
        CalendarViewMode ViewMode { get; }

        /// <summary>Stable key for the toolbar view-selector button (e.g. "week1").</summary>
        string Key { get; }

        /// <summary>Human label for the toolbar view-selector button (e.g. "Week 1").</summary>
        string DisplayLabel { get; }

        // ── Layout classification ───────────────────────────────────────────

        /// <summary>Number of day columns shown by this view (1, 4, 5, 6, or 7).</summary>
        int VisibleDayCount { get; }

        /// <summary>True when the view shows hours (time grid, day-of-week per hour).</summary>
        bool IsTimedView { get; }

        /// <summary>True when the view is a 6×7 month grid layout.</summary>
        bool IsMonthGrid { get; }

        /// <summary>True when the view needs a left-side time-label gutter.</summary>
        bool RequiresLeftGutter { get; }

        /// <summary>True when the view supports per-day all-day event strips above the time grid.</summary>
        bool HasAllDayStrip { get; }

        /// <summary>
        /// True when events in this view can be dragged to a new time / date
        /// (move gesture). True for timed views and resource-lane views
        /// (Timeline). False for month grids and event-card lists where the
        /// user selects an event rather than dragging it.
        /// </summary>
        bool SupportsEventDrag { get; }

        /// <summary>
        /// True when the time axis runs horizontally (resource lanes × day axis,
        /// Gantt tasks × time axis). When true, drag-delta resolution in
        /// <c>BeepCalendar.Interactions.Timing.Snap.cs</c> maps the mouse
        /// delta along X to a time delta (days) rather than along Y to minutes.
        /// Default false (vertical time axis = the standard day-grid-with-hours).
        /// </summary>
        bool IsHorizontalTimeAxis { get; }

        // ── Date / navigation ───────────────────────────────────────────────

        /// <summary>Compute the previous period's anchor date.</summary>
        DateTime NavigatePrevious(DateTime currentDate);

        /// <summary>Compute the next period's anchor date.</summary>
        DateTime NavigateNext(DateTime currentDate);

        /// <summary>Header text (rendered in the title bar).</summary>
        string GetHeaderText(DateTime currentDate);

        /// <summary>Start of the visible date range (for <c>GetEventsForDateRange</c> queries).</summary>
        DateTime GetVisibleRangeStart(DateTime currentDate);

        /// <summary>End of the visible date range (exclusive).</summary>
        DateTime GetVisibleRangeEnd(DateTime currentDate);

        // ── Layout / paint / hit-test ───────────────────────────────────────

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

        // ── Time-grid interaction (only meaningful for IsTimedView == true) ─

        /// <summary>
        /// Convert a point in the time grid to a (date, time-of-day) tuple.
        /// Returns null for views without a time grid.
        /// </summary>
        DateTime? GetDateTimeFromLocation(Point location, ViewPaintArgs args);
    }
}
