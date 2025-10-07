using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.Trees.Painters
{
    /// <summary>
    /// Modern file manager tree painter (Google Drive, OneDrive style).
    /// Clean minimal design with folder icons, nested structure, and smooth animations.
    /// Features: Rounded selection, subtle shadows, colorful folder icons, breadcrumb-style hierarchy.
    /// </summary>
    public class FileManagerTreePainter : BaseTreePainter
    {
        private const int RoundRadius = 6;
        private const int IconPadding = 4;

        public override void PaintNodeBackground(Graphics g, Rectangle nodeBounds, bool isHovered, bool isSelected)
        {
            if (nodeBounds.Width <= 0 || nodeBounds.Height <= 0) return;

            if (isSelected)
            {
                // Selected: use tree-specific selected background
                Color selectedColor = _theme.TreeNodeSelectedBackColor;
                using (var path = CreateRoundedRectangle(nodeBounds, RoundRadius))
                {
                    using (var brush = new SolidBrush(selectedColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Subtle border using accent color
                    using (var pen = new Pen(_theme.AccentColor, 1))
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
                    using (var brush = new SolidBrush(hoverColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
        }

        public override void PaintToggle(Graphics g, Rectangle toggleRect, bool isExpanded, bool hasChildren, bool isHovered)
        {
            if (!hasChildren || toggleRect.Width <= 0 || toggleRect.Height <= 0) return;

            // Modern minimal chevron using tree-specific colors
            Color chevronColor = isHovered ? _theme.AccentColor : _theme.TreeForeColor;
            using (var pen = new Pen(chevronColor, 1.5f))
            {
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

            // Use slightly bold font for selected items
            Font renderFont = isSelected ? new Font(font, FontStyle.Bold) : font;

            TextRenderer.DrawText(g, text, renderFont, textRect, textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);

            if (isSelected && renderFont != font)
            {
                renderFont.Dispose();
            }
        }

        public override void Paint(Graphics g, BeepTree owner, Rectangle bounds)
        {
            if (g == null || owner == null || bounds.Width <= 0 || bounds.Height <= 0) return;

            // Background using tree-specific theme color
            using (var brush = new SolidBrush(_theme.TreeBackColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Enable high-quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            base.Paint(g, owner, bounds);
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            // Shrink slightly for padding
            rect = new Rectangle(rect.X + 2, rect.Y + 1, rect.Width - 4, rect.Height - 2);

            if (rect.Width <= 0 || rect.Height <= 0) return path;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        public override int GetPreferredRowHeight(SimpleItem item, Font font)
        {
            // File manager style needs comfortable spacing
            return Math.Max(28, base.GetPreferredRowHeight(item, font));
        }
    }
}
