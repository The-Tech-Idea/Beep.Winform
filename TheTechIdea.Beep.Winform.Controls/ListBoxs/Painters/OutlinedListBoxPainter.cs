using System.Drawing;
using System.Drawing.Drawing2D;

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

            // Create rounded path
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(itemRect, 3))
            {
                // Fill with appropriate color
                if (isSelected)
                {
                    var selColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.PrimaryColor ?? Color.LightBlue;
                    using (var brush = new SolidBrush(Color.FromArgb(20, selColor.R, selColor.G, selColor.B)))
                    {
                        g.FillPath(brush, path);
                    }

                    // Thick selection border
                    using (var pen = new Pen(selColor, 2f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else if (isHovered)
                {
                    var hoverColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.AccentColor ?? Color.FromArgb(100, 150, 200);
                    using (var brush = new SolidBrush(Color.FromArgb(10, hoverColor.R, hoverColor.G, hoverColor.B)))
                    {
                        g.FillPath(brush, path);
                    }

                    // Hover border with accent color
                    using (var pen = new Pen(hoverColor, 1.5f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    // Normal outlined style - just border
                    using (var brush = new SolidBrush(Color.White))
                    {
                        g.FillPath(brush, path);
                    }

                    using (var pen = new Pen(Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.BorderColor ?? Color.FromArgb(200, 200, 200), 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Draw subtle divider line at bottom
            using (var pen = new Pen(Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.BorderColor ?? Color.FromArgb(220, 220, 220), 0.5f))
            {
                g.DrawLine(pen, itemRect.Left + 8, itemRect.Bottom - 1, itemRect.Right - 8, itemRect.Bottom - 1);
            }
        }
        
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(12, 6, 12, 6);
        }
    }
}
