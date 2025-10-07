using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Pill/rail style tree painter for sidebar navigation.
    /// Features: Rounded pill-shaped selection, sidebar rail appearance, compact icons.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class PillRailTreePainter : BaseTreePainter
    {
        private const int PillRadius = 20;
        private const int PillPadding = 6;

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            if (isSelected)
            {
                // Selected: full pill shape
                using (var path = CreatePillShape(nodeBounds))
                {
                    using (var brush = new SolidBrush(_theme.TreeNodeSelectedBackColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
            else if (isHovered)
            {
                // Hover: subtle pill
                using (var path = CreatePillShape(nodeBounds))
                {
                    using (var hoverBrush = new SolidBrush(_theme.TreeNodeHoverBackColor))
                    {
                        g.FillPath(hoverBrush, path);
                    }
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Simple dot indicator
            Color dotColor = _theme.AccentColor;

            int centerX = toggleRect.Left + toggleRect.Width / 2;
            int centerY = toggleRect.Top + toggleRect.Height / 2;
            int radius = Math.Min(toggleRect.Width, toggleRect.Height) / 6;

            using (var brush = new SolidBrush(dotColor))
            {
                if (isExpanded)
                {
                    // Filled dot
                    g.FillEllipse(brush, centerX - radius, centerY - radius, radius * 2, radius * 2);
                }
                else
                {
                    // Outlined dot
                    using (var pen = new Pen(dotColor, 2f))
                    {
                        g.DrawEllipse(pen, centerX - radius, centerY - radius, radius * 2, radius * 2);
                    }
                }
            }
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, string imagePath)
        {
            if (iconRect.Width <= 0 || iconRect.Height <= 0) return;

            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.PillRail);
                    return;
                }
                catch { }
            }

            // Default: circular icon
            PaintDefaultPillIcon(g, iconRect);
        }

        private void PaintDefaultPillIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.AccentColor;

            // Circular icon background
            using (var brush = new SolidBrush(Color.FromArgb(100, iconColor)))
            {
                g.FillEllipse(brush, iconRect);
            }

            // Icon symbol
            Font iconFont = new Font("Segoe UI", iconRect.Height * 0.4f, FontStyle.Bold);
            using (var textBrush = new SolidBrush(iconColor))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString("▶", iconFont, textBrush, iconRect, sf);
            }

            iconFont.Dispose();
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Pill rail uses bold text
            Font renderFont = new Font("Segoe UI", font.Size, isSelected ? FontStyle.Bold : FontStyle.Regular);

            TextRenderer.DrawText(g, text, renderFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            renderFont.Dispose();
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            // Background
            using (var brush = new SolidBrush(_theme.TreeBackColor))
            {
                g.FillRectangle(brush, bounds);
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            base.Paint(g, owner, bounds);
        }

        private GraphicsPath CreatePillShape(Rectangle rect)
        {
            var path = new GraphicsPath();

            // Pill padding
            rect = new Rectangle(rect.X + PillPadding, rect.Y + 3, rect.Width - PillPadding * 2, rect.Height - 6);

            int radius = Math.Min(rect.Height / 2, PillRadius);
            int diameter = radius * 2;

            if (rect.Width < diameter || rect.Height < diameter)
            {
                path.AddRectangle(rect);
                return path;
            }

            // Create pill (fully rounded ends)
            path.AddArc(rect.X, rect.Y, diameter, diameter, 90, 180);
            path.AddLine(rect.X + radius, rect.Y, rect.Right - radius, rect.Y);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 180);
            path.AddLine(rect.Right - radius, rect.Bottom, rect.X + radius, rect.Bottom);
            path.CloseFigure();

            return path;
        }

        public override int GetPreferredRowHeight(SimpleItem item, Font font)
        {
            // Pill rail needs comfortable height for rounded shape
            return Math.Max(36, base.GetPreferredRowHeight(item, font));
        }
    }
}
