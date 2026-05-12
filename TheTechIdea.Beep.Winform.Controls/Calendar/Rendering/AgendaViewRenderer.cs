using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    internal class AgendaViewRenderer : ICalendarViewRenderer
    {
        public void Draw(Graphics g, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            int padding = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.SidebarPadding, ctx.DensityScale);
            int headerHeight = CommonDrawing.ScaleMetric(24, ctx.DensityScale);
            int rowHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.ListRowHeight, ctx.DensityScale);
            int rowSpacing = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.ListRowSpacing, ctx.DensityScale);

            var groups = ctx.EventService.GetEventsForMonth(ctx.State.CurrentDate)
                .OrderBy(e => e.StartTime)
                .GroupBy(e => e.StartTime.Date)
                .OrderBy(g => g.Key)
                .ToList();

            int yPos = grid.Y + padding;
            foreach (var group in groups)
            {
                var headerRect = new Rectangle(grid.X + padding, yPos, grid.Width - (padding * 2), headerHeight);
                DrawDayHeader(g, ctx, group.Key, headerRect);
                yPos += headerHeight + rowSpacing;

                foreach (var evt in group)
                {
                    var rect = new Rectangle(grid.X + padding, yPos, grid.Width - (padding * 2), rowHeight);
                    bool isSelected = ctx.State.SelectedEvent?.Id == evt.Id;
                    CommonDrawing.DrawEventCard(g, ctx, evt, rect, isSelected, includeDescription: true, includeActions: true);
                    yPos += rowHeight + rowSpacing;
                    if (yPos > grid.Bottom)
                    {
                        return;
                    }
                }

                yPos += rowSpacing;
                if (yPos > grid.Bottom)
                {
                    return;
                }
            }
        }

        public void HandleClick(Point location, CalendarRenderContext ctx)
        {
            var grid = ctx.Rects.CalendarGridRect;
            int padding = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.SidebarPadding, ctx.DensityScale);
            int headerHeight = CommonDrawing.ScaleMetric(24, ctx.DensityScale);
            int rowHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.ListRowHeight, ctx.DensityScale);
            int rowSpacing = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.ListRowSpacing, ctx.DensityScale);

            var groups = ctx.EventService.GetEventsForMonth(ctx.State.CurrentDate)
                .OrderBy(e => e.StartTime)
                .GroupBy(e => e.StartTime.Date)
                .OrderBy(g => g.Key)
                .ToList();

            int yPos = grid.Y + padding;
            foreach (var group in groups)
            {
                var headerRect = new Rectangle(grid.X + padding, yPos, grid.Width - (padding * 2), headerHeight);
                if (headerRect.Contains(location))
                {
                    ctx.State.SelectedDate = group.Key;
                    ctx.State.CurrentDate = group.Key;
                    return;
                }

                yPos += headerHeight + rowSpacing;
                foreach (var evt in group)
                {
                    var rect = new Rectangle(grid.X + padding, yPos, grid.Width - (padding * 2), rowHeight);
                    if (rect.Contains(location))
                    {
                        ctx.State.SelectedEvent = evt;
                        ctx.State.SelectedDate = evt.StartTime.Date;
                        ctx.State.CurrentDate = evt.StartTime.Date;
                        return;
                    }

                    yPos += rowHeight + rowSpacing;
                    if (yPos > grid.Bottom)
                    {
                        return;
                    }
                }

                yPos += rowSpacing;
                if (yPos > grid.Bottom)
                {
                    return;
                }
            }
        }

        private static void DrawDayHeader(Graphics g, CalendarRenderContext ctx, DateTime date, Rectangle rect)
        {
            var backColor = ctx.Theme?.CardBackColor ?? Color.White;
            var borderColor = ctx.Theme?.CalendarBorderColor ?? Color.FromArgb(218, 220, 224);
            var titleColor = ctx.Theme?.CalendarForeColor ?? Color.Black;
            var accentColor = ctx.Theme?.PrimaryColor ?? Color.FromArgb(66, 133, 244);

            using (var backBrush = new SolidBrush(backColor))
            using (var borderPen = new Pen(borderColor))
            using (var titleBrush = new SolidBrush(titleColor))
            using (var accentBrush = new SolidBrush(accentColor))
            {
                g.FillRectangle(backBrush, rect);
                g.DrawRectangle(borderPen, rect);
                g.FillRectangle(accentBrush, new Rectangle(rect.X, rect.Y, 4, rect.Height));
                g.DrawString(date.ToString("dddd, MMMM dd"), ctx.DaysHeaderFont, titleBrush, new Rectangle(rect.X + 8, rect.Y, rect.Width - 16, rect.Height), new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                });
            }
        }
    }
}