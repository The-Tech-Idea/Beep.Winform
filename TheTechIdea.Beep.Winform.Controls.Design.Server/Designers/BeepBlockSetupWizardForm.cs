using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;
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
        private readonly RadioButton _designerGeneratedRadio = new();
        private readonly Label _layoutModeLabel = new();
        private readonly ComboBox _layoutModeCombo = new();
        private readonly Label _layoutPreviewLabel = new();
        private readonly Panel _layoutPreviewPanel = new();
        private readonly Label _summaryLabel = new();

        private readonly Button _backButton = new();
        private readonly Button _nextButton = new();
        private readonly Button _cancelButton = new();
        private readonly Button _selectAllFieldsButton = new();
        private readonly Button _clearFieldsButton = new();
        private readonly Button _defaultsButton = new();

        private readonly Dictionary<string, EntityField> _metadataFieldsByName = new(StringComparer.OrdinalIgnoreCase);
        private string _loadedFieldsConnectionName = string.Empty;
        private string _loadedFieldsEntityName = string.Empty;
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
            _designerGeneratedRadio.Checked
                ? BeepBlockPresentationMode.DesignerGenerated
                : _gridRadio.Checked ? BeepBlockPresentationMode.Grid : BeepBlockPresentationMode.Record;

        public BeepBlockFieldControlsLayoutMode SelectedFieldControlsLayoutMode =>
            _layoutModeCombo.SelectedItem is BeepBlockFieldControlsLayoutMode mode
                ? mode
                : BeepBlockFieldControlsLayoutMode.StackedVertical;

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
            definition.ManagerBlockName = string.IsNullOrWhiteSpace(definition.ManagerBlockName)
                ? resolvedBlockName
                : definition.ManagerBlockName;
            definition.Caption = string.IsNullOrWhiteSpace(definition.Caption)
                ? (string.IsNullOrWhiteSpace(SelectedEntityName) ? resolvedBlockName : SelectedEntityName)
                : definition.Caption;
            definition.PresentationMode = SelectedPresentationMode;
            if (SelectedPresentationMode == BeepBlockPresentationMode.DesignerGenerated)
            {
                BeepBlockFieldControlsLayoutModeHelper.Apply(definition, SelectedFieldControlsLayoutMode);
            }
            else
            {
                BeepBlockFieldControlsLayoutModeHelper.Clear(definition);
            }

            BeepBlockEntityDefinition entityDefinition = BuildEntityDefinition();
            definition.Entity = entityDefinition;

            var existingFields = definition.Fields
                .Where(field => field != null && !string.IsNullOrWhiteSpace(field.FieldName))
                .ToDictionary(field => field.FieldName, field => field, StringComparer.OrdinalIgnoreCase);
            var selectedFields = SelectedFieldNames;
            bool matchesBaselineEntity = string.Equals(entityDefinition.ConnectionName, _baselineEntity.ConnectionName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(entityDefinition.EntityName, _baselineEntity.EntityName, StringComparison.OrdinalIgnoreCase);
            var rebuiltFields = entityDefinition.CreateFieldDefinitions();
            var resolvedFields = new List<BeepFieldDefinition>(rebuiltFields.Count);

            foreach (var field in rebuiltFields)
            {
                if (matchesBaselineEntity &&
                    existingFields.Count > 0 &&
                    !existingFields.ContainsKey(field.FieldName) &&
                    !selectedFields.Contains(field.FieldName))
                {
                    continue;
                }

                if (!existingFields.TryGetValue(field.FieldName, out var existingField))
                {
                    field.IsVisible = selectedFields.Contains(field.FieldName);
                    resolvedFields.Add(field);
                    continue;
                }

                field.Label = string.IsNullOrWhiteSpace(existingField.Label) ? field.Label : existingField.Label;
                field.EditorKey = string.IsNullOrWhiteSpace(existingField.EditorKey) ? field.EditorKey : existingField.EditorKey;
                field.ControlType = string.IsNullOrWhiteSpace(existingField.ControlType) ? field.ControlType : existingField.ControlType;
                field.BindingProperty = string.IsNullOrWhiteSpace(existingField.BindingProperty) ? field.BindingProperty : existingField.BindingProperty;
                field.Order = existingField.Order;
                field.Width = existingField.Width > 0 ? existingField.Width : field.Width;
                field.IsVisible = selectedFields.Contains(field.FieldName);
                field.IsReadOnly = existingField.IsReadOnly || field.IsReadOnly;

                if (existingField.Options != null && existingField.Options.Count > 0)
                {
                    field.Options = existingField.Options.Select(option => option.Clone()).ToList();
                }

                resolvedFields.Add(field);
            }

            definition.Fields = resolvedFields;
            BeepBlockFieldDefinitionStateHelper.UpdateExplicitFieldState(definition, treatEmptyAsExplicit: true);
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

            _defaultsButton.Text = "Default Policy...";
            _defaultsButton.Location = new Point(460, 124);
            _defaultsButton.Size = new Size(140, 30);
            _defaultsButton.Click += (_, _) => OpenDefaultPolicyEditor();

            _stepFieldsPanel.Controls.Add(label);
            _stepFieldsPanel.Controls.Add(_fieldsList);
            _stepFieldsPanel.Controls.Add(_selectAllFieldsButton);
            _stepFieldsPanel.Controls.Add(_clearFieldsButton);
            _stepFieldsPanel.Controls.Add(_defaultsButton);
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
            _recordControlsRadio.CheckedChanged += (_, _) => UpdatePreviewState();

            _gridRadio.Text = "Grid";
            _gridRadio.Location = new Point(20, 92);
            _gridRadio.Size = new Size(200, 24);
            _gridRadio.CheckedChanged += (_, _) => UpdatePreviewState();

            _designerGeneratedRadio.Text = "Designer Generated Controls";
            _designerGeneratedRadio.Location = new Point(20, 124);
            _designerGeneratedRadio.Size = new Size(240, 24);
            _designerGeneratedRadio.CheckedChanged += (_, _) => UpdateLayoutModeState();

            _layoutModeLabel.Text = "Field Controls Layout:";
            _layoutModeLabel.Location = new Point(44, 158);
            _layoutModeLabel.Size = new Size(140, 24);

            _layoutModeCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            _layoutModeCombo.FormattingEnabled = true;
            _layoutModeCombo.Location = new Point(188, 156);
            _layoutModeCombo.Size = new Size(220, 28);
            _layoutModeCombo.DataSource = BeepBlockFieldControlsLayoutModeHelper.GetAvailableModes().ToArray();
            _layoutModeCombo.Format += (_, e) =>
            {
                if (e.ListItem is BeepBlockFieldControlsLayoutMode layoutMode)
                {
                    e.Value = BeepBlockFieldControlsLayoutModeHelper.GetDisplayName(layoutMode);
                }
            };
            _layoutModeCombo.SelectedIndexChanged += (_, _) => UpdatePreviewState();

            _layoutPreviewLabel.Text = "Layout Preview";
            _layoutPreviewLabel.Location = new Point(452, 24);
            _layoutPreviewLabel.Size = new Size(236, 24);
            _layoutPreviewLabel.Font = new Font(Font, FontStyle.Bold);

            _layoutPreviewPanel.Location = new Point(452, 52);
            _layoutPreviewPanel.Size = new Size(236, 170);
            _layoutPreviewPanel.BorderStyle = BorderStyle.FixedSingle;
            _layoutPreviewPanel.BackColor = Color.White;
            _layoutPreviewPanel.Paint += (_, e) => DrawLayoutPreview(e.Graphics, _layoutPreviewPanel.ClientRectangle);

            _summaryLabel.Location = new Point(20, 236);
            _summaryLabel.Size = new Size(668, 104);

            _stepViewPanel.Controls.Add(label);
            _stepViewPanel.Controls.Add(_recordControlsRadio);
            _stepViewPanel.Controls.Add(_gridRadio);
            _stepViewPanel.Controls.Add(_designerGeneratedRadio);
            _stepViewPanel.Controls.Add(_layoutModeLabel);
            _stepViewPanel.Controls.Add(_layoutModeCombo);
            _stepViewPanel.Controls.Add(_layoutPreviewLabel);
            _stepViewPanel.Controls.Add(_layoutPreviewPanel);
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
            _layoutModeCombo.SelectedItem = BeepBlockFieldControlsLayoutModeHelper.Resolve(_baselineDefinition);

            switch (_baselineDefinition.PresentationMode)
            {
                case BeepBlockPresentationMode.Grid:
                    _gridRadio.Checked = true;
                    break;
                case BeepBlockPresentationMode.DesignerGenerated:
                    _designerGeneratedRadio.Checked = true;
                    break;
                default:
                    _recordControlsRadio.Checked = true;
                    break;
            }

            UpdateLayoutModeState();
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

        private void OpenDefaultPolicyEditor()
        {
            using var dialog = new BeepFieldControlTypePolicyEditorForm();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                MessageBox.Show(
                    this,
                    "Field default policy saved. Finishing this wizard will use the updated defaults when it rebuilds the block field definitions.",
                    "Field Default Policy",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
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
            bool preserveCurrentSelection = _fieldsList.Items.Count > 0 &&
                string.Equals(connectionName, _loadedFieldsConnectionName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(entityName, _loadedFieldsEntityName, StringComparison.OrdinalIgnoreCase);
            var currentSelectedFields = preserveCurrentSelection
                ? SelectedFieldNames
                : new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            _fieldsList.Items.Clear();
            _metadataFieldsByName.Clear();

            bool matchesBaselineEntity = string.Equals(connectionName, _baselineEntity.ConnectionName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(entityName, _baselineEntity.EntityName, StringComparison.OrdinalIgnoreCase);
            bool preserveExplicitEmptyBaseline = matchesBaselineEntity
                && BeepBlockFieldDefinitionStateHelper.IsExplicitlyEmpty(_baselineDefinition);
            var selectedFields = preserveCurrentSelection
                ? currentSelectedFields
                : matchesBaselineEntity
                    ? _baselineDefinition.Fields
                        .Where(field => field != null && field.IsVisible && !string.IsNullOrWhiteSpace(field.FieldName))
                        .Select(field => field.FieldName)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase)
                    : new HashSet<string>(StringComparer.OrdinalIgnoreCase);

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

            if (!preserveCurrentSelection && selectedFields.Count == 0 && !preserveExplicitEmptyBaseline)
            {
                selectedFields = orderedFieldNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
            }

            foreach (string fieldName in orderedFieldNames.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                _fieldsList.Items.Add(fieldName, selectedFields.Contains(fieldName));
            }

            _loadedFieldsConnectionName = connectionName ?? string.Empty;
            _loadedFieldsEntityName = entityName ?? string.Empty;
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
                    IsCheck = field.IsCheck,
                    IsIdentity = field.IsIdentity,
                    IsHidden = field.IsHidden,
                    IsLong = field.IsLong,
                    IsRowVersion = field.IsRowVersion,
                    DefaultValue = field.DefaultValue ?? string.Empty
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
                    _stepSubtitle.Text = "Pick record, grid, or designer-generated presentation for the rebuilt block definition.";
                    UpdateSummary();
                    break;
            }
        }

        private void UpdateLayoutModeState()
        {
            bool isDesignerGenerated = _designerGeneratedRadio.Checked;
            _layoutModeLabel.Enabled = isDesignerGenerated;
            _layoutModeCombo.Enabled = isDesignerGenerated;
            UpdatePreviewState();
        }

        private void UpdatePreviewState()
        {
            UpdateSummary();
            _layoutPreviewPanel.Invalidate();
        }

        private void UpdateSummary()
        {
            string viewLabel = SelectedPresentationMode switch
            {
                BeepBlockPresentationMode.Grid => "Grid",
                BeepBlockPresentationMode.DesignerGenerated => "Designer Generated",
                _ => "Record Controls"
            };

            string layoutSummary = SelectedPresentationMode == BeepBlockPresentationMode.DesignerGenerated
                ? $"{BeepBlockFieldControlsLayoutModeHelper.GetDisplayName(SelectedFieldControlsLayoutMode)} - {BeepBlockFieldControlsLayoutModeHelper.GetDescription(SelectedFieldControlsLayoutMode)}"
                : "Used only when the block runs in Designer Generated mode.";

            _summaryLabel.Text =
                $"Connection: {SelectedConnectionName}\n" +
                $"Entity: {SelectedEntityName}\n" +
                $"Visible Fields: {SelectedFieldNames.Count}\n" +
                $"Presentation: {viewLabel}\n" +
                $"Field Controls Layout: {layoutSummary}";
        }

        private void DrawLayoutPreview(Graphics graphics, Rectangle bounds)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.Clear(Color.White);

            Rectangle canvas = Rectangle.Inflate(bounds, -8, -8);
            if (canvas.Width <= 0 || canvas.Height <= 0)
            {
                return;
            }

            using var borderPen = new Pen(Color.FromArgb(198, 206, 216));
            using var accentBrush = new SolidBrush(Color.FromArgb(56, 118, 255));
            using var mutedBrush = new SolidBrush(Color.FromArgb(236, 240, 245));
            using var fieldBrush = new SolidBrush(Color.FromArgb(248, 250, 252));
            using var textBrush = new SolidBrush(Color.FromArgb(54, 65, 79));
            using var textFont = new Font(Font.FontFamily, 7.5f, FontStyle.Regular);
            using var captionFont = new Font(Font.FontFamily, 8f, FontStyle.Bold);

            graphics.FillRectangle(mutedBrush, canvas);
            graphics.DrawRectangle(borderPen, canvas);

            Rectangle headerRect = new Rectangle(canvas.Left, canvas.Top, canvas.Width, 18);
            graphics.FillRectangle(accentBrush, headerRect);
            graphics.DrawString(GetPreviewCaption(), captionFont, Brushes.White, headerRect.Left + 6, headerRect.Top + 3);

            Rectangle bodyRect = new Rectangle(canvas.Left + 8, headerRect.Bottom + 8, canvas.Width - 16, canvas.Height - 34);
            if (bodyRect.Width <= 0 || bodyRect.Height <= 0)
            {
                return;
            }

            switch (SelectedPresentationMode)
            {
                case BeepBlockPresentationMode.Grid:
                    DrawGridPreview(graphics, bodyRect, borderPen, fieldBrush, textBrush, textFont);
                    break;
                case BeepBlockPresentationMode.DesignerGenerated:
                    DrawDesignerGeneratedPreview(graphics, bodyRect, borderPen, fieldBrush, textBrush, textFont);
                    break;
                default:
                    DrawRecordPreview(graphics, bodyRect, borderPen, fieldBrush, textBrush, textFont);
                    break;
            }
        }

        private string GetPreviewCaption()
        {
            return SelectedPresentationMode switch
            {
                BeepBlockPresentationMode.Grid => "Grid Preview",
                BeepBlockPresentationMode.DesignerGenerated => BeepBlockFieldControlsLayoutModeHelper.GetDisplayName(SelectedFieldControlsLayoutMode),
                _ => "Record Preview"
            };
        }

        private static void DrawRecordPreview(Graphics graphics, Rectangle bodyRect, Pen borderPen, Brush fieldBrush, Brush textBrush, Font textFont)
        {
            const int rowCount = 3;
            int spacing = 6;
            int labelWidth = Math.Min(60, Math.Max(44, bodyRect.Width / 3));
            int rowHeight = Math.Max(18, (bodyRect.Height - (spacing * (rowCount - 1))) / rowCount);
            int editorHeight = Math.Max(14, Math.Min(22, rowHeight - 2));

            for (int row = 0; row < rowCount; row++)
            {
                int top = bodyRect.Top + (row * (rowHeight + spacing));
                int editorTop = top + Math.Max(0, (rowHeight - editorHeight) / 2);
                Rectangle labelRect = new Rectangle(bodyRect.Left, top + Math.Max(2, (rowHeight - 14) / 2), labelWidth, 14);
                Rectangle editorRect = new Rectangle(bodyRect.Left + labelWidth + 8, editorTop, Math.Max(40, bodyRect.Width - labelWidth - 8), editorHeight);

                graphics.DrawString($"Field {row + 1}", textFont, textBrush, labelRect);
                graphics.FillRectangle(fieldBrush, editorRect);
                graphics.DrawRectangle(borderPen, editorRect);
            }
        }

        private static void DrawGridPreview(Graphics graphics, Rectangle bodyRect, Pen borderPen, Brush fieldBrush, Brush textBrush, Font textFont)
        {
            int headerHeight = Math.Max(18, Math.Min(20, bodyRect.Height / 4));
            int rowHeight = Math.Max(14, (bodyRect.Height - headerHeight) / 3);
            int columnWidth = Math.Max(36, bodyRect.Width / 3);

            Rectangle headerRect = new Rectangle(bodyRect.Left, bodyRect.Top, bodyRect.Width, headerHeight);
            graphics.FillRectangle(fieldBrush, headerRect);
            graphics.DrawRectangle(borderPen, headerRect);

            for (int column = 0; column < 3; column++)
            {
                int left = bodyRect.Left + (column * columnWidth);
                Rectangle cellRect = new Rectangle(left, bodyRect.Top, columnWidth, headerHeight);
                graphics.DrawRectangle(borderPen, cellRect);
                graphics.DrawString($"Col {column + 1}", textFont, textBrush, left + 4, bodyRect.Top + 4);
            }

            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    int left = bodyRect.Left + (column * columnWidth);
                    int top = bodyRect.Top + headerHeight + (row * rowHeight);
                    Rectangle cellRect = new Rectangle(left, top, columnWidth, rowHeight);
                    graphics.FillRectangle(Brushes.White, cellRect);
                    graphics.DrawRectangle(borderPen, cellRect);
                }
            }
        }

        private void DrawDesignerGeneratedPreview(Graphics graphics, Rectangle bodyRect, Pen borderPen, Brush fieldBrush, Brush textBrush, Font textFont)
        {
            switch (SelectedFieldControlsLayoutMode)
            {
                case BeepBlockFieldControlsLayoutMode.LabelFieldPairs:
                    DrawLabelFieldPairsPreview(graphics, bodyRect, borderPen, fieldBrush, textBrush, textFont);
                    break;
                case BeepBlockFieldControlsLayoutMode.GridLayout:
                    DrawGridLayoutPreview(graphics, bodyRect, borderPen, fieldBrush, textBrush, textFont);
                    break;
                default:
                    DrawStackedVerticalPreview(graphics, bodyRect, borderPen, fieldBrush, textBrush, textFont);
                    break;
            }
        }

        private static void DrawStackedVerticalPreview(Graphics graphics, Rectangle bodyRect, Pen borderPen, Brush fieldBrush, Brush textBrush, Font textFont)
        {
            const int rowCount = 3;
            int labelHeight = 12;
            int innerSpacing = 4;
            int spacing = 6;
            int rowHeight = Math.Max(22, (bodyRect.Height - (spacing * (rowCount - 1))) / rowCount);
            int blockHeight = Math.Max(12, rowHeight - labelHeight - innerSpacing);

            for (int row = 0; row < rowCount; row++)
            {
                int top = bodyRect.Top + (row * (rowHeight + spacing));
                graphics.DrawString($"Field {row + 1}", textFont, textBrush, bodyRect.Left, top);
                Rectangle editorRect = new Rectangle(bodyRect.Left, top + labelHeight + innerSpacing, bodyRect.Width, blockHeight);
                graphics.FillRectangle(fieldBrush, editorRect);
                graphics.DrawRectangle(borderPen, editorRect);
            }
        }

        private static void DrawLabelFieldPairsPreview(Graphics graphics, Rectangle bodyRect, Pen borderPen, Brush fieldBrush, Brush textBrush, Font textFont)
        {
            const int rowCount = 3;
            int spacing = 6;
            int rowHeight = Math.Max(18, (bodyRect.Height - (spacing * (rowCount - 1))) / rowCount);
            int labelWidth = Math.Min(60, Math.Max(44, bodyRect.Width / 3));
            int editorHeight = Math.Max(14, Math.Min(22, rowHeight - 2));

            for (int row = 0; row < rowCount; row++)
            {
                int top = bodyRect.Top + (row * (rowHeight + spacing));
                int editorTop = top + Math.Max(0, (rowHeight - editorHeight) / 2);
                graphics.DrawString($"Field {row + 1}", textFont, textBrush, bodyRect.Left, top + Math.Max(2, (rowHeight - 14) / 2));
                Rectangle editorRect = new Rectangle(bodyRect.Left + labelWidth + 8, editorTop, Math.Max(36, bodyRect.Width - labelWidth - 8), editorHeight);
                graphics.FillRectangle(fieldBrush, editorRect);
                graphics.DrawRectangle(borderPen, editorRect);
            }
        }

        private static void DrawGridLayoutPreview(Graphics graphics, Rectangle bodyRect, Pen borderPen, Brush fieldBrush, Brush textBrush, Font textFont)
        {
            int gap = 10;
            int columnWidth = Math.Max(40, (bodyRect.Width - gap) / 2);
            int labelHeight = 12;
            int innerSpacing = 4;
            int rowHeight = Math.Max(26, (bodyRect.Height - gap) / 2);
            int editorHeight = Math.Max(14, Math.Min(22, rowHeight - labelHeight - innerSpacing));

            for (int index = 0; index < 4; index++)
            {
                int column = index % 2;
                int row = index / 2;
                int left = bodyRect.Left + (column * (columnWidth + gap));
                int top = bodyRect.Top + (row * (rowHeight + gap));
                graphics.DrawString($"Field {index + 1}", textFont, textBrush, left, top);
                Rectangle editorRect = new Rectangle(left, top + labelHeight + innerSpacing, columnWidth, editorHeight);
                graphics.FillRectangle(fieldBrush, editorRect);
                graphics.DrawRectangle(borderPen, editorRect);
            }
        }
    }
}