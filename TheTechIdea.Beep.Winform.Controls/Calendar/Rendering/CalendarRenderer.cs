using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    internal class CalendarRenderer
    {
        private readonly ICalendarViewRenderer _month = new MonthViewRenderer();
        private readonly ICalendarViewRenderer _week = new WeekViewRenderer();
        private readonly ICalendarViewRenderer _day = new DayViewRenderer();
        private readonly ICalendarViewRenderer _list = new ListViewRenderer();

        public void Draw(Graphics g, CalendarRenderContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Header and selector backgrounds
            CommonDrawing.DrawHeader(g, ctx, GetHeaderText(ctx));
            CommonDrawing.DrawViewSelectorBackground(g, ctx);

            // Main view
            var renderer = GetRenderer(ctx.State.ViewMode);
            renderer.Draw(g, ctx);

            // Sidebar - only draw if enabled AND there's actual space for it
            if (ctx.State.ShowSidebar && ctx.Rects.SidebarRect.Width > 0)
            {
                DrawSidebar(g, ctx);
            }
        }

        public void HandleClick(Point location, CalendarRenderContext ctx)
        {
            var renderer = GetRenderer(ctx.State.ViewMode);
            renderer.HandleClick(location, ctx);
        }

        private ICalendarViewRenderer GetRenderer(CalendarViewMode mode)
            => mode switch
            {
                CalendarViewMode.Month => _month,
                CalendarViewMode.Week => _week,
                CalendarViewMode.Day => _day,
                CalendarViewMode.List => _list,
                _ => _month
            };

        private string GetHeaderText(CalendarRenderContext ctx)
        {
            var d = ctx.State.CurrentDate;
            return ctx.State.ViewMode switch
            {
                CalendarViewMode.Month => d.ToString("MMMM yyyy"),
                CalendarViewMode.Week => $"Week of {d.AddDays(-(int)d.DayOfWeek):MMMM dd, yyyy}",
                CalendarViewMode.Day => d.ToString("dddd, MMMM dd, yyyy"),
                CalendarViewMode.List => d.ToString("MMMM yyyy") + " Events",
                _ => d.ToString("MMMM yyyy")
            };
        }

        private void DrawSidebar(Graphics g, CalendarRenderContext ctx)
        {
            var rect = ctx.Rects.SidebarRect;
            int sidebarPadding = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.SidebarPadding, ctx.DensityScale);
            int cardHeight = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.SidebarCardHeight, ctx.DensityScale);
            int cardGap = CommonDrawing.ScaleMetric(CalendarLayoutMetrics.SidebarCardGap, ctx.DensityScale);
            
            // Draw sidebar background
            using (var brush = new SolidBrush(ctx.Theme?.CalendarBackColor ?? Color.FromArgb(248, 249, 250)))
                g.FillRectangle(brush, rect);
            
            // Draw left border to separate from main calendar
            using (var pen = new Pen(ctx.Theme?.CalendarBorderColor ?? Color.FromArgb(218, 220, 224)))
                g.DrawLine(pen, rect.X, rect.Y, rect.X, rect.Bottom);

            // Mini calendar placeholder
            var miniRect = new Rectangle(
                rect.X + sidebarPadding,
                rect.Y + sidebarPadding,
                Math.Max(10, rect.Width - (sidebarPadding * 2)),
                cardHeight);
            if (miniRect.Width > 20) // only draw if there's enough space
            {
                CommonDrawing.DrawMiniCalendarCard(g, ctx, miniRect, ctx.State.CurrentDate, ctx.State.SelectedDate);
            }

            var detailsRect = new Rectangle(
                rect.X + sidebarPadding,
                rect.Y + sidebarPadding + cardHeight + cardGap,
                Math.Max(10, rect.Width - (sidebarPadding * 2)),
                cardHeight);
            if (detailsRect.Width > 20) // only draw if there's enough space
            {
                CommonDrawing.DrawEventInsightsCard(g, ctx, detailsRect, ctx.State.SelectedEvent);
            }
        }
    }
}
