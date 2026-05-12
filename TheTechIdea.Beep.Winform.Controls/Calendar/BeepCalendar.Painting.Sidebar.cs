using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void DrawSidebarWithPainter(Graphics g, CalendarPainterContext ctx)
        {
            var rect = _rects.SidebarRect;
            int padding = ScaleMetric(CalendarLayoutMetrics.SidebarPadding);
            int cardHeight = ScaleMetric(CalendarLayoutMetrics.SidebarCardHeight);
            int cardGap = ScaleMetric(CalendarLayoutMetrics.SidebarCardGap);
            _stylePainter.PaintSidebar(g, rect, ctx);

            // Mini calendar
            var miniRect = new Rectangle(rect.X + padding, rect.Y + padding, Math.Max(10, rect.Width - (padding * 2)), cardHeight);
            if (miniRect.Width > 20)
            {
                _stylePainter.PaintMiniCalendar(g, miniRect, _state.CurrentDate, _state.SelectedDate, ctx);
            }

            // Event details / empty state
            var detailsRect = new Rectangle(
                rect.X + padding,
                rect.Y + padding + cardHeight + cardGap,
                Math.Max(10, rect.Width - (padding * 2)),
                cardHeight);
            if (detailsRect.Width > 20)
            {
                if (_state.SelectedEvent != null)
                {
                    _stylePainter.PaintEventDetails(g, detailsRect, _state.SelectedEvent, ctx);
                }
                else
                {
                    using (var backBrush = new SolidBrush(ctx.BackgroundColor))
                    using (var borderPen = new Pen(ctx.BorderColor))
                    using (var titleBrush = new SolidBrush(ctx.ForegroundColor))
                    using (var bodyBrush = new SolidBrush(Color.FromArgb(170, ctx.ForegroundColor)))
                    {
                        g.FillRectangle(backBrush, detailsRect);
                        g.DrawRectangle(borderPen, detailsRect);
                        g.DrawString("No event selected", DayFont, titleBrush, new Rectangle(detailsRect.X + 10, detailsRect.Y + 14, detailsRect.Width - 20, 24));
                        g.DrawString("Select a date/event or use + Create Event.", EventFont, bodyBrush, new Rectangle(detailsRect.X + 10, detailsRect.Y + 40, detailsRect.Width - 20, 40));
                    }
                }
            }
        }
    }
}