using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Editors;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Services;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    internal sealed class BeepBlockFieldEditorForm : Form
    {
        private readonly BindingList<FieldRow> _rows = new();
        private readonly DataGridView _grid = new();
        private readonly Button _addButton = new();
        private readonly Button _removeButton = new();
        private readonly Button _moveUpButton = new();
        private readonly Button _moveDownButton = new();
        private readonly Button _defaultsButton = new();
        private readonly Button _okButton = new();
        private readonly Button _cancelButton = new();

        public BeepBlockFieldEditorForm(IEnumerable<BeepFieldDefinition>? fieldDefinitions, BeepBlockEntityDefinition? entityDefinition)
        {
            InitializeUi();
            LoadRows(fieldDefinitions, entityDefinition);
        }

        public List<BeepFieldDefinition> BuildFieldDefinitions()
        {
            return _rows
                .OrderBy(row => row.Order)
                .ThenBy(row => row.FieldName, StringComparer.OrdinalIgnoreCase)
                .Select(row => new BeepFieldDefinition
                {
                    FieldName = row.FieldName,
                    Label = row.Label,
                    EditorKey = row.EditorKey,
                    ControlType = row.ControlType,
                    BindingProperty = row.BindingProperty,
                    DefaultValue = row.DefaultValue,
                    Order = row.Order,
                    Width = row.Width,
                    IsVisible = row.IsVisible,
                    IsReadOnly = row.IsReadOnly,
                    Options = row.Options.Select(option => option.Clone()).ToList()
                })
                .ToList();
        }

        private void InitializeUi()
        {
            Text = "Edit Block Field Properties";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MinimizeBox = false;
            MaximizeBox = false;
            MinimumSize = new Size(860, 520);
            Size = new Size(980, 620);

            _grid.Location = new Point(12, 12);
            _grid.Size = new Size(940, 520);
            _grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _grid.AutoGenerateColumns = false;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.RowHeadersVisible = false;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.EditMode = DataGridViewEditMode.EditOnEnter;
            _grid.DataError += (_, _) => { };
            ConfigureColumns();

            _okButton.Text = "OK";
            _okButton.Size = new Size(90, 30);
            _okButton.Location = new Point(668, 544);
            _okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _okButton.Click += (_, _) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };

            _cancelButton.Text = "Cancel";
            _cancelButton.Size = new Size(90, 30);
            _cancelButton.Location = new Point(764, 544);
            _cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _cancelButton.Click += (_, _) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            _addButton.Text = "+ Add";
            _addButton.Size = new Size(90, 30);
            _addButton.Location = new Point(12, 544);
            _addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _addButton.Click += (_, _) => AddRow();

            _removeButton.Text = "- Remove";
            _removeButton.Size = new Size(100, 30);
            _removeButton.Location = new Point(108, 544);
            _removeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _removeButton.Click += (_, _) => RemoveSelectedRow();

            _moveUpButton.Text = "Move Up";
            _moveUpButton.Size = new Size(90, 30);
            _moveUpButton.Location = new Point(214, 544);
            _moveUpButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _moveUpButton.Click += (_, _) => MoveSelectedRow(-1);

            _moveDownButton.Text = "Move Down";
            _moveDownButton.Size = new Size(100, 30);
            _moveDownButton.Location = new Point(310, 544);
            _moveDownButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _moveDownButton.Click += (_, _) => MoveSelectedRow(1);

            _defaultsButton.Text = "Default Policy...";
            _defaultsButton.Size = new Size(130, 30);
            _defaultsButton.Location = new Point(416, 544);
            _defaultsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _defaultsButton.Click += (_, _) => OpenDefaultPolicyEditor();

            Controls.Add(_grid);
            Controls.Add(_addButton);
            Controls.Add(_removeButton);
            Controls.Add(_moveUpButton);
            Controls.Add(_moveDownButton);
            Controls.Add(_defaultsButton);
            Controls.Add(_okButton);
            Controls.Add(_cancelButton);
        }

        private void ConfigureColumns()
        {
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Field",
                DataPropertyName = nameof(FieldRow.FieldName),
                ReadOnly = true,
                FillWeight = 18
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Label",
                DataPropertyName = nameof(FieldRow.Label),
                FillWeight = 22
            });

            _grid.Columns.Add(new DataGridViewComboBoxColumn
            {
                HeaderText = "Editor",
                DataPropertyName = nameof(FieldRow.EditorKey),
                DataSource = GetKnownEditorKeys(),
                FillWeight = 14,
                FlatStyle = FlatStyle.Standard
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Control Type",
                DataPropertyName = nameof(FieldRow.ControlType),
                FillWeight = 20
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Binding",
                DataPropertyName = nameof(FieldRow.BindingProperty),
                FillWeight = 12
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Default",
                DataPropertyName = nameof(FieldRow.DefaultValue),
                FillWeight = 16
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Order",
                DataPropertyName = nameof(FieldRow.Order),
                FillWeight = 10
            });

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Width",
                DataPropertyName = nameof(FieldRow.Width),
                FillWeight = 10
            });

            _grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = "Visible",
                DataPropertyName = nameof(FieldRow.IsVisible),
                FillWeight = 10
            });

            _grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = "Read Only",
                DataPropertyName = nameof(FieldRow.IsReadOnly),
                FillWeight = 12
            });
        }

        private void LoadRows(IEnumerable<BeepFieldDefinition>? fieldDefinitions, BeepBlockEntityDefinition? entityDefinition)
        {
            _rows.Clear();

            var existingFields = (fieldDefinitions ?? Array.Empty<BeepFieldDefinition>())
                .Where(field => field != null && !string.IsNullOrWhiteSpace(field.FieldName))
                .ToDictionary(field => field.FieldName, field => field, StringComparer.OrdinalIgnoreCase);

            var fieldNames = existingFields.Keys.ToList();
            if (entityDefinition?.Fields != null)
            {
                foreach (var field in entityDefinition.Fields)
                {
                    if (field != null && !string.IsNullOrWhiteSpace(field.FieldName) &&
                        !fieldNames.Contains(field.FieldName, StringComparer.OrdinalIgnoreCase))
                    {
                        fieldNames.Add(field.FieldName);
                    }
                }
            }

            var rows = new List<FieldRow>();
            foreach (var fieldName in fieldNames)
            {
                existingFields.TryGetValue(fieldName, out var existingField);
                var entityField = entityDefinition?.Fields?.FirstOrDefault(field => string.Equals(field.FieldName, fieldName, StringComparison.OrdinalIgnoreCase));

                rows.Add(new FieldRow
                {
                    FieldName = fieldName,
                    Label = existingField?.Label ?? entityField?.Label ?? fieldName,
                    EditorKey = string.IsNullOrWhiteSpace(existingField?.EditorKey) ? InferEditorKey(entityField) : existingField!.EditorKey,
                    ControlType = string.IsNullOrWhiteSpace(existingField?.ControlType)
                        ? InferControlType(entityField, string.IsNullOrWhiteSpace(existingField?.EditorKey) ? InferEditorKey(entityField) : existingField!.EditorKey)
                        : existingField!.ControlType,
                    BindingProperty = string.IsNullOrWhiteSpace(existingField?.BindingProperty)
                        ? InferBindingProperty(string.IsNullOrWhiteSpace(existingField?.ControlType)
                            ? InferControlType(entityField, string.IsNullOrWhiteSpace(existingField?.EditorKey) ? InferEditorKey(entityField) : existingField!.EditorKey)
                            : existingField!.ControlType,
                            string.IsNullOrWhiteSpace(existingField?.EditorKey) ? InferEditorKey(entityField) : existingField!.EditorKey)
                        : existingField!.BindingProperty,
                    DefaultValue = existingField?.DefaultValue ?? entityField?.DefaultValue ?? string.Empty,
                    Order = existingField?.Order ?? entityField?.Order ?? _rows.Count,
                    Width = existingField?.Width ?? 160,
                    IsVisible = existingField?.IsVisible ?? true,
                    IsReadOnly = existingField?.IsReadOnly ?? entityField?.IsReadOnly ?? false,
                    Options = existingField?.Options?.Select(option => option.Clone()).ToList() ?? new List<BeepFieldOptionDefinition>()
                });
            }

            foreach (FieldRow row in rows
                .OrderBy(row => row.Order)
                .ThenBy(row => row.FieldName, StringComparer.OrdinalIgnoreCase))
            {
                _rows.Add(row);
            }

            _grid.DataSource = _rows;
        }

        private void AddRow()
        {
            int nextOrder = _rows.Count == 0 ? 0 : _rows.Max(row => row.Order) + 1;
            int nextIndex = nextOrder + 1;
            var field = IntegratedDefinitionFactories.CreateDefaultFieldDefinition(nextIndex);

            _rows.Add(new FieldRow
            {
                FieldName = field.FieldName,
                Label = field.Label,
                EditorKey = field.EditorKey,
                ControlType = field.ControlType,
                BindingProperty = field.BindingProperty,
                DefaultValue = field.DefaultValue,
                Order = field.Order,
                Width = field.Width,
                IsVisible = field.IsVisible,
                IsReadOnly = field.IsReadOnly
            });

            _grid.ClearSelection();
            if (_rows.Count > 0)
            {
                int rowIndex = _rows.Count - 1;
                _grid.Rows[rowIndex].Selected = true;
                _grid.CurrentCell = _grid.Rows[rowIndex].Cells[0];
            }
        }

        private void RemoveSelectedRow()
        {
            if (_grid.CurrentRow?.DataBoundItem is not FieldRow selectedRow)
            {
                return;
            }

            _rows.Remove(selectedRow);
            RecalculateRowOrder();
            _rows.ResetBindings();
        }

        private void MoveSelectedRow(int direction)
        {
            if (_grid.CurrentRow?.DataBoundItem is not FieldRow selectedRow)
            {
                return;
            }

            int currentIndex = _rows.IndexOf(selectedRow);
            if (currentIndex < 0)
            {
                return;
            }

            int nextIndex = currentIndex + direction;
            if (nextIndex < 0 || nextIndex >= _rows.Count)
            {
                return;
            }

            _rows.RaiseListChangedEvents = false;
            _rows.RemoveAt(currentIndex);
            _rows.Insert(nextIndex, selectedRow);
            RecalculateRowOrder();
            _rows.RaiseListChangedEvents = true;
            _rows.ResetBindings();

            _grid.ClearSelection();
            _grid.Rows[nextIndex].Selected = true;
            _grid.CurrentCell = _grid.Rows[nextIndex].Cells[0];
        }

        private void OpenDefaultPolicyEditor()
        {
            using var dialog = new BeepFieldControlTypePolicyEditorForm();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                MessageBox.Show(
                    this,
                    "Field default policy saved. New generated fields and new rows added here will use the updated defaults.",
                    "Field Default Policy",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private static List<string> GetKnownEditorKeys()
        {
            var registry = new BeepBlockPresenterRegistry();
            registry.RegisterDefaults();

            return registry.Presenters
                .Select(presenter => presenter.Key)
                .Concat(new[] { "lov", "option" })
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(key => key, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string InferEditorKey(BeepBlockEntityFieldDefinition? field)
        {
            if (field == null)
            {
                return "text";
            }

            string dataType = field.DataType ?? string.Empty;
            if (field.IsCheck ||
                field.Category == TheTechIdea.Beep.Utilities.DbFieldCategory.Boolean ||
                dataType.IndexOf("bool", StringComparison.OrdinalIgnoreCase) >= 0 ||
                dataType.IndexOf("bit", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "checkbox";
            }

            if (field.Category == TheTechIdea.Beep.Utilities.DbFieldCategory.Enum)
            {
                return "combo";
            }

            if (field.Category == TheTechIdea.Beep.Utilities.DbFieldCategory.Date ||
                field.Category == TheTechIdea.Beep.Utilities.DbFieldCategory.DateTime ||
                dataType.IndexOf("date", StringComparison.OrdinalIgnoreCase) >= 0 ||
                dataType.IndexOf("time", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "date";
            }

            if (field.Category == TheTechIdea.Beep.Utilities.DbFieldCategory.Numeric ||
                field.Category == TheTechIdea.Beep.Utilities.DbFieldCategory.Integer ||
                field.Category == TheTechIdea.Beep.Utilities.DbFieldCategory.Decimal ||
                field.Category == TheTechIdea.Beep.Utilities.DbFieldCategory.Double ||
                field.Category == TheTechIdea.Beep.Utilities.DbFieldCategory.Float ||
                field.Category == TheTechIdea.Beep.Utilities.DbFieldCategory.Currency ||
                dataType.IndexOf("int", StringComparison.OrdinalIgnoreCase) >= 0 ||
                dataType.IndexOf("decimal", StringComparison.OrdinalIgnoreCase) >= 0 ||
                dataType.IndexOf("double", StringComparison.OrdinalIgnoreCase) >= 0 ||
                dataType.IndexOf("float", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "numeric";
            }

            return "text";
        }

        private static string InferControlType(BeepBlockEntityFieldDefinition? field, string editorKey)
        {
            if (field == null)
            {
                return BeepFieldControlTypeRegistry.ResolveDefaultControlType(editorKey);
            }

            return BeepFieldControlTypeRegistry.ResolveDefaultControlType(field.Category, field.DataType, field.IsCheck);
        }

        private static string InferBindingProperty(string controlType, string editorKey)
        {
            return BeepFieldControlTypeRegistry.ResolveDefaultBindingProperty(controlType, editorKey);
        }

        private void RecalculateRowOrder()
        {
            for (int index = 0; index < _rows.Count; index++)
            {
                _rows[index].Order = index;
            }
        }

        private sealed class FieldRow
        {
            public string FieldName { get; set; } = string.Empty;
            public string Label { get; set; } = string.Empty;
            public string EditorKey { get; set; } = "text";
            public string ControlType { get; set; } = nameof(BeepTextBox);
            public string BindingProperty { get; set; } = nameof(Control.Text);
            public string DefaultValue { get; set; } = string.Empty;
            public int Order { get; set; }
            public int Width { get; set; } = 160;
            public bool IsVisible { get; set; } = true;
            public bool IsReadOnly { get; set; }
            public List<BeepFieldOptionDefinition> Options { get; set; } = new();
        }
    }
}