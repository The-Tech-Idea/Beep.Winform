using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Calendar
{
    /// <summary>
    /// EventList - List of upcoming events painter
    /// </summary>
    internal sealed class EventListPainter : WidgetPainterBase
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
            // Draw list of upcoming events
            DrawEventList(g, ctx);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) 
        {
            // Draw separators and time indicators
            DrawEventSeparators(g, ctx);
        }

        private void DrawEventList(Graphics g, WidgetContext ctx)
        {
            // Sample event data
            var events = new[]
            {
                new { Title = "Team Meeting", Time = "9:00 AM", Duration = "1h", Color = Color.FromArgb(76, 175, 80) },
                new { Title = "Project Review", Time = "2:00 PM", Duration = "2h", Color = Color.FromArgb(33, 150, 243) },
                new { Title = "Client Call", Time = "4:30 PM", Duration = "30m", Color = Color.FromArgb(255, 193, 7) },
                new { Title = "Code Review", Time = "5:00 PM", Duration = "1h", Color = Color.FromArgb(156, 39, 176) }
            };

            int itemHeight = 40;
            int padding = 8;
            int startY = ctx.DrawingRect.Top + padding;

            using var titleFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Bold);
            using var timeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var titleBrush = new SolidBrush(Theme?.TextBoxForeColor ?? Color.Black);
            using var timeBrush = new SolidBrush(Color.FromArgb(120, Color.Black));

            for (int i = 0; i < events.Length && startY + itemHeight <= ctx.DrawingRect.Bottom - padding; i++)
            {
                var eventItem = events[i];
                var itemRect = new Rectangle(
                    ctx.DrawingRect.Left + padding,
                    startY + i * (itemHeight + 4),
                    ctx.DrawingRect.Width - padding * 2,
                    itemHeight
                );

                // Draw event color indicator
                var colorRect = new Rectangle(itemRect.Left, itemRect.Top + 8, 4, itemHeight - 16);
                using var colorBrush = new SolidBrush(eventItem.Color);
                g.FillRectangle(colorBrush, colorRect);

                // Draw event title
                var titleRect = new Rectangle(itemRect.Left + 12, itemRect.Top + 4, itemRect.Width - 80, 16);
                g.DrawString(eventItem.Title, titleFont, titleBrush, titleRect);

                // Draw event time and duration
                var timeRect = new Rectangle(itemRect.Left + 12, itemRect.Bottom - 16, itemRect.Width - 80, 12);
                g.DrawString($"{eventItem.Time} ({eventItem.Duration})", timeFont, timeBrush, timeRect);

                // Draw time on the right
                var rightTimeRect = new Rectangle(itemRect.Right - 70, itemRect.Top + 8, 65, itemHeight - 16);
                g.DrawString(eventItem.Time, timeFont, timeBrush, rightTimeRect,
                           new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });
            }
        }

        private void DrawEventSeparators(Graphics g, WidgetContext ctx)
        {
            // Draw subtle separators between events
            using var separatorPen = new Pen(Color.FromArgb(40, Color.Gray), 1);
            
            int itemHeight = 40;
            int padding = 8;
            int items = Math.Min(4, (ctx.DrawingRect.Height - padding * 2) / (itemHeight + 4));

            for (int i = 1; i < items; i++)
            {
                int y = ctx.DrawingRect.Top + padding + i * (itemHeight + 4) - 2;
                g.DrawLine(separatorPen,
                          ctx.DrawingRect.Left + padding + 12,
                          y,
                          ctx.DrawingRect.Right - padding,
                          y);
            }
        }
    }
}