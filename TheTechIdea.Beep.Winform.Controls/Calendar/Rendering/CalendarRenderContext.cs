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

        // Header text margins to avoid overlap with buttons
        public int HeaderLeftMargin { get; }
        public int HeaderRightMargin { get; }

        public CalendarRenderContext(BaseControl owner, IBeepTheme theme,
            Font headerFont, Font dayFont, Font eventFont, Font timeFont, Font daysHeaderFont,
            CalendarState state, CalendarRects rects, CalendarEventService eventService,
            List<EventCategory> categories, int headerLeftMargin = 160, int headerRightMargin = 20)
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
            HeaderLeftMargin = headerLeftMargin;
            HeaderRightMargin = headerRightMargin;
        }
    }
}
