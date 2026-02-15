using System;
using System.Drawing;
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
    /// Text-only chip painter: transparent chip with hover/selected tint. Close X when selected.
    /// </summary>
    internal class TextChipGroupPainter : IChipGroupPainter
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
            float scale = DpiScalingHelper.GetDpiScaleFactor(g);
            string text = item?.Text ?? string.Empty;
            var font = ResolveFont(opt, scale);
            var sz = TextRenderer.MeasureText(g, text, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine);
            
            int pad = GetPadding(opt.Size, scale);
            int h = GetHeight(opt.Size, scale);
            return new Size(sz.Width + pad, h);
        }

        public void RenderChip(Graphics g, SimpleItem item, Rectangle bounds, ChipVisualState state, ChipRenderOptions opt, out Rectangle closeRect)
        {
            float scale = DpiScalingHelper.GetDpiScaleFactor(g);
            closeRect = Rectangle.Empty;
            var font = ResolveFont(opt, scale);
            var accent = _theme?.PrimaryColor ?? Color.RoyalBlue;
            var text = _theme?.ForeColor ?? Color.Black;
            if (state.IsSelected) text = accent;

            if (state.IsHovered || state.IsSelected)
            {
                var br = PaintersFactory.GetSolidBrush(Color.FromArgb(state.IsSelected ? 32 : 16, accent));
                g.FillRectangle(br, bounds);
            }

            var tbr = PaintersFactory.GetSolidBrush(text);
            g.DrawString(item?.Text ?? string.Empty, font, tbr, bounds, _centerFmt);

            if (opt.ShowCloseOnSelected && state.IsSelected)
            {
                int s = Math.Min(bounds.Height - DpiScalingHelper.ScaleValue(6, scale), DpiScalingHelper.ScaleValue(12, scale));
                closeRect = new Rectangle(bounds.Right - s - DpiScalingHelper.ScaleValue(4, scale), bounds.Top + (bounds.Height - s) / 2, s, s);
                var xpen = (Pen)PaintersFactory.GetPen(text, DpiScalingHelper.ScaleValue(1.5f, scale)).Clone();
                try
                {
                    xpen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    xpen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    g.DrawLine(xpen, closeRect.Left + 3, closeRect.Top + 3, closeRect.Right - 3, closeRect.Bottom - 3);
                    g.DrawLine(xpen, closeRect.Right - 3, closeRect.Top + 3, closeRect.Left + 3, closeRect.Bottom - 3);
                }
                finally
                {
                    xpen.Dispose();
                }
            }
        }

        public void RenderGroupBackground(Graphics g, Rectangle drawingRect, ChipRenderOptions options) { }
        
        private Font ResolveFont(ChipRenderOptions opt, float scale = 1.0f)
        {
             // Use ChipFontHelpers for consistent scaling if possible, or manual scaling
             return ChipFontHelpers.GetChipFont(_owner.ControlStyle, opt.Size, scale);
        }

        private int GetPadding(ChipSize size, float scale)
        {
            int val = size switch { ChipSize.Small => 12, ChipSize.Medium => 16, ChipSize.Large => 20, _ => 16 };
            return DpiScalingHelper.ScaleValue(val, scale);
        }

        private int GetHeight(ChipSize size, float scale)
        {
            int val = size switch { ChipSize.Small => 22, ChipSize.Medium => 28, ChipSize.Large => 36, _ => 28 };
            return DpiScalingHelper.ScaleValue(val, scale);
        }
    }
}
