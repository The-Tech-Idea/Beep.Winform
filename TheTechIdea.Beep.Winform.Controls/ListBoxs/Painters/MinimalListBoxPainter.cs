using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Minimal list box painter - subtle styling with understated borders and backgrounds
    /// </summary>
    internal class MinimalListBoxPainter : StandardListBoxPainter
    {
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;

            // Minimal: flat background with subtle hover
            if (isSelected)
            {
                // Subtle selection highlight
                using (var brush = new SolidBrush(Color.FromArgb(25, _theme?.PrimaryColor ?? Color.LightBlue)))
                {
                    g.FillRectangle(brush, itemRect);
                }

                // Very subtle selection indicator on left
                using (var brush = new SolidBrush(_theme?.PrimaryColor ?? Color.Blue))
                {
                    g.FillRectangle(brush, itemRect.Left, itemRect.Top, 2, itemRect.Height);
                }
            }
            else if (isHovered)
            {
                // Very subtle hover background
                using (var brush = new SolidBrush(Color.FromArgb(8, Color.Black)))
                {
                    g.FillRectangle(brush, itemRect);
                }
            }
            // No background for normal state - minimal approach
        }
        
        public override int GetPreferredItemHeight()
        {
            return 28; // Slightly smaller for minimal Style
        }
    }
}
