using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Control
{
    /// <summary>
    /// DropdownFilter - Filter dropdowns
    /// </summary>
    internal sealed class DropdownFilterPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 8;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);

            if (ctx.ShowHeader)
            {
                ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 16);
            }

            // Dropdown area
            int dropdownHeight = 32;
            int dropdownTop = ctx.ShowHeader ? ctx.HeaderRect.Bottom + 4 : ctx.DrawingRect.Top + (ctx.DrawingRect.Height - dropdownHeight) / 2;

            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                dropdownTop,
                ctx.DrawingRect.Width - pad * 2,
                dropdownHeight
            );

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

            // Draw dropdown background
            using var dropdownBrush = new SolidBrush(Color.White);
            using var borderBrush = new SolidBrush(Color.FromArgb(200, Color.Gray));
            using var dropdownPath = CreateRoundedPath(ctx.ContentRect, 4);

            g.FillPath(dropdownBrush, dropdownPath);
            using var borderPen = new Pen(borderBrush, 1);
            g.DrawPath(borderPen, dropdownPath);

            // Draw selected value
            string selectedText = ctx.Value ?? "Select...";
            using var textFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var textBrush = new SolidBrush(Color.FromArgb(180, Color.Black));

            var textRect = Rectangle.Inflate(ctx.ContentRect, -8, 0);
            var format = new StringFormat { LineAlignment = StringAlignment.Center };
            g.DrawString(selectedText, textFont, textBrush, textRect, format);

            // Draw dropdown arrow
            var arrowRect = new Rectangle(ctx.ContentRect.Right - 24, ctx.ContentRect.Y + 8, 16, 16);
            WidgetRenderingHelpers.DrawDropdownArrow(g, arrowRect, Color.FromArgb(150, Color.Gray));
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw hover/focus states
        }
    }
}
