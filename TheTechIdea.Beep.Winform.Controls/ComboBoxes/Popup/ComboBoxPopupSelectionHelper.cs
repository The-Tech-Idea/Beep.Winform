using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    internal static class ComboBoxPopupSelectionHelper
    {
        internal static bool TryBuildSelectAllToggle(ComboBoxPopupRowModel row, out ComboBoxPopupRowModel toggled)
        {
            toggled = null;
            if (row == null || !row.IsCheckable || row.IsChecked || !ComboBoxPopupRowBehavior.IsSelectable(row))
            {
                return false;
            }

            toggled = ComboBoxPopupRowBehavior.BuildToggledCheckRow(row, isChecked: true);
            return true;
        }

        internal static bool TryBuildClearAllToggle(ComboBoxPopupRowModel row, out ComboBoxPopupRowModel toggled)
        {
            toggled = null;
            if (row == null || !row.IsCheckable || !row.IsChecked || !ComboBoxPopupRowBehavior.IsSelectable(row))
            {
                return false;
            }

            toggled = ComboBoxPopupRowBehavior.BuildToggledCheckRow(row, isChecked: false);
            return true;
        }
    }
}
