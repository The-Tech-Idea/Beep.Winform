using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Control
{
    /// <summary>
    /// CheckboxGroup - Group of related checkboxes with hit areas and hover states
    /// </summary>
    internal sealed class CheckboxGroupPainter : WidgetPainterBase
    {
        private readonly List<Rectangle> _checkboxRects = new();
        private List<string> _checkboxTexts = new();

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int padding = 8;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -4, -4);
            
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

            // Precompute checkbox rects
            _checkboxTexts = GetSampleCheckboxes();
            _checkboxRects.Clear();
            _checkboxRects.AddRange(CalculateCheckboxLayout(ctx.ContentRect, _checkboxTexts.Count));
            
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
                var titleColor = Theme?.TextBoxForeColor ?? Theme?.ForeColor ?? Color.FromArgb(70, 70, 70);
                using (var titleFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 9.5f, FontStyle.Bold))
                using (var titleBrush = new SolidBrush(titleColor))
                {
                    g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, 
                        new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                }
            }

            // Draw checkboxes
            for (int i = 0; i < _checkboxTexts.Count && i < _checkboxRects.Count; i++)
            {
                bool isChecked = i < ctx.CheckedItems.Count && ctx.CheckedItems[i];
                bool hovered = IsAreaHovered($"CheckboxGroup_Item_{i}");
                DrawCheckbox(g, _checkboxRects[i], _checkboxTexts[i], isChecked, hovered);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: additional accents can go here
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            for (int i = 0; i < _checkboxRects.Count; i++)
            {
                int idx = i;
                var rect = _checkboxRects[i];
                if (rect.IsEmpty) continue;
                owner.AddHitArea($"CheckboxGroup_Item_{idx}", rect, null, () =>
                {
                    while (ctx.CheckedItems.Count <= idx)
                    {
                        ctx.CheckedItems.Add(false);
                    }
                    ctx.CheckedItems[idx] = !ctx.CheckedItems[idx];
                    notifyAreaHit?.Invoke($"CheckboxGroup_Item_{idx}", rect);
                    Owner?.Invalidate();
                });
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

        private void DrawCheckbox(Graphics g, Rectangle rect, string text, bool isChecked, bool hovered)
        {
            int checkSize = 16;
            var checkRect = new Rectangle(rect.Left, rect.Top + (rect.Height - checkSize) / 2, checkSize, checkSize);
            var textRect = new Rectangle(checkRect.Right + 8, rect.Top, rect.Width - checkSize - 8, rect.Height);
            
            // Draw hover background
            if (hovered)
            {
                using var hover = new SolidBrush(Color.FromArgb(8, Theme?.PrimaryColor ?? Color.Blue));
                g.FillRoundedRectangle(hover, Rectangle.Inflate(rect, -2, -2), 3);
            }

            // Draw checkbox background
            var checkBgColor = isChecked 
                ? (Theme?.AccentColor ?? Color.FromArgb(0, 120, 215))
                : Theme?.BackColor ?? Color.White;
            var checkBorderColor = hovered ? (Theme?.AccentColor ?? Color.FromArgb(0, 120, 215)) : (Theme?.BorderColor ?? Color.FromArgb(150, 150, 150));
            
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
            var textColor =  Theme?.TextBoxForeColor ?? Theme?.ForeColor ?? Color.FromArgb(70, 70, 70);
            using (var textFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8.5f))
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
