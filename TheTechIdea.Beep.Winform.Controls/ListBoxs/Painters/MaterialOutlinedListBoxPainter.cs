using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;

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
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, false, Style);

                // Add hover effect with subtle gradient
                if (isHovered && !isSelected)
                {
                    var hover = _theme?.AccentColor ?? _theme?.PrimaryColor ?? Color.LightGray;
                    using (var hoverBrush = new LinearGradientBrush(itemRect,
                        Color.FromArgb(ListBoxTokens.HoverOverlayAlpha, hover),
                        Color.Transparent,
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(hoverBrush, path);
                    }
                }
            }

            // Draw ripple effect on selected
            if (isSelected)
            {
                using (var pen = new Pen(_theme?.PrimaryColor ?? _theme?.AccentColor ?? Color.Blue, Scale(2)))
                {
                    g.DrawLine(pen, itemRect.Left, itemRect.Top, itemRect.Left, itemRect.Bottom);
                }
            }
        }
        
        public override int GetPreferredItemHeight()
        {
            return Scale(48);
        }
    }
}
