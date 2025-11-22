using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Rounded list box painter - rounded item corners with distinct styling
    /// </summary>
    internal class RoundedListBoxPainter : BaseListBoxPainter
    {
        private const int ItemRadius = 8;
        
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            // Deflate slightly for spacing
            var rect = itemRect;
            rect.Inflate(-4, -2);

            DrawItemBackgroundEx(g, rect, item, isHovered, isSelected);

            // Use layout rects for content
            var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
            var checkRect = info?.CheckRect ?? Rectangle.Empty;
            var iconRect = info?.IconRect ?? Rectangle.Empty;
            var textRect = info?.TextRect ?? rect;

            if (_owner.ShowCheckBox && SupportsCheckboxes() && !checkRect.IsEmpty)
            {
                bool isChecked = _owner.SelectedItems?.Contains(item) == true;
                DrawCheckbox(g, checkRect, isChecked, isHovered);
            }

            if (_owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath) && !iconRect.IsEmpty)
            {
                DrawItemImage(g, iconRect, item.ImagePath);
            }

            Color textColor = isSelected ? Color.White : (_helper.GetTextColor());
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);
        }
        
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;

            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(itemRect, ItemRadius))
            {
                if (isSelected)
                {
                    var selColor = _theme?.PrimaryColor ?? Color.LightBlue;

                    // Shadow for selected state
                    var shadowRect = itemRect;
                    shadowRect.Offset(0, 2);
                    using (var shadowBrush = new LinearGradientBrush(shadowRect,
                        Color.FromArgb(50, Color.Black),
                        Color.Transparent,
                        90f))
                    {
                        g.FillRectangle(shadowBrush, shadowRect);
                    }

                    // Filled background
                    using (var brush = new LinearGradientBrush(itemRect,
                        Color.FromArgb(120, selColor.R, selColor.G, selColor.B),
                        Color.FromArgb(80, selColor.R, selColor.G, selColor.B),
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, path);
                    }

                    // Border
                    using (var pen = new Pen(selColor, 2.5f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else if (isHovered)
                {
                    // Hover shadow
                    using (var shadowBrush = new LinearGradientBrush(itemRect,
                        Color.FromArgb(25, Color.Black),
                        Color.Transparent,
                        90f))
                    {
                        g.FillRectangle(shadowBrush, itemRect);
                    }

                    // Hover background
                    using (var brush = new SolidBrush(Color.FromArgb(235, 235, 235)))
                    {
                        g.FillPath(brush, path);
                    }

                    // Hover border
                    using (var pen = new Pen(_theme?.AccentColor ?? Color.Gray, 1.5f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    // Normal rounded style
                    using (var brush = new SolidBrush(Color.White))
                    {
                        g.FillPath(brush, path);
                    }

                    // Subtle border
                    using (var pen = new Pen(Color.FromArgb(210, 210, 210), 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }
        
        public override int GetPreferredItemHeight()
        {
            return 36;
        }
    }
}
