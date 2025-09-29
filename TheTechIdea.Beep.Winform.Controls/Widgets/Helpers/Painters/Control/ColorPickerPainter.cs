using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Control
{
    /// <summary>
    /// ColorPicker - Color selection control with hit areas and hover states
    /// </summary>
    internal sealed class ColorPickerPainter : WidgetPainterBase
    {
        private readonly List<(Rectangle rect, Color color)> _paletteCells = new();
        private Rectangle _selectedColorRect;
        private int _selectedIndex = 0;

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int padding = 8;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -4, -4);
            
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

            // Precompute palette cells and selection rect
            _paletteCells.Clear();
            var paletteRect = GetColorPaletteRect(ctx.ContentRect);
            var colors = GetStandardColors();
            int cellSize = GetColorCellSize(paletteRect);
            int cols = Math.Max(1, paletteRect.Width / cellSize);
            int rows = Math.Max(1, paletteRect.Height / cellSize);
            int max = Math.Min(colors.Count, cols * rows);
            for (int i = 0; i < max; i++)
            {
                int col = i % cols;
                int row = i / cols;
                var cellRect = new Rectangle(
                    paletteRect.Left + col * cellSize,
                    paletteRect.Top + row * cellSize,
                    cellSize - 2,
                    cellSize - 2
                );
                _paletteCells.Add((cellRect, colors[i]));
            }

            _selectedColorRect = GetSelectedColorRect(ctx.ContentRect);
            
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
                var titleColor =  Theme?.TextBoxForeColor ?? Theme?.ForeColor ?? Color.FromArgb(70, 70, 70);
                using (var titleFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Regular))
                using (var titleBrush = new SolidBrush(titleColor))
                {
                    g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, 
                        new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
                }
            }

            // Draw color palette
            foreach (var (rect, color) in _paletteCells)
            {
                bool hovered = IsAreaHovered($"ColorPicker_Cell_{rect.X}_{rect.Y}");
                using var colorBrush = new SolidBrush(color);
                using var borderPen = new Pen(Theme?.BorderColor ?? Color.FromArgb(200, 200, 200));
                g.FillRoundedRectangle(colorBrush, rect, 3);
                g.DrawRoundedRectangle(borderPen, rect, 3);

                if (hovered)
                {
                    using var hover = new Pen(Theme?.AccentColor ?? Color.FromArgb(0, 120, 215), 1.5f);
                    g.DrawRoundedRectangle(hover, Rectangle.Inflate(rect, 2, 2), 3);
                }
            }

            // Draw selected color swatch and hex
            var selectedColor = _selectedIndex >= 0 && _selectedIndex < _paletteCells.Count ? _paletteCells[_selectedIndex].color : Color.Red;
            var border = Theme?.BorderColor ?? Color.FromArgb(150, 150, 150);
            using (var colorBrush = new SolidBrush(selectedColor))
            using (var borderPen = new Pen(border, 2))
            {
                g.FillRoundedRectangle(colorBrush, _selectedColorRect, 4);
                g.DrawRoundedRectangle(borderPen, _selectedColorRect, 4);
            }
            
            var textRect = new Rectangle(
                _selectedColorRect.Right + 10,
                _selectedColorRect.Top,
                ctx.ContentRect.Right - _selectedColorRect.Right - 10,
                _selectedColorRect.Height
            );
            var colorText = $"#{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}";
            using (var textFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 9f))
            using (var textBrush = new SolidBrush(Theme?.TextBoxForeColor ?? Theme?.ForeColor ?? Color.FromArgb(70, 70, 70)))
            {
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(colorText, textFont, textBrush, textRect, format);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Selection indicator on selected palette cell
            if (_selectedIndex >= 0 && _selectedIndex < _paletteCells.Count)
            {
                var rect = _paletteCells[_selectedIndex].rect;
                using var focusPen = new Pen(Theme?.AccentColor ?? Color.FromArgb(0, 120, 215), 2);
                g.DrawRoundedRectangle(focusPen, Rectangle.Inflate(rect, 2, 2), 3);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            
            for (int i = 0; i < _paletteCells.Count; i++)
            {
                int idx = i;
                var rect = _paletteCells[i].rect;
                string name = $"ColorPicker_Cell_{rect.X}_{rect.Y}";
                owner.AddHitArea(name, rect, null, () =>
                {
                    _selectedIndex = idx;
                    ctx.CustomData["SelectedColor"] = _paletteCells[idx].color;
                    notifyAreaHit?.Invoke(name, rect);
                    Owner?.Invalidate();
                });
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
