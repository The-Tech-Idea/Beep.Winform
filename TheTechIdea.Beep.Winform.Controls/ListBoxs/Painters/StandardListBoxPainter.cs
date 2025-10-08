using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Standard list box painter - default Windows-like style
    /// </summary>
    internal class StandardListBoxPainter : BaseListBoxPainter
    {
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            DrawItemBackground(g, itemRect, isHovered, isSelected);

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
                bgColor = _helper.GetBackgroundColor();
            
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, itemRect);
            }
        }
    }
}
