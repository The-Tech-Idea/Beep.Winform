using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Filter status list with colored backgrounds and counts (from image 2 - top left)
    /// Yellow/Red/Blue colored item backgrounds with count badges
    /// </summary>
    internal class FilterStatusPainter : BaseListBoxPainter
    {
        public override bool SupportsCheckboxes() => true;
        
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            // Apply slight deflation for spacing
            var rect = itemRect;
            rect.Inflate(-4, -2);
            
            DrawItemBackground(g, rect, isHovered, isSelected);
            
            int currentX = rect.Left + 12;
            
            // Draw checkbox with custom coloring
            if (_owner.ShowCheckBox && SupportsCheckboxes())
            {
                Rectangle checkRect = new Rectangle(currentX, rect.Y + (rect.Height - 20) / 2, 20, 20);
                bool isChecked = _owner.SelectedItems?.Contains(item) == true;
                DrawColoredCheckbox(g, checkRect, isChecked, GetItemStateColor(item, isSelected));
                currentX += 28;
            }
            
            // Draw text
            Rectangle textRect = new Rectangle(currentX, rect.Y, rect.Width - currentX - 40, rect.Height);
            Color textColor = GetItemTextColor(item, isSelected);
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);
            
            // Draw count badge on right
            if (item.Tag != null && int.TryParse(item.Tag.ToString(), out int count))
            {
                DrawCountBadge(g, rect, count, GetItemStateColor(item, isSelected));
            }
        }
        
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            Color bgColor = GetItemBackgroundColor(isSelected, isHovered);
            
            using (var brush = new SolidBrush(bgColor))
            using (var path = GetRoundedRectPath(itemRect, 6))
            {
                g.FillPath(brush, path);
            }
        }
        
        private Color GetItemBackgroundColor(bool isSelected, bool isHovered)
        {
            if (isSelected)
            {
                // Determine color based on item state (would need item reference)
                // For now, return primary colors
                return Color.FromArgb(255, 237, 213); // Yellow/Gold for payment alert
            }
            else if (isHovered)
            {
                return Color.FromArgb(245, 245, 245);
            }
            return Color.White;
        }
        
        private Color GetItemStateColor(SimpleItem item, bool isSelected)
        {
            // Determine color based on item content or state
            if (item.Text?.ToLower().Contains("error") == true || item.Text?.ToLower().Contains("delivery") == true)
                return Color.FromArgb(220, 53, 69); // Red
            else if (item.Text?.ToLower().Contains("payment") == true || item.Text?.ToLower().Contains("alert") == true)
                return Color.FromArgb(255, 193, 7); // Amber/Gold
            else
                return Color.FromArgb(108, 117, 125); // Gray
        }
        
        private Color GetItemTextColor(SimpleItem item, bool isSelected)
        {
            if (item.Text?.ToLower().Contains("error") == true || item.Text?.ToLower().Contains("delivery") == true)
                return Color.FromArgb(120, 30, 40); // Dark red
            else if (item.Text?.ToLower().Contains("payment") == true)
                return Color.FromArgb(140, 100, 30); // Dark amber
            else
                return Color.FromArgb(60, 60, 60); // Dark gray
        }
        
        private void DrawColoredCheckbox(Graphics g, Rectangle checkboxRect, bool isChecked, Color stateColor)
        {
            // Draw checkbox background
            Color bgColor = isChecked ? stateColor : Color.White;
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, checkboxRect);
            }
            
            // Draw checkbox border
            using (var pen = new Pen(stateColor, 1.5f))
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
                        new Point(checkboxRect.Left + 4, checkboxRect.Top + checkboxRect.Height / 2),
                        new Point(checkboxRect.Left + checkboxRect.Width / 2, checkboxRect.Bottom - 5),
                        new Point(checkboxRect.Right - 4, checkboxRect.Top + 4)
                    };
                    g.DrawLines(pen, checkPoints);
                }
            }
        }
        
        private void DrawCountBadge(Graphics g, Rectangle itemRect, int count, Color badgeColor)
        {
            string countText = count.ToString();
            var textSize = System.Windows.Forms.TextRenderer.MeasureText(countText, _owner.TextFont);
            int badgeWidth = Math.Max(textSize.Width + 8, 24);
            int badgeHeight = 20;
            
            Rectangle badgeRect = new Rectangle(
                itemRect.Right - badgeWidth - 8,
                itemRect.Y + (itemRect.Height - badgeHeight) / 2,
                badgeWidth,
                badgeHeight);
            
            // Draw badge background
            Color bgColor = Color.FromArgb(30, badgeColor);
            using (var brush = new SolidBrush(bgColor))
            using (var path = GetRoundedRectPath(badgeRect, badgeHeight / 2))
            {
                g.FillPath(brush, path);
            }
            
            // Draw count text
            System.Windows.Forms.TextRenderer.DrawText(g, countText, _owner.TextFont, badgeRect, badgeColor,
                System.Windows.Forms.TextFormatFlags.HorizontalCenter | System.Windows.Forms.TextFormatFlags.VerticalCenter);
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
            return 44;
        }
    }
}
