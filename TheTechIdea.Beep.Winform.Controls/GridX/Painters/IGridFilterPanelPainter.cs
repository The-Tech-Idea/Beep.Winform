using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Contract for painting the top filter panel in BeepGridPro.
    /// Implementations should render visuals and populate hit rectangles for input interactions.
    /// </summary>
    public interface IGridFilterPanelPainter
    {
        /// <summary>
        /// Gets the style this painter implements.
        /// </summary>
        navigationStyle Style { get; }

        /// <summary>
        /// Gets a human-readable style name.
        /// </summary>
        string StyleName { get; }

        /// <summary>
        /// Calculates a recommended top filter panel height for this style.
        /// </summary>
        int CalculateFilterPanelHeight(BeepGridPro grid);

        /// <summary>
        /// Paints the filter panel and updates hit-test rectangles for filter and clear interactions.
        /// </summary>
        void PaintFilterPanel(
            Graphics g,
            Rectangle panelRect,
            BeepGridPro grid,
            IBeepTheme? theme,
            Dictionary<int, Rectangle> filterCellRects,
            Dictionary<int, Rectangle> clearIconRects);
    }
}
