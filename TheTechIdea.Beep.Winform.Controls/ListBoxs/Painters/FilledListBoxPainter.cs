using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Filled list box with elevation and shadow
    /// </summary>
    internal class FilledListBoxPainter : BaseListBoxPainter
    {
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            var rect = itemRect;
            rect.Inflate(-4, -2);
            
            // Draw shadow for elevation
            if (isHovered || isSelected)
            {
                DrawShadow(g, rect);
            }
            
            DrawItemBackground(g, rect, isHovered, isSelected);
            
            int currentX = rect.Left + 12;
            
            // Draw image if enabled
            if (_owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath))
            {
                int imgSize = Math.Min(rect.Height - 8, 32);
                Rectangle imgRect = new Rectangle(currentX, rect.Y + (rect.Height - imgSize) / 2, imgSize, imgSize);
                DrawItemImage(g, imgRect, item.ImagePath);
                currentX += imgSize + 12;
            }
            
            // Draw text
            Rectangle textRect = new Rectangle(currentX, rect.Y, rect.Right - currentX - 12, rect.Height);
            Color textColor = isSelected ? Color.White : _helper.GetTextColor();
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);
        }
        
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            Color bgColor;
            if (isSelected)
                bgColor = _theme?.PrimaryColor ?? Color.Blue;
            else if (isHovered)
                bgColor = Color.FromArgb(240, 240, 245);
            else
                bgColor = Color.FromArgb(250, 250, 250);
            
            using (var brush = new SolidBrush(bgColor))
            using (var path = GetRoundedRectPath(itemRect, 6))
            {
                g.FillPath(brush, path);
            }
        }
        
        private void DrawShadow(Graphics g, Rectangle rect)
        {
            var shadowRect = rect;
            shadowRect.Offset(0, 2);
            
            using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
            using (var path = GetRoundedRectPath(shadowRect, 6))
            {
                g.FillPath(shadowBrush, path);
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
            return 44;
        }
    }
}
