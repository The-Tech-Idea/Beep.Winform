using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;


namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Chakra UI tree painter.
    /// Features: Accessible design, focus rings, balanced spacing, clean borders.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class ChakraUITreePainter : BaseTreePainter
    {
        private const int CornerRadius = 6;
        private const int FocusRingWidth = 2;

        /// <summary>
        /// Chakra UI-specific node painting with accessible focus rings.
        /// Features: Double focus ring on selection (2px), single ring on hover, accessible colors, rounded corners (6px), clean chevrons.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for Chakra UI smooth appearance
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw Chakra UI rounded background with focus ring
                if (isSelected || isHovered)
                {
                    using (var nodePath = CreateRoundedRectangle(nodeBounds, CornerRadius))
                    {
                        // Background fill
                        Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;
                        using (var bgBrush = new SolidBrush(bgColor))
                        {
                            g.FillPath(bgBrush, nodePath);
                        }

                        // STEP 2: Chakra UI focus ring (accessibility feature)
                        if (isSelected)
                        {
                            // Double focus ring on selection
                            using (var focusPen = new Pen(_theme.AccentColor, FocusRingWidth))
                            {
                                g.DrawPath(focusPen, nodePath);
                            }

                            // Outer ring for enhanced accessibility
                            var outerRect = new Rectangle(
                                nodeBounds.X - 2,
                                nodeBounds.Y - 2,
                                nodeBounds.Width + 4,
                                nodeBounds.Height + 4);

                            using (var outerPath = CreateRoundedRectangle(outerRect, CornerRadius + 2))
                            {
                                using (var outerPen = new Pen(Color.FromArgb(60, _theme.AccentColor), 1f))
                                {
                                    g.DrawPath(outerPen, outerPath);
                                }
                            }
                        }
                        else if (isHovered)
                        {
                            // Single subtle ring on hover
                            using (var hoverPen = new Pen(Color.FromArgb(100, _theme.AccentColor), 1f))
                            {
                                g.DrawPath(hoverPen, nodePath);
                            }
                        }
                    }
                }

                // STEP 3: Draw Chakra UI chevron toggle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = _owner.LayoutHelper.TransformToViewport(node.ToggleRectContent);
                    Color chevronColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;

                    using (var pen = new Pen(chevronColor, 2f))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;

                        int centerX = toggleRect.Left + toggleRect.Width / 2;
                        int centerY = toggleRect.Top + toggleRect.Height / 2;
                        int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                        if (node.Item.IsExpanded)
                        {
                            // Chevron down (smooth V shape)
                            g.DrawLine(pen, centerX - size, centerY - size / 2, centerX, centerY + size / 2);
                            g.DrawLine(pen, centerX, centerY + size / 2, centerX + size, centerY - size / 2);
                        }
                        else
                        {
                            // Chevron right (smooth > shape)
                            g.DrawLine(pen, centerX - size / 2, centerY - size, centerX + size / 2, centerY);
                            g.DrawLine(pen, centerX + size / 2, centerY, centerX - size / 2, centerY + size);
                        }
                    }
                }

                // STEP 4: Draw Chakra UI accessible checkbox
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = _owner.LayoutHelper.TransformToViewport(node.CheckRectContent);
                    var borderColor = node.Item.IsChecked ? _theme.AccentColor : 
                                      isHovered ? _theme.AccentColor : _theme.BorderColor;
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

                    // Chakra UI rounded checkbox (6px)
                    using (var checkPath = CreateRoundedRectangle(checkRect, 4))
                    {
                        using (var bgBrush = new SolidBrush(bgColor))
                        {
                            g.FillPath(bgBrush, checkPath);
                        }

                        // Border with accessible width
                        float borderWidth = node.Item.IsChecked ? 2f : 1.5f;
                        using (var borderPen = new Pen(borderColor, borderWidth))
                        {
                            g.DrawPath(borderPen, checkPath);
                        }
                    }

                    // Chakra UI checkmark
                    if (node.Item.IsChecked)
                    {
                        using (var checkPen = new Pen(Color.White, 2.5f))
                        {
                            checkPen.StartCap = LineCap.Round;
                            checkPen.EndCap = LineCap.Round;

                            var points = new Point[]
                            {
                                new Point(checkRect.X + checkRect.Width / 4, checkRect.Y + checkRect.Height / 2),
                                new Point(checkRect.X + checkRect.Width / 2 - 1, checkRect.Y + checkRect.Height * 3 / 4),
                                new Point(checkRect.X + checkRect.Width * 3 / 4 + 1, checkRect.Y + checkRect.Height / 4)
                            };
                            g.DrawLines(checkPen, points);
                        }
                    }
                }

                // STEP 5: Draw Chakra UI icon
                if (!string.IsNullOrEmpty(node.Item.ImagePath) && node.IconRectContent != Rectangle.Empty)
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    PaintIcon(g, iconRect, node.Item.ImagePath);
                }

                // STEP 6: Draw text with Chakra UI accessible typography
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = _owner.LayoutHelper.TransformToViewport(node.TextRectContent);
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                    // Chakra UI uses Inter/system fonts with good readability
                    using (var renderFont = new Font("Segoe UI", _owner.TextFont.Size, FontStyle.Regular))
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

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            if (isSelected)
            {
                // Selected: rounded with focus ring
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    using (var brush = new SolidBrush(_theme.TreeNodeSelectedBackColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Focus ring (accessibility)
                    using (var focusPen = new Pen(_theme.AccentColor, FocusRingWidth))
                    {
                        g.DrawPath(focusPen, path);
                    }
                }
            }
            else if (isHovered)
            {
                // Hover: subtle rounded background
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
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

            // Chakra UI chevron
            Color chevronColor = _theme.TreeForeColor;

            using (var pen = new Pen(chevronColor, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                if (isExpanded)
                {
                    // Chevron down
                    g.DrawLine(pen, centerX - size, centerY - size / 2, centerX, centerY + size / 2);
                    g.DrawLine(pen, centerX, centerY + size / 2, centerX + size, centerY - size / 2);
                }
                else
                {
                    // Chevron right
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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.ChakraUI);
                    return;
                }
                catch { }
            }

            // Default: Chakra-style icon
            PaintDefaultChakraIcon(g, iconRect);
        }

        private void PaintDefaultChakraIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.AccentColor;

            // Chakra uses simple rounded icons
            using (var path = CreateRoundedRectangle(iconRect, iconRect.Width / 4))
            {
                using (var brush = new SolidBrush(Color.FromArgb(80, iconColor)))
                {
                    g.FillPath(brush, path);
                }

                using (var pen = new Pen(iconColor, 1.5f))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Chakra UI uses Inter/system fonts
            Font renderFont = new Font("Segoe UI", font.Size, FontStyle.Regular);

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

            // Padding
            rect = new Rectangle(rect.X + 4, rect.Y + 2, rect.Width - 8, rect.Height - 4);

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
            // Chakra UI comfortable spacing
            return Math.Max(32, base.GetPreferredRowHeight(item, font));
        }
    }
}
