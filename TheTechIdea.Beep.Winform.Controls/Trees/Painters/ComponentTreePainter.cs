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
    /// Component/widget tree painter (Figma/VS Code Style).
    /// Features: Drag handles, grouped sections, layer hierarchy, visibility toggles.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class ComponentTreePainter : BaseTreePainter
    {
        private const int GroupSpacing = 8;
        private const int LayerIndent = 12;

        private Font _regularFont;

        /// <summary>
        /// Component tree-specific node painting with Figma/VS Code Style.
        /// Features: Drag handles (3 horizontal lines on hover), left accent stripe (2px on selection), filled triangle toggles, layered square icons.
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for component tree clarity
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw component tree background
                if (isSelected || isHovered)
                {
                    var bgBrush = PaintersFactory.GetSolidBrush(isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor);
                    g.FillRectangle(bgBrush, nodeBounds);
                }

                // STEP 2: Left accent stripe (distinctive Figma/VS Code feature)
                if (isSelected)
                {
                    using (var accentPen = new Pen(_theme.AccentColor, 2f))
                    {
                        g.DrawLine(accentPen, nodeBounds.X, nodeBounds.Y, nodeBounds.X, nodeBounds.Bottom);
                    }
                }

                // STEP 3: Draw drag handles (3 horizontal lines) on hover
                if (isHovered)
                {
                    int handleX = nodeBounds.X + 4;
                    int handleY = nodeBounds.Y + (nodeBounds.Height - 12) / 2;
                    int handleWidth = 10;
                    int handleSpacing = 2;

                    Color handleColor = Color.FromArgb(120, _theme.TreeForeColor);
                    using (var handlePen = new Pen(handleColor, 1.5f))
                    {
                        handlePen.StartCap = LineCap.Round;
                        handlePen.EndCap = LineCap.Round;

                        // Three horizontal lines
                        g.DrawLine(handlePen, handleX, handleY, handleX + handleWidth, handleY);
                        g.DrawLine(handlePen, handleX, handleY + handleSpacing + 2, handleX + handleWidth, handleY + handleSpacing + 2);
                        g.DrawLine(handlePen, handleX, handleY + (handleSpacing + 2) * 2, handleX + handleWidth, handleY + (handleSpacing + 2) * 2);
                    }
                }

                // STEP 4: Draw Figma-Style filled triangle toggle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = node.ToggleRectContent;
                    Color toggleColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;

                    using (var brush = new SolidBrush(toggleColor))
                    {
                        int centerX = toggleRect.Left + toggleRect.Width / 2;
                        int centerY = toggleRect.Top + toggleRect.Height / 2;
                        int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

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

                        g.FillPolygon(brush, triangle);
                    }
                }

                // STEP 5: Draw component checkbox
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

                // STEP 6: Draw layered component icon
                if (node.IconRectContent != Rectangle.Empty && !string.IsNullOrEmpty(node.Item.ImagePath))
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    try { StyledImagePainter.Paint(g, iconRect, node.Item.ImagePath); } catch { }
                }

                // STEP 7: Draw text with component tree typography (monospace bold on selection)
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = node.TextRectContent;
                    var textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;
                    TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, _regularFont, textRect, textColor,
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
                // Selected: accent background
                Color selectedColor = _theme.TreeNodeSelectedBackColor;
                using (var path = CreateRoundedRectangle(nodeBounds, 3))
                {
                    using (var brush = new SolidBrush(selectedColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Left accent stripe
                    using (var pen = new Pen(_theme.AccentColor, 2))
                    {
                        g.DrawLine(pen, nodeBounds.X, nodeBounds.Y, nodeBounds.X, nodeBounds.Bottom);
                    }
                }
            }
            else if (isHovered)
            {
                Color hoverColor = _theme.TreeNodeHoverBackColor;
                using (var path = CreateRoundedRectangle(nodeBounds, 3))
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Figma-Style triangle toggle
            Color toggleColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;
            using (var brush = new SolidBrush(toggleColor))
            {
                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

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

            // Default component icon (layered squares)
            PaintDefaultIcon(g, iconRect);
        }

        private void PaintDefaultIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = Color.FromArgb(
                (_theme.AccentColor.R + _theme.TreeForeColor.R) / 2,
                (_theme.AccentColor.G + _theme.TreeForeColor.G) / 2,
                (_theme.AccentColor.B + _theme.TreeForeColor.B) / 2);

            // Back square
            Rectangle backRect = new Rectangle(
                iconRect.X + 3,
                iconRect.Y + 3,
                iconRect.Width - 3,
                iconRect.Height - 3);

            using (var path = CreateRoundedRectangle(backRect, 2))
            {
                using (var brush = new SolidBrush(Color.FromArgb(60, iconColor)))
                {
                    g.FillPath(brush, path);
                }

                using (var pen = new Pen(Color.FromArgb(100, iconColor), 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Front square
            Rectangle frontRect = new Rectangle(
                iconRect.X,
                iconRect.Y,
                iconRect.Width - 3,
                iconRect.Height - 3);

            using (var path = CreateRoundedRectangle(frontRect, 2))
            {
                using (var brush = new SolidBrush(Color.FromArgb(120, iconColor)))
                {
                    g.FillPath(brush, path);
                }

                using (var pen = new Pen(iconColor, 1.5f))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Use monospace-Style font for component names if selected
            Font renderFont = isSelected ? new Font("Consolas", font.Size, FontStyle.Bold) : font;

            TextRenderer.DrawText(g, text, renderFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            if (isSelected && renderFont != font)
            {
                renderFont.Dispose();
            }
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
        /// Paint group header separator.
        /// </summary>
        public void PaintGroupHeader(Graphics g, Rectangle headerRect, string groupName)
        {
            if (headerRect.Width <= 0 || headerRect.Height <= 0) return;

            // Background
            Color headerColor = Color.FromArgb(30, _theme.TreeForeColor);
            using (var brush = new SolidBrush(headerColor))
            {
                g.FillRectangle(brush, headerRect);
            }

            // Text
            if (!string.IsNullOrEmpty(groupName))
            {
                Color textColor = Color.FromArgb(180, _theme.TreeForeColor);
                using (var font = new Font("Segoe UI", 8f, FontStyle.Bold))
                {
                    TextRenderer.DrawText(g, groupName.ToUpper(), font, headerRect, textColor,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            }
        }

        /// <summary>
        /// Paint visibility toggle (eye icon).
        /// </summary>
        public void PaintVisibilityToggle(Graphics g, Rectangle toggleRect, bool isVisible, bool isHovered)
        {
            if (toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            Color eyeColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;
            if (!isVisible)
            {
                eyeColor = Color.FromArgb(100, eyeColor);
            }

            int centerX = toggleRect.Left + toggleRect.Width / 2;
            int centerY = toggleRect.Top + toggleRect.Height / 2;
            int width = toggleRect.Width / 2;
            int height = toggleRect.Height / 3;

            if (isVisible)
            {
                // Eye shape
                using (var pen = new Pen(eyeColor, 1.5f))
                {
                    // Upper curve
                    g.DrawArc(pen, centerX - width, centerY - height / 2, width * 2, height, 180, 180);
                    // Lower curve
                    g.DrawArc(pen, centerX - width, centerY - height / 2, width * 2, height, 0, 180);
                }

                // Pupil
                int pupilSize = 3;
                using (var brush = new SolidBrush(eyeColor))
                {
                    g.FillEllipse(brush, centerX - pupilSize / 2, centerY - pupilSize / 2, pupilSize, pupilSize);
                }
            }
            else
            {
                // Eye with slash (hidden)
                using (var pen = new Pen(eyeColor, 1.5f))
                {
                    // Simplified eye outline
                    g.DrawEllipse(pen, centerX - width / 2, centerY - height / 2, width, height);
                    // Diagonal slash
                    g.DrawLine(pen, centerX - width, centerY - height, centerX + width, centerY + height);
                }
            }
        }

        /// <summary>
        /// Paint layer depth indicator lines.
        /// </summary>
        public void PaintLayerLines(Graphics g, Rectangle lineRect, int depth)
        {
            if (lineRect.Width <= 0 || lineRect.Height <= 0 || depth <= 0) return;

            Color lineColor = Color.FromArgb(40, _theme.BorderColor);
            using (var pen = new Pen(lineColor, 1))
            {
                pen.DashStyle = DashStyle.Dot;

                for (int i = 0; i < depth; i++)
                {
                    int x = lineRect.X + (i * LayerIndent);
                    g.DrawLine(pen, x, lineRect.Y, x, lineRect.Bottom);
                }
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
            // Component trees need compact but readable spacing
            return Math.Max(26, base.GetPreferredRowHeight(item, font));
        }

        public override void Initialize(BeepTree owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            _regularFont = owner?.TextFont ?? SystemFonts.DefaultFont;
        }
    }
}
