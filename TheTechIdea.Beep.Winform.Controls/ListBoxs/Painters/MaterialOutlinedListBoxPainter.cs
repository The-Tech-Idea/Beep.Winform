using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Material Design outlined list box
    /// </summary>
    internal class MaterialOutlinedListBoxPainter : OutlinedListBoxPainter
    {
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for MaterialOutlined Style background, border, and shadow
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, isSelected, Style);

                // Add hover effect with subtle gradient
                if (isHovered && !isSelected)
                {
                    using (var hoverBrush = new LinearGradientBrush(itemRect, Color.FromArgb(30, Color.LightGray), Color.Transparent, LinearGradientMode.Vertical))
                    {
                        g.FillPath(hoverBrush, path);
                    }
                }
            }

            // Draw ripple effect on selected
            if (isSelected)
            {
                using (var pen = new Pen(Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.PrimaryColor ?? Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.AccentColor ?? Color.Blue, 2f))
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
