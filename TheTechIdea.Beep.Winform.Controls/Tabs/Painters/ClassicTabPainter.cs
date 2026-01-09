using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    public class ClassicTabPainter : BaseTabPainter
    {
        public ClassicTabPainter(BeepTabs tabControl) : base(tabControl) { }

        public override void PaintTab(Graphics g, RectangleF tabRect, int index, bool isSelected, bool isHovered, float alpha = 1.0f)
        {
            // Use theme helpers for consistent color retrieval
            Color baseColor = TheTechIdea.Beep.Winform.Controls.Tabs.Helpers.TabThemeHelpers.GetTabBackgroundColor(
                Theme, 
                Theme != null, 
                isSelected, 
                isHovered);
            Color backgroundColor = Color.FromArgb((int)(alpha * 255), baseColor.R, baseColor.G, baseColor.B);
            
            // Get border radius from style helpers
            int borderRadius = TheTechIdea.Beep.Winform.Controls.Tabs.Helpers.TabStyleHelpers.GetBorderRadius(
                TabControl.TabStyle, 
                TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.Material3);
            
            using (GraphicsPath path = GetRoundedRect(tabRect, borderRadius))
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
