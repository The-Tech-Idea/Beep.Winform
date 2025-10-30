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
    /// Apple iOS15 tree painter.
    /// Features: Rounded group headers, SF Symbols Style icons, smooth shadows, fluid spacing.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class iOS15TreePainter : BaseTreePainter
    {
        private const int CornerRadius = 10;
        private const int GroupPadding = 8;
        private const float ShadowOpacity = 0.1f;

        private Font _regularFont;
        private Font _boldFont;

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

        /// <summary>
        /// iOS15-specific node painting with rounded groups and drop shadows.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw drop shadow FIRST (only on selection)
                if (isSelected)
                {
                    var shadowRect = new Rectangle(
                        nodeBounds.X,
                        nodeBounds.Y + 2,  // iOS drop shadow offset
                        nodeBounds.Width,
                        nodeBounds.Height);

                    using (var shadowPath = CreateRoundedRectangle(shadowRect, CornerRadius))
                    {
                        var shadowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb((int)(255 * ShadowOpacity), 0, 0, 0));
                        g.FillPath(shadowBrush, shadowPath);
                    }
                }

                // STEP 2: Draw rounded background (large iOS corners)
                if (isSelected || isHovered)
                {
                    using (var bgPath = CreateRoundedRectangle(nodeBounds, CornerRadius))
                    {
                        Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;
                        var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
                        g.FillPath(bgBrush, bgPath);
                    }
                }

                // STEP 3: Draw SF Symbols Style chevron toggle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = node.ToggleRectContent;
                    Color chevronColor = _theme.AccentColor;  // iOS uses accent color for chevrons

                    var pen = PaintersFactory.GetPen(chevronColor, 2f);
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;

                    int centerX = toggleRect.Left + toggleRect.Width / 2;
                    int centerY = toggleRect.Top + toggleRect.Height / 2;
                    int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                    if (node.Item.IsExpanded)
                    {
                        // iOS chevron down
                        g.DrawLine(pen, centerX - size, centerY - size / 2, centerX, centerY + size / 2);
                        g.DrawLine(pen, centerX, centerY + size / 2, centerX + size, centerY - size / 2);
                    }
                    else
                    {
                        // iOS chevron right
                        g.DrawLine(pen, centerX - size / 2, centerY - size, centerX + size / 2, centerY);
                        g.DrawLine(pen, centerX + size / 2, centerY, centerX - size / 2, centerY + size);
                    }
                }

                // STEP 4: Draw iOS-Style rounded checkbox
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = node.CheckRectContent;
                    var borderColor = isHovered ? _theme.AccentColor : Color.FromArgb(200, 200, 200);
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : Color.White;

                    using (var checkPath = CreateRoundedRectangle(checkRect, 5))
                    {
                        var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
                        g.FillPath(bgBrush, checkPath);

                        var borderPen = PaintersFactory.GetPen(borderColor, 2f);
                        g.DrawPath(borderPen, checkPath);
                    }

                    // iOS-Style checkmark (smooth)
                    if (node.Item.IsChecked)
                    {
                        var checkPen = PaintersFactory.GetPen(Color.White, 2.5f);
                        checkPen.StartCap = LineCap.Round;
                        checkPen.EndCap = LineCap.Round;
                        checkPen.LineJoin = LineJoin.Round;

                        var points = new Point[]
                        {
                            new Point(checkRect.X + checkRect.Width / 4, checkRect.Y + checkRect.Height / 2),
                            new Point(checkRect.X + checkRect.Width / 2 - 1, checkRect.Y + checkRect.Height * 2 / 3 + 1),
                            new Point(checkRect.X + checkRect.Width * 3 / 4, checkRect.Y + checkRect.Height / 3)
                        };
                        g.DrawLines(checkPen, points);
                    }
                }

                // STEP 5: Draw SF Symbols Style icon
                if (node.IconRectContent != Rectangle.Empty && !string.IsNullOrEmpty(node.Item.ImagePath))
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    PaintIcon(g, iconRect, node.Item.ImagePath);
                }

                // STEP 6: Draw text with SF Pro typography
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = node.TextRectContent;
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                    var renderFont = _regularFont ?? SystemFonts.DefaultFont;
                    TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, renderFont, textRect, textColor,
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
                // Selected: filled rounded rect with shadow
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    // Shadow
                    using (var shadowBrush = new SolidBrush(Color.FromArgb((int)(255 * ShadowOpacity), 0, 0, 0)))
                    {
                        var shadowRect = new Rectangle(
                            nodeBounds.X,
                            nodeBounds.Y + 2,
                            nodeBounds.Width,
                            nodeBounds.Height);

                        using (var shadowPath = CreateRoundedRectangle(shadowRect, CornerRadius))
                        {
                            g.FillPath(shadowBrush, shadowPath);
                        }
                    }

                    // Fill
                    using (var brush = PaintersFactory.GetSolidBrush(_theme.TreeNodeSelectedBackColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
            else if (isHovered)
            {
                // Hover: subtle gray background
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    using (var hoverBrush = new SolidBrush(_theme.TreeNodeHoverBackColor))
                    {
                        g.FillPath(hoverBrush, path);
                    }
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // SF Symbols Style chevron
            Color chevronColor = _theme.AccentColor;

            var pen = PaintersFactory.GetPen(chevronColor, 2f);
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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.iOS15);
                    return;
                }
                catch { }
            }

            PaintDefaultSFSymbol(g, iconRect);
        }

        private void PaintDefaultSFSymbol(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.AccentColor;

            using (var path = new GraphicsPath())
            {
                int padding = iconRect.Width / 5;
                Rectangle innerRect = new Rectangle(
                    iconRect.X + padding,
                    iconRect.Y + padding,
                    iconRect.Width - padding * 2,
                    iconRect.Height - padding * 2);

                // Folder.fill Style
                int tabWidth = innerRect.Width / 3;
                int tabHeight = innerRect.Height / 4;

                // Tab
                path.AddArc(innerRect.Left, innerRect.Top, tabWidth / 2, tabHeight * 2, 90, 90);
                path.AddLine(innerRect.Left + tabWidth / 4, innerRect.Top,
                             innerRect.Left + tabWidth - 2, innerRect.Top);

                // Main body
                path.AddArc(innerRect.Left + tabWidth - 2, innerRect.Top, 4, tabHeight, 270, 90);
                path.AddLine(innerRect.Left + tabWidth, innerRect.Top + tabHeight,
                             innerRect.Right - 4, innerRect.Top + tabHeight);
                path.AddArc(innerRect.Right - 8, innerRect.Top + tabHeight, 8, 8, 270, 90);
                path.AddLine(innerRect.Right, innerRect.Top + tabHeight + 4,
                             innerRect.Right, innerRect.Bottom - 4);
                path.AddArc(innerRect.Right - 8, innerRect.Bottom - 8, 8, 8, 0, 90);
                path.AddLine(innerRect.Right - 4, innerRect.Bottom,
                             innerRect.Left + 4, innerRect.Bottom);
                path.AddArc(innerRect.Left, innerRect.Bottom - 8, 8, 8, 90, 90);
                path.CloseFigure();

                var brush = PaintersFactory.GetSolidBrush(iconColor);
                g.FillPath(brush, path);
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            var renderFont = _regularFont ?? SystemFonts.DefaultFont;
            TextRenderer.DrawText(g, text, renderFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
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

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            // Padding for iOS Style
            rect = new Rectangle(rect.X + GroupPadding, rect.Y + 2, Math.Max(0, rect.Width - GroupPadding * 2), Math.Max(0, rect.Height - 4));

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
            // iOS comfortable touch targets
            return Math.Max(36, base.GetPreferredRowHeight(item, font));
        }
    }
}
