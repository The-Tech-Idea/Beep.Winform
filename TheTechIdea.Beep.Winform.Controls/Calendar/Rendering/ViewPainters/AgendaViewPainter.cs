using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.CellRender;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.ViewPainters
{
    /// <summary>
    /// Per-view painter for <see cref="CalendarViewMode.Agenda"/>. Month
    /// events grouped by day, each group prefixed with a date header.
    /// </summary>
    public sealed class AgendaViewPainter : ICalendarViewPainter
    {
        public CalendarViewMode ViewMode => CalendarViewMode.Agenda;
        public string Key => "agenda";
        public string DisplayLabel => "Agenda";
        public int VisibleDayCount => 7;
        public bool IsTimedView => false;
        public bool IsMonthGrid => false;
        public bool RequiresLeftGutter => false;
        public bool HasAllDayStrip => false;
        public bool SupportsEventDrag => false;
        public bool IsHorizontalTimeAxis => false;

        public DateTime NavigatePrevious(DateTime d) => d.AddMonths(-1);
        public DateTime NavigateNext(DateTime d) => d.AddMonths(1);
        public string GetHeaderText(DateTime d) => d.ToString("MMMM yyyy") + " Agenda";
        public DateTime GetVisibleRangeStart(DateTime d) => new DateTime(d.Year, d.Month, 1);
        public DateTime GetVisibleRangeEnd(DateTime d) => GetVisibleRangeStart(d).AddMonths(1);

        public void Layout(ViewPaintArgs args) { }

        public void Paint(Graphics g, ViewPaintArgs args)
        {
            if (args.Surface == null) return;
            var surface = args.Surface;
            var grid = args.Rects.CalendarGridRect;
            int rowHeight = surface.ListRowHeight;
            int rowSpacing = surface.ListRowSpacing;
            int padding = surface.SidebarPadding;
            int headerHeight = Math.Max(20, (args.HeaderFont?.Height ?? 14) + 6);

            var groups = (args.EventService?.GetEventsForMonth(surface.CurrentDate) ?? new List<CalendarEvent>())
                .OrderBy(e => e.StartTime)
                .GroupBy(e => e.StartTime.Date)
                .OrderBy(g2 => g2.Key)
                .ToList();

            int y = grid.Y + padding;
            int rowIndex = 0;
            foreach (var group in groups)
            {
                if (y > grid.Bottom) break;
                var headerRect = new Rectangle(grid.X + padding, y,
                    Math.Max(1, grid.Width - (padding * 2)), headerHeight);

                // W8 - delegate to developer's IBeepUIComponent DateCell factory for
                // the agenda group header (which is a date cell) when one is
                // registered. Falls through to the default filled header otherwise.
                string cellKey = $"date:{group.Key:yyyy-MM-dd}:agenda-header";
                var ctx = new CalendarCellContext(
                    CalendarCellKind.DateCell, null, group.Key,
                    args.Surface?.ViewMode ?? CalendarViewMode.Agenda, rowIndex, 0);
                if (CalendarPainterHelpers.TryDrawCellComponent(g, headerRect, cellKey, ctx, args))
                {
                    y += headerHeight + rowSpacing;
                    foreach (var evt in group)
                    {
                        if (y > grid.Bottom) break;
                        y += surface.ListRowHeight + surface.ListRowSpacing;
                    }
                    y += rowSpacing;
                    continue;
                }

                CalendarPainterHelpers.FillRoundedRect(g, headerRect,
                    args.Metrics.CornerRadius, args.SurfaceHeaderBack());
                CalendarPainterHelpers.DrawText(g,
                    group.Key.ToString("dddd, MMMM d, yyyy"),
                    args.HeaderFont ?? args.DayFont, args.ForegroundColor,
                    headerRect, StringAlignment.Near, StringAlignment.Center);
                y += headerHeight + rowSpacing;

                foreach (var evt in group)
                {
                    if (y > grid.Bottom) break;
                    var rowRect = new Rectangle(grid.X + padding, y,
                        Math.Max(1, grid.Width - (padding * 2)), rowHeight);
                    PaintAgendaRow(g, rowRect, evt, args);
                    y += rowHeight + rowSpacing;
                    rowIndex++;
                }
                y += rowSpacing;
            }
        }

        public CalendarInteractionHitTestResult HitTest(Point location, ViewPaintArgs args)
        {
            if (args.Surface == null)
                return EmptyHit(location, args);
            var surface = args.Surface;
            var grid = args.Rects.CalendarGridRect;
            int rowHeight = surface.ListRowHeight;
            int rowSpacing = surface.ListRowSpacing;
            int padding = surface.SidebarPadding;
            int headerHeight = Math.Max(20, (args.HeaderFont?.Height ?? 14) + 6);

            var groups = (args.EventService?.GetEventsForMonth(surface.CurrentDate) ?? new List<CalendarEvent>())
                .OrderBy(e => e.StartTime)
                .GroupBy(e => e.StartTime.Date)
                .OrderBy(g2 => g2.Key)
                .ToList();

            int y = grid.Y + padding;
            int rowIndex = 0;
            foreach (var group in groups)
            {
                if (y > grid.Bottom) break;
                var headerRect = new Rectangle(grid.X + padding, y,
                    Math.Max(1, grid.Width - (padding * 2)), headerHeight);
                if (headerRect.Contains(location))
                {
                    return new CalendarInteractionHitTestResult
                    {
                        TargetKind = CalendarInteractionTargetKind.DateCell,
                        RequestedMode = CalendarInteractionMode.SelectDate,
                        Location = location,
                        Date = group.Key,
                        Bounds = headerRect
                    };
                }
                y += headerHeight + rowSpacing;

                foreach (var evt in group)
                {
                    if (y > grid.Bottom) break;
                    var rowRect = new Rectangle(grid.X + padding, y,
                        Math.Max(1, grid.Width - (padding * 2)), rowHeight);
                    if (rowRect.Contains(location))
                    {
                        var edge = CalendarPainterHelpers.ResolveResizeEdge(location, rowRect, 6);
                        return new CalendarInteractionHitTestResult
                        {
                            TargetKind = CalendarInteractionTargetKind.EventBlock,
                            RequestedMode = edge == CalendarEventResizeEdge.Start
                                ? CalendarInteractionMode.ResizeStart
                                : edge == CalendarEventResizeEdge.End
                                    ? CalendarInteractionMode.ResizeEnd
                                    : CalendarInteractionMode.SelectEvent,
                            ResizeEdge = edge,
                            Location = location,
                            Date = evt.StartTime.Date,
                            Event = evt,
                            Bounds = rowRect
                        };
                    }
                    y += rowHeight + rowSpacing;
                    rowIndex++;
                }
                y += rowSpacing;
            }
            return EmptyHit(location, args);
        }

        public DateTime? GetDateTimeFromLocation(Point location, ViewPaintArgs args)
        {
            if (args.Surface == null) return null;
            var surface = args.Surface;
            int offset = (location.Y - (surface.CalendarGridRect.Y + surface.SidebarPadding))
                / Math.Max(1, surface.ListRowHeight + surface.ListRowSpacing);
            return GetVisibleRangeStart(surface.CurrentDate).AddDays(offset);
        }

        private static void PaintAgendaRow(Graphics g, Rectangle rect, CalendarEvent evt, ViewPaintArgs args)
        {
            // W8 - delegate to developer's IBeepUIComponent when registered.
            var cellKey = $"evt:{evt.Id}";
            var ctx = new CalendarCellContext(
                CalendarCellKind.EventBlock,
                evt,
                evt.StartTime.Date,
                args.State?.ViewMode ?? CalendarViewMode.Week1,
                0, 0);
            if (CalendarPainterHelpers.TryDrawCellComponent(g, rect, cellKey, ctx, args)) return;

            Color fill = args.GetCategoryColor(evt.CategoryId);
            if (args.SelectedEvent?.Id == evt.Id) fill = args.SelectedBackColor;
            if (args.HoveredEventId == evt.Id) fill = args.HoverBackColor;
            CalendarPainterHelpers.FillRoundedRect(g, rect, args.Metrics.EventCornerRadius, fill);
            if (args.Metrics.ShowEventAccentStripe)
            {
                var accent = new Rectangle(rect.X, rect.Y, args.Metrics.EventAccentWidth, rect.Height);
                CalendarPainterHelpers.FillRoundedRect(g, accent, 0, fill);
            }
            int textLeft = rect.X + args.Metrics.EventAccentWidth + 8;
            int timeWidth = 110;
            CalendarPainterHelpers.DrawText(g, evt.StartTime.ToString("h:mm tt"),
                args.EventFont ?? args.DayFont, args.ForegroundColor,
                new Rectangle(textLeft, rect.Y, timeWidth, rect.Height),
                StringAlignment.Near, StringAlignment.Center);
            CalendarPainterHelpers.DrawText(g, evt.Title,
                args.EventFont ?? args.DayFont, args.ForegroundColor,
                new Rectangle(textLeft + timeWidth, rect.Y,
                    rect.Width - timeWidth - args.Metrics.EventAccentWidth - 12, rect.Height),
                StringAlignment.Near, StringAlignment.Center);
        }

        private static CalendarInteractionHitTestResult EmptyHit(Point location, ViewPaintArgs args) =>
            new CalendarInteractionHitTestResult
            {
                TargetKind = CalendarInteractionTargetKind.EmptySurface,
                RequestedMode = CalendarInteractionMode.CreateEvent,
                Location = location,
                Date = args.Surface?.CurrentDate.Date ?? DateTime.Today
            };
    }

    internal static class ViewPaintArgsAgendaHelpers
    {
        public static Color SurfaceHeaderBack(this ViewPaintArgs args)
        {
            return Color.FromArgb(20, args.PrimaryColor.R, args.PrimaryColor.G, args.PrimaryColor.B);
        }
    }
}
