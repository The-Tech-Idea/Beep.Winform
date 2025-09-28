using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Calendar
{
    /// <summary>
    /// TimelineView - Timeline-based calendar painter
    /// Displays events in a chronological timeline format with time markers and event bars
    /// </summary>
    internal sealed class TimelineViewPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Timeline header with date range
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                32
            );
            
            // Time scale ruler
            ctx.IconRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                24
            );
            
            // Timeline content area with events
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.IconRect.Bottom + 4,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - ctx.IconRect.Bottom - pad - 4
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 8, layers: 3, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            var events = ctx.CustomData.ContainsKey("Events") ? 
                (List<CalendarEvent>)ctx.CustomData["Events"] : new List<CalendarEvent>();
            var displayMonth = ctx.CustomData.ContainsKey("DisplayMonth") ? 
                (DateTime)ctx.CustomData["DisplayMonth"] : DateTime.Now;
            var eventColor = ctx.CustomData.ContainsKey("EventColor") ? 
                (Color)ctx.CustomData["EventColor"] : Color.Blue;
            var showToday = ctx.CustomData.ContainsKey("ShowToday") ? 
                (bool)ctx.CustomData["ShowToday"] : true;
            var todayColor = ctx.CustomData.ContainsKey("TodayColor") ? 
                (Color)ctx.CustomData["TodayColor"] : Color.Red;

            // Draw timeline header
            DrawTimelineHeader(g, ctx.HeaderRect, ctx.Title, ctx.Value, displayMonth, ctx.AccentColor);
            
            // Draw time scale
            DrawTimeScale(g, ctx.IconRect, displayMonth, ctx.AccentColor);
            
            // Draw timeline with events
            DrawTimeline(g, ctx.ContentRect, events, displayMonth, eventColor, showToday, todayColor, ctx.AccentColor);
        }

        private void DrawTimelineHeader(Graphics g, Rectangle rect, string title, string subtitle, 
            DateTime displayMonth, Color accentColor)
        {
            // Draw title
            using var titleFont = new Font(Owner.Font.FontFamily, 12f, FontStyle.Bold);
            using var titleBrush = new SolidBrush(Color.FromArgb(200, Color.Black));
            g.DrawString(title, titleFont, titleBrush, rect.X, rect.Y);
            
            // Draw date range
            var startDate = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            
            using var rangeFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var rangeBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
            string rangeText = $"{startDate:MMM dd} - {endDate:MMM dd, yyyy}";
            g.DrawString(rangeText, rangeFont, rangeBrush, rect.X, rect.Y + 16);
            
            // Draw timeline type indicator
            using var typeBrush = new SolidBrush(accentColor);
            string typeText = "Timeline View";
            var typeSize = g.MeasureString(typeText, rangeFont);
            g.DrawString(typeText, rangeFont, typeBrush, rect.Right - typeSize.Width, rect.Y + 16);
        }

        private void DrawTimeScale(Graphics g, Rectangle rect, DateTime displayMonth, Color accentColor)
        {
            var startDate = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            int totalDays = (endDate - startDate).Days + 1;
            
            using var scaleFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var scaleBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            using var scalePen = new Pen(Color.FromArgb(200, 200, 200), 1);
            
            // Draw scale background
            using var scaleBgBrush = new SolidBrush(Color.FromArgb(248, 248, 248));
            g.FillRectangle(scaleBgBrush, rect);
            
            // Draw week markers
            float dayWidth = (float)rect.Width / totalDays;
            
            for (int day = 0; day < totalDays; day += 7) // Weekly markers
            {
                var currentDate = startDate.AddDays(day);
                float x = rect.X + day * dayWidth;
                
                // Draw week separator line
                g.DrawLine(scalePen, x, rect.Y, x, rect.Bottom);
                
                // Draw week label
                if (x + 40 < rect.Right) // Ensure text fits
                {
                    string weekText = currentDate.ToString("MMM dd");
                    g.DrawString(weekText, scaleFont, scaleBrush, x + 4, rect.Y + 4);
                }
            }
            
            // Draw today marker if visible
            if (DateTime.Today >= startDate && DateTime.Today <= endDate)
            {
                int todayOffset = (DateTime.Today - startDate).Days;
                float todayX = rect.X + todayOffset * dayWidth;
                
                using var todayPen = new Pen(Color.FromArgb(244, 67, 54), 2); // Red today line
                g.DrawLine(todayPen, todayX, rect.Y, todayX, rect.Bottom);
                
                // Draw "Today" label
                using var todayBrush = new SolidBrush(Color.FromArgb(244, 67, 54));
                g.DrawString("Today", scaleFont, todayBrush, todayX + 2, rect.Y + 4);
            }
        }

        private void DrawTimeline(Graphics g, Rectangle rect, List<CalendarEvent> events, DateTime displayMonth,
            Color eventColor, bool showToday, Color todayColor, Color accentColor)
        {
            if (rect.Height < 20) return;
            
            var startDate = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            int totalDays = (endDate - startDate).Days + 1;
            float dayWidth = (float)rect.Width / totalDays;
            
            // Filter events for this month
            var monthEvents = events.Where(e => e.StartTime.Date >= startDate && e.StartTime.Date <= endDate)
                                   .OrderBy(e => e.StartTime)
                                   .ToList();
            
            if (monthEvents.Count == 0)
            {
                DrawEmptyTimeline(g, rect, startDate, endDate, dayWidth, showToday, todayColor);
                return;
            }
            
            // Draw timeline lanes for events
            int maxLanes = Math.Min(monthEvents.Count, rect.Height / 40); // Max events that fit
            int laneHeight = rect.Height / Math.Max(maxLanes, 1);
            
            for (int i = 0; i < maxLanes; i++)
            {
                var eventItem = monthEvents[i];
                var laneRect = new Rectangle(rect.X, rect.Y + i * laneHeight, rect.Width, laneHeight - 4);
                
                DrawEventInTimeline(g, laneRect, eventItem, startDate, dayWidth, accentColor);
            }
            
            // Draw today indicator line across all lanes
            if (showToday && DateTime.Today >= startDate && DateTime.Today <= endDate)
            {
                int todayOffset = (DateTime.Today - startDate).Days;
                float todayX = rect.X + todayOffset * dayWidth;
                
                using var todayPen = new Pen(todayColor, 2);
                todayPen.DashStyle = DashStyle.Dash;
                g.DrawLine(todayPen, todayX, rect.Y, todayX, rect.Bottom);
            }
        }

        private void DrawEventInTimeline(Graphics g, Rectangle laneRect, CalendarEvent eventItem, 
            DateTime startDate, float dayWidth, Color accentColor)
        {
            // Calculate event position and width
            int eventStartOffset = (eventItem.StartTime.Date - startDate).Days;
            int eventDuration = Math.Max(1, (eventItem.EndTime.Date - eventItem.StartTime.Date).Days + 1);
            
            float eventX = laneRect.X + eventStartOffset * dayWidth;
            float eventWidth = Math.Min(eventDuration * dayWidth, laneRect.Right - eventX);
            
            var eventRect = new RectangleF(eventX, laneRect.Y + 4, eventWidth - 2, laneRect.Height - 8);
            
            if (eventRect.Width < 4) return; // Too small to draw
            
            // Use event color or default
            Color barColor = eventItem.Color != Color.Empty ? eventItem.Color : accentColor;
            
            // Draw event background bar
            using var eventBrush = new SolidBrush(Color.FromArgb(150, barColor));
            using var eventPath = CreateRoundedPath(Rectangle.Round(eventRect), 4);
            g.FillPath(eventBrush, eventPath);
            
            // Draw event border
            using var borderPen = new Pen(barColor, 1);
            g.DrawPath(borderPen, eventPath);
            
            // Draw event title if space allows
            if (eventRect.Width > 50)
            {
                using var eventFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
                using var textBrush = new SolidBrush(Color.White);
                
                var textRect = new RectangleF(eventRect.X + 4, eventRect.Y + 2, eventRect.Width - 8, eventRect.Height - 4);
                var textFormat = new StringFormat { 
                    Alignment = StringAlignment.Near, 
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                
                g.DrawString(eventItem.Title, eventFont, textBrush, textRect, textFormat);
            }
            
            // Draw event time indicators
            DrawEventTimeIndicators(g, eventRect, eventItem, barColor);
        }

        private void DrawEventTimeIndicators(Graphics g, RectangleF eventRect, CalendarEvent eventItem, Color barColor)
        {
            // Draw start time indicator
            using var timeBrush = new SolidBrush(Color.FromArgb(200, barColor));
            var startIndicator = new RectangleF(eventRect.X, eventRect.Y - 2, 3, eventRect.Height + 4);
            g.FillRectangle(timeBrush, startIndicator);
            
            // Draw end time indicator if event has duration
            if (eventItem.EndTime > eventItem.StartTime.AddHours(1))
            {
                var endIndicator = new RectangleF(eventRect.Right - 3, eventRect.Y - 2, 3, eventRect.Height + 4);
                g.FillRectangle(timeBrush, endIndicator);
            }
            
            // Draw duration indicator dots for multi-day events
            var duration = eventItem.EndTime - eventItem.StartTime;
            if (duration.TotalDays > 1)
            {
                int dotCount = Math.Min((int)duration.TotalDays, (int)(eventRect.Width / 8));
                for (int i = 0; i < dotCount; i++)
                {
                    float dotX = eventRect.X + 8 + i * 8;
                    if (dotX + 4 < eventRect.Right)
                    {
                        var dotRect = new RectangleF(dotX, eventRect.Y + eventRect.Height - 6, 2, 2);
                        g.FillEllipse(timeBrush, dotRect);
                    }
                }
            }
        }

        private void DrawEmptyTimeline(Graphics g, Rectangle rect, DateTime startDate, DateTime endDate, 
            float dayWidth, bool showToday, Color todayColor)
        {
            // Draw empty timeline message
            using var emptyFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Italic);
            using var emptyBrush = new SolidBrush(Color.FromArgb(120, Color.Gray));
            var emptyFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            
            string emptyText = $"No events scheduled for {startDate:MMMM yyyy}";
            g.DrawString(emptyText, emptyFont, emptyBrush, rect, emptyFormat);
            
            // Still draw today indicator
            if (showToday && DateTime.Today >= startDate && DateTime.Today <= endDate)
            {
                int todayOffset = (DateTime.Today - startDate).Days;
                float todayX = rect.X + todayOffset * dayWidth;
                
                using var todayPen = new Pen(todayColor, 2);
                todayPen.DashStyle = DashStyle.Dash;
                g.DrawLine(todayPen, todayX, rect.Y, todayX, rect.Bottom);
                
                // Draw today label
                using var todayFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Bold);
                using var todayBrush = new SolidBrush(todayColor);
                g.DrawString("Today", todayFont, todayBrush, todayX + 4, rect.Y + 10);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw timeline legend or additional indicators
            var legendRect = new Rectangle(ctx.DrawingRect.Right - 80, ctx.DrawingRect.Top + 8, 70, 60);
            
            using var legendFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Regular);
            using var legendBrush = new SolidBrush(Color.FromArgb(100, Color.Black));
            
            // Draw mini legend
            g.DrawString("Timeline", legendFont, legendBrush, legendRect.X, legendRect.Y);
            g.DrawString("Legend:", legendFont, legendBrush, legendRect.X, legendRect.Y + 10);
            
            // Event indicator
            using var eventBrush = new SolidBrush(Color.FromArgb(150, ctx.AccentColor));
            g.FillRectangle(eventBrush, legendRect.X, legendRect.Y + 20, 12, 6);
            g.DrawString("Event", legendFont, legendBrush, legendRect.X + 16, legendRect.Y + 18);
            
            // Today indicator
            using var todayPen = new Pen(Color.FromArgb(244, 67, 54), 2);
            g.DrawLine(todayPen, legendRect.X, legendRect.Y + 32, legendRect.X + 12, legendRect.Y + 32);
            g.DrawString("Today", legendFont, legendBrush, legendRect.X + 16, legendRect.Y + 28);
        }
    }
}