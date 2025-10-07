using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;


namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Ant Design tree painter.
    /// Features: Clean lines, checkbox support, folder icons, proper spacing.
    /// Uses theme colors for consistent appearance across light/dark themes.
    /// </summary>
    public class AntDesignTreePainter : BaseTreePainter
    {
        private const int ItemPadding = 4;

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            if (isSelected)
            {
                // Selected: subtle background
                using (var brush = new SolidBrush(_theme.TreeNodeSelectedBackColor))
                {
                    g.FillRectangle(brush, nodeBounds);
                }
            }
            else if (isHovered)
            {
                // Hover: very subtle
                using (var hoverBrush = new SolidBrush(_theme.TreeNodeHoverBackColor))
                {
                    g.FillRectangle(hoverBrush, nodeBounds);
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Ant Design uses caret icon
            Color caretColor = _theme.TreeForeColor;

            using (var pen = new Pen(caretColor, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                int centerX = toggleRect.Left + toggleRect.Width / 2;
                int centerY = toggleRect.Top + toggleRect.Height / 2;
                int size = Math.Min(toggleRect.Width, toggleRect.Height) / 3;

                if (isExpanded)
                {
                    // Caret down
                    g.DrawLine(pen, centerX - size, centerY - size / 2, centerX, centerY + size / 2);
                    g.DrawLine(pen, centerX, centerY + size / 2, centerX + size, centerY - size / 2);
                }
                else
                {
                    // Caret right
                    g.DrawLine(pen, centerX - size / 2, centerY - size, centerX + size / 2, centerY);
                    g.DrawLine(pen, centerX + size / 2, centerY, centerX - size / 2, centerY + size);
                }
            }
        }

        public override void PaintCheckbox(Graphics g, Rectangle checkboxRect, bool isChecked, bool isHovered)
        {
            if (checkboxRect.Width <= 0 || checkboxRect.Height <= 0) return;

            // Ant Design checkbox style
            Color borderColor = isHovered ? _theme.AccentColor : _theme.BorderColor;
            Color fillColor = isChecked ? _theme.AccentColor : _theme.TreeBackColor;

            // Border
            using (var pen = new Pen(borderColor, 1f))
            {
                Rectangle checkRect = new Rectangle(
                    checkboxRect.X + 2,
                    checkboxRect.Y + 2,
                    checkboxRect.Width - 4,
                    checkboxRect.Height - 4);

                g.DrawRectangle(pen, checkRect);
            }

            // Fill if checked
            if (isChecked)
            {
                using (var brush = new SolidBrush(fillColor))
                {
                    Rectangle fillRect = new Rectangle(
                        checkboxRect.X + 3,
                        checkboxRect.Y + 3,
                        checkboxRect.Width - 5,
                        checkboxRect.Height - 5);

                    g.FillRectangle(brush, fillRect);
                }

                // Checkmark
                using (var pen = new Pen(Color.White, 2f))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;

                    int centerX = checkboxRect.Left + checkboxRect.Width / 2;
                    int centerY = checkboxRect.Top + checkboxRect.Height / 2;

                    // Checkmark
                    g.DrawLine(pen, centerX - 4, centerY, centerX - 1, centerY + 3);
                    g.DrawLine(pen, centerX - 1, centerY + 3, centerX + 4, centerY - 3);
                }
            }
        }

        public override void PaintIcon(Graphics g, Rectangle iconRect, string imagePath)
        {
            if (iconRect.Width <= 0 || iconRect.Height <= 0) return;

            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    Styling.ImagePainters.StyledImagePainter.Paint(g, iconRect, imagePath, Common.BeepControlStyle.AntDesign);
                    return;
                }
                catch { }
            }

            // Default: Ant Design folder icon
            PaintDefaultAntIcon(g, iconRect);
        }

        private void PaintDefaultAntIcon(Graphics g, Rectangle iconRect)
        {
            Color iconColor = _theme.AccentColor;

            // Ant Design folder style
            int padding = iconRect.Width / 5;
            Rectangle innerRect = new Rectangle(
                iconRect.X + padding,
                iconRect.Y + padding,
                iconRect.Width - padding * 2,
                iconRect.Height - padding * 2);

            using (var path = new GraphicsPath())
            {
                // Folder shape
                int tabWidth = innerRect.Width / 3;
                int tabHeight = innerRect.Height / 4;

                path.AddLine(innerRect.Left, innerRect.Top + tabHeight, innerRect.Left + tabWidth, innerRect.Top + tabHeight);
                path.AddLine(innerRect.Left + tabWidth, innerRect.Top + tabHeight, innerRect.Left + tabWidth + 2, innerRect.Top);
                path.AddLine(innerRect.Left + tabWidth + 2, innerRect.Top, innerRect.Right, innerRect.Top);
                path.AddLine(innerRect.Right, innerRect.Top, innerRect.Right, innerRect.Bottom);
                path.AddLine(innerRect.Right, innerRect.Bottom, innerRect.Left, innerRect.Bottom);
                path.CloseFigure();

                using (var brush = new SolidBrush(Color.FromArgb(100, iconColor)))
                {
                    g.FillPath(brush, path);
                }

                using (var pen = new Pen(iconColor, 1f))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        public override void PaintText(Graphics g, Rectangle textRect, string text, Font font, bool isSelected, bool isHovered)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            Color textColor = isSelected ? _theme.TreeNodeSelectedForeColor : _theme.TreeForeColor;

            // Ant Design typography (Chinese Simplified font stack, Segoe UI fallback)
            Font renderFont = new Font("Segoe UI", font.Size, FontStyle.Regular);

            TextRenderer.DrawText(g, text, renderFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            renderFont.Dispose();
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            // Clean background
            using (var brush = new SolidBrush(_theme.TreeBackColor))
            {
                g.FillRectangle(brush, bounds);
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            base.Paint(g, owner, bounds);
        }

        public override int GetPreferredRowHeight(SimpleItem item, Font font)
        {
            // Ant Design comfortable spacing
            return Math.Max(28, base.GetPreferredRowHeight(item, font));
        }
    }
}
