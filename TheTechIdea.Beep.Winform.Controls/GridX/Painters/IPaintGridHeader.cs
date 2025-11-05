using System;
using System.Drawing;
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Enhanced interface for painting grid column headers with different styles.
    /// Header painters use the same navigationStyle enum as navigation painters for consistency.
    /// This ensures headers and navigation bars can be coordinated to match.
    /// </summary>
    public interface IPaintGridHeader
    {
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
