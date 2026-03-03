using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    internal class ListViewRenderer : ICalendarViewRenderer
    {
        public void Draw(Graphics g, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            var events = ctx.EventService.GetEventsForMonth(ctx.State.CurrentDate).OrderBy(e => e.StartTime).ToList();
            int padding = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.SidebarPadding, ctx.DensityScale);
            int rowHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.ListRowHeight, ctx.DensityScale);
            int rowSpacing = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.ListRowSpacing, ctx.DensityScale);
            int yPos = grid.Y + padding;
            foreach (var evt in events)
            {
                var rect = new Rectangle(
                    grid.X + padding,
                    yPos,
                    grid.Width - (padding * 2),
                    rowHeight);
                bool isSelected = ctx.State.SelectedEvent?.Id == evt.Id;
                DrawListEvent(g, ctx, evt, rect, isSelected);
                yPos += rowHeight + rowSpacing;
                if (yPos > grid.Bottom)
                {
                    break;
                }
            }
        }

        public void HandleClick(Point location, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            var events = ctx.EventService.GetEventsForMonth(ctx.State.CurrentDate).OrderBy(e => e.StartTime).ToList();
            int padding = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.SidebarPadding, ctx.DensityScale);
            int rowHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.ListRowHeight, ctx.DensityScale);
            int rowSpacing = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.ListRowSpacing, ctx.DensityScale);
            int yPos = grid.Y + padding;
            foreach (var evt in events)
            {
                var rect = new Rectangle(
                    grid.X + padding,
                    yPos,
                    grid.Width - (padding * 2),
                    rowHeight);
                if (rect.Contains(location))
                {
                    ctx.State.SelectedEvent = evt;
                    return;
                }
                yPos += rowHeight + rowSpacing;
            }
        }

        private void DrawListEvent(Graphics g, CalendarRenderContext ctx, CalendarEvent evt, Rectangle rect, bool isSelected)
        {
            CommonDrawing.DrawEventCard(g, ctx, evt, rect, isSelected, includeDescription: true, includeActions: true);
        }
    }
}
