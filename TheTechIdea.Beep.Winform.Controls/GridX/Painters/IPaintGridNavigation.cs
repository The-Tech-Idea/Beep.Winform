using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Interface for painting grid navigation area with different styles
    /// </summary>
    public interface IPaintGridNavigation
    {
        /// <summary>
        /// Paint the entire navigation area
        /// </summary>
        void PaintNavigation(Graphics g, Rectangle navigationRect, BeepGridPro grid);

        /// <summary>
        /// Get the name/identifier of this painter Style
        /// </summary>
        string StyleName { get; }

        /// <summary>
        /// Register hit areas for navigation interactions (buttons, paging controls)
        /// </summary>
        void RegisterNavigationHitAreas(BeepGridPro grid);

        /// <summary>
        /// Update page information display
        /// </summary>
        void UpdatePageInfo(int currentPage, int totalPages, int totalRecords);

        /// <summary>
        /// Get preferred height for this navigation Style
        /// </summary>
        int GetPreferredHeight();
    }
}
