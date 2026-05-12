using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Editors
{
    /// <summary>
    /// A popup control for filtering tree column values.
    /// Displays unique values with checkboxes for inclusion/exclusion.
    /// </summary>
    public class BeepTreeFilterPopup : Form
    {
        private CheckedListBox _checkedListBox;
        private TextBox _searchBox;
        private Button _okButton;
        private Button _cancelButton;
        private Button _selectAllButton;
        private Button _clearButton;
        private BeepTreeColumn _column;
        private List<object> _allValues;
        private List<object> _selectedValues;

        public List<object> SelectedValues => _selectedValues;
        public bool FilterApplied { get; private set; }

        public BeepTreeFilterPopup(BeepTreeColumn column, IEnumerable<object> values)
        {
            _column = column;
            _allValues = values?.Distinct().ToList() ?? new List<object>();
            _selectedValues = new List<object>(_allValues);

            InitializeComponent();
            PopulateValues();
        }

        private void InitializeComponent()
        {
            this.Text = $"Filter - {_column?.DisplayText ?? "Column"}";
            this.Size = new Size(300, 400);
            this.StartPosition = FormStartPosition.Manual;
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new Size(250, 300);

            // Search box
            _searchBox = new TextBox
            {
                Dock = DockStyle.Top,
                PlaceholderText = "Search...",
                Margin = new Padding(4)
            };
            _searchBox.TextChanged += SearchBox_TextChanged;
            this.Controls.Add(_searchBox);

            // Checked list box
            _checkedListBox = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                CheckOnClick = true,
                Sorted = true
            };
            _checkedListBox.ItemCheck += CheckedListBox_ItemCheck;
            this.Controls.Add(_checkedListBox);

            // Button panel
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40
            };

            _selectAllButton = new Button
            {
                Text = "All",
                Width = 50,
                Height = 24,
                Left = 4,
                Top = 8
            };
            _selectAllButton.Click += (s, e) => SetAllChecked(true);
            buttonPanel.Controls.Add(_selectAllButton);

            _clearButton = new Button
            {
                Text = "None",
                Width = 50,
                Height = 24,
                Left = 58,
                Top = 8
            };
            _clearButton.Click += (s, e) => SetAllChecked(false);
            buttonPanel.Controls.Add(_clearButton);

            _okButton = new Button
            {
                Text = "OK",
                Width = 60,
                Height = 24,
                Left = 170,
                Top = 8,
                DialogResult = DialogResult.OK
            };
            _okButton.Click += OkButton_Click;
            buttonPanel.Controls.Add(_okButton);

            _cancelButton = new Button
            {
                Text = "Cancel",
                Width = 60,
                Height = 24,
                Left = 234,
                Top = 8,
                DialogResult = DialogResult.Cancel
            };
            buttonPanel.Controls.Add(_cancelButton);

            this.Controls.Add(buttonPanel);

            this.AcceptButton = _okButton;
            this.CancelButton = _cancelButton;
        }

        private void PopulateValues()
        {
            _checkedListBox.Items.Clear();
            foreach (var value in _allValues)
            {
                bool isChecked = _selectedValues.Contains(value);
                _checkedListBox.Items.Add(value ?? "(Blank)", isChecked);
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = _searchBox.Text?.ToLowerInvariant() ?? string.Empty;
            _checkedListBox.Items.Clear();

            foreach (var value in _allValues)
            {
                string valueText = value?.ToString()?.ToLowerInvariant() ?? string.Empty;
                if (string.IsNullOrEmpty(searchText) || valueText.Contains(searchText))
                {
                    bool isChecked = _selectedValues.Contains(value);
                    _checkedListBox.Items.Add(value ?? "(Blank)", isChecked);
                }
            }
        }

        private void CheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var item = _checkedListBox.Items[e.Index];
            if (e.NewValue == CheckState.Checked)
            {
                if (!_selectedValues.Contains(item))
                    _selectedValues.Add(item);
            }
            else
            {
                _selectedValues.Remove(item);
            }
        }

        private void SetAllChecked(bool check)
        {
            for (int i = 0; i < _checkedListBox.Items.Count; i++)
            {
                _checkedListBox.SetItemChecked(i, check);
            }

            if (check)
            {
                _selectedValues = _allValues.ToList();
            }
            else
            {
                _selectedValues.Clear();
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            FilterApplied = _selectedValues.Count != _allValues.Count;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
