using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Standard list box painter - default Windows-like Style with enhanced borders and backgrounds
    /// </summary>
    internal class StandardListBoxPainter : BaseListBoxPainter
    {
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            // Use precomputed rects for best consistency
            var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
            Rectangle checkRect = info?.CheckRect ?? Rectangle.Empty;
            Rectangle iconRect = info?.IconRect ?? Rectangle.Empty;
            Rectangle textRect = info?.TextRect ?? itemRect;

            // Checkbox
            if (_owner.ShowCheckBox && SupportsCheckboxes() && !checkRect.IsEmpty)
            {
                bool isChecked = _owner.SelectedItems?.Contains(item) == true;
                DrawCheckbox(g, checkRect, isChecked, isHovered);
            }

            // Icon
            if (_owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath) && !iconRect.IsEmpty)
            {
                DrawItemImage(g, iconRect, item.ImagePath);
            }

            // Text
            Color textColor = _owner.IsItemSelected(item) ? Color.White : (_helper.GetTextColor());
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);
        }
        
        // Enhanced with border style and gradient background
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;

            // Create rounded rectangle path for modern look
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(itemRect, 4))
            {
                // Draw background with gradient based on state
                if (isSelected)
                {
                    var selColor = _theme?.PrimaryColor ?? Color.LightBlue;
                    using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(itemRect, 
                        Color.FromArgb(100, selColor.R, selColor.G, selColor.B),
                        Color.FromArgb(60, selColor.R, selColor.G, selColor.B),
                        System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, path);
                    }

                    // Selection border
                    using (var pen = new System.Drawing.Pen(_theme?.PrimaryColor ?? Color.Blue, 2f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else if (isHovered)
                {
                    var hoverColor = _theme?.ListItemHoverBackColor ?? Color.FromArgb(240, 240, 240);
                    using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(itemRect,
                        Color.FromArgb(50, hoverColor.R, hoverColor.G, hoverColor.B),
                        Color.FromArgb(20, hoverColor.R, hoverColor.G, hoverColor.B),
                        System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, path);
                    }

                    // Hover border
                    using (var pen = new System.Drawing.Pen(_theme?.AccentColor ?? Color.LightGray, 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    // Default background
                    using (var brush = new System.Drawing.SolidBrush(_theme?.BackgroundColor ?? Color.White))
                    {
                        g.FillPath(brush, path);
                    }

                    // Default subtle border
                    using (var pen = new System.Drawing.Pen(Color.FromArgb(200, 200, 200), 0.5f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }
    }
}
