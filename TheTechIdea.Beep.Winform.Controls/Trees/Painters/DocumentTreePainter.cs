using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Document tree painter for mixed content types (folders, files, documents, media).
    /// Features: Rich content type indicators, preview thumbnails, metadata display.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class DocumentTreePainter : BaseTreePainter
    {
        private const int ThumbnailSize = 32;
        private const int MetadataSpacing = 4;

        /// <summary>
        /// Document tree-specific node painting with document management style.
        /// Features: Card elevation with shadows, rounded corners (4px), document type badges, metadata display, thumbnail placeholders.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for document card appearance
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw card elevation shadow FIRST (if selected)
                if (isSelected)
                {
                    // Subtle shadow for elevation effect
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                    {
                        var shadowRect = new Rectangle(nodeBounds.X + 1, nodeBounds.Y + 1, nodeBounds.Width, nodeBounds.Height);
                        using (var shadowPath = CreateRoundedRectangle(shadowRect, 4))
                        {
                            g.FillPath(shadowBrush, shadowPath);
                        }
                    }
                }

                // STEP 2: Draw document card background
                if (isSelected || isHovered)
                {
                    Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;

                    using (var nodePath = CreateRoundedRectangle(nodeBounds, 4))
                    {
                        using (var bgBrush = new SolidBrush(bgColor))
                        {
                            g.FillPath(bgBrush, nodePath);
                        }

                        // STEP 3: Card border (accent on selection)
                        if (isSelected)
                        {
                            using (var borderPen = new Pen(_theme.AccentColor, 1f))
                            {
                                g.DrawPath(borderPen, nodePath);
                            }
                        }
                    }
                }

                // STEP 4: Draw chevron toggle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = node.ToggleRectContent;
                    Color chevronColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;

                    using (var pen = new Pen(chevronColor, 1.5f))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;

                        int centerX = toggleRect.Left + toggleRect.Width / 2;
                        int centerY = toggleRect.Top + toggleRect.Height / 2;
                        int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                        if (node.Item.IsExpanded)
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

                // STEP 5: Draw document checkbox
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = node.CheckRectContent;
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

                // STEP 6: Draw document icon with page curl
                if (node.IconRectContent != Rectangle.Empty && !string.IsNullOrEmpty(node.Item.ImagePath))
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    PaintIcon(g, iconRect, node.Item.ImagePath);
                }

                // STEP 7: Draw text
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = node.TextRectContent;
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                    using (var renderFont = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size, FontStyle.Regular))
                    {
                        TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, renderFont, textRect, textColor,
                            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
                    }
                }

                // STEP 8: Draw document type badge (right side)
                if (isSelected || isHovered)
                {
                    int badgeX = nodeBounds.Right - 50;
                    int badgeY = nodeBounds.Y + (nodeBounds.Height - 16) / 2;
                    Rectangle badgeRect = new Rectangle(badgeX, badgeY, 42, 16);

                    // Simulate document type (in real scenario would come from node.Item metadata)
                    string docType = node.Item.IsExpanded ? "PDF" : "DOC";
                    Color badgeColor = node.Item.IsExpanded ? Color.FromArgb(244, 67, 54) : Color.FromArgb(33, 150, 243);

                    using (var badgePath = CreateRoundedRectangle(badgeRect, 3))
                    {
                        using (var badgeBrush = new SolidBrush(Color.FromArgb(180, badgeColor)))
                        {
                            g.FillPath(badgeBrush, badgePath);
                        }

                        using (var badgePen = new Pen(badgeColor, 1f))
                        {
                            g.DrawPath(badgePen, badgePath);
                        }
                    }

                    // Badge text
                    using (var badgeFont = new Font("Segoe UI", 7f, FontStyle.Bold))
                    {
                        TextRenderer.DrawText(g, docType, badgeFont, badgeRect, Color.White,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
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
                // Selected: card-style elevation
                Color selectedColor = _theme.TreeNodeSelectedBackColor;
                using (var path = CreateRoundedRectangle(nodeBounds, 4))
                {
                    // Subtle shadow
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                    {
                        var shadowRect = new Rectangle(nodeBounds.X + 1, nodeBounds.Y + 1, nodeBounds.Width, nodeBounds.Height);
                        using (var shadowPath = CreateRoundedRectangle(shadowRect, 4))
                        {
                            g.FillPath(shadowBrush, shadowPath);
                        }
                    }

                    using (var brush = new SolidBrush(selectedColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Border
                    using (var pen = new Pen(_theme.AccentColor, 1))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
            else if (isHovered)
            {
                Color hoverColor = _theme.TreeNodeHoverBackColor;
                using (var path = CreateRoundedRectangle(nodeBounds, 4))
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Modern chevron toggle
            Color chevronColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;
            using (var pen = new Pen(chevronColor, 1.5f))
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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, _owner?.ControlStyle ?? Common.BeepControlStyle.Material3);
                    return;
                }
                catch { }
            }

            // Default document icon with page curl
            PaintDefaultDocumentIcon(g, iconRect);
        }

        private void PaintDefaultDocumentIcon(Graphics g, Rectangle iconRect)
        {
            Color docColor = Color.FromArgb(
                (_theme.AccentColor.R + _theme.TreeForeColor.R) / 2,
                (_theme.AccentColor.G + _theme.TreeForeColor.G) / 2,
                (_theme.AccentColor.B + _theme.TreeForeColor.B) / 2);

            // Document page
            Rectangle pageRect = new Rectangle(iconRect.X, iconRect.Y, iconRect.Width - 4, iconRect.Height - 4);

            using (var path = CreateRoundedRectangle(pageRect, 2))
            {
                // Page background
                using (var brush = new SolidBrush(Color.FromArgb(200, _theme.TreeBackColor)))
                {
                    g.FillPath(brush, path);
                }

                // Border
                using (var pen = new Pen(docColor, 1.5f))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Content lines
            using (var pen = new Pen(Color.FromArgb(100, docColor), 1))
            {
                int lineY = pageRect.Y + 6;
                int spacing = 3;
                for (int i = 0; i < 3; i++)
                {
                    g.DrawLine(pen, pageRect.X + 3, lineY, pageRect.Right - 3, lineY);
                    lineY += spacing;
                }
            }

            // Page curl (top-right corner)
            Point[] curl = new Point[]
            {
                new Point(pageRect.Right - 6, pageRect.Y),
                new Point(pageRect.Right, pageRect.Y),
                new Point(pageRect.Right, pageRect.Y + 6)
            };

            using (var brush = new SolidBrush(Color.FromArgb(150, docColor)))
            {
                g.FillPolygon(brush, curl);
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            TextRenderer.DrawText(g, text, font, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
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

        /// <summary>
        /// Paint document type indicator badge.
        /// </summary>
        public void PaintDocumentTypeBadge(Graphics g, Rectangle badgeRect, string docType)
        {
            if (badgeRect.Width <= 0 || badgeRect.Height <= 0) return;

            Color badgeColor = GetDocumentTypeColor(docType);

            using (var path = CreateRoundedRectangle(badgeRect, 3))
            {
                using (var brush = new SolidBrush(Color.FromArgb(180, badgeColor)))
                {
                    g.FillPath(brush, path);
                }

                using (var pen = new Pen(badgeColor, 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Type text
            if (!string.IsNullOrEmpty(docType))
            {
                using (var font = new Font("Segoe UI", 7f, FontStyle.Bold))
                {
                    TextRenderer.DrawText(g, docType.ToUpper(), font, badgeRect,
                        Color.White,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            }
        }

        /// <summary>
        /// Paint document metadata (size, date, author).
        /// </summary>
        public void PaintDocumentMetadata(Graphics g, Rectangle metadataRect, string metadata)
        {
            if (metadataRect.Width <= 0 || metadataRect.Height <= 0) return;

            Color metadataColor = Color.FromArgb(150, _theme.TreeForeColor);
            using (var font = new Font("Segoe UI", 7.5f, FontStyle.Regular))
            {
                TextRenderer.DrawText(g, metadata, font, metadataRect, metadataColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        /// <summary>
        /// Paint thumbnail placeholder for image/video documents.
        /// </summary>
        public void PaintThumbnail(Graphics g, Rectangle thumbRect, Color previewColor)
        {
            if (thumbRect.Width <= 0 || thumbRect.Height <= 0) return;

            // Thumbnail border
            using (var path = CreateRoundedRectangle(thumbRect, 3))
            {
                using (var brush = new SolidBrush(Color.FromArgb(50, previewColor)))
                {
                    g.FillPath(brush, path);
                }

                using (var pen = new Pen(Color.FromArgb(150, previewColor), 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Thumbnail icon (image symbol)
            int iconSize = thumbRect.Width / 2;
            int iconX = thumbRect.X + (thumbRect.Width - iconSize) / 2;
            int iconY = thumbRect.Y + (thumbRect.Height - iconSize) / 2;

            using (var pen = new Pen(Color.FromArgb(150, previewColor), 1.5f))
            {
                // Mountain shape
                g.DrawLine(pen, iconX, iconY + iconSize, iconX + iconSize / 2, iconY);
                g.DrawLine(pen, iconX + iconSize / 2, iconY, iconX + iconSize, iconY + iconSize);

                // Sun circle
                int sunSize = iconSize / 4;
                g.DrawEllipse(pen, iconX + iconSize - sunSize - 2, iconY + 2, sunSize, sunSize);
            }
        }

        private Color GetDocumentTypeColor(string docType)
        {
            switch (docType?.ToLower())
            {
                case "pdf":
                    return Color.FromArgb(244, 67, 54); // Red
                case "word":
                case "doc":
                    return Color.FromArgb(33, 150, 243); // Blue
                case "excel":
                case "xls":
                    return Color.FromArgb(76, 175, 80); // Green
                case "ppt":
                case "pptx":
                    return Color.FromArgb(255, 152, 0); // Orange
                case "image":
                    return Color.FromArgb(156, 39, 176); // Purple
                case "video":
                    return Color.FromArgb(233, 30, 99); // Pink
                default:
                    return _theme.AccentColor;
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

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
            // Document trees need space for metadata and thumbnails
            return Math.Max(36, base.GetPreferredRowHeight(item, font));
        }
    }
}
