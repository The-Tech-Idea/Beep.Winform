using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    public class CardTabPainter : BaseTabPainter
    {
        public CardTabPainter(BeepTabs tabControl) : base(tabControl) { }

        public override void PaintTab(Graphics g, RectangleF tabRect, int index, bool isSelected, bool isHovered, float alpha = 1.0f)
        {
            Color baseColor = isSelected ? Theme.TabSelectedBackColor : Theme.TabBackColor;
            Color borderColor = Theme.BorderColor;
            
            // Adjust rect for card look (connect to bottom if selected)
            RectangleF drawRect = tabRect;
            
            using (GraphicsPath path = GetRoundedTopRect(drawRect, 4))
            {
                // Fill
                Color fillColor = isSelected ? baseColor : Color.FromArgb((int)(alpha * 255), Theme.ButtonBackColor);
                if (isHovered && !isSelected)
                {
                    fillColor = Theme.ButtonHoverBackColor;
                }

                using (var brush = PaintersFactory.GetSolidBrush(fillColor))
                {
                    g.FillPath(brush, path);
                }

                // Border
                using (var pen = PaintersFactory.GetPen(borderColor))
                {
                    g.DrawPath(pen, path);
                }
                
                // If selected, cover the bottom border line to merge with content
                if (isSelected)
                {
                    using (var pen = PaintersFactory.GetPen(fillColor))
                    {
                        g.DrawLine(pen, drawRect.Left + 1, drawRect.Bottom, drawRect.Right - 1, drawRect.Bottom);
                    }
                }
            }

            bool vertical = (TabControl.HeaderPosition == TabHeaderPosition.Left || TabControl.HeaderPosition == TabHeaderPosition.Right);
            DrawTabText(g, tabRect, TabControl.TabPages[index].Text, isSelected, vertical, alpha);

            if (TabControl.ShowCloseButtons)
            {
                DrawCloseButton(g, tabRect, vertical);
            }
        }

        private GraphicsPath GetRoundedTopRect(RectangleF rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            RectangleF arc = new RectangleF(rect.Location, new SizeF(diameter, diameter));
            
            // Top Left
            path.AddArc(arc, 180, 90);
            
            // Top Right
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            
            // Bottom Right
            path.AddLine(rect.Right, rect.Top + radius, rect.Right, rect.Bottom);
            
            // Bottom Left
            path.AddLine(rect.Right, rect.Bottom, rect.Left, rect.Bottom);
            path.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top + radius);
            
            path.CloseFigure();
            return path;
        }
    }
}
