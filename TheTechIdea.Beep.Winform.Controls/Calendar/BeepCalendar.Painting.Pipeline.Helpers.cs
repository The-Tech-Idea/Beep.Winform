using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Calendar.Rendering;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        /// <summary>
        /// Build a <see cref="ViewPaintArgs"/> bundle from the current calendar
        /// state for use by interaction-time helpers (hit-test math, drag
        /// resolution, painter metadata lookups) that need the same view data
        /// the per-view painter consumes during a real paint cycle.
        ///
        /// Unlike <see cref="DrawWithPainter"/> this does NOT call
        /// <c>ApplyTheme</c>/<c>ApplyThemeFonts</c> (those mutate the bundle
        /// with colors the hit-test path doesn't read). It only populates the
        /// fields painters' interaction helpers actually use:
        /// <c>State</c>, <c>Rects</c>, <c>Surface</c>, <c>Events</c>,
        /// <c>Categories</c>, <c>Resources</c>, and <c>Owner</c>.
        ///
        /// Used by <c>GetTimedViewDateFromLocation</c> and any future
        /// painter-driven interaction helper that needs a <see cref="ViewPaintArgs"/>
        /// outside of a real <c>OnPaint</c> call.
        /// </summary>
        internal ViewPaintArgs BuildViewPaintArgsForInteraction()
        {
            return new ViewPaintArgs
            {
                ControlStyle = _calendarStyle,
                UseThemeColors = UseThemeColors,
                State = _state,
                Rects = _rects,
                Surface = _surfaceModel,
                EventService = _eventService,
                Events = _events,
                Categories = _categories,
                Resources = Resources,
                HeaderFont = HeaderFont,
                DayFont = DayFont,
                EventFont = EventFont,
                TimeFont = TimeFont,
                DaysHeaderFont = DaysHeaderFont,
                HoveredEventId = _hoveredEvent?.Id,
                HoveredDate = _hoveredDate,
                SelectedEvent = _state.SelectedEvent,
                Owner = this
            };
        }
    }
}
