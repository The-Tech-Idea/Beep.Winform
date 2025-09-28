using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Control
{
    /// <summary>
    /// ButtonGroup - Group of related action buttons
    /// </summary>
    internal sealed class ButtonGroupPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int padding = 8;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            // Header area for title
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + padding,
                ctx.DrawingRect.Top + padding,
                ctx.DrawingRect.Width - padding * 2,
                string.IsNullOrEmpty(ctx.Title) ? 0 : 24
            );
            
            // Button area
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
            // Draw subtle background with light border
            var bgColor = Theme?.BackColor ?? Color.FromArgb(250, 250, 250);
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
            // Draw title if present
            if (!string.IsNullOrEmpty(ctx.Title))
            {
                var titleColor = Theme?.TextColor ?? Color.FromArgb(70, 70, 70);
                using (var titleFont = new Font("Segoe UI", 10f, FontStyle.Bold))
                using (var titleBrush = new SolidBrush(titleColor))
                {
                    g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, 
                        new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                }
            }

            // Draw buttons
            DrawButtons(g, ctx);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw focus indicators on buttons if needed
            var buttons = GetSampleButtons();
            var buttonsRect = CalculateButtonLayout(ctx.ContentRect, buttons.Count);
            
            if (buttonsRect.Count > 0)
            {
                var focusColor = Theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
                using (var focusPen = new Pen(focusColor, 2))
                {
                    // Draw focus on first button as example
                    var focusRect = Rectangle.Inflate(buttonsRect[0], 2, 2);
                    g.DrawRoundedRectangle(focusPen, focusRect, 4);
                }
            }
        }

        private void DrawButtons(Graphics g, WidgetContext ctx)
        {
            var buttons = GetSampleButtons();
            var buttonsRect = CalculateButtonLayout(ctx.ContentRect, buttons.Count);
            
            for (int i = 0; i < buttons.Count; i++)
            {
                DrawButton(g, buttonsRect[i], buttons[i], i == 0); // First button selected as example
            }
        }

        private List<Rectangle> CalculateButtonLayout(Rectangle area, int buttonCount)
        {
            if (buttonCount == 0) return new List<Rectangle>();
            
            var buttons = new List<Rectangle>();
            int spacing = 4;
            int buttonWidth = (area.Width - (buttonCount - 1) * spacing) / buttonCount;
            
            for (int i = 0; i < buttonCount; i++)
            {
                buttons.Add(new Rectangle(
                    area.Left + i * (buttonWidth + spacing),
                    area.Top,
                    buttonWidth,
                    Math.Min(32, area.Height)
                ));
            }
            
            return buttons;
        }

        private void DrawButton(Graphics g, Rectangle rect, string text, bool isSelected)
        {
            var bgColor = isSelected 
                ? (Theme?.AccentColor ?? Color.FromArgb(0, 120, 215))
                : (Theme?.ButtonBackColor ?? Color.FromArgb(240, 240, 240));
            var textColor = isSelected 
                ? Color.White
                : (Theme?.TextColor ?? Color.FromArgb(70, 70, 70));
            var borderColor = Theme?.BorderColor ?? Color.FromArgb(200, 200, 200);
            
            using (var bgBrush = new SolidBrush(bgColor))
            using (var textBrush = new SolidBrush(textColor))
            using (var borderPen = new Pen(borderColor))
            using (var font = new Font("Segoe UI", 8.5f))
            {
                g.FillRoundedRectangle(bgBrush, rect, 4);
                g.DrawRoundedRectangle(borderPen, rect, 4);
                
                var format = new StringFormat 
                { 
                    Alignment = StringAlignment.Center, 
                    LineAlignment = StringAlignment.Center 
                };
                
                g.DrawString(text, font, textBrush, rect, format);
            }
        }

        private List<string> GetSampleButtons()
        {
            return new List<string> { "Create", "Edit", "Delete", "Archive" };
        }
    }
}
