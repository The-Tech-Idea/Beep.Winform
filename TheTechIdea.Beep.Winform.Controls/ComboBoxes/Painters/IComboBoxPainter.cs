using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;

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
        /// Paint the combo box using the computed render state and layout snapshot
        /// </summary>
        void Paint(Graphics g, BeepComboBox owner, ComboBoxRenderState state, ComboBoxLayoutSnapshot layout);
        
        /// <summary>
        /// Get the preferred button width for this Style
        /// </summary>
        int GetPreferredButtonWidth();
        
        /// <summary>
        /// Get the preferred padding for this Style
        /// </summary>
        Padding GetPreferredPadding();
    }
}
