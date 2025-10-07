using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Interface for combo box variant painters
    /// Each ComboBoxType has its own painter implementation
    /// </summary>
    internal interface IComboBoxPainter
    {
        /// <summary>
        /// Initialize the painter with owner and theme
        /// </summary>
        void Initialize(BeepComboBox owner, IBeepTheme theme);
        
        /// <summary>
        /// Paint the combo box in the specified style
        /// </summary>
        void Paint(Graphics g, BeepComboBox owner, Rectangle drawingRect);
        
        /// <summary>
        /// Get the preferred button width for this style
        /// </summary>
        int GetPreferredButtonWidth();
        
        /// <summary>
        /// Get the preferred padding for this style
        /// </summary>
        Padding GetPreferredPadding();
    }
}
