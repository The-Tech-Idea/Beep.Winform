using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Figma Layers panel card Style tree painter.
    /// Features: Design tool hierarchy, card-based layers, drag handles, visibility toggles.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class FigmaCardTreePainter : BaseTreePainter
    {
        private const int CornerRadius = 4;
        private const int CardPadding = 4;
        private const int HandleWidth = 16;

        /// <summary>
        /// Figma Layers panel card Style painting.
        /// Features: Card with accent border (2px on selection), drag handles on hover (3 lines), layer visibility toggle, component/frame icons.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for Figma appearance
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw Figma card background
                Rectangle cardBounds = new Rectangle(
                    nodeBounds.X + CardPadding,
                    nodeBounds.Y + 2,
                    nodeBounds.Width - CardPadding * 2,
                    nodeBounds.Height - 4);

                if (isSelected || isHovered)
                {
                    using (var cardPath = CreateRoundedRectangle(cardBounds, CornerRadius))
                    {
                        // Fill background
                        Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;
                        using (var bgBrush = new SolidBrush(bgColor))
                        {
                            g.FillPath(bgBrush, cardPath);

                            // Border (2px accent on selection, 1px on hover)
                            if (isSelected)
                            {
                                using (var accentPen = new Pen(_theme.AccentColor, 2f))
                                {
                                    g.DrawPath(accentPen, cardPath);
                                }
                            }
                            else
                            {
                                using (var borderPen = new Pen(_theme.BorderColor, 1f))
                                {
                                    g.DrawPath(borderPen, cardPath);
                                }
                            }
                        }
                    }

                    // STEP 2: Draw drag handle (Figma layers panel feature)
                    if (isSelected || isHovered)
                    {
                        Rectangle handleRect = new Rectangle(
                            nodeBounds.Left + 8,
                            nodeBounds.Top + nodeBounds.Height / 2 - 6,
                            HandleWidth,
                            12);

                        Color handleColor = Color.FromArgb(isSelected ? 120 : 80, _theme.TreeForeColor);

                        using (var pen = new Pen(handleColor, 1.5f))
                        {
                            pen.StartCap = LineCap.Round;
                            pen.EndCap = LineCap.Round;

                            // Three horizontal lines (drag handle)
                            int y1 = handleRect.Top;
                            int y2 = handleRect.Top + 4;
                            int y3 = handleRect.Top + 8;

                            g.DrawLine(pen, handleRect.Left, y1, handleRect.Right, y1);
                            g.DrawLine(pen, handleRect.Left, y2, handleRect.Right, y2);
                            g.DrawLine(pen, handleRect.Left, y3, handleRect.Right, y3);
                        }
                    }

                    // STEP 3: Draw Figma triangle toggle
                    bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                    if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                    {
                        var toggleRect = _owner.LayoutHelper.TransformToViewport(node.ToggleRectContent);
                        Color triangleColor = _theme.TreeForeColor;

                        using (var triangleBrush = new SolidBrush(triangleColor))
                        {
                            int centerX = toggleRect.Left + toggleRect.Width / 2;
                            int centerY = toggleRect.Top + toggleRect.Height / 2;
                            int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                            Point[] triangle;

                            if (node.Item.IsExpanded)
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

                            g.FillPolygon(triangleBrush, triangle);
                        }
                    }

                    // STEP 4: Draw Figma checkbox (if enabled)
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

                            using (var borderPen = new Pen(borderColor, 1.5f))
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

                    // STEP 5: Draw Figma layer/component icon
                    if (node.IconRectContent != Rectangle.Empty)
                    {
                        var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                        if (!string.IsNullOrEmpty(node.Item.ImagePath))
                        {
                            // Draw provided image
                            PaintIcon(g, iconRect, node.Item.ImagePath);
                        }
                        else
                        {
                            // Frame/component outline icon (Figma Style)
                            Color iconColor = _theme.AccentColor;
                            using (var iconPath = CreateRoundedRectangle(iconRect, 2))
                            {
                                // Outline only
                                using (var pen = new Pen(iconColor, 1.5f))
                                {
                                    g.DrawPath(pen, iconPath);
                                }

                                // Inner detail rectangle
                                int padding = iconRect.Width / 4;
                                Rectangle innerRect = new Rectangle(
                                    iconRect.X + padding,
                                    iconRect.Y + padding,
                                    iconRect.Width - padding * 2,
                                    iconRect.Height - padding * 2);

                                if (innerRect.Width > 0 && innerRect.Height > 0)
                                {
                                    using (var innerPen = new Pen(Color.FromArgb(100, iconColor), 1f))
                                    {
                                        g.DrawRectangle(innerPen, innerRect);
                                    }
                                }
                            }
                        }
                    }

                    // STEP 6: Draw visibility toggle (eye icon) - Figma layers feature
                    if (node.IconRectContent != Rectangle.Empty)
                    {
                        var iconRectVp = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                        Rectangle eyeRect = new Rectangle(
                            iconRectVp.Right + 4,
                            iconRectVp.Y + iconRectVp.Height / 4,
                            12,
                            iconRectVp.Height / 2);

                        if (eyeRect.Width > 0 && eyeRect.Height > 0)
                        {
                            Color eyeColor = Color.FromArgb(120, _theme.TreeForeColor);

                            int centerX = eyeRect.Left + eyeRect.Width / 2;
                            int centerY = eyeRect.Top + eyeRect.Height / 2;

                            using (var pen = new Pen(eyeColor, 1f))
                            {
                                // Eye outline
                                g.DrawEllipse(pen, centerX - 3, centerY - 2, 6, 4);

                                // Pupil
                                using (var brush = new SolidBrush(eyeColor))
                                {
                                    g.FillEllipse(brush, centerX - 1, centerY - 1, 2, 2);
                                }
                            }
                        }
                    }

                    // STEP 7: Draw text with Figma typography (Inter/Segoe UI)
                    if (node.TextRectContent != Rectangle.Empty)
                    {
                        var textRect = _owner.LayoutHelper.TransformToViewport(node.TextRectContent);
                        Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                        // Figma uses Inter font (Segoe UI fallback), regular weight
                        using (var renderFont = new Font("Segoe UI", _owner.TextFont.Size, FontStyle.Regular))
                        {
                            TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, renderFont, textRect, textColor,
                                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                        }
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
                // Selected: card with accent border
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    using (var brush = new SolidBrush(_theme.TreeNodeSelectedBackColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Accent border
                    using (var pen = new Pen(_theme.AccentColor, 2f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
            else if (isHovered)
            {
                // Hover: subtle card
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    using (var hoverBrush = new SolidBrush(_theme.TreeNodeHoverBackColor))
                    {
                        g.FillPath(hoverBrush, path);
                    }

                    using (var pen = new Pen(_theme.BorderColor, 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Paint drag handle on left
            PaintDragHandle(g, nodeBounds, isSelected || isHovered);
        }

        private void PaintDragHandle(Graphics g, Rectangle nodeBounds, bool isVisible)
        {
            if (!isVisible) return;

            Rectangle handleRect = new Rectangle(
                nodeBounds.Left + 4,
                nodeBounds.Top + nodeBounds.Height / 2 - 6,
                HandleWidth,
                12);

            Color handleColor = Color.FromArgb(80, _theme.TreeForeColor);

            using (var pen = new Pen(handleColor, 1.5f))
            {
                // Three horizontal lines (drag handle)
                int lineSpacing = 3;
                g.DrawLine(pen, handleRect.Left, handleRect.Top, handleRect.Right, handleRect.Top);
                g.DrawLine(pen, handleRect.Left, handleRect.Top + lineSpacing, handleRect.Right, handleRect.Top + lineSpacing);
                g.DrawLine(pen, handleRect.Left, handleRect.Top + lineSpacing * 2, handleRect.Right, handleRect.Top + lineSpacing * 2);
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Figma triangle
            Color triangleColor = _theme.TreeForeColor;

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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.FigmaCard);
                    return;
                }
                catch { }
            }

            // Default: Figma layer icon
            PaintDefaultFigmaIcon(g, iconRect);
        }

        private void PaintDefaultFigmaIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.AccentColor;

            // Figma frame/component icon
            using (var path = CreateRoundedRectangle(iconRect, 2))
            {
                // Outline only (Figma Style)
                using (var pen = new Pen(iconColor, 1.5f))
                {
                    g.DrawPath(pen, path);
                }

                // Inner detail
                int padding = iconRect.Width / 4;
                Rectangle innerRect = new Rectangle(
                    iconRect.X + padding,
                    iconRect.Y + padding,
                    iconRect.Width - padding * 2,
                    iconRect.Height - padding * 2);

                if (innerRect.Width > 0 && innerRect.Height > 0)
                {
                    using (var innerPen = new Pen(Color.FromArgb(100, iconColor), 1f))
                    {
                        g.DrawRectangle(innerPen, innerRect);
                    }
                }
            }

            // Paint visibility toggle (eye icon)
            PaintVisibilityIcon(g, new Rectangle(iconRect.Right + 4, iconRect.Y, 12, iconRect.Height));
        }

        private void PaintVisibilityIcon(Graphics g, Rectangle eyeRect)
        {
            if (eyeRect.Width <= 0 || eyeRect.Height <= 0) return;

            Color eyeColor = Color.FromArgb(120, _theme.TreeForeColor);

            int centerX = eyeRect.Left + eyeRect.Width / 2;
            int centerY = eyeRect.Top + eyeRect.Height / 2;

            using (var pen = new Pen(eyeColor, 1f))
            {
                // Eye outline (simplified)
                g.DrawEllipse(pen, centerX - 2, centerY - 2, 4, 4);

                // Pupil
                using (var brush = new SolidBrush(eyeColor))
                {
                    g.FillEllipse(brush, centerX - 1, centerY - 1, 2, 2);
                }
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Figma uses Inter font (Segoe UI fallback)
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

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            // Card padding
            rect = new Rectangle(rect.X + CardPadding, rect.Y + 2, rect.Width - CardPadding * 2, rect.Height - 4);

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
            // Figma comfortable spacing
            return Math.Max(28, base.GetPreferredRowHeight(item, font));
        }
    }
}
