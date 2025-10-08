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
                Color bgColor = isSelected ? _helper.GetSelectedBackColor() : _helper.GetHoverBackColor();
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
