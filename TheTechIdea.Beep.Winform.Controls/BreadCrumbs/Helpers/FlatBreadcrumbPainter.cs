using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
 
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.BreadCrumbs.Helpers
{
    internal sealed class FlatBreadcrumbPainter : BreadcrumbPainterBase
    {
        public override Rectangle CalculateItemRect(Graphics g, SimpleItem item, int x, int y, int height, bool isHovered)
        {
            string displayText = item?.Text ?? item?.Name ?? string.Empty;
            var textSize = MeasureText(g, displayText);
            int iconWidth = (ShowIcons && !string.IsNullOrEmpty(item?.ImagePath)) ? 20 : 0;
            int padding = 6;
            int width = textSize.Width + padding * 2 + iconWidth + (iconWidth > 0 ? 4 : 0);
            return new Rectangle(x, y, width, height);
        }

        public override void DrawItem(Graphics g, BeepButton button, SimpleItem item, Rectangle rect, bool isHovered, bool isSelected, bool isLast)
        {
            string displayText = item?.Text ?? item?.Name ?? string.Empty;
            button.Text = displayText;
            button.ImagePath = (ShowIcons && !string.IsNullOrEmpty(item?.ImagePath)) ? item.ImagePath : string.Empty;
            button.IsHovered = isHovered;
            button.IsSelected = isSelected;
            if (isHovered)
            {
                var underlineRect = new Rectangle(rect.X, rect.Bottom - 2, rect.Width, 2);
                using var brush = new SolidBrush(Theme.LinkColor);
                g.FillRectangle(brush, underlineRect);
            }
            button.BackColor = Color.Transparent;
            button.ForeColor = isLast ? Theme.ButtonForeColor : Theme.LinkColor;
            button.Draw(g, rect);
        }
    }
}
