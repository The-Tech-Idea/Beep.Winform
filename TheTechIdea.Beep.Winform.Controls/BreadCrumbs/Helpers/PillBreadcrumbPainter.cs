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
            int iconZone = IconZone(item, height);
            int padding = DpiScalingHelper.ScaleValue(10, Owner);
            int width = iconZone + textSize.Width + padding * 2;
            return new Rectangle(x, y, width, height);
        }

        public override void DrawItem(Graphics g, BeepButton button, SimpleItem item, Rectangle rect, bool isHovered, bool isSelected, bool isLast)
        {
            string displayText = item?.Text ?? item?.Name ?? string.Empty;
            button.Text = displayText;
            
            // Don't set button.ImagePath - we'll paint icons using StyledImagePainter directly
            button.ImagePath = string.Empty;
            
            button.IsHovered = isHovered;
            button.IsSelected = isSelected;
            button.TextFont = TextFont; // same font MeasureText sized the rect with

            // Use BreadcrumbThemeHelpers for colors
            var (textColor, hoverBackColor, selectedBackColor, separatorColor, borderColor) =
                BreadcrumbThemeHelpers.GetThemeColors(Theme, isLast, isHovered, isSelected);

            var pillRect = new Rectangle(rect.X, rect.Y + 4, rect.Width, rect.Height - 8);
            using var path = Base.Helpers.ControlPaintHelper.GetRoundedRectPath(pillRect, pillRect.Height / 2);
            if (isHovered)
            {
                var brush = PaintersFactory.GetSolidBrush(hoverBackColor);
                g.FillPath(brush, path);
            }
            if (isLast || isSelected)
            {
                var brush = PaintersFactory.GetSolidBrush(selectedBackColor);
                g.FillPath(brush, path);
            }
            button.BackColor = Color.Transparent;
            button.ForeColor = textColor;
            button.Draw(g, TextRect(rect, item));
            
            // Paint icon using StyledImagePainter
            if (ShowIcons && !string.IsNullOrEmpty(item?.ImagePath))
            {
                PaintIcon(g, rect, item, isLast, isHovered);
            }
        }
    }
}
