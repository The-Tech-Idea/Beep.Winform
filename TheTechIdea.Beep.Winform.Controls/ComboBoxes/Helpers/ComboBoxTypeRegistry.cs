using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers
{
    internal static class ComboBoxTypeRegistry
    {
        internal static IComboBoxPainter CreatePainter(ComboBoxType type)
        {
            return type switch
            {
                ComboBoxType.OutlineDefault => new OutlineDefaultComboBoxPainter(),
                ComboBoxType.OutlineSearchable => new OutlineSearchableComboBoxPainter(),
                ComboBoxType.FilledSoft => new FilledSoftComboBoxPainter(),
                ComboBoxType.RoundedPill => new RoundedPillComboBoxPainter(),
                ComboBoxType.SegmentedTrigger => new SegmentedTriggerComboBoxPainter(),
                ComboBoxType.MultiChipCompact => new MultiChipCompactComboBoxPainter(),
                ComboBoxType.MultiChipSearch => new MultiChipSearchComboBoxPainter(),
                ComboBoxType.DenseList => new DenseListComboBoxPainter(),
                ComboBoxType.MinimalBorderless => new MinimalBorderlessComboBoxPainter(),
                ComboBoxType.CommandMenu => new CommandMenuComboBoxPainter(),
                ComboBoxType.VisualDisplay => new VisualDisplayComboBoxPainter(),
                _ => new OutlineDefaultComboBoxPainter()
            };
        }

        internal static IComboBoxPopupHost CreatePopupHost(ComboBoxType type)
        {
            return type switch
            {
                ComboBoxType.OutlineDefault => new OutlineDefaultPopupHostForm(),
                ComboBoxType.OutlineSearchable => new OutlineSearchablePopupHostForm(),
                ComboBoxType.FilledSoft => new FilledSoftPopupHostForm(),
                ComboBoxType.RoundedPill => new RoundedPillPopupHostForm(),
                ComboBoxType.SegmentedTrigger => new SegmentedTriggerPopupHostForm(),
                ComboBoxType.MultiChipCompact => new MultiChipCompactPopupHostForm(),
                ComboBoxType.MultiChipSearch => new MultiChipSearchPopupHostForm(),
                ComboBoxType.DenseList => new DenseListPopupHostForm(),
                ComboBoxType.MinimalBorderless => new MinimalBorderlessPopupHostForm(),
                ComboBoxType.CommandMenu => new CommandMenuPopupHostForm(),
                ComboBoxType.VisualDisplay => new VisualDisplayPopupHostForm(),
                _ => new OutlineDefaultPopupHostForm()
            };
        }
    }
}
