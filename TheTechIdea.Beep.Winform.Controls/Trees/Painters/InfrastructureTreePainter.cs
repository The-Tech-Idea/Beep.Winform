using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// VMware vSphere-style infrastructure tree painter with colored tags and status indicators.
    /// Dark theme optimized for hierarchical datacenter/host/VM/resource pool structure.
    /// Features: Colored status pills, resource icons, hierarchical lines, metric badges.
    /// </summary>
    public class InfrastructureTreePainter : BaseTreePainter
    {
        private const int PillHeight = 18;
        private const int PillPadding = 6;
        private const int BadgeSize = 16;

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            Color backColor;
            if (isSelected)
            {
                // Selected: use tree-specific selected background color
                backColor = _theme.TreeNodeSelectedBackColor;
                using (var pen = new Pen(_theme.AccentColor, 1))
                {
                    g.DrawRectangle(pen, nodeBounds.X, nodeBounds.Y, nodeBounds.Width - 1, nodeBounds.Height - 1);
                }
            }
            else if (isHovered)
            {
                // Hover: use tree-specific hover color
                backColor = _theme.TreeNodeHoverBackColor;
            }
            else
            {
                return; // Transparent for normal state
            }

            using (var brush = new SolidBrush(backColor))
            {
                g.FillRectangle(brush, nodeBounds);
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Chevron style (modern infrastructure UI)
            Color chevronColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;
            using (var pen = new Pen(chevronColor, 2))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                if (isExpanded)
                {
                    // Chevron down (v)
                    g.DrawLine(pen, centerX - size, centerY - size / 2, centerX, centerY + size / 2);
                    g.DrawLine(pen, centerX, centerY + size / 2, centerX + size, centerY - size / 2);
                }
                else
                {
                    // Chevron right (>)
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
                    // Use StyledImagePainter for consistent rendering
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, _owner?.ControlStyle ?? Common.BeepControlStyle.Material3);
                }
                catch
                {
                    // Fallback to simple icon indicator
                    PaintDefaultIcon(g, iconRect);
                }
            }
            else
            {
                PaintDefaultIcon(g, iconRect);
            }
        }

        private void PaintDefaultIcon(Graphics g, Rectangle iconRect)
        {
            // Simple folder/resource icon for infrastructure
            Color iconColor = _theme.AccentColor;
            using (var brush = new SolidBrush(iconColor))
            {
                int padding = 2;
                var rect = new Rectangle(
                    iconRect.X + padding,
                    iconRect.Y + padding,
                    iconRect.Width - padding * 2,
                    iconRect.Height - padding * 2);
                g.FillRectangle(brush, rect);
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            // Use tree-specific text colors
            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            TextRenderer.DrawText(g, text, font, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            // Background fill using tree-specific theme color
            using (var brush = new SolidBrush(_theme.TreeBackColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Paint connection lines for hierarchy
            PaintHierarchyLines(g, owner);

            // Let base painter handle individual nodes
            base.Paint(g, owner, bounds);
        }

        private void PaintHierarchyLines(Graphics g, BeepTree owner)
        {
            // Draw subtle vertical connection lines showing parent-child relationships
            Color lineColor = Color.FromArgb(50, _theme.BorderColor);
            using (var pen = new Pen(lineColor, 1))
            {
                pen.DashStyle = DashStyle.Dot;
                // This would need access to layout helper to draw actual lines
                // For now, lines are drawn as part of node rendering
            }
        }

        /// <summary>
        /// Paint status tags/pills next to node (e.g., "Running", "Stopped", "Warning").
        /// This is called by the tree when rendering custom metadata.
        /// </summary>
        public void PaintStatusPill(Graphics g, Rectangle pillRect, string status, Color pillColor)
        {
            if (pillRect.Width <= 0 || pillRect.Height <= 0) return;

            // Rounded rectangle pill
            using (var path = CreateRoundedRectangle(pillRect, 3))
            {
                // Semi-transparent background
                using (var brush = new SolidBrush(Color.FromArgb(120, pillColor)))
                {
                    g.FillPath(brush, path);
                }

                // Brighter border
                using (var pen = new Pen(pillColor, 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Draw status text
            if (!string.IsNullOrEmpty(status))
            {
                using (var font = new Font("Segoe UI", 7f, FontStyle.Bold))
                {
                    TextRenderer.DrawText(g, status, font, pillRect, Color.White,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            }
        }

        /// <summary>
        /// Paint metric badge (e.g., CPU usage, memory usage).
        /// </summary>
        public void PaintMetricBadge(Graphics g, Rectangle badgeRect, string metric, Color metricColor)
        {
            if (badgeRect.Width <= 0 || badgeRect.Height <= 0) return;

            // Circular badge with metric value
            using (var brush = new SolidBrush(Color.FromArgb(180, metricColor)))
            {
                g.FillEllipse(brush, badgeRect);
            }

            if (!string.IsNullOrEmpty(metric))
            {
                using (var font = new Font("Segoe UI", 6.5f, FontStyle.Bold))
                {
                    TextRenderer.DrawText(g, metric, font, badgeRect, Color.White,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        public override int GetPreferredRowHeight(SimpleItem item, Font font)
        {
            // Infrastructure trees need more vertical space for status pills and metrics
            return Math.Max(32, base.GetPreferredRowHeight(item, font));
        }
    }
}
