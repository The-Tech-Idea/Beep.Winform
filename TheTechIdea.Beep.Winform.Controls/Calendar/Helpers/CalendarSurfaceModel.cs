using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Helpers
{
    /// <summary>
    /// Immutable snapshot of the calendar surface geometry for one paint cycle.
    /// Built once per <see cref="BeepCalendar.UpdateLayout"/> call and consumed by
    /// every painter and hit-test helper. Replaces the geometry math that was
    /// previously scattered across <c>BeepCalendar.LayoutTheme.*</c> and
    /// <c>Helpers/CalendarLayoutGeometry.cs</c>.
    /// </summary>
    public sealed class CalendarSurfaceModel
    {
        #region Root Rects (from layout)

        public Rectangle ClientRect { get; }
        public Rectangle HeaderRect { get; }
        public Rectangle ViewSelectorRect { get; }
        public Rectangle CalendarGridRect { get; }
        public Rectangle SidebarRect { get; }

        #endregion

        #region View State

        public CalendarViewMode ViewMode { get; }
        public DateTime CurrentDate { get; }
        public DateTime SelectedDate { get; }
        public DateTime FocusedDate { get; }
        public DateTime? VisibleRangeStart { get; }
        public DateTime? VisibleRangeEnd { get; }
        public bool ShowSidebar { get; }

        #endregion

        #region Scaled Metrics

        public int DayHeaderHeight { get; }
        public int TimeColumnWidth { get; }
        public int TimeSlotHeight { get; }
        public int EventInsetX { get; }
        public int EventInsetY { get; }
        public int MinEventHitHeight { get; }
        public int EventBarHeight { get; }
        public int EventSpacing { get; }
        public int SidebarPadding { get; }
        public int SidebarCardHeight { get; }
        public int SidebarCardGap { get; }
        public int ListRowHeight { get; }
        public int ListRowSpacing { get; }
        public int CornerRadius { get; }
        public int CellPadding { get; }
        public int MaxEventsPerCell { get; }

        #endregion

        #region Per-view Anchors

        public DateTime StartOfWeek { get; }
        public DateTime StartOfWorkWeek { get; }
        public DateTime FirstDayOfMonth { get; }
        public DateTime FirstDayOfCalendar { get; }
        public int WeekDayCount => ViewMode == CalendarViewMode.WorkWeek ? 5 : 7;

        #endregion

        #region Derived Regions

        public Rectangle MonthHeaderBand { get; }
        public Rectangle MonthBody { get; }
        public Rectangle WeekHeaderBand { get; }
        public Rectangle TimedArea { get; }
        public Rectangle DayHeaderRow { get; }
        public int SidebarWidth => SidebarRect.Width;

        #endregion

        #region Header Text Padding

        public int HeaderLeftPadding { get; }
        public int HeaderRightPadding { get; }

        #endregion

        private CalendarSurfaceModel(
            Rectangle clientRect, Rectangle headerRect, Rectangle viewSelectorRect,
            Rectangle calendarGridRect, Rectangle sidebarRect,
            CalendarViewMode viewMode, DateTime currentDate, DateTime selectedDate, DateTime focusedDate,
            DateTime? visibleRangeStart, DateTime? visibleRangeEnd, bool showSidebar,
            int dayHeaderHeight, int timeColumnWidth, int timeSlotHeight,
            int eventInsetX, int eventInsetY, int minEventHitHeight,
            int eventBarHeight, int eventSpacing,
            int sidebarPadding, int sidebarCardHeight, int sidebarCardGap,
            int listRowHeight, int listRowSpacing,
            int cornerRadius, int cellPadding, int maxEventsPerCell,
            int headerLeftPadding, int headerRightPadding)
        {
            ClientRect = clientRect;
            HeaderRect = headerRect;
            ViewSelectorRect = viewSelectorRect;
            CalendarGridRect = calendarGridRect;
            SidebarRect = sidebarRect;
            ViewMode = viewMode;
            CurrentDate = currentDate;
            SelectedDate = selectedDate;
            FocusedDate = focusedDate;
            VisibleRangeStart = visibleRangeStart;
            VisibleRangeEnd = visibleRangeEnd;
            ShowSidebar = showSidebar;
            DayHeaderHeight = dayHeaderHeight;
            TimeColumnWidth = timeColumnWidth;
            TimeSlotHeight = timeSlotHeight;
            EventInsetX = eventInsetX;
            EventInsetY = eventInsetY;
            MinEventHitHeight = minEventHitHeight;
            EventBarHeight = eventBarHeight;
            EventSpacing = eventSpacing;
            SidebarPadding = sidebarPadding;
            SidebarCardHeight = sidebarCardHeight;
            SidebarCardGap = sidebarCardGap;
            ListRowHeight = listRowHeight;
            ListRowSpacing = listRowSpacing;
            CornerRadius = cornerRadius;
            CellPadding = cellPadding;
            MaxEventsPerCell = maxEventsPerCell;
            HeaderLeftPadding = headerLeftPadding;
            HeaderRightPadding = headerRightPadding;

            // Per-view anchors
            StartOfWeek = currentDate.Date.AddDays(-(int)currentDate.DayOfWeek);
            int dayOfWeek = (int)currentDate.DayOfWeek;
            int mondayOffset = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
            StartOfWorkWeek = currentDate.Date.AddDays(-mondayOffset);
            FirstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            FirstDayOfCalendar = FirstDayOfMonth.AddDays(-(int)FirstDayOfMonth.DayOfWeek);

            // Derived regions
            MonthHeaderBand = new Rectangle(calendarGridRect.X, calendarGridRect.Y,
                calendarGridRect.Width, Math.Min(dayHeaderHeight, Math.Max(0, calendarGridRect.Height)));
            MonthBody = new Rectangle(calendarGridRect.X, MonthHeaderBand.Bottom,
                calendarGridRect.Width, Math.Max(0, calendarGridRect.Bottom - MonthHeaderBand.Bottom));
            DayHeaderRow = MonthHeaderBand;
            WeekHeaderBand = new Rectangle(
                calendarGridRect.X + timeColumnWidth, calendarGridRect.Y,
                Math.Max(0, calendarGridRect.Width - timeColumnWidth),
                Math.Min(dayHeaderHeight, Math.Max(0, calendarGridRect.Height)));
            TimedArea = new Rectangle(
                calendarGridRect.X + Math.Max(0, timeColumnWidth),
                calendarGridRect.Y + Math.Max(0, dayHeaderHeight),
                Math.Max(0, calendarGridRect.Right - (calendarGridRect.X + Math.Max(0, timeColumnWidth))),
                Math.Max(0, calendarGridRect.Bottom - (calendarGridRect.Y + Math.Max(0, dayHeaderHeight))));
        }

        #region Factory

        public static CalendarSurfaceModel Build(
            CalendarState state,
            CalendarRects rects,
            int dayHeaderHeight,
            int timeColumnWidth,
            int timeSlotHeight,
            int eventInsetX,
            int eventInsetY,
            int minEventHitHeight,
            int eventBarHeight,
            int eventSpacing,
            int sidebarPadding,
            int sidebarCardHeight,
            int sidebarCardGap,
            int listRowHeight,
            int listRowSpacing,
            int cornerRadius,
            int cellPadding,
            int maxEventsPerCell,
            int headerLeftPadding,
            int headerRightPadding)
        {
            return new CalendarSurfaceModel(
                rects.CalendarGridRect, // clientRect fallback
                rects.HeaderRect,
                rects.ViewSelectorRect,
                rects.CalendarGridRect,
                rects.SidebarRect,
                state.ViewMode,
                state.CurrentDate,
                state.SelectedDate,
                state.FocusedDate,
                state.VisibleRangeStart,
                state.VisibleRangeEnd,
                state.ShowSidebar,
                dayHeaderHeight,
                timeColumnWidth,
                timeSlotHeight,
                eventInsetX,
                eventInsetY,
                minEventHitHeight,
                eventBarHeight,
                eventSpacing,
                sidebarPadding,
                sidebarCardHeight,
                sidebarCardGap,
                listRowHeight,
                listRowSpacing,
                cornerRadius,
                cellPadding,
                maxEventsPerCell,
                headerLeftPadding,
                headerRightPadding);
        }

        #endregion

        #region Pure Geometry Helpers (moved from CalendarLayoutGeometry)

        public Rectangle GetColumnRect(Rectangle bounds, int index, int count)
        {
            if (count <= 0 || bounds.Width <= 0) return Rectangle.Empty;
            index = Math.Max(0, Math.Min(count - 1, index));
            int left = bounds.Left + (bounds.Width * index / count);
            int right = bounds.Left + (bounds.Width * (index + 1) / count);
            return new Rectangle(left, bounds.Top, Math.Max(0, right - left), bounds.Height);
        }

        public Rectangle GetRowRect(Rectangle bounds, int index, int count)
        {
            if (count <= 0 || bounds.Height <= 0) return Rectangle.Empty;
            index = Math.Max(0, Math.Min(count - 1, index));
            int top = bounds.Top + (bounds.Height * index / count);
            int bottom = bounds.Top + (bounds.Height * (index + 1) / count);
            return new Rectangle(bounds.Left, top, bounds.Width, Math.Max(0, bottom - top));
        }

        public int GetColumnIndex(Rectangle bounds, int x, int count)
        {
            if (count <= 0 || bounds.Width <= 0 || x < bounds.Left || x >= bounds.Right) return -1;
            int relativeX = Math.Max(0, Math.Min(bounds.Width - 1, x - bounds.Left));
            return Math.Max(0, Math.Min(count - 1, relativeX * count / bounds.Width));
        }

        public int GetRowIndex(Rectangle bounds, int y, int count)
        {
            if (count <= 0 || bounds.Height <= 0 || y < bounds.Top || y >= bounds.Bottom) return -1;
            int relativeY = Math.Max(0, Math.Min(bounds.Height - 1, y - bounds.Top));
            return Math.Max(0, Math.Min(count - 1, relativeY * count / bounds.Height));
        }

        public const int MinutesPerDay = 24 * 60;

        public Rectangle GetTimedAreaRect(Rectangle grid, int timeColumnWidth, int dayHeaderHeight)
        {
            int x = grid.Left + Math.Max(0, timeColumnWidth);
            int y = grid.Top + Math.Max(0, dayHeaderHeight);
            return new Rectangle(x, y,
                Math.Max(0, grid.Right - x),
                Math.Max(0, grid.Bottom - y));
        }

        public int GetMinuteFromY(Rectangle timedArea, int y)
        {
            if (timedArea.Height <= 0) return 0;
            int relativeY = Math.Max(0, Math.Min(timedArea.Height, y - timedArea.Top));
            return Math.Max(0, Math.Min(MinutesPerDay - 1, relativeY * MinutesPerDay / timedArea.Height));
        }

        public Rectangle GetTimedEventRect(Rectangle dayColumnRect, CalendarEvent evt, DateTime dayDate)
        {
            if (evt == null || dayColumnRect.Width <= 0 || dayColumnRect.Height <= 0) return Rectangle.Empty;
            double startMinutes = evt.IsAllDay ? 0 : (evt.StartTime - dayDate.Date).TotalMinutes;
            double endMinutes = evt.IsAllDay ? MinutesPerDay : (evt.EndTime - dayDate.Date).TotalMinutes;
            startMinutes = Math.Max(0, Math.Min(MinutesPerDay, startMinutes));
            endMinutes = Math.Max(0, Math.Min(MinutesPerDay, endMinutes));
            if (endMinutes <= startMinutes)
            {
                double fallbackDuration = Math.Max(30, evt.Duration.TotalMinutes);
                endMinutes = Math.Min(MinutesPerDay, startMinutes + fallbackDuration);
            }
            int top = dayColumnRect.Top + (int)Math.Round(dayColumnRect.Height * startMinutes / MinutesPerDay);
            int bottom = dayColumnRect.Top + (int)Math.Round(dayColumnRect.Height * endMinutes / MinutesPerDay);
            int horizontalInset = Math.Min(Math.Max(0, EventInsetX), Math.Max(0, (dayColumnRect.Width - 1) / 2));
            int verticalInset = Math.Min(Math.Max(0, EventInsetY), Math.Max(0, (dayColumnRect.Height - 1) / 2));
            int x = dayColumnRect.Left + horizontalInset;
            int y = top + verticalInset;
            int width = Math.Max(1, dayColumnRect.Width - (horizontalInset * 2));
            int availableHeight = Math.Max(1, dayColumnRect.Bottom - y - verticalInset);
            int requestedHeight = Math.Max(MinEventHitHeight, bottom - top - (verticalInset * 2));
            int height = Math.Max(1, Math.Min(availableHeight, requestedHeight));
            return new Rectangle(x, y, width, height);
        }

        #endregion

        #region View-specific Geometry

        public Rectangle GetMonthCellRect(DateTime date)
        {
            if (MonthBody.Width <= 0 || MonthBody.Height <= 0) return Rectangle.Empty;
            int offset = (date.Date - FirstDayOfCalendar.Date).Days;
            if (offset < 0 || offset > 41) return Rectangle.Empty;
            int col = offset % 7;
            int row = offset / 7;
            var rowRect = GetRowRect(MonthBody, row, 6);
            return GetColumnRect(rowRect, col, 7);
        }

        public Rectangle GetMonthCellEventRect(int offset, int eventIndex, int eventStartOffset)
        {
            if (offset < 0 || offset > 41) return Rectangle.Empty;
            int col = offset % 7;
            int row = offset / 7;
            var rowRect = GetRowRect(MonthBody, row, 6);
            var cellRect = GetColumnRect(rowRect, col, 7);
            int y = cellRect.Y + eventStartOffset + eventIndex * (EventBarHeight + EventSpacing);
            return new Rectangle(cellRect.X + 2, y, Math.Max(1, cellRect.Width - 4), EventBarHeight);
        }

        public Rectangle GetWeekDayHeaderRect(int dayIndex)
        {
            return GetColumnRect(WeekHeaderBand, dayIndex, WeekDayCount);
        }

        public Rectangle GetWeekDayColumnRect(int dayIndex)
        {
            return GetColumnRect(TimedArea, dayIndex, WeekDayCount);
        }

        public Rectangle GetTimeRowRect(int hour)
        {
            return GetRowRect(TimedArea, hour, 24);
        }

        public DateTime GetWeekDayDate(int dayIndex)
        {
            DateTime start = ViewMode == CalendarViewMode.WorkWeek ? StartOfWorkWeek : StartOfWeek;
            return start.AddDays(dayIndex);
        }

        public Rectangle GetListRowRect(int rowIndex)
        {
            int padding = SidebarPadding;
            int y = CalendarGridRect.Y + padding + rowIndex * (ListRowHeight + ListRowSpacing);
            return new Rectangle(
                CalendarGridRect.X + padding, y,
                Math.Max(1, CalendarGridRect.Width - (padding * 2)),
                ListRowHeight);
        }

        public Rectangle GetSidebarMiniCalendarRect()
        {
            int padding = SidebarPadding;
            return new Rectangle(
                SidebarRect.X + padding, SidebarRect.Y + padding,
                Math.Max(10, SidebarRect.Width - (padding * 2)),
                SidebarCardHeight);
        }

        public Rectangle GetSidebarEventDetailsRect()
        {
            int padding = SidebarPadding;
            return new Rectangle(
                SidebarRect.X + padding,
                SidebarRect.Y + padding + SidebarCardHeight + SidebarCardGap,
                Math.Max(10, SidebarRect.Width - (padding * 2)),
                SidebarCardHeight);
        }

        public Rectangle GetHeaderTextBounds()
        {
            Rectangle headerRect = HeaderRect;
            if (headerRect.Width <= 0 || headerRect.Height <= 0) return Rectangle.Empty;
            int left = Math.Max(0, HeaderLeftPadding);
            int right = Math.Max(0, HeaderRightPadding);
            int availableLeft = headerRect.X + left;
            int availableRight = headerRect.Right - right;
            if (availableRight <= availableLeft)
            {
                availableLeft = headerRect.X;
                availableRight = headerRect.Right;
            }
            return new Rectangle(availableLeft, headerRect.Y,
                Math.Max(1, availableRight - availableLeft), headerRect.Height);
        }

        #endregion
    }
}
