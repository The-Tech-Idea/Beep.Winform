using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Control
{
    /// <summary>
    /// RangeSelector - Dual-handle range slider control
    /// </summary>
    internal sealed class RangeSelectorPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int padding = 8;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Header area for label
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + padding,
                ctx.DrawingRect.Top + padding,
                ctx.DrawingRect.Width - padding * 2,
                string.IsNullOrEmpty(ctx.Title) ? 0 : 20
            );
            
            // Range selector area
            int contentTop = ctx.HeaderRect.Bottom + (ctx.HeaderRect.Height > 0 ? 8 : 0);
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + padding,
                contentTop,
                ctx.DrawingRect.Width - padding * 2,
                ctx.DrawingRect.Bottom - contentTop - padding
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            var bgColor = Theme?.BackColor ?? Color.FromArgb(248, 248, 248);
            var borderColor = Theme?.BorderColor ?? Color.FromArgb(220, 220, 220);
            
            using (var bgBrush = new SolidBrush(bgColor))
            using (var borderPen = new Pen(borderColor))
            {
                g.FillRoundedRectangle(bgBrush, ctx.DrawingRect, 6);
                g.DrawRoundedRectangle(borderPen, ctx.DrawingRect, 6);
            }
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw label
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                var titleColor =  Theme?.TextBoxForeColor ?? Color.FromArgb(70, 70, 70);
                using (var titleFont = new Font("Segoe UI", 9f, FontStyle.Regular))
                using (var titleBrush = new SolidBrush(titleColor))
                {
                    g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, 
                        new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                }
            }

            // Draw range slider
            DrawRangeSlider(g, ctx);
            
            // Draw value labels
            DrawValueLabels(g, ctx);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw focus indicators on handles
            var trackRect = GetTrackRect(ctx.ContentRect);
            var minHandle = GetMinHandleRect(trackRect, 0.25f); // Example: 25% position
            var focusColor = Theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            
            using (var focusPen = new Pen(focusColor, 2))
            {
                g.DrawEllipse(focusPen, Rectangle.Inflate(minHandle, 3, 3));
            }
        }

        private void DrawRangeSlider(Graphics g, WidgetContext ctx)
        {
            var trackRect = GetTrackRect(ctx.ContentRect);
            var minValue = 0.25f; // Example: 25%
            var maxValue = 0.75f; // Example: 75%
            
            // Draw track background
            DrawTrack(g, trackRect);
            
            // Draw selected range
            DrawSelectedRange(g, trackRect, minValue, maxValue);
            
            // Draw handles
            DrawHandle(g, GetMinHandleRect(trackRect, minValue), false);
            DrawHandle(g, GetMaxHandleRect(trackRect, maxValue), true);
        }

        private void DrawTrack(Graphics g, Rectangle trackRect)
        {
            var trackColor = Theme?.BorderColor ?? Color.FromArgb(200, 200, 200);
            using (var trackBrush = new SolidBrush(trackColor))
            {
                g.FillRoundedRectangle(trackBrush, trackRect, trackRect.Height / 2);
            }
        }

        private void DrawSelectedRange(Graphics g, Rectangle trackRect, float minValue, float maxValue)
        {
            int startX = (int)(trackRect.Left + minValue * trackRect.Width);
            int endX = (int)(trackRect.Left + maxValue * trackRect.Width);
            var rangeRect = new Rectangle(startX, trackRect.Top, endX - startX, trackRect.Height);
            
            var rangeColor = Theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            using (var rangeBrush = new SolidBrush(rangeColor))
            {
                g.FillRoundedRectangle(rangeBrush, rangeRect, trackRect.Height / 2);
            }
        }

        private void DrawHandle(Graphics g, Rectangle handleRect, bool isMax)
        {
            var handleColor = Color.White;
            var borderColor = Theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            var shadowColor = Color.FromArgb(60, Color.Black);
            
            // Draw shadow
            var shadowRect = new Rectangle(handleRect.X, handleRect.Y, handleRect.Width, handleRect.Height);
            shadowRect.Offset(1, 1);
            using (var shadowBrush = new SolidBrush(shadowColor))
            {
                g.FillEllipse(shadowBrush, shadowRect);
            }
            
            // Draw handle
            using (var handleBrush = new SolidBrush(handleColor))
            using (var borderPen = new Pen(borderColor, 2))
            {
                g.FillEllipse(handleBrush, handleRect);
                g.DrawEllipse(borderPen, handleRect);
            }
            
            // Draw center dot
            var dotRect = Rectangle.Inflate(handleRect, -4, -4);
            using (var dotBrush = new SolidBrush(borderColor))
            {
                g.FillEllipse(dotBrush, dotRect);
            }
        }

        private void DrawValueLabels(Graphics g, WidgetContext ctx)
        {
            var trackRect = GetTrackRect(ctx.ContentRect);
            var minValue = 25; // Example values
            var maxValue = 75;
            
            var textColor =  Theme?.TextBoxForeColor ?? Color.FromArgb(100, 100, 100);
            using (var textFont = new Font("Segoe UI", 8f))
            using (var textBrush = new SolidBrush(textColor))
            {
                var format = new StringFormat { Alignment = StringAlignment.Center };
                
                // Min value label
                var minRect = new Rectangle(trackRect.Left, trackRect.Bottom + 4, trackRect.Width / 3, 16);
                g.DrawString($"Min: {minValue}", textFont, textBrush, minRect, format);
                
                // Max value label
                var maxRect = new Rectangle(trackRect.Left + 2 * trackRect.Width / 3, trackRect.Bottom + 4, trackRect.Width / 3, 16);
                g.DrawString($"Max: {maxValue}", textFont, textBrush, maxRect, format);
                
                // Range label
                var rangeRect = new Rectangle(trackRect.Left + trackRect.Width / 3, trackRect.Bottom + 4, trackRect.Width / 3, 16);
                g.DrawString($"Range: {maxValue - minValue}", textFont, textBrush, rangeRect, format);
            }
        }

        private Rectangle GetTrackRect(Rectangle contentRect)
        {
            int trackHeight = 8;
            int handleSize = 16;
            int margin = handleSize / 2;
            
            return new Rectangle(
                contentRect.Left + margin,
                contentRect.Top + (contentRect.Height - trackHeight) / 2 - 10, // Leave space for labels
                contentRect.Width - margin * 2,
                trackHeight
            );
        }

        private Rectangle GetMinHandleRect(Rectangle trackRect, float position)
        {
            int handleSize = 16;
            int x = (int)(trackRect.Left + position * trackRect.Width - handleSize / 2);
            int y = trackRect.Top + trackRect.Height / 2 - handleSize / 2;
            
            return new Rectangle(x, y, handleSize, handleSize);
        }

        private Rectangle GetMaxHandleRect(Rectangle trackRect, float position)
        {
            return GetMinHandleRect(trackRect, position);
        }
    }
}
