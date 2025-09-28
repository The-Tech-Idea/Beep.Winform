using System;
using System.Drawing;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Calendar
{
    /// <summary>
    /// DatePicker - Date selection interface painter
    /// </summary>
    internal sealed class DatePickerPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Selected date display
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                40
            );
            
            // Calendar icon
            ctx.IconRect = new Rectangle(
                ctx.HeaderRect.Right - 32,
                ctx.HeaderRect.Y + 4,
                24, 24
            );
            
            // Date format info
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - ctx.HeaderRect.Bottom - pad * 2
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
            
            // Draw border like an input field
            using var borderPen = new Pen(Color.FromArgb(150, Color.Black), 1);
            g.DrawPath(borderPen, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            var selectedDate = ctx.CustomData.ContainsKey("SelectedDate") ? 
                (DateTime)ctx.CustomData["SelectedDate"] : DateTime.Today;

            // Draw selected date
            using var dateFont = new Font(Owner.Font.FontFamily, 14f, FontStyle.Regular);
            using var dateBrush = new SolidBrush(Color.FromArgb(200, Color.Black));
            string dateText = selectedDate.ToString("dddd, MMMM dd, yyyy");
            
            var textRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y, ctx.HeaderRect.Width - 40, ctx.HeaderRect.Height);
            var dateFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(dateText, dateFont, dateBrush, textRect, dateFormat);
            
            // Draw calendar icon
            DrawCalendarIcon(g, ctx.IconRect, ctx.AccentColor);
            
            // Draw format info
            using var infoFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var infoBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
            string infoText = "Click to select a different date";
            g.DrawString(infoText, infoFont, infoBrush, ctx.FooterRect);
        }

        private void DrawCalendarIcon(Graphics g, Rectangle rect, Color accentColor)
        {
            // Draw calendar base
            using var calendarBrush = new SolidBrush(Color.FromArgb(200, accentColor));
            var calendarRect = new Rectangle(rect.X, rect.Y + 4, rect.Width, rect.Height - 4);
            using var calendarPath = CreateRoundedPath(calendarRect, 3);
            g.FillPath(calendarBrush, calendarPath);
            
            // Draw calendar header
            using var headerBrush = new SolidBrush(accentColor);
            var headerRect = new Rectangle(rect.X + 1, rect.Y + 5, rect.Width - 2, 6);
            g.FillRectangle(headerBrush, headerRect);
            
            // Draw calendar rings
            using var ringBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
            g.FillRectangle(ringBrush, rect.X + 4, rect.Y, 2, 8);
            g.FillRectangle(ringBrush, rect.X + rect.Width - 6, rect.Y, 2, 8);
            
            // Draw calendar grid
            using var gridPen = new Pen(Color.White, 1);
            for (int i = 1; i < 3; i++)
            {
                int y = calendarRect.Y + 8 + i * 4;
                g.DrawLine(gridPen, calendarRect.X + 2, y, calendarRect.Right - 2, y);
            }
            for (int i = 1; i < 4; i++)
            {
                int x = calendarRect.X + 2 + i * 4;
                g.DrawLine(gridPen, x, calendarRect.Y + 8, x, calendarRect.Bottom - 2);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw date picker dropdown indicator
        }
    }
}