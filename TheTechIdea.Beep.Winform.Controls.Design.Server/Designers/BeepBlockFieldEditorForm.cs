using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Services;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    internal sealed class BeepBlockFieldEditorForm : Form
    {
        private readonly BindingList<FieldRow> _rows = new();
        private readonly DataGridView _grid = new();
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
            _okButton.Location = new Point(766, 544);
            _okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _okButton.Click += (_, _) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };

            _cancelButton.Text = "Cancel";
            _cancelButton.Size = new Size(90, 30);
            _cancelButton.Location = new Point(862, 544);
            _cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _cancelButton.Click += (_, _) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            Controls.Add(_grid);
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
                FillWeight = 16,
                FlatStyle = FlatStyle.Standard
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

            foreach (var fieldName in fieldNames.OrderBy(name => name, StringComparer.OrdinalIgnoreCase))
            {
                existingFields.TryGetValue(fieldName, out var existingField);
                var entityField = entityDefinition?.Fields?.FirstOrDefault(field => string.Equals(field.FieldName, fieldName, StringComparison.OrdinalIgnoreCase));

                _rows.Add(new FieldRow
                {
                    FieldName = fieldName,
                    Label = existingField?.Label ?? entityField?.Label ?? fieldName,
                    EditorKey = string.IsNullOrWhiteSpace(existingField?.EditorKey) ? InferEditorKey(entityField) : existingField!.EditorKey,
                    Order = existingField?.Order ?? entityField?.Order ?? _rows.Count,
                    Width = existingField?.Width ?? 160,
                    IsVisible = existingField?.IsVisible ?? true,
                    IsReadOnly = existingField?.IsReadOnly ?? entityField?.IsReadOnly ?? false,
                    Options = existingField?.Options?.Select(option => option.Clone()).ToList() ?? new List<BeepFieldOptionDefinition>()
                });
            }

            _grid.DataSource = _rows;
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

        private sealed class FieldRow
        {
            public string FieldName { get; set; } = string.Empty;
            public string Label { get; set; } = string.Empty;
            public string EditorKey { get; set; } = "text";
            public int Order { get; set; }
            public int Width { get; set; } = 160;
            public bool IsVisible { get; set; } = true;
            public bool IsReadOnly { get; set; }
            public List<BeepFieldOptionDefinition> Options { get; set; } = new();
        }
    }
}