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
            if (isSelected)
            {
                Color bgColor = Color.FromArgb(240, 240, 250);
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, itemRect);
                }
                
                // Bottom border on selected
                using (var pen = new Pen(_theme?.PrimaryColor ?? Color.Blue, 2f))
                {
                    g.DrawLine(pen, itemRect.Left + 8, itemRect.Bottom - 1, itemRect.Right - 8, itemRect.Bottom - 1);
                }
            }
            else if (isHovered)
            {
                Color bgColor = Color.FromArgb(250, 250, 250);
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, itemRect);
                }
            }
        }
    }
}
