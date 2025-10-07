using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Raised/elevated checkbox list (from image 2 - bottom right "ELEVATED ERROR")
    /// Shows elevated error states with raised appearance
    /// </summary>
    internal class RaisedCheckboxesPainter : BaseListBoxPainter
    {
        public override bool SupportsCheckboxes() => true;
        
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            var rect = itemRect;
            rect.Inflate(-6, -3);
            
            // Draw shadow for elevation
            if (isSelected || isHovered)
            {
                DrawElevationShadow(g, rect);
            }
            
            DrawItemBackground(g, rect, isHovered, isSelected);
            
            int currentX = rect.Left + 12;
            
            // Draw raised checkbox
            Rectangle checkRect = new Rectangle(currentX, rect.Y + (rect.Height - 22) / 2, 22, 22);
            bool isChecked = _owner.SelectedItems?.Contains(item) == true;
            DrawRaisedCheckbox(g, checkRect, isChecked, item);
            currentX += 30;
            
            // Draw text
            Rectangle textRect = new Rectangle(currentX, rect.Y, rect.Right - currentX - 12, rect.Height);
            Color textColor = GetItemTextColor(item);
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);
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
                return Color.FromArgb(255, 235, 235); // Light red/pink
            else if (isHovered)
                return Color.FromArgb(252, 245, 245);
            else
                return Color.White;
        }
        
        private Color GetItemTextColor(SimpleItem item)
        {
            bool isDisabled = item.Text?.ToLower().Contains("disabled") == true;
            return isDisabled ? Color.FromArgb(180, 180, 180) : Color.FromArgb(60, 60, 60);
        }
        
        private void DrawElevationShadow(Graphics g, Rectangle rect)
        {
            // Draw multiple shadow layers for depth
            for (int i = 3; i >= 1; i--)
            {
                var shadowRect = rect;
                shadowRect.Offset(0, i);
                
                int alpha = 10 + (i * 5);
                using (var shadowBrush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
                using (var path = GetRoundedRectPath(shadowRect, 6))
                {
                    g.FillPath(shadowBrush, path);
                }
            }
        }
        
        private void DrawRaisedCheckbox(Graphics g, Rectangle checkboxRect, bool isChecked, SimpleItem item)
        {
            bool isDisabled = item.Text?.ToLower().Contains("disabled") == true;
            Color borderColor = isDisabled ? Color.FromArgb(200, 200, 200) : Color.FromArgb(220, 53, 69);
            Color bgColor = isChecked ? Color.FromArgb(220, 53, 69) : Color.White;
            
            if (isDisabled)
            {
                bgColor = Color.FromArgb(240, 240, 240);
            }
            
            // Draw checkbox shadow for raised effect
            var shadowRect = checkboxRect;
            shadowRect.Offset(0, 2);
            using (var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
            {
                g.FillRectangle(shadowBrush, shadowRect);
            }
            
            // Draw background
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, checkboxRect);
            }
            
            // Draw border
            using (var pen = new Pen(borderColor, 2f))
            {
                g.DrawRectangle(pen, checkboxRect.X, checkboxRect.Y, checkboxRect.Width - 1, checkboxRect.Height - 1);
            }
            
            // Draw checkmark if checked
            if (isChecked && !isDisabled)
            {
                using (var pen = new Pen(Color.White, 2.5f))
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
            return 52;
        }
    }
}
