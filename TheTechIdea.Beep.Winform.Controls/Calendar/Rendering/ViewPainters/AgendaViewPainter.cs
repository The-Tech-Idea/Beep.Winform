using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
                CalendarPainterHelpers.FillRoundedRect(g, headerRect,
                    args.Metrics.CornerRadius, args.SurfaceHeaderBack(args));
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

        private static void PaintAgendaRow(Graphics g, Rectangle rect, CalendarEvent evt, ViewPaintArgs args)
        {
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
