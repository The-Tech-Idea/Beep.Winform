using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;
using TheTechIdea.Beep.Winform.Controls.Integrated.NuggetsManage;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepDataConnection component.
    /// </summary>
    public class BeepDataConnectionDesigner : ComponentDesigner
    {
        private DesignerActionListCollection? _actionLists;
        private IDesignTimeServiceLease? _serviceLease;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            if (component is BeepDataConnection dataConnection)
            {
                try
                {
                    _serviceLease = DesignTimeBeepServiceManager.Acquire(dataConnection);
                    dataConnection.AttachSharedBeepService(_serviceLease.BeepService, reloadConnections: true);
                }
                catch
                {
                    _serviceLease?.Dispose();
                    _serviceLease = null;
                }
            }
        }

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                _actionLists ??= new DesignerActionListCollection
                {
                    new BeepDataConnectionActionList(Component)
                };

                return _actionLists;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _actionLists = null;
                _serviceLease?.Dispose();
                _serviceLease = null;
            }

            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Smart-tag actions for connection list management.
    /// </summary>
    public sealed class BeepDataConnectionActionList : DesignerActionList
    {
        private enum OperationSeverity
        {
            Info,
            Warn,
            Error
        }

        private static readonly List<string> OperationLogEntries = new();
        private static readonly object OperationLogSync = new();
        private static bool AutoSaveOnDesignerEditEnabled;

        private BeepDataConnection? DataConnection => Component as BeepDataConnection;
        private IComponentChangeService? ChangeService => GetService(typeof(IComponentChangeService)) as IComponentChangeService;

        public BeepDataConnectionActionList(IComponent component) : base(component)
        {
        }

        [Category("Connections")]
        [Description("List of available data connections.")]
        public List<ConnectionProperties> DataConnections
        {
            get => DataConnection?.DataConnections ?? new List<ConnectionProperties>();
            set
            {
                SetProperty("DataConnections", value ?? new List<ConnectionProperties>());

                if (DataConnection == null)
                {
                    return;
                }

                if (AutoSaveOnDesignerEditEnabled)
                {
                    var saved = DataConnection.SaveConnections();
                    AppendOperationLog(
                        saved ? OperationSeverity.Info : OperationSeverity.Warn,
                        "AutoSaveOnDesignerEdit",
                        saved
                            ? "Collection edits were auto-saved."
                            : "Auto-save failed. Use Save Connections.");
                }
                else
                {
                    AppendOperationLog(
                        OperationSeverity.Warn,
                        "DataConnectionsChanged",
                        "Collection changed but not persisted yet. Run Save Connections.");
                }

                NotifyPropertyRefresh("DataConnections");
                NotifyPropertyRefresh("CurrentConnection");
            }
        }

        [Category("Connections")]
        [Description("Current/default connection name.")]
        public string CurrentConnectionName
        {
            get => DataConnection?.CurrentConnection?.ConnectionName ?? string.Empty;
            set => SetCurrentConnectionByName(value);
        }

        [Category("Persistence")]
        [Description("Active profile for grouped connection sets.")]
        public string ActiveProfileName
        {
            get => DataConnection?.ActiveProfileName ?? "Default";
            set => SetProperty(nameof(BeepDataConnection.ActiveProfileName), string.IsNullOrWhiteSpace(value) ? "Default" : value.Trim());
        }

        [Category("Persistence")]
        [Description("Scope used as primary read/write store.")]
        public ConnectionStorageScope PersistenceScope
        {
            get => DataConnection?.PersistenceScope ?? ConnectionStorageScope.Project;
            set => SetProperty(nameof(BeepDataConnection.PersistenceScope), value);
        }

        [Category("Persistence")]
        [Description("When true, reads Project->User->Machine chain.")]
        public bool UseScopePrecedence
        {
            get => DataConnection?.UseScopePrecedence ?? true;
            set => SetProperty(nameof(BeepDataConnection.UseScopePrecedence), value);
        }

        [Category("Validation")]
        [Description("Require successful test connection before save.")]
        public bool RequireSuccessfulTestBeforeSave
        {
            get => DataConnection?.RequireSuccessfulTestBeforeSave ?? true;
            set => SetProperty(nameof(BeepDataConnection.RequireSuccessfulTestBeforeSave), value);
        }

        [Category("Persistence")]
        [Description("Automatically persist connection collection edits after property-grid updates.")]
        public bool AutoSaveOnDesignerEdit
        {
            get => AutoSaveOnDesignerEditEnabled;
            set => AutoSaveOnDesignerEditEnabled = value;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection
            {
                new DesignerActionHeaderItem("Connection Configuration"),
                new DesignerActionPropertyItem("CurrentConnectionName", "Current Connection", "Connection Configuration", "Select current/default connection"),
                new DesignerActionPropertyItem("DataConnections", "Connections Collection", "Connection Configuration", "Edit the list of configured connections"),
                new DesignerActionPropertyItem("ActiveProfileName", "Active Profile", "Connection Configuration", "Profile for grouped connection sets"),
                new DesignerActionPropertyItem("PersistenceScope", "Persistence Scope", "Connection Configuration", "Primary scope for load/save"),
                new DesignerActionPropertyItem("UseScopePrecedence", "Use Scope Precedence", "Connection Configuration", "Read Project->User->Machine fallback chain"),
                new DesignerActionPropertyItem("RequireSuccessfulTestBeforeSave", "Require Test Before Save", "Connection Configuration", "Block save until test succeeds"),
                new DesignerActionPropertyItem("AutoSaveOnDesignerEdit", "Auto Save On Designer Edit", "Connection Configuration", "Persist collection edits immediately after changes"),

                new DesignerActionHeaderItem("Connection Operations"),
                new DesignerActionMethodItem(this, "AddConnection", "Add Connection...", "Connection Operations", "Create and edit a new connection", true),
                new DesignerActionMethodItem(this, "EditCurrentConnection", "Edit Current Connection...", "Connection Operations", "Edit selected current connection", true),
                new DesignerActionMethodItem(this, "ManageNuggetsAndDrivers", "Manage Nuggets/Drivers...", "Connection Operations", "Open nugget manager to install/uninstall driver packages", true),
                new DesignerActionMethodItem(this, "ReloadConnections", "Reload Connections", "Connection Operations", "Reload from shared configuration", true),
                new DesignerActionMethodItem(this, "SaveConnections", "Save Connections", "Connection Operations", "Persist current list to shared configuration", true),
                new DesignerActionMethodItem(this, "RemoveCurrentConnection", "Remove Current Connection", "Connection Operations", "Remove selected current connection", true),
                new DesignerActionMethodItem(this, "PromoteProjectToShared", "Promote ProjectLocal -> Shared", "Connection Operations", "Copy active profile from project-local into shared scope", true),
                new DesignerActionMethodItem(this, "PromoteSharedToProject", "Promote Shared -> ProjectLocal", "Connection Operations", "Copy active profile from shared into project-local scope", true),
                new DesignerActionMethodItem(this, "ExportEmbeddedDefaults", "Export Embedded Defaults...", "Connection Operations", "Export active profile package for embedded shipping", true),
                new DesignerActionMethodItem(this, "ImportEmbeddedDefaults", "Import Embedded Defaults...", "Connection Operations", "Import package into active scope/profile", true),
                new DesignerActionMethodItem(this, "ShowConnectionSummary", "Show Summary", "Connection Operations", "Show quick connection diagnostics", true),
                new DesignerActionMethodItem(this, "ShowOperationLogs", "Show Operation Logs", "Connection Operations", "Open operation log panel", true)
            };

            return items;
        }

        public void ReloadConnections()
        {
            if (DataConnection == null)
            {
                return;
            }

            DataConnection.ReloadConnections();
            NotifyPropertyRefresh("DataConnections");
            NotifyPropertyRefresh("CurrentConnection");
            AppendOperationLog(OperationSeverity.Info, "ReloadConnections", "Reloaded connections from storage.");
        }

        public void SaveConnections()
        {
            if (DataConnection == null)
            {
                return;
            }

            var saved = DataConnection.SaveConnections();
            NotifyPropertyRefresh("DataConnections");
            NotifyPropertyRefresh("CurrentConnection");

            MessageBox.Show(
                saved
                    ? "Connections were saved to shared configuration."
                    : "Save skipped. Design-time service may be unavailable.",
                "BeepDataConnection",
                MessageBoxButtons.OK,
                saved ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            AppendOperationLog(saved ? OperationSeverity.Info : OperationSeverity.Warn, "SaveConnections", saved ? "Saved active profile successfully." : "Save skipped or failed.");
        }

        public void AddConnection()
        {
            if (DataConnection == null)
            {
                return;
            }

            var candidate = new ConnectionProperties
            {
                ConnectionName = GetUniqueConnectionName()
            };

            var committed = OpenEditorAndCommit(candidate, isNew: true);
            if (!committed)
            {
                return;
            }

            NotifyPropertyRefresh("DataConnections");
            NotifyPropertyRefresh("CurrentConnection");
            AppendOperationLog(OperationSeverity.Info, "AddConnection", $"Added connection '{DataConnection.CurrentConnection?.ConnectionName ?? "(unknown)"}'.");
        }

        public void EditCurrentConnection()
        {
            if (DataConnection?.CurrentConnection == null)
            {
                MessageBox.Show(
                    "No current connection is selected.",
                    "BeepDataConnection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var workingCopy = CloneConnection(DataConnection.CurrentConnection);
            var committed = OpenEditorAndCommit(workingCopy, isNew: false);
            if (!committed)
            {
                return;
            }

            NotifyPropertyRefresh("DataConnections");
            NotifyPropertyRefresh("CurrentConnection");
            AppendOperationLog(OperationSeverity.Info, "EditCurrentConnection", $"Edited connection '{DataConnection.CurrentConnection?.ConnectionName ?? "(unknown)"}'.");
        }

        public void AddConnectionTemplate()
        {
            AddConnection();
        }

        public void RemoveCurrentConnection()
        {
            if (DataConnection?.CurrentConnection == null)
            {
                MessageBox.Show(
                    "No current connection is selected.",
                    "BeepDataConnection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var currentName = DataConnection.CurrentConnection.ConnectionName;
            if (string.IsNullOrWhiteSpace(currentName))
            {
                MessageBox.Show(
                    "Current connection has no name and cannot be removed.",
                    "BeepDataConnection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var removed = DataConnection.RemoveConnection(currentName, persist: false);
            if (removed)
            {
                removed = DataConnection.SaveConnections();
            }

            NotifyPropertyRefresh("DataConnections");
            NotifyPropertyRefresh("CurrentConnection");

            MessageBox.Show(
                removed
                    ? $"Removed '{currentName}' and persisted changes."
                    : $"Could not remove '{currentName}'.",
                "BeepDataConnection",
                MessageBoxButtons.OK,
                removed ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            AppendOperationLog(removed ? OperationSeverity.Info : OperationSeverity.Warn, "RemoveCurrentConnection", removed ? $"Removed '{currentName}'." : $"Failed to remove '{currentName}'.");
        }

        public void ShowConnectionSummary()
        {
            if (DataConnection == null)
            {
                return;
            }

            var connections = DataConnection.ReloadConnections();
            var currentName = DataConnection.CurrentConnection?.ConnectionName ?? "(none)";
            var repoName = string.IsNullOrWhiteSpace(DataConnection.AppRepoName) ? "(default)" : DataConnection.AppRepoName;
            var scope = DataConnection.PersistenceScope.ToString();

            MessageBox.Show(
                $"Connections: {connections.Count}\nCurrent: {currentName}\nRepo: {repoName}\nScope: {scope}",
                "BeepDataConnection Summary",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            AppendOperationLog(OperationSeverity.Info, "ShowConnectionSummary", $"Summary viewed for profile '{DataConnection.ActiveProfileName}'.");
        }

        public void PromoteProjectToShared()
        {
            RunPromotion(ConnectionStoreKind.ProjectLocal, ConnectionStoreKind.Shared);
        }

        public void PromoteSharedToProject()
        {
            RunPromotion(ConnectionStoreKind.Shared, ConnectionStoreKind.ProjectLocal);
        }

        public void ExportEmbeddedDefaults()
        {
            if (DataConnection == null)
            {
                return;
            }

            using var saveDialog = new SaveFileDialog
            {
                Title = "Export Embedded Defaults Package",
                Filter = "JSON files (*.json)|*.json",
                FileName = $"{DataConnection.ActiveProfileName}.embedded.defaults.json"
            };

            if (saveDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var includeSecrets = MessageBox.Show(
                "Include encrypted secrets in package?\n\nChoose No to strip secrets.",
                "Export Secrets",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes;

            var ok = DataConnection.ExportEmbeddedDefaults(saveDialog.FileName, includeSecrets, out var message);
            MessageBox.Show(
                ok ? $"Package exported to:\n{saveDialog.FileName}\n\n{message}" : $"Export failed:\n{message}",
                "BeepDataConnection",
                MessageBoxButtons.OK,
                ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            AppendOperationLog(ok ? OperationSeverity.Info : OperationSeverity.Error, "ExportEmbeddedDefaults", ok ? $"Exported package '{saveDialog.FileName}'." : $"Export failed: {message}");
        }

        public void ImportEmbeddedDefaults()
        {
            if (DataConnection == null)
            {
                return;
            }

            using var openDialog = new OpenFileDialog
            {
                Title = "Import Embedded Defaults Package",
                Filter = "JSON files (*.json)|*.json"
            };

            if (openDialog.ShowDialog() != DialogResult.OK || !File.Exists(openDialog.FileName))
            {
                return;
            }

            if (!TrySelectConflictPolicy("Import Embedded Defaults", showImportMode: true, out var policy, out var emptyOnly))
            {
                return;
            }

            var ok = DataConnection.ImportEmbeddedDefaults(openDialog.FileName, policy, emptyOnly, out var message);
            NotifyPropertyRefresh("DataConnections");
            NotifyPropertyRefresh("CurrentConnection");
            MessageBox.Show(
                ok ? $"Import completed.\n\n{message}" : $"Import failed:\n{message}",
                "BeepDataConnection",
                MessageBoxButtons.OK,
                ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            AppendOperationLog(ok ? OperationSeverity.Info : OperationSeverity.Error, "ImportEmbeddedDefaults", ok ? $"Imported package '{openDialog.FileName}'." : $"Import failed: {message}");
        }

        public void ShowOperationLogs()
        {
            var snapshot = GetOperationLogsSnapshot();
            using var panel = new OperationLogPanelForm(snapshot, ClearOperationLogs);
            panel.ShowDialog();
        }

        public void ManageNuggetsAndDrivers()
        {
            if (DataConnection?.BeepService is not IServiceProvider services)
            {
                MessageBox.Show(
                    "Nuggets manager requires a runtime IServiceProvider-enabled BeepService host.",
                    "BeepDataConnection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            using var managerHost = new Form
            {
                Text = "Nuggets and Driver Management",
                StartPosition = FormStartPosition.CenterScreen,
                MinimumSize = new System.Drawing.Size(900, 640),
                Size = new System.Drawing.Size(960, 700)
            };

            var manager = new uc_NuggetsManage(services)
            {
                Dock = DockStyle.Fill
            };
            managerHost.Controls.Add(manager);
            manager.InitializeManager();
            managerHost.ShowDialog();
            manager.Dispose();
        }

        private void SetCurrentConnectionByName(string? connectionName)
        {
            if (DataConnection == null)
            {
                return;
            }

            var name = connectionName?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                SetProperty("CurrentConnection", null);
                return;
            }

            var selected = DataConnection.ReloadConnections()
                .FirstOrDefault(c => string.Equals(c.ConnectionName, name, StringComparison.OrdinalIgnoreCase));

            if (selected == null)
            {
                MessageBox.Show(
                    $"Connection '{name}' was not found.",
                    "BeepDataConnection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            SetProperty("CurrentConnection", selected);
            NotifyPropertyRefresh("CurrentConnection");
        }

        private string GetUniqueConnectionName()
        {
            var existingNames = new HashSet<string>(
                DataConnection?.DataConnections?
                    .Where(c => !string.IsNullOrWhiteSpace(c.ConnectionName))
                    .Select(c => c.ConnectionName) ??
                Enumerable.Empty<string>(),
                StringComparer.OrdinalIgnoreCase);

            var index = 1;
            while (true)
            {
                var candidate = $"Connection{index}";
                if (!existingNames.Contains(candidate))
                {
                    return candidate;
                }

                index++;
            }
        }

        private bool OpenEditorAndCommit(ConnectionProperties seedConnection, bool isNew)
        {
            if (DataConnection == null)
            {
                return false;
            }

            using var dialog = new ConnectionEditorHostForm(seedConnection, DataConnection.BeepService, DataConnection.RequireSuccessfulTestBeforeSave);
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            var edited = dialog.GetUpdatedConnection();
            if (edited == null || string.IsNullOrWhiteSpace(edited.ConnectionName))
            {
                MessageBox.Show(
                    "Connection name is required.",
                    "BeepDataConnection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            var duplicateName = DataConnection.DataConnections.FirstOrDefault(c =>
                !ReferenceEquals(c, DataConnection.CurrentConnection) &&
                string.Equals(c.ConnectionName, edited.ConnectionName, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(c.GuidID, edited.GuidID, StringComparison.OrdinalIgnoreCase));
            if (duplicateName != null)
            {
                MessageBox.Show(
                    $"A connection named '{edited.ConnectionName}' already exists.",
                    "BeepDataConnection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            var upserted = DataConnection.AddOrUpdateConnection(edited, persist: false);
            if (!upserted)
            {
                MessageBox.Show(
                    isNew
                        ? "Could not add the connection."
                        : "Could not update the connection.",
                    "BeepDataConnection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            var saved = DataConnection.SaveConnections();
            if (!saved)
            {
                MessageBox.Show(
                    "Connection was edited but could not be persisted.",
                    "BeepDataConnection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            SetProperty("CurrentConnection", edited);
            return true;
        }

        private void RunPromotion(ConnectionStoreKind source, ConnectionStoreKind target)
        {
            if (DataConnection == null)
            {
                return;
            }

            if (!TrySelectConflictPolicy("Promotion Conflict Policy", showImportMode: false, out var policy, out _))
            {
                return;
            }

            var ok = DataConnection.PromoteConnections(source, target, policy, out var message);
            NotifyPropertyRefresh("DataConnections");
            NotifyPropertyRefresh("CurrentConnection");
            MessageBox.Show(
                ok ? $"Promotion completed.\n\n{message}" : $"Promotion failed.\n\n{message}",
                "BeepDataConnection",
                MessageBoxButtons.OK,
                ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            AppendOperationLog(ok ? OperationSeverity.Info : OperationSeverity.Error, "RunPromotion", ok
                ? $"Promoted {source} -> {target} using {policy}."
                : $"Promotion {source} -> {target} failed: {message}");
        }

        private static bool TrySelectConflictPolicy(string title, bool showImportMode, out ConnectionConflictPolicy policy, out bool importWhenEmptyOnly)
        {
            using var dialog = new ConflictPolicyDialog(title, showImportMode);
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                policy = ConnectionConflictPolicy.Skip;
                importWhenEmptyOnly = false;
                return false;
            }

            policy = dialog.SelectedPolicy;
            importWhenEmptyOnly = dialog.ImportWhenEmptyOnly;
            return true;
        }

        private static void AppendOperationLog(OperationSeverity severity, string operation, string details)
        {
            lock (OperationLogSync)
            {
                OperationLogEntries.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{severity.ToString().ToUpperInvariant()}] {operation} - {details}");
                if (OperationLogEntries.Count > 500)
                {
                    OperationLogEntries.RemoveRange(0, OperationLogEntries.Count - 500);
                }
            }
        }

        private static IReadOnlyList<string> GetOperationLogsSnapshot()
        {
            lock (OperationLogSync)
            {
                return OperationLogEntries.ToList();
            }
        }

        private static void ClearOperationLogs()
        {
            lock (OperationLogSync)
            {
                OperationLogEntries.Clear();
            }
        }

        private static ConnectionProperties CloneConnection(ConnectionProperties source)
        {
            var clone = new ConnectionProperties();
            var properties = typeof(ConnectionProperties).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0);

            foreach (var property in properties)
            {
                try
                {
                    property.SetValue(clone, property.GetValue(source));
                }
                catch
                {
                    // Ignore non-assignable property edge cases.
                }
            }

            clone.ParameterList = source.ParameterList != null
                ? new Dictionary<string, string>(source.ParameterList, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            return clone;
        }

        private void SetProperty(string propertyName, object? value)
        {
            if (DataConnection == null)
            {
                return;
            }

            var property = TypeDescriptor.GetProperties(DataConnection)[propertyName];
            if (property == null || property.IsReadOnly)
            {
                return;
            }

            if (value == null)
            {
                if (property.PropertyType.IsValueType && Nullable.GetUnderlyingType(property.PropertyType) == null)
                {
                    return;
                }
            }
            else if (!property.PropertyType.IsInstanceOfType(value))
            {
                return;
            }

            var oldValue = property.GetValue(DataConnection);
            if (Equals(oldValue, value))
            {
                return;
            }

            ChangeService?.OnComponentChanging(DataConnection, property);
            property.SetValue(DataConnection, value);
            ChangeService?.OnComponentChanged(DataConnection, property, oldValue, value);
        }

        private void NotifyPropertyRefresh(string propertyName)
        {
            if (DataConnection == null)
            {
                return;
            }

            var property = TypeDescriptor.GetProperties(DataConnection)[propertyName];
            if (property == null)
            {
                return;
            }

            var value = property.GetValue(DataConnection);
            ChangeService?.OnComponentChanged(DataConnection, property, value, value);
        }
    }
}
