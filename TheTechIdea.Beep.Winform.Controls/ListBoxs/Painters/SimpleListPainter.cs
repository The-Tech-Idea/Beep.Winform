using System.Drawing;
using System.Drawing.Drawing2D;

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
            // Use BeepStyling for Simple Style background, border, and shadow
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, false, Style);

                // Add hover effect with subtle gradient
                if (isHovered && !isSelected)
                {
                    using (var hoverBrush = new LinearGradientBrush(itemRect, Color.FromArgb(30, Color.LightGray), Color.Transparent, LinearGradientMode.Vertical))
                    {
                        g.FillPath(hoverBrush, path);
                    }
                }
            }

            if (isSelected)
            {
                // Draw selection indicator on left
                using (var brush = new SolidBrush(Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.PrimaryColor ?? Color.Blue))
                {
                    Rectangle indicator = new Rectangle(itemRect.Left, itemRect.Top, 3, itemRect.Height);
                    g.FillRectangle(brush, indicator);
                }
            }
        }
    }
}
