using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Control
{
    /// <summary>
    /// NumberSpinner - Numeric input with up/down buttons
    /// </summary>
    internal sealed class NumberSpinnerPainter : WidgetPainterBase
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
            
            // Spinner control area
            int contentTop = ctx.HeaderRect.Bottom + (ctx.HeaderRect.Height > 0 ? 6 : 0);
            int spinnerHeight = 32;
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + padding,
                contentTop,
                ctx.DrawingRect.Width - padding * 2,
                spinnerHeight
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            var bgColor = Theme?.BackColor ?? Color.Empty;
            var borderColor = Theme?.BorderColor ?? Color.Empty;
            
            using (var bgBrush = new SolidBrush(bgColor))
            using (var borderPen = new Pen(borderColor))
            {
                g.FillRoundedRectangle(bgBrush, ctx.DrawingRect, 4);
                g.DrawRoundedRectangle(borderPen, ctx.DrawingRect, 4);
            }
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw label
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                var titleColor =  Theme?.TextBoxForeColor ?? Color.Empty;
                using (var titleFont = new Font("Segoe UI", 9f, FontStyle.Regular))
                using (var titleBrush = new SolidBrush(titleColor))
                {
                    g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, 
                        new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                }
            }

            // Draw spinner control
            DrawSpinnerControl(g, ctx);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw focus border around the input area
            var inputRect = GetInputRect(ctx.ContentRect);
            var focusColor = Theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            
            using (var focusPen = new Pen(focusColor, 2))
            {
                g.DrawRoundedRectangle(focusPen, Rectangle.Inflate(inputRect, 1, 1), 3);
            }
        }

        private void DrawSpinnerControl(Graphics g, WidgetContext ctx)
        {
            var inputRect = GetInputRect(ctx.ContentRect);
            var upButtonRect = GetUpButtonRect(ctx.ContentRect);
            var downButtonRect = GetDownButtonRect(ctx.ContentRect);
            
            // Draw input field
            DrawInputField(g, inputRect);
            
            // Draw up/down buttons
            DrawSpinnerButton(g, upButtonRect, true);
            DrawSpinnerButton(g, downButtonRect, false);
        }

        private void DrawInputField(Graphics g, Rectangle rect)
        {
            var bgColor = Theme?.TextBoxBackColor ?? Color.Empty;
            var borderColor = Theme?.BorderColor ?? Color.Empty;
            var textColor =  Theme?.TextBoxForeColor ?? Color.Empty;
            
            using (var bgBrush = new SolidBrush(bgColor))
            using (var borderPen = new Pen(borderColor))
            {
                g.FillRoundedRectangle(bgBrush, rect, 4);
                g.DrawRoundedRectangle(borderPen, rect, 4);
            }
            
            // Draw sample value
            var sampleValue = "42";
            using (var textFont = new Font("Segoe UI", 10f))
            using (var textBrush = new SolidBrush(textColor))
            {
                var format = new StringFormat 
                { 
                    Alignment = StringAlignment.Center, 
                    LineAlignment = StringAlignment.Center 
                };
                g.DrawString(sampleValue, textFont, textBrush, rect, format);
            }
        }

        private void DrawSpinnerButton(Graphics g, Rectangle rect, bool isUp)
        {
            var buttonColor = Theme?.ButtonBackColor ?? Color.Empty;
            var borderColor = Theme?.BorderColor ?? Color.Empty;
            var arrowColor =  Theme?.TextBoxForeColor ?? Color.Empty;
            
            using (var buttonBrush = new SolidBrush(buttonColor))
            using (var borderPen = new Pen(borderColor))
            {
                g.FillRoundedRectangle(buttonBrush, rect, 3);
                g.DrawRoundedRectangle(borderPen, rect, 3);
            }
            
            // Draw arrow
            using (var arrowPen = new Pen(arrowColor, 1.5f))
            {
                int centerX = rect.Left + rect.Width / 2;
                int centerY = rect.Top + rect.Height / 2;
                int arrowSize = 4;
                
                if (isUp)
                {
                    // Up arrow
                    var points = new Point[]
                    {
                        new Point(centerX, centerY - arrowSize / 2),
                        new Point(centerX - arrowSize, centerY + arrowSize / 2),
                        new Point(centerX + arrowSize, centerY + arrowSize / 2)
                    };
                    g.DrawLines(arrowPen, new Point[] { points[1], points[0], points[2] });
                }
                else
                {
                    // Down arrow
                    var points = new Point[]
                    {
                        new Point(centerX, centerY + arrowSize / 2),
                        new Point(centerX - arrowSize, centerY - arrowSize / 2),
                        new Point(centerX + arrowSize, centerY - arrowSize / 2)
                    };
                    g.DrawLines(arrowPen, new Point[] { points[1], points[0], points[2] });
                }
            }
        }

        private Rectangle GetInputRect(Rectangle contentRect)
        {
            int buttonWidth = 20;
            return new Rectangle(
                contentRect.Left,
                contentRect.Top,
                contentRect.Width - buttonWidth,
                contentRect.Height
            );
        }

        private Rectangle GetUpButtonRect(Rectangle contentRect)
        {
            int buttonWidth = 20;
            return new Rectangle(
                contentRect.Right - buttonWidth,
                contentRect.Top,
                buttonWidth,
                contentRect.Height / 2
            );
        }

        private Rectangle GetDownButtonRect(Rectangle contentRect)
        {
            int buttonWidth = 20;
            return new Rectangle(
                contentRect.Right - buttonWidth,
                contentRect.Top + contentRect.Height / 2,
                buttonWidth,
                contentRect.Height - contentRect.Height / 2
            );
        }
    }
}
