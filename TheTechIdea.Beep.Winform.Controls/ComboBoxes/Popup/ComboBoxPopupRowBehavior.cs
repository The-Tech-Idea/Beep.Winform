using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    internal static class ComboBoxPopupRowBehavior
    {
        internal static bool IsStateRow(ComboBoxPopupRowModel row)
        {
            if (row == null)
            {
                return false;
            }

            return row.RowKind == ComboBoxPopupRowKind.EmptyState
                || row.RowKind == ComboBoxPopupRowKind.LoadingState
                || row.RowKind == ComboBoxPopupRowKind.NoResults;
        }

        internal static bool IsSelectable(ComboBoxPopupRowModel row)
        {
            if (row == null || !row.IsEnabled)
            {
                return false;
            }

            return row.RowKind switch
            {
                ComboBoxPopupRowKind.GroupHeader => false,
                ComboBoxPopupRowKind.Separator => false,
                ComboBoxPopupRowKind.EmptyState => false,
                ComboBoxPopupRowKind.LoadingState => false,
                ComboBoxPopupRowKind.NoResults => false,
                _ => true
            };
        }

        internal static ComboBoxPopupRowModel BuildToggledCheckRow(ComboBoxPopupRowModel row, bool isChecked)
        {
            if (row == null)
            {
                return null;
            }

            return new ComboBoxPopupRowModel
            {
                SourceItem = row.SourceItem,
                RowKind = row.RowKind,
                Text = row.Text,
                SubText = row.SubText,
                TrailingText = row.TrailingText,
                TrailingValueText = row.TrailingValueText,
                ImagePath = row.ImagePath,
                GroupName = row.GroupName,
                LayoutPreset = row.LayoutPreset,
                IsSelected = isChecked,
                IsEnabled = row.IsEnabled,
                IsKeyboardFocused = row.IsKeyboardFocused,
                IsCheckable = row.IsCheckable,
                IsChecked = isChecked,
                ListIndex = row.ListIndex,
                MatchStart = row.MatchStart,
                MatchLength = row.MatchLength
            };
        }
    }
}
