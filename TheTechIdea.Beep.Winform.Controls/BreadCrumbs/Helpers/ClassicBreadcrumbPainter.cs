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

            int iconWidth = (ShowIcons && !string.IsNullOrEmpty(item?.ImagePath)) ? 20 : 0;
            int padding = 8;
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
                var brush = PaintersFactory.GetSolidBrush(hoverBackColor);
                g.FillRectangle(brush, rect);
                var pen = PaintersFactory.GetPen(borderColor, 1);
                g.DrawRectangle(pen, rect);
            }

            button.BackColor = isHovered ? Color.FromArgb(30, hoverBackColor) : Color.Transparent;
            button.ForeColor = textColor;
            button.TextFont=BeepFontManager.GetFont(Owner?._currentTheme.ButtonFont.FontFamily,10);
            button.Draw(g, rect);
            
            // Paint icon using StyledImagePainter
            if (ShowIcons && !string.IsNullOrEmpty(item?.ImagePath))
            {
                PaintIcon(g, rect, item, isLast, isHovered);
            }
        }
    }
}
