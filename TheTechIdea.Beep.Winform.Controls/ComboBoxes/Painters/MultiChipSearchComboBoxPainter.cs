using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Multi-select chips + search — synced with BeepListBox (ListBoxType.ChipStyle).
    /// Inherits chip rendering + dashed separator + count badge from MultiChipCompact.
    /// Search lives in the popup (BeepListBox), NOT inline — so the field
    /// only displays selected chips or the placeholder when empty.
    /// The dropdown button shows a search icon (via IsSearchIconButton) to hint
    /// that the popup offers search.
    /// </summary>
    internal sealed class MultiChipSearchComboBoxPainter : MultiChipCompactComboBoxPainter
    {
        protected override bool IsSearchIconButton => true;
    }
}
