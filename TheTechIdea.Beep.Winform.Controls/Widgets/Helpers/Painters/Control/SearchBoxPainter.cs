using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Control
{
    /// <summary>
    /// SearchBox - Search input with suggestions
    /// </summary>
    internal sealed class SearchBoxPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 8;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);

            if (ctx.ShowHeader)
            {
                ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 16);
            }

            int searchHeight = 36;
            int searchTop = ctx.ShowHeader ? ctx.HeaderRect.Bottom + 4 : ctx.DrawingRect.Top + (ctx.DrawingRect.Height - searchHeight) / 2;

            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, searchTop, ctx.DrawingRect.Width - pad * 2, searchHeight);

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

            // Draw search box background
            using var searchBrush = new SolidBrush(Color.White);
            using var borderBrush = new SolidBrush(ctx.AccentColor);
            using var searchPath = CreateRoundedPath(ctx.ContentRect, 18);

            g.FillPath(searchBrush, searchPath);
            using var borderPen = new Pen(borderBrush, 2);
            g.DrawPath(borderPen, searchPath);

            // Draw search icon
            var iconRect = new Rectangle(ctx.ContentRect.X + 12, ctx.ContentRect.Y + 10, 16, 16);
            WidgetRenderingHelpers.DrawSearchIcon(g, iconRect, Color.FromArgb(150, Color.Gray));

            // Draw search text or placeholder
            string searchText = !string.IsNullOrEmpty(ctx.Value) ? ctx.Value : "Search...";
            Color textColor = !string.IsNullOrEmpty(ctx.Value) ? Color.FromArgb(180, Color.Black) : Color.FromArgb(120, Color.Gray);

            using var textFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Regular);
            using var textBrush = new SolidBrush(textColor);

            var textRect = new Rectangle(ctx.ContentRect.X + 36, ctx.ContentRect.Y, ctx.ContentRect.Width - 48, ctx.ContentRect.Height);
            var format = new StringFormat { LineAlignment = StringAlignment.Center };
            g.DrawString(searchText, textFont, textBrush, textRect, format);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw suggestion dropdown indicator
        }
    }

}
