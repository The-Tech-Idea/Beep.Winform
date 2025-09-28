using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Globalization;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Calendar
{
    /// <summary>
    /// CalendarView - Full calendar month view painter
    /// </summary>
    internal sealed class CalendarViewPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 12;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Calendar header (month/year)
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                30
            );
            
            // Day names header
            ctx.IconRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                24
            );
            
            // Calendar grid
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
            var displayMonth = ctx.CustomData.ContainsKey("DisplayMonth") ? (DateTime)ctx.CustomData["DisplayMonth"] : DateTime.Now;
            var selectedDate = ctx.CustomData.ContainsKey("SelectedDate") ? (DateTime)ctx.CustomData["SelectedDate"] : DateTime.Now;
            var events = ctx.CustomData.ContainsKey("Events") ? (List<CalendarEvent>)ctx.CustomData["Events"] : new List<CalendarEvent>();
            var todayColor = ctx.CustomData.ContainsKey("TodayColor") ? (Color)ctx.CustomData["TodayColor"] : Color.Red;
            var selectedColor = ctx.CustomData.ContainsKey("SelectedColor") ? (Color)ctx.CustomData["SelectedColor"] : Color.Blue;
            var showEvents = ctx.CustomData.ContainsKey("ShowEvents") ? (bool)ctx.CustomData["ShowEvents"] : true;
            var showToday = ctx.CustomData.ContainsKey("ShowToday") ? (bool)ctx.CustomData["ShowToday"] : true;

            // Draw month/year header
            DrawCalendarHeader(g, ctx.HeaderRect, displayMonth, ctx.AccentColor);
            
            // Draw day names
            DrawDayNames(g, ctx.IconRect);
            
            // Draw calendar grid
            DrawCalendarGrid(g, ctx.ContentRect, displayMonth, selectedDate, events, todayColor, selectedColor, showEvents, showToday);
        }

        private void DrawCalendarHeader(Graphics g, Rectangle rect, DateTime month, Color accentColor)
        {
            using var headerFont = new Font(Owner.Font.FontFamily, 14f, FontStyle.Bold);
            using var headerBrush = new SolidBrush(Color.FromArgb(200, Color.Black));
            
            string monthYear = month.ToString("MMMM yyyy", CultureInfo.CurrentCulture);
            var headerFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(monthYear, headerFont, headerBrush, rect, headerFormat);
            
            // Draw navigation arrows
            DrawNavigationArrows(g, rect, accentColor);
        }

        private void DrawNavigationArrows(Graphics g, Rectangle rect, Color accentColor)
        {
            using var arrowBrush = new SolidBrush(accentColor);
            int arrowSize = 16;
            
            // Previous month arrow
            var prevRect = new Rectangle(rect.X + 10, rect.Y + (rect.Height - arrowSize) / 2, arrowSize, arrowSize);
            DrawArrow(g, prevRect, arrowBrush, true);
            
            // Next month arrow
            var nextRect = new Rectangle(rect.Right - 26, rect.Y + (rect.Height - arrowSize) / 2, arrowSize, arrowSize);
            DrawArrow(g, nextRect, arrowBrush, false);
        }

        private void DrawArrow(Graphics g, Rectangle rect, Brush brush, bool isLeft)
        {
            var points = new PointF[3];
            if (isLeft)
            {
                points[0] = new PointF(rect.Right - 4, rect.Y + 4);
                points[1] = new PointF(rect.X + 4, rect.Y + rect.Height / 2);
                points[2] = new PointF(rect.Right - 4, rect.Bottom - 4);
            }
            else
            {
                points[0] = new PointF(rect.X + 4, rect.Y + 4);
                points[1] = new PointF(rect.Right - 4, rect.Y + rect.Height / 2);
                points[2] = new PointF(rect.X + 4, rect.Bottom - 4);
            }
            g.FillPolygon(brush, points);
        }

        private void DrawDayNames(Graphics g, Rectangle rect)
        {
            using var dayFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Bold);
            using var dayBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
            
            string[] dayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            float cellWidth = rect.Width / 7f;
            
            for (int i = 0; i < 7; i++)
            {
                var dayRect = new RectangleF(rect.X + i * cellWidth, rect.Y, cellWidth, rect.Height);
                var dayFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(dayNames[i], dayFont, dayBrush, dayRect, dayFormat);
            }
        }

        private void DrawCalendarGrid(Graphics g, Rectangle rect, DateTime displayMonth, DateTime selectedDate, 
            List<CalendarEvent> events, Color todayColor, Color selectedColor, bool showEvents, bool showToday)
        {
            // Get first day of month and calculate grid
            var firstDay = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            var startDate = firstDay.AddDays(-(int)firstDay.DayOfWeek);
            
            float cellWidth = rect.Width / 7f;
            float cellHeight = rect.Height / 6f; // 6 weeks max
            
            using var dateFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
            using var dateBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
            using var otherMonthBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            using var todayBrush = new SolidBrush(todayColor);
            using var selectedBrush = new SolidBrush(selectedColor);
            
            for (int week = 0; week < 6; week++)
            {
                for (int day = 0; day < 7; day++)
                {
                    var currentDate = startDate.AddDays(week * 7 + day);
                    var cellRect = new RectangleF(
                        rect.X + day * cellWidth,
                        rect.Y + week * cellHeight,
                        cellWidth,
                        cellHeight
                    );
                    
                    // Draw cell background
                    DrawDateCell(g, cellRect, currentDate, displayMonth, selectedDate, 
                        todayColor, selectedColor, showToday);
                    
                    // Draw date number
                    bool isCurrentMonth = currentDate.Month == displayMonth.Month;
                    var brush = isCurrentMonth ? dateBrush : otherMonthBrush;
                    
                    var dateFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Near };
                    var textRect = new RectangleF(cellRect.X + 2, cellRect.Y + 2, cellRect.Width - 4, cellRect.Height - 4);
                    g.DrawString(currentDate.Day.ToString(), dateFont, brush, textRect, dateFormat);
                    
                    // Draw events if enabled
                    if (showEvents && isCurrentMonth)
                    {
                        DrawEventsInCell(g, cellRect, currentDate, events);
                    }
                }
            }
            
            // Draw grid lines
            DrawGridLines(g, rect, cellWidth, cellHeight);
        }

        private void DrawDateCell(Graphics g, RectangleF cellRect, DateTime date, DateTime displayMonth, 
            DateTime selectedDate, Color todayColor, Color selectedColor, bool showToday)
        {
            // Highlight today
            if (showToday && date.Date == DateTime.Today)
            {
                using var todayBrush = new SolidBrush(Color.FromArgb(50, todayColor));
                g.FillRectangle(todayBrush, cellRect);
            }
            
            // Highlight selected date
            if (date.Date == selectedDate.Date)
            {
                using var selectedBrush = new SolidBrush(Color.FromArgb(100, selectedColor));
                g.FillRectangle(selectedBrush, cellRect);
                
                // Draw selection border
                using var borderPen = new Pen(selectedColor, 2);
                g.DrawRectangle(borderPen, Rectangle.Round(cellRect));
            }
        }

        private void DrawEventsInCell(Graphics g, RectangleF cellRect, DateTime date, List<CalendarEvent> events)
        {
            var dayEvents = events.FindAll(e => e.StartTime.Date == date.Date);
            if (dayEvents.Count == 0) return;
            
            int maxDots = Math.Min(dayEvents.Count, 3);
            float dotSize = 6f;
            float startX = cellRect.X + 4;
            float startY = cellRect.Bottom - dotSize - 4;
            
            for (int i = 0; i < maxDots; i++)
            {
                using var eventBrush = new SolidBrush(dayEvents[i].Color);
                var dotRect = new RectangleF(startX + i * (dotSize + 2), startY, dotSize, dotSize);
                g.FillEllipse(eventBrush, dotRect);
            }
            
            // Show overflow indicator
            if (dayEvents.Count > 3)
            {
                using var overflowFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Bold);
                using var overflowBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
                var overflowRect = new RectangleF(startX + 3 * (dotSize + 2), startY - 2, 20, 10);
                g.DrawString($"+{dayEvents.Count - 3}", overflowFont, overflowBrush, overflowRect);
            }
        }

        private void DrawGridLines(Graphics g, Rectangle rect, float cellWidth, float cellHeight)
        {
            using var gridPen = new Pen(Color.FromArgb(30, Color.Black), 1);
            
            // Vertical lines
            for (int i = 1; i < 7; i++)
            {
                float x = rect.X + i * cellWidth;
                g.DrawLine(gridPen, x, rect.Y, x, rect.Bottom);
            }
            
            // Horizontal lines
            for (int i = 1; i < 6; i++)
            {
                float y = rect.Y + i * cellHeight;
                g.DrawLine(gridPen, rect.X, y, rect.Right, y);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw additional calendar indicators
        }
    }
}