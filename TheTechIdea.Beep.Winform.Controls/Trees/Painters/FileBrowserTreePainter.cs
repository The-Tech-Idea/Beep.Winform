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
    /// Deep file browser tree painter optimized for large nested directory structures.
    /// Features: Type-specific icons, folder hierarchy, breadcrumb trails, compact spacing.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class FileBrowserTreePainter : BaseTreePainter
    {
        private const int CompactSpacing = 2;
        private const int IconSize = 16;

        private Font _compactFont;

        public override void Initialize(BeepTree owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            _compactFont = new Font(owner?.TextFont?.FontFamily ?? SystemFonts.DefaultFont.FontFamily,
                Math.Max(6f, (owner?.TextFont?.Size ?? SystemFonts.DefaultFont.Size) - 0.5f), owner?.TextFont?.Style ?? FontStyle.Regular);
        }

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
                    var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
                    g.FillRectangle(bgBrush, nodeBounds);
                }

                // STEP 2: Draw left border accent (file browser distinctive feature)
                if (isSelected)
                {
                    var accentPen = PaintersFactory.GetPen(_theme.AccentColor, 2f);
                    g.DrawLine(accentPen, nodeBounds.X, nodeBounds.Y, nodeBounds.X, nodeBounds.Bottom);
                }

                // STEP 3: Draw compact triangle toggle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    // Transform to viewport coordinates before drawing
                    var toggleRect = _owner.LayoutHelper.TransformToViewport(node.ToggleRectContent);
                    Color toggleColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;
                    var toggleBrush = PaintersFactory.GetSolidBrush(toggleColor);

                    int centerX = toggleRect.Left + toggleRect.Width / 2;
                    int centerY = toggleRect.Top + toggleRect.Height / 2;
                    int size = Math.Min(toggleRect.Width, toggleRect.Height) / 4;

                    Point[] triangle;
                    if (node.Item.IsExpanded)
                    {
                        // Down triangle
                        triangle = new[] { new Point(centerX - size, centerY - size / 2), new Point(centerX + size, centerY - size / 2), new Point(centerX, centerY + size) };
                    }
                    else
                    {
                        // Right triangle
                        triangle = new[] { new Point(centerX - size / 2, centerY - size), new Point(centerX + size, centerY), new Point(centerX - size / 2, centerY + size) };
                    }

                    g.FillPolygon(toggleBrush, triangle);
                }

                // STEP 4: Draw checkbox (compact Style)
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = _owner.LayoutHelper.TransformToViewport(node.CheckRectContent);
                    var bgBrush = PaintersFactory.GetSolidBrush(node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor);
                    g.FillRectangle(bgBrush, checkRect);

                    var borderPen = PaintersFactory.GetPen(node.Item.IsChecked ? _theme.AccentColor : _theme.BorderColor, 1f);
                    g.DrawRectangle(borderPen, checkRect);

                    if (node.Item.IsChecked)
                    {
                        var checkPen = PaintersFactory.GetPen(Color.White, 1.5f);
                        var points = new[] { new Point(checkRect.X + checkRect.Width / 4, checkRect.Y + checkRect.Height / 2), new Point(checkRect.X + checkRect.Width / 2 - 1, checkRect.Y + checkRect.Height * 3 / 4), new Point(checkRect.X + checkRect.Width * 3 / 4, checkRect.Y + checkRect.Height / 4) };
                        g.DrawLines(checkPen, points);
                    }
                }

                // STEP 5: Draw folder/file icon (Windows Explorer Style)
                if (node.IconRectContent != Rectangle.Empty)
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    bool isFolder = hasChildren;

                    if (isFolder)
                    {
                        Color folderColor = Color.FromArgb((_theme.AccentColor.R + _theme.TreeForeColor.R) / 2, (_theme.AccentColor.G + _theme.TreeForeColor.G) / 2, (_theme.AccentColor.B + _theme.TreeForeColor.B) / 2);
                        var brush = PaintersFactory.GetSolidBrush(folderColor);
                        int tabHeight = iconRect.Height / 4;
                        var tabRect = new Rectangle(iconRect.X, iconRect.Y, iconRect.Width / 2, tabHeight);
                        g.FillRectangle(brush, tabRect);

                        var bodyBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(200, folderColor));
                        var bodyRect = new Rectangle(iconRect.X, iconRect.Y + tabHeight, iconRect.Width, iconRect.Height - tabHeight);
                        g.FillRectangle(bodyBrush, bodyRect);

                        var pen = PaintersFactory.GetPen(folderColor, 1f);
                        g.DrawRectangle(pen, bodyRect);
                    }
                    else
                    {
                        var bgBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(200, _theme.TreeBackColor));
                        g.FillRectangle(bgBrush, iconRect);

                        var pen = PaintersFactory.GetPen(_theme.TreeForeColor, 1f);
                        g.DrawRectangle(pen, iconRect);

                        int lineY = iconRect.Y + iconRect.Height / 3;
                        g.DrawLine(pen, iconRect.X + 2, lineY, iconRect.Right - 2, lineY);
                        g.DrawLine(pen, iconRect.X + 2, lineY + 3, iconRect.Right - 2, lineY + 3);
                        g.DrawLine(pen, iconRect.X + 2, lineY + 6, iconRect.Right - 4, lineY + 6);
                    }
                }

                // STEP 6: Draw text (compact font for deep hierarchies)
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = _owner.LayoutHelper.TransformToViewport(node.TextRectContent);
                    var textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;
                    TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, _compactFont, textRect, textColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
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
                        var badgeRect = new Rectangle(textRectVp.Right + 4, textRectVp.Y + textRectVp.Height / 4, 32, textRectVp.Height / 2);
                        if (badgeRect.Width > 0 && badgeRect.Height > 0)
                        {
                            Color badgeColor = GetFileTypeColor(extension);
                            var badgeBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(150, badgeColor));
                            using (var badgePath = CreateRoundedRectangle(badgeRect, 2))
                            {
                                g.FillPath(badgeBrush, badgePath);
                            }

                            var pen = PaintersFactory.GetPen(badgeColor, 1f);
                            using (var sfBrush = PaintersFactory.GetSolidBrush(badgeColor))
                            {
                                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                                using (var badgeFont = new Font("Segoe UI", 6.5f, FontStyle.Bold))
                                {
                                    g.DrawString(extension.TrimStart('.').ToUpper(), badgeFont, sfBrush, badgeRect, sf);
                                }
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
                var brush = PaintersFactory.GetSolidBrush(_theme.TreeNodeSelectedBackColor);
                g.FillRectangle(brush, nodeBounds);

                var pen = PaintersFactory.GetPen(_theme.AccentColor, 2);
                g.DrawLine(pen, nodeBounds.X, nodeBounds.Y, nodeBounds.X, nodeBounds.Bottom);
            }
            else if (isHovered)
            {
                var hoverBrush = PaintersFactory.GetSolidBrush(_theme.TreeNodeHoverBackColor);
                g.FillRectangle(hoverBrush, nodeBounds);
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Small triangle toggle (compact)
            Color toggleColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;
            var toggleBrush = PaintersFactory.GetSolidBrush(toggleColor);
            int centerX = toggleRect.Left + toggleRect.Width / 2;
            int centerY = toggleRect.Top + toggleRect.Height / 2;
            int size = Math.Min(toggleRect.Width, toggleRect.Height) / 4;

            Point[] triangle;
            if (isExpanded)
            {
                triangle = new[] { new Point(centerX - size, centerY - size / 2), new Point(centerX + size, centerY - size / 2), new Point(centerX, centerY + size) };
            }
            else
            {
                triangle = new[] { new Point(centerX - size / 2, centerY - size), new Point(centerX + size, centerY), new Point(centerX - size / 2, centerY + size) };
            }

            g.FillPolygon(toggleBrush, triangle);
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

            PaintDefaultIcon(g, iconRect);
        }

        private void PaintDefaultIcon(Graphics g, Rectangle iconRect)
        {
            // Simple folder icon (Windows Explorer Style)
            Color folderColor = Color.FromArgb(
                (_theme.AccentColor.R + _theme.TreeForeColor.R) / 2,
                (_theme.AccentColor.G + _theme.TreeForeColor.G) / 2,
                (_theme.AccentColor.B + _theme.TreeForeColor.B) / 2);

            var tabBrush = PaintersFactory.GetSolidBrush(folderColor);
            int tabHeight = iconRect.Height / 4;
            var tabRect = new Rectangle(iconRect.X, iconRect.Y, iconRect.Width / 2, tabHeight);
            g.FillRectangle(tabBrush, tabRect);

            var bodyBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(200, folderColor));
            var bodyRect = new Rectangle(iconRect.X, iconRect.Y + tabHeight, iconRect.Width, iconRect.Height - tabHeight);
            g.FillRectangle(bodyBrush, bodyRect);

            var pen = PaintersFactory.GetPen(folderColor, 1);
            g.DrawRectangle(pen, bodyRect);
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;
            TextRenderer.DrawText(g, text, _compactFont, textRect, textColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            var brush = PaintersFactory.GetSolidBrush(_theme.TreeBackColor);
            g.FillRectangle(brush, bounds);

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

            Color badgeColor = GetFileTypeColor(extension);
            var brush = PaintersFactory.GetSolidBrush(Color.FromArgb(150, badgeColor));
            using (var path = CreateRoundedRectangle(badgeRect, 2))
            {
                g.FillPath(brush, path);
            }

            var pen = PaintersFactory.GetPen(badgeColor, 1);
            g.DrawPath(pen, CreateRoundedRectangle(badgeRect, 2));

            if (!string.IsNullOrEmpty(extension))
            {
                using (var font = new Font("Segoe UI", 6.5f, FontStyle.Bold))
                {
                    TextRenderer.DrawText(g, extension.TrimStart('.').ToUpper(), font, badgeRect, _theme.TreeNodeSelectedForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
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
                TextRenderer.DrawText(g, sizeText, font, sizeRect, sizeColor, TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        public override int GetPreferredRowHeight(SimpleItem item, Font font)
        {
            // File browser needs compact spacing for large directories
            return Math.Max(20, base.GetPreferredRowHeight(item, font) - 4);
        }
    }
}
