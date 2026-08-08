using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
 
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.BreadCrumbs.Helpers
{
    // New optional Style using chevron-like right arrow shapes between items
    internal sealed class ChevronBreadcrumbPainter : BreadcrumbPainterBase
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

            // Chevron container background when hovered/selected
            if (isHovered || isSelected)
            {
                using var path = CreateChevronPath(rect);
                var brush = PaintersFactory.GetSolidBrush(isSelected ? selectedBackColor : hoverBackColor);
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

        private static GraphicsPath CreateChevronPath(Rectangle rect)
        {
            int arrow = rect.Height /3;
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddLine(rect.Left, rect.Top, rect.Right - arrow, rect.Top);
            path.AddLine(rect.Right - arrow, rect.Top, rect.Right, rect.Top + rect.Height /2);
            path.AddLine(rect.Right, rect.Top + rect.Height /2, rect.Right - arrow, rect.Bottom);
            path.AddLine(rect.Right - arrow, rect.Bottom, rect.Left, rect.Bottom);
            path.CloseFigure();
            return path;
        }

        public override int DrawSeparator(Graphics g, BeepLabel label, int x, int y, int height, string separatorText, Font textFont, Color separatorColor, int itemSpacing)
        {
            // Draw a simple right chevron instead of text-based separator
            int arrow = height /3;
            var pen = PaintersFactory.GetPen(Color.FromArgb(160, separatorColor),2);
            int cx = x + itemSpacing + arrow;
            int cy = y + height /2;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawLines(pen, new[]
 {
 new Point(cx - arrow, cy - arrow),
 new Point(cx, cy),
 new Point(cx - arrow, cy + arrow)
 });
            return arrow *2 + itemSpacing *2; // width consumed
        }
    }
}
