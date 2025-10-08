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

        /// <summary>
        /// Pill/rail style tree painting for sidebar navigation.
        /// Features: Pill-shaped backgrounds (high radius rounded ends, 6px horizontal padding, 3px vertical padding), dot toggles (filled when expanded, outlined when collapsed), circular icons (100 alpha), bold text on selection.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for pill appearance
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw pill-shaped background (distinctive rounded ends)
                if (isSelected || isHovered)
                {
                    Rectangle pillBounds = new Rectangle(
                        nodeBounds.X + PillPadding,
                        nodeBounds.Y + 3,
                        nodeBounds.Width - PillPadding * 2,
                        nodeBounds.Height - 6);

                    using (var pillPath = CreatePillShape(pillBounds))
                    {
                        Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;
                        using (var bgBrush = new SolidBrush(bgColor))
                        {
                            g.FillPath(bgBrush, pillPath);
                        }
                    }
                }

                // STEP 2: Draw dot toggle indicator (filled or outlined)
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = node.ToggleRectContent;
                    Color dotColor = _theme.AccentColor;

                    int centerX = toggleRect.Left + toggleRect.Width / 2;
                    int centerY = toggleRect.Top + toggleRect.Height / 2;
                    int radius = Math.Min(toggleRect.Width, toggleRect.Height) / 6;

                    if (node.Item.IsExpanded)
                    {
                        // Filled dot when expanded
                        using (var brush = new SolidBrush(dotColor))
                        {
                            g.FillEllipse(brush, centerX - radius, centerY - radius, radius * 2, radius * 2);
                        }
                    }
                    else
                    {
                        // Outlined dot when collapsed
                        using (var pen = new Pen(dotColor, 2f))
                        {
                            g.DrawEllipse(pen, centerX - radius, centerY - radius, radius * 2, radius * 2);
                        }
                    }
                }

                // STEP 3: Draw checkbox (pill rail style with rounded corners)
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = node.CheckRectContent;
                    var borderColor = node.Item.IsChecked ? _theme.AccentColor : _theme.BorderColor;
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

                    using (var checkPath = CreateRoundedRectangle(checkRect, 4))
                    {
                        using (var bgBrush = new SolidBrush(bgColor))
                        {
                            g.FillPath(bgBrush, checkPath);
                        }

                        using (var borderPen = new Pen(borderColor, 1.5f))
                        {
                            g.DrawPath(borderPen, checkPath);
                        }
                    }

                    if (node.Item.IsChecked)
                    {
                        using (var checkPen = new Pen(Color.White, 1.5f))
                        {
                            checkPen.StartCap = LineCap.Round;
                            checkPen.EndCap = LineCap.Round;

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

                // STEP 4: Draw circular icon (pill rail sidebar style)
                if (node.IconRectContent != Rectangle.Empty)
                {
                    var iconRect = node.IconRectContent;
                    Color iconColor = _theme.AccentColor;

                    // Circular icon background
                    using (var brush = new SolidBrush(Color.FromArgb(100, iconColor)))
                    {
                        g.FillEllipse(brush, iconRect);
                    }

                    // Icon symbol
                    using (var iconFont = new Font("Segoe UI", iconRect.Height * 0.4f, FontStyle.Bold))
                    {
                        using (var textBrush = new SolidBrush(iconColor))
                        {
                            StringFormat sf = new StringFormat
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center
                            };

                            g.DrawString("▶", iconFont, textBrush, iconRect, sf);
                        }
                    }
                }

                // STEP 5: Draw text with bold on selection (pill rail typography)
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = node.TextRectContent;
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                    // Bold on selection for emphasis
                    using (var renderFont = new Font("Segoe UI", _owner.TextFont.Size, isSelected ? FontStyle.Bold : FontStyle.Regular))
                    {
                        TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, renderFont, textRect, textColor,
                            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                    }
                }
            }
            finally
            {
                g.SmoothingMode = oldSmoothing;
                g.TextRenderingHint = oldTextRendering;
            }
        }

        private GraphicsPath CreatePillShape(Rectangle rect)
        {
            var path = new GraphicsPath();

            if (rect.Width <= 0 || rect.Height <= 0)
                return path;

            int radius = Math.Min(rect.Height / 2, PillRadius);
            int diameter = radius * 2;

            if (rect.Width < diameter || rect.Height < diameter)
            {
                if (rect.Width > 0 && rect.Height > 0)
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

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            if (rect.Width < diameter || rect.Height < diameter || rect.Width <= 0 || rect.Height <= 0)
            {
                if (rect.Width > 0 && rect.Height > 0)
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

        public override int GetPreferredRowHeight(SimpleItem item, Font font)
        {
            // Pill rail needs comfortable height for rounded shape
            return Math.Max(36, base.GetPreferredRowHeight(item, font));
        }
    }
}
