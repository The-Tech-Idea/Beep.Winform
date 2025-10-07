using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Syncfusion tree painter.
    /// Features: Modern flat design, accent colors, clean icons, smooth transitions.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class SyncfusionTreePainter : BaseTreePainter
    {
        private const int CornerRadius = 3;
        private const int AccentBarWidth = 3;

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            if (isSelected)
            {
                // Selected: flat with accent bar
                using (var brush = new SolidBrush(_theme.TreeNodeSelectedBackColor))
                {
                    g.FillRectangle(brush, nodeBounds);
                }

                // Accent bar on left
                Rectangle accentBar = new Rectangle(
                    nodeBounds.Left,
                    nodeBounds.Top,
                    AccentBarWidth,
                    nodeBounds.Height);

                using (var accentBrush = new SolidBrush(_theme.AccentColor))
                {
                    g.FillRectangle(accentBrush, accentBar);
                }
            }
            else if (isHovered)
            {
                // Hover: subtle background
                using (var hoverBrush = new SolidBrush(_theme.TreeNodeHoverBackColor))
                {
                    g.FillRectangle(hoverBrush, nodeBounds);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Syncfusion arrow style
            Color arrowColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;

            using (var pen = new Pen(arrowColor, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                if (isExpanded)
                {
                    // Arrow down
                    g.DrawLine(pen, centerX - size, centerY - size / 2, centerX, centerY + size / 2);
                    g.DrawLine(pen, centerX, centerY + size / 2, centerX + size, centerY - size / 2);
                }
                else
                {
                    // Arrow right
                    g.DrawLine(pen, centerX - size / 2, centerY - size, centerX + size / 2, centerY);
                    g.DrawLine(pen, centerX + size / 2, centerY, centerX - size / 2, centerY + size);
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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath);
                    return;
                }
                catch { }
            }

            // Default: Syncfusion flat icon
            PaintDefaultSyncfusionIcon(g, iconRect);
        }

        private void PaintDefaultSyncfusionIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.AccentColor;

            // Flat rounded icon
            using (var path = CreateRoundedRectangle(iconRect, iconRect.Width / 5))
            {
                using (var brush = new SolidBrush(Color.FromArgb(100, iconColor)))
                {
                    g.FillPath(brush, path);
                }

                using (var pen = new Pen(iconColor, 1.5f))
                {
                    g.DrawPath(pen, path);
                }

                // Simple folder lines
                int padding = iconRect.Width / 4;
                using (var linePen = new Pen(iconColor, 1.5f))
                {
                    int midY = iconRect.Top + iconRect.Height / 2;
                    g.DrawLine(linePen,
                        iconRect.Left + padding,
                        midY,
                        iconRect.Right - padding,
                        midY);
                }
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Syncfusion uses Roboto/Segoe UI
            Font renderFont = new Font("Segoe UI", font.Size, isSelected ? FontStyle.Bold : FontStyle.Regular);

            TextRenderer.DrawText(g, text, renderFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            renderFont.Dispose();
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            // Clean background
            using (var brush = new SolidBrush(_theme.TreeBackColor))
            {
                g.FillRectangle(brush, bounds);
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            base.Paint(g, owner, bounds);
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            int padding = rect.Width / 5;
            rect = new Rectangle(
                rect.X + padding,
                rect.Y + padding,
                rect.Width - padding * 2,
                rect.Height - padding * 2);

            if (rect.Width < diameter || rect.Height < diameter)
            {
                path.AddRectangle(rect);
                return path;
            }

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        public override int GetPreferredRowHeight(SimpleItem item, Font font)
        {
            // Syncfusion comfortable spacing
            return Math.Max(28, base.GetPreferredRowHeight(item, font));
        }
    }
}
