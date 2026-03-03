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

            // Keep today discoverable even in selected/hovered states.
            if (state.IsToday)
            {
                using (var markerBrush = new SolidBrush(ctx.PrimaryColor))
                {
                    g.FillEllipse(markerBrush, bounds.Right - 9, bounds.Y + 5, 4, 4);
                }
            }

            if (state.IsFocused)
            {
                using (var focusPen = new Pen(ctx.PrimaryColor, 2f))
                {
                    var focusBounds = Rectangle.Inflate(bounds, -2, -2);
                    g.DrawRectangle(focusPen, focusBounds);
                }
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
                g.DrawString(date.ToString("ddd").ToUpper(), ctx.DaysHeaderFont, brush, dayNameRect, format);
                
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
            CommonDrawing.DrawEventCard(g, ctx, evt, bounds, isSelected, includeDescription: true, includeActions: true);
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
            CommonDrawing.DrawMiniCalendarCard(g, ctx, bounds, displayMonth, selectedDate);
        }

        #endregion

        #region Event Details

        public void PaintEventDetails(Graphics g, Rectangle bounds, CalendarEvent evt, CalendarPainterContext ctx)
        {
            CommonDrawing.DrawEventInsightsCard(g, ctx, bounds, evt);
        }

        #endregion
    }
}

