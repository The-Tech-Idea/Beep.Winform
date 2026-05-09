using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    public interface ITabPainter
    {
        BeepTabs TabControl { get; set; }
        IBeepTheme Theme { get; set; }

        void PaintBackground(Graphics g, Rectangle bounds);
        void PaintHeaderBackground(Graphics g, Rectangle headerBounds);

        /// <summary>
        /// Legacy paint overload – used by the current BeepTabs shell.
        /// New host code should call <see cref="PaintTabItem"/> instead.
        /// </summary>
        void PaintTab(Graphics g, RectangleF tabRect, int index, bool isSelected, bool isHovered, float alpha = 1.0f);

        SizeF MeasureTab(Graphics g, int index, Font font);
        RectangleF GetCloseButtonRect(RectangleF tabRect, bool vertical);

        /// <summary>
        /// Phase 2 paint overload – accepts the full layout produced by the custom host pipeline,
        /// including adornment bounds (icon, subtext, badge, dirty marker, busy indicator).
        /// Painters that do not override this will fall back to <see cref="PaintTab"/> via
        /// <see cref="BaseTabPainter"/>.
        /// </summary>
        void PaintTabItem(Graphics g, BeepTabHeaderItemLayout itemLayout, float alpha = 1.0f);
    }
}
