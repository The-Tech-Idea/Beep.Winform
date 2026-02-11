using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepDataConnection component.
    /// </summary>
    public class BeepDataConnectionDesigner : ComponentDesigner
    {
        private DesignerActionListCollection? _actionLists;

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
            }

            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Smart-tag actions for connection list management.
    /// </summary>
    public sealed class BeepDataConnectionActionList : DesignerActionList
    {
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
            set => SetProperty("DataConnections", value ?? new List<ConnectionProperties>());
        }

        [Category("Connections")]
        [Description("Current/default connection name.")]
        public string CurrentConnectionName
        {
            get => DataConnection?.CurrentConnection?.ConnectionName ?? string.Empty;
            set => SetCurrentConnectionByName(value);
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection
            {
                new DesignerActionHeaderItem("Connection Configuration"),
                new DesignerActionPropertyItem("CurrentConnectionName", "Current Connection", "Connection Configuration", "Select current/default connection"),
                new DesignerActionPropertyItem("DataConnections", "Connections Collection", "Connection Configuration", "Edit the list of configured connections"),

                new DesignerActionHeaderItem("Connection Operations"),
                new DesignerActionMethodItem(this, "ReloadConnections", "Reload Connections", "Connection Operations", "Reload from shared configuration", true),
                new DesignerActionMethodItem(this, "SaveConnections", "Save Connections", "Connection Operations", "Persist current list to shared configuration", true),
                new DesignerActionMethodItem(this, "AddConnectionTemplate", "Add Connection Template", "Connection Operations", "Create a placeholder connection entry", true),
                new DesignerActionMethodItem(this, "RemoveCurrentConnection", "Remove Current Connection", "Connection Operations", "Remove selected current connection", true),
                new DesignerActionMethodItem(this, "ShowConnectionSummary", "Show Summary", "Connection Operations", "Show quick connection diagnostics", true)
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
        }

        public void AddConnectionTemplate()
        {
            if (DataConnection == null)
            {
                return;
            }

            var candidate = new ConnectionProperties
            {
                ConnectionName = GetUniqueConnectionName()
            };

            var added = DataConnection.AddOrUpdateConnection(candidate, persist: false);
            if (!added)
            {
                MessageBox.Show(
                    "Could not add a new connection template.",
                    "BeepDataConnection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            SetProperty("CurrentConnection", candidate);
            NotifyPropertyRefresh("DataConnections");
            NotifyPropertyRefresh("CurrentConnection");

            MessageBox.Show(
                "Connection template added.\n\nEdit details in Connections Collection, then use Save Connections.",
                "BeepDataConnection",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
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
            NotifyPropertyRefresh("DataConnections");
            NotifyPropertyRefresh("CurrentConnection");

            MessageBox.Show(
                removed
                    ? $"Removed '{currentName}'. Use Save Connections to persist."
                    : $"Could not remove '{currentName}'.",
                "BeepDataConnection",
                MessageBoxButtons.OK,
                removed ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        public void ShowConnectionSummary()
        {
            if (DataConnection == null)
            {
                return;
            }

            var connections = DataConnection.ReloadConnections();
            var currentName = DataConnection.CurrentConnection?.ConnectionName ?? "(none)";

            MessageBox.Show(
                $"Connections: {connections.Count}\nCurrent: {currentName}",
                "BeepDataConnection Summary",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
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
