using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
 
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.BreadCrumbs.Helpers
{
    internal sealed class ModernBreadcrumbPainter : BreadcrumbPainterBase
    {
        public override Rectangle CalculateItemRect(Graphics g, SimpleItem item, int x, int y, int height, bool isHovered)
        {
            string displayText = item?.Text ?? item?.Name ?? string.Empty;
            var textSize = MeasureText(g, displayText);

            int iconWidth = (ShowIcons && !string.IsNullOrEmpty(item?.ImagePath)) ? 20 : 0;
            int padding = 8;
            int width = textSize.Width + padding * 2 + iconWidth + (iconWidth > 0 ? 4 : 0);

            if (isHovered)
            {
                width += 4; height = height - 2; y += 1;
            }
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
                using var path = Base.Helpers.ControlPaintHelper.GetRoundedRectPath(rect, 4);
                var brush = PaintersFactory.GetSolidBrush(Color.FromArgb(40, Theme.ButtonHoverBackColor));
                g.FillPath(brush, path);
            }
            button.BackColor = Color.Transparent;
            button.ForeColor = isLast ? Theme.ButtonForeColor : Theme.LinkColor;
            button.IsRounded = true;
            button.BorderRadius = 4;
            button.Draw(g, rect);
        }
    }
}
