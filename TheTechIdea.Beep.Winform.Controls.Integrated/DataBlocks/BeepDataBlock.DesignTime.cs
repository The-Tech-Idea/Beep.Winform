using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows.Forms;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepDataBlock
    {
        private readonly BindingList<BeepDataBlockEditorTemplate> _editorTemplates = new();
        private int _templateSchemaVersion = 1;
        private static readonly object EditorCatalogLock = new();
        private static readonly object AssemblyHookLock = new();
        private static List<Type>? _cachedEditorCatalog;
        private static bool _isAssemblyHookRegistered;

        [Browsable(true)]
        [Category("Templates")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Reusable editor templates that can be assigned to fields in Record view.")]
        public BindingList<BeepDataBlockEditorTemplate> EditorTemplates => _editorTemplates;

        [Browsable(true)]
        [Category("Templates")]
        [DefaultValue(1)]
        [Description("Version of the editor template payload schema.")]
        public int TemplateSchemaVersion
        {
            get => _templateSchemaVersion;
            set => _templateSchemaVersion = Math.Max(1, value);
        }

        [Browsable(true)]
        [Category("Templates")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Serialized template payload (schemaVersion/templates/fieldAssignments JSON).")]
        public string EditorTemplatePayloadJson
        {
            get
            {
                var payload = new BeepDataBlockTemplatePayload
                {
                    SchemaVersion = TemplateSchemaVersion,
                    Templates = EditorTemplates.ToList(),
                    FieldAssignments = FieldSelections
                        .Where(x => !string.IsNullOrWhiteSpace(x.FieldName))
                        .Select(x => new BeepDataBlockFieldTemplateAssignment
                        {
                            FieldName = x.FieldName,
                            TemplateId = x.TemplateId,
                            EditorTypeOverrideFullName = x.EditorTypeOverrideFullName,
                            InlineSettingsJson = string.IsNullOrWhiteSpace(x.InlineSettingsJson) ? "{}" : x.InlineSettingsJson
                        })
                        .ToList()
                };

                return JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                try
                {
                    var payload = JsonSerializer.Deserialize<BeepDataBlockTemplatePayload>(value);
                    if (payload == null)
                    {
                        return;
                    }

                    TemplateSchemaVersion = payload.SchemaVersion <= 0 ? 1 : payload.SchemaVersion;
                    _editorTemplates.Clear();
                    foreach (var template in payload.Templates ?? new List<BeepDataBlockEditorTemplate>())
                    {
                        _editorTemplates.Add(template);
                    }

                    foreach (var assignment in payload.FieldAssignments ?? new List<BeepDataBlockFieldTemplateAssignment>())
                    {
                        var selection = _fieldSelections.FirstOrDefault(x =>
                            string.Equals(x.FieldName, assignment.FieldName, StringComparison.OrdinalIgnoreCase));
                        if (selection == null)
                        {
                            continue;
                        }

                        selection.TemplateId = assignment.TemplateId ?? string.Empty;
                        selection.EditorTypeOverrideFullName = assignment.EditorTypeOverrideFullName ?? string.Empty;
                        selection.InlineSettingsJson = assignment.InlineSettingsJson ?? "{}";
                    }
                }
                catch
                {
                    // Keep existing config when JSON is invalid.
                }
            }
        }

        private void EnsureDefaultEditorTemplates()
        {
            if (_editorTemplates.Count > 0)
            {
                return;
            }

            _editorTemplates.Add(new BeepDataBlockEditorTemplate
            {
                TemplateId = "status_combo",
                DisplayName = "Status Combo",
                EditorTypeFullName = typeof(BeepComboBox).FullName ?? string.Empty,
                SupportedFieldCategoriesCsv = "String,Enum",
                SettingsJson = "{\"itemsSource\":\"Static\",\"statusValues\":[\"New\",\"InProgress\",\"Done\"],\"allowNull\":false}",
                ValidationJson = "{\"required\":true}",
                Version = 1
            });

            _editorTemplates.Add(new BeepDataBlockEditorTemplate
            {
                TemplateId = "avatar_image",
                DisplayName = "Avatar Image",
                EditorTypeFullName = typeof(BeepImage).FullName ?? string.Empty,
                SupportedFieldCategoriesCsv = "String,Binary",
                SettingsJson = "{\"imagePathField\":\"AvatarPath\",\"imageBytesField\":\"AvatarBytes\",\"placeholderSvg\":\"user-circle\",\"stretchMode\":\"Zoom\"}",
                ValidationJson = "{}",
                Version = 1
            });

            _editorTemplates.Add(new BeepDataBlockEditorTemplate
            {
                TemplateId = "currency_input",
                DisplayName = "Currency Input",
                EditorTypeFullName = typeof(BeepTextBox).FullName ?? string.Empty,
                SupportedFieldCategoriesCsv = "Currency,Numeric",
                SettingsJson = "{\"format\":\"C2\",\"culture\":\"en-US\",\"min\":0,\"thousandSeparator\":true}",
                ValidationJson = "{}",
                Version = 1
            });
        }

        internal IReadOnlyList<string> GetAvailableConnectionNames()
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var container = Site?.Container;
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

            var editor = ResolveEditorForMetadata();
            var configConnections = editor?.ConfigEditor?.LoadDataConnectionsValues() ?? editor?.ConfigEditor?.DataConnections;
            if (configConnections != null)
            {
                foreach (var connection in configConnections)
                {
                    if (!string.IsNullOrWhiteSpace(connection.ConnectionName))
                    {
                        names.Add(connection.ConnectionName);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(_connectionName))
            {
                names.Add(_connectionName);
            }

            if (!string.IsNullOrWhiteSpace(QueryDataSourceName))
            {
                names.Add(QueryDataSourceName);
            }

            return names.OrderBy(x => x).ToList();
        }

        internal IReadOnlyList<string> GetAvailableEntityNames()
        {
            var entityNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var connectionName = GetEffectiveConnectionName();

            if (string.IsNullOrWhiteSpace(connectionName))
            {
                return Array.Empty<string>();
            }

            var editor = ResolveEditorForMetadata();
            if (editor == null)
            {
                return Array.Empty<string>();
            }

            try
            {
                editor.OpenDataSource(connectionName);
                var dataSource = editor.GetDataSource(connectionName);
                if (dataSource == null)
                {
                    return Array.Empty<string>();
                }

                var discovered = dataSource.GetEntitesList();
                if (discovered != null)
                {
                    foreach (var name in discovered)
                    {
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            entityNames.Add(name);
                        }
                    }
                }

                if (entityNames.Count == 0 && dataSource.Entities != null)
                {
                    foreach (var entity in dataSource.Entities)
                    {
                        if (!string.IsNullOrWhiteSpace(entity.EntityName))
                        {
                            entityNames.Add(entity.EntityName);
                        }
                    }
                }
            }
            catch
            {
                // Design-time metadata calls should fail silently.
            }

            return entityNames.OrderBy(x => x).ToList();
        }

        public IReadOnlyList<Type> GetKnownRecordEditorTypes()
        {
            EnsureEditorCatalogSubscription();
            lock (EditorCatalogLock)
            {
                if (_cachedEditorCatalog != null)
                {
                    return _cachedEditorCatalog;
                }

                _cachedEditorCatalog = DiscoverBeepEditorTypes()
                    .OrderBy(x => x.FullName, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                return _cachedEditorCatalog;
            }
        }

        public IReadOnlyList<Type> GetKnownRecordEditorTypes(DbFieldCategory category)
        {
            return GetKnownRecordEditorTypes()
                .Where(type => IsCompatibleEditorType(type, category, template: null))
                .Where(type => IsEditorTypeRecommendedForCategory(type, category))
                .ToList();
        }

        public IReadOnlyList<BeepDataBlockEditorTemplate> GetEditorTemplates(DbFieldCategory category)
        {
            return _editorTemplates
                .Where(template => template.SupportsCategory(category.ToString()))
                .ToList();
        }

        public bool RefreshEntityMetadata(bool regenerateSurface = true)
        {
            if (!TryResolveEntityStructureFromDataSource(refreshMetadata: true))
            {
                return false;
            }

            InitializeEntityRelationships();
            SyncFieldSelectionsWithEntityStructure();
            ApplyFieldSelectionsToComponents(regenerateSurface);
            return true;
        }

        private void OnConnectionChanged()
        {
            if (!string.IsNullOrWhiteSpace(_entityName))
            {
                RefreshEntityMetadata();
            }
        }

        private void OnEntitySelectionChanged()
        {
            if (string.IsNullOrWhiteSpace(_entityName))
            {
                return;
            }

            if (TryResolveEntityStructureFromDataSource(refreshMetadata: true))
            {
                InitializeEntityRelationships();
                SyncFieldSelectionsWithEntityStructure();
                ApplyFieldSelectionsToComponents(regenerateSurface: true);
            }
        }

        private bool TryResolveEntityStructureFromDataSource(bool refreshMetadata)
        {
            var connectionName = GetEffectiveConnectionName();
            if (string.IsNullOrWhiteSpace(connectionName) || string.IsNullOrWhiteSpace(_entityName))
            {
                return false;
            }

            var editor = ResolveEditorForMetadata();
            if (editor == null)
            {
                return false;
            }

            try
            {
                editor.OpenDataSource(connectionName);
                var dataSource = editor.GetDataSource(connectionName);
                if (dataSource == null)
                {
                    return false;
                }

                var entity = dataSource.GetEntityStructure(_entityName, refreshMetadata);
                if (entity == null && refreshMetadata)
                {
                    entity = dataSource.GetEntityStructure(_entityName, false);
                }

                if (entity == null)
                {
                    return false;
                }

                EntityStructure = entity;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private IDMEEditor? ResolveEditorForMetadata()
        {
            if (_dmeEditor != null)
            {
                return _dmeEditor;
            }

            if (beepService?.DMEEditor != null)
            {
                _dmeEditor = beepService.DMEEditor;
                return _dmeEditor;
            }

            try
            {
                FindServicesInParentChain();
                if (_dmeEditor != null)
                {
                    return _dmeEditor;
                }

                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                {
                    // Do not instantiate BeepService as a design-time fallback.
                    // The WinForms designer process may not resolve the full runtime
                    // logging dependency graph (e.g., Serilog adapters), which can
                    // surface noisy load failures while dragging controls.
                    return null;
                }
            }
            catch
            {
                // Design-time service initialization is best effort.
            }

            return _dmeEditor;
        }

        private string GetEffectiveConnectionName()
        {
            if (!string.IsNullOrWhiteSpace(_connectionName))
            {
                return _connectionName;
            }

            return QueryDataSourceName ?? string.Empty;
        }

        private void FieldSelections_ListChanged(object? sender, ListChangedEventArgs e)
        {
            if (_isSynchronizingFieldSelections)
            {
                return;
            }

            ApplyFieldSelectionsToComponents(regenerateSurface: true);
        }

        private void SyncFieldSelectionsWithEntityStructure()
        {
            var fields = EntityStructure?.Fields;
            if (fields == null)
            {
                _isSynchronizingFieldSelections = true;
                try
                {
                    _fieldSelections.Clear();
                }
                finally
                {
                    _isSynchronizingFieldSelections = false;
                }

                return;
            }

            var existing = _fieldSelections
                .Where(x => !string.IsNullOrWhiteSpace(x.FieldName))
                .ToDictionary(x => x.FieldName, x => x, StringComparer.OrdinalIgnoreCase);

            _isSynchronizingFieldSelections = true;
            try
            {
                _fieldSelections.Clear();

                foreach (var field in fields)
                {
                    if (existing.TryGetValue(field.FieldName, out var selection))
                    {
                        if (string.IsNullOrWhiteSpace(selection.ControlTypeFullName))
                        {
                            selection.ControlType = ControlExtensions.GetDefaultControlType(field.FieldCategory);
                        }

                        _fieldSelections.Add(selection);
                        continue;
                    }

                    _fieldSelections.Add(new BeepDataBlockFieldSelection
                    {
                        FieldName = field.FieldName,
                        IncludeInView = true,
                        ControlType = ControlExtensions.GetDefaultControlType(field.FieldCategory)
                    });
                }
            }
            finally
            {
                _isSynchronizingFieldSelections = false;
            }
        }

        private IEnumerable<EntityField> GetFieldsForRendering()
        {
            var fields = EntityStructure?.Fields;
            if (fields == null)
            {
                return Enumerable.Empty<EntityField>();
            }

            var configuredFields = _fieldSelections
                .Where(x => !string.IsNullOrWhiteSpace(x.FieldName))
                .Select(x => x.FieldName)
                .ToList();

            if (configuredFields.Count == 0)
            {
                return fields;
            }

            var included = _fieldSelections
                .Where(x => x.IncludeInView && !string.IsNullOrWhiteSpace(x.FieldName))
                .Select(x => x.FieldName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return fields.Where(f => included.Contains(f.FieldName));
        }

        private void ApplyFieldSelectionsToComponents(bool regenerateSurface = true)
        {
            var fields = EntityStructure?.Fields;
            if (fields == null)
            {
                return;
            }

            var configuredFields = _fieldSelections
                .Where(x => !string.IsNullOrWhiteSpace(x.FieldName))
                .Select(x => x.FieldName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var includedFields = _fieldSelections
                .Where(x => x.IncludeInView && !string.IsNullOrWhiteSpace(x.FieldName))
                .Select(x => x.FieldName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            for (int i = Components.Count - 1; i >= 0; i--)
            {
                var component = Components[i];
                var boundName = string.IsNullOrWhiteSpace(component.BoundProperty) ? component.Name : component.BoundProperty;

                if (configuredFields.Count > 0 && !includedFields.Contains(boundName))
                {
                    Components.RemoveAt(i);
                }
            }

            foreach (var field in fields)
            {
                if (configuredFields.Count > 0 && !includedFields.Contains(field.FieldName))
                {
                    continue;
                }

                var selection = _fieldSelections.FirstOrDefault(x =>
                    string.Equals(x.FieldName, field.FieldName, StringComparison.OrdinalIgnoreCase));
                var resolved = ResolveEffectiveFieldEditorMetadata(field, selection);
                var selectedType = resolved.EditorType;
                var selectedTypeFullName = resolved.EditorTypeFullName;

                var component = Components.FirstOrDefault(x =>
                    string.Equals(x.BoundProperty, field.FieldName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(x.Name, field.FieldName, StringComparison.OrdinalIgnoreCase));

                if (component == null)
                {
                    Components.Add(new BeepComponents
                    {
                        Name = field.FieldName,
                        Type = selectedType,
                        TypeFullName = selectedTypeFullName,
                        GUID = Guid.NewGuid().ToString(),
                        Left = 10,
                        Top = 10 + Components.Count * 30,
                        Width = 200,
                        Height = 25,
                        Category = field.FieldCategory,
                        BoundProperty = field.FieldName,
                        DataSourceProperty = field.FieldName,
                        LinkedProperty = string.Empty
                    });
                }
                else
                {
                    component.Type = selectedType;
                    component.TypeFullName = selectedTypeFullName;
                    component.Category = field.FieldCategory;
                    component.BoundProperty = field.FieldName;
                    component.DataSourceProperty = field.FieldName;
                }
            }

            if (regenerateSurface)
            {
                ClearAllComponentControls();
                InitializeControls();
                ApplyViewMode();
            }
        }

        private (Type EditorType, string EditorTypeFullName, BeepDataBlockEditorTemplate? Template)
            ResolveEffectiveFieldEditorMetadata(EntityField field, BeepDataBlockFieldSelection? selection)
        {
            var selectedType = ResolveControlTypeForField(field, selection);
            var selectedTypeFullName = selectedType.FullName ?? string.Empty;
            var template = ResolveTemplate(selection?.TemplateId);
            return (selectedType, selectedTypeFullName, template);
        }

        internal IReadOnlyDictionary<string, Type> ResolveIncludedFieldEditorTypes()
        {
            var fields = EntityStructure?.Fields;
            if (fields == null)
            {
                return new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            }

            var includedNames = _fieldSelections
                .Where(x => x.IncludeInView && !string.IsNullOrWhiteSpace(x.FieldName))
                .Select(x => x.FieldName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var useAllFields = includedNames.Count == 0;
            var map = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            foreach (var field in fields)
            {
                if (!useAllFields && !includedNames.Contains(field.FieldName))
                {
                    continue;
                }

                var selection = _fieldSelections.FirstOrDefault(x =>
                    string.Equals(x.FieldName, field.FieldName, StringComparison.OrdinalIgnoreCase));
                map[field.FieldName] = ResolveControlTypeForField(field, selection);
            }

            return map;
        }

        private Type ResolveControlTypeForField(EntityField field, BeepDataBlockFieldSelection? selection)
        {
            var defaultType = ControlExtensions.GetDefaultControlType(field.FieldCategory);
            if (selection == null)
            {
                return defaultType;
            }

            var template = ResolveTemplate(selection.TemplateId);
            // Precedence must remain: Template -> Override -> ControlType -> Default.
            var candidateType = template?.EditorType ?? selection.EditorTypeOverride ?? selection.ControlType ?? defaultType;
            if (candidateType == null)
            {
                return defaultType;
            }

            if (!IsCompatibleEditorType(candidateType, field, template))
            {
                return defaultType;
            }

            return candidateType;
        }

        private BeepDataBlockEditorTemplate? ResolveTemplate(string templateId)
        {
            if (string.IsNullOrWhiteSpace(templateId))
            {
                return null;
            }

            return _editorTemplates.FirstOrDefault(x =>
                string.Equals(x.TemplateId, templateId, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsCompatibleEditorType(Type editorType, EntityField field, BeepDataBlockEditorTemplate? template)
        {
            return IsCompatibleEditorType(editorType, field.FieldCategory, template);
        }

        private static bool IsCompatibleEditorType(Type editorType, DbFieldCategory category, BeepDataBlockEditorTemplate? template)
        {
            if (!typeof(Control).IsAssignableFrom(editorType) || !typeof(IBeepUIComponent).IsAssignableFrom(editorType))
            {
                return false;
            }

            if (editorType.IsAbstract || editorType.IsGenericTypeDefinition)
            {
                return false;
            }

            if (editorType.GetConstructor(Type.EmptyTypes) == null)
            {
                return false;
            }

            if (template != null && !template.SupportsCategory(category.ToString()))
            {
                return false;
            }

            return true;
        }

        private static bool IsEditorTypeRecommendedForCategory(Type editorType, DbFieldCategory category)
        {
            var fullName = editorType.FullName ?? editorType.Name;
            var typeName = editorType.Name;
            var defaultType = ControlExtensions.GetDefaultControlType(category);
            if (string.Equals(defaultType.FullName, fullName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string[] keywords;
            switch (category)
            {
                case DbFieldCategory.Boolean:
                    keywords = new[] { "Check", "Toggle", "Switch" };
                    break;
                case DbFieldCategory.Date:
                case DbFieldCategory.Timestamp:
                    keywords = new[] { "Date", "Time", "Calendar", "Picker" };
                    break;
                case DbFieldCategory.Binary:
                    keywords = new[] { "Image", "Picture", "Avatar" };
                    break;
                case DbFieldCategory.Enum:
                    keywords = new[] { "Combo", "Drop", "List", "Lookup" };
                    break;
                case DbFieldCategory.Numeric:
                case DbFieldCategory.Currency:
                    keywords = new[] { "Numeric", "Number", "Currency", "Spin", "TextBox" };
                    break;
                case DbFieldCategory.Guid:
                    keywords = new[] { "TextBox", "Label" };
                    break;
                case DbFieldCategory.Json:
                case DbFieldCategory.Xml:
                case DbFieldCategory.Complex:
                case DbFieldCategory.Geography:
                    keywords = new[] { "TextBox", "Memo", "Editor" };
                    break;
                default:
                    keywords = Array.Empty<string>();
                    break;
            }

            if (keywords.Length == 0)
            {
                return true;
            }

            return keywords.Any(keyword =>
                typeName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                fullName.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        private static IEnumerable<Type> DiscoverBeepEditorTypes()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(GetLoadableTypes)
                .Where(IsEligibleBeepEditorType)
                .Distinct();
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(x => x != null)!;
            }
            catch
            {
                return Array.Empty<Type>();
            }
        }

        private static bool IsEligibleBeepEditorType(Type type)
        {
            if (type == null || !type.IsClass || type.IsAbstract || type.IsGenericTypeDefinition)
            {
                return false;
            }

            if (!typeof(Control).IsAssignableFrom(type) || !typeof(IBeepUIComponent).IsAssignableFrom(type))
            {
                return false;
            }

            return type.GetConstructor(Type.EmptyTypes) != null;
        }

        private static void EnsureEditorCatalogSubscription()
        {
            if (_isAssemblyHookRegistered)
            {
                return;
            }

            lock (AssemblyHookLock)
            {
                if (_isAssemblyHookRegistered)
                {
                    return;
                }

                AppDomain.CurrentDomain.AssemblyLoad += (_, __) =>
                {
                    lock (EditorCatalogLock)
                    {
                        _cachedEditorCatalog = null;
                    }
                };
                _isAssemblyHookRegistered = true;
            }
        }
    }
}
