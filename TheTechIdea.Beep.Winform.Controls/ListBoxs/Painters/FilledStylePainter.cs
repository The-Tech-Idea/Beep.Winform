using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

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
            rect.Inflate(-4, -2);
            
            DrawItemBackground(g, rect, isHovered, isSelected);
            
            int currentX = rect.Left + 12;
            
            // Draw checkbox
            if (_owner.ShowCheckBox && SupportsCheckboxes())
            {
                Rectangle checkRect = new Rectangle(currentX, rect.Y + (rect.Height - 16) / 2, 16, 16);
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
                currentX += 24;
            }
            
            // Draw text
            int avatarSpace = !string.IsNullOrEmpty(item.ImagePath) ? 40 : 0;
            Rectangle textRect = new Rectangle(currentX, rect.Y, rect.Width - currentX - avatarSpace - 12, rect.Height);
            Color textColor = isSelected ? Color.White : _helper.GetTextColor();
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);
            
            // Draw avatar circle on right
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int avatarSize = 28;
                Rectangle avatarRect = new Rectangle(rect.Right - avatarSize - 8, 
                    rect.Y + (rect.Height - avatarSize) / 2, avatarSize, avatarSize);
                
                using (var path = new GraphicsPath())
                {
                    path.AddEllipse(avatarRect);
                    g.SetClip(path);
                    DrawItemImage(g, avatarRect, item.ImagePath);
                    g.ResetClip();
                }
            }
        }
        
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for FilledStyle background, border, and shadow
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, isSelected, Style);
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
            return 48;
        }
    }
}
