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
            int iconWidth = (ShowIcons && !string.IsNullOrEmpty(item?.ImagePath)) ?20 :0;
            int padding =10;
            int width = textSize.Width + padding *2 + iconWidth + (iconWidth >0 ?4 :0);
            return new Rectangle(x, y, width, height);
        }

        public override void DrawItem(Graphics g, BeepButton button, SimpleItem item, Rectangle rect, bool isHovered, bool isSelected, bool isLast)
        {
            string displayText = item?.Text ?? item?.Name ?? string.Empty;
            button.Text = displayText;
            button.ImagePath = (ShowIcons && !string.IsNullOrEmpty(item?.ImagePath)) ? item.ImagePath : string.Empty;
            button.IsHovered = isHovered;
            button.IsSelected = isSelected;

            // Chevron container background when hovered/selected
            if (isHovered || isSelected)
            {
                using var path = CreateChevronPath(rect);
                var brush = PaintersFactory.GetSolidBrush(Color.FromArgb(isSelected ?90 :50, Theme.ButtonHoverBackColor));
                g.FillPath(brush, path);
            }

            button.BackColor = Color.Transparent;
            button.ForeColor = isLast ? Theme.ButtonForeColor : Theme.LinkColor;
            button.Draw(g, rect);
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
