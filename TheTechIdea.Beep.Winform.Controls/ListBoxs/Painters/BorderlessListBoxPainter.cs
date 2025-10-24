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
            // Use BeepStyling for Borderless Style background, border, and shadow
          
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, isSelected, Style);
            }
            // Bottom border on selected
            if (isSelected)
            {
                using (var pen = new Pen(Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.PrimaryColor ?? Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.AccentColor ?? Color.Blue, 2f))
                {
                    g.DrawLine(pen, itemRect.Left + 8, itemRect.Bottom - 1, itemRect.Right - 8, itemRect.Bottom - 1);
                }
            }
        }
    }
}
