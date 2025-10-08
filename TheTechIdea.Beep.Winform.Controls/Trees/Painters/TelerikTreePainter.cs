using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Telerik tree painter.
    /// Features: Professional polish, subtle gradients, rich visuals, office-style appearance.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class TelerikTreePainter : BaseTreePainter
    {
        private const int CornerRadius = 2;

        /// <summary>
        /// Telerik-specific node painting with professional polished appearance.
        /// Features: Glass effect gradients, top highlight line, thick borders, filled triangle toggles, shiny icons with gloss.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for Telerik polished appearance
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw Telerik glass effect gradient background
                if (isSelected || isHovered)
                {
                    Color topColor, bottomColor;

                    if (isSelected)
                    {
                        // Selected: gradient with glass effect (light to darker)
                        topColor = ControlPaint.Light(_theme.TreeNodeSelectedBackColor, 0.1f);
                        bottomColor = _theme.TreeNodeSelectedBackColor;
                    }
                    else
                    {
                        // Hover: subtle glass gradient
                        topColor = ControlPaint.Light(_theme.TreeNodeHoverBackColor, 0.05f);
                        bottomColor = _theme.TreeNodeHoverBackColor;
                    }

                    using (var gradientBrush = new LinearGradientBrush(
                        nodeBounds,
                        topColor,
                        bottomColor,
                        LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(gradientBrush, nodeBounds);
                    }

                    // STEP 2: Telerik top highlight (glass/shiny effect)
                    if (isSelected)
                    {
                        using (var highlightPen = new Pen(Color.FromArgb(60, Color.White), 1f))
                        {
                            g.DrawLine(highlightPen, nodeBounds.Left, nodeBounds.Top + 1, nodeBounds.Right, nodeBounds.Top + 1);
                        }
                    }

                    // STEP 3: Telerik thick border
                    Rectangle borderRect = new Rectangle(
                        nodeBounds.X,
                        nodeBounds.Y,
                        nodeBounds.Width - 1,
                        nodeBounds.Height - 1);

                    if (isSelected)
                    {
                        using (var borderPen = new Pen(_theme.AccentColor, 1f))
                        {
                            g.DrawRectangle(borderPen, borderRect);
                        }
                    }
                    else if (isHovered)
                    {
                        using (var borderPen = new Pen(Color.FromArgb(40, _theme.BorderColor), 1f))
                        {
                            g.DrawRectangle(borderPen, borderRect);
                        }
                    }
                }

                // STEP 4: Draw Telerik filled triangle toggle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = node.ToggleRectContent;
                    Color triangleColor = _theme.TreeForeColor;

                    // Subtle hover background
                    if (isHovered)
                    {
                        using (var hoverBrush = new SolidBrush(Color.FromArgb(30, _theme.AccentColor)))
                        {
                            g.FillRectangle(hoverBrush, toggleRect);
                        }
                    }

                    // Filled triangle
                    using (var brush = new SolidBrush(triangleColor))
                    {
                        int centerX = toggleRect.Left + toggleRect.Width / 2;
                        int centerY = toggleRect.Top + toggleRect.Height / 2;
                        int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                        Point[] triangle;

                        if (node.Item.IsExpanded)
                        {
                            // Triangle pointing down
                            triangle = new Point[]
                            {
                                new Point(centerX - size, centerY - size / 2),
                                new Point(centerX + size, centerY - size / 2),
                                new Point(centerX, centerY + size / 2)
                            };
                        }
                        else
                        {
                            // Triangle pointing right
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

                // STEP 5: Draw Telerik professional checkbox
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = node.CheckRectContent;
                    var borderColor = node.Item.IsChecked ? _theme.AccentColor : _theme.BorderColor;

                    // Checkbox with gradient
                    if (node.Item.IsChecked)
                    {
                        using (var gradientBrush = new LinearGradientBrush(
                            checkRect,
                            ControlPaint.Light(_theme.AccentColor, 0.1f),
                            _theme.AccentColor,
                            LinearGradientMode.Vertical))
                        {
                            g.FillRectangle(gradientBrush, checkRect);
                        }
                    }
                    else
                    {
                        using (var bgBrush = new SolidBrush(_theme.TreeBackColor))
                        {
                            g.FillRectangle(bgBrush, checkRect);
                        }
                    }

                    using (var borderPen = new Pen(borderColor, 1f))
                    {
                        g.DrawRectangle(borderPen, checkRect);
                    }

                    // Checkmark
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

                // STEP 6: Draw Telerik shiny icon
                if (!string.IsNullOrEmpty(node.Item.ImagePath) && node.IconRectContent != Rectangle.Empty)
                {
                    PaintIcon(g, node.IconRectContent, node.Item.ImagePath);
                }

                // STEP 7: Draw text with Telerik professional typography
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = node.TextRectContent;
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                    // Telerik uses Segoe UI
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
                // Selected: gradient with glass effect
                using (var brush = new LinearGradientBrush(
                    nodeBounds,
                    ControlPaint.Light(_theme.TreeNodeSelectedBackColor, 0.1f),
                    _theme.TreeNodeSelectedBackColor,
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, nodeBounds);
                }

                // Top highlight
                using (var highlightPen = new Pen(Color.FromArgb(60, Color.White), 1f))
                {
                    g.DrawLine(highlightPen, nodeBounds.Left, nodeBounds.Top + 1, nodeBounds.Right, nodeBounds.Top + 1);
                }

                // Border
                using (var borderPen = new Pen(_theme.AccentColor, 1f))
                {
                    Rectangle borderRect = new Rectangle(
                        nodeBounds.X,
                        nodeBounds.Y,
                        nodeBounds.Width - 1,
                        nodeBounds.Height - 1);
                    g.DrawRectangle(borderPen, borderRect);
                }
            }
            else if (isHovered)
            {
                // Hover: subtle gradient
                using (var brush = new LinearGradientBrush(
                    nodeBounds,
                    ControlPaint.Light(_theme.TreeNodeHoverBackColor, 0.05f),
                    _theme.TreeNodeHoverBackColor,
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, nodeBounds);
                }

                // Subtle border
                using (var borderPen = new Pen(Color.FromArgb(40, _theme.BorderColor), 1f))
                {
                    Rectangle borderRect = new Rectangle(
                        nodeBounds.X,
                        nodeBounds.Y,
                        nodeBounds.Width - 1,
                        nodeBounds.Height - 1);
                    g.DrawRectangle(borderPen, borderRect);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Telerik triangle expand/collapse
            Color triangleColor = _theme.TreeForeColor;

            if (isHovered)
            {
                // Subtle hover background
                using (var hoverBrush = new SolidBrush(Color.FromArgb(30, _theme.AccentColor)))
                {
                    g.FillRectangle(hoverBrush, toggleRect);
                }
            }

            // Triangle
            using (var brush = new SolidBrush(triangleColor))
            {
                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                Point[] triangle;

                if (isExpanded)
                {
                    // Triangle down
                    triangle = new Point[]
                    {
                        new Point(centerX - size, centerY - size / 2),
                        new Point(centerX + size, centerY - size / 2),
                        new Point(centerX, centerY + size / 2)
                    };
                }
                else
                {
                    // Triangle right
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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath);
                    return;
                }
                catch { }
            }

            // Default: Telerik professional icon
            PaintDefaultTelerikIcon(g, iconRect);
        }

        private void PaintDefaultTelerikIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.AccentColor;

            int padding = iconRect.Width / 5;
            Rectangle innerRect = new Rectangle(
                iconRect.X + padding,
                iconRect.Y + padding,
                iconRect.Width - padding * 2,
                iconRect.Height - padding * 2);

            // Folder with professional gradient
            using (var path = new GraphicsPath())
            {
                int tabWidth = innerRect.Width / 3;
                int tabHeight = innerRect.Height / 4;

                path.AddLine(innerRect.Left, innerRect.Top + tabHeight, innerRect.Left + tabWidth, innerRect.Top + tabHeight);
                path.AddLine(innerRect.Left + tabWidth, innerRect.Top + tabHeight, innerRect.Left + tabWidth + 2, innerRect.Top);
                path.AddLine(innerRect.Left + tabWidth + 2, innerRect.Top, innerRect.Right, innerRect.Top);
                path.AddLine(innerRect.Right, innerRect.Top, innerRect.Right, innerRect.Bottom);
                path.AddLine(innerRect.Right, innerRect.Bottom, innerRect.Left, innerRect.Bottom);
                path.CloseFigure();

                // Gradient
                using (var brush = new LinearGradientBrush(
                    innerRect,
                    ControlPaint.Light(iconColor, 0.2f),
                    ControlPaint.Dark(iconColor, 0.1f),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }

                // Glass highlight
                Rectangle glassRect = new Rectangle(innerRect.X, innerRect.Y, innerRect.Width, innerRect.Height / 2);
                using (var glassBrush = new LinearGradientBrush(
                    glassRect,
                    Color.FromArgb(80, Color.White),
                    Color.FromArgb(0, Color.White),
                    LinearGradientMode.Vertical))
                {
                    using (var glassPath = new GraphicsPath())
                    {
                        glassPath.AddRectangle(glassRect);
                        g.FillPath(glassBrush, glassPath);
                    }
                }

                // Border
                using (var pen = new Pen(ControlPaint.Dark(iconColor, 0.4f), 1f))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Telerik uses Segoe UI
            Font renderFont = new Font("Segoe UI", font.Size, FontStyle.Regular);

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
            // Telerik standard spacing
            return Math.Max(26, base.GetPreferredRowHeight(item, font));
        }
    }
}
