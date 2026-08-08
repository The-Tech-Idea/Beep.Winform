using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
 
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.BreadCrumbs.Helpers
{
    internal sealed class ClassicBreadcrumbPainter : BreadcrumbPainterBase
    {
        public override Rectangle CalculateItemRect(Graphics g, SimpleItem item, int x, int y, int height, bool isHovered)
        {
            string displayText = item?.Text ?? item?.Name ?? string.Empty;
            var textSize = MeasureText(g, displayText);

            int iconZone = IconZone(item, height);
            int padding = DpiScalingHelper.ScaleValue(8, Owner);
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

            if (isHovered)
            {
                var brush = PaintersFactory.GetSolidBrush(hoverBackColor);
                g.FillRectangle(brush, rect);
                var pen = PaintersFactory.GetPen(borderColor, 1);
                g.DrawRectangle(pen, rect);
            }

            button.BackColor = isHovered ? Color.FromArgb(30, hoverBackColor) : Color.Transparent;
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
