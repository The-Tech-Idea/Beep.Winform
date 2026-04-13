using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Services;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    internal sealed class BeepFieldControlTypePolicyEditorForm : Form
    {
        private readonly BindingList<PolicyRow> _rows = new();
        private readonly DataGridView _grid = new();
        private readonly Label _pathLabel = new();
        private readonly Label _hintLabel = new();
        private readonly Button _addButton = new();
        private readonly Button _removeButton = new();
        private readonly Button _reloadButton = new();
        private readonly Button _resetButton = new();
        private readonly Button _okButton = new();
        private readonly Button _cancelButton = new();

        public BeepFieldControlTypePolicyEditorForm()
        {
            InitializeUi();
            LoadRows(BeepFieldControlTypeRegistry.GetPolicyRules());
        }

        private void InitializeUi()
        {
            Text = "Field Default Control Policy";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MinimizeBox = false;
            MaximizeBox = false;
            MinimumSize = new Size(1040, 560);
            Size = new Size(1180, 700);

            _pathLabel.AutoEllipsis = true;
            _pathLabel.Location = new Point(12, 12);
            _pathLabel.Size = new Size(1140, 18);
            _pathLabel.Text = $"Policy file: {BeepFieldControlTypeRegistry.DefaultPolicyFilePath}";

            _hintLabel.Location = new Point(12, 36);
            _hintLabel.Size = new Size(1140, 34);
            _hintLabel.Text = "Rules are matched from most specific to least specific. Leave Category, IsCheck, Data Type Pattern, or Editor blank to keep that part open. Use wildcard patterns like nvarchar* or decimal*.";

            _grid.Location = new Point(12, 80);
            _grid.Size = new Size(1140, 540);
            _grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _grid.AutoGenerateColumns = false;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.RowHeadersVisible = false;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.EditMode = DataGridViewEditMode.EditOnEnter;
            _grid.DataError += (_, _) => { };
            _grid.CurrentCellDirtyStateChanged += (_, _) =>
            {
                if (_grid.IsCurrentCellDirty)
                {
                    _grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };
            _grid.CellEndEdit += (_, e) => ApplySuggestedDefaults(e.RowIndex, e.ColumnIndex);
            ConfigureColumns();

            _addButton.Text = "+ Add";
            _addButton.Size = new Size(90, 30);
            _addButton.Location = new Point(12, 632);
            _addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _addButton.Click += (_, _) => AddRow();

            _removeButton.Text = "- Remove";
            _removeButton.Size = new Size(100, 30);
            _removeButton.Location = new Point(108, 632);
            _removeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _removeButton.Click += (_, _) => RemoveSelectedRow();

            _reloadButton.Text = "Reload";
            _reloadButton.Size = new Size(90, 30);
            _reloadButton.Location = new Point(214, 632);
            _reloadButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _reloadButton.Click += (_, _) => ReloadRows();

            _resetButton.Text = "Reset Rules";
            _resetButton.Size = new Size(110, 30);
            _resetButton.Location = new Point(310, 632);
            _resetButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _resetButton.Click += (_, _) => ResetRows();

            _okButton.Text = "OK";
            _okButton.Size = new Size(90, 30);
            _okButton.Location = new Point(964, 632);
            _okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _okButton.Click += (_, _) => AcceptChanges();

            _cancelButton.Text = "Cancel";
            _cancelButton.Size = new Size(90, 30);
            _cancelButton.Location = new Point(1062, 632);
            _cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _cancelButton.Click += (_, _) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            Controls.Add(_pathLabel);
            Controls.Add(_hintLabel);
            Controls.Add(_grid);
            Controls.Add(_addButton);
            Controls.Add(_removeButton);
            Controls.Add(_reloadButton);
            Controls.Add(_resetButton);
            Controls.Add(_okButton);
            Controls.Add(_cancelButton);
        }

        private void ConfigureColumns()
        {
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Rule",
                DataPropertyName = nameof(PolicyRow.Name),
                FillWeight = 16
            });

            _grid.Columns.Add(new DataGridViewComboBoxColumn
            {
                HeaderText = "Category",
                DataPropertyName = nameof(PolicyRow.CategoryName),
                DataSource = CreateCategoryOptions(),
                FillWeight = 14,
                FlatStyle = FlatStyle.Standard
            });

            _grid.Columns.Add(new DataGridViewComboBoxColumn
            {
                HeaderText = "Is Check",
                DataPropertyName = nameof(PolicyRow.IsCheckMode),
                DataSource = new List<string> { string.Empty, bool.TrueString, bool.FalseString },
                FillWeight = 10,
                FlatStyle = FlatStyle.Standard
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Data Type Pattern",
                DataPropertyName = nameof(PolicyRow.DataTypePattern),
                FillWeight = 18
            });

            _grid.Columns.Add(new DataGridViewComboBoxColumn
            {
                HeaderText = "Editor",
                DataPropertyName = nameof(PolicyRow.EditorKey),
                DataSource = GetKnownEditorKeys(),
                FillWeight = 12,
                FlatStyle = FlatStyle.Standard
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Control Type",
                DataPropertyName = nameof(PolicyRow.ControlType),
                FillWeight = 18
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Binding Property",
                DataPropertyName = nameof(PolicyRow.BindingProperty),
                FillWeight = 16
            });

            _grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = "Enabled",
                DataPropertyName = nameof(PolicyRow.IsEnabled),
                FillWeight = 8
            });
        }

        private void LoadRows(IEnumerable<BeepFieldControlTypePolicyRule> rules)
        {
            _rows.Clear();
            foreach (var rule in rules)
            {
                _rows.Add(new PolicyRow
                {
                    Name = rule.Name,
                    CategoryName = rule.Category?.ToString() ?? string.Empty,
                    IsCheckMode = rule.IsCheck?.ToString() ?? string.Empty,
                    DataTypePattern = rule.DataTypePattern,
                    EditorKey = rule.EditorKey,
                    ControlType = rule.ControlType,
                    BindingProperty = rule.BindingProperty,
                    IsEnabled = rule.IsEnabled
                });
            }

            _grid.DataSource = _rows;
        }

        private void AddRow()
        {
            int nextIndex = _rows.Count + 1;
            var field = IntegratedDefinitionFactories.CreateDefaultFieldDefinition(nextIndex);
            _rows.Add(new PolicyRow
            {
                Name = $"Rule {nextIndex}",
                CategoryName = string.Empty,
                IsCheckMode = string.Empty,
                DataTypePattern = string.Empty,
                EditorKey = field.EditorKey,
                ControlType = field.ControlType,
                BindingProperty = field.BindingProperty,
                IsEnabled = true
            });

            FocusLastRow();
        }

        private void RemoveSelectedRow()
        {
            if (_grid.CurrentRow?.DataBoundItem is not PolicyRow selectedRow)
            {
                return;
            }

            _rows.Remove(selectedRow);
        }

        private void ReloadRows()
        {
            if (!BeepFieldControlTypeRegistry.ReloadPolicy(out string message))
            {
                MessageBox.Show(this, message, "Field Default Policy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoadRows(BeepFieldControlTypeRegistry.GetPolicyRules());
        }

        private void ResetRows()
        {
            if (MessageBox.Show(
                    this,
                    "Clear all custom policy rules and fall back to the built-in defaults? This does not save until you press OK.",
                    "Field Default Policy",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            _rows.Clear();
        }

        private void AcceptChanges()
        {
            if (!TryBuildRules(out var rules, out string message))
            {
                MessageBox.Show(this, message, "Field Default Policy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!BeepFieldControlTypeRegistry.SavePolicyRules(rules, out message))
            {
                MessageBox.Show(this, message, "Field Default Policy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private bool TryBuildRules(out List<BeepFieldControlTypePolicyRule> rules, out string message)
        {
            rules = new List<BeepFieldControlTypePolicyRule>();
            message = string.Empty;

            for (int index = 0; index < _rows.Count; index++)
            {
                var row = _rows[index];
                string resolvedName = string.IsNullOrWhiteSpace(row.Name) ? $"Rule {index + 1}" : row.Name.Trim();

                if (!string.IsNullOrWhiteSpace(row.CategoryName)
                    && !Enum.TryParse(row.CategoryName, true, out DbFieldCategory category))
                {
                    message = $"Row {index + 1}: '{row.CategoryName}' is not a valid field category.";
                    return false;
                }

                bool? isCheck = null;
                if (!string.IsNullOrWhiteSpace(row.IsCheckMode))
                {
                    if (!bool.TryParse(row.IsCheckMode, out bool parsedIsCheck))
                    {
                        message = $"Row {index + 1}: '{row.IsCheckMode}' is not a valid IsCheck value.";
                        return false;
                    }

                    isCheck = parsedIsCheck;
                }

                rules.Add(new BeepFieldControlTypePolicyRule
                {
                    Name = resolvedName,
                    Category = string.IsNullOrWhiteSpace(row.CategoryName) ? null : Enum.Parse<DbFieldCategory>(row.CategoryName, true),
                    IsCheck = isCheck,
                    DataTypePattern = row.DataTypePattern?.Trim() ?? string.Empty,
                    EditorKey = row.EditorKey?.Trim() ?? string.Empty,
                    ControlType = row.ControlType?.Trim() ?? string.Empty,
                    BindingProperty = row.BindingProperty?.Trim() ?? string.Empty,
                    IsEnabled = row.IsEnabled
                });
            }

            return true;
        }

        private void ApplySuggestedDefaults(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || rowIndex >= _rows.Count)
            {
                return;
            }

            var row = _rows[rowIndex];
            if (string.IsNullOrWhiteSpace(row.EditorKey))
            {
                return;
            }

            string columnName = _grid.Columns[columnIndex].DataPropertyName ?? string.Empty;
            if (string.Equals(columnName, nameof(PolicyRow.EditorKey), StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(row.ControlType))
            {
                row.ControlType = BeepFieldControlTypeRegistry.ResolveDefaultControlType(row.EditorKey);
            }

            if ((string.Equals(columnName, nameof(PolicyRow.EditorKey), StringComparison.OrdinalIgnoreCase)
                    || string.Equals(columnName, nameof(PolicyRow.ControlType), StringComparison.OrdinalIgnoreCase))
                && string.IsNullOrWhiteSpace(row.BindingProperty))
            {
                row.BindingProperty = BeepFieldControlTypeRegistry.ResolveDefaultBindingProperty(row.ControlType, row.EditorKey);
            }

            _grid.Refresh();
        }

        private void FocusLastRow()
        {
            _grid.ClearSelection();
            if (_rows.Count == 0)
            {
                return;
            }

            int rowIndex = _rows.Count - 1;
            _grid.Rows[rowIndex].Selected = true;
            _grid.CurrentCell = _grid.Rows[rowIndex].Cells[0];
        }

        private static List<string> CreateCategoryOptions()
        {
            return new[] { string.Empty }
                .Concat(Enum.GetNames(typeof(DbFieldCategory)).OrderBy(name => name, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        private static List<string> GetKnownEditorKeys()
        {
            var registry = new BeepBlockPresenterRegistry();
            registry.RegisterDefaults();

            return registry.Presenters
                .Select(presenter => presenter.Key)
                .Concat(new[] { string.Empty, "lov", "option" })
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(key => key, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private sealed class PolicyRow
        {
            public string Name { get; set; } = string.Empty;
            public string CategoryName { get; set; } = string.Empty;
            public string IsCheckMode { get; set; } = string.Empty;
            public string DataTypePattern { get; set; } = string.Empty;
            public string EditorKey { get; set; } = string.Empty;
            public string ControlType { get; set; } = string.Empty;
            public string BindingProperty { get; set; } = string.Empty;
            public bool IsEnabled { get; set; } = true;
        }
    }
}