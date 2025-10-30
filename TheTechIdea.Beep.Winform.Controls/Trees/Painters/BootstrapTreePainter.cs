using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Bootstrap tree painter.
    /// Features: Card-based nodes, Bootstrap colors, badges, clean borders.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class BootstrapTreePainter : BaseTreePainter
    {
        private const int CornerRadius = 4;
        private const int CardPadding = 8;

        private Font _regularFont;

        public override void Initialize(BeepTree owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            _regularFont = owner?.TextFont ?? SystemFonts.DefaultFont;
        }

        /// <summary>
        /// Bootstrap-specific node painting with card-based design.
        /// Features: Card-Style nodes with shadows, thick 2px borders, Bootstrap primary color accents, badge-Style icons.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for Bootstrap clean appearance
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw Bootstrap card shadow FIRST (if selected)
                if (isSelected)
                {
                    var shadowPath = CreateRoundedRectangle(new Rectangle(
                        nodeBounds.X, nodeBounds.Y, nodeBounds.Width, nodeBounds.Height), CornerRadius);
                    var shadowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(25, 0, 0, 0));
                    g.FillPath(shadowBrush, shadowPath);
                }

                // STEP 2: Draw Bootstrap card background
                if (isSelected || isHovered)
                {
                    using (var cardPath = CreateRoundedRectangle(nodeBounds, CornerRadius))
                    {
                        Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;
                        var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
                        g.FillPath(bgBrush, cardPath);

                        // STEP 3: Bootstrap thick border (2px on selection, 1px on hover)
                        Color borderColor = isSelected ? _theme.AccentColor : _theme.BorderColor;
                        float borderWidth = isSelected ? 2f : 1f;
                        var borderPen = PaintersFactory.GetPen(borderColor, borderWidth);
                        g.DrawPath(borderPen, cardPath);
                    }
                }

                // STEP 4: Draw Bootstrap caret (filled triangle)
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = node.ToggleRectContent;
                    Color caretColor = _theme.TreeForeColor;

                    var brush = PaintersFactory.GetSolidBrush(caretColor);
                    int centerX = toggleRect.Left + toggleRect.Width / 2;
                    int centerY = toggleRect.Top + toggleRect.Height / 2;
                    int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                    Point[] caret;

                    if (node.Item.IsExpanded)
                    {
                        // Caret down (filled triangle)
                        caret = new Point[]
                        {
                            new Point(centerX - size, centerY - size / 2),
                            new Point(centerX + size, centerY - size / 2),
                            new Point(centerX, centerY + size / 2)
                        };
                    }
                    else
                    {
                        // Caret right (filled triangle)
                        caret = new Point[]
                        {
                            new Point(centerX - size / 2, centerY - size),
                            new Point(centerX + size / 2, centerY),
                            new Point(centerX - size / 2, centerY + size)
                        };
                    }

                    g.FillPolygon(brush, caret);
                }

                // STEP 5: Draw Bootstrap checkbox
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = node.CheckRectContent;
                    var borderColor = isHovered ? _theme.AccentColor : _theme.BorderColor;
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

                    using (var checkPath = CreateRoundedRectangle(checkRect, 3))
                    {
                        var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
                        g.FillPath(bgBrush, checkPath);

                        var borderPen = PaintersFactory.GetPen(borderColor, 1.5f);
                        g.DrawPath(borderPen, checkPath);
                    }

                    // Bootstrap checkmark
                    if (node.Item.IsChecked)
                    {
                        var checkPen = PaintersFactory.GetPen(Color.White, 2f);
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

                // STEP 6: Draw Bootstrap badge-Style icon
                if (node.IconRectContent != Rectangle.Empty && !string.IsNullOrEmpty(node.Item.ImagePath))
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    PaintIcon(g, iconRect, node.Item.ImagePath);
                }

                // STEP 7: Draw text with Bootstrap typography
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = node.TextRectContent;
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                    TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, _regularFont, textRect, textColor,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
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
                // Selected: card with border
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    using (var brush = new SolidBrush(_theme.TreeNodeSelectedBackColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Bootstrap primary border
                    using (var pen = new Pen(_theme.AccentColor, 1f))
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
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Bootstrap caret
            Color caretColor = _theme.TreeForeColor;

            using (var brush = new SolidBrush(caretColor))
            {
                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                Point[] caret;

                if (isExpanded)
                {
                    // Caret down
                    caret = new Point[]
                    {
                        new Point(centerX - size, centerY - size / 2),
                        new Point(centerX + size, centerY - size / 2),
                        new Point(centerX, centerY + size / 2)
                    };
                }
                else
                {
                    // Caret right
                    caret = new Point[]
                    {
                        new Point(centerX - size / 2, centerY - size),
                        new Point(centerX + size / 2, centerY),
                        new Point(centerX - size / 2, centerY + size)
                    };
                }

                g.FillPolygon(brush, caret);
            }
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, string imagePath)
        {
            if (iconRect.Width <= 0 || iconRect.Height <= 0) return;

            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.Bootstrap);
                    return;
                }
                catch { }
            }

            // Default: Bootstrap badge-Style icon
            PaintDefaultBootstrapIcon(g, iconRect);
        }

        private void PaintDefaultBootstrapIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.AccentColor;

            // Bootstrap badge Style
            using (var path = CreateRoundedRectangle(iconRect, 3))
            {
                using (var brush = new SolidBrush(iconColor))
                {
                    g.FillPath(brush, path);
                }

                // Icon symbol
                Font iconFont = new Font("Segoe UI", iconRect.Height * 0.5f, FontStyle.Bold);
                using (var textBrush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    g.DrawString("📁", iconFont, textBrush, iconRect, sf);
                }

                iconFont.Dispose();
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Bootstrap uses system fonts
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
            // Bootstrap comfortable spacing
            return Math.Max(32, base.GetPreferredRowHeight(item, font));
        }
    }
}
