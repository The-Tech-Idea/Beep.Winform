using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    public interface ITabPainter
    {
        BeepTabs TabControl { get; set; }
        IBeepTheme Theme { get; set; }
        
        void PaintBackground(Graphics g, Rectangle bounds);
        void PaintHeaderBackground(Graphics g, Rectangle headerBounds);
        void PaintTab(Graphics g, RectangleF tabRect, int index, bool isSelected, bool isHovered, float alpha = 1.0f);
        SizeF MeasureTab(Graphics g, int index, Font font);
        RectangleF GetCloseButtonRect(RectangleF tabRect, bool vertical);
    }
}
