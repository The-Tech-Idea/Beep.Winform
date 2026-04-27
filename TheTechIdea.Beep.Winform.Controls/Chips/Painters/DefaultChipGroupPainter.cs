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
    internal class DefaultChipGroupPainter : IChipGroupPainter
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private StringFormat _centerFmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };

        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            UpdateTheme(theme);
        }

        public void UpdateTheme(IBeepTheme theme)
        {
            _theme = theme;
        }

        public Size MeasureChip(SimpleItem item, Graphics g, ChipRenderOptions opt)
        {
            float scale = DpiScalingHelper.GetDpiScaleFactor(_owner);
            string text = ResolveChipText(item);
            var font = ResolveFont(opt, scale);
            var sz = TextRenderer.MeasureText(g, text, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine);
            
            int extra = 0;
            if (opt.ShowLeadingIcon && !string.IsNullOrEmpty(item?.ImagePath))
                extra += DpiScalingHelper.ScaleSize(opt.IconMaxSize, scale).Width + DpiScalingHelper.ScaleValue(6, scale);
            
            if (opt.ShowSelectionCheck) 
                extra += DpiScalingHelper.ScaleValue(14, scale); // reserve for dot/check
            if (opt.ShowCloseOnSelected)
                extra += DpiScalingHelper.ScaleValue(16, scale);
                
            int pad = GetHorizontalPadding(opt.Size, scale);
            int h = GetChipHeight(opt.Size, scale);
            return new Size(sz.Width + pad + extra, h);
        }

        public void RenderChip(Graphics g, SimpleItem item, Rectangle bounds, ChipVisualState state, ChipRenderOptions opt, out Rectangle closeRect)
        {
            float scale = DpiScalingHelper.GetDpiScaleFactor(_owner);
            closeRect = Rectangle.Empty;
            var font = ResolveFont(opt, scale);
            var (bg, fg, border) = GetColors(state, opt);
            var groupBack = (opt.Theme ?? _theme)?.BackColor ?? _owner?.BackColor ?? Color.White;

            if (state.IsDisabled)
            {
                bg = Color.FromArgb(120, bg);
                fg = Color.FromArgb(120, fg);
                border = Color.FromArgb(120, border);
            }

            if (!state.IsSelected && bg.ToArgb() == groupBack.ToArgb())
            {
                bool isDark = (opt.Theme ?? _theme)?.IsDarkTheme == true;
                bg = ShiftLuminance(groupBack, isDark ? -0.08f : 0.12f);
                border = ShiftLuminance(border, isDark ? 0.05f : -0.08f);
            }

            if (!state.IsSelected && fg.ToArgb() == bg.ToArgb())
            {
                fg = bg.GetBrightness() > 0.55f ? Color.Black : Color.White;
            }
            using var path = RoundedPath(bounds, DpiScalingHelper.ScaleValue(opt.CornerRadius, scale));

            float originalAlpha = state.IsDisabled ? 0.6f : 1.0f;
            var oldClip = g.Clip;
            try
            {
                if (state.IsDisabled)
                {
                    var clipRegion = new Region(path);
                    g.Clip = clipRegion;
                }

                switch (opt.Style)
                {
                    case ChipStyle.Minimalist:
                        if (state.IsHovered || state.IsSelected)
                        {
                            var tint = PaintersFactory.GetSolidBrush(Color.FromArgb(state.IsSelected ? 28 : 14, fg));
                            g.FillPath(tint, path);
                        }
                        break;
                    case ChipStyle.Classic:
                    case ChipStyle.Professional:
                    case ChipStyle.HighContrast:
                        {
                            var bgBr = PaintersFactory.GetSolidBrush(Color.FromArgb(state.IsHovered || state.IsSelected ? 18 : 8, bg));
                            g.FillPath(bgBr, path);
                        }
                        break;
                    default:
                        {
                            var fillColor = state.IsSelected ? bg : Color.FromArgb(220, bg);
                            var bgBr = PaintersFactory.GetSolidBrush(fillColor);
                            g.FillPath(bgBr, path);
                        }
                        break;
                }

                if (opt.ShowBorders)
                {
                    var stroke = state.IsSelected ? border : Color.FromArgb(210, border);
                    var pen = (Pen)PaintersFactory.GetPen(stroke, Math.Max(1, DpiScalingHelper.ScaleValue(opt.BorderWidth, scale))).Clone();
                    using (pen)
                    {
                        g.DrawPath(pen, path);
                    }
                }

                var contentRect = Rectangle.Inflate(bounds, -DpiScalingHelper.ScaleValue(8, scale), -DpiScalingHelper.ScaleValue(2, scale));
                int leftPad = 0;

                if (opt.ShowSelectionCheck && state.IsSelected)
                {
                    if (opt.SelectionMark == SelectionMarkKind.Dot)
                    {
                        int dot = Math.Min(contentRect.Height - DpiScalingHelper.ScaleValue(6, scale), DpiScalingHelper.ScaleValue(10, scale));
                        var dotRect = new Rectangle(contentRect.Left, contentRect.Top + (contentRect.Height - dot) / 2, dot, dot);
                        var dotBr = PaintersFactory.GetSolidBrush(fg);
                        g.FillEllipse(dotBr, dotRect);
                        leftPad += dot + DpiScalingHelper.ScaleValue(6, scale);
                    }
                    else
                    {
                        int s = Math.Min(contentRect.Height - DpiScalingHelper.ScaleValue(6, scale), DpiScalingHelper.ScaleValue(12, scale));
                        var r = new Rectangle(contentRect.Left, contentRect.Top + (contentRect.Height - s) / 2, s, s);
                        var penC = (Pen)PaintersFactory.GetPen(fg, 2f).Clone();
                        try
                        {
                            penC.StartCap = LineCap.Round;
                            penC.EndCap = LineCap.Round;
                            g.DrawLines(penC, new[] { new Point(r.Left + 2, r.Top + s / 2), new Point(r.Left + s / 2, r.Bottom - 2), new Point(r.Right - 2, r.Top + 3) });
                        }
                        finally
                        {
                            penC.Dispose();
                        }
                        leftPad += s + DpiScalingHelper.ScaleValue(6, scale);
                    }
                }

                if (opt.ShowLeadingIcon && !string.IsNullOrEmpty(item?.ImagePath))
                {
                    var sz = DpiScalingHelper.ScaleSize(opt.IconMaxSize, scale);
                    var iconAnchorRect = new Rectangle(contentRect.Left + leftPad, contentRect.Top, sz.Width + DpiScalingHelper.ScaleValue(2, scale), contentRect.Height);
                    var iconRect = ChipIconHelpers.CalculateChipIconBounds(iconAnchorRect, opt.Size, true);
                    ChipIconHelpers.PaintIcon(
                        g,
                        iconRect,
                        item.ImagePath,
                        fg,
                        opt.Theme ?? _theme,
                        false,
                        ChipVariant.Filled,
                        state.Color,
                        state.IsSelected,
                        state.IsHovered,
                        _owner.ControlStyle);
                    leftPad += iconRect.Width + DpiScalingHelper.ScaleValue(6, scale);
                }

                int rightPad = 0;
                if (opt.ShowCloseOnSelected && state.IsSelected && !state.IsDisabled)
                {
                    int s = Math.Min(contentRect.Height - DpiScalingHelper.ScaleValue(6, scale), DpiScalingHelper.ScaleValue(12, scale));
                    closeRect = new Rectangle(contentRect.Right - s, contentRect.Top + (contentRect.Height - s) / 2, s, s);
                    var penX = (Pen)PaintersFactory.GetPen(fg, 1.5f).Clone();
                    try
                    {
                        penX.StartCap = LineCap.Round;
                        penX.EndCap = LineCap.Round;
                        g.DrawLine(penX, closeRect.Left + 3, closeRect.Top + 3, closeRect.Right - 3, closeRect.Bottom - 3);
                        g.DrawLine(penX, closeRect.Right - 3, closeRect.Top + 3, closeRect.Left + 3, closeRect.Bottom - 3);
                    }
                    finally
                    {
                        penX.Dispose();
                    }
                    rightPad = s + DpiScalingHelper.ScaleValue(4, scale);
                }

                var textRect = new Rectangle(contentRect.Left + leftPad, contentRect.Top, contentRect.Width - leftPad - rightPad, contentRect.Height);
                var textBr = PaintersFactory.GetSolidBrush(fg);
                g.DrawString(ResolveChipText(item), font, textBr, textRect, _centerFmt);
            }
            finally
            {
                g.Clip = oldClip;
            }

            if (state.IsDisabled)
            {
                using var disabledOverlay = new SolidBrush(Color.FromArgb(60, groupBack));
                g.FillPath(disabledOverlay, path);
            }
        }

        public void RenderGroupBackground(Graphics g, Rectangle drawingRect, ChipRenderOptions options) { }

        private static GraphicsPath RoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int r = Math.Max(0, Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2));
            if (r <= 0) { path.AddRectangle(rect); return path; }
            int d = r * 2;
            path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
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

        private Font ResolveFont(ChipRenderOptions opt, float scale = 1.0f)
        {
            return ChipFontHelpers.GetChipFont(_owner.ControlStyle, opt.Size, scale);
        }

        private static string ResolveChipText(SimpleItem item)
        {
            return item?.Text ?? item?.Name ?? item?.DisplayField ?? string.Empty;
        }

        private (Color bg, Color fg, Color border) GetColors(ChipVisualState s, ChipRenderOptions opt)
        {
            var th = opt.Theme ?? _theme;
            
            // Use theme button colors for all states
            Color bg, fg, border;
            
            if (s.IsSelected)
            {
                bg = th?.ButtonSelectedBackColor ?? th?.ButtonBackColor ?? Color.RoyalBlue;
                fg = th?.ButtonSelectedForeColor ?? th?.ButtonForeColor ?? Color.White;
                border = th?.ButtonSelectedBorderColor ?? th?.ButtonBorderColor ?? Color.RoyalBlue;
            }
            else if (s.IsHovered)
            {
                bg = th?.ButtonHoverBackColor ?? th?.ButtonBackColor ?? Color.LightGray;
                fg = th?.ButtonHoverForeColor ?? th?.ButtonForeColor ?? Color.Black;
                border = th?.ButtonHoverBorderColor ?? th?.ButtonBorderColor ?? Color.Gray;
            }
            else
            {
                bg = th?.ButtonBackColor ?? Color.White;
                fg = th?.ButtonForeColor ?? Color.Black;
                border = th?.ButtonBorderColor ?? Color.Silver;
            }
            
            // Override for semantic colors
            if (s.Color == ChipColor.Success) { bg = Color.FromArgb(34, 197, 94); fg = Color.White; border = bg; }
            else if (s.Color == ChipColor.Warning) { bg = Color.FromArgb(234, 179, 8); fg = Color.White; border = bg; }
            else if (s.Color == ChipColor.Error) { bg = Color.FromArgb(239, 68, 68); fg = Color.White; border = bg; }
            else if (s.Color == ChipColor.Info) { bg = Color.FromArgb(59, 130, 246); fg = Color.White; border = bg; }
            else if (s.Color == ChipColor.Dark) { bg = Color.FromArgb(51, 65, 85); fg = Color.White; border = Color.FromArgb(71, 85, 105); }
            
            return (bg, fg, border);
        }

        private static Color ShiftLuminance(Color color, float amount)
        {
            return ColorUtils.ShiftLuminance(color, amount);
        }
    }
}
