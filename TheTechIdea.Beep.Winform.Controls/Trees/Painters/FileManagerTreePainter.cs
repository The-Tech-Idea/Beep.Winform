using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Modern file manager tree painter (Google Drive, OneDrive Style).
    /// Clean minimal design with folder icons, nested structure, and smooth animations.
    /// Features: Rounded selection, subtle shadows, colorful folder icons, breadcrumb-Style hierarchy.
    /// </summary>
    public class FileManagerTreePainter : BaseTreePainter
    {
        private const int RoundRadius = 6;
        private const int IconPadding = 4;

        private Font _regularFont;
        private Font _boldFont;

        /// <summary>
        /// Modern file manager tree painting (Google Drive/OneDrive Style).
        /// Features: Rounded selection (6px corners), subtle shadows, colorful gradient folder icons, chevron toggles, bold text on selection.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for modern file manager
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw rounded selection with subtle shadow (Google Drive Style)
                if (isSelected || isHovered)
                {
                    Rectangle selectionBounds = new Rectangle(
                        nodeBounds.X + 2,
                        nodeBounds.Y + 1,
                        nodeBounds.Width - 4,
                        nodeBounds.Height - 2);

                    using (var selectionPath = CreateRoundedRectangle(selectionBounds, RoundRadius))
                    {
                        // Subtle shadow on selection
                        if (isSelected)
                        {
                            Rectangle shadowRect = new Rectangle(
                                selectionBounds.X + 1,
                                selectionBounds.Y + 1,
                                selectionBounds.Width,
                                selectionBounds.Height);

                            using (var shadowPath = CreateRoundedRectangle(shadowRect, RoundRadius))
                            {
                                var shadowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(30, 0, 0, 0));
                                g.FillPath(shadowBrush, shadowPath);
                            }
                        }

                        // Fill background
                        var bgBrush = PaintersFactory.GetSolidBrush(isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor);
                        g.FillPath(bgBrush, selectionPath);

                        // Border (accent on selection)
                        if (isSelected)
                        {
                            var borderPen = PaintersFactory.GetPen(_theme.AccentColor, 1f);
                            g.DrawPath(borderPen, selectionPath);
                        }
                    }
                }

                // STEP 2: Draw modern chevron toggle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = _owner.LayoutHelper.TransformToViewport(node.ToggleRectContent);
                    Color chevronColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;

                    var pen = PaintersFactory.GetPen(chevronColor, 1.5f);
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

                // STEP 3: Draw checkbox (rounded Style)
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = _owner.LayoutHelper.TransformToViewport(node.CheckRectContent);
                    var bgBrush = PaintersFactory.GetSolidBrush(node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor);
                    g.FillPath(bgBrush, CreateRoundedRectangle(checkRect, 3));

                    var borderPen = PaintersFactory.GetPen(node.Item.IsChecked ? _theme.AccentColor : _theme.BorderColor, 1.5f);
                    g.DrawPath(borderPen, CreateRoundedRectangle(checkRect, 3));

                    if (node.Item.IsChecked)
                    {
                        var checkPen = PaintersFactory.GetPen(Color.White, 1.5f);
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

                // STEP 4: Draw colorful gradient folder icon (Google Drive Style)
                if (node.IconRectContent != Rectangle.Empty)
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    int tabHeight = iconRect.Height / 4;
                    int tabWidth = iconRect.Width / 2;

                    // Folder tab
                    Rectangle tabRect = new Rectangle(iconRect.X, iconRect.Y, tabWidth, tabHeight);
                    // Folder body
                    Rectangle bodyRect = new Rectangle(iconRect.X, iconRect.Y + tabHeight, iconRect.Width, iconRect.Height - tabHeight);

                    // Gradient fill using theme accent color
                    Color folderColor1 = _theme.AccentColor;
                    Color folderColor2 = Color.FromArgb(
                        Math.Min(255, _theme.AccentColor.R + 30),
                        Math.Min(255, _theme.AccentColor.G + 30),
                        Math.Min(255, _theme.AccentColor.B + 30));

                    using (var brush = new LinearGradientBrush(
                        iconRect,
                        folderColor1,
                        folderColor2,
                        LinearGradientMode.Vertical))
                    {
                        // Draw tab
                        using (var path = new GraphicsPath())
                        {
                            path.AddArc(tabRect.X, tabRect.Y, tabHeight, tabHeight, 180, 90);
                            path.AddLine(tabRect.X + tabHeight / 2, tabRect.Y, tabRect.Right - tabHeight / 2, tabRect.Y);
                            path.AddArc(tabRect.Right - tabHeight, tabRect.Y, tabHeight, tabHeight, 270, 90);
                            path.AddLine(tabRect.Right, tabRect.Bottom, tabRect.X, tabRect.Bottom);
                            path.CloseFigure();
                            g.FillPath(brush, path);
                        }

                        // Draw body
                        using (var path = new GraphicsPath())
                        {
                            path.AddArc(bodyRect.X, bodyRect.Y, 4, 4, 180, 90);
                            path.AddLine(bodyRect.X + 2, bodyRect.Y, bodyRect.Right - 2, bodyRect.Y);
                            path.AddArc(bodyRect.Right - 4, bodyRect.Y, 4, 4, 270, 90);
                            path.AddLine(bodyRect.Right, bodyRect.Y + 2, bodyRect.Right, bodyRect.Bottom - 2);
                            path.AddArc(bodyRect.Right - 4, bodyRect.Bottom - 4, 4, 4, 0, 90);
                            path.AddLine(bodyRect.Right - 2, bodyRect.Bottom, bodyRect.X + 2, bodyRect.Bottom);
                            path.AddArc(bodyRect.X, bodyRect.Bottom - 4, 4, 4, 90, 90);
                            path.CloseFigure();
                            g.FillPath(brush, path);
                        }
                    }

                    // Subtle highlight on folder
                    using (var pen = new Pen(Color.FromArgb(100, 255, 255, 255), 1f))
                    {
                        g.DrawLine(pen, bodyRect.X + 2, bodyRect.Y + 2, bodyRect.Right - 2, bodyRect.Y + 2);
                    }
                }

                // STEP 5: Draw text with bold on selection (Google Drive Style)
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = _owner.LayoutHelper.TransformToViewport(node.TextRectContent);
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                    var fontToUse = isSelected ? _boldFont ?? _regularFont : _regularFont;
                    TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, fontToUse, textRect, textColor,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
                }
            }
            finally
            {
                g.SmoothingMode = oldSmoothing;
                g.TextRenderingHint = oldTextRendering;
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            if (rect.Width <= 0 || rect.Height <= 0 || rect.Width < diameter || rect.Height < diameter)
            {
                if (rect.Width > 0 && rect.Height > 0)
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
                // Selected: use tree-specific selected background
                Color selectedColor = _theme.TreeNodeSelectedBackColor;
                using (var path = CreateRoundedRectangle(nodeBounds, RoundRadius))
                {
                    var brush = PaintersFactory.GetSolidBrush(selectedColor);
                    g.FillPath(brush, path);

                    // Subtle border using accent color
                    using (var pen = PaintersFactory.GetPen(_theme.AccentColor, 1))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
            else if (isHovered)
            {
                // Hover: use tree-specific hover background
                Color hoverColor = _theme.TreeNodeHoverBackColor;
                using (var path = CreateRoundedRectangle(nodeBounds, RoundRadius))
                {
                    var hoverBrush = PaintersFactory.GetSolidBrush(hoverColor);
                    g.FillPath(hoverBrush, path);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Modern minimal chevron using tree-specific colors
            Color chevronColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;
            var pen = PaintersFactory.GetPen(chevronColor, 1.5f);
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

            // Default folder icon with modern colorful design
            PaintFolderIcon(g, iconRect);
        }

        private void PaintFolderIcon(Graphics g, Rectangle iconRect)
        {
            // Modern folder icon with gradient
            int tabHeight = iconRect.Height / 4;
            int tabWidth = iconRect.Width / 2;

            // Folder tab
            Rectangle tabRect = new Rectangle(iconRect.X, iconRect.Y, tabWidth, tabHeight);
            // Folder body
            Rectangle bodyRect = new Rectangle(iconRect.X, iconRect.Y + tabHeight, iconRect.Width, iconRect.Height - tabHeight);

            // Gradient fill using theme accent color
            Color folderColor1 = _theme.AccentColor;
            Color folderColor2 = Color.FromArgb(
                Math.Min(255, _theme.AccentColor.R + 30),
                Math.Min(255, _theme.AccentColor.G + 30),
                Math.Min(255, _theme.AccentColor.B + 30));

            using (var brush = new LinearGradientBrush(
                iconRect,
                folderColor1,
                folderColor2,
                LinearGradientMode.Vertical))
            {
                // Draw tab
                using (var path = new GraphicsPath())
                {
                    path.AddArc(tabRect.X, tabRect.Y, tabHeight, tabHeight, 180, 90);
                    path.AddLine(tabRect.X + tabHeight / 2, tabRect.Y, tabRect.Right - tabHeight / 2, tabRect.Y);
                    path.AddArc(tabRect.Right - tabHeight, tabRect.Y, tabHeight, tabHeight, 270, 90);
                    path.AddLine(tabRect.Right, tabRect.Bottom, tabRect.X, tabRect.Bottom);
                    path.CloseFigure();
                    g.FillPath(brush, path);
                }

                // Draw body
                using (var path = new GraphicsPath())
                {
                    path.AddArc(bodyRect.X, bodyRect.Y, 4, 4, 180, 90);
                    path.AddLine(bodyRect.X + 2, bodyRect.Y, bodyRect.Right - 2, bodyRect.Y);
                    path.AddArc(bodyRect.Right - 4, bodyRect.Y, 4, 4, 270, 90);
                    path.AddLine(bodyRect.Right, bodyRect.Y + 2, bodyRect.Right, bodyRect.Bottom - 2);
                    path.AddArc(bodyRect.Right - 4, bodyRect.Bottom - 4, 4, 4, 0, 90);
                    path.AddLine(bodyRect.Right - 2, bodyRect.Bottom, bodyRect.X + 2, bodyRect.Bottom);
                    path.AddArc(bodyRect.X, bodyRect.Bottom - 4, 4, 4, 90, 90);
                    path.CloseFigure();
                    g.FillPath(brush, path);
                }
            }

            // Subtle highlight
            using (var pen = new Pen(Color.FromArgb(100, 255, 255, 255), 1))
            {
                g.DrawLine(pen, bodyRect.X + 2, bodyRect.Y + 2, bodyRect.Right - 2, bodyRect.Y + 2);
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            // Use tree-specific text colors
            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            var fontToUse = isSelected ? _boldFont ?? _regularFont : _regularFont;
            TextRenderer.DrawText(g, text, fontToUse, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            var brush = PaintersFactory.GetSolidBrush(_theme.TreeBackColor);
            g.FillRectangle(brush, bounds);

            // Enable high-quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            base.Paint(g, owner, bounds);
        }

        public override void Initialize(BeepTree owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            _regularFont = owner?.TextFont ?? SystemFonts.DefaultFont;
            if (_boldFont == null || _boldFont.Size != _regularFont.Size || !_boldFont.FontFamily.Equals(_regularFont.FontFamily))
            {
                try { _boldFont?.Dispose(); } catch { }
                _boldFont = new Font(_regularFont.FontFamily, _regularFont.Size, FontStyle.Bold);
            }
        }

        public override int GetPreferredRowHeight(SimpleItem item, Font font)
        {
            // File manager Style needs comfortable spacing
            return Math.Max(28, base.GetPreferredRowHeight(item, font));
        }
    }
}
