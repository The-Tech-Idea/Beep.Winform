using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.StylePainters
{
    /// <summary>
    /// Clean, minimal style calendar painter
    /// Features: Clean lines, subtle colors, typography-focused design
    /// </summary>
    public class MinimalCalendarPainter : ICalendarStylePainter
    {
        private readonly CalendarStyleMetrics _metrics;

        public string StyleName => "Minimal";
        public string DisplayName => "Clean Minimal";

        public MinimalCalendarPainter()
        {
            _metrics = new CalendarStyleMetrics
            {
                HeaderHeight = 56,
                ViewSelectorHeight = 40,
                DayHeaderHeight = 28,
                TimeSlotHeight = 48,
                EventBarHeight = 16,
                EventSpacing = 2,
                CornerRadius = 4,
                CellPadding = 2,
                SidebarWidth = 280,
                TimeColumnWidth = 56,
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
            using (var brush = new SolidBrush(ctx.BackgroundColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Clean typography-focused header
            using (var brush = new SolidBrush(ctx.ForegroundColor))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(headerText, ctx.HeaderFont, brush, bounds, format);
            }
        }

        public void PaintViewSelector(Graphics g, Rectangle bounds, CalendarPainterContext ctx)
        {
            using (var brush = new SolidBrush(ctx.BackgroundColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Thin bottom border
            using (var pen = new Pen(Color.FromArgb(50, ctx.BorderColor), 1))
            {
                g.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }
        }

        public void PaintSidebar(Graphics g, Rectangle bounds, CalendarPainterContext ctx)
        {
            using (var brush = new SolidBrush(ctx.BackgroundColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Subtle left border
            using (var pen = new Pen(Color.FromArgb(40, ctx.BorderColor), 1))
            {
                g.DrawLine(pen, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
            }
        }

        #endregion

        #region Day Cells

        public void PaintDayCell(Graphics g, Rectangle bounds, DateTime date, DayCellState state, CalendarPainterContext ctx)
        {
            // Background
            Color bgColor = !state.IsCurrentMonth ? ctx.OutOfMonthBackColor :
                           state.IsSelected ? ctx.SelectedBackColor :
                           state.IsHovered ? ctx.HoverBackColor :
                           ctx.BackgroundColor;

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Subtle grid
            using (var pen = new Pen(Color.FromArgb(25, ctx.BorderColor), 1))
            {
                g.DrawRectangle(pen, bounds);
            }

            // Day number
            Color textColor = state.IsSelected ? ctx.SelectedForeColor :
                             !state.IsCurrentMonth ? ctx.OutOfMonthForeColor :
                             ctx.ForegroundColor;

            // Today indicator - simple underline
            if (state.IsToday)
            {
                using (var pen = new Pen(ctx.PrimaryColor, 2))
                {
                    int underlineY = bounds.Y + 22;
                    int underlineWidth = 16;
                    int underlineX = bounds.X + 6;
                    g.DrawLine(pen, underlineX, underlineY, underlineX + underlineWidth, underlineY);
                }
                textColor = ctx.PrimaryColor;
            }

            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(date.Day.ToString(), ctx.DayFont, brush, bounds.X + 6, bounds.Y + 4);
            }
        }

        public void PaintDayHeader(Graphics g, Rectangle bounds, string dayName, bool isToday, CalendarPainterContext ctx)
        {
            using (var brush = new SolidBrush(ctx.BackgroundColor))
            {
                g.FillRectangle(brush, bounds);
            }

            Color textColor = isToday ? ctx.PrimaryColor : Color.FromArgb(140, ctx.ForegroundColor);
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
            using (var brush = new SolidBrush(ctx.BackgroundColor))
            {
                g.FillRectangle(brush, bounds);
            }

            Color textColor = isToday ? ctx.PrimaryColor : ctx.ForegroundColor;
            using (var brush = new SolidBrush(textColor))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                
                // Day name
                var dayNameRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height / 2);
                using (var smallFont = new Font(ctx.DaysHeaderFont.FontFamily, ctx.DaysHeaderFont.Size - 1))
                {
                    g.DrawString(date.ToString("ddd").ToUpper(), smallFont, brush, dayNameRect, format);
                }
                
                // Date number
                var dateRect = new Rectangle(bounds.X, bounds.Y + bounds.Height / 2, bounds.Width, bounds.Height / 2);
                g.DrawString(date.Day.ToString(), ctx.DayFont, brush, dateRect, format);
            }

            // Today indicator
            if (isToday)
            {
                using (var pen = new Pen(ctx.PrimaryColor, 2))
                {
                    g.DrawLine(pen, bounds.X + 10, bounds.Bottom - 2, bounds.Right - 10, bounds.Bottom - 2);
                }
            }
        }

        #endregion

        #region Events

        public void PaintEventBar(Graphics g, Rectangle bounds, CalendarEvent evt, bool isSelected, bool isHovered, CalendarPainterContext ctx)
        {
            Color eventColor = ctx.GetCategoryColor(evt.CategoryId);

            // Simple left border indicator
            using (var brush = new SolidBrush(eventColor))
            {
                g.FillRectangle(brush, bounds.X, bounds.Y, 3, bounds.Height);
            }

            // Background
            Color bgColor = isSelected ? Color.FromArgb(30, eventColor) :
                           isHovered ? Color.FromArgb(15, eventColor) :
                           Color.FromArgb(8, eventColor);
            
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds.X + 3, bounds.Y, bounds.Width - 3, bounds.Height);
            }

            // Text
            using (var brush = new SolidBrush(ctx.ForegroundColor))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                var textRect = new Rectangle(bounds.X + 6, bounds.Y, bounds.Width - 8, bounds.Height);
                g.DrawString(evt.Title, ctx.EventFont, brush, textRect, format);
            }
        }

        public void PaintEventBlock(Graphics g, Rectangle bounds, CalendarEvent evt, bool isSelected, bool isHovered, CalendarPainterContext ctx)
        {
            Color eventColor = ctx.GetCategoryColor(evt.CategoryId);

            // Left accent bar
            using (var brush = new SolidBrush(eventColor))
            {
                g.FillRectangle(brush, bounds.X, bounds.Y, 3, bounds.Height);
            }

            // Background
            Color bgColor = isSelected ? Color.FromArgb(40, eventColor) :
                           isHovered ? Color.FromArgb(25, eventColor) :
                           Color.FromArgb(15, eventColor);
            
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds.X + 3, bounds.Y, bounds.Width - 3, bounds.Height);
            }

            // Border on selection
            if (isSelected)
            {
                using (var pen = new Pen(eventColor, 1))
                {
                    g.DrawRectangle(pen, bounds);
                }
            }

            // Content
            var contentRect = new Rectangle(bounds.X + 8, bounds.Y + 4, bounds.Width - 12, bounds.Height - 8);
            using (var brush = new SolidBrush(ctx.ForegroundColor))
            {
                g.DrawString(evt.Title, ctx.EventFont, brush, contentRect.X, contentRect.Y);
            }
            
            using (var brush = new SolidBrush(Color.FromArgb(140, ctx.ForegroundColor)))
            {
                g.DrawString($"{evt.StartTime:HH:mm} - {evt.EndTime:HH:mm}", ctx.EventFont, brush, contentRect.X, contentRect.Y + 16);
            }
        }

        public void PaintListViewEvent(Graphics g, Rectangle bounds, CalendarEvent evt, bool isSelected, bool isHovered, CalendarPainterContext ctx)
        {
            Color eventColor = ctx.GetCategoryColor(evt.CategoryId);

            // Background
            Color bgColor = isSelected ? Color.FromArgb(20, ctx.PrimaryColor) :
                           isHovered ? Color.FromArgb(8, ctx.ForegroundColor) :
                           ctx.BackgroundColor;
            
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Left accent
            using (var brush = new SolidBrush(eventColor))
            {
                g.FillRectangle(brush, bounds.X, bounds.Y, 3, bounds.Height);
            }

            // Bottom border
            using (var pen = new Pen(Color.FromArgb(30, ctx.BorderColor), 1))
            {
                g.DrawLine(pen, bounds.X, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }

            // Content
            int contentX = bounds.X + 12;
            
            // Title
            using (var brush = new SolidBrush(ctx.ForegroundColor))
            {
                g.DrawString(evt.Title, ctx.DayFont, brush, contentX, bounds.Y + 8);
            }

            // Time
            using (var brush = new SolidBrush(Color.FromArgb(140, ctx.ForegroundColor)))
            {
                string timeText = evt.IsAllDay ? "All day" : $"{evt.StartTime:HH:mm} - {evt.EndTime:HH:mm}";
                g.DrawString($"{evt.StartTime:MMM dd} • {timeText}", ctx.EventFont, brush, contentX, bounds.Y + 28);
            }

            // Description
            if (!string.IsNullOrEmpty(evt.Description))
            {
                using (var brush = new SolidBrush(Color.FromArgb(100, ctx.ForegroundColor)))
                {
                    g.DrawString(evt.Description, ctx.EventFont, brush, contentX, bounds.Y + 44);
                }
            }
        }

        #endregion

        #region Time Slots

        public void PaintTimeSlot(Graphics g, Rectangle bounds, int hour, bool isCurrentHour, CalendarPainterContext ctx)
        {
            // Subtle alternating
            if (hour % 2 == 0)
            {
                using (var brush = new SolidBrush(Color.FromArgb(5, ctx.ForegroundColor)))
                {
                    g.FillRectangle(brush, bounds);
                }
            }

            // Grid line
            using (var pen = new Pen(Color.FromArgb(20, ctx.BorderColor), 1))
            {
                g.DrawLine(pen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
            }

            // Current hour indicator
            if (isCurrentHour)
            {
                int currentMinute = DateTime.Now.Minute;
                int indicatorY = bounds.Top + (int)(bounds.Height * currentMinute / 60.0);
                
                using (var pen = new Pen(ctx.PrimaryColor, 1))
                {
                    g.DrawLine(pen, bounds.Left, indicatorY, bounds.Right, indicatorY);
                }
            }
        }

        public void PaintTimeLabel(Graphics g, Rectangle bounds, int hour, CalendarPainterContext ctx)
        {
            using (var brush = new SolidBrush(Color.FromArgb(120, ctx.ForegroundColor)))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Far,
                    LineAlignment = StringAlignment.Near
                };
                var textRect = new Rectangle(bounds.X, bounds.Y + 2, bounds.Width - 8, bounds.Height);
                g.DrawString($"{hour:00}:00", ctx.TimeFont, brush, textRect, format);
            }
        }

        #endregion

        #region Mini Calendar

        public void PaintMiniCalendar(Graphics g, Rectangle bounds, DateTime displayMonth, DateTime selectedDate, CalendarPainterContext ctx)
        {
            using (var brush = new SolidBrush(ctx.BackgroundColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Subtle border
            using (var pen = new Pen(Color.FromArgb(30, ctx.BorderColor), 1))
            {
                g.DrawRectangle(pen, bounds);
            }

            // Month title
            var titleRect = new Rectangle(bounds.X + 8, bounds.Y + 8, bounds.Width - 16, 20);
            using (var brush = new SolidBrush(ctx.ForegroundColor))
            using (var font = new Font(ctx.DayFont.FontFamily, 10, FontStyle.Bold))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(displayMonth.ToString("MMMM yyyy"), font, brush, titleRect, format);
            }

            // Mini calendar grid
            int cellSize = Math.Min(22, (bounds.Width - 16) / 7);
            int startY = bounds.Y + 36;
            
            // Day headers
            string[] days = { "S", "M", "T", "W", "T", "F", "S" };
            for (int i = 0; i < 7; i++)
            {
                var dayRect = new Rectangle(bounds.X + 8 + i * cellSize, startY, cellSize, 14);
                using (var brush = new SolidBrush(Color.FromArgb(100, ctx.ForegroundColor)))
                using (var font = new Font(ctx.EventFont.FontFamily, 7))
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
                    var cellRect = new Rectangle(bounds.X + 8 + day * cellSize, startY + 18 + week * cellSize, cellSize, cellSize);
                    
                    bool isCurrentMonth = cellDate.Month == displayMonth.Month;
                    bool isToday = cellDate.Date == DateTime.Today;
                    bool isSelected = cellDate.Date == selectedDate.Date;
                    
                    Color textColor = !isCurrentMonth ? Color.FromArgb(80, ctx.ForegroundColor) :
                                     isSelected ? ctx.PrimaryColor :
                                     isToday ? ctx.PrimaryColor :
                                     ctx.ForegroundColor;

                    // Selection/Today indicator
                    if (isSelected)
                    {
                        using (var pen = new Pen(ctx.PrimaryColor, 1))
                        {
                            g.DrawRectangle(pen, cellRect.X + 2, cellRect.Y + 2, cellRect.Width - 4, cellRect.Height - 4);
                        }
                    }
                    else if (isToday)
                    {
                        using (var pen = new Pen(ctx.PrimaryColor, 1) { DashStyle = DashStyle.Dot })
                        {
                            g.DrawRectangle(pen, cellRect.X + 2, cellRect.Y + 2, cellRect.Width - 4, cellRect.Height - 4);
                        }
                    }
                    
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
            if (evt == null) return;

            using (var brush = new SolidBrush(ctx.BackgroundColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Border
            using (var pen = new Pen(Color.FromArgb(30, ctx.BorderColor), 1))
            {
                g.DrawRectangle(pen, bounds);
            }

            Color eventColor = ctx.GetCategoryColor(evt.CategoryId);

            // Top color bar
            using (var brush = new SolidBrush(eventColor))
            {
                g.FillRectangle(brush, bounds.X, bounds.Y, bounds.Width, 3);
            }

            int contentY = bounds.Y + 12;
            int contentX = bounds.X + 12;

            // Title
            using (var brush = new SolidBrush(ctx.ForegroundColor))
            using (var font = new Font(ctx.DayFont.FontFamily, 11, FontStyle.Bold))
            {
                g.DrawString(evt.Title, font, brush, contentX, contentY);
            }
            contentY += 24;

            // Date & Time
            using (var brush = new SolidBrush(Color.FromArgb(140, ctx.ForegroundColor)))
            {
                string dateText = evt.IsAllDay ? 
                    evt.StartTime.ToString("dddd, MMMM dd") :
                    $"{evt.StartTime:dddd, MMMM dd}\n{evt.StartTime:HH:mm} – {evt.EndTime:HH:mm}";
                g.DrawString(dateText, ctx.EventFont, brush, contentX, contentY);
            }
            contentY += evt.IsAllDay ? 18 : 34;

            // Category
            var category = ctx.Categories?.Find(c => c.Id == evt.CategoryId);
            if (category != null)
            {
                using (var brush = new SolidBrush(eventColor))
                {
                    g.FillRectangle(brush, contentX, contentY + 2, 8, 8);
                }
                using (var brush = new SolidBrush(Color.FromArgb(140, ctx.ForegroundColor)))
                {
                    g.DrawString(category.Name, ctx.EventFont, brush, contentX + 14, contentY);
                }
                contentY += 20;
            }

            // Description
            if (!string.IsNullOrEmpty(evt.Description))
            {
                contentY += 8;
                using (var brush = new SolidBrush(Color.FromArgb(100, ctx.ForegroundColor)))
                {
                    var descRect = new Rectangle(contentX, contentY, bounds.Width - 24, bounds.Bottom - contentY - 12);
                    g.DrawString(evt.Description, ctx.EventFont, brush, descRect);
                }
            }
        }

        #endregion
    }
}

