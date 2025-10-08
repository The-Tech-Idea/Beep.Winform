using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Searchable list with integrated search field and icon (from images 1-3)
    /// </summary>
    internal class SearchableListPainter : OutlinedListBoxPainter
    {
        public override bool SupportsSearch() => true;
        
        protected override int DrawSearchArea(Graphics g, Rectangle drawingRect, int yOffset)
        {
            int searchHeight = 40;
            Rectangle searchRect = new Rectangle(drawingRect.X + 8, yOffset + 8, drawingRect.Width - 16, searchHeight);
            
            // Draw search box background with rounded corners
            Color searchBg = Color.FromArgb(245, 245, 245);
            using (var brush = new SolidBrush(searchBg))
            using (var path = GetRoundedRectPath(searchRect, 8))
            {
                g.FillPath(brush, path);
            }
            
            // Draw search icon
            int iconSize = 20;
            Rectangle iconRect = new Rectangle(searchRect.Left + 12, searchRect.Y + (searchRect.Height - iconSize) / 2, iconSize, iconSize);
            DrawSearchIcon(g, iconRect);
            
            // Draw placeholder or search text
            string displayText = string.IsNullOrEmpty(_owner.SearchText) ? "Search..." : _owner.SearchText;
            Rectangle textRect = new Rectangle(searchRect.Left + 40, searchRect.Y, searchRect.Width - 50, searchRect.Height);
            Color textColor = string.IsNullOrEmpty(_owner.SearchText) ? Color.Gray : (_theme?.PrimaryTextColor ?? Color.Black);
            
            System.Windows.Forms.TextRenderer.DrawText(g, displayText, _owner.TextFont, textRect, textColor,
                System.Windows.Forms.TextFormatFlags.Left | System.Windows.Forms.TextFormatFlags.VerticalCenter);
            
            return yOffset + searchHeight + 16;
        }
        
        private void DrawSearchIcon(Graphics g, Rectangle iconRect)
        {
            using (var pen = new Pen(Color.Gray, 2f))
            {
                // Draw magnifying glass circle
                int radius = iconRect.Width / 3;
                g.DrawEllipse(pen, iconRect.X + 2, iconRect.Y + 2, radius * 2, radius * 2);
                
                // Draw handle
                g.DrawLine(pen, iconRect.X + radius + radius - 2, iconRect.Y + radius + radius - 2,
                          iconRect.Right - 4, iconRect.Bottom - 4);
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
    }
}
