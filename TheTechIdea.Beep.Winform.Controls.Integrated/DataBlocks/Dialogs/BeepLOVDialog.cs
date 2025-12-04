using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Dialogs
{
    /// <summary>
    /// LOV (List of Values) dialog for BeepDataBlock
    /// Oracle Forms-compatible popup for value selection
    /// </summary>
    public class BeepLOVDialog : Form
    {
        #region Fields
        
        private readonly BeepDataBlockLOV _lov;
        private readonly List<object> _data;
        private DataGridView _grid;
        private BeepTextBox _searchBox;
        private Button _okButton;
        private Button _cancelButton;
        private BeepLabel _statusLabel;
        private List<object> _filteredData;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Selected value (return field value)
        /// </summary>
        public object SelectedValue { get; private set; }
        
        /// <summary>
        /// Selected record (full object)
        /// </summary>
        public object SelectedRecord { get; private set; }
        
        /// <summary>
        /// Selected records (for multi-select)
        /// </summary>
        public List<object> SelectedRecords { get; private set; } = new List<object>();
        
        /// <summary>
        /// Initial value to select when LOV opens
        /// </summary>
        public object InitialValue { get; set; }
        
        #endregion
        
        #region Constructor
        
        public BeepLOVDialog(BeepDataBlockLOV lov, List<object> data)
        {
            _lov = lov ?? throw new ArgumentNullException(nameof(lov));
            _data = data ?? new List<object>();
            _filteredData = new List<object>(_data);
            
            InitializeDialog();
            InitializeComponents();
            LoadData();
            SelectInitialValue();
        }
        
        #endregion
        
        #region Initialization
        
        private void InitializeDialog()
        {
            // Form properties
            Text = _lov.Title ?? _lov.LOVName ?? "Select Value";
            Size = new Size(_lov.Width, _lov.Height);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(400, 300);
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
        }
        
        private void InitializeComponents()
        {
            int padding = 12;
            int buttonHeight = 36;
            int searchBoxHeight = 32;
            int statusHeight = 24;
            
            // Search box at top
            _searchBox = new BeepTextBox
            {
                Location = new Point(padding, padding),
                Size = new Size(ClientSize.Width - (padding * 2), searchBoxHeight),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                PlaceholderText = _lov.AllowSearch ? "Type to search..." : "Search disabled"
            };
            
            if (_lov.AllowSearch)
            {
                _searchBox.TextChanged += SearchBox_TextChanged;
            }
            else
            {
                _searchBox.ReadOnly = true;
            }
            
            // Grid for LOV data
            int gridTop = _searchBox.Bottom + padding;
            int gridBottom = ClientSize.Height - buttonHeight - statusHeight - (padding * 3);
            
            _grid = new DataGridView
            {
                Location = new Point(padding, gridTop),
                Size = new Size(ClientSize.Width - (padding * 2), gridBottom - gridTop),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                MultiSelect = _lov.AllowMultiSelect,
                SelectionMode = _lov.AllowMultiSelect 
                    ? DataGridViewSelectionMode.FullRowSelect 
                    : DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = _lov.ShowRowNumbers,
                AutoGenerateColumns = true,
                AllowUserToResizeColumns = true,
                AllowUserToResizeRows = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            };
            
            _grid.DoubleClick += Grid_DoubleClick;
            _grid.KeyDown += Grid_KeyDown;
            
            // Status label
            _statusLabel = new BeepLabel
            {
                Location = new Point(padding, _grid.Bottom + padding),
                Size = new Size(ClientSize.Width - (padding * 2), statusHeight),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleLeft
            };
            UpdateStatusLabel();
            
            // OK button
            int buttonY = _statusLabel.Bottom + padding;
            int buttonWidth = 90;
            
            _okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(ClientSize.Width - buttonWidth - padding - buttonWidth - padding, buttonY),
                Size = new Size(buttonWidth, buttonHeight),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat
            };
            _okButton.Click += OkButton_Click;
            
            // Cancel button
            _cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(ClientSize.Width - buttonWidth - padding, buttonY),
                Size = new Size(buttonWidth, buttonHeight),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat
            };
            
            // Add controls
            Controls.AddRange(new Control[] 
            { 
                _searchBox, 
                _grid, 
                _statusLabel, 
                _okButton, 
                _cancelButton 
            });
            
            AcceptButton = _okButton;
            CancelButton = _cancelButton;
        }
        
        #endregion
        
        #region Data Loading
        
        private void LoadData()
        {
            if (_filteredData == null || _filteredData.Count == 0)
            {
                _grid.DataSource = null;
                UpdateStatusLabel();
                return;
            }
            
            _grid.DataSource = _filteredData;
            
            // Configure columns based on LOV definition
            ConfigureColumns();
            
            UpdateStatusLabel();
        }
        
        private void ConfigureColumns()
        {
            if (_lov.Columns == null || _lov.Columns.Count == 0)
            {
                // Auto-size all columns
                if (_lov.AutoSizeColumns)
                {
                    _grid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                }
                return;
            }
            
            // Configure columns based on LOV definition
            foreach (var lovColumn in _lov.Columns)
            {
                if (_grid.Columns.Contains(lovColumn.FieldName))
                {
                    var gridColumn = _grid.Columns[lovColumn.FieldName];
                    
                    gridColumn.HeaderText = lovColumn.DisplayName ?? lovColumn.FieldName;
                    gridColumn.Width = lovColumn.Width > 0 ? lovColumn.Width : 100;
                    gridColumn.Visible = lovColumn.Visible;
                    
                    // Apply format if specified
                    if (!string.IsNullOrEmpty(lovColumn.Format))
                    {
                        gridColumn.DefaultCellStyle.Format = lovColumn.Format;
                    }
                    
                    // Apply alignment
                    gridColumn.DefaultCellStyle.Alignment = lovColumn.Alignment switch
                    {
                        LOVColumnAlignment.Center => DataGridViewContentAlignment.MiddleCenter,
                        LOVColumnAlignment.Right => DataGridViewContentAlignment.MiddleRight,
                        _ => DataGridViewContentAlignment.MiddleLeft
                    };
                }
            }
            
            // Hide columns not in LOV definition
            if (_lov.Columns.Count > 0)
            {
                foreach (DataGridViewColumn column in _grid.Columns)
                {
                    if (!_lov.Columns.Any(c => c.FieldName == column.Name))
                    {
                        column.Visible = false;
                    }
                }
            }
        }
        
        private void SelectInitialValue()
        {
            if (InitialValue == null || _grid.Rows.Count == 0)
                return;
                
            var initialValueString = InitialValue.ToString();
            
            // Find row with matching value
            foreach (DataGridViewRow row in _grid.Rows)
            {
                var dataItem = row.DataBoundItem;
                if (dataItem != null)
                {
                    var property = dataItem.GetType().GetProperty(_lov.ReturnField);
                    if (property != null)
                    {
                        var value = property.GetValue(dataItem)?.ToString();
                        if (string.Equals(value, initialValueString, StringComparison.OrdinalIgnoreCase))
                        {
                            row.Selected = true;
                            _grid.FirstDisplayedScrollingRowIndex = row.Index;
                            break;
                        }
                    }
                }
            }
        }
        
        #endregion
        
        #region Search & Filter
        
        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            var searchText = _searchBox.Text?.Trim();
            
            if (string.IsNullOrEmpty(searchText))
            {
                // No search text → Show all data
                _filteredData = new List<object>(_data);
            }
            else
            {
                // Filter data based on search mode
                _filteredData = FilterData(searchText);
            }
            
            LoadData();
        }
        
        private List<object> FilterData(string searchText)
        {
            if (_data == null || _data.Count == 0)
                return new List<object>();
                
            // Get searchable columns
            var searchableColumns = _lov.Columns
                .Where(c => c.Searchable)
                .Select(c => c.FieldName)
                .ToList();
                
            // If no columns specified, search all string properties
            if (searchableColumns.Count == 0)
            {
                var firstRecord = _data.FirstOrDefault();
                if (firstRecord != null)
                {
                    searchableColumns = firstRecord.GetType()
                        .GetProperties()
                        .Where(p => p.PropertyType == typeof(string))
                        .Select(p => p.Name)
                        .ToList();
                }
            }
            
            // Filter based on search mode
            return _data.Where(record =>
            {
                foreach (var columnName in searchableColumns)
                {
                    try
                    {
                        var property = record.GetType().GetProperty(columnName);
                        if (property != null)
                        {
                            var value = property.GetValue(record)?.ToString() ?? "";
                            
                            bool matches = _lov.SearchMode switch
                            {
                                LOVSearchMode.StartsWith => value.StartsWith(searchText, StringComparison.OrdinalIgnoreCase),
                                LOVSearchMode.EndsWith => value.EndsWith(searchText, StringComparison.OrdinalIgnoreCase),
                                LOVSearchMode.Exact => value.Equals(searchText, StringComparison.OrdinalIgnoreCase),
                                _ => value.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
                            };
                            
                            if (matches)
                                return true;
                        }
                    }
                    catch
                    {
                        // Ignore property access errors
                    }
                }
                
                return false;
            }).ToList();
        }
        
        #endregion
        
        #region Event Handlers
        
        private void Grid_DoubleClick(object sender, EventArgs e)
        {
            // Double-click on row = OK
            if (_grid.SelectedRows.Count > 0)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
        
        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            // Enter key = OK
            if (e.KeyCode == Keys.Enter && _grid.SelectedRows.Count > 0)
            {
                e.Handled = true;
                DialogResult = DialogResult.OK;
                Close();
            }
            
            // Escape key = Cancel
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }
        
        private void OkButton_Click(object sender, EventArgs e)
        {
            if (_grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a value", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.None;
                return;
            }
        }
        
        #endregion
        
        #region Form Closing
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                ExtractSelectedValues();
                
                if (SelectedValue == null && !_lov.AllowMultiSelect)
                {
                    MessageBox.Show("Please select a value", "Selection Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e.Cancel = true;
                    return;
                }
            }
            
            base.OnFormClosing(e);
        }
        
        private void ExtractSelectedValues()
        {
            if (_lov.AllowMultiSelect)
            {
                // Multi-select: Get all selected rows
                foreach (DataGridViewRow row in _grid.SelectedRows)
                {
                    if (row.DataBoundItem != null)
                    {
                        SelectedRecords.Add(row.DataBoundItem);
                        
                        // Extract return field value
                        var property = row.DataBoundItem.GetType().GetProperty(_lov.ReturnField);
                        if (property != null)
                        {
                            var value = property.GetValue(row.DataBoundItem);
                            if (value != null && !SelectedRecords.Contains(value))
                            {
                                SelectedRecords.Add(value);
                            }
                        }
                    }
                }
                
                // Set SelectedValue to first selected
                if (SelectedRecords.Count > 0)
                {
                    SelectedRecord = _grid.SelectedRows[0].DataBoundItem;
                    var property = SelectedRecord?.GetType().GetProperty(_lov.ReturnField);
                    SelectedValue = property?.GetValue(SelectedRecord);
                }
            }
            else
            {
                // Single select
                if (_grid.SelectedRows.Count > 0)
                {
                    SelectedRecord = _grid.SelectedRows[0].DataBoundItem;
                    
                    if (SelectedRecord != null)
                    {
                        var property = SelectedRecord.GetType().GetProperty(_lov.ReturnField);
                        if (property != null)
                        {
                            SelectedValue = property.GetValue(SelectedRecord);
                        }
                    }
                }
            }
        }
        
        #endregion
        
        #region Status Updates
        
        private void UpdateStatusLabel()
        {
            if (_statusLabel == null)
                return;
                
            var totalCount = _data?.Count ?? 0;
            var filteredCount = _filteredData?.Count ?? 0;
            
            if (totalCount == filteredCount)
            {
                _statusLabel.Text = $"{totalCount} record(s)";
            }
            else
            {
                _statusLabel.Text = $"Showing {filteredCount} of {totalCount} record(s)";
            }
        }
        
        #endregion
        
        #region Theme Support
        
        /// <summary>
        /// Apply theme to LOV dialog
        /// </summary>
        public void ApplyTheme(IBeepTheme theme)
        {
            if (theme == null)
                return;
                
            BackColor = theme.BackColor;
            ForeColor = theme.ForeColor;
            
            if (_searchBox != null)
            {
                _searchBox.BackColor = theme.TextBoxBackColor;
                _searchBox.ForeColor = theme.TextBoxForeColor;
            }
            
            if (_grid != null)
            {
                _grid.BackgroundColor = theme.BackColor;
                _grid.ForeColor = theme.ForeColor;
                _grid.GridColor = theme.BorderColor;
                _grid.DefaultCellStyle.BackColor = theme.BackColor;
                _grid.DefaultCellStyle.ForeColor = theme.ForeColor;
                _grid.DefaultCellStyle.SelectionBackColor = theme.AccentColor;
                _grid.DefaultCellStyle.SelectionForeColor = theme.OnPrimaryColor;
            }
            
            if (_okButton != null)
            {
                _okButton.BackColor = theme.ButtonBackColor;
                _okButton.ForeColor = theme.ButtonForeColor;
            }
            
            if (_cancelButton != null)
            {
                _cancelButton.BackColor = theme.ButtonBackColor;
                _cancelButton.ForeColor = theme.ButtonForeColor;
            }
            
            if (_statusLabel != null)
            {
                _statusLabel.BackColor = theme.BackColor;
                _statusLabel.ForeColor = theme.ForeColor;
            }
        }
        
        #endregion
    }
}

