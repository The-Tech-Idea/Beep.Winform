using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Vercel clean tree painter.
    /// Features: Ultra-minimal, monospace text, subtle borders, clean spacing.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class VercelCleanTreePainter : BaseTreePainter
    {
        private const int BorderWidth = 1;

        /// <summary>
        /// Vercel-specific node painting with ultra-minimal clean design.
        /// Features: Top accent border (2px on selection), flat backgrounds, plus/minus toggles, monospace fonts, compact spacing.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for Vercel clean appearance
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw Vercel flat background
                if (isSelected || isHovered)
                {
                    Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;
                    using (var bgBrush = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(bgBrush, nodeBounds);
                    }

                    // STEP 2: Vercel top accent border (distinctive feature)
                    if (isSelected)
                    {
                        // Top accent line (2px thick)
                        using (var accentPen = new Pen(_theme.AccentColor, 2f))
                        {
                            g.DrawLine(accentPen, 
                                nodeBounds.Left, nodeBounds.Top, 
                                nodeBounds.Right, nodeBounds.Top);
                        }
                    }
                    else if (isHovered)
                    {
                        // Subtle border on hover
                        using (var hoverPen = new Pen(Color.FromArgb(40, _theme.BorderColor), 1f))
                        {
                            g.DrawRectangle(hoverPen, nodeBounds);
                        }
                    }
                }

                // STEP 3: Draw Vercel plus/minus toggle (minimalist)
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = node.ToggleRectContent;
                    Color iconColor = _theme.TreeForeColor;

                    using (var pen = new Pen(iconColor, 1.5f))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;

                        int centerX = toggleRect.Left + toggleRect.Width / 2;
                        int centerY = toggleRect.Top + toggleRect.Height / 2;
                        int size = Math.Min(toggleRect.Width, toggleRect.Height) / 4;

                        if (node.Item.IsExpanded)
                        {
                            // Minus (horizontal line only)
                            g.DrawLine(pen, centerX - size, centerY, centerX + size, centerY);
                        }
                        else
                        {
                            // Plus (horizontal + vertical)
                            g.DrawLine(pen, centerX - size, centerY, centerX + size, centerY);
                            g.DrawLine(pen, centerX, centerY - size, centerX, centerY + size);
                        }
                    }
                }

                // STEP 4: Draw Vercel minimal checkbox
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = node.CheckRectContent;
                    var borderColor = node.Item.IsChecked ? _theme.AccentColor : _theme.BorderColor;
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

                    // Vercel square checkbox (no rounding)
                    using (var bgBrush = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(bgBrush, checkRect);
                    }

                    using (var borderPen = new Pen(borderColor, 1f))
                    {
                        g.DrawRectangle(borderPen, checkRect);
                    }

                    // Minimalist checkmark
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

                // STEP 5: Draw Vercel minimal outlined icon
                if (node.IconRectContent != Rectangle.Empty && !string.IsNullOrEmpty(node.Item.ImagePath))
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    PaintIcon(g, iconRect, node.Item.ImagePath);
                }

                // STEP 6: Draw text with Vercel monospace typography
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = node.TextRectContent;
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                    // Vercel uses monospace fonts (Consolas/Courier New)
                    using (var renderFont = new Font("Consolas", _owner.TextFont.Size, FontStyle.Regular))
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
                // Selected: flat with left accent border
                using (var brush = new SolidBrush(_theme.TreeNodeSelectedBackColor))
                {
                    g.FillRectangle(brush, nodeBounds);
                }

                // Left accent border
                using (var pen = new Pen(_theme.AccentColor, 2f))
                {
                    g.DrawLine(pen, nodeBounds.Left, nodeBounds.Top, nodeBounds.Left, nodeBounds.Bottom);
                }
            }
            else if (isHovered)
            {
                // Hover: very subtle border
                using (var hoverBrush = new SolidBrush(_theme.TreeNodeHoverBackColor))
                {
                    g.FillRectangle(hoverBrush, nodeBounds);
                }

                using (var pen = new Pen(Color.FromArgb(30, _theme.BorderColor), BorderWidth))
                {
                    g.DrawRectangle(pen, nodeBounds);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Vercel style: minimalist plus/minus
            Color iconColor = _theme.TreeForeColor;

            using (var pen = new Pen(iconColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 4;

                if (isExpanded)
                {
                    // Minus (horizontal line)
                    g.DrawLine(pen, centerX - size, centerY, centerX + size, centerY);
                }
                else
                {
                    // Plus (horizontal + vertical)
                    g.DrawLine(pen, centerX - size, centerY, centerX + size, centerY);
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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.VercelClean);
                    return;
                }
                catch { }
            }

            // Default: minimal outlined icon
            PaintDefaultVercelIcon(g, iconRect);
        }

        private void PaintDefaultVercelIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.TreeForeColor;

            // Simple outlined square
            int padding = iconRect.Width / 4;
            Rectangle innerRect = new Rectangle(
                iconRect.X + padding,
                iconRect.Y + padding,
                iconRect.Width - padding * 2,
                iconRect.Height - padding * 2);

            using (var pen = new Pen(iconColor, 1f))
            {
                g.DrawRectangle(pen, innerRect);

                // Diagonal line for "folder"
                g.DrawLine(pen, innerRect.Left, innerRect.Top, innerRect.Right, innerRect.Bottom);
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Vercel uses monospace fonts
            Font renderFont = new Font("Consolas", font.Size, FontStyle.Regular);

            TextRenderer.DrawText(g, text, renderFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            renderFont.Dispose();
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            // Ultra-clean background
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
            // Vercel compact spacing
            return Math.Max(28, base.GetPreferredRowHeight(item, font));
        }
    }
}
