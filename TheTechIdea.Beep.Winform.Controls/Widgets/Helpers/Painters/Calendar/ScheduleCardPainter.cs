using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Calendar
{
    /// <summary>
    /// ScheduleCard - Schedule/appointment display painter
    /// </summary>
    internal sealed class ScheduleCardPainter : WidgetPainterBase
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
            // Draw schedule card with time and event details
            DrawScheduleHeader(g, ctx);
            DrawScheduleEvents(g, ctx);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) 
        {
            // Draw status indicators and priority markers
            DrawStatusIndicators(g, ctx);
        }

        private void DrawScheduleHeader(Graphics g, WidgetContext ctx)
        {
            // Draw date header
            var today = DateTime.Today;
            var headerRect = new Rectangle(
                ctx.DrawingRect.Left + 12,
                ctx.DrawingRect.Top + 12,
                ctx.DrawingRect.Width - 24,
                30
            );

            using var dateFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
            using var dayFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var dateBrush = new SolidBrush(Theme?.TextBoxForeColor ?? Color.Black);
            using var dayBrush = new SolidBrush(Color.FromArgb(120, Color.Black));

            // Draw main date
            var dateText = today.ToString("MMM dd");
            var dateSize = g.MeasureString(dateText, dateFont);
            g.DrawString(dateText, dateFont, dateBrush, headerRect.Left, headerRect.Top);

            // Draw day of week
            var dayText = today.ToString("dddd");
            g.DrawString(dayText, dayFont, dayBrush, headerRect.Left, headerRect.Top + dateSize.Height);
        }

        private void DrawScheduleEvents(Graphics g, WidgetContext ctx)
        {
            // Sample schedule events
            var events = new[]
            {
                new { Time = "09:00", Title = "Daily Standup", Type = "meeting", Priority = "high" },
                new { Time = "10:30", Title = "Code Review", Type = "review", Priority = "medium" },
                new { Time = "14:00", Title = "Client Demo", Type = "demo", Priority = "high" },
                new { Time = "16:00", Title = "Team Sync", Type = "meeting", Priority = "low" }
            };

            int eventHeight = 35;
            int startY = ctx.DrawingRect.Top + 55;
            int padding = 12;

            using var timeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
            using var titleFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var timeBrush = new SolidBrush(Theme?.AccentColor ?? Color.Blue);
            using var titleBrush = new SolidBrush(Theme?.TextBoxForeColor ?? Color.Black);

            for (int i = 0; i < events.Length && startY + eventHeight <= ctx.DrawingRect.Bottom - padding; i++)
            {
                var evt = events[i];
                var eventRect = new Rectangle(
                    ctx.DrawingRect.Left + padding,
                    startY + i * (eventHeight + 8),
                    ctx.DrawingRect.Width - padding * 2,
                    eventHeight
                );

                // Draw time
                var timeRect = new Rectangle(eventRect.Left, eventRect.Top, 50, eventHeight);
                g.DrawString(evt.Time, timeFont, timeBrush, timeRect,
                           new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                // Draw event title
                var titleRect = new Rectangle(eventRect.Left + 60, eventRect.Top + 5, eventRect.Width - 80, 18);
                g.DrawString(evt.Title, titleFont, titleBrush, titleRect);

                // Draw event type
                var typeRect = new Rectangle(eventRect.Left + 60, eventRect.Top + 20, eventRect.Width - 80, 12);
                using var typeFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Italic);
                using var typeBrush = new SolidBrush(Color.FromArgb(100, Color.Black));
                g.DrawString(evt.Type, typeFont, typeBrush, typeRect);

                // Draw priority indicator
                var priorityColor = evt.Priority switch
                {
                    "high" => Color.FromArgb(244, 67, 54),
                    "medium" => Color.FromArgb(255, 193, 7),
                    _ => Color.FromArgb(76, 175, 80)
                };

                var priorityRect = new Rectangle(eventRect.Right - 8, eventRect.Top + 8, 4, eventHeight - 16);
                using var priorityBrush = new SolidBrush(priorityColor);
                g.FillRectangle(priorityBrush, priorityRect);
            }
        }

        private void DrawStatusIndicators(Graphics g, WidgetContext ctx)
        {
            // Draw overall schedule status
            var statusRect = new Rectangle(ctx.DrawingRect.Right - 40, ctx.DrawingRect.Top + 12, 30, 20);
            
            using var statusFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
            using var statusBrush = new SolidBrush(Color.FromArgb(76, 175, 80));
            
            g.DrawString("4/4", statusFont, statusBrush, statusRect,
                       new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            // Draw completion indicator
            using var completionBrush = new SolidBrush(Color.FromArgb(40, 76, 175, 80));
            var completionRect = new Rectangle(statusRect.Left - 2, statusRect.Bottom + 2, statusRect.Width + 4, 2);
            g.FillRectangle(completionBrush, completionRect);
        }
    }
}