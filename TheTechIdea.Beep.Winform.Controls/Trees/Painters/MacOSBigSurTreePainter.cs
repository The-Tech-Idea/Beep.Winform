using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// macOS Big Sur sidebar tree painter.
    /// Features: Translucent backgrounds, vibrancy, sidebar style, icon badges.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class MacOSBigSurTreePainter : BaseTreePainter
    {
        private const int CornerRadius = 6;
        private const int SidebarPadding = 4;
        private const float VibrancyAlpha = 0.7f;

        /// <summary>
        /// macOS Big Sur-specific node painting with translucent vibrancy effects.
        /// Features: Semi-transparent backgrounds (70% opacity), subtle top highlights, disclosure triangles, gradient icons.
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
                // STEP 1: Draw translucent background (vibrancy effect)
                if (isSelected || isHovered)
                {
                    using (var bgPath = CreateRoundedRectangle(nodeBounds, CornerRadius))
                    {
                        // Semi-transparent fill for macOS vibrancy
                        float alpha = isSelected ? VibrancyAlpha : 0.5f;
                        Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;
                        Color translucentColor = Color.FromArgb((int)(255 * alpha), bgColor);

                        using (var bgBrush = new SolidBrush(translucentColor))
                        {
                            g.FillPath(bgBrush, bgPath);
                        }

                        // STEP 2: Subtle highlight on top edge (macOS gloss)
                        if (isSelected)
                        {
                            using (var highlightPen = new Pen(Color.FromArgb(30, Color.White), 1f))
                            {
                                g.DrawLine(highlightPen,
                                    nodeBounds.Left + CornerRadius + SidebarPadding,
                                    nodeBounds.Top + 3,
                                    nodeBounds.Right - CornerRadius - SidebarPadding,
                                    nodeBounds.Top + 3);
                            }
                        }
                    }
                }

                // STEP 3: Draw macOS disclosure triangle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = node.ToggleRectContent;
                    Color triangleColor = _theme.TreeForeColor;

                    using (var brush = new SolidBrush(triangleColor))
                    {
                        int centerX = toggleRect.Left + toggleRect.Width / 2;
                        int centerY = toggleRect.Top + toggleRect.Height / 2;
                        int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                        Point[] triangle;

                        if (node.Item.IsExpanded)
                        {
                            // Triangle pointing down
                            triangle = new Point[]
                            {
                                new Point(centerX - size, centerY - size / 2),
                                new Point(centerX + size, centerY - size / 2),
                                new Point(centerX, centerY + size / 2)
                            };
                        }
                        else
                        {
                            // Triangle pointing right
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

                // STEP 4: Draw macOS checkbox (rounded square)
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = node.CheckRectContent;
                    var borderColor = isHovered ? _theme.AccentColor : Color.FromArgb(180, 180, 180);
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : Color.White;

                    // macOS checkbox with rounded corners
                    using (var checkPath = CreateRoundedRectangle(checkRect, 4))
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

                    // macOS-style checkmark
                    if (node.Item.IsChecked)
                    {
                        using (var checkPen = new Pen(Color.White, 2f))
                        {
                            checkPen.StartCap = LineCap.Round;
                            checkPen.EndCap = LineCap.Round;

                            var points = new Point[]
                            {
                                new Point(checkRect.X + checkRect.Width / 4, checkRect.Y + checkRect.Height / 2),
                                new Point(checkRect.X + checkRect.Width / 2 - 1, checkRect.Y + checkRect.Height * 2 / 3),
                                new Point(checkRect.X + checkRect.Width * 3 / 4 + 1, checkRect.Y + checkRect.Height / 3)
                            };
                            g.DrawLines(checkPen, points);
                        }
                    }
                }

                // STEP 5: Draw macOS icon with gradient
                if (!string.IsNullOrEmpty(node.Item.ImagePath) && node.IconRectContent != Rectangle.Empty)
                {
                    PaintIcon(g, node.IconRectContent, node.Item.ImagePath);
                }

                // STEP 6: Draw text with macOS typography
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = node.TextRectContent;
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

                    // SF Pro / Segoe UI
                    using (var renderFont = new Font("Segoe UI", _owner.TextFont.Size, FontStyle.Regular))
                    {
                        TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, renderFont, textRect, textColor,
                            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
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
                // Selected: translucent filled background (sidebar selection)
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    // Semi-transparent fill for vibrancy effect
                    Color fillColor = Color.FromArgb((int)(255 * VibrancyAlpha), _theme.TreeNodeSelectedBackColor);
                    using (var brush = new SolidBrush(fillColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Subtle highlight on top edge
                    using (var highlightPen = new Pen(Color.FromArgb(30, Color.White), 1f))
                    {
                        g.DrawLine(highlightPen,
                            nodeBounds.Left + CornerRadius,
                            nodeBounds.Top + 2,
                            nodeBounds.Right - CornerRadius,
                            nodeBounds.Top + 2);
                    }
                }
            }
            else if (isHovered)
            {
                // Hover: very subtle background
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    Color hoverColor = Color.FromArgb((int)(255 * 0.5f), _theme.TreeNodeHoverBackColor);
                    using (var hoverBrush = new SolidBrush(hoverColor))
                    {
                        g.FillPath(hoverBrush, path);
                    }
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // macOS disclosure triangle
            Color triangleColor = _theme.TreeForeColor;

            using (var brush = new SolidBrush(triangleColor))
            {
                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                Point[] triangle;

                if (isExpanded)
                {
                    // Triangle pointing down
                    triangle = new Point[]
                    {
                        new Point(centerX - size, centerY - size / 2),
                        new Point(centerX + size, centerY - size / 2),
                        new Point(centerX, centerY + size / 2)
                    };
                }
                else
                {
                    // Triangle pointing right
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
            // macOS style icon with gradient and rounded corners
            using (var path = CreateRoundedRectangle(iconRect, iconRect.Width / 5))
            {
                // Gradient fill
                Color topColor = _theme.AccentColor;
                Color bottomColor = ControlPaint.Dark(_theme.AccentColor, 0.2f);

                using (var brush = new LinearGradientBrush(iconRect, topColor, bottomColor, LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }

                // Gloss effect (top half lighter)
                Rectangle glossRect = new Rectangle(
                    iconRect.X,
                    iconRect.Y,
                    iconRect.Width,
                    iconRect.Height / 2);

                using (var glossPath = CreateRoundedRectangle(glossRect, iconRect.Width / 5))
                {
                    using (var glossBrush = new SolidBrush(Color.FromArgb(40, Color.White)))
                    {
                        g.FillPath(glossBrush, glossPath);
                    }
                }

                // Subtle border
                using (var borderPen = new Pen(Color.FromArgb(60, Color.Black), 1f))
                {
                    g.DrawPath(borderPen, path);
                }
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // SF Pro / Segoe UI
            Font renderFont = new Font("Segoe UI", font.Size, isSelected ? FontStyle.Regular : FontStyle.Regular);

            TextRenderer.DrawText(g, text, renderFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            renderFont.Dispose();
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            // Background with vibrancy simulation
            using (var brush = new SolidBrush(_theme.TreeBackColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Very subtle texture for depth
            PaintVibrancyTexture(g, bounds);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            base.Paint(g, owner, bounds);
        }

        private void PaintVibrancyTexture(Graphics g, Rectangle bounds)
        {
            // Subtle noise for vibrancy effect
            Random rand = new Random(bounds.GetHashCode());
            using (var pen = new Pen(Color.FromArgb(2, _theme.TreeForeColor)))
            {
                for (int i = 0; i < bounds.Width * bounds.Height / 3000; i++)
                {
                    int x = rand.Next(bounds.Left, bounds.Right);
                    int y = rand.Next(bounds.Top, bounds.Bottom);
                    g.DrawRectangle(pen, x, y, 1, 1);
                }
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            // Padding
            rect = new Rectangle(rect.X + SidebarPadding, rect.Y + 2, rect.Width - SidebarPadding * 2, rect.Height - 4);

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
            // macOS sidebar comfortable spacing
            return Math.Max(28, base.GetPreferredRowHeight(item, font));
        }
    }
}
