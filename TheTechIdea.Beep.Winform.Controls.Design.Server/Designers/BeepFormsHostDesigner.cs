using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Services;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms.Models;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Editors;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public sealed class BeepFormsHostDesigner : BaseBeepParentControlDesigner
    {
        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            return new DesignerActionListCollection
            {
                new BeepFormsHostActionList(this)
            };
        }
    }

    public sealed class BeepFormsHostActionList : DesignerActionList
    {
        private readonly BeepFormsHostDesigner _designer;

        public BeepFormsHostActionList(BeepFormsHostDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        public string FormName
        {
            get => _designer.GetProperty<string>(nameof(BeepForms.FormName)) ?? string.Empty;
            set => _designer.SetProperty(nameof(BeepForms.FormName), value);
        }

        public bool AutoCreateBlocksFromDefinition
        {
            get => _designer.GetProperty<bool>(nameof(BeepForms.AutoCreateBlocksFromDefinition));
            set => _designer.SetProperty(nameof(BeepForms.AutoCreateBlocksFromDefinition), value);
        }

        public BeepFormsDefinition? Definition
        {
            get => _designer.GetProperty<BeepFormsDefinition>(nameof(BeepForms.Definition));
            set => _designer.SetProperty(nameof(BeepForms.Definition), value);
        }

        public BeepDataConnection? DataConnection
        {
            get => _designer.GetProperty<BeepDataConnection>(nameof(BeepForms.DataConnection));
            set => _designer.SetProperty(nameof(BeepForms.DataConnection), value);
        }

        public void CreateStarterDefinition()
        {
            BeepFormsDefinition definition = Definition?.Clone() ?? new BeepFormsDefinition();
            string resolvedFormName = string.IsNullOrWhiteSpace(FormName) ? "BeepForm" : FormName;

            definition.FormName = resolvedFormName;
            if (string.IsNullOrWhiteSpace(definition.Title))
            {
                definition.Title = resolvedFormName;
            }

            if (definition.Blocks.Count == 0)
            {
                definition.Blocks.Add(new BeepBlockDefinition
                {
                    BlockName = "MainBlock",
                    Caption = "Main Block",
                    PresentationMode = BeepBlockPresentationMode.Record,
                    Fields =
                    {
                        new BeepFieldDefinition
                        {
                            FieldName = "Name",
                            Label = "Name",
                            EditorKey = "text",
                            ControlType = BeepFieldControlTypeRegistry.ResolveDefaultControlType("text"),
                            BindingProperty = BeepFieldControlTypeRegistry.ResolveDefaultBindingProperty(nameof(BeepTextBox), "text"),
                            Order = 0
                        }
                    }
                });
            }

            Definition = definition;
        }

        public void RebuildBlocksNow()
        {
            if (_designer.Component is BeepForms forms)
            {
                forms.RebuildBlocksFromDefinition();
            }
        }

        public void RestoreDefinitionDefaults()
        {
            AutoCreateBlocksFromDefinition = true;
            if (Definition == null)
            {
                CreateStarterDefinition();
            }
        }

        public void OpenSetupWizard()
        {
            if (_designer.Component is not BeepForms forms)
            {
                return;
            }
            using var wizard = new BeepFormsSetupWizardForm(forms);
            if (wizard.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            Definition = wizard.Result;
            if (wizard.Result.Blocks.Count > 0)
            {
                forms.RebuildBlocksFromDefinition();
            }
        }

        public void OpenFormPropertiesEditor()
        {
            BeepFormsDefinition definition = Definition?.Clone() ?? new BeepFormsDefinition { FormName = FormName };
            using var editor = new BeepFormPropertiesEditorForm(definition);
            if (editor.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            Definition = editor.Result;
        }

        public void OpenFullDefinitionEditor()
        {
            BeepFormsDefinition definition = Definition?.Clone() ?? new BeepFormsDefinition { FormName = FormName };
            using var editor = new DefinitionObjectEditorForm<BeepFormsDefinition>(
                "Edit Form Definition",
                definition,
                item => item.Clone());
            if (editor.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            Definition = editor.Result?.Clone();
        }

        public void RunValidation()
        {
            using var report = new BeepFormsValidationReportForm(Definition);
            report.ShowDialog();
        }

        public void AddBlock()
        {
            BeepFormsDefinition definition = Definition?.Clone() ?? new BeepFormsDefinition { FormName = FormName };
            int n = definition.Blocks.Count + 1;
            definition.Blocks.Add(new BeepBlockDefinition
            {
                BlockName = $"Block{n}",
                Caption = $"Block {n}",
                PresentationMode = BeepBlockPresentationMode.Record,
                Entity = new BeepBlockEntityDefinition { EntityName = string.Empty, Caption = string.Empty }
            });
            Definition = definition;
            if (_designer.Component is BeepForms forms && forms.AutoCreateBlocksFromDefinition)
            {
                forms.RebuildBlocksFromDefinition();
            }
        }

        public void EditBlocks()
        {
            OpenFullDefinitionEditor();
        }

        public void ClearAllBlocks()
        {
            BeepFormsDefinition definition = Definition?.Clone() ?? new BeepFormsDefinition { FormName = FormName };
            if (definition.Blocks.Count == 0)
            {
                return;
            }
            var confirm = MessageBox.Show(
                $"Remove all {definition.Blocks.Count} block(s) from this form?",
                "Clear Blocks",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning);
            if (confirm != DialogResult.OK)
            {
                return;
            }
            definition.Blocks.Clear();
            Definition = definition;
            if (_designer.Component is BeepForms forms)
            {
                forms.RebuildBlocksFromDefinition();
            }
        }

        public void SetAsStartupForm()
        {
            if (_designer.Component is not BeepForms forms)
            {
                return;
            }
            // Persist a hint in the metadata bag; the IDE extension / startup runner reads this.
            BeepFormsDefinition definition = Definition?.Clone() ?? new BeepFormsDefinition { FormName = FormName };
            definition.Metadata["StartupForm"] = bool.TrueString;
            Definition = definition;
            MessageBox.Show(
                $"'{FormName}' marked as startup form via definition metadata.",
                "Startup Form",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Definition"));
            items.Add(new DesignerActionPropertyItem(nameof(FormName), "Form Name", "Definition", "Logical form name used for FormsManager coordination."));
            items.Add(new DesignerActionPropertyItem(nameof(AutoCreateBlocksFromDefinition), "Auto Create Blocks", "Definition", "Automatically materialize block controls from the assigned definition."));
            items.Add(new DesignerActionPropertyItem(nameof(Definition), "Definition", "Definition", "Edit the form definition, block set, and field composition."));
            items.Add(new DesignerActionMethodItem(this, nameof(OpenFormPropertiesEditor), "Edit Form Properties...", "Definition", true));
            items.Add(new DesignerActionMethodItem(this, nameof(OpenFullDefinitionEditor), "Open Definition Editor...", "Definition", true));

            items.Add(new DesignerActionHeaderItem("Data"));
            items.Add(new DesignerActionPropertyItem(nameof(DataConnection), "Data Connection", "Data", "BeepDataConnection used at runtime by LogonAsync and shared data lookups."));
            items.Add(new DesignerActionMethodItem(this, nameof(OpenCurrentConnectionEditor), "Open Connection Editor...", "Data", true));
            items.Add(new DesignerActionMethodItem(this, nameof(TestAllConnections), "Test All Connections", "Data", true));

            items.Add(new DesignerActionHeaderItem("Blocks"));
            items.Add(new DesignerActionMethodItem(this, nameof(AddBlock), "Add Block", "Blocks", true));
            items.Add(new DesignerActionMethodItem(this, nameof(EditBlocks), "Edit Blocks...", "Blocks", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RebuildBlocksNow), "Rebuild Blocks Now", "Blocks", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ClearAllBlocks), "Clear All Blocks", "Blocks", false));

            items.Add(new DesignerActionHeaderItem("Runtime"));
            items.Add(new DesignerActionMethodItem(this, nameof(SetAsStartupForm), "Set As Startup Form", "Runtime", false));
            items.Add(new DesignerActionMethodItem(this, nameof(RunValidation), "Validate...", "Runtime", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RestoreDefinitionDefaults), "Restore Definition Defaults", "Runtime", false));

            items.Add(new DesignerActionHeaderItem("Wizard"));
            items.Add(new DesignerActionMethodItem(this, nameof(OpenSetupWizard), "New Form Wizard...", "Wizard", true));
            items.Add(new DesignerActionMethodItem(this, nameof(CreateStarterDefinition), "Create Starter Definition", "Wizard", true));

            return items;
        }

        // Forwarders used by Data smart-tag verbs (delegate to the active BeepDataConnection on the form).
        public void OpenCurrentConnectionEditor()
        {
            var connection = ResolveDataConnection();
            if (connection == null)
            {
                return;
            }
            using var dialog = new BeepConnectionEditorForm(
                connection.CurrentConnection ?? new ConnectionProperties { ConnectionName = "NewConnection" },
                ResolveEditor(connection),
                isNew: connection.CurrentConnection == null);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                connection.AddOrUpdateConnection(dialog.Result, persist: false);
            }
        }

        public void TestAllConnections()
        {
            var connection = ResolveDataConnection();
            if (connection == null || connection.DataConnections == null || connection.DataConnections.Count == 0)
            {
                return;
            }
            var editor = ResolveEditor(connection);
            var outcomes = new List<BeepConnectionTestOutcome>(connection.DataConnections.Count);
            foreach (var c in connection.DataConnections)
            {
                outcomes.Add(TestOne(c, editor));
            }
            using var report = new BeepConnectionTestReportForm(outcomes);
            report.ShowDialog();
        }

        private BeepDataConnection? ResolveDataConnection()
        {
            if (DataConnection != null)
            {
                return DataConnection;
            }
            var site = _designer.Component?.Site;
            if (site?.Container == null)
            {
                return null;
            }
            foreach (var component in site.Container.Components)
            {
                if (component is BeepDataConnection connection)
                {
                    return connection;
                }
            }
            return null;
        }

        private static IDMEEditor? ResolveEditor(BeepDataConnection connection)
        {
            return connection.BeepService?.DMEEditor;
        }

        private static BeepConnectionTestOutcome TestOne(ConnectionProperties connection, IDMEEditor? editor)
        {
            if (string.IsNullOrWhiteSpace(connection.ConnectionName))
            {
                return new BeepConnectionTestOutcome { ConnectionName = "(unnamed)", Success = false, Message = "Empty connection name." };
            }
            if (editor == null)
            {
                return new BeepConnectionTestOutcome { ConnectionName = connection.ConnectionName, Success = false, Message = "Editor not available." };
            }
            var sw = System.Diagnostics.Stopwatch.StartNew();
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
    }
}
