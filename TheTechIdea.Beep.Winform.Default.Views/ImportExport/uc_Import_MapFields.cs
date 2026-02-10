using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Default.Views.Template;
using TheTechIdea.Beep.Workflow;
using TheTechIdea.Beep.Workflow.Mapping;

namespace TheTechIdea.Beep.Winform.Default.Views.ImportExport
{
    [AddinAttribute(Caption = "Map Entity Fields to Another", Name = "uc_Import_MapFields", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]
    public partial class uc_Import_MapFields : TemplateUserControl, IWizardStepContent
    {
        private readonly BindingList<ImportFieldMapRow> _rows = new();
        private readonly Label _summaryLabel;
        private readonly Button _autoMapButton;
        private readonly DataGridView _mappingGrid;
        private ImportSelectionContext? _selection;
        private EntityStructure? _sourceStructure;
        private EntityStructure? _destinationStructure;
        private bool _isLoading;

        public uc_Import_MapFields(IServiceProvider services) : base(services)
        {
            InitializeComponent();

            _summaryLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 44,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding = new Padding(4, 0, 0, 0)
            };

            _autoMapButton = new Button
            {
                Dock = DockStyle.Top,
                Height = 32,
                Text = "Auto map by field name"
            };
            _autoMapButton.Click += AutoMapButton_Click;

            _mappingGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            _mappingGrid.CurrentCellDirtyStateChanged += MappingGrid_CurrentCellDirtyStateChanged;
            _mappingGrid.CellValueChanged += MappingGrid_CellValueChanged;
            _mappingGrid.CellEndEdit += MappingGrid_CellEndEdit;

            _mappingGrid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = nameof(ImportFieldMapRow.Selected),
                HeaderText = "Use",
                Width = 50
            });
            _mappingGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ImportFieldMapRow.SourceField),
                HeaderText = "Source Field",
                ReadOnly = true,
                Width = 180
            });
            _mappingGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ImportFieldMapRow.SourceType),
                HeaderText = "Source Type",
                ReadOnly = true,
                Width = 120
            });
            _mappingGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ImportFieldMapRow.DestinationField),
                HeaderText = "Destination Field",
                Width = 180
            });
            _mappingGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(ImportFieldMapRow.DestinationType),
                HeaderText = "Destination Type",
                ReadOnly = true,
                Width = 120
            });
            _mappingGrid.DataSource = _rows;

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8)
            };
            panel.Controls.Add(_mappingGrid);
            panel.Controls.Add(_autoMapButton);
            panel.Controls.Add(_summaryLabel);
            Controls.Add(panel);
        }

        public event EventHandler<StepValidationEventArgs>? ValidationStateChanged;

        public bool IsComplete => ValidateStep().IsValid;

        public string NextButtonText => string.Empty;

        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
            UpdateSummaryText();
            RaiseValidationState();
        }

        public override void Configure(Dictionary<string, object> settings)
        {
            base.Configure(settings);
            UpdateSummaryText();
            RaiseValidationState();
        }

        public void OnStepEnter(WizardContext context)
        {
            _isLoading = true;
            try
            {
                _selection =
                    ImportExportContextStore.ParseSelection(context.GetAllData()) ??
                    ImportExportContextStore.GetSelection();

                LoadStructuresAndRows();

                var mappingFromContext = context.GetValue<EntityDataMap?>(ImportExportParameterKeys.Mapping, null);
                ApplyExistingMapping(mappingFromContext ?? ImportExportContextStore.GetMapping());
            }
            finally
            {
                _isLoading = false;
            }

            UpdateSummaryText();
            RaiseValidationState();
        }

        public void OnStepLeave(WizardContext context)
        {
            var mapping = BuildMappingFromRows();
            if (mapping != null)
            {
                context.SetValue(ImportExportParameterKeys.Mapping, mapping);
                ImportExportContextStore.SaveMapping(mapping);
            }
            else
            {
                context.Remove(ImportExportParameterKeys.Mapping);
                ImportExportContextStore.SaveMapping(null);
            }
        }

        WizardValidationResult IWizardStepContent.Validate()
        {
            return ValidateStep();
        }

        private WizardValidationResult ValidateStep()
        {
            if (_selection == null || !_selection.IsValid)
            {
                return WizardValidationResult.Error("Complete source and destination selection in the previous step.");
            }

            if (_sourceStructure?.Fields == null || !_sourceStructure.Fields.Any())
            {
                return WizardValidationResult.Error("Unable to load source structure.");
            }

            if (_destinationStructure?.Fields == null || !_destinationStructure.Fields.Any())
            {
                return WizardValidationResult.Error("Unable to load destination structure.");
            }

            var selectedMappings = _rows.Count(row =>
                row.Selected &&
                !string.IsNullOrWhiteSpace(row.DestinationField) &&
                DestinationFieldExists(row.DestinationField));

            if (selectedMappings == 0)
            {
                return WizardValidationResult.Error("Select at least one valid field mapping.");
            }

            return WizardValidationResult.Success();
        }

        public Task<WizardValidationResult> ValidateAsync()
        {
            return Task.FromResult(ValidateStep());
        }

        private void AutoMapButton_Click(object? sender, EventArgs e)
        {
            if (_destinationStructure?.Fields == null)
            {
                return;
            }

            foreach (var row in _rows)
            {
                var destinationField = _destinationStructure.Fields.FirstOrDefault(field =>
                    field.FieldName.Equals(row.SourceField, System.StringComparison.OrdinalIgnoreCase));

                row.DestinationField = destinationField?.FieldName ?? string.Empty;
                row.DestinationType = destinationField?.Fieldtype ?? string.Empty;
                row.Selected = destinationField != null;
            }

            _mappingGrid.Refresh();
            UpdateSummaryText();
            RaiseValidationState();
        }

        private void MappingGrid_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (_mappingGrid.IsCurrentCellDirty)
            {
                _mappingGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void MappingGrid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (_isLoading || e.RowIndex < 0 || e.RowIndex >= _rows.Count)
            {
                return;
            }

            SyncDestinationTypeForRow(e.RowIndex);
            UpdateSummaryText();
            RaiseValidationState();
        }

        private void MappingGrid_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (_isLoading || e.RowIndex < 0 || e.RowIndex >= _rows.Count)
            {
                return;
            }

            SyncDestinationTypeForRow(e.RowIndex);
            UpdateSummaryText();
            RaiseValidationState();
        }

        private void LoadStructuresAndRows()
        {
            _rows.Clear();
            _sourceStructure = null;
            _destinationStructure = null;

            if (_selection == null || !_selection.IsValid || Editor == null)
            {
                return;
            }

            try
            {
                var sourceDataSource = Editor.GetDataSource(_selection.SourceDataSourceName);
                var destinationDataSource = Editor.GetDataSource(_selection.DestinationDataSourceName);

                if (sourceDataSource != null && sourceDataSource.ConnectionStatus != System.Data.ConnectionState.Open)
                {
                    sourceDataSource.Openconnection();
                }

                if (destinationDataSource != null && destinationDataSource.ConnectionStatus != System.Data.ConnectionState.Open)
                {
                    destinationDataSource.Openconnection();
                }

                _sourceStructure = sourceDataSource?.GetEntityStructure(_selection.SourceEntityName, false);
                _destinationStructure = destinationDataSource?.GetEntityStructure(_selection.DestinationEntityName, false);

                var destinationFields = _destinationStructure?.Fields ?? new List<EntityField>();
                foreach (var sourceField in _sourceStructure?.Fields ?? Enumerable.Empty<EntityField>())
                {
                    var destinationMatch = destinationFields.FirstOrDefault(field =>
                        field.FieldName.Equals(sourceField.FieldName, System.StringComparison.OrdinalIgnoreCase));

                    _rows.Add(new ImportFieldMapRow
                    {
                        Selected = destinationMatch != null,
                        SourceField = sourceField.FieldName,
                        SourceType = sourceField.Fieldtype,
                        DestinationField = destinationMatch?.FieldName ?? string.Empty,
                        DestinationType = destinationMatch?.Fieldtype ?? string.Empty
                    });
                }
            }
            catch (System.Exception ex)
            {
                Editor.AddLogMessage("ImportExport", $"Error loading structures: {ex.Message}", System.DateTime.Now, 0, null, Errors.Failed);
            }
        }

        private void ApplyExistingMapping(EntityDataMap? mapping)
        {
            if (mapping?.MappedEntities == null || !mapping.MappedEntities.Any())
            {
                return;
            }

            var mappedEntity = mapping.MappedEntities.FirstOrDefault(entity =>
                _selection != null &&
                entity.EntityName.Equals(_selection.DestinationEntityName, System.StringComparison.OrdinalIgnoreCase))
                ?? mapping.MappedEntities.First();

            if (mappedEntity.FieldMapping == null || !mappedEntity.FieldMapping.Any())
            {
                return;
            }

            foreach (var row in _rows)
            {
                var fieldMap = mappedEntity.FieldMapping.FirstOrDefault(field =>
                    field.FromFieldName.Equals(row.SourceField, System.StringComparison.OrdinalIgnoreCase));

                if (fieldMap == null)
                {
                    row.Selected = false;
                    row.DestinationField = string.Empty;
                    row.DestinationType = string.Empty;
                }
                else
                {
                    row.Selected = true;
                    row.DestinationField = fieldMap.ToFieldName;
                    row.DestinationType = LookupDestinationType(fieldMap.ToFieldName);
                }
            }

            _mappingGrid.Refresh();
        }

        private EntityDataMap? BuildMappingFromRows()
        {
            if (_selection == null || !_selection.IsValid || _destinationStructure?.Fields == null)
            {
                return null;
            }

            var selectedRows = _rows
                .Where(row => row.Selected && !string.IsNullOrWhiteSpace(row.DestinationField))
                .Where(row => DestinationFieldExists(row.DestinationField))
                .ToList();

            if (!selectedRows.Any())
            {
                return null;
            }

            var fieldMappings = selectedRows.Select((row, index) => new Mapping_rep_fields
            {
                FromEntityName = _selection.SourceEntityName,
                FromFieldName = row.SourceField,
                FromFieldType = row.SourceType,
                FromFieldIndex = index,
                ToEntityName = _selection.DestinationEntityName,
                ToFieldName = row.DestinationField,
                ToFieldType = LookupDestinationType(row.DestinationField),
                ToFieldIndex = index
            }).ToList();

            var mappedEntity = new EntityDataMap_DTL
            {
                EntityDataSource = _selection.DestinationDataSourceName,
                EntityName = _selection.DestinationEntityName,
                EntityFields = _destinationStructure.Fields,
                SelectedDestFields = _destinationStructure.Fields,
                FieldMapping = fieldMappings
            };

            return new EntityDataMap
            {
                EntityName = _selection.DestinationEntityName,
                EntityDataSource = _selection.DestinationDataSourceName,
                MappingName = $"{_selection.SourceEntityName}_to_{_selection.DestinationEntityName}",
                EntityFields = _destinationStructure.Fields,
                MappedEntities = new List<EntityDataMap_DTL> { mappedEntity }
            };
        }

        private bool DestinationFieldExists(string destinationField)
        {
            return _destinationStructure?.Fields?.Any(field =>
                       field.FieldName.Equals(destinationField, System.StringComparison.OrdinalIgnoreCase)) == true;
        }

        private string LookupDestinationType(string destinationField)
        {
            var destinationMatch = _destinationStructure?.Fields?.FirstOrDefault(field =>
                field.FieldName.Equals(destinationField, System.StringComparison.OrdinalIgnoreCase));

            return destinationMatch?.Fieldtype ?? string.Empty;
        }

        private void SyncDestinationTypeForRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _rows.Count)
            {
                return;
            }

            var row = _rows[rowIndex];
            row.DestinationType = LookupDestinationType(row.DestinationField);
            _rows.ResetItem(rowIndex);
        }

        private void UpdateSummaryText()
        {
            var totalSourceFields = _rows.Count;
            var selectedMappings = _rows.Count(row => row.Selected && !string.IsNullOrWhiteSpace(row.DestinationField));

            _summaryLabel.Text = _selection == null
                ? "Select source/destination first."
                : $"Source: {_selection.SourceEntityName} -> Destination: {_selection.DestinationEntityName} | " +
                  $"Mapped: {selectedMappings}/{totalSourceFields}";
        }

        private void RaiseValidationState()
        {
            var result = ValidateStep();
            ValidationStateChanged?.Invoke(this, new StepValidationEventArgs(result.IsValid, result.ErrorMessage));
        }
    }
}
