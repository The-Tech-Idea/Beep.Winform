using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.StylePainters
{
    /// <summary>
    /// Material Design 3 style calendar painter
    /// Features: Elevated cards, rounded corners, state layers, dynamic color
    /// </summary>
    public class MaterialCalendarPainter : ICalendarStylePainter
    {
        private readonly CalendarStyleMetrics _metrics;
        private BeepControlStyle _controlStyle = BeepControlStyle.Material3;

        public string StyleName => "Material";
        public string DisplayName => "Material Design 3";

        public MaterialCalendarPainter()
        {
            _metrics = new CalendarStyleMetrics
            {
                HeaderHeight = 64,
                ViewSelectorHeight = 48,
                DayHeaderHeight = 36,
                TimeSlotHeight = 60,
                EventBarHeight = 20,
                EventSpacing = 2,
                CornerRadius = 12,
                CellPadding = 4,
                SidebarWidth = 320,
                TimeColumnWidth = 64,
                MaxEventsPerCell = 3
            };
        }

        public CalendarStyleMetrics GetMetrics() => _metrics;

        #region Background and Chrome

        public void PaintBackground(Graphics g, Rectangle bounds, CalendarPainterContext ctx)
        {
            using (var brush = new SolidBrush(ctx.BackgroundColor))
            {
                g.FillRectangle(brush, bounds);
            }
        }

        public void PaintHeader(Graphics g, Rectangle bounds, string headerText, CalendarPainterContext ctx)
        {
            // Material surface with subtle elevation
            using (var brush = new SolidBrush(ctx.BackgroundColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Header text with Material typography
            using (var brush = new SolidBrush(ctx.ForegroundColor))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(headerText, ctx.HeaderFont, brush, bounds, format);
            }

            // Bottom divider
            using (var pen = new Pen(Color.FromArgb(30, ctx.BorderColor), 1))
            {
                g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }
        }

        public void PaintViewSelector(Graphics g, Rectangle bounds, CalendarPainterContext ctx)
        {
            // Surface container
            using (var brush = new SolidBrush(Color.FromArgb(250, ctx.BackgroundColor)))
            {
                g.FillRectangle(brush, bounds);
            }

            // Bottom border
            using (var pen = new Pen(Color.FromArgb(40, ctx.BorderColor), 1))
            {
                g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }
        }

        public void PaintSidebar(Graphics g, Rectangle bounds, CalendarPainterContext ctx)
        {
            // Elevated surface
            using (var brush = new SolidBrush(ctx.BackgroundColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Left border with subtle shadow
            using (var pen = new Pen(Color.FromArgb(20, 0, 0, 0), 1))
            {
                g.DrawLine(pen, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
            }
        }

        #endregion

        #region Day Cells

        public void PaintDayCell(Graphics g, Rectangle bounds, DateTime date, DayCellState state, CalendarPainterContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Determine background color
            Color bgColor;
            if (state.IsSelected)
            {
                bgColor = ctx.SelectedBackColor;
            }
            else if (state.IsHovered)
            {
                bgColor = ctx.HoverBackColor;
            }
            else if (state.IsToday)
            {
                bgColor = ctx.TodayBackColor;
            }
            else if (!state.IsCurrentMonth)
            {
                bgColor = ctx.OutOfMonthBackColor;
            }
            else if (state.IsWeekend)
            {
                bgColor = ctx.WeekendBackColor;
            }
            else
            {
                bgColor = ctx.BackgroundColor;
            }

            // Cell background with rounded corners for selected/today
            if (state.IsSelected || state.IsToday)
            {
                using (var path = CreateRoundedRect(bounds, 8))
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }
            }
            else
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, bounds);
                }
            }

            // Cell border
            using (var pen = new Pen(Color.FromArgb(20, ctx.BorderColor), 1))
            {
                g.DrawRectangle(pen, bounds);
            }

            // Day number
            Color textColor = state.IsSelected ? ctx.SelectedForeColor :
                             state.IsToday ? ctx.TodayForeColor :
                             !state.IsCurrentMonth ? ctx.OutOfMonthForeColor :
                             ctx.ForegroundColor;

            // Material Design: Day number in a circle for today/selected
            var dayRect = new Rectangle(bounds.X + 4, bounds.Y + 4, 28, 28);
            if (state.IsToday && !state.IsSelected)
            {
                // Today indicator circle
                using (var brush = new SolidBrush(ctx.PrimaryColor))
                {
                    g.FillEllipse(brush, dayRect);
                }
                textColor = Color.White;
            }

            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat
                {
                    Alignment = state.IsToday ? StringAlignment.Center : StringAlignment.Near,
                    LineAlignment = state.IsToday ? StringAlignment.Center : StringAlignment.Near
                };
                var textRect = state.IsToday ? dayRect : new Rectangle(bounds.X + 8, bounds.Y + 6, bounds.Width - 12, 20);
                g.DrawString(date.Day.ToString(), ctx.DayFont, brush, textRect, format);
            }
        }

        public void PaintDayHeader(Graphics g, Rectangle bounds, string dayName, bool isToday, CalendarPainterContext ctx)
        {
            // Surface tint for header
            using (var brush = new SolidBrush(Color.FromArgb(245, ctx.BackgroundColor)))
            {
                g.FillRectangle(brush, bounds);
            }

            Color textColor = isToday ? ctx.PrimaryColor : Color.FromArgb(150, ctx.ForegroundColor);
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(dayName, ctx.DaysHeaderFont, brush, bounds, format);
            }
        }

        public void PaintWeekDayHeader(Graphics g, Rectangle bounds, DateTime date, bool isToday, CalendarPainterContext ctx)
        {
            Color bgColor = isToday ? ctx.PrimaryColor : Color.FromArgb(248, ctx.BackgroundColor);
            Color textColor = isToday ? Color.White : ctx.ForegroundColor;

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }

            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                string text = $"{date:ddd}\n{date:dd}";
                g.DrawString(text, ctx.DayFont, brush, bounds, format);
            }
        }

        #endregion

        #region Events

        public void PaintEventBar(Graphics g, Rectangle bounds, CalendarEvent evt, bool isSelected, bool isHovered, CalendarPainterContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color eventColor = ctx.GetCategoryColor(evt.CategoryId);
            
            // Hover state layer
            if (isHovered)
            {
                eventColor = Color.FromArgb(Math.Min(255, eventColor.R + 20), 
                                           Math.Min(255, eventColor.G + 20), 
                                           Math.Min(255, eventColor.B + 20));
            }

            // Material Design pill shape
            using (var path = CreatePillPath(bounds))
            using (var brush = new SolidBrush(eventColor))
            {
                g.FillPath(brush, path);
            }

            // Selection indicator
            if (isSelected)
            {
                using (var path = CreatePillPath(bounds))
                using (var pen = new Pen(Color.White, 2))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Event title
            using (var brush = new SolidBrush(Color.White))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                var textRect = new Rectangle(bounds.X + 6, bounds.Y, bounds.Width - 10, bounds.Height);
                g.DrawString(evt.Title, ctx.EventFont, brush, textRect, format);
            }
        }

        public void PaintEventBlock(Graphics g, Rectangle bounds, CalendarEvent evt, bool isSelected, bool isHovered, CalendarPainterContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color eventColor = ctx.GetCategoryColor(evt.CategoryId);
            
            // Semi-transparent background
            Color bgColor = Color.FromArgb(isHovered ? 220 : 200, eventColor);

            using (var path = CreateRoundedRect(bounds, 8))
            {
                // Background
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                // Left accent bar
                var accentRect = new Rectangle(bounds.X, bounds.Y + 4, 4, bounds.Height - 8);
                using (var accentPath = CreateRoundedRect(accentRect, 2))
                using (var brush = new SolidBrush(eventColor))
                {
                    g.FillPath(brush, accentPath);
                }

                // Selection border
                if (isSelected)
                {
                    using (var pen = new Pen(eventColor, 2))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Event content
            var contentRect = new Rectangle(bounds.X + 10, bounds.Y + 4, bounds.Width - 14, bounds.Height - 8);
            using (var brush = new SolidBrush(Color.White))
            {
                string text = $"{evt.Title}\n{evt.StartTime:HH:mm} - {evt.EndTime:HH:mm}";
                g.DrawString(text, ctx.EventFont, brush, contentRect);
            }
        }

        public void PaintListViewEvent(Graphics g, Rectangle bounds, CalendarEvent evt, bool isSelected, bool isHovered, CalendarPainterContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color eventColor = ctx.GetCategoryColor(evt.CategoryId);
            Color bgColor = isSelected ? Color.FromArgb(20, ctx.PrimaryColor) :
                           isHovered ? Color.FromArgb(10, ctx.ForegroundColor) :
                           ctx.BackgroundColor;

            // Card background
            using (var path = CreateRoundedRect(bounds, 12))
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                // Subtle border
                using (var pen = new Pen(Color.FromArgb(30, ctx.BorderColor), 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Color indicator bar
            var indicatorRect = new Rectangle(bounds.X + 4, bounds.Y + 8, 4, bounds.Height - 16);
            using (var indicatorPath = CreateRoundedRect(indicatorRect, 2))
            using (var brush = new SolidBrush(eventColor))
            {
                g.FillPath(brush, indicatorPath);
            }

            // Event content
            var contentRect = new Rectangle(bounds.X + 16, bounds.Y + 8, bounds.Width - 24, bounds.Height - 16);
            using (var brush = new SolidBrush(ctx.ForegroundColor))
            {
                string title = evt.Title;
                string time = $"{evt.StartTime:MMM dd, yyyy HH:mm} - {evt.EndTime:HH:mm}";
                
                using (var titleFont = new Font(ctx.EventFont.FontFamily, ctx.EventFont.Size + 1, FontStyle.Bold))
                {
                    g.DrawString(title, titleFont, brush, contentRect.X, contentRect.Y);
                }
                
                using (var timeBrush = new SolidBrush(Color.FromArgb(150, ctx.ForegroundColor)))
                {
                    g.DrawString(time, ctx.EventFont, timeBrush, contentRect.X, contentRect.Y + 20);
                }
                
                if (!string.IsNullOrEmpty(evt.Description))
                {
                    using (var descBrush = new SolidBrush(Color.FromArgb(120, ctx.ForegroundColor)))
                    {
                        g.DrawString(evt.Description, ctx.EventFont, descBrush, contentRect.X, contentRect.Y + 38);
                    }
                }
            }
        }

        #endregion

        #region Time Slots

        public void PaintTimeSlot(Graphics g, Rectangle bounds, int hour, bool isCurrentHour, CalendarPainterContext ctx)
        {
            // Subtle alternating background
            Color bgColor = hour % 2 == 0 ? ctx.BackgroundColor : Color.FromArgb(252, ctx.BackgroundColor);
            if (isCurrentHour)
            {
                bgColor = Color.FromArgb(15, ctx.PrimaryColor);
            }

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Grid line
            using (var pen = new Pen(Color.FromArgb(30, ctx.BorderColor), 1))
            {
                g.DrawLine(pen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
            }

            // Current time indicator
            if (isCurrentHour)
            {
                int currentMinute = DateTime.Now.Minute;
                int indicatorY = bounds.Top + (int)(bounds.Height * currentMinute / 60.0);
                
                using (var pen = new Pen(ctx.PrimaryColor, 2))
                {
                    g.DrawLine(pen, bounds.Left, indicatorY, bounds.Right, indicatorY);
                }
                
                // Circle indicator
                using (var brush = new SolidBrush(ctx.PrimaryColor))
                {
                    g.FillEllipse(brush, bounds.Left - 4, indicatorY - 4, 8, 8);
                }
            }
        }

        public void PaintTimeLabel(Graphics g, Rectangle bounds, int hour, CalendarPainterContext ctx)
        {
            using (var brush = new SolidBrush(Color.FromArgb(150, ctx.ForegroundColor)))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString($"{hour:00}:00", ctx.TimeFont, brush, bounds, format);
            }
        }

        #endregion

        #region Mini Calendar

        public void PaintMiniCalendar(Graphics g, Rectangle bounds, DateTime displayMonth, DateTime selectedDate, CalendarPainterContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Card background
            using (var path = CreateRoundedRect(bounds, 12))
            using (var brush = new SolidBrush(Color.White))
            {
                g.FillPath(brush, path);
            }

            // Card border
            using (var path = CreateRoundedRect(bounds, 12))
            using (var pen = new Pen(Color.FromArgb(30, ctx.BorderColor), 1))
            {
                g.DrawPath(pen, path);
            }

            // Month title
            var titleRect = new Rectangle(bounds.X + 8, bounds.Y + 8, bounds.Width - 16, 24);
            using (var brush = new SolidBrush(ctx.ForegroundColor))
            using (var font = new Font(ctx.DayFont.FontFamily, 11, FontStyle.Bold))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(displayMonth.ToString("MMMM yyyy"), font, brush, titleRect, format);
            }

            // Mini calendar grid
            int cellSize = Math.Min(24, (bounds.Width - 16) / 7);
            int startY = bounds.Y + 40;
            
            // Day headers
            string[] days = { "S", "M", "T", "W", "T", "F", "S" };
            for (int i = 0; i < 7; i++)
            {
                var dayRect = new Rectangle(bounds.X + 8 + i * cellSize, startY, cellSize, 16);
                using (var brush = new SolidBrush(Color.FromArgb(120, ctx.ForegroundColor)))
                using (var font = new Font(ctx.EventFont.FontFamily, 8))
                {
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(days[i], font, brush, dayRect, format);
                }
            }

            // Days
            var firstDay = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            var firstCalendarDay = firstDay.AddDays(-(int)firstDay.DayOfWeek);
            
            for (int week = 0; week < 6; week++)
            {
                for (int day = 0; day < 7; day++)
                {
                    var cellDate = firstCalendarDay.AddDays(week * 7 + day);
                    var cellRect = new Rectangle(bounds.X + 8 + day * cellSize, startY + 20 + week * cellSize, cellSize, cellSize);
                    
                    bool isCurrentMonth = cellDate.Month == displayMonth.Month;
                    bool isToday = cellDate.Date == DateTime.Today;
                    bool isSelected = cellDate.Date == selectedDate.Date;
                    
                    if (isSelected)
                    {
                        using (var brush = new SolidBrush(ctx.PrimaryColor))
                        {
                            g.FillEllipse(brush, cellRect.X + 2, cellRect.Y + 2, cellRect.Width - 4, cellRect.Height - 4);
                        }
                    }
                    else if (isToday)
                    {
                        using (var pen = new Pen(ctx.PrimaryColor, 1))
                        {
                            g.DrawEllipse(pen, cellRect.X + 2, cellRect.Y + 2, cellRect.Width - 4, cellRect.Height - 4);
                        }
                    }
                    
                    Color textColor = isSelected ? Color.White :
                                     isToday ? ctx.PrimaryColor :
                                     isCurrentMonth ? ctx.ForegroundColor :
                                     Color.FromArgb(100, ctx.ForegroundColor);
                    
                    using (var brush = new SolidBrush(textColor))
                    using (var font = new Font(ctx.EventFont.FontFamily, 8))
                    {
                        var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        g.DrawString(cellDate.Day.ToString(), font, brush, cellRect, format);
                    }
                }
            }
        }

        #endregion

        #region Event Details

        public void PaintEventDetails(Graphics g, Rectangle bounds, CalendarEvent evt, CalendarPainterContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (evt == null) return;

            // Card background
            using (var path = CreateRoundedRect(bounds, 12))
            using (var brush = new SolidBrush(Color.White))
            {
                g.FillPath(brush, path);
            }

            // Card border
            using (var path = CreateRoundedRect(bounds, 12))
            using (var pen = new Pen(Color.FromArgb(30, ctx.BorderColor), 1))
            {
                g.DrawPath(pen, path);
            }

            // Category color header
            Color eventColor = ctx.GetCategoryColor(evt.CategoryId);
            var headerRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, 8);
            using (var path = CreateRoundedRect(headerRect, 12, true, true, false, false))
            using (var brush = new SolidBrush(eventColor))
            {
                g.FillPath(brush, path);
            }

            // Content
            int contentY = bounds.Y + 16;
            int contentX = bounds.X + 12;
            int contentWidth = bounds.Width - 24;

            // Title
            using (var brush = new SolidBrush(ctx.ForegroundColor))
            using (var font = new Font(ctx.DayFont.FontFamily, 12, FontStyle.Bold))
            {
                g.DrawString(evt.Title, font, brush, contentX, contentY);
            }
            contentY += 28;

            // Date & Time
            using (var brush = new SolidBrush(Color.FromArgb(150, ctx.ForegroundColor)))
            {
                string dateText = evt.IsAllDay ? 
                    evt.StartTime.ToString("dddd, MMMM dd, yyyy") :
                    $"{evt.StartTime:dddd, MMMM dd, yyyy}\n{evt.StartTime:HH:mm} - {evt.EndTime:HH:mm}";
                g.DrawString(dateText, ctx.EventFont, brush, contentX, contentY);
            }
            contentY += evt.IsAllDay ? 20 : 36;

            // Category
            var category = ctx.Categories?.Find(c => c.Id == evt.CategoryId);
            if (category != null)
            {
                // Category chip
                var chipRect = new Rectangle(contentX, contentY, 80, 20);
                using (var path = CreatePillPath(chipRect))
                using (var brush = new SolidBrush(Color.FromArgb(30, eventColor)))
                {
                    g.FillPath(brush, path);
                }
                using (var brush = new SolidBrush(eventColor))
                {
                    var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(category.Name, ctx.EventFont, brush, chipRect, format);
                }
                contentY += 28;
            }

            // Description
            if (!string.IsNullOrEmpty(evt.Description))
            {
                using (var brush = new SolidBrush(Color.FromArgb(120, ctx.ForegroundColor)))
                {
                    var descRect = new Rectangle(contentX, contentY, contentWidth, bounds.Bottom - contentY - 12);
                    g.DrawString(evt.Description, ctx.EventFont, brush, descRect);
                }
            }
        }

        #endregion

        #region Helper Methods

        private GraphicsPath CreateRoundedRect(Rectangle rect, int radius, bool topLeft = true, bool topRight = true, bool bottomRight = true, bool bottomLeft = true)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            if (topLeft)
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            else
                path.AddLine(rect.X, rect.Y + radius, rect.X, rect.Y);

            if (topRight)
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            else
                path.AddLine(rect.Right - radius, rect.Y, rect.Right, rect.Y);

            if (bottomRight)
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            else
                path.AddLine(rect.Right, rect.Bottom - radius, rect.Right, rect.Bottom);

            if (bottomLeft)
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            else
                path.AddLine(rect.X + radius, rect.Bottom, rect.X, rect.Bottom);

            path.CloseFigure();
            return path;
        }

        private GraphicsPath CreatePillPath(Rectangle rect)
        {
            var path = new GraphicsPath();
            int radius = rect.Height / 2;

            path.AddArc(rect.X, rect.Y, rect.Height, rect.Height, 90, 180);
            path.AddLine(rect.X + radius, rect.Y, rect.Right - radius, rect.Y);
            path.AddArc(rect.Right - rect.Height, rect.Y, rect.Height, rect.Height, 270, 180);
            path.AddLine(rect.Right - radius, rect.Bottom, rect.X + radius, rect.Bottom);
            path.CloseFigure();

            return path;
        }

        #endregion
    }
}

