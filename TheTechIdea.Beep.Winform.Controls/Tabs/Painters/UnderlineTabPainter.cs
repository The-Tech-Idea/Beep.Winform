using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    public class UnderlineTabPainter : BaseTabPainter
    {
        public UnderlineTabPainter(BeepTabs tabControl) : base(tabControl) { }

        public override void PaintTab(Graphics g, RectangleF tabRect, int index, bool isSelected, bool isHovered, float alpha = 1.0f)
        {
            // No background fill for underline style
            
            bool vertical = (TabControl.HeaderPosition == TabHeaderPosition.Left || TabControl.HeaderPosition == TabHeaderPosition.Right);
            DrawTabText(g, tabRect, TabControl.GetTabTitle(index), index, isSelected, vertical, alpha);

            if (TabControl.ShowCloseButtons)
            {
                DrawCloseButton(g, tabRect, vertical);
            }
        }

        public override void PaintTabItem(Graphics g, Tabs.Models.BeepTabHeaderItemLayout itemLayout, float alpha = 1.0f)
        {
            DrawTabItemContent(g, itemLayout, alpha);
        }
    }
}
