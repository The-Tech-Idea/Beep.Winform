using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Linq;

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
            
            DrawItemBackground(g, rect, isHovered, isSelected);

            // Use layout rects for content
            var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
            var iconRect = info?.IconRect ?? Rectangle.Empty;
            var textRect = info?.TextRect ?? rect;

            if (_owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath) && !iconRect.IsEmpty)
            {
                DrawItemImage(g, iconRect, item.ImagePath);
            }

            Color textColor = isSelected ? Color.White : _helper.GetTextColor();
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);
        }
        
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for Filled Style background, border, and shadow
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, isSelected, Style);
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
