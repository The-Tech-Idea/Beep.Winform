using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Minimal list box painter - subtle styling
    /// </summary>
    internal class MinimalListBoxPainter : StandardListBoxPainter
    {
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (isHovered || isSelected)
            {
                Color bgColor = isSelected 
                    ? Color.FromArgb(240, 240, 245)
                    : Color.FromArgb(250, 250, 250);
                
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, itemRect);
                }
            }
        }
        
        public override int GetPreferredItemHeight()
        {
            return 28; // Slightly smaller for minimal style
        }
    }
}
