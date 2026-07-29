using System;
using System.Drawing;
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Geometry of one header cell, produced by the painter and consumed by the renderer.
    /// </summary>
    /// <remarks>
    /// The painter owns header geometry so each style can place things its own way, while the
    /// renderer still knows where the interactive parts landed: it copies
    /// <see cref="SortIndicatorRect"/> and <see cref="MenuButtonRect"/> into the grid's hit-rect
    /// dictionaries, so input handling needs no per-style knowledge.
    /// </remarks>
    public sealed class HeaderCellLayout
    {
        /// <summary>Area the caption is drawn in — already excludes the indicator and menu slots.</summary>
        public Rectangle TextRect { get; set; }

        /// <summary>Sort indicator slot. Reserved whenever the column is sortable, so text does not reflow on sort.</summary>
        public Rectangle SortIndicatorRect { get; set; }

        /// <summary>The single column menu button (sort + filter + clear). Empty when the column offers no menu.</summary>
        public Rectangle MenuButtonRect { get; set; }

        /// <summary>Clickable area that toggles sort — the caption plus its indicator.</summary>
        public Rectangle SortHitRect { get; set; }
    }

    /// <summary>
    /// Enhanced interface for painting grid column headers with different styles.
    /// Header painters use the same navigationStyle enum as navigation painters for consistency.
    /// This ensures headers and navigation bars can be coordinated to match.
    /// </summary>
    public interface IPaintGridHeader
    {
        /// <summary>
        /// Computes where the caption, sort indicator and column menu button go for one cell.
        /// </summary>
        /// <remarks>
        /// Called by the renderer before painting. Implementations should reserve the sort slot for
        /// sortable columns whether or not the column is currently sorted, and size everything from
        /// <paramref name="dpiScale"/> rather than fixed pixels.
        /// </remarks>
        HeaderCellLayout CalculateHeaderCellLayout(Rectangle cellRect, BeepColumnConfig column,
            BeepGridPro grid, float dpiScale);

        /// <summary>
        /// Paints the column menu button (funnel/chevron) that opens sort + filter + clear.
        /// </summary>
        void PaintColumnMenuButton(Graphics g, Rectangle rect, bool filterActive, bool isHovered, IBeepTheme? theme);

        /// <summary>
        /// Get the name/identifier of this painter style
        /// </summary>
        string StyleName { get; }

        /// <summary>
        /// Get the style type this painter implements (same enum as navigation)
        /// </summary>
        navigationStyle Style { get; }

        /// <summary>
        /// Paint the entire header area
        /// </summary>
        void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid, IBeepTheme? theme);

        /// <summary>
        /// Paint a single header cell
        /// </summary>
        void PaintHeaderCell(Graphics g, Rectangle cellRect, BeepColumnConfig column, 
            int columnIndex, BeepGridPro grid, IBeepTheme? theme);

        /// <summary>
        /// Calculate the recommended height for headers with this style
        /// </summary>
        int CalculateHeaderHeight(BeepGridPro grid);

        /// <summary>
        /// Calculate the recommended padding for header cells
        /// </summary>
        int CalculateHeaderPadding();

        /// <summary>
        /// Register hit areas for header interactions (sort, filter, resize)
        /// </summary>
        void RegisterHeaderHitAreas(BeepGridPro grid);

        /// <summary>
        /// Paint sort indicator
        /// </summary>
        void PaintSortIndicator(Graphics g, Rectangle rect, SortDirection direction, IBeepTheme? theme);

        /// <summary>
        /// Paint filter icon
        /// </summary>
        void PaintFilterIcon(Graphics g, Rectangle rect, bool active, IBeepTheme? theme);

        /// <summary>
        /// Paint header cell background
        /// </summary>
        void PaintHeaderBackground(Graphics g, Rectangle rect, bool isHovered, IBeepTheme? theme);

        /// <summary>
        /// Paint header cell text
        /// </summary>
        void PaintHeaderText(Graphics g, Rectangle rect, string text, Font font, 
            ContentAlignment alignment, IBeepTheme? theme);
    }
}
