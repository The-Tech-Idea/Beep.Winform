using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Grouped list with category headers and indented items
    /// </summary>
    internal class GroupedListPainter : OutlinedListBoxPainter
    {
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            // Check if this is a group header (item with children acts as group header)
            if (item.Children != null && item.Children.Count > 0)
            {
                DrawGroupHeader(g, itemRect, item);
            }
            else
            {
                // Draw regular items using layout-driven rects, but indent text by 16px
                var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
                Rectangle checkRect = info?.CheckRect ?? Rectangle.Empty;
                Rectangle iconRect = info?.IconRect ?? Rectangle.Empty;
                Rectangle textRect = info?.TextRect ?? itemRect;

                // Background
                DrawItemBackground(g, itemRect, isHovered, isSelected);

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

                // Text (indented)
                var indentedText = new Rectangle(textRect.X + 16, textRect.Y, Math.Max(0, textRect.Width - 16), textRect.Height);
                Color textColor = isSelected ? Color.White : (_helper.GetTextColor());
                DrawItemText(g, indentedText, item.Text, textColor, _owner.TextFont);
            }
        }
        
        private void DrawGroupHeader(Graphics g, Rectangle rect, SimpleItem item)
        {
            // Draw group header background using a subtle themed color
            Color headerBg = Color.FromArgb(12, (_theme?.PrimaryTextColor ?? Color.Black));
            using (var brush = new SolidBrush(headerBg))
            {
                g.FillRectangle(brush, rect);
            }
            
            // Draw group header text
            using (Font headerFont = new Font(_owner.TextFont, FontStyle.Bold))
            {
                Color headerColor = _theme?.SecondaryTextColor ?? _helper.GetTextColor();
                Rectangle textRect = new Rectangle(rect.X + 12, rect.Y, rect.Width - 12, rect.Height);
                DrawItemText(g, textRect, item.Text, headerColor, headerFont);
            }
            
            // Draw divider line
            using (var pen = new Pen(_theme?.BorderColor ?? Color.FromArgb(220, 220, 220), 1f))
            {
                g.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
            }
        }
        
        public override int GetPreferredItemHeight()
        {
            return 32;
        }
    }
}
