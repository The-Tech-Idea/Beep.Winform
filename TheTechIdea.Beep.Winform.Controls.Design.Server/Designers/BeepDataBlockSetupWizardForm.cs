using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    internal sealed class BeepDataBlockSetupWizardForm : Form
    {
        private readonly BeepDataBlock _dataBlock;

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
        private readonly ComboBox _fieldEditorTypeCombo = new();
        private readonly ComboBox _fieldTemplateCombo = new();
        private readonly Label _fieldEditorLabel = new();
        private readonly Label _fieldTemplateLabel = new();
        private readonly Label _fieldSelectionHintLabel = new();

        private int _currentStepIndex;
        private readonly Dictionary<string, string> _selectedEditorTypeByField = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _selectedTemplateByField = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, DbFieldCategory> _fieldCategoryByFieldName = new(StringComparer.OrdinalIgnoreCase);
        private string _lastSelectedFieldName = string.Empty;
        private bool _isUpdatingFieldSelectorUi;

        public string SelectedConnectionName => _connectionCombo.Text?.Trim() ?? string.Empty;
        public string SelectedEntityName => _entityCombo.Text?.Trim() ?? string.Empty;
        public HashSet<string> SelectedFieldNames =>
            _fieldsList.CheckedItems
                .Cast<object>()
                .Select(x => x?.ToString() ?? string.Empty)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

        public DataBlockViewMode SelectedViewMode =>
            _gridRadio.Checked ? DataBlockViewMode.BeepGridPro : DataBlockViewMode.RecordControls;

        public IReadOnlyDictionary<string, string> SelectedFieldEditorTypeByField => _selectedEditorTypeByField;
        public IReadOnlyDictionary<string, string> SelectedTemplateByField => _selectedTemplateByField;

        public BeepDataBlockSetupWizardForm(BeepDataBlock dataBlock)
        {
            _dataBlock = dataBlock ?? throw new ArgumentNullException(nameof(dataBlock));
            InitializeUi();
            LoadInitialValues();
            UpdateStepUi();
        }

        private void InitializeUi()
        {
            Text = "BeepDataBlock Setup Wizard";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Size = new Size(760, 560);

            _stepTitle.Location = new Point(16, 12);
            _stepTitle.Size = new Size(700, 28);
            _stepTitle.Font = new Font(Font, FontStyle.Bold);

            _stepSubtitle.Location = new Point(16, 40);
            _stepSubtitle.Size = new Size(700, 40);

            _contentPanel.Location = new Point(16, 88);
            _contentPanel.Size = new Size(712, 392);
            _contentPanel.BorderStyle = BorderStyle.FixedSingle;

            _backButton.Text = "< Back";
            _backButton.Location = new Point(456, 492);
            _backButton.Size = new Size(84, 30);
            _backButton.Click += (_, _) => MoveBack();

            _nextButton.Text = "Next >";
            _nextButton.Location = new Point(546, 492);
            _nextButton.Size = new Size(84, 30);
            _nextButton.Click += (_, _) => MoveNext();

            _cancelButton.Text = "Cancel";
            _cancelButton.Location = new Point(636, 492);
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
                Text = "Select fields to show:",
                Location = new Point(20, 20),
                Size = new Size(320, 24)
            };

            _fieldsList.Location = new Point(20, 52);
            _fieldsList.Size = new Size(420, 270);
            _fieldsList.CheckOnClick = true;
            _fieldsList.SelectedIndexChanged += (_, _) =>
            {
                SaveFieldEditorSelection(_lastSelectedFieldName);
                SaveFieldTemplateSelection(_lastSelectedFieldName);
                SyncFieldEditorSelectorsFromFieldSelection();
            };

            _fieldSelectionHintLabel.Text = "Per-field editor/template";
            _fieldSelectionHintLabel.Location = new Point(458, 52);
            _fieldSelectionHintLabel.Size = new Size(220, 24);

            _fieldEditorLabel.Text = "Editor Type:";
            _fieldEditorLabel.Location = new Point(458, 86);
            _fieldEditorLabel.Size = new Size(220, 22);
            _fieldEditorTypeCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            _fieldEditorTypeCombo.Location = new Point(458, 110);
            _fieldEditorTypeCombo.Size = new Size(220, 28);
            _fieldEditorTypeCombo.SelectedIndexChanged += (_, _) => SaveCurrentFieldEditorSelection();

            _fieldTemplateLabel.Text = "Template:";
            _fieldTemplateLabel.Location = new Point(458, 150);
            _fieldTemplateLabel.Size = new Size(220, 22);
            _fieldTemplateCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            _fieldTemplateCombo.Location = new Point(458, 174);
            _fieldTemplateCombo.Size = new Size(220, 28);
            _fieldTemplateCombo.SelectedIndexChanged += (_, _) => SaveCurrentFieldTemplateSelection();

            _selectAllFieldsButton.Text = "Select All";
            _selectAllFieldsButton.Location = new Point(20, 334);
            _selectAllFieldsButton.Size = new Size(100, 30);
            _selectAllFieldsButton.Click += (_, _) =>
            {
                for (int i = 0; i < _fieldsList.Items.Count; i++)
                {
                    _fieldsList.SetItemChecked(i, true);
                }
            };

            _clearFieldsButton.Text = "Clear";
            _clearFieldsButton.Location = new Point(126, 334);
            _clearFieldsButton.Size = new Size(100, 30);
            _clearFieldsButton.Click += (_, _) =>
            {
                for (int i = 0; i < _fieldsList.Items.Count; i++)
                {
                    _fieldsList.SetItemChecked(i, false);
                }
            };

            _stepFieldsPanel.Controls.Add(label);
            _stepFieldsPanel.Controls.Add(_fieldsList);
            _stepFieldsPanel.Controls.Add(_fieldSelectionHintLabel);
            _stepFieldsPanel.Controls.Add(_fieldEditorLabel);
            _stepFieldsPanel.Controls.Add(_fieldEditorTypeCombo);
            _stepFieldsPanel.Controls.Add(_fieldTemplateLabel);
            _stepFieldsPanel.Controls.Add(_fieldTemplateCombo);
            _stepFieldsPanel.Controls.Add(_selectAllFieldsButton);
            _stepFieldsPanel.Controls.Add(_clearFieldsButton);
        }

        private void BuildViewStep()
        {
            _stepViewPanel.Dock = DockStyle.Fill;

            var label = new Label
            {
                Text = "Choose how to render records:",
                Location = new Point(20, 24),
                Size = new Size(360, 24)
            };

            _recordControlsRadio.Text = "Record Controls (Beep controls per field)";
            _recordControlsRadio.Location = new Point(20, 60);
            _recordControlsRadio.Size = new Size(380, 30);

            _gridRadio.Text = "BeepGridPro (tabular grid view)";
            _gridRadio.Location = new Point(20, 94);
            _gridRadio.Size = new Size(380, 30);

            _summaryLabel.Location = new Point(20, 150);
            _summaryLabel.Size = new Size(660, 200);

            _stepViewPanel.Controls.Add(label);
            _stepViewPanel.Controls.Add(_recordControlsRadio);
            _stepViewPanel.Controls.Add(_gridRadio);
            _stepViewPanel.Controls.Add(_summaryLabel);
        }

        private void LoadInitialValues()
        {
            ResetFieldSelectionOptions();

            var connections = GetAvailableConnections();
            _connectionCombo.Items.Clear();
            _connectionCombo.Items.AddRange(connections.Cast<object>().ToArray());

            var preferredConnection = string.IsNullOrWhiteSpace(_dataBlock.ConnectionName)
                ? _dataBlock.QueryDataSourceName
                : _dataBlock.ConnectionName;

            if (!string.IsNullOrWhiteSpace(preferredConnection) && _connectionCombo.Items.Contains(preferredConnection))
            {
                _connectionCombo.SelectedItem = preferredConnection;
            }
            else if (_connectionCombo.Items.Count > 0)
            {
                _connectionCombo.SelectedIndex = 0;
            }

            LoadEntities(_connectionCombo.Text);

            if (!string.IsNullOrWhiteSpace(_dataBlock.EntityName) && _entityCombo.Items.Contains(_dataBlock.EntityName))
            {
                _entityCombo.SelectedItem = _dataBlock.EntityName;
            }
            else if (_entityCombo.Items.Count > 0)
            {
                _entityCombo.SelectedIndex = 0;
            }

            LoadFields(_connectionCombo.Text, _entityCombo.Text);

            if (_dataBlock.ViewMode == DataBlockViewMode.BeepGridPro)
            {
                _gridRadio.Checked = true;
            }
            else
            {
                _recordControlsRadio.Checked = true;
            }
        }

        private void ResetFieldSelectionOptions()
        {
            _isUpdatingFieldSelectorUi = true;
            _fieldEditorTypeCombo.Items.Clear();
            _fieldEditorTypeCombo.Items.Add("<Default>");
            _fieldEditorTypeCombo.SelectedIndex = 0;
            _fieldTemplateCombo.Items.Clear();
            _fieldTemplateCombo.Items.Add("<None>");
            _fieldTemplateCombo.SelectedIndex = 0;
            _isUpdatingFieldSelectorUi = false;
        }

        private void LoadEditorTypeOptions(DbFieldCategory? category, string selectedValue)
        {
            _isUpdatingFieldSelectorUi = true;
            _fieldEditorTypeCombo.Items.Clear();
            _fieldEditorTypeCombo.Items.Add("<Default>");

            var availableEditorTypes = category.HasValue
                ? _dataBlock.GetKnownRecordEditorTypes(category.Value)
                : _dataBlock.GetKnownRecordEditorTypes();

            foreach (var type in availableEditorTypes)
            {
                if (!string.IsNullOrWhiteSpace(type.FullName))
                {
                    _fieldEditorTypeCombo.Items.Add(type.FullName);
                }
            }

            if (!string.IsNullOrWhiteSpace(selectedValue) &&
                !string.Equals(selectedValue, "<Default>", StringComparison.OrdinalIgnoreCase) &&
                !_fieldEditorTypeCombo.Items.Contains(selectedValue))
            {
                _fieldEditorTypeCombo.Items.Add(selectedValue);
            }

            _fieldEditorTypeCombo.SelectedItem = _fieldEditorTypeCombo.Items.Contains(selectedValue)
                ? selectedValue
                : "<Default>";
            _isUpdatingFieldSelectorUi = false;
        }

        private void LoadTemplateOptions(DbFieldCategory? category, string selectedValue)
        {
            _isUpdatingFieldSelectorUi = true;
            _fieldTemplateCombo.Items.Clear();
            _fieldTemplateCombo.Items.Add("<None>");

            var templates = category.HasValue
                ? _dataBlock.GetEditorTemplates(category.Value)
                : _dataBlock.EditorTemplates.ToList();

            foreach (var template in templates)
            {
                if (!string.IsNullOrWhiteSpace(template.TemplateId))
                {
                    _fieldTemplateCombo.Items.Add(template.TemplateId);
                }
            }

            if (!string.IsNullOrWhiteSpace(selectedValue) &&
                !string.Equals(selectedValue, "<None>", StringComparison.OrdinalIgnoreCase) &&
                !_fieldTemplateCombo.Items.Contains(selectedValue))
            {
                _fieldTemplateCombo.Items.Add(selectedValue);
            }

            _fieldTemplateCombo.SelectedItem = _fieldTemplateCombo.Items.Contains(selectedValue)
                ? selectedValue
                : "<None>";
            _isUpdatingFieldSelectorUi = false;
        }

        private List<string> GetAvailableConnections()
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var container = _dataBlock.Site?.Container;
            if (container != null)
            {
                foreach (var dataConnection in container.Components.OfType<BeepDataConnection>())
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

            var editor = GetEditor();
            var editorConnections = editor?.ConfigEditor?.LoadDataConnectionsValues() ?? editor?.ConfigEditor?.DataConnections;
            if (editorConnections != null)
            {
                foreach (var connection in editorConnections)
                {
                    if (!string.IsNullOrWhiteSpace(connection.ConnectionName))
                    {
                        names.Add(connection.ConnectionName);
                    }
                }
            }

            return names.OrderBy(x => x).ToList();
        }

        private void LoadEntities(string connectionName)
        {
            _entityCombo.Items.Clear();
            if (string.IsNullOrWhiteSpace(connectionName))
            {
                return;
            }

            var entities = GetEntityNames(connectionName);
            _entityCombo.Items.AddRange(entities.Cast<object>().ToArray());
        }

        private List<string> GetEntityNames(string connectionName)
        {
            var editor = GetEditor();
            if (editor == null || string.IsNullOrWhiteSpace(connectionName))
            {
                return new List<string>();
            }

            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                editor.OpenDataSource(connectionName);
                var dataSource = editor.GetDataSource(connectionName);
                if (dataSource == null)
                {
                    return names.ToList();
                }

                var discovered = dataSource.GetEntitesList();
                if (discovered != null)
                {
                    foreach (var entity in discovered)
                    {
                        if (!string.IsNullOrWhiteSpace(entity))
                        {
                            names.Add(entity);
                        }
                    }
                }

                if (names.Count == 0 && dataSource.Entities != null)
                {
                    foreach (var entity in dataSource.Entities)
                    {
                        if (!string.IsNullOrWhiteSpace(entity.EntityName))
                        {
                            names.Add(entity.EntityName);
                        }
                    }
                }
            }
            catch
            {
                // Ignore metadata errors during design-time wizard.
            }

            return names.OrderBy(x => x).ToList();
        }

        private void LoadFields(string connectionName, string entityName)
        {
            _fieldsList.Items.Clear();
            _lastSelectedFieldName = string.Empty;
            _fieldCategoryByFieldName.Clear();
            ResetFieldSelectionOptions();

            if (string.IsNullOrWhiteSpace(connectionName) || string.IsNullOrWhiteSpace(entityName))
            {
                return;
            }

            var fields = GetEntityFields(connectionName, entityName);
            if (fields.Count == 0)
            {
                return;
            }

            var includeMap = _dataBlock.FieldSelections
                .Where(x => !string.IsNullOrWhiteSpace(x.FieldName))
                .ToDictionary(x => x.FieldName, x => x.IncludeInView, StringComparer.OrdinalIgnoreCase);
            var knownFieldNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var field in fields)
            {
                knownFieldNames.Add(field.FieldName);
                _fieldCategoryByFieldName[field.FieldName] = field.FieldCategory;
                var isChecked = includeMap.TryGetValue(field.FieldName, out var include)
                    ? include
                    : true;
                _fieldsList.Items.Add(field.FieldName, isChecked);

                var selection = _dataBlock.FieldSelections.FirstOrDefault(x =>
                    string.Equals(x.FieldName, field.FieldName, StringComparison.OrdinalIgnoreCase));
                if (selection != null)
                {
                    if (!string.IsNullOrWhiteSpace(selection.EditorTypeOverrideFullName))
                    {
                        _selectedEditorTypeByField[field.FieldName] = selection.EditorTypeOverrideFullName;
                    }

                    if (!string.IsNullOrWhiteSpace(selection.TemplateId))
                    {
                        _selectedTemplateByField[field.FieldName] = selection.TemplateId;
                    }
                }
            }

            RemoveStaleSelections(_selectedEditorTypeByField, knownFieldNames);
            RemoveStaleSelections(_selectedTemplateByField, knownFieldNames);

            if (_fieldsList.Items.Count > 0)
            {
                _fieldsList.SelectedIndex = 0;
            }
        }

        private string GetSelectedFieldName()
        {
            return _fieldsList.SelectedItem?.ToString() ?? string.Empty;
        }

        private void SyncFieldEditorSelectorsFromFieldSelection()
        {
            var fieldName = GetSelectedFieldName();
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                _lastSelectedFieldName = string.Empty;
                ResetFieldSelectionOptions();
                return;
            }

            _selectedEditorTypeByField.TryGetValue(fieldName, out var editorTypeName);
            _selectedTemplateByField.TryGetValue(fieldName, out var templateId);
            var hasCategory = _fieldCategoryByFieldName.TryGetValue(fieldName, out var category);

            var editorSelection = string.IsNullOrWhiteSpace(editorTypeName) ? "<Default>" : editorTypeName;
            var templateSelection = string.IsNullOrWhiteSpace(templateId) ? "<None>" : templateId;
            LoadEditorTypeOptions(hasCategory ? category : (DbFieldCategory?)null, editorSelection);
            LoadTemplateOptions(hasCategory ? category : (DbFieldCategory?)null, templateSelection);

            _lastSelectedFieldName = fieldName;
        }

        private void SaveCurrentFieldEditorSelection()
        {
            var fieldName = GetSelectedFieldName();
            SaveFieldEditorSelection(fieldName);
        }

        private void SaveFieldEditorSelection(string fieldName)
        {
            if (_isUpdatingFieldSelectorUi)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return;
            }

            var value = _fieldEditorTypeCombo.SelectedItem?.ToString() ?? "<Default>";
            if (string.Equals(value, "<Default>", StringComparison.OrdinalIgnoreCase))
            {
                _selectedEditorTypeByField.Remove(fieldName);
                return;
            }

            _selectedEditorTypeByField[fieldName] = value;
        }

        private void SaveCurrentFieldTemplateSelection()
        {
            var fieldName = GetSelectedFieldName();
            SaveFieldTemplateSelection(fieldName);
        }

        private void SaveFieldTemplateSelection(string fieldName)
        {
            if (_isUpdatingFieldSelectorUi)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return;
            }

            var value = _fieldTemplateCombo.SelectedItem?.ToString() ?? "<None>";
            if (string.Equals(value, "<None>", StringComparison.OrdinalIgnoreCase))
            {
                _selectedTemplateByField.Remove(fieldName);
                return;
            }

            _selectedTemplateByField[fieldName] = value;
        }

        private List<EntityField> GetEntityFields(string connectionName, string entityName)
        {
            var editor = GetEditor();
            if (editor == null || string.IsNullOrWhiteSpace(connectionName) || string.IsNullOrWhiteSpace(entityName))
            {
                return new List<EntityField>();
            }

            try
            {
                editor.OpenDataSource(connectionName);
                var dataSource = editor.GetDataSource(connectionName);
                if (dataSource == null)
                {
                    return new List<EntityField>();
                }

                var structure = dataSource.GetEntityStructure(entityName, true) ?? dataSource.GetEntityStructure(entityName, false);
                return structure?.Fields?.ToList() ?? new List<EntityField>();
            }
            catch
            {
                return new List<EntityField>();
            }
        }

        private IDMEEditor? GetEditor()
        {
            if (_dataBlock.DMEEditor != null)
            {
                return _dataBlock.DMEEditor;
            }

            try
            {
                var initMethod = typeof(BeepDataBlock).GetMethod("InitializeServices", BindingFlags.Instance | BindingFlags.NonPublic);
                initMethod?.Invoke(_dataBlock, null);
            }
            catch
            {
                // Best effort for design-time service initialization.
            }

            return _dataBlock.DMEEditor;
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
            if (_currentStepIndex == 0 && string.IsNullOrWhiteSpace(_connectionCombo.Text))
            {
                MessageBox.Show("Please select a connection.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_currentStepIndex == 1 && string.IsNullOrWhiteSpace(_entityCombo.Text))
            {
                MessageBox.Show("Please select an entity/table.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_currentStepIndex == 2 && _fieldsList.CheckedItems.Count == 0)
            {
                MessageBox.Show("Select at least one field to display.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_currentStepIndex == 2)
            {
                SaveFieldEditorSelection(_lastSelectedFieldName);
                SaveFieldTemplateSelection(_lastSelectedFieldName);
                if (!ValidateFieldSelections())
                {
                    return false;
                }
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
                    _stepSubtitle.Text = "Choose the Beep connection this block should use.";
                    break;
                case 1:
                    _stepTitle.Text = "Step 2 of 4: Select Entity/Table";
                    _stepSubtitle.Text = "Pick the entity/table from the selected connection.";
                    break;
                case 2:
                    _stepTitle.Text = "Step 3 of 4: Select Fields";
                    _stepSubtitle.Text = "Choose which fields should be shown in the block.";
                    break;
                case 3:
                    _stepTitle.Text = "Step 4 of 4: Select View Mode";
                    _stepSubtitle.Text = "Choose Record Controls or BeepGridPro rendering.";
                    UpdateSummary();
                    break;
            }
        }

        private void UpdateSummary()
        {
            var selected = SelectedFieldNames;
            var editorOverrideCount = _selectedEditorTypeByField.Keys.Count(selected.Contains);
            var templateCount = _selectedTemplateByField.Keys.Count(selected.Contains);
            var warningCount = GetSelectionWarningCount();
            var modeLabel = SelectedViewMode == DataBlockViewMode.BeepGridPro
                ? "Table (BeepGridPro)"
                : "Record Controls";

            _summaryLabel.Text =
                $"Connection: {SelectedConnectionName}\n" +
                $"Entity: {SelectedEntityName}\n" +
                $"Selected Fields: {selected.Count}\n" +
                $"Editor Overrides: {editorOverrideCount}\n" +
                $"Template Assignments: {templateCount}\n" +
                $"Selection Warnings: {warningCount}\n" +
                $"View Mode: {modeLabel}";
        }

        private int GetSelectionWarningCount()
        {
            var warningCount = 0;
            foreach (var pair in _selectedEditorTypeByField)
            {
                if (string.IsNullOrWhiteSpace(pair.Value))
                {
                    continue;
                }

                if (!_fieldCategoryByFieldName.TryGetValue(pair.Key, out var category))
                {
                    continue;
                }

                var allowedTypes = _dataBlock.GetKnownRecordEditorTypes(category);
                var isAllowed = allowedTypes.Any(type =>
                    string.Equals(type.FullName, pair.Value, StringComparison.OrdinalIgnoreCase));
                if (!isAllowed)
                {
                    warningCount++;
                }
            }

            foreach (var pair in _selectedTemplateByField)
            {
                if (string.IsNullOrWhiteSpace(pair.Value))
                {
                    continue;
                }

                if (!_fieldCategoryByFieldName.TryGetValue(pair.Key, out var category))
                {
                    continue;
                }

                var template = _dataBlock.EditorTemplates.FirstOrDefault(x =>
                    string.Equals(x.TemplateId, pair.Value, StringComparison.OrdinalIgnoreCase));
                if (template == null || !template.SupportsCategory(category.ToString()))
                {
                    warningCount++;
                }
            }

            return warningCount;
        }

        private bool ValidateFieldSelections()
        {
            var warnings = new List<string>();
            foreach (var pair in _selectedEditorTypeByField)
            {
                if (string.IsNullOrWhiteSpace(pair.Value))
                {
                    continue;
                }

                if (!_fieldCategoryByFieldName.TryGetValue(pair.Key, out var category))
                {
                    continue;
                }

                var allowedTypes = _dataBlock.GetKnownRecordEditorTypes(category);
                var isAllowed = allowedTypes.Any(type =>
                    string.Equals(type.FullName, pair.Value, StringComparison.OrdinalIgnoreCase));
                if (!isAllowed)
                {
                    warnings.Add($"Field '{pair.Key}' override '{pair.Value}' is unresolved or outside the current category filter.");
                }
            }

            foreach (var pair in _selectedTemplateByField)
            {
                if (string.IsNullOrWhiteSpace(pair.Value))
                {
                    continue;
                }

                if (!_fieldCategoryByFieldName.TryGetValue(pair.Key, out var category))
                {
                    continue;
                }

                var template = _dataBlock.EditorTemplates.FirstOrDefault(x =>
                    string.Equals(x.TemplateId, pair.Value, StringComparison.OrdinalIgnoreCase));
                if (template == null)
                {
                    warnings.Add($"Field '{pair.Key}' keeps missing template '{pair.Value}'.");
                    continue;
                }

                if (!template.SupportsCategory(category.ToString()))
                {
                    warnings.Add($"Field '{pair.Key}' template '{pair.Value}' is outside category '{category}'.");
                }
            }

            if (warnings.Count > 0)
            {
                _fieldSelectionHintLabel.Text = $"Per-field editor/template ({warnings.Count} warning(s))";
            }
            else
            {
                _fieldSelectionHintLabel.Text = "Per-field editor/template";
            }

            return true;
        }

        private static void RemoveStaleSelections(Dictionary<string, string> map, HashSet<string> knownFieldNames)
        {
            var stale = map.Keys.Where(x => !knownFieldNames.Contains(x)).ToList();
            foreach (var key in stale)
            {
                map.Remove(key);
            }
        }
    }
}
