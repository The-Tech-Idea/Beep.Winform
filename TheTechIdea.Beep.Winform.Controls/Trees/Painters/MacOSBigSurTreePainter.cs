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
    /// macOS Big Sur sidebar tree painter.
    /// Features: Translucent backgrounds, vibrancy, sidebar Style, icon badges.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class MacOSBigSurTreePainter : BaseTreePainter
    {
        private const int CornerRadius =6;
        private const int SidebarPadding =4;
        private const float VibrancyAlpha =0.7f;

        private Font _regularFont;
        private Font _boldFont;
        private static readonly Random _rand = new();

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
        /// macOS Big Sur-specific node painting with translucent vibrancy effects.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for macOS smooth appearance
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP1: Draw translucent background (vibrancy effect)
                if (isSelected || isHovered)
                {
                    using (var bgPath = CreateRoundedRectangle(nodeBounds, CornerRadius))
                    {
                        float alpha = isSelected ? VibrancyAlpha :0.5f;
                        Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;
                        Color translucentColor = Color.FromArgb((int)(255 * alpha), bgColor);

                        var bgBrush = PaintersFactory.GetSolidBrush(translucentColor);
                        g.FillPath(bgBrush, bgPath);

                        // STEP2: Subtle highlight on top edge (macOS gloss)
                        if (isSelected)
                        {
                            var highlightPen = PaintersFactory.GetPen(Color.FromArgb(30, Color.White),1f);
                            g.DrawLine(highlightPen,
                                nodeBounds.Left + CornerRadius + SidebarPadding,
                                nodeBounds.Top +3,
                                nodeBounds.Right - CornerRadius - SidebarPadding,
                                nodeBounds.Top +3);
                        }
                    }
                }

                // STEP3: Draw macOS disclosure triangle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count >0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = node.ToggleRectContent;
                    Color triangleColor = _theme.TreeForeColor;

                    var brush = PaintersFactory.GetSolidBrush(triangleColor);

                    int centerX = toggleRect.Left + toggleRect.Width /2;
                    int centerY = toggleRect.Top + toggleRect.Height /2;
                    int size = Math.Min(toggleRect.Width, toggleRect.Height) /3;

                    Point[] triangle;

                    if (node.Item.IsExpanded)
                    {
                        triangle = new Point[]
                        {
                            new Point(centerX - size, centerY - size /2),
                            new Point(centerX + size, centerY - size /2),
                            new Point(centerX, centerY + size /2)
                        };
                    }
                    else
                    {
                        triangle = new Point[]
                        {
                            new Point(centerX - size /2, centerY - size),
                            new Point(centerX + size /2, centerY),
                            new Point(centerX - size /2, centerY + size)
                        };
                    }

                    g.FillPolygon(brush, triangle);
                }

                // STEP4: Draw macOS checkbox (rounded square)
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = node.CheckRectContent;
                    var borderColor = isHovered ? _theme.AccentColor : Color.FromArgb(180,180,180);
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : Color.White;

                    using (var checkPath = CreateRoundedRectangle(checkRect,4))
                    {
                        var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
                        g.FillPath(bgBrush, checkPath);

                        var borderPen = PaintersFactory.GetPen(borderColor,1.5f);
                        g.DrawPath(borderPen, checkPath);
                    }

                    if (node.Item.IsChecked)
                    {
                        var checkPen = PaintersFactory.GetPen(Color.White,2f);
                        checkPen.StartCap = LineCap.Round;
                        checkPen.EndCap = LineCap.Round;

                        var points = new Point[]
                        {
                            new Point(checkRect.X + checkRect.Width /4, checkRect.Y + checkRect.Height /2),
                            new Point(checkRect.X + checkRect.Width /2 -1, checkRect.Y + checkRect.Height *2 /3),
                            new Point(checkRect.X + checkRect.Width *3 /4 +1, checkRect.Y + checkRect.Height /3)
                        };
                        g.DrawLines(checkPen, points);
                    }
                }

                // STEP5: Draw macOS icon with gradient
                if (node.IconRectContent != Rectangle.Empty && !string.IsNullOrEmpty(node.Item.ImagePath))
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    PaintIcon(g, iconRect, node.Item.ImagePath);
                }

                // STEP6: Draw text with macOS typography
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
            if (nodeBounds.Width <=0 || nodeBounds.Height <=0) return;

            if (isSelected)
            {
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    Color fillColor = Color.FromArgb((int)(255 * VibrancyAlpha), _theme.TreeNodeSelectedBackColor);
                    var brush = PaintersFactory.GetSolidBrush(fillColor);
                    g.FillPath(brush, path);

                    var highlightPen = PaintersFactory.GetPen(Color.FromArgb(30, Color.White),1f);
                    g.DrawLine(highlightPen,
                        nodeBounds.Left + CornerRadius,
                        nodeBounds.Top +2,
                        nodeBounds.Right - CornerRadius,
                        nodeBounds.Top +2);
                }
            }
            else if (isHovered)
            {
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    Color hoverColor = Color.FromArgb((int)(255 *0.5f), _theme.TreeNodeHoverBackColor);
                    var hoverBrush = PaintersFactory.GetSolidBrush(hoverColor);
                    g.FillPath(hoverBrush, path);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <=0 || toggleRect.Height <=0) return;

            Color triangleColor = _theme.TreeForeColor;
            var brush = PaintersFactory.GetSolidBrush(triangleColor);

            int centerX = toggleRect.Left + toggleRect.Width /2;
            int centerY = toggleRect.Top + toggleRect.Height /2;
            int size = Math.Min(toggleRect.Width, toggleRect.Height) /3;

            Point[] triangle;

            if (isExpanded)
            {
                triangle = new Point[]
                {
                    new Point(centerX - size, centerY - size /2),
                    new Point(centerX + size, centerY - size /2),
                    new Point(centerX, centerY + size /2)
                };
            }
            else
            {
                triangle = new Point[]
                {
                    new Point(centerX - size /2, centerY - size),
                    new Point(centerX + size /2, centerY),
                    new Point(centerX - size /2, centerY + size)
                };
            }

            g.FillPolygon(brush, triangle);
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, string imagePath)
        {
            if (iconRect.Width <=0 || iconRect.Height <=0) return;

            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.MacOSBigSur);
                    return;
                }
                catch { }
            }

            // Default macOS icon (rounded square with gradient)
            PaintDefaultMacIcon(g, iconRect);
        }

        private void PaintDefaultMacIcon(Graphics g, Rectangle iconRect)
        {
            // macOS Style icon with gradient and rounded corners
            using (var path = CreateRoundedRectangle(iconRect, iconRect.Width /5))
            {
                Color topColor = _theme.AccentColor;
                Color bottomColor = ControlPaint.Dark(_theme.AccentColor,0.2f);

                using (var brush = new LinearGradientBrush(iconRect, topColor, bottomColor, LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }

                Rectangle glossRect = new Rectangle(iconRect.X, iconRect.Y, iconRect.Width, iconRect.Height /2);
                using (var glossPath = CreateRoundedRectangle(glossRect, iconRect.Width /5))
                {
                    var glossBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(40, Color.White));
                    g.FillPath(glossBrush, glossPath);
                }

                var borderPen = PaintersFactory.GetPen(Color.FromArgb(60, Color.Black),1f);
                g.DrawPath(borderPen, path);
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <=0 || textRect.Height <=0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            var renderFont = _regularFont ?? SystemFonts.DefaultFont;
            TextRenderer.DrawText(g, text, renderFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <=0 || bounds.Height <=0) return;

            var brush = PaintersFactory.GetSolidBrush(_theme.TreeBackColor);
            g.FillRectangle(brush, bounds);

            PaintVibrancyTexture(g, bounds);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            base.Paint(g, owner, bounds);
        }

        private void PaintVibrancyTexture(Graphics g, Rectangle bounds)
        {
            var dotPen = PaintersFactory.GetPen(Color.FromArgb(2, _theme.TreeForeColor),1);
            int count = Math.Max(1, bounds.Width * bounds.Height /3000);
            for (int i =0; i < count; i++)
            {
                int x = _rand.Next(bounds.Left, bounds.Right);
                int y = _rand.Next(bounds.Top, bounds.Bottom);
                g.DrawRectangle(dotPen, x, y,1,1);
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius *2;

            rect = new Rectangle(rect.X + SidebarPadding, rect.Y +2, Math.Max(0, rect.Width - SidebarPadding *2), Math.Max(0, rect.Height -4));

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
            // macOS sidebar comfortable spacing
            return Math.Max(28, base.GetPreferredRowHeight(item, font));
        }
    }
}
