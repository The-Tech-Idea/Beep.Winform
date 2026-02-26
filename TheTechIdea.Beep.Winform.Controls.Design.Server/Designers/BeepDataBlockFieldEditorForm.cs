using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    internal sealed class BeepDataBlockFieldEditorForm : Form
    {
        private readonly BeepDataBlock _dataBlock;
        private readonly Dictionary<string, DbFieldCategory> _categoryByField = new(StringComparer.OrdinalIgnoreCase);
        private readonly DataGridView _grid = new();
        private readonly BindingList<FieldSelectionRow> _rows = new();
        private readonly Button _selectAllButton = new();
        private readonly Button _clearAllButton = new();
        private readonly Button _okButton = new();
        private readonly Button _cancelButton = new();
        private readonly Label _titleLabel = new();
        private readonly Label _subtitleLabel = new();

        private const string ControlTypeColumnName = "ControlTypeColumn";
        private const string EditorOverrideColumnName = "EditorOverrideColumn";
        private const string TemplateColumnName = "TemplateColumn";

        public BeepDataBlockFieldEditorForm(BeepDataBlock dataBlock)
        {
            _dataBlock = dataBlock ?? throw new ArgumentNullException(nameof(dataBlock));
            InitializeUi();
            LoadRows();
        }

        public bool ApplySelections(IComponentChangeService? changeService)
        {
            var property = TypeDescriptor.GetProperties(_dataBlock)["FieldSelections"];
            changeService?.OnComponentChanging(_dataBlock, property);

            try
            {
                _dataBlock.FieldSelections.Clear();
                foreach (var row in _rows)
                {
                    var selection = new BeepDataBlockFieldSelection
                    {
                        FieldName = row.FieldName,
                        IncludeInView = row.IncludeInView,
                        ControlTypeFullName = NormalizeDefaultValue(row.ControlTypeFullName),
                        EditorTypeOverrideFullName = NormalizeDefaultValue(row.EditorTypeOverrideFullName),
                        TemplateId = NormalizeNoneValue(row.TemplateId),
                        InlineSettingsJson = row.InlineSettingsJson?.Trim() ?? string.Empty
                    };

                    _dataBlock.FieldSelections.Add(selection);
                }

                changeService?.OnComponentChanged(_dataBlock, property, null, null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void InitializeUi()
        {
            Text = "Edit Data Block Field Properties";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MinimizeBox = false;
            MaximizeBox = false;
            MinimumSize = new Size(900, 560);
            Size = new Size(1020, 640);

            _titleLabel.Text = "Field Properties";
            _titleLabel.Font = new Font(Font, FontStyle.Bold);
            _titleLabel.AutoSize = true;
            _titleLabel.Location = new Point(12, 12);

            _subtitleLabel.Text = "Configure visibility, editor type, template, and inline settings for each field.";
            _subtitleLabel.AutoSize = true;
            _subtitleLabel.Location = new Point(12, 38);

            _grid.Location = new Point(12, 68);
            _grid.Size = new Size(980, 480);
            _grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _grid.AutoGenerateColumns = false;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.RowHeadersVisible = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.EditMode = DataGridViewEditMode.EditOnEnter;
            _grid.DataError += (_, _) => { };
            _grid.EditingControlShowing += Grid_EditingControlShowing;
            _grid.CurrentCellDirtyStateChanged += (_, _) =>
            {
                if (_grid.IsCurrentCellDirty)
                {
                    _grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };

            ConfigureColumns();

            _selectAllButton.Text = "Select All";
            _selectAllButton.Size = new Size(100, 30);
            _selectAllButton.Location = new Point(12, 560);
            _selectAllButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _selectAllButton.Click += (_, _) => SetAllIncluded(true);

            _clearAllButton.Text = "Clear All";
            _clearAllButton.Size = new Size(100, 30);
            _clearAllButton.Location = new Point(118, 560);
            _clearAllButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _clearAllButton.Click += (_, _) => SetAllIncluded(false);

            _okButton.Text = "OK";
            _okButton.Size = new Size(90, 30);
            _okButton.Location = new Point(808, 560);
            _okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _okButton.Click += (_, _) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };

            _cancelButton.Text = "Cancel";
            _cancelButton.Size = new Size(90, 30);
            _cancelButton.Location = new Point(902, 560);
            _cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _cancelButton.Click += (_, _) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            Controls.Add(_titleLabel);
            Controls.Add(_subtitleLabel);
            Controls.Add(_grid);
            Controls.Add(_selectAllButton);
            Controls.Add(_clearAllButton);
            Controls.Add(_okButton);
            Controls.Add(_cancelButton);
        }

        private void ConfigureColumns()
        {
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Field",
                DataPropertyName = nameof(FieldSelectionRow.FieldName),
                ReadOnly = true,
                FillWeight = 22
            });

            _grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = "Include",
                DataPropertyName = nameof(FieldSelectionRow.IncludeInView),
                FillWeight = 8
            });

            _grid.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = ControlTypeColumnName,
                HeaderText = "Control Type",
                DataPropertyName = nameof(FieldSelectionRow.ControlTypeFullName),
                FlatStyle = FlatStyle.Standard,
                FillWeight = 22
            });

            _grid.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = EditorOverrideColumnName,
                HeaderText = "Editor Override",
                DataPropertyName = nameof(FieldSelectionRow.EditorTypeOverrideFullName),
                FlatStyle = FlatStyle.Standard,
                FillWeight = 22
            });

            _grid.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = TemplateColumnName,
                HeaderText = "Template",
                DataPropertyName = nameof(FieldSelectionRow.TemplateId),
                FlatStyle = FlatStyle.Standard,
                FillWeight = 14
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Inline Settings JSON",
                DataPropertyName = nameof(FieldSelectionRow.InlineSettingsJson),
                FillWeight = 22
            });
        }

        private void LoadRows()
        {
            EnsureEntityMetadata();
            BuildCategoryMap();

            var existingSelections = _dataBlock.FieldSelections
                .Where(x => !string.IsNullOrWhiteSpace(x.FieldName))
                .ToDictionary(x => x.FieldName, x => x, StringComparer.OrdinalIgnoreCase);

            var orderedFieldNames = _categoryByField.Keys
                .Concat(existingSelections.Keys)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();

            _rows.Clear();
            foreach (var fieldName in orderedFieldNames)
            {
                if (string.IsNullOrWhiteSpace(fieldName))
                {
                    continue;
                }

                existingSelections.TryGetValue(fieldName, out var selection);
                _categoryByField.TryGetValue(fieldName, out var category);

                _rows.Add(new FieldSelectionRow
                {
                    FieldName = fieldName,
                    Category = category,
                    IncludeInView = selection?.IncludeInView ?? true,
                    ControlTypeFullName = string.IsNullOrWhiteSpace(selection?.ControlTypeFullName) ? "<Default>" : selection.ControlTypeFullName,
                    EditorTypeOverrideFullName = string.IsNullOrWhiteSpace(selection?.EditorTypeOverrideFullName) ? "<Default>" : selection.EditorTypeOverrideFullName,
                    TemplateId = string.IsNullOrWhiteSpace(selection?.TemplateId) ? "<None>" : selection.TemplateId,
                    InlineSettingsJson = selection?.InlineSettingsJson ?? string.Empty
                });
            }

            _grid.DataSource = _rows;
        }

        private void EnsureEntityMetadata()
        {
            if (!string.IsNullOrWhiteSpace(_dataBlock.EntityName))
            {
                try
                {
                    _dataBlock.RefreshEntityMetadata(regenerateSurface: false);
                }
                catch
                {
                    // Best effort in design-time environments.
                }
            }
        }

        private void BuildCategoryMap()
        {
            _categoryByField.Clear();

            var fields = _dataBlock.EntityStructure?.Fields;
            if (fields == null)
            {
                return;
            }

            foreach (var field in fields)
            {
                if (!string.IsNullOrWhiteSpace(field.FieldName))
                {
                    _categoryByField[field.FieldName] = field.FieldCategory;
                }
            }
        }

        private void SetAllIncluded(bool include)
        {
            foreach (var row in _rows)
            {
                row.IncludeInView = include;
            }

            _grid.Refresh();
        }

        private void Grid_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (_grid.CurrentCell == null || _grid.CurrentCell.RowIndex < 0)
            {
                return;
            }

            if (e.Control is not ComboBox comboBox)
            {
                return;
            }

            var row = _rows[_grid.CurrentCell.RowIndex];
            var columnName = _grid.Columns[_grid.CurrentCell.ColumnIndex].Name;

            List<string> values;
            if (string.Equals(columnName, ControlTypeColumnName, StringComparison.Ordinal))
            {
                values = GetControlTypeOptions(row.Category, includeNoneOption: false);
            }
            else if (string.Equals(columnName, EditorOverrideColumnName, StringComparison.Ordinal))
            {
                values = GetControlTypeOptions(row.Category, includeNoneOption: false);
            }
            else if (string.Equals(columnName, TemplateColumnName, StringComparison.Ordinal))
            {
                values = GetTemplateOptions(row.Category);
            }
            else
            {
                return;
            }

            comboBox.DataSource = values;
        }

        private List<string> GetControlTypeOptions(DbFieldCategory category, bool includeNoneOption)
        {
            var values = new List<string> { "<Default>" };
            if (includeNoneOption)
            {
                values.Add("<None>");
            }

            var known = _dataBlock.GetKnownRecordEditorTypes(category)
                .Select(x => x.FullName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase);

            values.AddRange(known!);
            return values;
        }

        private List<string> GetTemplateOptions(DbFieldCategory category)
        {
            var values = new List<string> { "<None>" };
            values.AddRange(_dataBlock.GetEditorTemplates(category)
                .Select(x => x.TemplateId)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase));

            return values;
        }

        private static string NormalizeDefaultValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "<Default>", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            return value.Trim();
        }

        private static string NormalizeNoneValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "<None>", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            return value.Trim();
        }

        private sealed class FieldSelectionRow
        {
            public string FieldName { get; set; } = string.Empty;
            public bool IncludeInView { get; set; } = true;
            public string ControlTypeFullName { get; set; } = "<Default>";
            public string EditorTypeOverrideFullName { get; set; } = "<Default>";
            public string TemplateId { get; set; } = "<None>";
            public string InlineSettingsJson { get; set; } = string.Empty;
            public DbFieldCategory Category { get; set; } = DbFieldCategory.String;
        }
    }
}
