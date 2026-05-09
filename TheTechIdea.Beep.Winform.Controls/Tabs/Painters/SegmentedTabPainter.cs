using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    public class SegmentedTabPainter : BaseTabPainter
    {
        public SegmentedTabPainter(BeepTabs tabControl) : base(tabControl) { }

        public override void PaintTab(Graphics g, RectangleF tabRect, int index, bool isSelected, bool isHovered, float alpha = 1.0f)
        {
            Color baseColor = TheTechIdea.Beep.Winform.Controls.Tabs.Helpers.TabThemeHelpers.GetTabBackgroundColor(Theme, Theme != null, isSelected, isHovered);
            Color backgroundColor = Color.FromArgb((int)(alpha * 255), baseColor.R, baseColor.G, baseColor.B);
            
            using (GraphicsPath path = GetRoundedRect(tabRect, 6))
            {
                var brush = PaintersFactory.GetSolidBrush(backgroundColor);
                g.FillPath(brush, path);
            }

            bool vertical = (TabControl.HeaderPosition == TabHeaderPosition.Left || TabControl.HeaderPosition == TabHeaderPosition.Right);
            DrawTabText(g, tabRect, TabControl.GetTabTitle(index), index, isSelected, vertical, alpha);

            if (TabControl.ShowCloseButtons)
            {
                DrawCloseButton(g, tabRect, vertical);
            }
        }

        public override void PaintTabItem(Graphics g, Tabs.Models.BeepTabHeaderItemLayout itemLayout, float alpha = 1.0f)
        {
            Color baseColor = TheTechIdea.Beep.Winform.Controls.Tabs.Helpers.TabThemeHelpers.GetTabBackgroundColor(Theme, Theme != null, itemLayout.Item.IsSelected, itemLayout.Item.IsHovered);
            Color backgroundColor = Color.FromArgb((int)(alpha * 255), baseColor.R, baseColor.G, baseColor.B);

            using (GraphicsPath path = GetRoundedRect(itemLayout.Bounds, 6))
            {
                var brush = PaintersFactory.GetSolidBrush(backgroundColor);
                g.FillPath(brush, path);
            }

            DrawTabItemContent(g, itemLayout, alpha);
        }
    }
}
