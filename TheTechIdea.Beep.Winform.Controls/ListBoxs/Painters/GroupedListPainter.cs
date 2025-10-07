using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

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
                // Indent regular items
                var indentedRect = itemRect;
                indentedRect.X += 16;
                indentedRect.Width -= 16;
                base.DrawItem(g, indentedRect, item, isHovered, isSelected);
            }
        }
        
        private void DrawGroupHeader(Graphics g, Rectangle rect, SimpleItem item)
        {
            // Draw group header background
            using (var brush = new SolidBrush(Color.FromArgb(240, 240, 240)))
            {
                g.FillRectangle(brush, rect);
            }
            
            // Draw group header text
            Font headerFont = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size, FontStyle.Bold);
            Color headerColor = Color.FromArgb(100, 100, 100);
            
            Rectangle textRect = new Rectangle(rect.X + 12, rect.Y, rect.Width - 12, rect.Height);
            DrawItemText(g, textRect, item.Text, headerColor, headerFont);
            
            // Draw divider line
            using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1f))
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
