using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Interface for painting grid column headers with different styles
    /// </summary>
    public interface IPaintGridHeader
    {
        /// <summary>
        /// Paint the entire header area
        /// </summary>
        void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid);

        /// <summary>
        /// Paint a single header cell
        /// </summary>
        void PaintHeaderCell(Graphics g, Rectangle cellRect, BeepColumnConfig column, int columnIndex, BeepGridPro grid);

        /// <summary>
        /// Get the name/identifier of this painter style
        /// </summary>
        string StyleName { get; }

        /// <summary>
        /// Register hit areas for header interactions (sort, filter, resize)
        /// </summary>
        void RegisterHeaderHitAreas(BeepGridPro grid);
    }
}
