using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Calendar
{
    /// <summary>
    /// WeekView - Weekly calendar display painter
    /// </summary>
    internal sealed class WeekViewPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw week view calendar layout
            DrawWeekHeader(g, ctx);
            DrawWeekGrid(g, ctx);
            DrawWeekEvents(g, ctx);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) 
        {
            // Draw current day highlight and grid lines
            DrawCurrentDayHighlight(g, ctx);
            DrawWeekGridLines(g, ctx);
        }

        private void DrawWeekHeader(Graphics g, WidgetContext ctx)
        {
            // Draw day headers for the week
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            
            int dayWidth = ctx.DrawingRect.Width / 7;
            int headerHeight = 30;

            using var dayFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
            using var dateFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Regular);
            using var dayBrush = new SolidBrush(Theme?.TextBoxForeColor ?? Color.Black);
            using var dateBrush = new SolidBrush(Color.FromArgb(120, Color.Black));

            for (int i = 0; i < 7; i++)
            {
                var currentDay = startOfWeek.AddDays(i);
                var dayRect = new Rectangle(
                    ctx.DrawingRect.Left + i * dayWidth,
                    ctx.DrawingRect.Top,
                    dayWidth,
                    headerHeight
                );

                // Highlight today
                if (currentDay.Date == today.Date)
                {
                    using var todayBrush = new SolidBrush(Color.FromArgb(40, Theme?.AccentColor ?? Color.Blue));
                    g.FillRectangle(todayBrush, dayRect);
                }

                // Draw day name
                var dayName = currentDay.ToString("ddd");
                var dayNameRect = new Rectangle(dayRect.X, dayRect.Y + 2, dayRect.Width, 14);
                g.DrawString(dayName, dayFont, dayBrush, dayNameRect,
                           new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                // Draw date
                var dateText = currentDay.Day.ToString();
                var dateRect = new Rectangle(dayRect.X, dayRect.Y + 16, dayRect.Width, 12);
                g.DrawString(dateText, dateFont, dateBrush, dateRect,
                           new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }

        private void DrawWeekGrid(Graphics g, WidgetContext ctx)
        {
            // Draw time slots on the left side
            int timeSlotWidth = 60;
            int hourHeight = 40;
            int startHour = 8;
            int endHour = 18;

            var timeAreaRect = new Rectangle(
                ctx.DrawingRect.Left,
                ctx.DrawingRect.Top + 30,
                timeSlotWidth,
                ctx.DrawingRect.Height - 30
            );

            using var timeFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Regular);
            using var timeBrush = new SolidBrush(Color.FromArgb(100, Color.Black));

            for (int hour = startHour; hour <= endHour; hour++)
            {
                var timeSlotRect = new Rectangle(
                    timeAreaRect.Left,
                    timeAreaRect.Top + (hour - startHour) * hourHeight,
                    timeAreaRect.Width,
                    hourHeight
                );

                var timeText = $"{hour}:00";
                g.DrawString(timeText, timeFont, timeBrush, timeSlotRect,
                           new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Top });
            }
        }

        private void DrawWeekEvents(Graphics g, WidgetContext ctx)
        {
            // Sample events throughout the week
            var events = new[]
            {
                new { Day = 1, StartHour = 9, Duration = 1, Title = "Meeting", Color = Color.FromArgb(76, 175, 80) },
                new { Day = 2, StartHour = 14, Duration = 2, Title = "Project Review", Color = Color.FromArgb(33, 150, 243) },
                new { Day = 3, StartHour = 10, Duration = 1, Title = "Client Call", Color = Color.FromArgb(255, 193, 7) },
                new { Day = 5, StartHour = 16, Duration = 1, Title = "Team Sync", Color = Color.FromArgb(156, 39, 176) }
            };

            int timeSlotWidth = 60;
            int dayWidth = (ctx.DrawingRect.Width - timeSlotWidth) / 7;
            int hourHeight = 40;
            int startHour = 8;

            using var eventFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Bold);
            using var eventTextBrush = new SolidBrush(Color.White);

            foreach (var evt in events)
            {
                var eventRect = new Rectangle(
                    ctx.DrawingRect.Left + timeSlotWidth + evt.Day * dayWidth + 2,
                    ctx.DrawingRect.Top + 30 + (evt.StartHour - startHour) * hourHeight + 2,
                    dayWidth - 4,
                    evt.Duration * hourHeight - 4
                );

                // Draw event background
                using var eventBrush = new SolidBrush(evt.Color);
                using var eventPath = CreateRoundedPath(eventRect, 4);
                g.FillPath(eventBrush, eventPath);

                // Draw event title
                if (eventRect.Height > 16)
                {
                    var textRect = Rectangle.Inflate(eventRect, -4, -2);
                    g.DrawString(evt.Title, eventFont, eventTextBrush, textRect);
                }
            }
        }

        private void DrawCurrentDayHighlight(Graphics g, WidgetContext ctx)
        {
            // Highlight current day column
            var today = DateTime.Today;
            var dayOfWeek = (int)today.DayOfWeek;
            
            int timeSlotWidth = 60;
            int dayWidth = (ctx.DrawingRect.Width - timeSlotWidth) / 7;

            var highlightRect = new Rectangle(
                ctx.DrawingRect.Left + timeSlotWidth + dayOfWeek * dayWidth,
                ctx.DrawingRect.Top + 30,
                dayWidth,
                ctx.DrawingRect.Height - 30
            );

            using var highlightBrush = new SolidBrush(Color.FromArgb(10, Theme?.AccentColor ?? Color.Blue));
            g.FillRectangle(highlightBrush, highlightRect);
        }

        private void DrawWeekGridLines(Graphics g, WidgetContext ctx)
        {
            using var gridPen = new Pen(Color.FromArgb(40, Color.LightGray), 1);
            
            int timeSlotWidth = 60;
            int dayWidth = (ctx.DrawingRect.Width - timeSlotWidth) / 7;
            int hourHeight = 40;
            int startHour = 8;
            int endHour = 18;

            // Vertical lines for days
            for (int i = 0; i <= 7; i++)
            {
                int x = ctx.DrawingRect.Left + timeSlotWidth + i * dayWidth;
                g.DrawLine(gridPen, x, ctx.DrawingRect.Top + 30, x, ctx.DrawingRect.Bottom);
            }

            // Horizontal lines for hours
            for (int hour = startHour; hour <= endHour; hour++)
            {
                int y = ctx.DrawingRect.Top + 30 + (hour - startHour) * hourHeight;
                g.DrawLine(gridPen, ctx.DrawingRect.Left + timeSlotWidth, y, ctx.DrawingRect.Right, y);
            }
        }
    }
}