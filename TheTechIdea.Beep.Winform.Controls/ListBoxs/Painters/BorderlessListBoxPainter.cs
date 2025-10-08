using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Borderless list box with bottom border on selection
    /// </summary>
    internal class BorderlessListBoxPainter : MinimalListBoxPainter
    {
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use theme/helper backgrounds for consistency
            if (isSelected || isHovered)
            {
                Color bgColor = isSelected ? _helper.GetSelectedBackColor() : _helper.GetHoverBackColor();
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, itemRect);
                }
            }

            // Bottom border on selected
            if (isSelected)
            {
                using (var pen = new Pen(_theme?.PrimaryColor ?? _theme?.AccentColor ?? Color.Blue, 2f))
                {
                    g.DrawLine(pen, itemRect.Left + 8, itemRect.Bottom - 1, itemRect.Right - 8, itemRect.Bottom - 1);
                }
            }
        }
    }
}
