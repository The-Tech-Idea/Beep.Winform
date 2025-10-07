using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Simple clean category list (from image 4)
    /// Shows selection indicator on left side
    /// </summary>
    internal class SimpleListPainter : MinimalListBoxPainter
    {
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (isSelected)
            {
                // Draw selection indicator on left
                using (var brush = new SolidBrush(_theme?.PrimaryColor ?? Color.Blue))
                {
                    Rectangle indicator = new Rectangle(itemRect.Left, itemRect.Top, 3, itemRect.Height);
                    g.FillRectangle(brush, indicator);
                }
            }
            
            base.DrawItemBackground(g, itemRect, isHovered, isSelected);
        }
    }
}
