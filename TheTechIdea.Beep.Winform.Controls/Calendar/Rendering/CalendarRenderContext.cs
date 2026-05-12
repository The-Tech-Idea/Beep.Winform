using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Calendar;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    internal class CalendarRenderContext
    {
        public BaseControl Owner { get; }
        public IBeepTheme Theme { get; }
        public Font HeaderFont { get; }
        public Font DayFont { get; }
        public Font EventFont { get; }
        public Font TimeFont { get; }
        public Font DaysHeaderFont { get; }
        public CalendarState State { get; }
        public CalendarRects Rects { get; }
        public CalendarEventService EventService { get; }
        public List<EventCategory> Categories { get; }
        public List<CalendarResource> Resources { get; }

        // Header text margins to avoid overlap with buttons
        public int HeaderLeftMargin { get; }
        public int HeaderRightMargin { get; }
        public float DensityScale { get; }
        public DateTime? HoveredDate => State?.HoveredDate;
        public int? HoveredEventId => State?.HoveredEventId;
        public DateTime FocusedDate => State?.FocusedDate ?? DateTime.Today;
        public bool IsKeyboardFocusVisible => State?.IsKeyboardFocusVisible ?? false;
        public DateTime? VisibleRangeStart => State?.VisibleRangeStart;
        public DateTime? VisibleRangeEnd => State?.VisibleRangeEnd;
        public int MaxEventsPerCell => CalendarLayoutMetrics.MaxEventsPerCell;

        public CalendarRenderContext(BaseControl owner, IBeepTheme theme,
            Font headerFont, Font dayFont, Font eventFont, Font timeFont, Font daysHeaderFont,
            CalendarState state, CalendarRects rects, CalendarEventService eventService,
            List<EventCategory> categories, List<CalendarResource> resources, int headerLeftMargin = 160, int headerRightMargin = 20, float densityScale = 1.0f)
        {
            Owner = owner;
            Theme = theme;
            HeaderFont = headerFont;
            DayFont = dayFont;
            EventFont = eventFont;
            TimeFont = timeFont;
            DaysHeaderFont = daysHeaderFont;
            State = state;
            Rects = rects;
            EventService = eventService;
            Categories = categories;
            Resources = resources ?? new List<CalendarResource>();
            HeaderLeftMargin = headerLeftMargin;
            HeaderRightMargin = headerRightMargin;
            DensityScale = densityScale <= 0 ? 1.0f : densityScale;
        }
    }
}
