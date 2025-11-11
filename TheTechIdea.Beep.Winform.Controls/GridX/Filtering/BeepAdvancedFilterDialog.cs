using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Filtering;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Filtering
{
    /// <summary>
    /// Modern advanced filter dialog using BeepiFormPro
    /// Allows multi-column filtering with various operators
    /// </summary>
    public class BeepAdvancedFilterDialog : BeepiFormPro
    {
        private Panel _mainPanel = null!;
        private FlowLayoutPanel _filterRowsPanel = null!;
        private BeepButton _addFilterButton = null!;
        private BeepButton _applyButton = null!;
        private BeepButton _clearButton = null!;
        private BeepButton _cancelButton = null!;
        private BeepButton _saveConfigButton = null!;
        private BeepButton _loadConfigButton = null!;
        private BeepComboBox _logicCombo = null!;
        private BeepLabel _logicLabel = null!;

        private List<BeepFilterRow> _filterRows = new List<BeepFilterRow>();
        private Dictionary<string, Type> _columnTypes = null!;
        private FilterConfiguration? _result;

        /// <summary>
        /// Gets the filter configuration result after dialog is closed with OK
        /// </summary>
        public FilterConfiguration? Result => _result;

        /// <summary>
        /// Initializes a new instance of the BeepAdvancedFilterDialog class
        /// </summary>
        /// <param name="columns">Column collection to filter</param>
        /// <param name="existingConfig">Optional existing filter configuration to load</param>
        public BeepAdvancedFilterDialog(BeepGridColumnConfigCollection columns, FilterConfiguration? existingConfig = null)
        {
            InitializeDialog();
            LoadColumns(columns);
            
            if (existingConfig != null)
            {
                LoadConfiguration(existingConfig);
            }
            else
            {
                AddFilterRow(); // Add one empty row by default
            }
        }

        private void InitializeDialog()
        {
            // Form settings
            Text = "Advanced Filter";
            Size = new Size(900, 600);
            StartPosition = FormStartPosition.CenterParent;
            FormStyle = FormStyle.Modern;
            ShowCaptionBar = true;
            ShowMinMaxButtons = false;
     
            ShowCloseButton = true;
            MaximizeBox = true;
            MinimizeBox = false;

            // Main panel
            _mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10)
            };

            // Logic selector panel (AND/OR)
            var logicPanel = new Panel
            {
                Height = 40,
                Dock = DockStyle.Top,
                Padding = new Padding(5)
            };

            _logicLabel = new BeepLabel
            {
                Text = "Combine filters using:",
                AutoSize = true,
                Location = new Point(10, 10),
                IsChild = true,
                IsFrameless = true
            };

            _logicCombo = new BeepComboBox
            {
                Width = 100,
                Height = 30,
                Location = new Point(150, 7),
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true
            };
            _logicCombo.Items.Add("AND");
            _logicCombo.Items.Add("OR");
            _logicCombo.SelectedIndex = 0;

            logicPanel.Controls.Add(_logicLabel);
            logicPanel.Controls.Add(_logicCombo);

            // Filter rows panel
            _filterRowsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(10)
            };

            // Button panel
            var buttonPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Bottom,
                Padding = new Padding(10)
            };

            _addFilterButton = new BeepButton
            {
                Text = "+ Add Filter",
                Width = 120,
                Height = 35,
                Location = new Point(10, 12),
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true
            };
            _addFilterButton.Click += (s, e) => AddFilterRow();

            _clearButton = new BeepButton
            {
                Text = "Clear All",
                Width = 100,
                Height = 35,
                Location = new Point(140, 12),
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true
            };
            _clearButton.Click += (s, e) => ClearAllFilters();

            _saveConfigButton = new BeepButton
            {
                Text = "💾 Save",
                Width = 100,
                Height = 35,
                Location = new Point(250, 12),
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true
            };
            _saveConfigButton.Click += (s, e) => SaveConfiguration();

            _loadConfigButton = new BeepButton
            {
                Text = "📂 Load",
                Width = 100,
                Height = 35,
                Location = new Point(360, 12),
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true
            };
            _loadConfigButton.Click += (s, e) => LoadConfigurationFromFile();

            _applyButton = new BeepButton
            {
                Text = "Apply",
                Width = 100,
                Height = 35,
                Location = new Point(580, 12),
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true
            };
            _applyButton.Click += (s, e) => ApplyFilters();

            _cancelButton = new BeepButton
            {
                Text = "Cancel",
                Width = 100,
                Height = 35,
                Location = new Point(690, 12),
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true
            };
            _cancelButton.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            buttonPanel.Controls.Add(_addFilterButton);
            buttonPanel.Controls.Add(_clearButton);
            buttonPanel.Controls.Add(_saveConfigButton);
            buttonPanel.Controls.Add(_loadConfigButton);
            buttonPanel.Controls.Add(_applyButton);
            buttonPanel.Controls.Add(_cancelButton);

            _mainPanel.Controls.Add(_filterRowsPanel);
            _mainPanel.Controls.Add(logicPanel);
            _mainPanel.Controls.Add(buttonPanel);

            Controls.Add(_mainPanel);
        }

        private void LoadColumns(BeepGridColumnConfigCollection columns)
        {
            _columnTypes = new Dictionary<string, Type>();
            
            foreach (var col in columns.Where(c => c.Visible && !c.IsSelectionCheckBox && !c.IsRowID && !c.IsRowNumColumn  ))
            {
                _columnTypes[col.ColumnName] = col.DataType ?? typeof(string);
            }
        }

        private void AddFilterRow()
        {
            var row = new BeepFilterRow
            {
                AvailableColumns = _columnTypes.Keys.ToArray(),
                Margin = new Padding(5),
                IsChild = true
            };

            row.RemoveRequested += (s, e) => RemoveFilterRow(row);
            
            // Set column type when column is selected
            row.FilterChanged += (s, e) =>
            {
                var selectedCol = row.SelectedColumn;
                if (!string.IsNullOrEmpty(selectedCol) && _columnTypes.ContainsKey(selectedCol))
                {
                    row.SetColumnType(_columnTypes[selectedCol]);
                }
            };

            _filterRows.Add(row);
            _filterRowsPanel.Controls.Add(row);

            // Set default column if available
            if (_columnTypes.Count > 0)
            {
                row.SelectedColumn = _columnTypes.Keys.First();
                row.SetColumnType(_columnTypes[row.SelectedColumn]);
            }
        }

        private void RemoveFilterRow(BeepFilterRow row)
        {
            _filterRows.Remove(row);
            _filterRowsPanel.Controls.Remove(row);
            row.Dispose();
        }

        private void ClearAllFilters()
        {
            foreach (var row in _filterRows.ToList())
            {
                _filterRowsPanel.Controls.Remove(row);
                row.Dispose();
            }
            _filterRows.Clear();
            
            // Add one empty row
            AddFilterRow();
        }

        private void ApplyFilters()
        {
            _result = new FilterConfiguration("Custom Filter")
            {
                Logic = _logicCombo.SelectedIndex == 0 ? FilterLogic.And : FilterLogic.Or,
                IsActive = true
            };

            foreach (var row in _filterRows)
            {
                var criteria = row.GetFilterCriteria();
                if (criteria != null)
                {
                    _result.AddCriteria(criteria);
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void LoadConfiguration(FilterConfiguration config)
        {
            // Clear existing rows
            ClearAllFilters();
            _filterRows.Clear();
            _filterRowsPanel.Controls.Clear();

            // Set logic
            _logicCombo.SelectedIndex = config.Logic == FilterLogic.And ? 0 : 1;

            // Add rows for each criterion
            foreach (var criterion in config.Criteria)
            {
                var row = new BeepFilterRow
                {
                    AvailableColumns = _columnTypes.Keys.ToArray(),
                    Margin = new Padding(5),
                    IsChild = true
                };

                row.RemoveRequested += (s, e) => RemoveFilterRow(row);
                row.SetFilterCriteria(criterion);
                
                if (_columnTypes.ContainsKey(criterion.ColumnName))
                {
                    row.SetColumnType(_columnTypes[criterion.ColumnName]);
                }

                _filterRows.Add(row);
                _filterRowsPanel.Controls.Add(row);
            }
        }

        private void SaveConfiguration()
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Filter Configuration|*.json";
                dialog.Title = "Save Filter Configuration";
                dialog.FileName = "filter_config.json";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var config = new FilterConfiguration("Saved Filter")
                    {
                        Logic = _logicCombo.SelectedIndex == 0 ? FilterLogic.And : FilterLogic.Or
                    };

                    foreach (var row in _filterRows)
                    {
                        var criteria = row.GetFilterCriteria();
                        if (criteria != null)
                        {
                            config.AddCriteria(criteria);
                        }
                    }

                    var json = System.Text.Json.JsonSerializer.Serialize(config, new System.Text.Json.JsonSerializerOptions 
                    { 
                        WriteIndented = true 
                    });
                    System.IO.File.WriteAllText(dialog.FileName, json);

                    MessageBox.Show("Filter configuration saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void LoadConfigurationFromFile()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Filter Configuration|*.json";
                dialog.Title = "Load Filter Configuration";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var json = System.IO.File.ReadAllText(dialog.FileName);
                        var config = System.Text.Json.JsonSerializer.Deserialize<FilterConfiguration>(json);
                        
                        if (config != null)
                        {
                            LoadConfiguration(config);
                            MessageBox.Show("Filter configuration loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var row in _filterRows)
                {
                    row.Dispose();
                }
                _filterRows.Clear();

                _mainPanel?.Dispose();
                _filterRowsPanel?.Dispose();
                _addFilterButton?.Dispose();
                _applyButton?.Dispose();
                _clearButton?.Dispose();
                _cancelButton?.Dispose();
                _saveConfigButton?.Dispose();
                _loadConfigButton?.Dispose();
                _logicCombo?.Dispose();
                _logicLabel?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
