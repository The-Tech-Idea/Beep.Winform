using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Control
{
    /// <summary>
    /// ColorPicker - Color selection control
    /// </summary>
    internal sealed class ColorPickerPainter : WidgetPainterBase
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
            
            // Color picker area
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
                var titleColor = Theme?.TextColor ?? Color.FromArgb(70, 70, 70);
                using (var titleFont = new Font("Segoe UI", 9f, FontStyle.Regular))
                using (var titleBrush = new SolidBrush(titleColor))
                {
                    g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, 
                        new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                }
            }

            // Draw color picker elements
            DrawColorPalette(g, ctx);
            DrawSelectedColor(g, ctx);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Draw selection indicator on color palette
            var paletteRect = GetColorPaletteRect(ctx.ContentRect);
            var cellSize = GetColorCellSize(paletteRect);
            
            // Highlight selected color (example: red)
            var selectedRect = new Rectangle(paletteRect.Left + cellSize, paletteRect.Top, cellSize, cellSize);
            var focusColor = Theme?.AccentColor ?? Color.FromArgb(0, 120, 215);
            
            using (var focusPen = new Pen(focusColor, 2))
            {
                g.DrawRoundedRectangle(focusPen, Rectangle.Inflate(selectedRect, 2, 2), 3);
            }
        }

        private void DrawColorPalette(Graphics g, WidgetContext ctx)
        {
            var paletteRect = GetColorPaletteRect(ctx.ContentRect);
            var colors = GetStandardColors();
            var cellSize = GetColorCellSize(paletteRect);
            
            int cols = paletteRect.Width / cellSize;
            int rows = Math.Min((colors.Count + cols - 1) / cols, paletteRect.Height / cellSize);
            
            for (int i = 0; i < Math.Min(colors.Count, cols * rows); i++)
            {
                int col = i % cols;
                int row = i / cols;
                
                var colorRect = new Rectangle(
                    paletteRect.Left + col * cellSize,
                    paletteRect.Top + row * cellSize,
                    cellSize - 2,
                    cellSize - 2
                );
                
                using (var colorBrush = new SolidBrush(colors[i]))
                using (var borderPen = new Pen(Color.FromArgb(200, 200, 200)))
                {
                    g.FillRoundedRectangle(colorBrush, colorRect, 3);
                    g.DrawRoundedRectangle(borderPen, colorRect, 3);
                }
            }
        }

        private void DrawSelectedColor(Graphics g, WidgetContext ctx)
        {
            var selectedColorRect = GetSelectedColorRect(ctx.ContentRect);
            var selectedColor = Color.FromArgb(255, 100, 100); // Example selected color
            var borderColor = Theme?.BorderColor ?? Color.FromArgb(150, 150, 150);
            
            using (var colorBrush = new SolidBrush(selectedColor))
            using (var borderPen = new Pen(borderColor, 2))
            {
                g.FillRoundedRectangle(colorBrush, selectedColorRect, 4);
                g.DrawRoundedRectangle(borderPen, selectedColorRect, 4);
            }
            
            // Draw color info text
            var textRect = new Rectangle(
                selectedColorRect.Right + 10,
                selectedColorRect.Top,
                ctx.ContentRect.Right - selectedColorRect.Right - 10,
                selectedColorRect.Height
            );
            
            var colorText = $"#{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}";
            var textColor = Theme?.TextColor ?? Color.FromArgb(70, 70, 70);
            
            using (var textFont = new Font("Segoe UI", 9f))
            using (var textBrush = new SolidBrush(textColor))
            {
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(colorText, textFont, textBrush, textRect, format);
            }
        }

        private Rectangle GetColorPaletteRect(Rectangle contentRect)
        {
            return new Rectangle(
                contentRect.Left,
                contentRect.Top,
                contentRect.Width,
                Math.Max(60, contentRect.Height - 40)
            );
        }

        private Rectangle GetSelectedColorRect(Rectangle contentRect)
        {
            var paletteRect = GetColorPaletteRect(contentRect);
            return new Rectangle(
                contentRect.Left,
                paletteRect.Bottom + 8,
                40,
                24
            );
        }

        private int GetColorCellSize(Rectangle paletteRect)
        {
            return Math.Min(20, Math.Min(paletteRect.Width / 8, paletteRect.Height / 3));
        }

        private List<Color> GetStandardColors()
        {
            return new List<Color>
            {
                Color.Red, Color.Orange, Color.Yellow, Color.Green,
                Color.Blue, Color.Purple, Color.Pink, Color.Brown,
                Color.Black, Color.Gray, Color.Silver, Color.White,
                Color.DarkRed, Color.DarkOrange, Color.Gold, Color.DarkGreen,
                Color.DarkBlue, Color.DarkViolet, Color.HotPink, Color.SaddleBrown
            };
        }
    }
}
