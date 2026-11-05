using System.Collections.Generic;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    /// <summary>
    /// Minimal adapter contract for hosts that need to drive an Excel-style
    /// filter popup without depending on <see cref="BeepGridPro"/> internals.
    /// </summary>
    public interface BeepSimpleGridLike
    {
        IList<BeepColumnConfig> Columns { get; }
        System.ComponentModel.BindingList<BeepRowConfig> Rows { get; }
        void Sort(string columnName, SortDirection direction);
        void ClearFilter();
        void ApplyInFilter(string columnName, IEnumerable<object> selectedValues);
        Control AsControl();
    }
}
