using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
 
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.BreadCrumbs.Helpers
{
    internal sealed class ClassicBreadcrumbPainter : BreadcrumbPainterBase
    {
        public override Rectangle CalculateItemRect(Graphics g, SimpleItem item, int x, int y, int height, bool isHovered)
        {
            string displayText = item?.Text ?? item?.Name ?? string.Empty;
            var textSize = MeasureText(g, displayText);

            int iconWidth = (ShowIcons && !string.IsNullOrEmpty(item?.ImagePath)) ? 20 : 0;
            int padding = 8;
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
                using var brush = new SolidBrush(Color.FromArgb(50, Theme.ButtonHoverBackColor));
                g.FillRectangle(brush, rect);
                using var pen = new Pen(Theme.ButtonHoverBorderColor, 1);
                g.DrawRectangle(pen, rect);
            }

            button.BackColor = isHovered ? Color.FromArgb(30, Theme.ButtonHoverBackColor) : Color.Transparent;
            button.ForeColor = isLast ? Theme.ButtonForeColor : Theme.LinkColor;
            button.Draw(g, rect);
        }
    }
}
