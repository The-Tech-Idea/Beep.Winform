using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules; // for SortDirection

namespace TheTechIdea.Beep.Winform.Controls.GridX.Filters
{
    // Lightweight popup similar to Excel filter menu
    internal class BeepGridFilterPopup : Form
    {
        private readonly string _columnCaption;
        private readonly List<object> _values;

        // UI
        private Button btnSortAsc;
        private Button btnSortDesc;
        private Button btnClear;
        private TextBox txtSearch;
        private CheckBox chkSelectAll;
        private CheckedListBox clbValues;
        private Button btnApply;
        private Button btnCancel;

        public event EventHandler<SortDirection> SortRequested;
        public event EventHandler ClearRequested;
        public event EventHandler<FilterAppliedEventArgs> FilterApplied;

        public BeepGridFilterPopup(string columnCaption, IEnumerable<object> values)
        {
            _columnCaption = columnCaption;
            _values = values?.Select(v => v ?? string.Empty).Distinct().ToList() ?? new List<object>();

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            Size = new Size(260, 320);
            Padding = new Padding(6);
            BackColor = SystemColors.Window;

            BuildUI();
        }

        private void BuildUI()
        {
            btnSortAsc = new Button { Text = "Sort Smallest to Largest", Dock = DockStyle.Top, Height = 28, FlatStyle = FlatStyle.Flat };
            btnSortAsc.Click += (s, e) => SortRequested?.Invoke(this, SortDirection.Ascending);
            btnSortDesc = new Button { Text = "Sort Largest to Smallest", Dock = DockStyle.Top, Height = 28, FlatStyle = FlatStyle.Flat };
            btnSortDesc.Click += (s, e) => SortRequested?.Invoke(this, SortDirection.Descending);
            btnClear = new Button { Text = "Clear Filter", Dock = DockStyle.Top, Height = 28, FlatStyle = FlatStyle.Flat };
            btnClear.Click += (s, e) => ClearRequested?.Invoke(this, EventArgs.Empty);

            var lblFilter = new Label { Text = _columnCaption, Dock = DockStyle.Top, Height = 22, Padding = new Padding(2) };

            txtSearch = new TextBox { Dock = DockStyle.Top, Height = 24, PlaceholderText = "Search..." };
            txtSearch.TextChanged += (s, e) => ApplySearch();

            chkSelectAll = new CheckBox { Text = "Select All", Dock = DockStyle.Top, Height = 22, Checked = true };
            chkSelectAll.CheckedChanged += (s, e) => ToggleAll(chkSelectAll.Checked);

            clbValues = new CheckedListBox { Dock = DockStyle.Fill, CheckOnClick = true, IntegralHeight = false };

            btnApply = new Button { Text = "Apply", Dock = DockStyle.Bottom, Height = 28, FlatStyle = FlatStyle.Flat };
            btnApply.Click += (s, e) => RaiseApply();
            btnCancel = new Button { Text = "Cancel", Dock = DockStyle.Bottom, Height = 28, FlatStyle = FlatStyle.Flat };
            btnCancel.Click += (s, e) => Close();

            Controls.Add(clbValues);
            Controls.Add(chkSelectAll);
            Controls.Add(txtSearch);
            Controls.Add(lblFilter);
            Controls.Add(btnClear);
            Controls.Add(btnSortDesc);
            Controls.Add(btnSortAsc);
            Controls.Add(btnApply);
            Controls.Add(btnCancel);

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
    }

    internal class FilterAppliedEventArgs : EventArgs
    {
        public List<object> SelectedValues { get; set; }
        public string SearchText { get; set; }
    }
}
