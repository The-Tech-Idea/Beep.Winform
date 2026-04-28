using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    internal static class ComboBoxPopupFocusHelper
    {
        internal static int FindRowIndexByItem(IReadOnlyList<ComboBoxPopupRowModel> rows, SimpleItem item)
        {
            if (item == null || rows == null || rows.Count == 0)
            {
                return -1;
            }

            string target = BeepComboBox.GetSimpleItemIdentity(item);
            for (int i = 0; i < rows.Count; i++)
            {
                var rowItem = rows[i]?.SourceItem;
                if (rowItem != null &&
                    string.Equals(BeepComboBox.GetSimpleItemIdentity(rowItem), target, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
