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

        /// <summary>
        /// Activity log-specific node painting with timeline/event style.
        /// Features: Timeline dots with glow effects, vertical timeline line, chronological indicators, status colors (green/blue/red), timestamp display.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for timeline appearance
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw timeline background
                if (isSelected || isHovered)
                {
                    Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;
                    using (var bgBrush = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(bgBrush, nodeBounds);
                    }
                }

                // STEP 2: Draw timeline dot with glow (distinctive feature)
                if (node.IconRectContent != Rectangle.Empty)
                {
                    var iconRect = node.IconRectContent;
                    
                    // Determine status color (simulated - in real scenario from node.Item metadata)
                    Color dotColor = node.Item.IsExpanded ? 
                        Color.FromArgb(76, 175, 80) :  // Green for "created"
                        Color.FromArgb(33, 150, 243);   // Blue for "updated"

                    int centerX = iconRect.Left + iconRect.Width / 2;
                    int centerY = iconRect.Top + iconRect.Height / 2;
                    int dotRadius = DotSize / 2;

                    // Outer glow (PathGradientBrush for radial effect)
                    using (var glowPath = new GraphicsPath())
                    {
                        glowPath.AddEllipse(centerX - dotRadius - 2, centerY - dotRadius - 2, DotSize + 4, DotSize + 4);
                        using (var glowBrush = new PathGradientBrush(glowPath))
                        {
                            glowBrush.CenterColor = Color.FromArgb(100, dotColor);
                            glowBrush.SurroundColors = new[] { Color.FromArgb(0, dotColor) };
                            g.FillPath(glowBrush, glowPath);
                        }
                    }

                    // Solid timeline dot
                    using (var dotBrush = new SolidBrush(dotColor))
                    {
                        g.FillEllipse(dotBrush, centerX - dotRadius, centerY - dotRadius, DotSize, DotSize);
                    }

                    // White border for contrast
                    using (var borderPen = new Pen(Color.FromArgb(200, 255, 255, 255), 1.5f))
                    {
                        g.DrawEllipse(borderPen, centerX - dotRadius, centerY - dotRadius, DotSize, DotSize);
                    }
                }

                // STEP 3: Draw plus/minus toggle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = node.ToggleRectContent;
                    Color toggleColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;

                    using (var pen = new Pen(toggleColor, 1.5f))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;

                        int centerX = toggleRect.Left + toggleRect.Width / 2;
                        int centerY = toggleRect.Top + toggleRect.Height / 2;
                        int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                        // Horizontal line (always present)
                        g.DrawLine(pen, centerX - size, centerY, centerX + size, centerY);

                        if (!node.Item.IsExpanded)
                        {
                            // Vertical line (plus sign for collapsed)
                            g.DrawLine(pen, centerX, centerY - size, centerX, centerY + size);
                        }
                    }
                }

                // STEP 4: Draw activity checkbox
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = node.CheckRectContent;
                    var borderColor = node.Item.IsChecked ? _theme.AccentColor : _theme.BorderColor;
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

                    using (var bgBrush = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(bgBrush, checkRect);
                    }

                    using (var borderPen = new Pen(borderColor, 1f))
                    {
                        g.DrawRectangle(borderPen, checkRect);
                    }

                    if (node.Item.IsChecked)
                    {
                        using (var checkPen = new Pen(Color.White, 1.5f))
                        {
                            var points = new Point[]
                            {
                                new Point(checkRect.X + checkRect.Width / 4, checkRect.Y + checkRect.Height / 2),
                                new Point(checkRect.X + checkRect.Width / 2 - 1, checkRect.Y + checkRect.Height * 3 / 4),
                                new Point(checkRect.X + checkRect.Width * 3 / 4, checkRect.Y + checkRect.Height / 4)
                            };
                            g.DrawLines(checkPen, points);
                        }
                    }
                }

                // STEP 5: Draw activity text
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = node.TextRectContent;
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                    using (var renderFont = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size, FontStyle.Regular))
                    {
                        TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, renderFont, textRect, textColor,
                            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                    }
                }

                // STEP 6: Draw timestamp indicator (right side)
                if (isSelected || isHovered)
                {
                    int timestampX = nodeBounds.Right - 70;
                    int timestampY = nodeBounds.Y;
                    Rectangle timestampRect = new Rectangle(timestampX, timestampY, 65, nodeBounds.Height);

                    // Simulated timestamp (in real scenario would come from node.Item metadata)
                    string timestamp = node.Item.IsExpanded ? "2m ago" : "1h ago";
                    Color timestampColor = Color.FromArgb(150, _theme.TreeForeColor);

                    using (var timestampFont = new Font("Segoe UI", 7.5f, FontStyle.Regular))
                    {
                        TextRenderer.DrawText(g, timestamp, timestampFont, timestampRect, timestampColor,
                            TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                    }
                }
            }
            finally
            {
                g.SmoothingMode = oldSmoothing;
                g.TextRenderingHint = oldTextRendering;
            }
        }

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
