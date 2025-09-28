using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Control
{
    /// <summary>
    /// CheckboxGroup - Group of related checkboxes
    /// </summary>
    internal sealed class CheckboxGroupPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int padding = 8;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Header area for group title
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + padding,
                ctx.DrawingRect.Top + padding,
                ctx.DrawingRect.Width - padding * 2,
                string.IsNullOrEmpty(ctx.Title) ? 0 : 20
            );
            
            // Checkbox items area
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
            var bgColor = Theme?.BackColor ?? Color.FromArgb(249, 249, 249);
            var borderColor = Theme?.BorderColor ?? Color.FromArgb(225, 225, 225);
            
            using (var bgBrush = new SolidBrush(bgColor))
            using (var borderPen = new Pen(borderColor))
            {
                g.FillRoundedRectangle(bgBrush, ctx.DrawingRect, 4);
                g.DrawRoundedRectangle(borderPen, ctx.DrawingRect, 4);
            }
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw group title
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                var titleColor = Theme?.TextColor ?? Color.FromArgb(70, 70, 70);
                using (var titleFont = new Font("Segoe UI", 9.5f, FontStyle.Bold))
                using (var titleBrush = new SolidBrush(titleColor))
                {
                    g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, 
                        new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                }
            }

            // Draw checkboxes
            DrawCheckboxes(g, ctx);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw focus indicators if needed
            var checkboxes = GetSampleCheckboxes();
            var checkboxRects = CalculateCheckboxLayout(ctx.ContentRect, checkboxes.Count);
            
            if (checkboxRects.Count > 0)
            {
                var focusColor = Theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
                using (var focusPen = new Pen(focusColor, 2))
                {
                    // Draw focus on first checkbox as example
                    var focusRect = Rectangle.Inflate(checkboxRects[0], 1, 1);
                    g.DrawRoundedRectangle(focusPen, focusRect, 2);
                }
            }
        }

        private void DrawCheckboxes(Graphics g, WidgetContext ctx)
        {
            var checkboxes = GetSampleCheckboxes();
            var checkboxRects = CalculateCheckboxLayout(ctx.ContentRect, checkboxes.Count);
            
            for (int i = 0; i < checkboxes.Count; i++)
            {
                DrawCheckbox(g, checkboxRects[i], checkboxes[i], i % 2 == 0); // Alternate checked state
            }
        }

        private List<Rectangle> CalculateCheckboxLayout(Rectangle area, int count)
        {
            var rects = new List<Rectangle>();
            int itemHeight = 24;
            int spacing = 4;
            
            for (int i = 0; i < count; i++)
            {
                int y = area.Top + i * (itemHeight + spacing);
                if (y + itemHeight > area.Bottom) break;
                
                rects.Add(new Rectangle(area.Left, y, area.Width, itemHeight));
            }
            
            return rects;
        }

        private void DrawCheckbox(Graphics g, Rectangle rect, string text, bool isChecked)
        {
            int checkSize = 16;
            var checkRect = new Rectangle(rect.Left, rect.Top + (rect.Height - checkSize) / 2, checkSize, checkSize);
            var textRect = new Rectangle(checkRect.Right + 8, rect.Top, rect.Width - checkSize - 8, rect.Height);
            
            // Draw checkbox background
            var checkBgColor = isChecked 
                ? (Theme?.AccentColor ?? Color.FromArgb(0, 120, 215))
                : Color.White;
            var checkBorderColor = Theme?.BorderColor ?? Color.FromArgb(150, 150, 150);
            
            using (var checkBgBrush = new SolidBrush(checkBgColor))
            using (var checkBorderPen = new Pen(checkBorderColor, 1.5f))
            {
                g.FillRoundedRectangle(checkBgBrush, checkRect, 3);
                g.DrawRoundedRectangle(checkBorderPen, checkRect, 3);
            }
            
            // Draw checkmark
            if (isChecked)
            {
                using (var checkPen = new Pen(Color.White, 2f))
                {
                    var checkPoints = new Point[]
                    {
                        new Point(checkRect.Left + 4, checkRect.Top + 8),
                        new Point(checkRect.Left + 7, checkRect.Top + 11),
                        new Point(checkRect.Left + 12, checkRect.Top + 5)
                    };
                    g.DrawLines(checkPen, checkPoints);
                }
            }
            
            // Draw text
            var textColor = Theme?.TextColor ?? Color.FromArgb(70, 70, 70);
            using (var textFont = new Font("Segoe UI", 8.5f))
            using (var textBrush = new SolidBrush(textColor))
            {
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(text, textFont, textBrush, textRect, format);
            }
        }

        private List<string> GetSampleCheckboxes()
        {
            return new List<string> 
            { 
                "Option 1", 
                "Option 2", 
                "Option 3", 
                "Enable feature",
                "Show advanced"
            };
        }
    }
}
