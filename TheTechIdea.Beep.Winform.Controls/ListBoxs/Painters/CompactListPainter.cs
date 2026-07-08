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
                g.FillRectangle(GetBrush(Color.FromArgb(ListBoxTokens.ActiveOverlayAlpha, selColor.R, selColor.G, selColor.B)), itemRect);

                // Selection border on left side (compact style)
                g.FillRectangle(GetBrush(selColor), itemRect.Left, itemRect.Top, Scale(3), itemRect.Height);

                // Right subtle border
                g.DrawLine(GetPen(selColor, 1f), itemRect.Left + 1, itemRect.Top, itemRect.Left + 1, itemRect.Bottom);
            }
            else if (isHovered)
            {
                g.FillRectangle(GetBrush(_theme?.ListItemHoverBackColor ?? _theme?.BackgroundColor ?? Color.White), itemRect);

                g.DrawRectangle(GetPen(_theme?.AccentColor ?? Color.Gray, 1f), itemRect.X, itemRect.Y, itemRect.Width - 1, itemRect.Height - 1);
            }
            else
            {
                // Normal compact style - minimal background
                g.FillRectangle(GetBrush(_theme?.BackgroundColor ?? Color.White), itemRect);
            }
        }
    }
}
