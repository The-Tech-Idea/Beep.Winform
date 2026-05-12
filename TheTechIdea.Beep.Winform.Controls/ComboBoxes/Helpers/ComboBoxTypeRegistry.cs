using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters;

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
    }
}
