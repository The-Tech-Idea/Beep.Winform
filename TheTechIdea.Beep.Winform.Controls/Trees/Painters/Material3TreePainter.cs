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
    /// Google Material Design3 tree painter.
    /// Features: Elevation shadows, rounded corners, material colors, state layers.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class Material3TreePainter : BaseTreePainter
    {
        private const int ElevationOffset = 2;
        private const int CornerRadius = 8;
        private const int StateLayerAlpha = 12;

        private Font _regularFont;
        private Font _boldFont;

        public override void Initialize(BeepTree owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            // Cache fonts to avoid allocating per-node
            _regularFont = owner?.TextFont ?? SystemFonts.DefaultFont;
            if (_boldFont == null || _boldFont.Size != _regularFont.Size || !_boldFont.FontFamily.Equals(_regularFont.FontFamily))
            {
                try
                {
                    _boldFont?.Dispose();
                }
                catch { }
                _boldFont = new Font(_regularFont.FontFamily, _regularFont.Size, FontStyle.Bold);
            }
        }

        /// <summary>
        /// Material3-specific node painting with elevation shadows, state layers, and rounded corners.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            var oldSmoothing = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            try
            {
                // STEP 1: Draw elevation shadow FIRST (if selected)
                if (isSelected)
                {
                    using (var shadowPath = CreateRoundedRectangle(
                        new Rectangle(
                            nodeBounds.X + ElevationOffset,
                            nodeBounds.Y + ElevationOffset,
                            nodeBounds.Width,
                            nodeBounds.Height),
                        CornerRadius))
                    {
                        var shadowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(40, 0, 0, 0));
                        g.FillPath(shadowBrush, shadowPath);
                    }
                }

                // STEP 2: Draw rounded background surface
                using (var surfacePath = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    Color surfaceColor = isSelected ? _theme.TreeNodeSelectedBackColor :
                                        isHovered ? _theme.TreeNodeHoverBackColor :
                                        _theme.TreeBackColor;

                    var surfaceBrush = PaintersFactory.GetSolidBrush(surfaceColor);
                    g.FillPath(surfaceBrush, surfacePath);

                    // STEP 3: Draw state layer (Material Design overlay)
                    if (isSelected)
                    {
                        var stateBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(StateLayerAlpha * 2, _theme.AccentColor));
                        g.FillPath(stateBrush, surfacePath);
                    }
                    else if (isHovered)
                    {
                        var stateBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(StateLayerAlpha, _theme.TreeForeColor));
                        g.FillPath(stateBrush, surfacePath);
                    }
                }

                // STEP 4: Draw toggle with ripple effect
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = node.ToggleRectContent;
                    Color iconColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;

                    // Ripple effect background (circular)
                    if (isHovered)
                    {
                        var rippleBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(StateLayerAlpha * 2, iconColor));
                        g.FillEllipse(rippleBrush, toggleRect);
                    }

                    var pen = PaintersFactory.GetPen(iconColor, 2f);
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

                // STEP 5: Draw checkbox with Material Design styling
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = node.CheckRectContent;
                    var borderColor = isHovered ? _theme.AccentColor : _theme.BorderColor;
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

                    using (var checkPath = CreateRoundedRectangle(checkRect, 2))
                    {
                        var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
                        g.FillPath(bgBrush, checkPath);

                        var borderPen = PaintersFactory.GetPen(borderColor, 2f);
                        g.DrawPath(borderPen, checkPath);
                    }

                    // Checkmark with Material animation feel
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

                // STEP 6: Draw Material icon
                if (node.IconRectContent != Rectangle.Empty && !string.IsNullOrEmpty(node.Item.ImagePath))
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    PaintIcon(g, iconRect, node.Item.ImagePath);
                }

                // STEP 7: Draw text with Material typography
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = node.TextRectContent;
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                    var renderFont = isSelected ? _boldFont ?? _regularFont : _regularFont;
                    TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, renderFont, textRect, textColor,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            }
            finally
            {
                g.SmoothingMode = oldSmoothing;
            }
        }

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            if (isSelected)
            {
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    var shadowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(40, 0, 0, 0));
                    var shadowRect = new Rectangle(
                        nodeBounds.X + ElevationOffset,
                        nodeBounds.Y + ElevationOffset,
                        nodeBounds.Width,
                        nodeBounds.Height);

                    using (var shadowPath = CreateRoundedRectangle(shadowRect, CornerRadius))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }

                    var brush = PaintersFactory.GetSolidBrush(_theme.TreeNodeSelectedBackColor);
                    g.FillPath(brush, path);

                    var stateBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(StateLayerAlpha * 2, _theme.AccentColor));
                    g.FillPath(stateBrush, path);
                }
            }
            else if (isHovered)
            {
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    var hoverBrush = PaintersFactory.GetSolidBrush(_theme.TreeNodeHoverBackColor);
                    g.FillPath(hoverBrush, path);

                    var stateBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(StateLayerAlpha, _theme.TreeForeColor));
                    g.FillPath(stateBrush, path);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            Color iconColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;

            if (isHovered)
            {
                var rippleBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(StateLayerAlpha, iconColor));
                g.FillEllipse(rippleBrush, toggleRect);
            }

            var pen = PaintersFactory.GetPen(iconColor, 2f);
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

            PaintDefaultMaterialIcon(g, iconRect);
        }

        private void PaintDefaultMaterialIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.AccentColor;

            using (var path = CreateRoundedRectangle(iconRect, 4))
            {
                var brush = PaintersFactory.GetSolidBrush(Color.FromArgb(100, iconColor));
                g.FillPath(brush, path);
            }

            int padding = iconRect.Width / 4;
            Rectangle symbolRect = new Rectangle(
                iconRect.X + padding,
                iconRect.Y + padding,
                iconRect.Width - padding * 2,
                iconRect.Height - padding * 2);

            var pen = PaintersFactory.GetPen(iconColor, 1.5f);
            pen.LineJoin = LineJoin.Round;
            g.DrawRectangle(pen, symbolRect);
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;
            var renderFont = isSelected ? _boldFont ?? _regularFont : _regularFont;

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

            // Shrink for padding
            rect = new Rectangle(rect.X + 2, rect.Y + 2, Math.Max(0, rect.Width - 4), Math.Max(0, rect.Height - 4));

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
            // Material Design needs comfortable touch targets (48dp minimum)
            return Math.Max(32, base.GetPreferredRowHeight(item, font));
        }
    }
}
