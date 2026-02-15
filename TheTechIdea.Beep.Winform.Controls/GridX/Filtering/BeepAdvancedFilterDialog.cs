using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Filtering;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Filtering
{
    /// <summary>
    /// Data model for a single filter row displayed in the grid
    /// </summary>
    public class FilterRowData : INotifyPropertyChanged
    {
        private string _columnName = string.Empty;
        private string _operator = "Contains";
        private string _value = string.Empty;
        private string _value2 = string.Empty;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>Gets or sets the column name to filter on</summary>
        public string ColumnName
        {
            get => _columnName;
            set { _columnName = value ?? string.Empty; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ColumnName))); }
        }

        /// <summary>Gets or sets the filter operator name</summary>
        public string Operator
        {
            get => _operator;
            set { _operator = value ?? "Contains"; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Operator))); }
        }

        /// <summary>Gets or sets the primary filter value</summary>
        public string Value
        {
            get => _value;
            set { _value = value ?? string.Empty; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value))); }
        }

        /// <summary>Gets or sets the secondary filter value (for Between operators)</summary>
        public string Value2
        {
            get => _value2;
            set { _value2 = value ?? string.Empty; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value2))); }
        }
    }

    /// <summary>
    /// Modern advanced filter dialog using BeepiFormPro with BeepGridPro for filter rows
    /// </summary>
    public class BeepAdvancedFilterDialog : BeepiFormPro
    {
        private Panel _buttonPanel = null!;
        private Panel _logicPanel = null!;
        private BeepGridPro _filterGrid = null!;
        private BeepButton _addFilterButton = null!;
        private BeepButton _removeFilterButton = null!;
        private BeepButton _applyButton = null!;
        private BeepButton _clearButton = null!;
        private BeepButton _cancelButton = null!;
        private BeepButton _saveConfigButton = null!;
        private BeepButton _loadConfigButton = null!;
        private BeepComboBox _logicCombo = null!;
        private BeepLabel _logicLabel = null!;

        private BindingList<FilterRowData> _filterData = new BindingList<FilterRowData>();
        private Dictionary<string, Type> _columnTypes = null!;
        private string[] _columnNames = Array.Empty<string>();
        private FilterConfiguration? _result;

        /// <summary>
        /// Gets the filter configuration result after dialog is closed with OK
        /// </summary>
        public FilterConfiguration? Result => _result;

        /// <summary>
        /// Initializes a new instance of the BeepAdvancedFilterDialog class
        /// </summary>
        public BeepAdvancedFilterDialog(BeepGridColumnConfigCollection columns, FilterConfiguration? existingConfig = null, string? themeName = null)
        {
            if (!string.IsNullOrWhiteSpace(themeName))
            {
                Theme = themeName;
            }

            UseThemeColors = true;
            InitializeDialog();
            LoadColumns(columns);
            ConfigureFilterGrid();

            if (existingConfig != null)
            {
                LoadConfiguration(existingConfig);
            }
            else
            {
                // Add one empty row by default
                AddFilterRow();
            }

            ApplyTheme();
        }

        private void InitializeDialog()
        {
            Text = "Advanced Filter";
            Size = new Size(900, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormStyle = FormStyle.Modern;
            ShowCaptionBar = true;
            ShowMinMaxButtons = false;
            ShowCloseButton = true;
            MaximizeBox = true;
            MinimizeBox = false;

            // Logic selector panel (AND/OR) - top
            _logicPanel = new Panel
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

            _logicPanel.Controls.Add(_logicLabel);
            _logicPanel.Controls.Add(_logicCombo);

            // Button panel - bottom
            _buttonPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Bottom,
                Padding = new Padding(5)
            };

            int bx = 10;
            _addFilterButton = CreateButton("+ Add", ref bx);
            _addFilterButton.Click += (s, e) => AddFilterRow();

            _removeFilterButton = CreateButton("- Remove", ref bx);
            _removeFilterButton.Click += (s, e) => RemoveSelectedFilterRow();

            _clearButton = CreateButton("Clear All", ref bx);
            _clearButton.Click += (s, e) => ClearAllFilters();

            _saveConfigButton = CreateButton("Save", ref bx);
            _saveConfigButton.Click += (s, e) => SaveConfiguration();

            _loadConfigButton = CreateButton("Load", ref bx);
            _loadConfigButton.Click += (s, e) => LoadConfigurationFromFile();

            // Spacer
            bx = 650;
            _applyButton = CreateButton("Apply", ref bx);
            _applyButton.Click += (s, e) => ApplyFilters();

            _cancelButton = CreateButton("Cancel", ref bx);
            _cancelButton.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            _buttonPanel.Controls.Add(_addFilterButton);
            _buttonPanel.Controls.Add(_removeFilterButton);
            _buttonPanel.Controls.Add(_clearButton);
            _buttonPanel.Controls.Add(_saveConfigButton);
            _buttonPanel.Controls.Add(_loadConfigButton);
            _buttonPanel.Controls.Add(_applyButton);
            _buttonPanel.Controls.Add(_cancelButton);

            // Filter grid - fill
            _filterGrid = new BeepGridPro
            {
                Dock = DockStyle.Fill,
                ShowNavigator = false,
                ShowTopFilterPanel = false,
                ShowCheckBox = false,
                ShowColumnHeaders = true,
                ReadOnly = false,
                AllowColumnReorder = false,
                AllowUserToResizeRows = false,
                IsChild = true,
                IsFrameless = true,
            };

            // Add controls in correct order for Dock layout
            Controls.Add(_filterGrid);
            Controls.Add(_buttonPanel);
            Controls.Add(_logicPanel);
        }

        private BeepButton CreateButton(string text, ref int x)
        {
            var btn = new BeepButton
            {
                Text = text,
                Width = 90,
                Height = 35,
                Location = new Point(x, 8),
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true
            };
            x += 100;
            return btn;
        }

        private void LoadColumns(BeepGridColumnConfigCollection columns)
        {
            _columnTypes = new Dictionary<string, Type>();

            foreach (var col in columns.Where(c => c.Visible && !c.IsSelectionCheckBox && !c.IsRowID && !c.IsRowNumColumn))
            {
                _columnTypes[col.ColumnName] = col.DataType ?? typeof(string);
            }

            _columnNames = _columnTypes.Keys.ToArray();
        }

        private void ConfigureFilterGrid()
        {
            // Build operator items (all operators for display)
            var allOperators = Enum.GetValues(typeof(FilterOperator)).Cast<FilterOperator>().ToArray();

            // Bind data
            _filterGrid.DataSource = _filterData;

            // Configure columns after binding
            _filterGrid.BeginInvoke(new Action(() =>
            {
                try
                {
                    ConfigureGridColumns(allOperators);
                }
                catch { }
            }));
        }

        private void ConfigureGridColumns(FilterOperator[] allOperators)
        {
            foreach (var col in _filterGrid.Data.Columns)
            {
                if (col.IsSelectionCheckBox || col.IsRowNumColumn || col.IsRowID)
                    continue;

                switch (col.ColumnName)
                {
                    case "ColumnName":
                        col.ColumnCaption = "Column";
                        col.CellEditor = BeepColumnType.ComboBox;
                        col.Width = 200;
                        col.Items = _columnNames.Select(n => new SimpleItem { Name = n, Text = n, DisplayField = n, Value = n }).ToList();
                        break;

                    case "Operator":
                        col.ColumnCaption = "Operator";
                        col.CellEditor = BeepColumnType.ComboBox;
                        col.Width = 200;
                        col.Items = allOperators.Select(op => new SimpleItem
                        {
                            Name = op.ToString(),
                            Text = $"{op.GetSymbol()} {op.GetDisplayName()}",
                            DisplayField = $"{op.GetSymbol()} {op.GetDisplayName()}",
                            Value = op.ToString()
                        }).ToList();
                        break;

                    case "Value":
                        col.ColumnCaption = "Value";
                        col.CellEditor = BeepColumnType.Text;
                        col.Width = 200;
                        break;

                    case "Value2":
                        col.ColumnCaption = "Value 2 (Between)";
                        col.CellEditor = BeepColumnType.Text;
                        col.Width = 180;
                        break;
                }
            }

            _filterGrid.Layout.Recalculate();
            _filterGrid.Invalidate();
        }

        private void AddFilterRow()
        {
            var defaultColumn = _columnNames.Length > 0 ? _columnNames[0] : string.Empty;
            _filterData.Add(new FilterRowData
            {
                ColumnName = defaultColumn,
                Operator = FilterOperator.Contains.ToString(),
                Value = string.Empty,
                Value2 = string.Empty
            });
        }

        private void RemoveSelectedFilterRow()
        {
            var selectedIndices = _filterGrid.SelectedRowIndices;
            if (selectedIndices.Count > 0)
            {
                // Remove from highest index to lowest to avoid index shifting
                foreach (var idx in selectedIndices.OrderByDescending(i => i))
                {
                    if (idx >= 0 && idx < _filterData.Count)
                    {
                        _filterData.RemoveAt(idx);
                    }
                }
            }
            else if (_filterData.Count > 0)
            {
                // Remove last row if nothing selected
                _filterData.RemoveAt(_filterData.Count - 1);
            }
        }

        private void ClearAllFilters()
        {
            _filterData.Clear();
            AddFilterRow();
        }

        private void ApplyFilters()
        {
            _result = new FilterConfiguration("Custom Filter")
            {
                Logic = _logicCombo.SelectedIndex == 0 ? FilterLogic.And : FilterLogic.Or,
                IsActive = true
            };

            foreach (var rowData in _filterData)
            {
                if (string.IsNullOrWhiteSpace(rowData.ColumnName))
                    continue;

                if (!Enum.TryParse<FilterOperator>(rowData.Operator, out var op))
                    op = FilterOperator.Contains;

                var criteria = (op == FilterOperator.Between || op == FilterOperator.NotBetween)
                    ? new FilterCriteria(rowData.ColumnName, op, rowData.Value, rowData.Value2)
                    : new FilterCriteria(rowData.ColumnName, op, rowData.Value);

                _result.AddCriteria(criteria);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void LoadConfiguration(FilterConfiguration config)
        {
            _filterData.Clear();

            _logicCombo.SelectedIndex = config.Logic == FilterLogic.And ? 0 : 1;

            foreach (var criterion in config.Criteria)
            {
                _filterData.Add(new FilterRowData
                {
                    ColumnName = criterion.ColumnName,
                    Operator = criterion.Operator.ToString(),
                    Value = criterion.Value?.ToString() ?? string.Empty,
                    Value2 = criterion.Value2?.ToString() ?? string.Empty
                });
            }

            if (_filterData.Count == 0)
            {
                AddFilterRow();
            }
        }

        /// <inheritdoc/>
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            var theme = CurrentTheme ?? BeepThemesManager.CurrentTheme ?? BeepThemesManager.GetDefaultTheme();
            if (theme == null)
                return;

            BackColor = theme.BackColor != Color.Empty ? theme.BackColor : SystemColors.Control;
            ForeColor = theme.ForeColor != Color.Empty ? theme.ForeColor : SystemColors.ControlText;

            var surfaceBack = theme.PanelBackColor != Color.Empty
                ? theme.PanelBackColor
                : (theme.BackColor != Color.Empty ? theme.BackColor : SystemColors.Control);
            var surfaceFore = theme.ForeColor != Color.Empty ? theme.ForeColor : SystemColors.ControlText;

            if (_logicPanel != null)
            {
                _logicPanel.BackColor = surfaceBack;
                _logicPanel.ForeColor = surfaceFore;
            }
            if (_buttonPanel != null)
            {
                _buttonPanel.BackColor = surfaceBack;
                _buttonPanel.ForeColor = surfaceFore;
            }

            // Propagate theme to BeepGridPro and all child controls
            if (_filterGrid != null)
            {
                _filterGrid.Theme = Theme;
                _filterGrid.ApplyTheme();
            }

            // Propagate theme to all Beep child controls (buttons, combos, labels)
            PropagateThemeToChildren(this);
        }

        private void PropagateThemeToChildren(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is BaseControl bc)
                {
                    bc.Theme = Theme;
                    bc.ApplyTheme();
                }
                else if (child is IBeepUIComponent ui)
                {
                    ui.Theme = Theme;
                }

                if (child.HasChildren)
                {
                    PropagateThemeToChildren(child);
                }
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

                    foreach (var rowData in _filterData)
                    {
                        if (string.IsNullOrWhiteSpace(rowData.ColumnName))
                            continue;

                        if (!Enum.TryParse<FilterOperator>(rowData.Operator, out var op))
                            op = FilterOperator.Contains;

                        var criteria = (op == FilterOperator.Between || op == FilterOperator.NotBetween)
                            ? new FilterCriteria(rowData.ColumnName, op, rowData.Value, rowData.Value2)
                            : new FilterCriteria(rowData.ColumnName, op, rowData.Value);

                        config.AddCriteria(criteria);
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
                _filterGrid?.Dispose();
                _buttonPanel?.Dispose();
                _logicPanel?.Dispose();
                _addFilterButton?.Dispose();
                _removeFilterButton?.Dispose();
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
