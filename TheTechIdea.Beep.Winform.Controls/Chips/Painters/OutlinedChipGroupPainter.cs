using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Chips.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Chips;

namespace TheTechIdea.Beep.Winform.Controls.Chips.Painters
{
    /// <summary>
    /// Outlined chip painter (MD/Fluent Style): thin stroke, transparent surface, accent on selected.
    /// </summary>
    internal class OutlinedChipGroupPainter : IChipGroupPainter
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private readonly StringFormat _centerFmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };

        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            UpdateTheme(theme);
        }
        public void UpdateTheme(IBeepTheme theme) => _theme = theme;

        public Size MeasureChip(SimpleItem item, Graphics g, ChipRenderOptions opt)
        {
            float scale = DpiScalingHelper.GetDpiScaleFactor(_owner);
            var font = ResolveFont(opt, scale);
            string text = item?.Text ?? item?.Name ?? item?.DisplayField ?? string.Empty;
            var sz = TextRenderer.MeasureText(g, text, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine);
            return new Size(sz.Width + GetHorizontalPadding(opt.Size, scale), GetChipHeight(opt.Size, scale));
        }

        public void RenderChip(Graphics g, SimpleItem item, Rectangle bounds, ChipVisualState state, ChipRenderOptions opt, out Rectangle closeRect)
        {
            float scale = DpiScalingHelper.GetDpiScaleFactor(_owner);
            closeRect = Rectangle.Empty;
            var font = ResolveFont(opt, scale);
            var (stroke, text, hoverFill) = GetPalette(state);
            using var path = RoundedPath(bounds, DpiScalingHelper.ScaleValue(opt.CornerRadius, scale));

            // hover/selected subtle background
            if (state.IsHovered || state.IsSelected)
            {
                var bg = PaintersFactory.GetSolidBrush(hoverFill);
                g.FillPath(bg, path);
            }

            if (opt.ShowBorders)
            {
                // Ensure min width of 1 after scaling if it was intended to be visible
                var pen = (Pen)PaintersFactory.GetPen(stroke, Math.Max(1, DpiScalingHelper.ScaleValue(opt.BorderWidth, scale))).Clone();
                try
                {
                    g.DrawPath(pen, path);
                }
                finally
                {
                    pen.Dispose();
                }
            }

            var contentRect = Rectangle.Inflate(bounds, -DpiScalingHelper.ScaleValue(8, scale), -DpiScalingHelper.ScaleValue(2, scale));
            int leftPad = 0;
            if (opt.ShowSelectionCheck && state.IsSelected)
            {
                int dot = Math.Min(contentRect.Height - 6, DpiScalingHelper.ScaleValue(10, scale));
                var dotRect = new Rectangle(contentRect.Left, contentRect.Top + (contentRect.Height - dot) / 2, dot, dot);
                var dotBr = PaintersFactory.GetSolidBrush(stroke);
                g.FillEllipse(dotBr, dotRect);
                leftPad = dot + DpiScalingHelper.ScaleValue(6, scale);
            }

            int rightPad = 0;
            if (opt.ShowCloseOnSelected && state.IsSelected)
            {
                int s = Math.Min(contentRect.Height - 6, DpiScalingHelper.ScaleValue(12, scale));
                closeRect = new Rectangle(contentRect.Right - s, contentRect.Top + (contentRect.Height - s) / 2, s, s);
                var xpen = (Pen)PaintersFactory.GetPen(stroke, DpiScalingHelper.ScaleValue(1.5f, scale)).Clone();
                try
                {
                    xpen.StartCap = LineCap.Round;
                    xpen.EndCap = LineCap.Round;
                    g.DrawLine(xpen, closeRect.Left + 3, closeRect.Top + 3, closeRect.Right - 3, closeRect.Bottom - 3);
                    g.DrawLine(xpen, closeRect.Right - 3, closeRect.Top + 3, closeRect.Left + 3, closeRect.Bottom - 3);
                }
                finally
                {
                    xpen.Dispose();
                }
                rightPad = s + DpiScalingHelper.ScaleValue(4, scale);
            }

            var textRect = new Rectangle(contentRect.Left + leftPad, contentRect.Top, contentRect.Width - leftPad - rightPad, contentRect.Height);
            var tbr = PaintersFactory.GetSolidBrush(text);
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

        private static int GetChipHeight(ChipSize size, float scale) 
        {
            int val = size switch { ChipSize.Small => 24, ChipSize.Medium => 32, ChipSize.Large => 40, _ => 32 };
            return DpiScalingHelper.ScaleValue(val, scale);
        }

        private static int GetHorizontalPadding(ChipSize size, float scale)
        {
            int val = size switch { ChipSize.Small => 16, ChipSize.Medium => 20, ChipSize.Large => 24, _ => 20 };
            return DpiScalingHelper.ScaleValue(val, scale);
        }

        private Font ResolveFont(ChipRenderOptions opt, float scale)
        {
            // Prefer using ChipFontHelpers for consistent scaling
            return ChipFontHelpers.GetChipFont(_owner.ControlStyle, opt.Size, scale);
        }

        private (Color stroke, Color text, Color hover) GetPalette(ChipVisualState s)
        {
            // Use theme button colors
            Color stroke, text, hover;
            Color primary = _theme?.ButtonBorderColor ?? (_theme?.PrimaryColor ?? Color.RoyalBlue);
            
            // Override for semantic colors
            if (s.Color == ChipColor.Success) primary = Color.FromArgb(34, 197, 94);
            else if (s.Color == ChipColor.Warning) primary = Color.FromArgb(234, 179, 8);
            else if (s.Color == ChipColor.Error) primary = Color.FromArgb(239, 68, 68);
            else if (s.Color == ChipColor.Info) primary = Color.FromArgb(59, 130, 246);
            
            if (s.IsSelected)
            {
                stroke = _theme?.ButtonSelectedBorderColor ?? primary;
                text = _theme?.ButtonSelectedForeColor ?? primary;
                hover = Color.FromArgb(32, primary);
            }
            else if (s.IsHovered)
            {
                stroke = _theme?.ButtonHoverBorderColor ?? (_theme?.BorderColor ?? Color.Silver);
                text = _theme?.ButtonHoverForeColor ?? (_theme?.ForeColor ?? Color.Black);
                hover = Color.FromArgb(16, primary);
            }
            else
            {
                stroke = _theme?.ButtonBorderColor ?? (_theme?.BorderColor ?? Color.Silver);
                text = _theme?.ButtonForeColor ?? (_theme?.ForeColor ?? Color.Black);
                hover = Color.FromArgb(8, primary);
            }
            
            if (s.Color == ChipColor.Dark) { text = Color.White; }
            
            return (stroke, text, hover);
        }
    }
}
