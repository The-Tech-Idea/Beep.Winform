using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Filled Style list with colored backgrounds (from image 1 - bottom right)
    /// Blue filled background for selected items with white text
    /// </summary>
    internal class FilledStylePainter : BaseListBoxPainter
    {
        public override bool SupportsCheckboxes() => true;
        
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            // Apply slight deflation for spacing
            var rect = itemRect;
            rect.Inflate(-Scale(4), -Scale(2));
            
            DrawItemBackgroundEx(g, rect, item, isHovered, isSelected);
            
            int currentX = rect.Left + Scale(12);
            
            // Draw checkbox
            if (_owner.ShowCheckBox && SupportsCheckboxes())
            {
                Rectangle checkRect = new Rectangle(currentX, rect.Y + (rect.Height - Scale(16)) / 2, Scale(16), Scale(16));
                bool isChecked = _owner.SelectedItems?.Contains(item) == true;
                
                // White checkbox on blue background
                if (isSelected)
                {
                    DrawWhiteCheckbox(g, checkRect, isChecked);
                }
                else
                {
                    DrawCheckbox(g, checkRect, isChecked, isHovered);
                }
                currentX += Scale(24);
            }
            
            // Draw text
            int avatarSpace = !string.IsNullOrEmpty(item.ImagePath) ? Scale(40) : 0;
            Rectangle textRect = new Rectangle(currentX, rect.Y, rect.Width - currentX - avatarSpace - Scale(12), rect.Height);
            Color textColor = _owner.IsItemSelected(item) ? Color.White : _helper.GetTextColor();
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);
            
            // Draw avatar circle on right using StyledImagePainter
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int avatarSize = Scale(28);
                Rectangle avatarRect = new Rectangle(rect.Right - avatarSize - Scale(8), 
                    rect.Y + (rect.Height - avatarSize) / 2, avatarSize, avatarSize);
                
                float cx = avatarRect.X + avatarRect.Width / 2f;
                float cy = avatarRect.Y + avatarRect.Height / 2f;
                float radius = avatarRect.Width / 2f;
                StyledImagePainter.PaintInCircle(g, cx, cy, radius, item.ImagePath);
            }
        }
        
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for FilledStyle background, border, and shadow
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, false, Style);
                if (isHovered)
                {
                    using (var hoverBrush = new SolidBrush(Color.FromArgb(50, Color.Gray)))
                    {
                        g.FillPath(hoverBrush, path);
                    }
                }
            }
        }
        
        private void DrawWhiteCheckbox(Graphics g, Rectangle checkboxRect, bool isChecked)
        {
            // Draw white checkbox on blue background
            using (var brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, checkboxRect);
            }
            
            // Draw border
            using (var pen = new Pen(Color.White, 1.5f))
            {
                g.DrawRectangle(pen, checkboxRect.X, checkboxRect.Y, checkboxRect.Width - 1, checkboxRect.Height - 1);
            }
            
            // Draw checkmark if checked
            if (isChecked)
            {
                using (var pen = new Pen(Color.FromArgb(74, 144, 226), 2f))
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
            return Scale(48);
        }
    }
}
