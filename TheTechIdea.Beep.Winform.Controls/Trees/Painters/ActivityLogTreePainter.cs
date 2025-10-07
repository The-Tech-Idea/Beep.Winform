using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Activity log/timeline tree painter for chronological events.
    /// Features: Timeline indicators, status icons, timestamps, expandable event details.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class ActivityLogTreePainter : BaseTreePainter
    {
        private const int TimelineWidth = 3;
        private const int DotSize = 10;
        private const int TimelinePadding = 20;

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            if (isSelected)
            {
                // Selected: subtle highlight
                Color selectedColor = _theme.TreeNodeSelectedBackColor;
                using (var brush = new SolidBrush(selectedColor))
                {
                    g.FillRectangle(brush, nodeBounds);
                }
            }
            else if (isHovered)
            {
                Color hoverColor = _theme.TreeNodeHoverBackColor;
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillRectangle(brush, nodeBounds);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Plus/minus toggle for activity groups
            Color toggleColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;
            using (var pen = new Pen(toggleColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                // Horizontal line
                g.DrawLine(pen, centerX - size, centerY, centerX + size, centerY);

                if (!isExpanded)
                {
                    // Vertical line (plus sign)
                    g.DrawLine(pen, centerX, centerY - size, centerX, centerY + size);
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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, _owner?.ControlStyle ?? Common.BeepControlStyle.Material3);
                    return;
                }
                catch { }
            }

            // Default timeline dot
            PaintTimelineDot(g, iconRect, _theme.AccentColor);
        }

        private void PaintTimelineDot(Graphics g, Rectangle iconRect, Color dotColor)
        {
            // Timeline dot with glow
            int centerX = iconRect.Left + iconRect.Width / 2;
            int centerY = iconRect.Top + iconRect.Height / 2;
            int dotRadius = DotSize / 2;

            // Outer glow
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(centerX - dotRadius - 2, centerY - dotRadius - 2, DotSize + 4, DotSize + 4);
                using (var brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.FromArgb(100, dotColor);
                    brush.SurroundColors = new[] { Color.FromArgb(0, dotColor) };
                    g.FillPath(brush, path);
                }
            }

            // Solid dot
            using (var brush = new SolidBrush(dotColor))
            {
                g.FillEllipse(brush, centerX - dotRadius, centerY - dotRadius, DotSize, DotSize);
            }

            // Border
            using (var pen = new Pen(Color.FromArgb(200, 255, 255, 255), 1.5f))
            {
                g.DrawEllipse(pen, centerX - dotRadius, centerY - dotRadius, DotSize, DotSize);
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

            // Paint timeline line
            PaintTimelineLine(g, bounds);

            base.Paint(g, owner, bounds);
        }

        private void PaintTimelineLine(Graphics g, Rectangle bounds)
        {
            // Vertical timeline line on the left
            Color lineColor = Color.FromArgb(80, _theme.BorderColor);
            using (var pen = new Pen(lineColor, TimelineWidth))
            {
                int x = bounds.X + TimelinePadding;
                g.DrawLine(pen, x, bounds.Y, x, bounds.Bottom);
            }
        }

        /// <summary>
        /// Paint timestamp badge for activity event.
        /// </summary>
        public void PaintTimestamp(Graphics g, Rectangle timestampRect, string timestamp)
        {
            if (timestampRect.Width <= 0 || timestampRect.Height <= 0) return;

            Color timestampColor = Color.FromArgb(150, _theme.TreeForeColor);
            using (var font = new Font("Segoe UI", 7.5f, FontStyle.Regular))
            {
                TextRenderer.DrawText(g, timestamp, font, timestampRect, timestampColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        /// <summary>
        /// Paint status icon for activity type (created, updated, deleted, etc.).
        /// </summary>
        public void PaintStatusIcon(Graphics g, Rectangle iconRect, string activityType)
        {
            if (iconRect.Width <= 0 || iconRect.Height <= 0) return;

            Color statusColor = GetStatusColor(activityType);
            PaintTimelineDot(g, iconRect, statusColor);
        }

        private Color GetStatusColor(string activityType)
        {
            // Map activity types to colors (can be customized)
            switch (activityType?.ToLower())
            {
                case "created":
                case "added":
                    return Color.FromArgb(76, 175, 80); // Green
                case "updated":
                case "modified":
                    return Color.FromArgb(33, 150, 243); // Blue
                case "deleted":
                case "removed":
                    return Color.FromArgb(244, 67, 54); // Red
                case "warning":
                    return Color.FromArgb(255, 152, 0); // Orange
                default:
                    return _theme.AccentColor;
            }
        }

        public override int GetPreferredRowHeight(SimpleItem item, Font font)
        {
            // Activity log needs comfortable spacing for timestamps
            return Math.Max(32, base.GetPreferredRowHeight(item, font));
        }
    }
}
