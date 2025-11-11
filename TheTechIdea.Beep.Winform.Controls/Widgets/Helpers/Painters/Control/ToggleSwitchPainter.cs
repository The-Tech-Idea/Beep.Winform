using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Control
{
    /// <summary>
    /// ToggleSwitch - On/off toggle switches
    /// </summary>
    internal sealed class ToggleSwitchPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 8;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);

            // Label area
            if (ctx.ShowHeader)
            {
                ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 16);
            }

            // Toggle switch area
            int toggleWidth = 50;
            int toggleHeight = 24;
            int toggleTop = ctx.ShowHeader ? ctx.HeaderRect.Bottom + 4 : ctx.DrawingRect.Top + (ctx.DrawingRect.Height - toggleHeight) / 2;

            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                toggleTop,
                toggleWidth,
                toggleHeight
            );

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            // Minimal background for controls
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

            // Draw toggle switch
            bool isOn = Convert.ToBoolean(ctx.CurrentValue);
            Color trackColor = isOn ? ctx.AccentColor : Color.FromArgb(200, Color.Gray);
            Color thumbColor = Color.White;

            // Track
            using var trackBrush = new SolidBrush(trackColor);
            using var trackPath = CreateRoundedPath(ctx.ContentRect, ctx.ContentRect.Height / 2);
            g.FillPath(trackBrush, trackPath);

            // Thumb
            int thumbSize = ctx.ContentRect.Height - 4;
            int thumbX = isOn ? ctx.ContentRect.Right - thumbSize - 2 : ctx.ContentRect.X + 2;
            var thumbRect = new Rectangle(thumbX, ctx.ContentRect.Y + 2, thumbSize, thumbSize);

            using var thumbBrush = new SolidBrush(thumbColor);
            g.FillEllipse(thumbBrush, thumbRect);

            // Thumb shadow
            using var shadowBrush = new SolidBrush(Color.FromArgb(50, Color.Black));
            g.FillEllipse(shadowBrush, Rectangle.Inflate(thumbRect, 1, 1));
            g.FillEllipse(thumbBrush, thumbRect);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw focus indicator
        }
    }

}
