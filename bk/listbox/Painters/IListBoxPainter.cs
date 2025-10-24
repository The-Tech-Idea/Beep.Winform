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

        BeepControlStyle Style { get; set; }
        /// <summary>
        /// Initialize the painter with owner and theme
        /// </summary>
        void Initialize(BeepListBox owner, IBeepTheme theme);
        
        /// <summary>
        /// Paint the list box in the specified Style
        /// </summary>
        void Paint(Graphics g, BeepListBox owner, Rectangle drawingRect);
        
        /// <summary>
        /// Get the preferred item height for this Style
        /// </summary>
        int GetPreferredItemHeight();
        
        /// <summary>
        /// Get the preferred padding for this Style
        /// </summary>
        Padding GetPreferredPadding();
        
        /// <summary>
        /// Get whether this Style supports search
        /// </summary>
        bool SupportsSearch();
        
        /// <summary>
        /// Get whether this Style supports checkboxes
        /// </summary>
        bool SupportsCheckboxes();
    }
}
