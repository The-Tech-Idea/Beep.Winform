using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Outlined list box painter - clear borders with dividers and distinct outline style
    /// </summary>
    internal class OutlinedListBoxPainter : StandardListBoxPainter
    {
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;

            int radius = Scale(3);
            int inset = Scale(8);

            // Create rounded path
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(itemRect, radius))
            {
                // Fill with appropriate color
                if (isSelected)
                {
                    var selColor = _theme?.PrimaryColor ?? Color.LightBlue;
                    g.FillPath(GetBrush(Color.FromArgb(ListBoxTokens.ActiveOverlayAlpha, selColor.R, selColor.G, selColor.B)), path);

                    // Thick selection border
                    g.DrawPath(GetPen(selColor, Scale(2)), path);
                }
                else if (isHovered)
                {
                    var hoverColor = _theme?.AccentColor ?? Color.FromArgb(100, 150, 200);
                    g.FillPath(GetBrush(Color.FromArgb(ListBoxTokens.HoverOverlayAlpha, hoverColor.R, hoverColor.G, hoverColor.B)), path);

                    // Hover border with accent color
                    g.DrawPath(GetPen(hoverColor, Scale(1)), path);
                }
                else
                {
                    // Normal outlined style - just border
                    g.FillPath(GetBrush(_theme?.BackgroundColor ?? Color.White), path);

                    g.DrawPath(GetPen(_theme?.BorderColor ?? Color.FromArgb(200, 200, 200), 1f), path);
                }
            }

            // Draw subtle divider line at bottom
            g.DrawLine(GetPen(_theme?.BorderColor ?? Color.FromArgb(220, 220, 220), 1f), itemRect.Left + inset, itemRect.Bottom - 1, itemRect.Right - inset, itemRect.Bottom - 1);
        }
        
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(Scale(12), Scale(6), Scale(12), Scale(6));
        }
    }
}
