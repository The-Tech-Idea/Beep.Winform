using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Calendar
{
    /// <summary>
    /// DateGrid - Calendar date grid layout painter
    /// </summary>
    internal sealed class DateGridPainter : WidgetPainterBase
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
            // Draw calendar date grid layout
            DrawDateGrid(g, ctx);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) 
        {
            // Draw grid borders and today highlight
            DrawGridBorders(g, ctx);
            DrawTodayHighlight(g, ctx);
        }

        private void DrawDateGrid(Graphics g, WidgetContext ctx)
        {
            // Calculate grid dimensions (7 columns for days, 6 rows for weeks)
            int cols = 7;
            int rows = 6;
            int cellWidth = ctx.DrawingRect.Width / cols;
            int cellHeight = ctx.DrawingRect.Height / rows;

            // Get current date for reference
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var startDate = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);

            using var dateFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var currentMonthBrush = new SolidBrush(Theme?.TextBoxForeColor ?? Color.Black);
            using var otherMonthBrush = new SolidBrush(Color.FromArgb(150, Color.Gray));
            using var todayBrush = new SolidBrush(Theme?.AccentColor ?? Color.Blue);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var currentDate = startDate.AddDays(row * 7 + col);
                    var cellRect = new Rectangle(
                        ctx.DrawingRect.Left + col * cellWidth,
                        ctx.DrawingRect.Top + row * cellHeight,
                        cellWidth,
                        cellHeight
                    );

                    // Draw cell background for current month days
                    if (currentDate.Month == today.Month)
                    {
                        using var cellBgBrush = new SolidBrush(Color.FromArgb(20, Theme?.AccentColor ?? Color.Blue));
                        g.FillRectangle(cellBgBrush, cellRect);
                    }

                    // Choose appropriate brush for date text
                    Brush dateBrush = currentDate.Month == today.Month ? currentMonthBrush : otherMonthBrush;
                    if (currentDate.Date == today.Date)
                    {
                        dateBrush = todayBrush;
                        
                        // Draw today's background circle
                        var circleRect = new Rectangle(
                            cellRect.X + cellRect.Width/2 - 12,
                            cellRect.Y + cellRect.Height/2 - 12,
                            24, 24
                        );
                        using var todayBg = new SolidBrush(Color.FromArgb(40, Theme?.AccentColor ?? Color.Blue));
                        g.FillEllipse(todayBg, circleRect);
                    }

                    // Draw date number
                    var dateText = currentDate.Day.ToString();
                    var textRect = new Rectangle(cellRect.X + 2, cellRect.Y + 2, cellRect.Width - 4, cellRect.Height - 4);
                    g.DrawString(dateText, dateFont, dateBrush, textRect,
                               new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                    // Draw small event indicators if date has events (sample data)
                    if (currentDate.Month == today.Month && currentDate.Day % 3 == 0)
                    {
                        var indicatorRect = new Rectangle(
                            cellRect.Right - 8,
                            cellRect.Top + 2,
                            4, 4
                        );
                        using var eventBrush = new SolidBrush(Color.FromArgb(255, 76, 175, 80));
                        g.FillEllipse(eventBrush, indicatorRect);
                    }
                }
            }
        }

        private void DrawGridBorders(Graphics g, WidgetContext ctx)
        {
            // Draw grid lines
            using var gridPen = new Pen(Color.FromArgb(60, Color.LightGray), 1);
            
            int cols = 7;
            int rows = 6;
            int cellWidth = ctx.DrawingRect.Width / cols;
            int cellHeight = ctx.DrawingRect.Height / rows;

            // Vertical lines
            for (int col = 0; col <= cols; col++)
            {
                int x = ctx.DrawingRect.Left + col * cellWidth;
                g.DrawLine(gridPen, x, ctx.DrawingRect.Top, x, ctx.DrawingRect.Bottom);
            }

            // Horizontal lines
            for (int row = 0; row <= rows; row++)
            {
                int y = ctx.DrawingRect.Top + row * cellHeight;
                g.DrawLine(gridPen, ctx.DrawingRect.Left, y, ctx.DrawingRect.Right, y);
            }
        }

        private void DrawTodayHighlight(Graphics g, WidgetContext ctx)
        {
            // Draw day headers at the top
            string[] dayHeaders = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            int cellWidth = ctx.DrawingRect.Width / 7;

            using var headerFont = new Font(Owner.Font.FontFamily, 7f, FontStyle.Bold);
            using var headerBrush = new SolidBrush(Color.FromArgb(120, Color.Black));

            for (int i = 0; i < dayHeaders.Length; i++)
            {
                var headerRect = new Rectangle(
                    ctx.DrawingRect.Left + i * cellWidth,
                    ctx.DrawingRect.Top - 18,
                    cellWidth,
                    16
                );

                g.DrawString(dayHeaders[i], headerFont, headerBrush, headerRect,
                           new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }
    }
}