using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private (DateTime Start, DateTime End) BuildCreationRange(Point location)
        {
            var anchorPoint = _pointerDown ? _pointerDownLocation : location;
            var fallbackStart = GetSnappedStartFromLocation(anchorPoint) ?? _state.SelectedDate.Date.AddHours(9);
            var fallbackEnd = fallbackStart.AddMinutes(Math.Max(InteractionSnapIntervalMinutes, 60));

            if (_viewPainter == null || !_viewPainter.IsTimedView)
            {
                return (fallbackStart, fallbackEnd);
            }

            var anchor = GetTimedViewDateFromLocation(anchorPoint);
            var current = GetTimedViewDateFromLocation(location);
            if (!anchor.HasValue || !current.HasValue)
            {
                return (fallbackStart, fallbackEnd);
            }

            var snappedAnchor = SnapDateTime(anchor.Value);
            var snappedCurrent = SnapDateTime(current.Value);
            var start = snappedAnchor <= snappedCurrent ? snappedAnchor : snappedCurrent;
            var end = snappedAnchor <= snappedCurrent ? snappedCurrent : snappedAnchor;

            if (end <= start)
            {
                end = start.AddMinutes(Math.Max(InteractionSnapIntervalMinutes, 60));
            }

            return (start, end);
        }
    }
}