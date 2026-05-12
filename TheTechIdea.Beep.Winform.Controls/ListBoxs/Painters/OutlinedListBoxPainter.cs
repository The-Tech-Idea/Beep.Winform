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
                    using (var brush = new SolidBrush(Color.FromArgb(ListBoxTokens.ActiveOverlayAlpha, selColor.R, selColor.G, selColor.B)))
                    {
                        g.FillPath(brush, path);
                    }

                    // Thick selection border
                    using (var pen = new Pen(selColor, Scale(2)))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else if (isHovered)
                {
                    var hoverColor = _theme?.AccentColor ?? Color.FromArgb(100, 150, 200);
                    using (var brush = new SolidBrush(Color.FromArgb(ListBoxTokens.HoverOverlayAlpha, hoverColor.R, hoverColor.G, hoverColor.B)))
                    {
                        g.FillPath(brush, path);
                    }

                    // Hover border with accent color
                    using (var pen = new Pen(hoverColor, Scale(1)))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    // Normal outlined style - just border
                    using (var brush = new SolidBrush(_theme?.BackgroundColor ?? Color.White))
                    {
                        g.FillPath(brush, path);
                    }

                    using (var pen = new Pen(_theme?.BorderColor ?? Color.FromArgb(200, 200, 200), 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Draw subtle divider line at bottom
            using (var pen = new Pen(_theme?.BorderColor ?? Color.FromArgb(220, 220, 220), 1f))
            {
                g.DrawLine(pen, itemRect.Left + inset, itemRect.Bottom - 1, itemRect.Right - inset, itemRect.Bottom - 1);
            }
        }
        
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(Scale(12), Scale(6), Scale(12), Scale(6));
        }
    }
}
