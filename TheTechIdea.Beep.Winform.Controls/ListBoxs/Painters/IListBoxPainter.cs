using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Interface for list box variant painters
    /// Each ListBoxType has its own painter implementation
    /// </summary>
    internal interface IListBoxPainter
    {
        /// <summary>
        /// Initialize the painter with owner and theme
        /// </summary>
        void Initialize(BeepListBox owner, IBeepTheme theme);
        
        /// <summary>
        /// Paint the list box in the specified style
        /// </summary>
        void Paint(Graphics g, BeepListBox owner, Rectangle drawingRect);
        
        /// <summary>
        /// Get the preferred item height for this style
        /// </summary>
        int GetPreferredItemHeight();
        
        /// <summary>
        /// Get the preferred padding for this style
        /// </summary>
        Padding GetPreferredPadding();
        
        /// <summary>
        /// Get whether this style supports search
        /// </summary>
        bool SupportsSearch();
        
        /// <summary>
        /// Get whether this style supports checkboxes
        /// </summary>
        bool SupportsCheckboxes();
    }
}
