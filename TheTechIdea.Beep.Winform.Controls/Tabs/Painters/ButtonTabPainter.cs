using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    public class ButtonTabPainter : BaseTabPainter
    {
        public ButtonTabPainter(BeepTabs tabControl) : base(tabControl) { }

        public override void PaintTab(Graphics g, RectangleF tabRect, int index, bool isSelected, bool isHovered, float alpha = 1.0f)
        {
            // Shrink rect slightly to create spacing between buttons
            RectangleF buttonRect = new RectangleF(tabRect.X + 2, tabRect.Y + 2, tabRect.Width - 4, tabRect.Height - 4);

            Color fillColor = isSelected ? Theme.PrimaryColor : Theme.ButtonBackColor;
            Color foreColor = isSelected ? Color.White : Theme.ButtonForeColor;
            
            if (isHovered && !isSelected)
            {
                fillColor = Theme.ButtonHoverBackColor;
            }

            using (GraphicsPath path = GetRoundedRect(buttonRect, 4))
            {
                using (var brush = PaintersFactory.GetSolidBrush(fillColor))
                {
                    g.FillPath(brush, path);
                }
                
                // Optional border
                if (!isSelected)
                {
                    using (var pen = PaintersFactory.GetPen(Theme.BorderColor))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            bool vertical = (TabControl.HeaderPosition == TabHeaderPosition.Left || TabControl.HeaderPosition == TabHeaderPosition.Right);
            
            // Custom text drawing to handle specific button colors
            using (Font font = new Font(TabControl.Font, isSelected ? FontStyle.Bold : FontStyle.Regular))
            {
                var textBrush = PaintersFactory.GetSolidBrush(foreColor);
                string text = TabControl.TabPages[index].Text;
                
                if (!vertical)
                {
                    SizeF textSize = TextUtils.MeasureText(g, text, font);
                    PointF textPoint = new PointF(buttonRect.X + GetScaledTextPadding(), buttonRect.Y + (buttonRect.Height - textSize.Height) / 2);
                    g.DrawString(text, font, textBrush, textPoint);
                }
                else
                {
                    GraphicsState state = g.Save();
                    g.TranslateTransform(buttonRect.X + buttonRect.Width / 2, buttonRect.Y + buttonRect.Height / 2);
                    g.RotateTransform(90);
                    SizeF textSize = TextUtils.MeasureText(g, text, font);
                    PointF textPoint = new PointF(-textSize.Width / 2, -textSize.Height / 2);
                    g.DrawString(text, font, textBrush, textPoint);
                    g.Restore(state);
                }
            }

            if (TabControl.ShowCloseButtons)
            {
                DrawCloseButton(g, buttonRect, vertical);
            }
        }
    }
}
