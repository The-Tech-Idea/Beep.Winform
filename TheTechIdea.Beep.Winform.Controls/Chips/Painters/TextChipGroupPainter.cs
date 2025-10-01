using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Chips.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Chips.Painters
{
    /// <summary>
    /// Text-only chip painter: transparent chip with hover/selected tint. Close X when selected.
    /// </summary>
    internal class TextChipGroupPainter : IChipGroupPainter
    {
        private IBeepTheme _theme;
        private readonly StringFormat _centerFmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
        public void Initialize(BaseControl owner, IBeepTheme theme) => UpdateTheme(theme);
        public void UpdateTheme(IBeepTheme theme) => _theme = theme;

        public Size MeasureChip(SimpleItem item, Graphics g, ChipRenderOptions opt)
        {
            string text = item?.Text ?? string.Empty;
            var font = ResolveFont(opt);
            var sz = TextRenderer.MeasureText(g, text, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine);
            int pad = opt.Size switch { ChipSize.Small => 12, ChipSize.Medium => 16, ChipSize.Large => 20, _ => 16 };
            int h = opt.Size switch { ChipSize.Small => 22, ChipSize.Medium => 28, ChipSize.Large => 36, _ => 28 };
            return new Size(sz.Width + pad, h);
        }

        public void RenderChip(Graphics g, SimpleItem item, Rectangle bounds, ChipVisualState state, ChipRenderOptions opt, out Rectangle closeRect)
        {
            closeRect = Rectangle.Empty;
            var font = ResolveFont(opt);
            var accent = _theme?.PrimaryColor ?? Color.RoyalBlue;
            var text = _theme?.ForeColor ?? Color.Black;
            if (state.IsSelected) text = accent;

            if (state.IsHovered || state.IsSelected)
            {
                using var br = new SolidBrush(Color.FromArgb(state.IsSelected ? 32 : 16, accent));
                g.FillRectangle(br, bounds);
            }

            using var tbr = new SolidBrush(text);
            g.DrawString(item?.Text ?? string.Empty, font, tbr, bounds, _centerFmt);

            if (opt.ShowCloseOnSelected && state.IsSelected)
            {
                int s = Math.Min(bounds.Height - 6, 12);
                closeRect = new Rectangle(bounds.Right - s - 4, bounds.Top + (bounds.Height - s) / 2, s, s);
                using var xpen = new Pen(text, 1.5f) { StartCap = System.Drawing.Drawing2D.LineCap.Round, EndCap = System.Drawing.Drawing2D.LineCap.Round };
                g.DrawLine(xpen, closeRect.Left + 3, closeRect.Top + 3, closeRect.Right - 3, closeRect.Bottom - 3);
                g.DrawLine(xpen, closeRect.Right - 3, closeRect.Top + 3, closeRect.Left + 3, closeRect.Bottom - 3);
            }
        }

        public void RenderGroupBackground(Graphics g, Rectangle drawingRect, ChipRenderOptions options) { }
        private static Font ResolveFont(ChipRenderOptions opt) => (opt.Size == ChipSize.Small) ? new Font(opt.Font.FontFamily, Math.Max(6f, opt.Font.Size * 0.9f), opt.Font.Style) : opt.Font;
    }
}
