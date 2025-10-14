using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Calendar;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering
{
    internal static class CommonDrawing
    {
        public static void DrawHeader(Graphics g, CalendarRenderContext ctx, string headerText)
        {
            var rect = ctx.Rects.HeaderRect;
            using (var brush = new SolidBrush(ctx.Theme?.CalendarBackColor ?? ctx.Owner.BackColor))
                g.FillRectangle(brush, rect);

            // Compute a centered position that also respects left/right margins
            int leftMargin = ctx.HeaderLeftMargin;
            int rightMargin = ctx.HeaderRightMargin;
            int availableLeft = rect.X + leftMargin;
            int availableRight = rect.Right - rightMargin;
            int availableWidth = Math.Max(10, availableRight - availableLeft);

            SizeF textSize = TextUtils.MeasureText(g,headerText, ctx.HeaderFont);
            float centeredX = rect.X + (rect.Width - textSize.Width) / 2f;
            float drawX = Math.Max(availableLeft, centeredX);

            // Ensure we don't exceed the right bound
            if (drawX + textSize.Width > availableRight)
            {
                drawX = Math.Max(availableLeft, availableRight - textSize.Width);
            }

            var textRect = new RectangleF(drawX, rect.Y, Math.Min(textSize.Width + 4, availableRight - drawX), rect.Height);
            using (var brush = new SolidBrush(ctx.Theme?.CalendarTitleForColor ?? ctx.Owner.ForeColor))
                g.DrawString(headerText, ctx.HeaderFont, brush, textRect,
                    new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
        }

        public static void DrawViewSelectorBackground(Graphics g, CalendarRenderContext ctx)
        {
            var rect = ctx.Rects.ViewSelectorRect;
            using (var brush = new SolidBrush(ctx.Theme?.CalendarBackColor ?? Color.FromArgb(248, 249, 250)))
                g.FillRectangle(brush, rect);

            using (var pen = new Pen(ctx.Theme?.CalendarBorderColor ?? Color.FromArgb(218, 220, 224)))
                g.DrawLine(pen, rect.X, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
        }

        public static GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        public static Color GetCategoryColor(CalendarRenderContext ctx, CalendarEvent evt)
        {
            var category = ctx.Categories.FirstOrDefault(c => c.Id == evt.CategoryId);
            return category?.Color ?? Color.Gray;
        }
    }
}
