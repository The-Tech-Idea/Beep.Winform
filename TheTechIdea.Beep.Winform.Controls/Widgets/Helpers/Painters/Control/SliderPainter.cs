using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Control
{
    /// <summary>
    /// Slider - Range sliders
    /// </summary>
    internal sealed class SliderPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 8;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);

            if (ctx.ShowHeader)
            {
                ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 16);
            }

            // Slider track area
            int trackHeight = 6;
            int trackTop = ctx.ShowHeader ? ctx.HeaderRect.Bottom + 8 : ctx.DrawingRect.Top + (ctx.DrawingRect.Height - trackHeight) / 2;

            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad + 10,
                trackTop,
                ctx.DrawingRect.Width - pad * 2 - 20,
                trackHeight
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

            // Get slider values
            double minValue = ctx.MinValue;
            double maxValue = ctx.MaxValue;
            double currentValue = Convert.ToDouble(ctx.CurrentValue);

            // Draw track
            using var trackBrush = new SolidBrush(Color.FromArgb(200, Color.Gray));
            using var trackPath = CreateRoundedPath(ctx.ContentRect, ctx.ContentRect.Height / 2);
            g.FillPath(trackBrush, trackPath);

            // Draw active track
            double percentage = (currentValue - minValue) / (maxValue - minValue);
            int activeWidth = (int)(ctx.ContentRect.Width * percentage);
            var activeRect = new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y, activeWidth, ctx.ContentRect.Height);

            using var activeBrush = new SolidBrush(ctx.AccentColor);
            using var activePath = CreateRoundedPath(activeRect, ctx.ContentRect.Height / 2);
            g.FillPath(activeBrush, activePath);

            // Draw thumb
            int thumbSize = 16;
            int thumbX = ctx.ContentRect.X + activeWidth - thumbSize / 2;
            int thumbY = ctx.ContentRect.Y + ctx.ContentRect.Height / 2 - thumbSize / 2;
            var thumbRect = new Rectangle(thumbX, thumbY, thumbSize, thumbSize);

            using var thumbBrush = new SolidBrush(Color.White);
            using var thumbBorderBrush = new SolidBrush(ctx.AccentColor);

            g.FillEllipse(thumbBorderBrush, Rectangle.Inflate(thumbRect, 1, 1));
            g.FillEllipse(thumbBrush, thumbRect);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw value label
        }
    }

}
