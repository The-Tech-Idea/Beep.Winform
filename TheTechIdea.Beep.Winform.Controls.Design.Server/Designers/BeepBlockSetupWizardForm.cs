using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    internal sealed class BeepBlockSetupWizardForm : Form
    {
        private readonly BeepBlock _block;
        private readonly IDMEEditor? _editor;
        private readonly BeepBlockDefinition _baselineDefinition;
        private readonly BeepBlockEntityDefinition _baselineEntity;

        private readonly Label _stepTitle = new();
        private readonly Label _stepSubtitle = new();
        private readonly Panel _contentPanel = new();

        private readonly Panel _stepConnectionPanel = new();
        private readonly Panel _stepEntityPanel = new();
        private readonly Panel _stepFieldsPanel = new();
        private readonly Panel _stepViewPanel = new();

        private readonly ComboBox _connectionCombo = new();
        private readonly ComboBox _entityCombo = new();
        private readonly CheckedListBox _fieldsList = new();
        private readonly RadioButton _recordControlsRadio = new();
        private readonly RadioButton _gridRadio = new();
        private readonly Label _summaryLabel = new();

        private readonly Button _backButton = new();
        private readonly Button _nextButton = new();
        private readonly Button _cancelButton = new();
        private readonly Button _selectAllFieldsButton = new();
        private readonly Button _clearFieldsButton = new();

        private readonly Dictionary<string, EntityField> _metadataFieldsByName = new(StringComparer.OrdinalIgnoreCase);
        private int _currentStepIndex;

        public string SelectedConnectionName => _connectionCombo.Text?.Trim() ?? string.Empty;
        public string SelectedEntityName => _entityCombo.Text?.Trim() ?? string.Empty;
        public HashSet<string> SelectedFieldNames =>
            _fieldsList.CheckedItems
                .Cast<object>()
                .Select(item => item?.ToString() ?? string.Empty)
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

        public BeepBlockPresentationMode SelectedPresentationMode =>
            _gridRadio.Checked ? BeepBlockPresentationMode.Grid : BeepBlockPresentationMode.Record;

        public BeepBlockSetupWizardForm(BeepBlock block, IDMEEditor? editor)
        {
            _block = block ?? throw new ArgumentNullException(nameof(block));
            _editor = editor;
            _baselineDefinition = (block.Definition ?? new BeepBlockDefinition()).Clone();
            _baselineEntity = (_baselineDefinition.Entity ?? block.Entity ?? new BeepBlockEntityDefinition()).Clone();

            InitializeUi();
            LoadInitialValues();
            UpdateStepUi();
        }

        public BeepBlockDefinition BuildUpdatedDefinition()
        {
            var definition = _baselineDefinition.Clone();
            string resolvedBlockName = string.IsNullOrWhiteSpace(definition.BlockName)
                ? (!string.IsNullOrWhiteSpace(_block.BlockName) ? _block.BlockName : _block.Name)
                : definition.BlockName;

            if (string.IsNullOrWhiteSpace(resolvedBlockName))
            {
                resolvedBlockName = string.IsNullOrWhiteSpace(SelectedEntityName) ? "MainBlock" : SelectedEntityName;
            }

            definition.BlockName = resolvedBlockName;
            definition.Caption = string.IsNullOrWhiteSpace(definition.Caption)
                ? (string.IsNullOrWhiteSpace(SelectedEntityName) ? resolvedBlockName : SelectedEntityName)
                : definition.Caption;
            definition.PresentationMode = SelectedPresentationMode;

            BeepBlockEntityDefinition entityDefinition = BuildEntityDefinition();
            definition.Entity = entityDefinition;

            var existingFields = definition.Fields
                .Where(field => field != null && !string.IsNullOrWhiteSpace(field.FieldName))
                .ToDictionary(field => field.FieldName, field => field, StringComparer.OrdinalIgnoreCase);
            var selectedFields = SelectedFieldNames;
            var rebuiltFields = entityDefinition.CreateFieldDefinitions();

            foreach (var field in rebuiltFields)
            {
                if (!existingFields.TryGetValue(field.FieldName, out var existingField))
                {
                    field.IsVisible = selectedFields.Contains(field.FieldName);
                    continue;
                }

                field.Label = string.IsNullOrWhiteSpace(existingField.Label) ? field.Label : existingField.Label;
                field.EditorKey = string.IsNullOrWhiteSpace(existingField.EditorKey) ? field.EditorKey : existingField.EditorKey;
                field.Order = existingField.Order;
                field.Width = existingField.Width > 0 ? existingField.Width : field.Width;
                field.IsVisible = selectedFields.Contains(field.FieldName);
                field.IsReadOnly = existingField.IsReadOnly || field.IsReadOnly;

                if (existingField.Options != null && existingField.Options.Count > 0)
                {
                    field.Options = existingField.Options.Select(option => option.Clone()).ToList();
                }
            }

            definition.Fields = rebuiltFields;
            return definition;
        }

        private void InitializeUi()
        {
            Text = "BeepBlock Setup Wizard";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Size = new Size(760, 540);

            _stepTitle.Location = new Point(16, 12);
            _stepTitle.Size = new Size(700, 28);
            _stepTitle.Font = new Font(Font, FontStyle.Bold);

            _stepSubtitle.Location = new Point(16, 40);
            _stepSubtitle.Size = new Size(700, 40);

            _contentPanel.Location = new Point(16, 88);
            _contentPanel.Size = new Size(712, 360);
            _contentPanel.BorderStyle = BorderStyle.FixedSingle;

            _backButton.Text = "< Back";
            _backButton.Location = new Point(456, 466);
            _backButton.Size = new Size(84, 30);
            _backButton.Click += (_, _) => MoveBack();

            _nextButton.Text = "Next >";
            _nextButton.Location = new Point(546, 466);
            _nextButton.Size = new Size(84, 30);
            _nextButton.Click += (_, _) => MoveNext();

            _cancelButton.Text = "Cancel";
            _cancelButton.Location = new Point(636, 466);
            _cancelButton.Size = new Size(84, 30);
            _cancelButton.Click += (_, _) => Close();

            BuildConnectionStep();
            BuildEntityStep();
            BuildFieldsStep();
            BuildViewStep();

            _contentPanel.Controls.Add(_stepConnectionPanel);
            _contentPanel.Controls.Add(_stepEntityPanel);
            _contentPanel.Controls.Add(_stepFieldsPanel);
            _contentPanel.Controls.Add(_stepViewPanel);

            Controls.Add(_stepTitle);
            Controls.Add(_stepSubtitle);
            Controls.Add(_contentPanel);
            Controls.Add(_backButton);
            Controls.Add(_nextButton);
            Controls.Add(_cancelButton);
        }

        private void BuildConnectionStep()
        {
            _stepConnectionPanel.Dock = DockStyle.Fill;

            var label = new Label
            {
                Text = "Choose a connection:",
                Location = new Point(20, 24),
                Size = new Size(320, 24)
            };

            _connectionCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            _connectionCombo.Location = new Point(20, 56);
            _connectionCombo.Size = new Size(420, 28);
            _connectionCombo.SelectedIndexChanged += (_, _) =>
            {
                LoadEntities(_connectionCombo.Text);
                LoadFields(_connectionCombo.Text, _entityCombo.Text);
            };

            _stepConnectionPanel.Controls.Add(label);
            _stepConnectionPanel.Controls.Add(_connectionCombo);
        }

        private void BuildEntityStep()
        {
            _stepEntityPanel.Dock = DockStyle.Fill;

            var label = new Label
            {
                Text = "Choose an entity/table:",
                Location = new Point(20, 24),
                Size = new Size(320, 24)
            };

            _entityCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            _entityCombo.Location = new Point(20, 56);
            _entityCombo.Size = new Size(420, 28);
            _entityCombo.SelectedIndexChanged += (_, _) => LoadFields(_connectionCombo.Text, _entityCombo.Text);

            _stepEntityPanel.Controls.Add(label);
            _stepEntityPanel.Controls.Add(_entityCombo);
        }

        private void BuildFieldsStep()
        {
            _stepFieldsPanel.Dock = DockStyle.Fill;

            var label = new Label
            {
                Text = "Select fields to surface in the block:",
                Location = new Point(20, 20),
                Size = new Size(360, 24)
            };

            _fieldsList.Location = new Point(20, 52);
            _fieldsList.Size = new Size(420, 240);
            _fieldsList.CheckOnClick = true;

            _selectAllFieldsButton.Text = "Select All";
            _selectAllFieldsButton.Location = new Point(460, 52);
            _selectAllFieldsButton.Size = new Size(120, 30);
            _selectAllFieldsButton.Click += (_, _) => SetAllFieldsChecked(true);

            _clearFieldsButton.Text = "Clear All";
            _clearFieldsButton.Location = new Point(460, 88);
            _clearFieldsButton.Size = new Size(120, 30);
            _clearFieldsButton.Click += (_, _) => SetAllFieldsChecked(false);

            _stepFieldsPanel.Controls.Add(label);
            _stepFieldsPanel.Controls.Add(_fieldsList);
            _stepFieldsPanel.Controls.Add(_selectAllFieldsButton);
            _stepFieldsPanel.Controls.Add(_clearFieldsButton);
        }

        private void BuildViewStep()
        {
            _stepViewPanel.Dock = DockStyle.Fill;

            var label = new Label
            {
                Text = "Choose how this block should render:",
                Location = new Point(20, 24),
                Size = new Size(360, 24)
            };

            _recordControlsRadio.Text = "Record Controls";
            _recordControlsRadio.Location = new Point(20, 60);
            _recordControlsRadio.Size = new Size(200, 24);
            _recordControlsRadio.Checked = true;
            _recordControlsRadio.CheckedChanged += (_, _) => UpdateSummary();

            _gridRadio.Text = "Grid";
            _gridRadio.Location = new Point(20, 92);
            _gridRadio.Size = new Size(200, 24);
            _gridRadio.CheckedChanged += (_, _) => UpdateSummary();

            _summaryLabel.Location = new Point(20, 140);
            _summaryLabel.Size = new Size(640, 160);

            _stepViewPanel.Controls.Add(label);
            _stepViewPanel.Controls.Add(_recordControlsRadio);
            _stepViewPanel.Controls.Add(_gridRadio);
            _stepViewPanel.Controls.Add(_summaryLabel);
        }

        private void LoadInitialValues()
        {
            var connections = GetAvailableConnections();
            _connectionCombo.Items.Clear();
            _connectionCombo.Items.AddRange(connections.Cast<object>().ToArray());

            string preferredConnection = _baselineEntity.ConnectionName;
            if (!string.IsNullOrWhiteSpace(preferredConnection) && !_connectionCombo.Items.Contains(preferredConnection))
            {
                _connectionCombo.Items.Add(preferredConnection);
            }

            if (_connectionCombo.Items.Count > 0)
            {
                _connectionCombo.SelectedItem = _connectionCombo.Items.Contains(preferredConnection)
                    ? preferredConnection
                    : _connectionCombo.Items[0];
            }

            string preferredEntity = _baselineEntity.EntityName;
            LoadEntities(SelectedConnectionName);
            if (!string.IsNullOrWhiteSpace(preferredEntity) && !_entityCombo.Items.Contains(preferredEntity))
            {
                _entityCombo.Items.Add(preferredEntity);
            }

            if (_entityCombo.Items.Count > 0)
            {
                _entityCombo.SelectedItem = _entityCombo.Items.Contains(preferredEntity)
                    ? preferredEntity
                    : _entityCombo.Items[0];
            }

            LoadFields(SelectedConnectionName, SelectedEntityName);
            if (_baselineDefinition.PresentationMode == BeepBlockPresentationMode.Grid)
            {
                _gridRadio.Checked = true;
            }

            UpdateSummary();
        }

        private List<string> GetAvailableConnections()
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (_block.Site?.Container != null)
            {
                foreach (IComponent component in _block.Site.Container.Components)
                {
                    if (component is BeepDataConnection dataConnection)
                    {
                        foreach (var connection in dataConnection.GetConnectionsSnapshot(includeRepository: false))
                        {
                            if (!string.IsNullOrWhiteSpace(connection.ConnectionName))
                            {
                                names.Add(connection.ConnectionName);
                            }
                        }
                    }
                }
            }

            var editorConnections = _editor?.ConfigEditor?.LoadDataConnectionsValues() ?? _editor?.ConfigEditor?.DataConnections;
            if (editorConnections != null)
            {
                foreach (var connection in editorConnections)
                {
                    if (!string.IsNullOrWhiteSpace(connection?.ConnectionName))
                    {
                        names.Add(connection.ConnectionName);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(_baselineEntity.ConnectionName))
            {
                names.Add(_baselineEntity.ConnectionName);
            }

            return names.OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToList();
        }

        private void LoadEntities(string connectionName)
        {
            _entityCombo.Items.Clear();
            foreach (string entityName in GetEntityNames(connectionName))
            {
                _entityCombo.Items.Add(entityName);
            }
        }

        private List<string> GetEntityNames(string connectionName)
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (_editor != null && !string.IsNullOrWhiteSpace(connectionName))
            {
                try
                {
                    _editor.OpenDataSource(connectionName);
                    var dataSource = _editor.GetDataSource(connectionName);
                    var entities = dataSource?.GetEntitesList();
                    if (entities != null)
                    {
                        foreach (var entity in entities)
                        {
                            if (!string.IsNullOrWhiteSpace(entity))
                            {
                                names.Add(entity);
                            }
                        }
                    }

                    if (names.Count == 0 && dataSource?.Entities != null)
                    {
                        foreach (var entity in dataSource.Entities)
                        {
                            if (!string.IsNullOrWhiteSpace(entity?.EntityName))
                            {
                                names.Add(entity.EntityName);
                            }
                        }
                    }
                }
                catch
                {
                    // Keep the wizard alive when design-time metadata access is unavailable.
                }
            }

            if (!string.IsNullOrWhiteSpace(_baselineEntity.EntityName))
            {
                names.Add(_baselineEntity.EntityName);
            }

            return names.OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToList();
        }

        private void LoadFields(string connectionName, string entityName)
        {
            _fieldsList.Items.Clear();
            _metadataFieldsByName.Clear();

            var selectedFields = _baselineDefinition.Fields
                .Where(field => field != null && field.IsVisible && !string.IsNullOrWhiteSpace(field.FieldName))
                .Select(field => field.FieldName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var orderedFieldNames = new List<string>();

            foreach (var field in GetEntityFields(connectionName, entityName))
            {
                if (string.IsNullOrWhiteSpace(field?.FieldName))
                {
                    continue;
                }

                _metadataFieldsByName[field.FieldName] = field;
                orderedFieldNames.Add(field.FieldName);
            }

            if (orderedFieldNames.Count == 0)
            {
                orderedFieldNames.AddRange(_baselineEntity.Fields
                    .Where(field => field != null && !string.IsNullOrWhiteSpace(field.FieldName))
                    .OrderBy(field => field.Order)
                    .Select(field => field.FieldName));
            }

            if (selectedFields.Count == 0)
            {
                selectedFields = orderedFieldNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
            }

            foreach (string fieldName in orderedFieldNames.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                _fieldsList.Items.Add(fieldName, selectedFields.Contains(fieldName));
            }
        }

        private List<EntityField> GetEntityFields(string connectionName, string entityName)
        {
            if (_editor == null || string.IsNullOrWhiteSpace(connectionName) || string.IsNullOrWhiteSpace(entityName))
            {
                return new List<EntityField>();
            }

            try
            {
                _editor.OpenDataSource(connectionName);
                var dataSource = _editor.GetDataSource(connectionName);
                var structure = dataSource?.GetEntityStructure(entityName, true) ?? dataSource?.GetEntityStructure(entityName, false);
                return structure?.Fields?.ToList() ?? new List<EntityField>();
            }
            catch
            {
                return new List<EntityField>();
            }
        }

        private BeepBlockEntityDefinition BuildEntityDefinition()
        {
            if (_metadataFieldsByName.Count == 0 &&
                string.Equals(SelectedEntityName, _baselineEntity.EntityName, StringComparison.OrdinalIgnoreCase))
            {
                var baselineClone = _baselineEntity.Clone();
                baselineClone.ConnectionName = SelectedConnectionName;
                baselineClone.EntityName = SelectedEntityName;
                baselineClone.DatasourceEntityName = string.IsNullOrWhiteSpace(baselineClone.DatasourceEntityName)
                    ? SelectedEntityName
                    : baselineClone.DatasourceEntityName;
                baselineClone.Caption = string.IsNullOrWhiteSpace(baselineClone.Caption)
                    ? SelectedEntityName
                    : baselineClone.Caption;
                return baselineClone;
            }

            var entityDefinition = new BeepBlockEntityDefinition
            {
                ConnectionName = SelectedConnectionName,
                EntityName = SelectedEntityName,
                DatasourceEntityName = SelectedEntityName,
                Caption = string.IsNullOrWhiteSpace(_baselineEntity.Caption) ? SelectedEntityName : _baselineEntity.Caption,
                Description = _baselineEntity.Description,
                DataSourceId = _baselineEntity.DataSourceId,
                IsMasterBlock = _baselineEntity.IsMasterBlock,
                MasterBlockName = _baselineEntity.MasterBlockName,
                MasterKeyField = _baselineEntity.MasterKeyField,
                ForeignKeyField = _baselineEntity.ForeignKeyField
            };

            foreach (var field in _metadataFieldsByName.Values.OrderBy(field => field.OrdinalPosition))
            {
                entityDefinition.Fields.Add(new BeepBlockEntityFieldDefinition
                {
                    FieldName = field.FieldName ?? string.Empty,
                    Label = string.IsNullOrWhiteSpace(field.Caption) ? field.FieldName ?? string.Empty : field.Caption,
                    Description = field.Description ?? string.Empty,
                    DataType = field.Fieldtype ?? string.Empty,
                    Category = field.FieldCategory,
                    Order = field.OrdinalPosition,
                    Size = field.Size,
                    NumericPrecision = field.NumericPrecision,
                    NumericScale = field.NumericScale,
                    IsRequired = field.IsRequired,
                    AllowDBNull = field.AllowDBNull,
                    IsPrimaryKey = field.IsKey,
                    IsUnique = field.IsUnique,
                    IsIndexed = field.IsIndexed,
                    IsAutoIncrement = field.IsAutoIncrement,
                    IsReadOnly = field.IsReadOnly,
                    IsCheck = field.IsCheck
                });
            }

            if (entityDefinition.Fields.Count == 0)
            {
                return _baselineEntity.Clone();
            }

            return entityDefinition;
        }

        private void SetAllFieldsChecked(bool isChecked)
        {
            for (int index = 0; index < _fieldsList.Items.Count; index++)
            {
                _fieldsList.SetItemChecked(index, isChecked);
            }
        }

        private void MoveBack()
        {
            if (_currentStepIndex <= 0)
            {
                return;
            }

            _currentStepIndex--;
            UpdateStepUi();
        }

        private void MoveNext()
        {
            if (!ValidateCurrentStep())
            {
                return;
            }

            if (_currentStepIndex == 3)
            {
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            _currentStepIndex++;
            UpdateStepUi();
        }

        private bool ValidateCurrentStep()
        {
            if (_currentStepIndex == 0 && string.IsNullOrWhiteSpace(SelectedConnectionName))
            {
                MessageBox.Show("Please select a connection.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_currentStepIndex == 1 && string.IsNullOrWhiteSpace(SelectedEntityName))
            {
                MessageBox.Show("Please select an entity/table.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_currentStepIndex == 2 && SelectedFieldNames.Count == 0)
            {
                MessageBox.Show("Select at least one field to display.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void UpdateStepUi()
        {
            _stepConnectionPanel.Visible = _currentStepIndex == 0;
            _stepEntityPanel.Visible = _currentStepIndex == 1;
            _stepFieldsPanel.Visible = _currentStepIndex == 2;
            _stepViewPanel.Visible = _currentStepIndex == 3;

            _backButton.Enabled = _currentStepIndex > 0;
            _nextButton.Text = _currentStepIndex == 3 ? "Finish" : "Next >";

            switch (_currentStepIndex)
            {
                case 0:
                    _stepTitle.Text = "Step 1 of 4: Select Connection";
                    _stepSubtitle.Text = "Choose the data connection this block should use for its entity snapshot.";
                    break;
                case 1:
                    _stepTitle.Text = "Step 2 of 4: Select Entity";
                    _stepSubtitle.Text = "Pick the entity/table whose schema will be stored in the block definition.";
                    break;
                case 2:
                    _stepTitle.Text = "Step 3 of 4: Select Visible Fields";
                    _stepSubtitle.Text = "Choose which fields should be visible when the block renders.";
                    break;
                case 3:
                    _stepTitle.Text = "Step 4 of 4: Choose Presentation Mode";
                    _stepSubtitle.Text = "Pick record or grid presentation for the rebuilt block definition.";
                    UpdateSummary();
                    break;
            }
        }

        private void UpdateSummary()
        {
            string viewLabel = SelectedPresentationMode == BeepBlockPresentationMode.Grid ? "Grid" : "Record Controls";
            _summaryLabel.Text =
                $"Connection: {SelectedConnectionName}\n" +
                $"Entity: {SelectedEntityName}\n" +
                $"Visible Fields: {SelectedFieldNames.Count}\n" +
                $"Presentation: {viewLabel}";
        }
    }
}