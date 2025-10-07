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
                Color bgColor = isSelected
                    ? Color.FromArgb(230, (_theme?.PrimaryColor ?? Color.Blue).R, 
                                    (_theme?.PrimaryColor ?? Color.Blue).G,
                                    (_theme?.PrimaryColor ?? Color.Blue).B)
                    : Color.FromArgb(250, 250, 250);
                
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, itemRect);
                }
            }
            
            // Draw ripple effect on selected
            if (isSelected)
            {
                using (var pen = new Pen(_theme?.PrimaryColor ?? Color.Blue, 2f))
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
