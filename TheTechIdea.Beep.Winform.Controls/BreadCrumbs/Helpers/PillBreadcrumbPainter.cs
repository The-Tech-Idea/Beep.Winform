using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
 
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.BreadCrumbs.Helpers
{
    internal sealed class PillBreadcrumbPainter : BreadcrumbPainterBase
    {
        public override Rectangle CalculateItemRect(Graphics g, SimpleItem item, int x, int y, int height, bool isHovered)
        {
            string displayText = item?.Text ?? item?.Name ?? string.Empty;
            var textSize = MeasureText(g, displayText);
            int iconWidth = (ShowIcons && !string.IsNullOrEmpty(item?.ImagePath)) ? 20 : 0;
            int padding = 10;
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

            var pillRect = new Rectangle(rect.X, rect.Y + 4, rect.Width, rect.Height - 8);
            using var path = Base.Helpers.ControlPaintHelper.GetRoundedRectPath(pillRect, pillRect.Height / 2);
            if (isHovered)
            {
                var brush = PaintersFactory.GetSolidBrush(Color.FromArgb(60, Theme.ButtonHoverBackColor));
                g.FillPath(brush, path);
            }
            if (isLast)
            {
                var brush = PaintersFactory.GetSolidBrush(Color.FromArgb(80, Theme.ButtonBackColor));
                g.FillPath(brush, path);
            }
            button.BackColor = Color.Transparent;
            button.ForeColor = isLast ? Theme.ButtonForeColor : Theme.LinkColor;
            button.Draw(g, rect);
        }
    }
}
