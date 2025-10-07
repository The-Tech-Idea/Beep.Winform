using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

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
            
            int currentX = itemRect.Left + 8;
            
            // Draw checkbox if enabled
            if (_owner.ShowCheckBox && SupportsCheckboxes())
            {
                Rectangle checkRect = new Rectangle(currentX, itemRect.Y + (itemRect.Height - 16) / 2, 16, 16);
                bool isChecked = _owner.SelectedItems?.Contains(item) == true;
                DrawCheckbox(g, checkRect, isChecked, isHovered);
                currentX += 20;
            }
            
            // Draw image if enabled
            if (_owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
            {
                int imgSize = Math.Min(itemRect.Height - 8, 24);
                Rectangle imgRect = new Rectangle(currentX, itemRect.Y + (itemRect.Height - imgSize) / 2, imgSize, imgSize);
                DrawItemImage(g, imgRect, item.ImagePath);
                currentX += imgSize + 8;
            }
            
            // Draw text
            Rectangle textRect = new Rectangle(currentX, itemRect.Y, itemRect.Right - currentX - 8, itemRect.Height);
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
