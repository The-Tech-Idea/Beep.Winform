using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Material Design outlined list box
    /// </summary>
    internal class MaterialOutlinedListBoxPainter : OutlinedListBoxPainter
    {
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (isSelected || isHovered)
            {
                Color bgColor = isSelected ? _helper.GetSelectedBackColor() : _helper.GetHoverBackColor();
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, itemRect);
                }
            }
            
            // Draw ripple effect on selected
            if (isSelected)
            {
                using (var pen = new Pen(_theme?.PrimaryColor ?? _theme?.AccentColor ?? Color.Blue, 2f))
                {
                    g.DrawLine(pen, itemRect.Left, itemRect.Top, itemRect.Left, itemRect.Bottom);
                }
            }
        }
        
        public override int GetPreferredItemHeight()
        {
            return 48; // Material Design list item height
        }
    }
}
