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

            int iconZone = IconZone(item, height);
            int padding = DpiScalingHelper.ScaleValue(8, Owner);
            int width = iconZone + textSize.Width + padding * 2;

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
                using var path = Base.Helpers.ControlPaintHelper.GetRoundedRectPath(rect, 4);
                var brush = PaintersFactory.GetSolidBrush(hoverBackColor);
                g.FillPath(brush, path);
            }

            // The current (last/selected) crumb carries a rounded chip at rest - this is the
            // style's identity. Without it, Modern rendered pixel-identical to Classic idle
            // (Classic's old "distinct look" was an accidental hardcoded 10pt font).
            if (isSelected || isLast)
            {
                using var path = Base.Helpers.ControlPaintHelper.GetRoundedRectPath(rect, 4);
                var brush = PaintersFactory.GetSolidBrush(selectedBackColor);
                g.FillPath(brush, path);
            }
            
            button.BackColor = Color.Transparent;
            button.ForeColor = textColor;
            button.IsRounded = true;
            button.BorderRadius = 4;
            button.Draw(g, TextRect(rect, item));
            
            // Paint icon using StyledImagePainter (if ShowIcons is true)
            // This ensures icons are painted with proper theme colors and tinting
            if (ShowIcons && !string.IsNullOrEmpty(item?.ImagePath))
            {
                PaintIcon(g, rect, item, isLast, isHovered);
            }
        }
    }
}
