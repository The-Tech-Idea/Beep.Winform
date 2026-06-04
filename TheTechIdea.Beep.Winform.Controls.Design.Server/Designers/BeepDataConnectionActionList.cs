using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public sealed class BeepDataConnectionActionList : DesignerActionList
    {
        private readonly BeepDataConnectionDesigner _designer;

        public BeepDataConnectionActionList(BeepDataConnectionDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        public BeepDataConnection? Connection
        {
            get => Component as BeepDataConnection;
        }

        public ConnectionProperties? CurrentConnection
        {
            get => _designer.GetProperty<ConnectionProperties>(nameof(BeepDataConnection.CurrentConnection));
            set => _designer.SetProperty(nameof(BeepDataConnection.CurrentConnection), value);
        }

        public string PersistenceScope
        {
            get => _designer.GetProperty<ConnectionStorageScope>(nameof(BeepDataConnection.PersistenceScope)).ToString();
            set
        {
            if (Enum.TryParse<ConnectionStorageScope>(value, out var parsed))
            {
                _designer.SetProperty(nameof(BeepDataConnection.PersistenceScope), parsed);
            }
        }
        }

        public void OpenCurrentConnectionEditor()
        {
            var owner = Connection;
            if (owner == null)
            {
                return;
            }
            var current = owner.CurrentConnection ?? new ConnectionProperties { ConnectionName = "NewConnection" };
            using var dialog = new BeepConnectionEditorForm(current, _designer.GetEditor(), isNew: owner.CurrentConnection == null);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                owner.AddOrUpdateConnection(dialog.Result, persist: false);
                CurrentConnection = dialog.Result;
            }
        }

        public void OpenConnectionListEditor()
        {
            var owner = Connection;
            if (owner == null)
            {
                return;
            }
            using var dialog = new BeepConnectionListEditorForm(owner, _designer.GetEditor());
            dialog.ShowDialog();
        }

        public void TestCurrentConnection()
        {
            var owner = Connection;
            if (owner?.CurrentConnection == null)
            {
                return;
            }
            var outcome = new List<BeepConnectionTestOutcome>
            {
                TestOne(owner.CurrentConnection)
            };
            using var report = new BeepConnectionTestReportForm(outcome);
            report.ShowDialog();
        }

        public void TestAllConnections()
        {
            var owner = Connection;
            if (owner == null || owner.DataConnections == null || owner.DataConnections.Count == 0)
            {
                return;
            }
            var outcomes = new List<BeepConnectionTestOutcome>(owner.DataConnections.Count);
            foreach (var c in owner.DataConnections)
            {
                outcomes.Add(TestOne(c));
            }
            using var report = new BeepConnectionTestReportForm(outcomes);
            report.ShowDialog();
        }

        public void SaveConnectionsNow()
        {
            Connection?.SaveConnections();
        }

        public void ReloadConnectionsNow()
        {
            Connection?.ReloadConnections();
        }

        public void AddConnection()
        {
            var owner = Connection;
            if (owner == null)
            {
                return;
            }
            var newConnection = new ConnectionProperties { ConnectionName = "NewConnection" };
            using var dialog = new BeepConnectionEditorForm(newConnection, _designer.GetEditor(), isNew: true);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                owner.AddOrUpdateConnection(dialog.Result, persist: false);
            }
        }

        public void PromoteCurrentToShared()
        {
            var owner = Connection;
            if (owner == null)
            {
                return;
            }
            using var dialog = new ConflictPolicyDialog("Promote connections to shared store", showImportMode: false);
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            if (owner.PromoteCurrentToShared(dialog.SelectedPolicy, out var message))
            {
                MessageBox.Show($"Promoted: {message}", "Promote", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(message, "Promote failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void DemoteSharedToProject()
        {
            var owner = Connection;
            if (owner == null)
            {
                return;
            }
            using var dialog = new ConflictPolicyDialog("Demote shared connections to project", showImportMode: false);
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            if (owner.DemoteSharedToProject(dialog.SelectedPolicy, out var message))
            {
                MessageBox.Show($"Demoted: {message}", "Demote", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(message, "Demote failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void ExportEmbeddedDefaults()
        {
            var owner = Connection;
            if (owner == null)
            {
                return;
            }
            using var saveDialog = new SaveFileDialog
            {
                Title = "Export embedded defaults",
                Filter = "Connection package (*.json)|*.json",
                FileName = "BeepConnections.json"
            };
            if (saveDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            if (owner.ExportEmbeddedDefaults(saveDialog.FileName, includeEncryptedSecretsOnly: false, out var message))
            {
                MessageBox.Show($"Exported to {saveDialog.FileName}", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(message, "Export failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void ImportEmbeddedDefaults()
        {
            var owner = Connection;
            if (owner == null)
            {
                return;
            }
            using var openDialog = new OpenFileDialog
            {
                Title = "Import embedded defaults",
                Filter = "Connection package (*.json)|*.json"
            };
            if (openDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            using var policy = new ConflictPolicyDialog("Import embedded defaults", showImportMode: true);
            if (policy.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            if (owner.ImportEmbeddedDefaults(openDialog.FileName, policy.SelectedPolicy, policy.ImportWhenEmptyOnly, out var message))
            {
                MessageBox.Show($"Imported from {Path.GetFileName(openDialog.FileName)}", "Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(message, "Import failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void OpenLogFolder()
        {
            try
            {
                var path = Path.Combine(AppContext.BaseDirectory, "logs");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open log folder: {ex.Message}", "Log Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void ResetToFactoryDefaults()
        {
            var confirm = MessageBox.Show(
                "Reset connection storage settings to factory defaults? This does not delete saved connections.",
                "Reset",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning);
            if (confirm != DialogResult.OK)
            {
                return;
            }
            _designer.SetProperty(nameof(BeepDataConnection.PersistenceScope), ConnectionStorageScope.Project);
            _designer.SetProperty(nameof(BeepDataConnection.ActiveProfileName), "Default");
            _designer.SetProperty(nameof(BeepDataConnection.UseScopePrecedence), true);
            _designer.SetProperty(nameof(BeepDataConnection.RequireSuccessfulTestBeforeSave), true);
        }

        private BeepConnectionTestOutcome TestOne(ConnectionProperties connection)
        {
            if (string.IsNullOrWhiteSpace(connection.ConnectionName))
            {
                return new BeepConnectionTestOutcome { ConnectionName = "(unnamed)", Success = false, Message = "Empty connection name." };
            }
            var editor = _designer.GetEditor();
            if (editor == null)
            {
                return new BeepConnectionTestOutcome { ConnectionName = connection.ConnectionName, Success = false, Message = "Editor not available." };
            }
            var sw = Stopwatch.StartNew();
            try
            {
                var ds = editor.GetDataSource(connection.ConnectionName);
                if (ds == null)
                {
                    sw.Stop();
                    return new BeepConnectionTestOutcome
                    {
                        ConnectionName = connection.ConnectionName,
                        DriverName = connection.DriverName,
                        Success = false,
                        LatencyMs = sw.ElapsedMilliseconds,
                        Message = "Datasource not registered."
                    };
                }
                var state = ds.Openconnection();
                sw.Stop();
                return new BeepConnectionTestOutcome
                {
                    ConnectionName = connection.ConnectionName,
                    DriverName = connection.DriverName,
                    Success = state == System.Data.ConnectionState.Open,
                    LatencyMs = sw.ElapsedMilliseconds,
                    Message = state == System.Data.ConnectionState.Open ? "Connected." : $"State: {state}."
                };
            }
            catch (Exception ex)
            {
                sw.Stop();
                return new BeepConnectionTestOutcome
                {
                    ConnectionName = connection.ConnectionName,
                    DriverName = connection.DriverName,
                    Success = false,
                    LatencyMs = sw.ElapsedMilliseconds,
                    Message = $"{ex.GetType().Name}: {ex.Message}"
                };
            }
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Current Connection"));
            items.Add(new DesignerActionPropertyItem(nameof(CurrentConnection), "Current Connection", "Current Connection", "Connection used by bound blocks for logon/queries."));
            items.Add(new DesignerActionMethodItem(this, nameof(OpenCurrentConnectionEditor), "Open Current In Editor...", "Current Connection", true));
            items.Add(new DesignerActionMethodItem(this, nameof(TestCurrentConnection), "Test Current...", "Current Connection", true));

            items.Add(new DesignerActionHeaderItem("Connections"));
            items.Add(new DesignerActionMethodItem(this, nameof(AddConnection), "Add Connection...", "Connections", true));
            items.Add(new DesignerActionMethodItem(this, nameof(OpenConnectionListEditor), "Edit Connections...", "Connections", true));
            items.Add(new DesignerActionMethodItem(this, nameof(TestAllConnections), "Test All...", "Connections", true));
            items.Add(new DesignerActionMethodItem(this, nameof(SaveConnectionsNow), "Save Now", "Connections", false));
            items.Add(new DesignerActionMethodItem(this, nameof(ReloadConnectionsNow), "Reload From Disk", "Connections", false));

            items.Add(new DesignerActionHeaderItem("Persistence"));
            items.Add(new DesignerActionPropertyItem(nameof(PersistenceScope), "Scope", "Persistence", "Project / User / Machine storage scope."));
            items.Add(new DesignerActionMethodItem(this, nameof(ExportEmbeddedDefaults), "Export Embedded Defaults...", "Persistence", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ImportEmbeddedDefaults), "Import Embedded Defaults...", "Persistence", true));

            items.Add(new DesignerActionHeaderItem("Sharing"));
            items.Add(new DesignerActionMethodItem(this, nameof(PromoteCurrentToShared), "Promote To Shared...", "Sharing", true));
            items.Add(new DesignerActionMethodItem(this, nameof(DemoteSharedToProject), "Demote Shared To Project...", "Sharing", true));

            items.Add(new DesignerActionHeaderItem("Diagnostics"));
            items.Add(new DesignerActionMethodItem(this, nameof(OpenLogFolder), "Open Log Folder", "Diagnostics", false));
            items.Add(new DesignerActionMethodItem(this, nameof(ResetToFactoryDefaults), "Reset To Factory Defaults", "Diagnostics", false));

            return items;
        }
    }
}
