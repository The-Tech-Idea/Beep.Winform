using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Notion minimal tree painter.
    /// Features: Clean design, subtle hover states, emoji icons, minimal spacing.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class NotionMinimalTreePainter : BaseTreePainter
    {
        private const int MinimalPadding = 2;

        /// <summary>
        /// Notion-specific node painting with minimal clean design.
        /// Features: Bottom accent line (2px on selection), flat backgrounds, simple arrow toggles, emoji-Style icons, compact spacing.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for Notion clean appearance
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw Notion flat background (very subtle)
                if (isSelected || isHovered)
                {
                    Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;
                    using (var bgBrush = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(bgBrush, nodeBounds);
                    }

                    // STEP 2: Notion bottom accent line (distinctive feature)
                    if (isSelected)
                    {
                        // Bottom accent line (2px thick)
                        using (var accentPen = new Pen(_theme.AccentColor, 2f))
                        {
                            g.DrawLine(accentPen, 
                                nodeBounds.Left, nodeBounds.Bottom - 1, 
                                nodeBounds.Right, nodeBounds.Bottom - 1);
                        }
                    }
                }

                // STEP 3: Draw Notion simple arrow toggle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = _owner.LayoutHelper.TransformToViewport(node.ToggleRectContent);
                    Color arrowColor = _theme.TreeForeColor;

                    using (var pen = new Pen(arrowColor, 1.5f))
                    {
                        int centerX = toggleRect.Left + toggleRect.Width / 2;
                        int centerY = toggleRect.Top + toggleRect.Height / 2;
                        int size = Math.Min(toggleRect.Width, toggleRect.Height) / 4;

                        if (node.Item.IsExpanded)
                        {
                            // Arrow down (simple V shape)
                            g.DrawLine(pen, centerX - size, centerY - size / 2, centerX, centerY + size / 2);
                            g.DrawLine(pen, centerX, centerY + size / 2, centerX + size, centerY - size / 2);
                        }
                        else
                        {
                            // Arrow right (simple > shape)
                            g.DrawLine(pen, centerX - size / 2, centerY - size, centerX + size / 2, centerY);
                            g.DrawLine(pen, centerX + size / 2, centerY, centerX - size / 2, centerY + size);
                        }
                    }
                }

                // STEP 4: Draw Notion minimal checkbox
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = _owner.LayoutHelper.TransformToViewport(node.CheckRectContent);
                    var borderColor = node.Item.IsChecked ? _theme.AccentColor : _theme.BorderColor;
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

                    // Notion rounded checkbox (subtle)
                    using (var bgBrush = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(bgBrush, checkRect);
                    }

                    using (var borderPen = new Pen(borderColor, 1f))
                    {
                        g.DrawRectangle(borderPen, checkRect);
                    }

                    // Simple checkmark
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

                // STEP 5: Draw Notion emoji-Style icon
                if (!string.IsNullOrEmpty(node.Item.ImagePath) && node.IconRectContent != Rectangle.Empty)
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    PaintIcon(g, iconRect, node.Item.ImagePath);
                }
                else if (node.IconRectContent != Rectangle.Empty)
                {
                    // Draw default emoji-Style icon
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    Color iconBg = Color.FromArgb(40, _theme.AccentColor);

                    using (var brush = new SolidBrush(iconBg))
                    {
                        g.FillRectangle(brush, iconRect);
                    }

                    // Simple document emoji
                    using (var iconFont = new Font("Segoe UI Emoji", iconRect.Height * 0.6f, FontStyle.Regular))
                    {
                        string iconChar = "📄";
                        using (var textBrush = new SolidBrush(_theme.TreeForeColor))
                        {
                            StringFormat sf = new StringFormat
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center
                            };
                            g.DrawString(iconChar, iconFont, textBrush, iconRect, sf);
                        }
                    }
                }

                // STEP 6: Draw text with Notion clean typography
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = _owner.LayoutHelper.TransformToViewport(node.TextRectContent);
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                    // Notion uses clean sans-serif fonts
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

            // Notion uses very subtle backgrounds
            if (isSelected)
            {
                // Selected: flat background, no borders
                using (var brush = new SolidBrush(_theme.TreeNodeSelectedBackColor))
                {
                    g.FillRectangle(brush, nodeBounds);
                }
            }
            else if (isHovered)
            {
                // Hover: extremely subtle
                using (var hoverBrush = new SolidBrush(_theme.TreeNodeHoverBackColor))
                {
                    g.FillRectangle(hoverBrush, nodeBounds);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Notion Style: simple arrow
            Color arrowColor = _theme.TreeForeColor;

            using (var pen = new Pen(arrowColor, 1.5f))
            {
                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 4;

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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.NotionMinimal);
                    return;
                }
                catch { }
            }

            // Default: emoji-Style icon (simple square with character)
            PaintDefaultNotionIcon(g, iconRect);
        }

        private void PaintDefaultNotionIcon(Graphics g, Rectangle iconRect)
        {
            // Notion icons are typically emoji or simple shapes
            Color iconBg = Color.FromArgb(40, _theme.AccentColor);

            using (var brush = new SolidBrush(iconBg))
            {
                g.FillRectangle(brush, iconRect);
            }

            // Simple document icon
            Font iconFont = new Font("Segoe UI Emoji", iconRect.Height * 0.6f, FontStyle.Regular);
            string iconChar = "📄";

            using (var textBrush = new SolidBrush(_theme.TreeForeColor))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(iconChar, iconFont, textBrush, iconRect, sf);
            }

            iconFont.Dispose();
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Notion uses clean sans-serif
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

        public override int GetPreferredRowHeight(SimpleItem item, Font font)
        {
            // Notion uses compact spacing
            return Math.Max(26, base.GetPreferredRowHeight(item, font));
        }
    }
}
