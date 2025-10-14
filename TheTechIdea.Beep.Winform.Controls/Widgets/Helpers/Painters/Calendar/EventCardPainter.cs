using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Calendar
{
    /// <summary>
    /// EventCard - Event display card painter
    /// </summary>
    internal sealed class EventCardPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Event time and type
            ctx.IconRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                60, 24
            );
            
            // Event title
            ctx.HeaderRect = new Rectangle(
                ctx.IconRect.Right + 12,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - ctx.IconRect.Width - pad * 3,
                24
            );
            
            // Event details
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.IconRect.Bottom + 12,
                ctx.DrawingRect.Width - pad * 2,
                40
            );
            
            // Event actions/status
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.ContentRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - ctx.ContentRect.Bottom - pad * 2
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 6, layers: 2, offset: 1);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            var events = ctx.CustomData.ContainsKey("Events") ? 
                (List<CalendarEvent>)ctx.CustomData["Events"] : new List<CalendarEvent>();
            
            // Get next upcoming event or first event
            var upcomingEvent = events.FirstOrDefault(e => e.StartTime >= DateTime.Now) ?? events.FirstOrDefault();
            
            if (upcomingEvent == null)
            {
                DrawNoEventsMessage(g, ctx.ContentRect);
                return;
            }

            // Draw event time badge
            DrawEventTimeBadge(g, ctx.IconRect, upcomingEvent, ctx.AccentColor);
            
            // Draw event title
            using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.FromArgb(200, Color.Black));
            g.DrawString(upcomingEvent.Title, titleFont, titleBrush, ctx.HeaderRect);
            
            // Draw event details
            DrawEventDetails(g, ctx.ContentRect, upcomingEvent);
            
            // Draw event status/actions
            DrawEventFooter(g, ctx.FooterRect, upcomingEvent);
        }

        private void DrawEventTimeBadge(Graphics g, Rectangle rect, CalendarEvent eventItem, Color accentColor)
        {
            // Draw time badge background
            using var badgeBrush = new SolidBrush(Color.FromArgb(200, accentColor));
            using var badgePath = CreateRoundedPath(rect, 12);
            g.FillPath(badgeBrush, badgePath);
            
            // Draw time text
            using var timeFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Bold);
            using var timeBrush = new SolidBrush(Color.White);
            string timeText = eventItem.StartTime.ToString("HH:mm");
            var timeFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(timeText, timeFont, timeBrush, rect, timeFormat);
        }

        private void DrawEventDetails(Graphics g, Rectangle rect, CalendarEvent eventItem)
        {
            using var detailFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var detailBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
            
            int y = rect.Y;
            int lineHeight = 16;
            
            // Event type
            if (!string.IsNullOrEmpty(eventItem.Type))
            {
                g.DrawString($"Type: {eventItem.Type}", detailFont, detailBrush, rect.X, y);
                y += lineHeight;
            }
            
            // Event location
            if (!string.IsNullOrEmpty(eventItem.Location))
            {
                g.DrawString($"Location: {eventItem.Location}", detailFont, detailBrush, rect.X, y);
                y += lineHeight;
            }
            
            // Event description (truncated)
            if (!string.IsNullOrEmpty(eventItem.Description))
            {
                string description = eventItem.Description.Length > 50 ? 
                    eventItem.Description.Substring(0, 47) + "..." : eventItem.Description;
                g.DrawString(description, detailFont, detailBrush, rect.X, y);
            }
        }

        private void DrawEventFooter(Graphics g, Rectangle rect, CalendarEvent eventItem)
        {
            // Draw event duration
            using var footerFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var footerBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            
            var duration = eventItem.EndTime - eventItem.StartTime;
            string durationText = duration.TotalHours >= 1 ? 
                $"{duration.TotalHours:F1}h" : $"{duration.TotalMinutes:F0}m";
            
            g.DrawString($"Duration: {durationText}", footerFont, footerBrush, rect.X, rect.Y);
            
            // Draw status indicator
            DrawEventStatus(g, rect, eventItem);
        }

        private void DrawEventStatus(Graphics g, Rectangle rect, CalendarEvent eventItem)
        {
            string status;
            Color statusColor;
            
            if (eventItem.StartTime > DateTime.Now)
            {
                status = "Upcoming";
                statusColor = Color.FromArgb(255, 193, 7); // Amber
            }
            else if (eventItem.EndTime > DateTime.Now)
            {
                status = "In Progress";
                statusColor = Color.FromArgb(76, 175, 80); // Green
            }
            else
            {
                status = "Completed";
                statusColor = Color.FromArgb(156, 39, 176); // Purple
            }
            
            using var statusFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
            using var statusBrush = new SolidBrush(statusColor);
            var statusSize = TextUtils.MeasureText(g,status, statusFont);
            g.DrawString(status, statusFont, statusBrush, rect.Right - statusSize.Width, rect.Y);
        }

        private void DrawNoEventsMessage(Graphics g, Rectangle rect)
        {
            using var messageFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Italic);
            using var messageBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            var messageFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("No events scheduled", messageFont, messageBrush, rect, messageFormat);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw event type indicators
        }
    }
}