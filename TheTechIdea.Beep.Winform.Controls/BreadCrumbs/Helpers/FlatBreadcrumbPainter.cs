using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
 
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

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
            
            // Don't set button.ImagePath - we'll paint icons using StyledImagePainter directly
            button.ImagePath = string.Empty;
            
            button.IsHovered = isHovered;
            button.IsSelected = isSelected;
            
            // Use BreadcrumbThemeHelpers for colors
            var useTheme = Theme != null && Owner != null && Owner.UseThemeColors;
            var (textColor, hoverBackColor, selectedBackColor, separatorColor, borderColor) = 
                BreadcrumbThemeHelpers.GetThemeColors(Theme, useTheme, isLast, isHovered, isSelected);
            
            // Adjust colors for high contrast mode if enabled
            if (BreadcrumbAccessibilityHelpers.IsHighContrastMode())
            {
                var (hcTextColor, hcHoverBackColor, hcSeparatorColor, hcBorderColor) = 
                    BreadcrumbAccessibilityHelpers.GetHighContrastColors();
                textColor = hcTextColor;
                hoverBackColor = hcHoverBackColor;
                separatorColor = hcSeparatorColor;
                borderColor = hcBorderColor;
            }
            
            // Ensure text color meets WCAG contrast requirements
            if (Owner is BeepBreadcrump breadcrumb)
            {
                Color backColor = breadcrumb.BackColor != Color.Empty ? breadcrumb.BackColor : Color.White;
                textColor = BreadcrumbAccessibilityHelpers.AdjustForContrast(textColor, backColor, 4.5);
            }
            
            if (isHovered)
            {
                var underlineRect = new Rectangle(rect.X, rect.Bottom - 2, rect.Width, 2);
                var brush = PaintersFactory.GetSolidBrush(textColor);
                g.FillRectangle(brush, underlineRect);
            }
            button.BackColor = Color.Transparent;
            button.ForeColor = textColor;
            button.Draw(g, rect);
            
            // Paint icon using StyledImagePainter
            if (ShowIcons && !string.IsNullOrEmpty(item?.ImagePath))
            {
                PaintIcon(g, rect, item, isLast, isHovered);
            }
        }
    }
}
