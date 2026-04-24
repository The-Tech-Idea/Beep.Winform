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

            // Keep a persistent today marker even when selected/hovered.
            if (state.IsToday)
            {
                var markerRect = new Rectangle(bounds.Right - 9, bounds.Y + 5, 4, 4);
                using (var markerBrush = new SolidBrush(ctx.PrimaryColor))
                {
                    g.FillEllipse(markerBrush, markerRect);
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
            Color textColor = isToday ? CommonDrawing.GetContrastingTextColor(ctx.PrimaryColor) : ctx.ForegroundColor;

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
                using (var pen = new Pen(CommonDrawing.GetContrastingTextColor(eventColor), 2))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Event title - use contrasting text color
            Color eventTextColor = CommonDrawing.GetContrastingTextColor(eventColor);
            using (var brush = new SolidBrush(eventTextColor))
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
            Color blockTextColor = CommonDrawing.GetContrastingTextColor(bgColor);
            using (var brush = new SolidBrush(blockTextColor))
            {
                string text = $"{evt.Title}\n{evt.StartTime:HH:mm} - {evt.EndTime:HH:mm}";
                g.DrawString(text, ctx.EventFont, brush, contentRect);
            }
        }

        public void PaintListViewEvent(Graphics g, Rectangle bounds, CalendarEvent evt, bool isSelected, bool isHovered, CalendarPainterContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            CommonDrawing.DrawEventCard(g, ctx, evt, bounds, isSelected, includeDescription: true, includeActions: true);
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
            CommonDrawing.DrawMiniCalendarCard(g, ctx, bounds, displayMonth, selectedDate);
        }

        #endregion

        #region Event Details

        public void PaintEventDetails(Graphics g, Rectangle bounds, CalendarEvent evt, CalendarPainterContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            CommonDrawing.DrawEventInsightsCard(g, ctx, bounds, evt);
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

