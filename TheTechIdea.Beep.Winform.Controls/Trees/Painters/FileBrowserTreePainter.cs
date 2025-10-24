using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Deep file browser tree painter optimized for large nested directory structures.
    /// Features: Type-specific icons, folder hierarchy, breadcrumb trails, compact spacing.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class FileBrowserTreePainter : BaseTreePainter
    {
        private const int CompactSpacing = 2;
        private const int IconSize = 16;

        /// <summary>
        /// File browser tree painting optimized for deep directory structures.
        /// Features: Left border accent (2px on selection), compact spacing, file type badges with color-coding, folder icons with tab, extension indicators.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for file browser
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw file browser background (full-width)
                if (isSelected || isHovered)
                {
                    Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;

                    using (var bgBrush = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(bgBrush, nodeBounds);
                    }
                }

                // STEP 2: Draw left border accent (file browser distinctive feature)
                if (isSelected)
                {
                    using (var accentPen = new Pen(_theme.AccentColor, 2f))
                    {
                        g.DrawLine(accentPen, nodeBounds.X, nodeBounds.Y, nodeBounds.X, nodeBounds.Bottom);
                    }
                }

                // STEP 3: Draw compact triangle toggle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    // Transform to viewport coordinates before drawing
                    var toggleRect = _owner.LayoutHelper.TransformToViewport(node.ToggleRectContent);
                    Color toggleColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;

                    using (var toggleBrush = new SolidBrush(toggleColor))
                    {
                        int centerX = toggleRect.Left + toggleRect.Width / 2;
                        int centerY = toggleRect.Top + toggleRect.Height / 2;
                        int size = Math.Min(toggleRect.Width, toggleRect.Height) / 4;

                        Point[] triangle;
                        if (node.Item.IsExpanded)
                        {
                            // Down triangle
                            triangle = new Point[]
                            {
                                new Point(centerX - size, centerY - size / 2),
                                new Point(centerX + size, centerY - size / 2),
                                new Point(centerX, centerY + size)
                            };
                        }
                        else
                        {
                            // Right triangle
                            triangle = new Point[]
                            {
                                new Point(centerX - size / 2, centerY - size),
                                new Point(centerX + size, centerY),
                                new Point(centerX - size / 2, centerY + size)
                            };
                        }

                        g.FillPolygon(toggleBrush, triangle);
                    }
                }

                // STEP 4: Draw checkbox (compact Style)
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = _owner.LayoutHelper.TransformToViewport(node.CheckRectContent);
                    var borderColor = node.Item.IsChecked ? _theme.AccentColor : _theme.BorderColor;
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

                    using (var bgBrush = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(bgBrush, checkRect);
                    }

                    using (var borderPen = new Pen(borderColor, 1f))
                    {
                        g.DrawRectangle(borderPen, checkRect);
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

                // STEP 5: Draw folder/file icon (Windows Explorer Style)
                if (node.IconRectContent != Rectangle.Empty)
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    bool isFolder = hasChildren;

                    if (isFolder)
                    {
                        // Folder icon with tab
                        Color folderColor = Color.FromArgb(
                            (_theme.AccentColor.R + _theme.TreeForeColor.R) / 2,
                            (_theme.AccentColor.G + _theme.TreeForeColor.G) / 2,
                            (_theme.AccentColor.B + _theme.TreeForeColor.B) / 2);

                        // Folder tab
                        int tabHeight = iconRect.Height / 4;
                        Rectangle tabRect = new Rectangle(iconRect.X, iconRect.Y, iconRect.Width / 2, tabHeight);

                        using (var brush = new SolidBrush(folderColor))
                        {
                            g.FillRectangle(brush, tabRect);
                        }

                        // Folder body
                        Rectangle bodyRect = new Rectangle(iconRect.X, iconRect.Y + tabHeight, iconRect.Width, iconRect.Height - tabHeight);
                        using (var brush = new SolidBrush(Color.FromArgb(200, folderColor)))
                        {
                            g.FillRectangle(brush, bodyRect);
                        }

                        // Border
                        using (var pen = new Pen(folderColor, 1f))
                        {
                            g.DrawRectangle(pen, bodyRect);
                        }
                    }
                    else
                    {
                        // File icon (simple document)
                        Color fileColor = _theme.TreeForeColor;

                        using (var brush = new SolidBrush(Color.FromArgb(200, _theme.TreeBackColor)))
                        {
                            g.FillRectangle(brush, iconRect);
                        }

                        using (var pen = new Pen(fileColor, 1f))
                        {
                            g.DrawRectangle(pen, iconRect);

                            // Document lines
                            int lineY = iconRect.Y + iconRect.Height / 3;
                            g.DrawLine(pen, iconRect.X + 2, lineY, iconRect.Right - 2, lineY);
                            g.DrawLine(pen, iconRect.X + 2, lineY + 3, iconRect.Right - 2, lineY + 3);
                            g.DrawLine(pen, iconRect.X + 2, lineY + 6, iconRect.Right - 4, lineY + 6);
                        }
                    }
                }

                // STEP 6: Draw text (compact font for deep hierarchies)
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = _owner.LayoutHelper.TransformToViewport(node.TextRectContent);
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                    // Slightly smaller font for compact display
                    using (var renderFont = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size - 0.5f, _owner.TextFont.Style))
                    {
                        TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, renderFont, textRect, textColor,
                            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
                    }
                }

                // STEP 7: Draw file type badge (extension indicator)
                if (node.TextRectContent != Rectangle.Empty && !hasChildren)
                {
                    string text = node.Item.Text ?? string.Empty;
                    string extension = System.IO.Path.GetExtension(text);

                    if (!string.IsNullOrEmpty(extension))
                    {
                        // Start from transformed text rect for proper placement
                        var textRectVp = _owner.LayoutHelper.TransformToViewport(node.TextRectContent);
                        Rectangle badgeRect = new Rectangle(
                            textRectVp.Right + 4,
                            textRectVp.Y + textRectVp.Height / 4,
                            32,
                            textRectVp.Height / 2);

                        if (badgeRect.Width > 0 && badgeRect.Height > 0)
                        {
                            Color badgeColor = GetFileTypeColor(extension);

                            using (var badgePath = CreateRoundedRectangle(badgeRect, 2))
                            {
                                using (var brush = new SolidBrush(Color.FromArgb(150, badgeColor)))
                                {
                                    g.FillPath(brush, badgePath);
                                }

                                using (var pen = new Pen(badgeColor, 1f))
                                {
                                    g.DrawPath(pen, badgePath);
                                }
                            }

                            // Extension text
                            using (var badgeFont = new Font("Segoe UI", 6.5f, FontStyle.Bold))
                            {
                                TextRenderer.DrawText(g, extension.TrimStart('.').ToUpper(), badgeFont, badgeRect,
                                    _theme.TreeNodeSelectedForeColor,
                                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                            }
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

        private Color GetFileTypeColor(string extension)
        {
            // Color-code by file type
            switch (extension?.ToLower())
            {
                case ".cs":
                case ".cpp":
                case ".java":
                case ".py":
                    return Color.FromArgb(33, 150, 243); // Blue - code files
                case ".txt":
                case ".md":
                case ".doc":
                case ".docx":
                    return Color.FromArgb(76, 175, 80); // Green - documents
                case ".jpg":
                case ".png":
                case ".gif":
                case ".svg":
                    return Color.FromArgb(255, 152, 0); // Orange - images
                case ".mp4":
                case ".avi":
                case ".mov":
                    return Color.FromArgb(156, 39, 176); // Purple - video
                case ".mp3":
                case ".wav":
                case ".flac":
                    return Color.FromArgb(233, 30, 99); // Pink - audio
                case ".zip":
                case ".rar":
                case ".7z":
                    return Color.FromArgb(121, 85, 72); // Brown - archives
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

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            if (isSelected)
            {
                // Selected: full-width highlight
                Color selectedColor = _theme.TreeNodeSelectedBackColor;
                using (var brush = new SolidBrush(selectedColor))
                {
                    g.FillRectangle(brush, nodeBounds);
                }

                // Left border accent
                using (var pen = new Pen(_theme.AccentColor, 2))
                {
                    g.DrawLine(pen, nodeBounds.X, nodeBounds.Y, nodeBounds.X, nodeBounds.Bottom);
                }
            }
            else if (isHovered)
            {
                Color hoverColor = _theme.TreeNodeHoverBackColor;
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillRectangle(brush, nodeBounds);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Small triangle toggle (compact)
            Color toggleColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;
            using (var brush = new SolidBrush(toggleColor))
            {
                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 4;

                Point[] triangle;
                if (isExpanded)
                {
                    // Down triangle
                    triangle = new Point[]
                    {
                        new Point(centerX - size, centerY - size / 2),
                        new Point(centerX + size, centerY - size / 2),
                        new Point(centerX, centerY + size)
                    };
                }
                else
                {
                    // Right triangle
                    triangle = new Point[]
                    {
                        new Point(centerX - size / 2, centerY - size),
                        new Point(centerX + size, centerY),
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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.Material3);
                    return;
                }
                catch { }
            }

            // Default folder/file icon based on context
            PaintDefaultIcon(g, iconRect);
        }

        private void PaintDefaultIcon(Graphics g, Rectangle iconRect)
        {
            // Simple folder icon (Windows Explorer Style)
            Color folderColor = Color.FromArgb(
                (_theme.AccentColor.R + _theme.TreeForeColor.R) / 2,
                (_theme.AccentColor.G + _theme.TreeForeColor.G) / 2,
                (_theme.AccentColor.B + _theme.TreeForeColor.B) / 2);

            // Folder tab
            int tabHeight = iconRect.Height / 4;
            Rectangle tabRect = new Rectangle(iconRect.X, iconRect.Y, iconRect.Width / 2, tabHeight);

            using (var brush = new SolidBrush(folderColor))
            {
                g.FillRectangle(brush, tabRect);
            }

            // Folder body
            Rectangle bodyRect = new Rectangle(iconRect.X, iconRect.Y + tabHeight, iconRect.Width, iconRect.Height - tabHeight);
            using (var brush = new SolidBrush(Color.FromArgb(200, folderColor)))
            {
                g.FillRectangle(brush, bodyRect);
            }

            // Border
            using (var pen = new Pen(folderColor, 1))
            {
                g.DrawRectangle(pen, bodyRect);
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Compact font for deep hierarchies
            Font renderFont = new Font(font.FontFamily, font.Size - 0.5f, font.Style);

            TextRenderer.DrawText(g, text, renderFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);

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

        /// <summary>
        /// Paint file type badge (extension indicator).
        /// </summary>
        public void PaintFileTypeBadge(Graphics g, Rectangle badgeRect, string extension)
        {
            if (badgeRect.Width <= 0 || badgeRect.Height <= 0) return;

            // Small rounded rectangle with extension text
            Color badgeColor = GetFileTypeColor(extension);

            using (var path = CreateRoundedRectangle(badgeRect, 2))
            {
                using (var brush = new SolidBrush(Color.FromArgb(150, badgeColor)))
                {
                    g.FillPath(brush, path);
                }

                using (var pen = new Pen(badgeColor, 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Extension text
            if (!string.IsNullOrEmpty(extension))
            {
                using (var font = new Font("Segoe UI", 6.5f, FontStyle.Bold))
                {
                    TextRenderer.DrawText(g, extension.TrimStart('.').ToUpper(), font, badgeRect,
                        _theme.TreeNodeSelectedForeColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            }
        }

        /// <summary>
        /// Paint folder size indicator.
        /// </summary>
        public void PaintFolderSize(Graphics g, Rectangle sizeRect, string sizeText)
        {
            if (sizeRect.Width <= 0 || sizeRect.Height <= 0) return;

            Color sizeColor = Color.FromArgb(150, _theme.TreeForeColor);
            using (var font = new Font("Segoe UI", 7f, FontStyle.Regular))
            {
                TextRenderer.DrawText(g, sizeText, font, sizeRect, sizeColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        public override int GetPreferredRowHeight(SimpleItem item, Font font)
        {
            // File browser needs compact spacing for large directories
            return Math.Max(20, base.GetPreferredRowHeight(item, font) - 4);
        }
    }
}
