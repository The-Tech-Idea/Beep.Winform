using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules; // for SortDirection

namespace TheTechIdea.Beep.Winform.Controls.GridX.Filters
{
    // Lightweight popup similar to Excel filter menu
    internal partial class BeepGridFilterPopup : Form
    {
        private readonly string _columnCaption;
        private readonly List<object> _values;

        public event EventHandler<SortDirection> SortRequested;
        public event EventHandler ClearRequested;
        public event EventHandler<FilterAppliedEventArgs> FilterApplied;

        public BeepGridFilterPopup(string columnCaption, IEnumerable<object> values)
        {
            _columnCaption = columnCaption;
            _values = values?.Select(v => v ?? string.Empty).Distinct().ToList() ?? new List<object>();

            InitializeComponent();

            // Designer created controls - set runtime text and data
            lblFilter.Text = _columnCaption;
            LoadValues(_values);
        }

        private void LoadValues(IEnumerable<object> values)
        {
            clbValues.Items.Clear();
            foreach (var v in values)
            {
                clbValues.Items.Add(v, true);
            }
        }

        private void ApplySearch()
        {
            string q = (txtSearch.Text ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(q))
            {
                LoadValues(_values);
                chkSelectAll.Checked = true;
                return;
            }
            var filtered = _values.Where(v => (v?.ToString() ?? string.Empty).IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0);
            LoadValues(filtered);
            chkSelectAll.Checked = true;
        }

        private void ToggleAll(bool check)
        {
            for (int i = 0; i < clbValues.Items.Count; i++)
            {
                clbValues.SetItemChecked(i, check);
            }
        }

        private void RaiseApply()
        {
            var selected = clbValues.CheckedItems.Cast<object>().ToList();
            FilterApplied?.Invoke(this, new FilterAppliedEventArgs
            {
                SelectedValues = selected,
                SearchText = txtSearch.Text?.Trim() ?? string.Empty
            });
            Close();
        }

        protected override bool ShowWithoutActivation => true;

        // Event handlers (wired in designer)
        private void btnSortAsc_Click(object sender, EventArgs e) => SortRequested?.Invoke(this, SortDirection.Ascending);
        private void btnSortDesc_Click(object sender, EventArgs e) => SortRequested?.Invoke(this, SortDirection.Descending);
        private void btnClear_Click(object sender, EventArgs e) => ClearRequested?.Invoke(this, EventArgs.Empty);
        private void txtSearch_TextChanged(object sender, EventArgs e) => ApplySearch();
        private void chkSelectAll_CheckedChanged(object sender, EventArgs e) => ToggleAll(chkSelectAll.Checked);
        private void btnApply_Click(object sender, EventArgs e) => RaiseApply();
        private void btnCancel_Click(object sender, EventArgs e) => Close();
    }

    internal class FilterAppliedEventArgs : EventArgs
    {
        public List<object> SelectedValues { get; set; }
        public string SearchText { get; set; }
    }
}
