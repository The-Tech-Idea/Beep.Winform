using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Compact list with smaller items for high-density display with distinct styling
    /// </summary>
    internal class CompactListPainter : MinimalListBoxPainter
    {
        public override int GetPreferredItemHeight()
        {
            return Scale(ListBoxTokens.ItemHeightDense);
        }
        
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            int h = Scale(ListBoxTokens.ItemPaddingH / 2);
            int v = Scale(ListBoxTokens.ItemPaddingV / 2);
            return new System.Windows.Forms.Padding(h, v, h, v);
        }

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;

            if (isSelected)
            {
                var selColor = _theme?.PrimaryColor ?? Color.LightBlue;
                
                // Filled background for selected
                using (var brush = new SolidBrush(Color.FromArgb(ListBoxTokens.ActiveOverlayAlpha, selColor.R, selColor.G, selColor.B)))
                {
                    g.FillRectangle(brush, itemRect);
                }

                // Selection border on left side (compact style)
                using (var brush = new SolidBrush(selColor))
                {
                    g.FillRectangle(brush, itemRect.Left, itemRect.Top, Scale(3), itemRect.Height);
                }

                // Right subtle border
                using (var pen = new Pen(selColor, 1f))
                {
                    g.DrawLine(pen, itemRect.Left + 1, itemRect.Top, itemRect.Left + 1, itemRect.Bottom);
                }
            }
            else if (isHovered)
            {
                using (var brush = new SolidBrush(_theme?.ListItemHoverBackColor ?? _theme?.BackgroundColor ?? Color.White))
                {
                    g.FillRectangle(brush, itemRect);
                }

                using (var pen = new Pen(_theme?.AccentColor ?? Color.Gray, 1f))
                {
                    g.DrawRectangle(pen, itemRect.X, itemRect.Y, itemRect.Width - 1, itemRect.Height - 1);
                }
            }
            else
            {
                // Normal compact style - minimal background
                using (var brush = new SolidBrush(_theme?.BackgroundColor ?? Color.White))
                {
                    g.FillRectangle(brush, itemRect);
                }
            }
        }
    }
}
