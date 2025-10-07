using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Selected items with full width colored backgrounds (from image 4)
    /// Gray/Green colored full-width selection backgrounds with descriptions
    /// </summary>
    internal class ColoredSelectionPainter : BaseListBoxPainter
    {
        public override bool SupportsCheckboxes() => true;
        
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            DrawItemBackground(g, itemRect, isHovered, isSelected);
            
            int currentX = itemRect.Left + 16;
            
            // Draw checkbox
            if (_owner.ShowCheckBox && SupportsCheckboxes())
            {
                Rectangle checkRect = new Rectangle(currentX, itemRect.Y + (itemRect.Height - 18) / 2, 18, 18);
                bool isChecked = _owner.SelectedItems?.Contains(item) == true;
                DrawColoredCheckbox(g, checkRect, isChecked, GetSelectionColor(item));
                currentX += 26;
            }
            
            // Draw main text
            Rectangle textRect = new Rectangle(currentX, itemRect.Y + 8, itemRect.Width - currentX - 16, itemRect.Height / 2);
            Color textColor = isSelected ? Color.FromArgb(40, 60, 40) : Color.FromArgb(40, 40, 40);
            Font boldFont = new Font(_owner.TextFont, FontStyle.Bold);
            DrawItemText(g, textRect, item.Text, textColor, boldFont);
            
            // Draw description
            if (!string.IsNullOrEmpty(item.Description))
            {
                Font smallFont = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size - 1);
                Rectangle descRect = new Rectangle(currentX, itemRect.Y + itemRect.Height / 2, itemRect.Width - currentX - 16, itemRect.Height / 2 - 8);
                Color descColor = isSelected ? Color.FromArgb(80, 100, 80) : Color.FromArgb(120, 120, 120);
                System.Windows.Forms.TextRenderer.DrawText(g, item.Description, smallFont, descRect, descColor,
                    System.Windows.Forms.TextFormatFlags.Left | System.Windows.Forms.TextFormatFlags.Top);
            }
        }
        
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            Color bgColor;
            if (isSelected)
            {
                // Use item-specific colors
                bgColor = GetSelectionBackgroundColor(isSelected);
            }
            else if (isHovered)
            {
                bgColor = Color.FromArgb(248, 248, 248);
            }
            else
            {
                bgColor = Color.White;
            }
            
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, itemRect);
            }
            
            // Draw bottom border
            using (var pen = new Pen(Color.FromArgb(235, 235, 235), 1f))
            {
                g.DrawLine(pen, itemRect.Left, itemRect.Bottom - 1, itemRect.Right, itemRect.Bottom - 1);
            }
        }
        
        private Color GetSelectionBackgroundColor(bool isSelected)
        {
            // Return different colors based on item state
            // Gray for default, green for custom choice
            return Color.FromArgb(220, 245, 230); // Light green
        }
        
        private Color GetSelectionColor(SimpleItem item)
        {
            // Determine checkbox color based on item
            if (item.Text?.ToLower().Contains("custom") == true)
                return Color.FromArgb(76, 175, 80); // Green
            else
                return Color.FromArgb(120, 120, 120); // Gray
        }
        
        private void DrawColoredCheckbox(Graphics g, Rectangle checkboxRect, bool isChecked, Color checkColor)
        {
            // Draw background
            Color bgColor = isChecked ? checkColor : Color.White;
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, checkboxRect);
            }
            
            // Draw border
            using (var pen = new Pen(checkColor, 1.5f))
            {
                g.DrawRectangle(pen, checkboxRect.X, checkboxRect.Y, checkboxRect.Width - 1, checkboxRect.Height - 1);
            }
            
            // Draw checkmark if checked
            if (isChecked)
            {
                using (var pen = new Pen(Color.White, 2f))
                {
                    Point[] checkPoints = new Point[]
                    {
                        new Point(checkboxRect.Left + 3, checkboxRect.Top + checkboxRect.Height / 2),
                        new Point(checkboxRect.Left + checkboxRect.Width / 2 - 1, checkboxRect.Bottom - 4),
                        new Point(checkboxRect.Right - 3, checkboxRect.Top + 3)
                    };
                    g.DrawLines(pen, checkPoints);
                }
            }
        }
        
        public override int GetPreferredItemHeight()
        {
            return 64; // Taller for two-line display
        }
    }
}
