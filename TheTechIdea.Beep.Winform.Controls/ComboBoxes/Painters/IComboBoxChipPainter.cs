using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Role-based painter specifically for drawing multi-select chips.
    /// Used by the main field painters to delegate chip rendering.
    /// </summary>
    internal interface IComboBoxChipPainter
    {
        /// <summary>
        /// Renders all chips defined in the layout snapshot.
        /// </summary>
        void PaintChips(Graphics g, BeepComboBox owner, ComboBoxRenderState state, ComboBoxLayoutSnapshot layout);
    }
}
