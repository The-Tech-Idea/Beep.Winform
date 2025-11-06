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
    /// Jira/Atlassian-Style portfolio tree painter for project management.
    /// Features: Progress bars, effort indicators, theme grouping, epic/story hierarchy.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class PortfolioTreePainter : BaseTreePainter
    {
        private const int ProgressBarHeight = 4;
        private const int ProgressBarWidth = 60;
        private const int BadgeSize = 18;

        private Font _regularFont;
        private Font _badgeFont;

        /// <summary>
        /// Jira/Atlassian-Style portfolio tree painting for project management.
        /// Features: Left accent bar (3px on selection), progress indicators, effort badges, epic/story icons, rounded backgrounds (4px).
        /// </summary>
        public override void PaintNode(Graphics g, NodeInfo node, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (g == null || node.Item == null) return;

            // Enable high-quality rendering for Jira/Atlassian appearance
            var oldSmoothing = g.SmoothingMode;
            var oldTextRendering = g.TextRenderingHint;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            try
            {
                // STEP 1: Draw Jira rounded background
                if (isSelected || isHovered)
                {
                    var bgBrush = PaintersFactory.GetSolidBrush(isSelected ? _theme.TreeNodeSelectedBackColor : _theme.TreeNodeHoverBackColor);
                    using (var bgPath = CreateRoundedRectangle(nodeBounds, 4))
                    {
                        g.FillPath(bgBrush, bgPath);
                    }
                }

                // STEP 2: Draw left accent bar (Jira distinctive feature)
                if (isSelected)
                {
                    var accentPen = PaintersFactory.GetPen(_theme.AccentColor, 3f);
                    g.DrawLine(accentPen, nodeBounds.X + 1, nodeBounds.Y + 2, nodeBounds.X + 1, nodeBounds.Bottom - 2);
                }

                // STEP 3: Draw Atlassian arrow toggle
                bool hasChildren = node.Item.Children != null && node.Item.Children.Count > 0;
                if (hasChildren && node.ToggleRectContent != Rectangle.Empty)
                {
                    var toggleRect = _owner.LayoutHelper.TransformToViewport(node.ToggleRectContent);
                    var triBrush = PaintersFactory.GetSolidBrush(isHovered ? _theme.AccentColor : _theme.TreeForeColor);
                    int centerX = toggleRect.Left + toggleRect.Width / 2;
                    int centerY = toggleRect.Top + toggleRect.Height / 2;
                    int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;
                    Point[] triangle = node.Item.IsExpanded ? new Point[] { new Point(centerX - size, centerY - size / 2), new Point(centerX + size, centerY - size / 2), new Point(centerX, centerY + size / 2) } : new Point[] { new Point(centerX - size / 2, centerY - size), new Point(centerX + size / 2, centerY), new Point(centerX - size / 2, centerY + size) };
                    g.FillPolygon(triBrush, triangle);
                }

                // STEP 4: Draw checkbox (rounded Jira Style)
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
                        var points = new Point[] { new Point(checkRect.X + checkRect.Width / 4, checkRect.Y + checkRect.Height / 2), new Point(checkRect.X + checkRect.Width / 2 - 1, checkRect.Y + checkRect.Height * 3 / 4), new Point(checkRect.X + checkRect.Width * 3 / 4, checkRect.Y + checkRect.Height / 4) };
                        g.DrawLines(checkPen, points);
                    }
                }

                // STEP 5: Draw epic/story icon (Jira Style)
                if (node.IconRectContent != Rectangle.Empty)
                {
                    var iconRect = _owner.LayoutHelper.TransformToViewport(node.IconRectContent);
                    if (!string.IsNullOrEmpty(node.Item.ImagePath))
                        try
                        {
                            StyledImagePainter.Paint(g, iconRect, node.Item.ImagePath, _owner?.ControlStyle ?? Common.BeepControlStyle.Minimal);
                        }
                        catch
                        {
                            PaintDefaultIcon(g, iconRect);
                        }
                    else
                    {
                        var iconPath = CreateRoundedRectangle(iconRect, 3);
                        var fillBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(100, (_theme.AccentColor.R + _theme.TreeForeColor.R) / 2, (_theme.AccentColor.G + _theme.TreeForeColor.G) / 2, (_theme.AccentColor.B + _theme.TreeForeColor.B) / 2));
                        g.FillPath(fillBrush, iconPath);
                        var pen = PaintersFactory.GetPen((_theme.AccentColor.R + _theme.TreeForeColor.R) / 2 == 0 ? _theme.AccentColor : _theme.AccentColor, 1.5f);
                        g.DrawPath(pen, iconPath);
                    }
                }

                // STEP 6: Draw text
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var textRect = _owner.LayoutHelper.TransformToViewport(node.TextRectContent);
                    var textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;
                    TextRenderer.DrawText(g, node.Item.Text ?? string.Empty, _regularFont, textRect, textColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }

                // STEP 7: Draw progress bar (Jira task completion indicator)
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var progressRect = new Rectangle(node.TextRectContent.Right + 8, node.TextRectContent.Y + node.TextRectContent.Height / 2 - ProgressBarHeight / 2, ProgressBarWidth, ProgressBarHeight);
                    if (progressRect.Right < nodeBounds.Right - 30)
                    {
                        int progress = (node.Item.Text?.Length ?? 0) % 101;

                        var trackBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(50, _theme.TreeForeColor));
                        g.FillPath(trackBrush, CreateRoundedRectangle(progressRect, 2));

                        if (progress > 0)
                        {
                            int fillWidth = (int)(progressRect.Width * (progress / 100f));
                            var fillRect = new Rectangle(progressRect.X, progressRect.Y, fillWidth, progressRect.Height);
                            var fillBrush = PaintersFactory.GetSolidBrush(progress >= 100 ? Color.FromArgb(76, 175, 80) : _theme.AccentColor);
                            g.FillPath(fillBrush, CreateRoundedRectangle(fillRect, 2));
                        }
                    }
                }

                // STEP 8: Draw effort/story points badge (Jira feature)
                if (node.TextRectContent != Rectangle.Empty)
                {
                    var badgeRect = new Rectangle(nodeBounds.Right - BadgeSize - 6, nodeBounds.Y + (nodeBounds.Height - BadgeSize) / 2, BadgeSize, BadgeSize);
                    if (badgeRect.X > node.TextRectContent.Right + ProgressBarWidth + 16)
                    {
                        string points = ((node.Item.Text?.Length ?? 0) % 10).ToString();
                        var badgeBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(150, _theme.AccentColor));
                        g.FillEllipse(badgeBrush, badgeRect);
                        var pen = PaintersFactory.GetPen(_theme.AccentColor, 1f);
                        g.DrawEllipse(pen, badgeRect);
                        TextRenderer.DrawText(g, points, _badgeFont, badgeRect, _theme.TreeNodeSelectedForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                    }
                }
            }
            finally
            {
                g.SmoothingMode = oldSmoothing;
                g.TextRenderingHint = oldTextRendering;
            }
        }

        private void PaintDefaultIcon(Graphics g, Rectangle iconRect)
        {
            var brush = PaintersFactory.GetSolidBrush(Color.FromArgb(100, (_theme.AccentColor.R + _theme.TreeForeColor.R) / 2, (_theme.AccentColor.G + _theme.TreeForeColor.G) / 2, (_theme.AccentColor.B + _theme.TreeForeColor.B) / 2));
            g.FillPath(brush, CreateRoundedRectangle(iconRect, 3));
            var pen = PaintersFactory.GetPen(_theme.AccentColor, 1.5f);
            g.DrawPath(pen, CreateRoundedRectangle(iconRect, 3));
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            if (rect.Width < diameter || rect.Height < diameter || rect.Width <= 0 || rect.Height <= 0)
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
                // Selected: subtle rounded rectangle
                Color selectedColor = _theme.TreeNodeSelectedBackColor;
                using (var path = CreateRoundedRectangle(nodeBounds, 4))
                {
                    using (var brush = new SolidBrush(selectedColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Left accent bar
                    using (var pen = new Pen(_theme.AccentColor, 3))
                    {
                        g.DrawLine(pen, nodeBounds.X + 1, nodeBounds.Y + 2, nodeBounds.X + 1, nodeBounds.Bottom - 2);
                    }
                }
            }
            else if (isHovered)
            {
                Color hoverColor = _theme.TreeNodeHoverBackColor;
                using (var path = CreateRoundedRectangle(nodeBounds, 4))
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Atlassian-Style arrow toggle
            Color arrowColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;
            using (var brush = new SolidBrush(arrowColor))
            {
                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                Point[] triangle;
                if (isExpanded)
                {
                    // Down arrow
                    triangle = new Point[]
                    {
                        new Point(centerX - size, centerY - size / 2),
                        new Point(centerX + size, centerY - size / 2),
                        new Point(centerX, centerY + size / 2)
                    };
                }
                else
                {
                    // Right arrow
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
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, _owner?.ControlStyle ?? Common.BeepControlStyle.Minimal);
                    return;
                }
                catch { }
            }

            // Default epic/story icon
            PaintDefaultIcon(g, iconRect);
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            TextRenderer.DrawText(g, text, font, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
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
        /// Paint progress bar for task completion (0-100).
        /// </summary>
        public void PaintProgressBar(Graphics g, Rectangle barRect, int progress)
        {
            if (barRect.Width <= 0 || barRect.Height <= 0) return;

            progress = Math.Max(0, Math.Min(100, progress));

            // Background track
            Color trackColor = Color.FromArgb(50, _theme.TreeForeColor);
            using (var brush = new SolidBrush(trackColor))
            using (var path = CreateRoundedRectangle(barRect, 2))
            {
                g.FillPath(brush, path);
            }

            // Progress fill
            if (progress > 0)
            {
                int fillWidth = (int)(barRect.Width * (progress / 100f));
                Rectangle fillRect = new Rectangle(barRect.X, barRect.Y, fillWidth, barRect.Height);

                Color fillColor = progress >= 100 ? Color.FromArgb(76, 175, 80) : _theme.AccentColor;
                using (var brush = new SolidBrush(fillColor))
                using (var path = CreateRoundedRectangle(fillRect, 2))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        /// <summary>
        /// Paint effort/story points badge.
        /// </summary>
        public void PaintEffortBadge(Graphics g, Rectangle badgeRect, string points)
        {
            if (badgeRect.Width <= 0 || badgeRect.Height <= 0) return;

            // Circular badge
            Color badgeColor = Color.FromArgb(150, _theme.AccentColor);
            using (var brush = new SolidBrush(badgeColor))
            {
                g.FillEllipse(brush, badgeRect);
            }

            // Border
            using (var pen = new Pen(_theme.AccentColor, 1))
            {
                g.DrawEllipse(pen, badgeRect);
            }

            // Text
            if (!string.IsNullOrEmpty(points))
            {
                using (var font = new Font("Segoe UI", 7f, FontStyle.Bold))
                {
                    TextRenderer.DrawText(g, points, font, badgeRect, _theme.TreeNodeSelectedForeColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            }
        }

        public override int GetPreferredRowHeight(SimpleItem item, Font font)
        {
            // Portfolio trees need space for progress bars and badges
            return Math.Max(30, base.GetPreferredRowHeight(item, font));
        }

        public override void Initialize(BeepTree owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            _regularFont = owner?.TextFont ?? SystemFonts.DefaultFont;
            _badgeFont = new Font(_regularFont.FontFamily, 7f, FontStyle.Bold);
        }
    }
}
