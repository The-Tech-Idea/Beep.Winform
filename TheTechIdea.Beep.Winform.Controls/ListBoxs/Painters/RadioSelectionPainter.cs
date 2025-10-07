using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Single choice radio-style list (from image 5 - Tickets Setup)
    /// Radio buttons on right, one item has colored background for selection
    /// </summary>
    internal class RadioSelectionPainter : BaseListBoxPainter
    {
        public override bool SupportsCheckboxes() => false; // Uses radio buttons instead
        
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            var rect = itemRect;
            rect.Inflate(-6, -3);
            
            DrawItemBackground(g, rect, isHovered, isSelected);
            
            int currentX = rect.Left + 16;
            
            // Draw main text
            Rectangle textRect = new Rectangle(currentX, rect.Y + 10, rect.Width - currentX - 50, rect.Height / 2);
            Color textColor = isSelected ? Color.White : Color.FromArgb(40, 40, 40);
            Font mainFont = isSelected ? new Font(_owner.TextFont, FontStyle.Bold) : _owner.TextFont;
            DrawItemText(g, textRect, item.Text, textColor, mainFont);
            
            // Draw description if available
            if (!string.IsNullOrEmpty(item.Description))
            {
                Font smallFont = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size - 1);
                Rectangle descRect = new Rectangle(currentX, rect.Y + rect.Height / 2 + 2, rect.Width - currentX - 50, rect.Height / 2 - 10);
                Color descColor = isSelected ? Color.FromArgb(220, 220, 255) : Color.FromArgb(120, 120, 120);
                System.Windows.Forms.TextRenderer.DrawText(g, item.Description, smallFont, descRect, descColor,
                    System.Windows.Forms.TextFormatFlags.Left | System.Windows.Forms.TextFormatFlags.Top);
            }
            
            // Draw radio button on right
            int radioSize = 20;
            Rectangle radioRect = new Rectangle(rect.Right - radioSize - 16, rect.Y + (rect.Height - radioSize) / 2, radioSize, radioSize);
            DrawRadioButton(g, radioRect, isSelected, isHovered, item);
        }
        
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            Color bgColor;
            if (isSelected)
            {
                // Purple/Blue gradient background
                bgColor = Color.FromArgb(120, 81, 238); // Purple
            }
            else if (isHovered)
            {
                bgColor = Color.FromArgb(248, 248, 252);
            }
            else
            {
                bgColor = Color.White;
            }
            
            using (var brush = new SolidBrush(bgColor))
            using (var path = GetRoundedRectPath(itemRect, 8))
            {
                g.FillPath(brush, path);
            }
            
            // Draw border
            if (!isSelected)
            {
                Color borderColor = isHovered ? Color.FromArgb(200, 200, 210) : Color.FromArgb(220, 220, 230);
                using (var pen = new Pen(borderColor, 1f))
                using (var path = GetRoundedRectPath(itemRect, 8))
                {
                    g.DrawPath(pen, path);
                }
            }
        }
        
        private void DrawRadioButton(Graphics g, Rectangle radioRect, bool isSelected, bool isHovered, SimpleItem item)
        {
            bool isDisabled = item.Text?.ToLower().Contains("poor") == true;
            Color borderColor = isSelected ? Color.White : (isDisabled ? Color.FromArgb(200, 200, 200) : Color.FromArgb(180, 180, 180));
            Color bgColor = isSelected ? Color.FromArgb(120, 81, 238) : (isDisabled ? Color.FromArgb(240, 240, 240) : Color.White);
            
            // Draw outer circle
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillEllipse(brush, radioRect);
            }
            
            using (var pen = new Pen(borderColor, 2f))
            {
                g.DrawEllipse(pen, radioRect.X + 1, radioRect.Y + 1, radioRect.Width - 3, radioRect.Height - 3);
            }
            
            // Draw inner circle if selected
            if (isSelected)
            {
                var innerRect = radioRect;
                innerRect.Inflate(-6, -6);
                
                using (var brush = new SolidBrush(Color.White))
                {
                    g.FillEllipse(brush, innerRect);
                }
            }
            
            // Draw checkmark for selected
            if (isSelected)
            {
                var checkRect = radioRect;
                checkRect.Inflate(-4, -4);
                
                using (var pen = new Pen(Color.White, 2f))
                {
                    Point[] checkPoints = new Point[]
                    {
                        new Point(checkRect.Left + 2, checkRect.Top + checkRect.Height / 2),
                        new Point(checkRect.Left + checkRect.Width / 2, checkRect.Bottom - 2),
                        new Point(checkRect.Right - 1, checkRect.Top + 2)
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
            return 68; // Taller for two-line display with radio button
        }
    }
}
