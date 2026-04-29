using TheTechIdea.Beep.Winform.Controls.ListBoxs;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers
{
    internal static class ComboBoxListBoxTypeMapper
    {
        internal static ListBoxType Map(ComboBoxType type)
        {
            return type switch
            {
                ComboBoxType.OutlineDefault => ListBoxType.Outlined,
                ComboBoxType.OutlineSearchable => ListBoxType.SearchableList,
                ComboBoxType.FilledSoft => ListBoxType.Filled,
                ComboBoxType.RoundedPill => ListBoxType.Rounded,
                ComboBoxType.SegmentedTrigger => ListBoxType.NavigationRail,
                ComboBoxType.MultiChipCompact => ListBoxType.ChipStyle,
                ComboBoxType.MultiChipSearch => ListBoxType.ChipStyle,
                ComboBoxType.DenseList => ListBoxType.Compact,
                ComboBoxType.MinimalBorderless => ListBoxType.Borderless,
                ComboBoxType.CommandMenu => ListBoxType.CommandList,
                ComboBoxType.VisualDisplay => ListBoxType.AvatarList,
                _ => ListBoxType.Outlined
            };
        }
    }
}
