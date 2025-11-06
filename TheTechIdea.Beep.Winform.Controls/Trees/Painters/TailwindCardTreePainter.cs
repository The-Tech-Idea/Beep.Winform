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
    /// Tailwind CSS card-based tree painter.
    /// Features: Card containers for nodes, utility-first design, shadow layers, clean spacing.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class TailwindCardTreePainter : BaseTreePainter
    {
        private const int CornerRadius = 8;
        private const int CardPadding = 6;
        private const int ShadowOffset = 2;

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
        /// Tailwind CSS-specific node painting with utility-first design.
        /// Features: Layered shadows (shadow-lg on selection, shadow-md on hover), ring borders (2px), rounded corners (8px), gradient icons.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for Tailwind clean appearance
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw Tailwind shadow layers FIRST
                if (isSelected || isHovered)
                {
                    using (var nodePath = CreateRoundedRectangle(nodeBounds, CornerRadius))
                    {
                        // Tailwind shadow layers (utility-first approach)
                        if (isSelected)
                        {
                            // shadow-lg: multiple shadow layers
                            var shadows = new[]
                            {
                                new { Offset = 3, Alpha = 35, Color = Color.Black },
                                new { Offset = 2, Alpha = 25, Color = Color.Black },
                                new { Offset = 1, Alpha = 15, Color = Color.Black }
                            };

                            foreach (var shadow in shadows)
                            {
                                var shadowRect = new Rectangle(
                                    nodeBounds.X,
                                    nodeBounds.Y + shadow.Offset,
                                    nodeBounds.Width,
                                    nodeBounds.Height);

                                using (var shadowPath = CreateRoundedRectangle(shadowRect, CornerRadius))
                                {
                                    var shadowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(shadow.Alpha, shadow.Color));
                                    g.FillPath(shadowBrush, shadowPath);
                                }
                            }
                        }
                        else if (isHovered)
                        {
                            // shadow-md: single subtle shadow
                            var shadowRect = new Rectangle(
                                nodeBounds.X,
                                nodeBounds.Y + 1,
                                nodeBounds.Width,
                                nodeBounds.Height);

                            using (var shadowPath = CreateRoundedRectangle(shadowRect, CornerRadius))
                            {
                                var shadowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(20, 0, 0, 0));
                                g.FillPath(shadowBrush, shadowPath);
                            }
                        }

                        // STEP 2: Draw card background
                        Color bgColor = isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor;
                        var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
                        g.FillPath(bgBrush, nodePath);

                        // STEP 3: Tailwind ring border (utility: ring-2)
                        if (isSelected)
                        {
                            var ringPen = PaintersFactory.GetPen(_theme.AccentColor, 2f);
                            g.DrawPath(ringPen, nodePath);
                        }
                        else if (isHovered)
                        {
                            var ringPen = PaintersFactory.GetPen(Color.FromArgb(100, _theme.AccentColor), 1f);
                            g.DrawPath(ringPen, nodePath);
                        }
                    }
                }

                // STEP 4: Draw Tailwind chevron toggle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = _owner.LayoutHelper.TransformToViewport(node.ToggleRectContent);

                    // Hover background (rounded-full bg-opacity-20)
                    if (isHovered)
                    {
                        var hoverBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(30, _theme.AccentColor));
                        g.FillEllipse(hoverBrush, toggleRect);
                    }

                    var pen = PaintersFactory.GetPen(_theme.TreeForeColor, 2f);
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

                // STEP 5: Draw Tailwind checkbox (rounded-md border-2)
                if (_owner.ShowCheckBox && node.CheckRectContent != Rectangle.Empty)
                {
                    var checkRect = _owner.LayoutHelper.TransformToViewport(node.CheckRectContent);
                    var borderColor = node.Item.IsChecked ? _theme.AccentColor : _theme.BorderColor;
                    var bgColor = node.Item.IsChecked ? _theme.AccentColor : _theme.TreeBackColor;

                    using (var checkPath = CreateRoundedRectangle(checkRect, 3))
                    {
                        var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
                        g.FillPath(bgBrush, checkPath);

                        // Tailwind border-2
                        float borderWidth = node.Item.IsChecked ? 2f : 1.5f;
                        var borderPen = PaintersFactory.GetPen(borderColor, borderWidth);
                        g.DrawPath(borderPen, checkPath);
                    }

                    // Tailwind checkmark
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

                // STEP 6: Draw Tailwind gradient icon
                if (node.IconRectContent != Rectangle.Empty && !string.IsNullOrEmpty(node.Item.ImagePath))
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    PaintIcon(g, iconRect, node.Item.ImagePath);
                }

                // STEP 7: Draw text with Tailwind typography
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = _owner.LayoutHelper.TransformToViewport(node.TextRectContent);
                    Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;
                    var renderFont = isSelected ? _boldFont ?? _regularFont : _regularFont;
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
                // Selected: card with shadow (shadow-lg)
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    // Shadow layer
                    var shadowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(30, 0, 0, 0));
                    var shadowRect = new Rectangle(
                        nodeBounds.X,
                        nodeBounds.Y + ShadowOffset,
                        nodeBounds.Width,
                        nodeBounds.Height);

                    using (var shadowPath = CreateRoundedRectangle(shadowRect, CornerRadius))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }

                    // Card
                    using (var brush = PaintersFactory.GetSolidBrush(_theme.TreeNodeSelectedBackColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Ring (border)
                    using (var pen = PaintersFactory.GetPen(_theme.AccentColor, 2f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
            else if (isHovered)
            {
                // Hover: subtle card with shadow (shadow-md)
                using (var path = CreateRoundedRectangle(nodeBounds, CornerRadius))
                {
                    // Shadow
                    var shadowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(15, 0, 0, 0));
                    var shadowRect = new Rectangle(
                        nodeBounds.X,
                        nodeBounds.Y + 1,
                        nodeBounds.Width,
                        nodeBounds.Height);

                    using (var shadowPath = CreateRoundedRectangle(shadowRect, CornerRadius))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }

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

            // Tailwind chevron
            Color chevronColor = _theme.TreeForeColor;

            if (isHovered)
            {
                // Hover background
                var hoverBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(20, _theme.AccentColor));
                g.FillEllipse(hoverBrush, toggleRect);
            }

            var pen = PaintersFactory.GetPen(chevronColor, 2f);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            int centerX = toggleRect.Left + toggleRect.Width / 2;
            int centerY = toggleRect.Top + toggleRect.Height / 2;
            int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;
            if (isExpanded)
            {
                g.DrawLine(pen, centerX - size, centerY - size / 2, centerX, centerY + size / 2);
                g.DrawLine(pen, centerX, centerY + size / 2, centerX + size, centerY - size / 2);
            }
            else
            {
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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.TailwindCard);
                    return;
                }
                catch { }
            }

            // Default: Tailwind-Style icon
            PaintDefaultTailwindIcon(g, iconRect);
        }

        private void PaintDefaultTailwindIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.AccentColor;

            // Rounded square with gradient
            using (var path = CreateRoundedRectangle(iconRect, iconRect.Width / 4))
            {
                // Gradient background
                Color topColor = Color.FromArgb(120, iconColor);
                Color bottomColor = Color.FromArgb(80, iconColor);

                using (var brush = new LinearGradientBrush(iconRect, topColor, bottomColor, LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }

                var pen = PaintersFactory.GetPen(Color.FromArgb(200, iconColor), 1.5f);
                g.DrawRectangle(pen, new Rectangle(iconRect.X + iconRect.Width / 4, iconRect.Y + iconRect.Height / 4, iconRect.Width / 2, iconRect.Height / 2));
            }
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

            // Background
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

            // Card padding
            rect = new Rectangle(rect.X + CardPadding, rect.Y + 2, Math.Max(0, rect.Width - CardPadding * 2), Math.Max(0, rect.Height - 4));

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
            // Tailwind comfortable spacing
            return Math.Max(36, base.GetPreferredRowHeight(item, font));
        }
    }
}
