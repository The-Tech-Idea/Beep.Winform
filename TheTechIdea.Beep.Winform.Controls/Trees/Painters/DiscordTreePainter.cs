using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Discord tree painter.
    /// Features: Server/channel style, colored indicators, rounded selections, icons.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class DiscordTreePainter : BaseTreePainter
    {
        private const int CornerRadius = 4;
        private const int IndicatorWidth = 4;

        /// <summary>
        /// Discord-specific node painting with server/channel style.
        /// Features: Left indicator pill (4px on selection, 2px on hover), rounded selections (4px), hashtag icons, bold text on selection.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for Discord appearance
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw Discord rounded background
                if (isSelected || isHovered)
                {
                    Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;

                    using (var nodePath = CreateRoundedRectangle(nodeBounds, CornerRadius))
                    {
                        using (var bgBrush = new SolidBrush(bgColor))
                        {
                            g.FillPath(bgBrush, nodePath);
                        }
                    }
                }

                // STEP 2: Draw Discord left indicator pill (distinctive feature)
                if (isSelected)
                {
                    // Full indicator pill on selection (4px wide, half height centered)
                    Rectangle indicator = new Rectangle(
                        nodeBounds.Left,
                        nodeBounds.Top + nodeBounds.Height / 4,
                        IndicatorWidth,
                        nodeBounds.Height / 2);

                    using (var indicatorBrush = new SolidBrush(_theme.AccentColor))
                    {
                        g.FillRectangle(indicatorBrush, indicator);
                    }
                }
                else if (isHovered)
                {
                    // Small indicator on hover (2px wide, third height centered)
                    Rectangle indicator = new Rectangle(
                        nodeBounds.Left,
                        nodeBounds.Top + nodeBounds.Height / 3,
                        IndicatorWidth / 2,
                        nodeBounds.Height / 3);

                    using (var indicatorBrush = new SolidBrush(Color.FromArgb(100, _theme.TreeForeColor)))
                    {
                        g.FillRectangle(indicatorBrush, indicator);
                    }
                }

                // STEP 3: Draw Discord arrow toggle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = _owner.LayoutHelper.TransformToViewport(node.ToggleRectContent);
                    Color arrowColor = _theme.TreeForeColor;

                    using (var pen = new Pen(arrowColor, 1.5f))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;

                        int centerX = toggleRect.Left + toggleRect.Width / 2;
                        int centerY = toggleRect.Top + toggleRect.Height / 2;
                        int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                        if (node.Item.IsExpanded)
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

                // STEP 4: Draw Discord checkbox
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = _owner.LayoutHelper.TransformToViewport(node.CheckRectContent);
                    var borderColor = node.Item.IsChecked ? _theme.AccentColor : _theme.BorderColor;
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

                    using (var checkPath = CreateRoundedRectangle(checkRect, 2))
                    {
                        using (var bgBrush = new SolidBrush(bgColor))
                        {
                            g.FillPath(bgBrush, checkPath);
                        }

                        using (var borderPen = new Pen(borderColor, 1f))
                        {
                            g.DrawPath(borderPen, checkPath);
                        }
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

                // STEP 5: Draw Discord hashtag icon
                if (node.IconRectContent != Rectangle.Empty)
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    if (!string.IsNullOrEmpty(node.Item.ImagePath))
                    {
                        PaintIcon(g, iconRect, node.Item.ImagePath);
                    }
                    else
                    {
                        Color iconColor = _theme.AccentColor;
                        using (var iconPath = CreateRoundedRectangle(iconRect, iconRect.Width / 4))
                        {
                            using (var bgBrush = new SolidBrush(Color.FromArgb(60, iconColor)))
                            {
                                g.FillPath(bgBrush, iconPath);
                            }

                            using (var hashFont = new Font("Segoe UI", iconRect.Height * 0.5f, FontStyle.Bold))
                            using (var textBrush = new SolidBrush(iconColor))
                            {
                                StringFormat sf = new StringFormat
                                {
                                    Alignment = StringAlignment.Center,
                                    LineAlignment = StringAlignment.Center
                                };

                                g.DrawString("#", hashFont, textBrush, iconRect, sf);
                            }
                        }
                    }
                }

                // STEP 6: Draw text with Discord typography (bold on selection)
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = _owner.LayoutHelper.TransformToViewport(node.TextRectContent);
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                    // Discord uses Whitney/Segoe UI, bold on selection
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

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            if (isSelected)
            {
                // Selected: rounded rectangle with accent indicator
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    using (var brush = new SolidBrush(_theme.TreeNodeSelectedBackColor))
                    {
                        g.FillPath(brush, path);
                    }
                }

                // Left indicator pill
                Rectangle indicator = new Rectangle(
                    nodeBounds.Left,
                    nodeBounds.Top + nodeBounds.Height / 4,
                    IndicatorWidth,
                    nodeBounds.Height / 2);

                using (var indicatorBrush = new SolidBrush(_theme.AccentColor))
                {
                    g.FillRectangle(indicatorBrush, indicator);
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

                // Small left indicator on hover
                Rectangle indicator = new Rectangle(
                    nodeBounds.Left,
                    nodeBounds.Top + nodeBounds.Height / 3,
                    IndicatorWidth / 2,
                    nodeBounds.Height / 3);

                using (var indicatorBrush = new SolidBrush(Color.FromArgb(100, _theme.TreeForeColor)))
                {
                    g.FillRectangle(indicatorBrush, indicator);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Discord style: simple arrow
            Color arrowColor = _theme.TreeForeColor;

            using (var pen = new Pen(arrowColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath);
                    return;
                }
                catch { }
            }

            // Default: Discord-style rounded icon
            PaintDefaultDiscordIcon(g, iconRect);
        }

        private void PaintDefaultDiscordIcon(Graphics g, Rectangle iconRect)
        {
            // Discord channels use hashtag or voice icons
            Color iconColor = _theme.AccentColor;

            using (var path = CreateRoundedRectangle(iconRect, iconRect.Width / 4))
            {
                // Background
                using (var bgBrush = new SolidBrush(Color.FromArgb(60, iconColor)))
                {
                    g.FillPath(bgBrush, path);
                }

                // Hashtag symbol (#)
                Font hashFont = new Font("Segoe UI", iconRect.Height * 0.5f, FontStyle.Bold);
                using (var textBrush = new SolidBrush(iconColor))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    g.DrawString("#", hashFont, textBrush, iconRect, sf);
                }

                hashFont.Dispose();
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Discord uses Whitney/Segoe UI
            Font renderFont = new Font("Segoe UI", font.Size, isSelected ? FontStyle.Bold : FontStyle.Regular);

            TextRenderer.DrawText(g, text, renderFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            renderFont.Dispose();
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            // Discord dark background
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

            // Padding (Discord has more padding on left)
            rect = new Rectangle(rect.X + 8, rect.Y + 2, rect.Width - 12, rect.Height - 4);

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
            // Discord comfortable spacing
            return Math.Max(32, base.GetPreferredRowHeight(item, font));
        }
    }
}
