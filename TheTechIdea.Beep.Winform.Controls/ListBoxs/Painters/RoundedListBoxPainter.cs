using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Rounded list box painter - rounded item corners
    /// </summary>
    internal class RoundedListBoxPainter : BaseListBoxPainter
    {
        private const int ItemRadius = 8;
        
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            // Deflate slightly for spacing
            var rect = itemRect;
            rect.Inflate(-4, -2);

            DrawItemBackground(g, rect, isHovered, isSelected);

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
            Color bgColor;
            if (isSelected)
                bgColor = _helper.GetSelectedBackColor();
            else if (isHovered)
                bgColor = _helper.GetHoverBackColor();
            else
                return; // No background for non-selected, non-hovered
            
            using (var brush = new SolidBrush(bgColor))
            using (var path = GetRoundedRectPath(itemRect, ItemRadius))
            {
                g.FillPath(brush, path);
            }
        }
        
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter - 1, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter - 1, rect.Bottom - diameter - 1, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter - 1, diameter, diameter, 90, 90);
            path.CloseFigure();
            
            return path;
        }
        
        public override int GetPreferredItemHeight()
        {
            return 36;
        }
    }
}
