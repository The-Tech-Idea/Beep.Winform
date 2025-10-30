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
    /// Discord tree painter.
    /// Features: Server/channel Style, colored indicators, rounded selections, icons.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class DiscordTreePainter : BaseTreePainter
    {
        private const int CornerRadius =4;
        private const int IndicatorWidth =4;

        private Font _regularFont;
        private Font _symbolFont;

        public override void Initialize(BeepTree owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            _regularFont = owner?.TextFont ?? SystemFonts.DefaultFont;
            try { _symbolFont?.Dispose(); } catch { }
            _symbolFont = new Font(_regularFont.FontFamily, Math.Max(6f, _regularFont.Size *0.6f), FontStyle.Bold);
        }

        /// <summary>
        /// Discord-specific node painting with server/channel Style.
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
                // Background
                if (isSelected || isHovered)
                {
                    Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;
                    var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
                    using (var nodePath = CreateRoundedRectangle(nodeBounds, CornerRadius))
                    {
                        g.FillPath(bgBrush, nodePath);
                    }
                }

                // Left indicator pill
                if (isSelected)
                {
                    Rectangle indicator = new Rectangle(nodeBounds.Left, nodeBounds.Top + nodeBounds.Height /4, IndicatorWidth, nodeBounds.Height /2);
                    var indicatorBrush = PaintersFactory.GetSolidBrush(_theme.AccentColor);
                    g.FillRectangle(indicatorBrush, indicator);
                }
                else if (isHovered)
                {
                    Rectangle indicator = new Rectangle(nodeBounds.Left, nodeBounds.Top + nodeBounds.Height /3, IndicatorWidth /2, nodeBounds.Height /3);
                    var indicatorBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(100, _theme.TreeForeColor));
                    g.FillRectangle(indicatorBrush, indicator);
                }

                // Toggle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count >0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = _owner.LayoutHelper.TransformToViewport(node.ToggleRectContent);
                    Color arrowColor = _theme.TreeForeColor;
                    var pen = PaintersFactory.GetPen(arrowColor,1.5f);
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;

                    int centerX = toggleRect.Left + toggleRect.Width /2;
                    int centerY = toggleRect.Top + toggleRect.Height /2;
                    int size = Math.Min(toggleRect.Width, toggleRect.Height) /3;

                    if (node.Item.IsExpanded)
                    {
                        g.DrawLine(pen, centerX - size, centerY - size /2, centerX, centerY + size /2);
                        g.DrawLine(pen, centerX, centerY + size /2, centerX + size, centerY - size /2);
                    }
                    else
                    {
                        g.DrawLine(pen, centerX - size /2, centerY - size, centerX + size /2, centerY);
                        g.DrawLine(pen, centerX + size /2, centerY, centerX - size /2, centerY + size);
                    }
                }

                // Checkbox
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = _owner.LayoutHelper.TransformToViewport(node.CheckRectContent);
                    var borderColor = node.Item.IsChecked ? _theme.AccentColor : _theme.BorderColor;
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

                    using (var checkPath = CreateRoundedRectangle(checkRect,2))
                    {
                        var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
                        g.FillPath(bgBrush, checkPath);

                        var borderPen = PaintersFactory.GetPen(borderColor,1f);
                        g.DrawPath(borderPen, checkPath);
                    }

                    if (node.Item.IsChecked)
                    {
                        var checkPen = PaintersFactory.GetPen(Color.White,1.5f);
                        var points = new Point[]
                        {
                            new Point(checkRect.X + checkRect.Width /4, checkRect.Y + checkRect.Height /2),
                            new Point(checkRect.X + checkRect.Width /2 -1, checkRect.Y + checkRect.Height *3 /4),
                            new Point(checkRect.X + checkRect.Width *3 /4, checkRect.Y + checkRect.Height /4)
                        };
                        g.DrawLines(checkPen, points);
                    }
                }

                // Icon
                if (node.IconRectContent != Rectangle.Empty)
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    if (!string.IsNullOrEmpty(node.Item.ImagePath))
                    {
                        try { Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, node.Item.ImagePath); } catch { }
                    }
                    else
                    {
                        Color iconColor = _theme.AccentColor;
                        using (var iconPath = CreateRoundedRectangle(iconRect, iconRect.Width /4))
                        {
                            var bgBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(60, iconColor));
                            g.FillPath(bgBrush, iconPath);

                            var textBrush = PaintersFactory.GetSolidBrush(iconColor);
                            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                            g.DrawString("#", _symbolFont, textBrush, iconRect, sf);
                        }
                    }
                }

                // Text
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = _owner.LayoutHelper.TransformToViewport(node.TextRectContent);
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;
                    var renderFont = isSelected ? new Font(_regularFont, _regularFont.Style | FontStyle.Bold) : _regularFont;
                    TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, renderFont, textRect, textColor,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                    if (isSelected) renderFont.Dispose();
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
            if (nodeBounds.Width <=0 || nodeBounds.Height <=0) return;

            if (isSelected)
            {
                var path = CreateRoundedRectangle(nodeBounds, CornerRadius);
                var brush = PaintersFactory.GetSolidBrush(_theme.TreeNodeSelectedBackColor);
                g.FillPath(brush, path);

                Rectangle indicator = new Rectangle(nodeBounds.Left, nodeBounds.Top + nodeBounds.Height /4, IndicatorWidth, nodeBounds.Height /2);
                var indicatorBrush = PaintersFactory.GetSolidBrush(_theme.AccentColor);
                g.FillRectangle(indicatorBrush, indicator);
            }
            else if (isHovered)
            {
                var path = CreateRoundedRectangle(nodeBounds, CornerRadius);
                var hoverBrush = PaintersFactory.GetSolidBrush(_theme.TreeNodeHoverBackColor);
                g.FillPath(hoverBrush, path);

                Rectangle indicator = new Rectangle(nodeBounds.Left, nodeBounds.Top + nodeBounds.Height /3, IndicatorWidth /2, nodeBounds.Height /3);
                var indicatorBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(100, _theme.TreeForeColor));
                g.FillRectangle(indicatorBrush, indicator);
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <=0 || toggleRect.Height <=0) return;

            Color arrowColor = _theme.TreeForeColor;
            var pen = PaintersFactory.GetPen(arrowColor,1.5f);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;

            int centerX = toggleRect.Left + toggleRect.Width /2;
            int centerY = toggleRect.Top + toggleRect.Height /2;
            int size = Math.Min(toggleRect.Width, toggleRect.Height) /3;

            if (isExpanded)
            {
                g.DrawLine(pen, centerX - size, centerY - size /2, centerX, centerY + size /2);
                g.DrawLine(pen, centerX, centerY + size /2, centerX + size, centerY - size /2);
            }
            else
            {
                g.DrawLine(pen, centerX - size /2, centerY - size, centerX + size /2, centerY);
                g.DrawLine(pen, centerX + size /2, centerY, centerX - size /2, centerY + size);
            }
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, string imagePath)
        {
            if (iconRect.Width <=0 || iconRect.Height <=0) return;

            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath);
                    return;
                }
                catch { }
            }

            PaintDefaultDiscordIcon(g, iconRect);
        }

        private void PaintDefaultDiscordIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.AccentColor;
            using (var path = CreateRoundedRectangle(iconRect, iconRect.Width /4))
            {
                var bgBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(60, iconColor));
                g.FillPath(bgBrush, path);

                var textBrush = PaintersFactory.GetSolidBrush(iconColor);
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("#", _symbolFont, textBrush, iconRect, sf);
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <=0 || textRect.Height <=0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;
            TextRenderer.DrawText(g, text, _regularFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <=0 || bounds.Height <=0) return;

            var brush = PaintersFactory.GetSolidBrush(_theme.TreeBackColor);
            g.FillRectangle(brush, bounds);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            base.Paint(g, owner, bounds);
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius *2;

            // Padding (Discord has more padding on left)
            rect = new Rectangle(rect.X +8, rect.Y +2, rect.Width -12, rect.Height -4);

            if (rect.Width < diameter || rect.Height < diameter)
            {
                path.AddRectangle(rect);
                return path;
            }

            path.AddArc(rect.X, rect.Y, diameter, diameter,180,90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter,270,90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter,0,90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter,90,90);
            path.CloseFigure();

            return path;
        }

        public override int GetPreferredRowHeight(SimpleItem item, Font font)
        {
            return Math.Max(32, base.GetPreferredRowHeight(item, font));
        }
    }
}
