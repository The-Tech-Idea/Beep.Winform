using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Jira/Atlassian-style portfolio tree painter for project management.
    /// Features: Progress bars, effort indicators, theme grouping, epic/story hierarchy.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class PortfolioTreePainter : BaseTreePainter
    {
        private const int ProgressBarHeight = 4;
        private const int ProgressBarWidth = 60;
        private const int BadgeSize = 18;

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            if (isSelected)
            {
                // Selected: subtle rounded rectangle
                Color selectedColor = _theme.TreeNodeSelectedBackColor;
                using (var path = CreateRoundedRectangle(nodeBounds, 4))
                {
                    using (var brush = new SolidBrush(selectedColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Left accent bar
                    using (var pen = new Pen(_theme.AccentColor, 3))
                    {
                        g.DrawLine(pen, nodeBounds.X + 1, nodeBounds.Y + 2, nodeBounds.X + 1, nodeBounds.Bottom - 2);
                    }
                }
            }
            else if (isHovered)
            {
                Color hoverColor = _theme.TreeNodeHoverBackColor;
                using (var path = CreateRoundedRectangle(nodeBounds, 4))
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Atlassian-style arrow toggle
            Color arrowColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;
            using (var brush = new SolidBrush(arrowColor))
            {
                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                Point[] triangle;
                if (isExpanded)
                {
                    // Down arrow
                    triangle = new Point[]
                    {
                        new Point(centerX - size, centerY - size / 2),
                        new Point(centerX + size, centerY - size / 2),
                        new Point(centerX, centerY + size / 2)
                    };
                }
                else
                {
                    // Right arrow
                    triangle = new Point[]
                    {
                        new Point(centerX - size / 2, centerY - size),
                        new Point(centerX + size / 2, centerY),
                        new Point(centerX - size / 2, centerY + size)
                    };
                }

                g.FillPolygon(brush, triangle);
            }
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, string imagePath)
        {
            if (iconRect.Width <= 0 || iconRect.Height <= 0) return;

            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, _owner?.ControlStyle ?? Common.BeepControlStyle.Material3);
                    return;
                }
                catch { }
            }

            // Default epic/story icon
            PaintDefaultIcon(g, iconRect);
        }

        private void PaintDefaultIcon(Graphics g, Rectangle iconRect)
        {
            // Rounded square icon (Jira style)
            Color iconColor = Color.FromArgb(
                (_theme.AccentColor.R + _theme.TreeForeColor.R) / 2,
                (_theme.AccentColor.G + _theme.TreeForeColor.G) / 2,
                (_theme.AccentColor.B + _theme.TreeForeColor.B) / 2);

            using (var path = CreateRoundedRectangle(iconRect, 3))
            using (var brush = new SolidBrush(Color.FromArgb(100, iconColor)))
            {
                g.FillPath(brush, path);
            }

            using (var pen = new Pen(iconColor, 1.5f))
            using (var path = CreateRoundedRectangle(iconRect, 3))
            {
                g.DrawPath(pen, path);
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            TextRenderer.DrawText(g, text, font, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
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

        /// <summary>
        /// Paint progress bar for task completion (0-100).
        /// </summary>
        public void PaintProgressBar(Graphics g, Rectangle barRect, int progress)
        {
            if (barRect.Width <= 0 || barRect.Height <= 0) return;

            progress = Math.Max(0, Math.Min(100, progress));

            // Background track
            Color trackColor = Color.FromArgb(50, _theme.TreeForeColor);
            using (var brush = new SolidBrush(trackColor))
            using (var path = CreateRoundedRectangle(barRect, 2))
            {
                g.FillPath(brush, path);
            }

            // Progress fill
            if (progress > 0)
            {
                int fillWidth = (int)(barRect.Width * (progress / 100f));
                Rectangle fillRect = new Rectangle(barRect.X, barRect.Y, fillWidth, barRect.Height);

                Color fillColor = progress >= 100 ? Color.FromArgb(76, 175, 80) : _theme.AccentColor;
                using (var brush = new SolidBrush(fillColor))
                using (var path = CreateRoundedRectangle(fillRect, 2))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        /// <summary>
        /// Paint effort/story points badge.
        /// </summary>
        public void PaintEffortBadge(Graphics g, Rectangle badgeRect, string points)
        {
            if (badgeRect.Width <= 0 || badgeRect.Height <= 0) return;

            // Circular badge
            Color badgeColor = Color.FromArgb(150, _theme.AccentColor);
            using (var brush = new SolidBrush(badgeColor))
            {
                g.FillEllipse(brush, badgeRect);
            }

            // Border
            using (var pen = new Pen(_theme.AccentColor, 1))
            {
                g.DrawEllipse(pen, badgeRect);
            }

            // Text
            if (!string.IsNullOrEmpty(points))
            {
                using (var font = new Font("Segoe UI", 7f, FontStyle.Bold))
                {
                    TextRenderer.DrawText(g, points, font, badgeRect, _theme.TreeNodeSelectedForeColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

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
            // Portfolio trees need space for progress bars and badges
            return Math.Max(30, base.GetPreferredRowHeight(item, font));
        }
    }
}
