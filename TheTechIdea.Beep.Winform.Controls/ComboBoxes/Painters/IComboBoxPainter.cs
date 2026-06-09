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
        void Initialize(BeepComboBox owner, IBeepTheme theme);
        void Paint(Graphics g, BeepComboBox owner, ComboBoxRenderState state, ComboBoxLayoutSnapshot layout);
        int GetPreferredButtonWidth();
        Padding GetPreferredPadding();
    }
}
