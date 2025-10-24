using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// DevExpress tree painter.
    /// Features: Professional gradients, icons with badges, focus indicators, polished appearance.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class DevExpressTreePainter : BaseTreePainter
    {
        private const int CornerRadius = 2;

        /// <summary>
        /// DevExpress-specific node painting with professional enterprise styling.
        /// Features: Professional vertical gradients, plus/minus box toggles, focus borders, gloss effects on icons, polished appearance.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for DevExpress polished appearance
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw DevExpress professional gradient background
                if (isSelected || isHovered)
                {
                    Color topColor, bottomColor;

                    if (isSelected)
                    {
                        // Selected: gradient from light to slightly darker
                        topColor = _theme.TreeNodeSelectedBackColor;
                        bottomColor = ControlPaint.Dark(_theme.TreeNodeSelectedBackColor, 0.05f);
                    }
                    else
                    {
                        // Hover: subtle gradient
                        topColor = _theme.TreeNodeHoverBackColor;
                        bottomColor = ControlPaint.Light(_theme.TreeNodeHoverBackColor, 0.02f);
                    }

                    using (var gradientBrush = new LinearGradientBrush(
                        nodeBounds,
                        topColor,
                        bottomColor,
                        LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(gradientBrush, nodeBounds);
                    }

                    // STEP 2: DevExpress focus border (professional indicator)
                    if (isSelected)
                    {
                        Rectangle focusRect = new Rectangle(
                            nodeBounds.X + 1,
                            nodeBounds.Y,
                            nodeBounds.Width - 2,
                            nodeBounds.Height - 1);

                        using (var focusPen = new Pen(_theme.AccentColor, 1f))
                        {
                            g.DrawRectangle(focusPen, focusRect);
                        }
                    }
                }

                // STEP 3: Draw DevExpress plus/minus box toggle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = node.ToggleRectContent;
                    Color boxColor = _theme.BorderColor;
                    Color signColor = _theme.TreeForeColor;

                    // Box background
                    using (var boxBrush = new SolidBrush(_theme.TreeBackColor))
                    {
                        g.FillRectangle(boxBrush, toggleRect);
                    }

                    // Box border
                    using (var borderPen = new Pen(boxColor, 1f))
                    {
                        g.DrawRectangle(borderPen, toggleRect);
                    }

                    // Plus/minus sign
                    using (var signPen = new Pen(signColor, 1.5f))
                    {
                        int centerX = toggleRect.Left + toggleRect.Width / 2;
                        int centerY = toggleRect.Top + toggleRect.Height / 2;
                        int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                        // Horizontal line (always present for both + and -)
                        g.DrawLine(signPen, centerX - size, centerY, centerX + size, centerY);

                        if (!node.Item.IsExpanded)
                        {
                            // Vertical line (only for collapsed = plus sign)
                            g.DrawLine(signPen, centerX, centerY - size, centerX, centerY + size);
                        }
                    }
                }

                // STEP 4: Draw DevExpress professional checkbox
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = node.CheckRectContent;
                    var borderColor = node.Item.IsChecked ? _theme.AccentColor : _theme.BorderColor;
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

                    // DevExpress checkbox with slight gradient
                    if (node.Item.IsChecked)
                    {
                        using (var gradientBrush = new LinearGradientBrush(
                            checkRect,
                            bgColor,
                            ControlPaint.Dark(bgColor, 0.05f),
                            LinearGradientMode.Vertical))
                        {
                            g.FillRectangle(gradientBrush, checkRect);
                        }
                    }
                    else
                    {
                        using (var bgBrush = new SolidBrush(bgColor))
                        {
                            g.FillRectangle(bgBrush, checkRect);
                        }
                    }

                    using (var borderPen = new Pen(borderColor, 1f))
                    {
                        g.DrawRectangle(borderPen, checkRect);
                    }

                    // Professional checkmark
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

                // STEP 5: Draw DevExpress icon with gloss effect
                if (node.IconRectContent != Rectangle.Empty && !string.IsNullOrEmpty(node.Item.ImagePath))
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    PaintIcon(g, iconRect, node.Item.ImagePath);
                }

                // STEP 6: Draw text with DevExpress professional typography
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = node.TextRectContent;
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                    // DevExpress uses Tahoma/Segoe UI
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
                // Selected: gradient fill
                using (var brush = new LinearGradientBrush(
                    nodeBounds,
                    _theme.TreeNodeSelectedBackColor,
                    ControlPaint.Dark(_theme.TreeNodeSelectedBackColor, 0.05f),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, nodeBounds);
                }

                // Focus border
                using (var pen = new Pen(_theme.AccentColor, 1f))
                {
                    Rectangle focusRect = new Rectangle(
                        nodeBounds.X + 1,
                        nodeBounds.Y,
                        nodeBounds.Width - 2,
                        nodeBounds.Height - 1);
                    g.DrawRectangle(pen, focusRect);
                }
            }
            else if (isHovered)
            {
                // Hover: subtle gradient
                using (var brush = new LinearGradientBrush(
                    nodeBounds,
                    _theme.TreeNodeHoverBackColor,
                    ControlPaint.Light(_theme.TreeNodeHoverBackColor, 0.02f),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, nodeBounds);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // DevExpress plus/minus box
            Color boxColor = _theme.BorderColor;
            Color signColor = _theme.TreeForeColor;

            // Box background
            using (var boxBrush = new SolidBrush(_theme.TreeBackColor))
            {
                g.FillRectangle(boxBrush, toggleRect);
            }

            // Box border
            using (var pen = new Pen(boxColor, 1f))
            {
                g.DrawRectangle(pen, toggleRect);
            }

            // Plus/minus sign
            using (var signPen = new Pen(signColor, 1.5f))
            {
                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                // Horizontal line (always present)
                g.DrawLine(signPen, centerX - size, centerY, centerX + size, centerY);

                if (!isExpanded)
                {
                    // Vertical line (only for collapsed)
                    g.DrawLine(signPen, centerX, centerY - size, centerX, centerY + size);
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

            // Default: DevExpress-Style icon with gloss
            PaintDefaultDevExpressIcon(g, iconRect);
        }

        private void PaintDefaultDevExpressIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.AccentColor;

            // Folder with gradient
            int padding = iconRect.Width / 5;
            Rectangle innerRect = new Rectangle(
                iconRect.X + padding,
                iconRect.Y + padding,
                iconRect.Width - padding * 2,
                iconRect.Height - padding * 2);

            using (var path = new GraphicsPath())
            {
                // Folder shape
                int tabWidth = innerRect.Width / 3;
                int tabHeight = innerRect.Height / 4;

                path.AddLine(innerRect.Left, innerRect.Top + tabHeight, innerRect.Left + tabWidth, innerRect.Top + tabHeight);
                path.AddLine(innerRect.Left + tabWidth, innerRect.Top + tabHeight, innerRect.Left + tabWidth + 2, innerRect.Top);
                path.AddLine(innerRect.Left + tabWidth + 2, innerRect.Top, innerRect.Right, innerRect.Top);
                path.AddLine(innerRect.Right, innerRect.Top, innerRect.Right, innerRect.Bottom);
                path.AddLine(innerRect.Right, innerRect.Bottom, innerRect.Left, innerRect.Bottom);
                path.CloseFigure();

                // Gradient fill
                using (var brush = new LinearGradientBrush(
                    innerRect,
                    iconColor,
                    ControlPaint.Dark(iconColor, 0.2f),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }

                // Gloss effect on top
                Rectangle glossRect = new Rectangle(innerRect.X, innerRect.Y, innerRect.Width, innerRect.Height / 2);
                using (var glossBrush = new SolidBrush(Color.FromArgb(40, Color.White)))
                {
                    using (var glossPath = new GraphicsPath())
                    {
                        glossPath.AddRectangle(glossRect);
                        g.FillPath(glossBrush, glossPath);
                    }
                }

                // Border
                using (var pen = new Pen(ControlPaint.Dark(iconColor, 0.3f), 1f))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // DevExpress uses Tahoma/Segoe UI
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
            // DevExpress standard spacing
            return Math.Max(24, base.GetPreferredRowHeight(item, font));
        }
    }
}
