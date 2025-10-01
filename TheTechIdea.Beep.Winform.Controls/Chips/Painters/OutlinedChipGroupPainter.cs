using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Chips.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Chips.Painters
{
    /// <summary>
    /// Outlined chip painter (MD/Fluent style): thin stroke, transparent surface, accent on selected.
    /// </summary>
    internal class OutlinedChipGroupPainter : IChipGroupPainter
    {
        private IBeepTheme _theme;
        private readonly StringFormat _centerFmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };

        public void Initialize(BaseControl owner, IBeepTheme theme) => UpdateTheme(theme);
        public void UpdateTheme(IBeepTheme theme) => _theme = theme;

        public Size MeasureChip(SimpleItem item, Graphics g, ChipRenderOptions opt)
        {
            var font = ResolveFont(opt);
            string text = item?.Text ?? item?.Name ?? item?.DisplayField ?? string.Empty;
            var sz = TextRenderer.MeasureText(g, text, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine);
            return new Size(sz.Width + GetHorizontalPadding(opt.Size), GetChipHeight(opt.Size));
        }

        public void RenderChip(Graphics g, SimpleItem item, Rectangle bounds, ChipVisualState state, ChipRenderOptions opt, out Rectangle closeRect)
        {
            closeRect = Rectangle.Empty;
            var font = ResolveFont(opt);
            var (stroke, text, hoverFill) = GetPalette(state);
            using var path = RoundedPath(bounds, opt.CornerRadius);

            // hover/selected subtle background
            if (state.IsHovered || state.IsSelected)
            {
                using var bg = new SolidBrush(hoverFill);
                g.FillPath(bg, path);
            }

            if (opt.ShowBorders)
            {
                using var pen = new Pen(stroke, Math.Max(1, opt.BorderWidth));
                g.DrawPath(pen, path);
            }

            var contentRect = Rectangle.Inflate(bounds, -8, -2);
            int leftPad = 0;
            if (opt.ShowSelectionCheck && state.IsSelected)
            {
                int dot = Math.Min(contentRect.Height - 6, 10);
                var dotRect = new Rectangle(contentRect.Left, contentRect.Top + (contentRect.Height - dot) / 2, dot, dot);
                using var dotBr = new SolidBrush(stroke);
                g.FillEllipse(dotBr, dotRect);
                leftPad = dot + 6;
            }

            int rightPad = 0;
            if (opt.ShowCloseOnSelected && state.IsSelected)
            {
                int s = Math.Min(contentRect.Height - 6, 12);
                closeRect = new Rectangle(contentRect.Right - s, contentRect.Top + (contentRect.Height - s) / 2, s, s);
                using var xpen = new Pen(stroke, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
                g.DrawLine(xpen, closeRect.Left + 3, closeRect.Top + 3, closeRect.Right - 3, closeRect.Bottom - 3);
                g.DrawLine(xpen, closeRect.Right - 3, closeRect.Top + 3, closeRect.Left + 3, closeRect.Bottom - 3);
                rightPad = s + 4;
            }

            var textRect = new Rectangle(contentRect.Left + leftPad, contentRect.Top, contentRect.Width - leftPad - rightPad, contentRect.Height);
            using var tbr = new SolidBrush(text);
            g.DrawString(item?.Text ?? string.Empty, font, tbr, textRect, _centerFmt);
        }

        public void RenderGroupBackground(Graphics g, Rectangle drawingRect, ChipRenderOptions options) { }

        private static GraphicsPath RoundedPath(Rectangle rect, int radius)
        {
            var p = new GraphicsPath(); int r = Math.Max(0, Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2));
            if (r <= 0) { p.AddRectangle(rect); return p; } int d = r * 2;
            p.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            p.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            p.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            p.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
            p.CloseFigure(); return p;
        }
        private static int GetChipHeight(ChipSize size) => size switch { ChipSize.Small => 24, ChipSize.Medium => 32, ChipSize.Large => 40, _ => 32 };
        private static int GetHorizontalPadding(ChipSize size) => size switch { ChipSize.Small => 16, ChipSize.Medium => 20, ChipSize.Large => 24, _ => 20 };
        private static Font ResolveFont(ChipRenderOptions opt) => (opt.Size == ChipSize.Small) ? new Font(opt.Font.FontFamily, Math.Max(6f, opt.Font.Size * 0.9f), opt.Font.Style) : opt.Font;

        private (Color stroke, Color text, Color hover) GetPalette(ChipVisualState s)
        {
            Color primary = _theme?.PrimaryColor ?? Color.RoyalBlue;
            Color text = _theme?.ForeColor ?? Color.Black;
            if (s.Color == ChipColor.Success) primary = Color.FromArgb(34, 197, 94);
            else if (s.Color == ChipColor.Warning) primary = Color.FromArgb(234, 179, 8);
            else if (s.Color == ChipColor.Error) primary = Color.FromArgb(239, 68, 68);
            else if (s.Color == ChipColor.Info) primary = Color.FromArgb(59, 130, 246);
            else if (s.Color == ChipColor.Dark) { text = Color.White; }
            var stroke = s.IsSelected ? primary : (_theme?.BorderColor ?? Color.Silver);
            var hover = Color.FromArgb(s.IsSelected ? 32 : 16, primary);
            var txt = s.IsSelected ? primary : text;
            return (stroke, txt, hover);
        }
    }
}
