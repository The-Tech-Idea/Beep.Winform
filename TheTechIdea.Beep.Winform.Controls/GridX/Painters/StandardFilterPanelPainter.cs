using System.Drawing;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Standard style modern top filter panel painter.
    /// </summary>
    public sealed class StandardFilterPanelPainter : BaseFilterPanelPainter
    {
        /// <summary>
        /// Gets the navigation style associated with this painter.
        /// </summary>
        public override navigationStyle Style => navigationStyle.Standard;

        /// <summary>
        /// Gets the painter display name.
        /// </summary>
        public override string StyleName => "Standard";

        /// <summary>
        /// Paints the standard modern filter toolbar and registers hit-test areas.
        /// </summary>
        public override void PaintFilterPanel(Graphics g, Rectangle panelRect, BeepGridPro grid, IBeepTheme? theme, Dictionary<int, Rectangle> filterCellRects, Dictionary<int, Rectangle> clearIconRects)
        {
            PaintModernToolbar(g, panelRect, grid, theme, filterCellRects, clearIconRects,
                ScaleModernToolbarOptions(new ModernToolbarOptions
                {
                    ControlHeight = 24,
                    CornerRadius = 5,
                    SearchMinWidth = 140,
                    SearchMaxWidth = 250,
                    ClearWidth = 80,
                    CountWidth = 86,
                    FilterWidth = 72,
                    FilterText = "Filter",
                    ClearText = "Clear",
                    CountFormat = "{0} active",
                    SearchPlaceholder = "Search",
                    FlatControls = false
                }, grid));
        }
    }
}
