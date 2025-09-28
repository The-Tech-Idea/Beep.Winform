using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Control
{
    /// <summary>
    /// DatePicker - Date/time selection
    /// </summary>
    internal sealed class DatePickerPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 8;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);

            if (ctx.ShowHeader)
            {
                ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 16);
            }

            int dateHeight = 32;
            int dateTop = ctx.ShowHeader ? ctx.HeaderRect.Bottom + 4 : ctx.DrawingRect.Top + (ctx.DrawingRect.Height - dateHeight) / 2;

            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, dateTop, ctx.DrawingRect.Width - pad * 2, dateHeight);

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw label
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                using var labelFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
                using var labelBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                g.DrawString(ctx.Title, labelFont, labelBrush, ctx.HeaderRect);
            }

            // Draw date picker background
            using var pickerBrush = new SolidBrush(Color.White);
            using var borderBrush = new SolidBrush(Color.FromArgb(200, Color.Gray));
            using var pickerPath = CreateRoundedPath(ctx.ContentRect, 4);

            g.FillPath(pickerBrush, pickerPath);
            using var borderPen = new Pen(borderBrush, 1);
            g.DrawPath(borderPen, pickerPath);

            // Draw date value
            string dateText = ctx.Value ?? DateTime.Now.ToString("MM/dd/yyyy");
            using var textFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var textBrush = new SolidBrush(Color.FromArgb(180, Color.Black));

            var textRect = Rectangle.Inflate(ctx.ContentRect, -8, 0);
            var format = new StringFormat { LineAlignment = StringAlignment.Center };
            g.DrawString(dateText, textFont, textBrush, textRect, format);

            // Draw calendar icon
            var iconRect = new Rectangle(ctx.ContentRect.Right - 24, ctx.ContentRect.Y + 8, 16, 16);
            WidgetRenderingHelpers.DrawCalendarIcon(g, iconRect, ctx.AccentColor);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw focus indicator
        }
    }
}
