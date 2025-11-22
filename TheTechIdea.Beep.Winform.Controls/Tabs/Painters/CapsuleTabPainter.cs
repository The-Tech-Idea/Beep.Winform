using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    public class CapsuleTabPainter : BaseTabPainter
    {
        public CapsuleTabPainter(BeepTabs tabControl) : base(tabControl) { }

        public override void PaintTab(Graphics g, RectangleF tabRect, int index, bool isSelected, bool isHovered, float alpha = 1.0f)
        {
            Color baseColor = isSelected ? Theme.TabSelectedBackColor : Theme.TabBackColor;
            Color backgroundColor = Color.FromArgb((int)(alpha * 255), baseColor.R, baseColor.G, baseColor.B);
            var radius = (int)Math.Min(tabRect.Height / 2, 18);
            
            using (GraphicsPath path = GetRoundedRect(tabRect, radius))
            {
                var brush = PaintersFactory.GetSolidBrush(backgroundColor);
                g.FillPath(brush, path);
            }

            bool vertical = (TabControl.HeaderPosition == TabHeaderPosition.Left || TabControl.HeaderPosition == TabHeaderPosition.Right);
            DrawTabText(g, tabRect, TabControl.TabPages[index].Text, isSelected, vertical, alpha);

            if (TabControl.ShowCloseButtons)
            {
                DrawCloseButton(g, tabRect, vertical);
            }
        }
    }
}
