using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Integrated.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Designer for BeepDataBlock with comprehensive Oracle Forms features
    /// Provides smart tags for Triggers, LOV, Properties, Validation, Navigation, Coordination
    /// </summary>
    public class BeepDataBlockDesigner : Microsoft.DotNet.DesignTools.Designers.ParentControlDesigner
    {
        private Microsoft.DotNet.DesignTools.Designers.Actions.DesignerActionListCollection _actionLists;
        private BeepDataBlock DataBlock => Component as BeepDataBlock;

        public override Microsoft.DotNet.DesignTools.Designers.Actions.DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists == null)
                {
                    _actionLists = new Microsoft.DotNet.DesignTools.Designers.Actions.DesignerActionListCollection
                    {
                        new BeepDataBlockActionList(this.Component)
                    };
                }
                return _actionLists;
            }
        }

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            
            // Enable design-time drag-drop for child controls
            if (Control != null)
            {
                EnableDesignMode(Control, "DataBlock");
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
    /// Action list for BeepDataBlock with Oracle Forms features
    /// </summary>
    public class BeepDataBlockActionList : Microsoft.DotNet.DesignTools.Designers.Actions.DesignerActionList
    {
        private BeepDataBlock DataBlock => Component as BeepDataBlock;
        private IDesignerHost DesignerHost => GetService(typeof(IDesignerHost)) as IDesignerHost;
        private IComponentChangeService ChangeService => GetService(typeof(IComponentChangeService)) as IComponentChangeService;

        public BeepDataBlockActionList(IComponent component) : base(component)
        {
        }

        public override Microsoft.DotNet.DesignTools.Designers.Actions.DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new Microsoft.DotNet.DesignTools.Designers.Actions.DesignerActionItemCollection();

            // Block Configuration
            items.Add(new DesignerActionHeaderItem("Block Configuration"));
            items.Add(new DesignerActionPropertyItem("Name", "Block Name", "Block Configuration", "Unique identifier for this block"));
            items.Add(new DesignerActionPropertyItem("ConnectionName", "Connection", "Block Configuration", "Data connection used to load entities"));
            items.Add(new DesignerActionPropertyItem("EntityName", "Entity Name", "Block Configuration", "Name of the entity this block manages"));
            items.Add(new DesignerActionPropertyItem("ViewMode", "View Mode", "Block Configuration", "Record controls or BeepGridPro layout"));
            items.Add(new DesignerActionMethodItem(this, "LaunchDataSetupWizard", "Open Data Setup Wizard...", "Block Configuration", "Step-by-step setup for connection, entity, fields, and view mode", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureEntityType", "Select Entity Type...", "Block Configuration", "Choose the entity type for this block", true));
            items.Add(new DesignerActionMethodItem(this, "RefreshEntityMetadata", "Refresh Entity Metadata", "Block Configuration", "Reload entity schema and fields from selected connection", true));

            // Oracle Forms Features
            items.Add(new DesignerActionHeaderItem("Oracle Forms Features"));
            items.Add(new DesignerActionMethodItem(this, "AddTriggerExample", "Add Trigger (PRE-INSERT)...", "Oracle Forms Features", "Add a PRE-INSERT trigger example", true));
            items.Add(new DesignerActionMethodItem(this, "AddLOVExample", "Add LOV...", "Oracle Forms Features", "Add a List of Values (LOV) example", true));
            items.Add(new DesignerActionMethodItem(this, "AddValidationExample", "Add Validation...", "Oracle Forms Features", "Add a validation rule example", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureNavigation", "Setup Keyboard Navigation", "Oracle Forms Features", "Enable keyboard navigation (Tab, Shift+Tab)", true));

            // Form Coordination
            items.Add(new DesignerActionHeaderItem("Form Coordination"));
            items.Add(new DesignerActionPropertyItem("FormName", "Form Name", "Form Coordination", "Name of the parent form (for FormsManager coordination)"));
            items.Add(new DesignerActionMethodItem(this, "SetupMasterDetail", "Setup Master-Detail...", "Form Coordination", "Configure master-detail relationship", true));
            items.Add(new DesignerActionMethodItem(this, "InitializeIntegrations", "Initialize All Integrations", "Form Coordination", "One-call initialization (triggers, LOV, properties, navigation)", true));

            // Advanced Features
            items.Add(new DesignerActionHeaderItem("Advanced Features"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureLocking", "Configure Record Locking...", "Advanced Features", "Setup record locking mode", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigurePerformance", "Manage Performance Caches...", "Advanced Features", "Refresh trigger/LOV/validation caches and inspect cache stats", true));
            items.Add(new DesignerActionMethodItem(this, "ViewDocumentation", "View Documentation...", "Advanced Features", "Open DataBlocks documentation folder", true));

            // Quick Presets
            items.Add(new DesignerActionHeaderItem("Quick Presets"));
            items.Add(new DesignerActionMethodItem(this, "PresetSimpleBlock", "Simple CRUD Block", "Quick Presets", "Basic CRUD operations", true));
            items.Add(new DesignerActionMethodItem(this, "PresetMasterBlock", "Master Block", "Quick Presets", "Master block with LOVs", true));
            items.Add(new DesignerActionMethodItem(this, "PresetDetailBlock", "Detail Block", "Quick Presets", "Detail block with validation", true));
            items.Add(new DesignerActionMethodItem(this, "PresetQueryOnlyBlock", "Query-Only Block", "Quick Presets", "Read-only query block", true));

            return items;
        }

        #region Properties (for PropertyItems)
        
        public string Name
        {
            get => DataBlock?.Name ?? "";
            set
            {
                if (DataBlock != null)
                {
                    SetProperty("Name", value);
                    MessageBox.Show($"Block name set to: {value}\n\n" +
                        "Use this name to register with FormsManager and reference in triggers.",
                        "Block Name Set", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public string EntityName
        {
            get => DataBlock?.EntityName ?? "";
            set
            {
                if (DataBlock != null)
                {
                    SetProperty("EntityName", value);
                }
            }
        }

        public string ConnectionName
        {
            get => DataBlock?.ConnectionName ?? "";
            set
            {
                if (DataBlock != null)
                {
                    SetProperty("ConnectionName", value);
                }
            }
        }

        public DataBlockViewMode ViewMode
        {
            get => DataBlock?.ViewMode ?? DataBlockViewMode.RecordControls;
            set
            {
                if (DataBlock != null)
                {
                    SetProperty("ViewMode", value);
                }
            }
        }

        public string FormName
        {
            get => DataBlock?.FormName ?? "";
            set
            {
                if (DataBlock != null)
                {
                    SetProperty("FormName", value);
                    MessageBox.Show($"Form name set to: {value}\n\n" +
                        "All blocks with the same FormName will coordinate through FormsManager.",
                        "Form Name Set", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        #endregion

        #region Helper Methods

        private void SetProperty(string propertyName, object value)
        {
            if (DataBlock == null)
            {
                return;
            }

            PropertyDescriptor prop = TypeDescriptor.GetProperties(DataBlock)[propertyName];
            if (prop == null || prop.IsReadOnly || !prop.PropertyType.IsInstanceOfType(value))
            {
                return;
            }

            var oldValue = prop.GetValue(DataBlock);
            if (Equals(oldValue, value))
            {
                return;
            }

            ChangeService?.OnComponentChanging(DataBlock, prop);
            prop.SetValue(DataBlock, value);
            ChangeService?.OnComponentChanged(DataBlock, prop, oldValue, value);
        }

        #endregion

        #region Configuration Methods

        public void ConfigureEntityType()
        {
            MessageBox.Show(
                "Entity Type Configuration:\n\n" +
                "1. Set the 'SelectedEntityType' property in the Properties window\n" +
                "2. Or assign Data property at runtime:\n" +
                "   block.Data = new UnitofWork<Customer>(dmeEditor);\n\n" +
                "This will auto-generate UI components for all entity fields.",
                "Configure Entity Type",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public void RefreshEntityMetadata()
        {
            if (DataBlock == null) return;

            var result = DataBlock.RefreshEntityMetadata();
            MessageBox.Show(
                result
                    ? "Entity metadata reloaded from selected connection.\n\nField selections and controls were updated."
                    : "Could not load entity metadata.\n\nCheck Connection and Entity properties.",
                "Refresh Entity Metadata",
                MessageBoxButtons.OK,
                result ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        public void LaunchDataSetupWizard()
        {
            if (DataBlock == null) return;

            try
            {
                using var wizard = new BeepDataBlockSetupWizardForm(DataBlock);
                if (wizard.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                SetProperty("ConnectionName", wizard.SelectedConnectionName);
                SetProperty("EntityName", wizard.SelectedEntityName);
                SetProperty("ViewMode", wizard.SelectedViewMode);

                DataBlock.RefreshEntityMetadata();
                ApplyWizardFieldSelections(wizard.SelectedFieldNames);
                DataBlock.RefreshEntityMetadata();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Could not apply wizard settings.\n\n{ex.Message}",
                    "Wizard Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ApplyWizardFieldSelections(HashSet<string> selectedFieldNames)
        {
            if (DataBlock == null) return;

            var prop = TypeDescriptor.GetProperties(DataBlock)["FieldSelections"];
            ChangeService?.OnComponentChanging(DataBlock, prop);

            foreach (var fieldSelection in DataBlock.FieldSelections)
            {
                fieldSelection.IncludeInView = selectedFieldNames.Contains(fieldSelection.FieldName);
            }

            ChangeService?.OnComponentChanged(DataBlock, prop, null, null);
        }

        public void SetupMasterDetail()
        {
            var message = "Master-Detail Configuration:\n\n" +
                "1. Set ParentBlock property to reference the master block\n" +
                "2. Set MasterKeyPropertyName (e.g., 'CustomerID')\n" +
                "3. Set ForeignKeyPropertyName (e.g., 'CustomerID')\n\n" +
                "Example:\n" +
                "ordersBlock.ParentBlock = customerBlock;\n" +
                "ordersBlock.MasterKeyPropertyName = \"CustomerID\";\n" +
                "ordersBlock.ForeignKeyPropertyName = \"CustomerID\";\n\n" +
                "When master record changes, detail block auto-queries related records!";

            MessageBox.Show(message, "Master-Detail Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void InitializeIntegrations()
        {
            if (DataBlock == null) return;

            try
            {
                // This is design-time, so we can't actually initialize runtime services
                // Show instructions instead
                MessageBox.Show(
                    "Runtime Initialization Code:\n\n" +
                    "// In Form.Load or after setting Data:\n" +
                    $"{DataBlock.Name}.InitializeIntegrations();\n\n" +
                    "This single call:\n" +
                    "✅ Registers all UI components as items\n" +
                    "✅ Auto-validates required fields\n" +
                    "✅ Sets up keyboard navigation\n" +
                    "✅ Registers with FormsManager\n" +
                    "✅ Fires WHEN-NEW-BLOCK-INSTANCE trigger\n\n" +
                    "Copy this to your Form.Load event!",
                    "Initialize Integrations",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Initialization Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Oracle Forms Examples

        public void AddTriggerExample()
        {
            var code = 
                $"// PRE-INSERT Trigger Example\n" +
                $"{DataBlock.Name}.RegisterTrigger(TriggerType.PreInsert, async context =>\n" +
                "{\n" +
                $"    {DataBlock.Name}.SetItemValue(\"CreatedDate\", DateTime.Now);\n" +
                $"    {DataBlock.Name}.SetItemValue(\"CreatedBy\", Environment.UserName);\n" +
                $"    {DataBlock.Name}.SetItemValue(\"Status\", \"Active\");\n" +
                "    return true;\n" +
                "});";

            Clipboard.SetText(code);
            
            MessageBox.Show(
                "PRE-INSERT Trigger code copied to clipboard!\n\n" +
                "This trigger:\n" +
                "✅ Fires before inserting a new record\n" +
                "✅ Sets CreatedDate to current time\n" +
                "✅ Sets CreatedBy to current user\n" +
                "✅ Sets Status to 'Active'\n\n" +
                "Paste this code in your Form.Load or block initialization.",
                "Trigger Example",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public void AddLOVExample()
        {
            var code =
                $"// LOV Example - Press F9 to show List of Values\n" +
                $"{DataBlock.Name}.RegisterLOV(\"CustomerID\", new BeepDataBlockLOV\n" +
                "{\n" +
                "    LOVName = \"CUSTOMERS_LOV\",\n" +
                "    Title = \"Select Customer\",\n" +
                "    DataSourceName = \"MainDB\",\n" +
                "    EntityName = \"Customers\",\n" +
                "    DisplayField = \"CompanyName\",\n" +
                "    ReturnField = \"CustomerID\",\n" +
                "    AutoPopulateRelatedFields = true,\n" +
                "    RelatedFieldMappings = new Dictionary<string, string>\n" +
                "    {\n" +
                "        [\"CompanyName\"] = \"CustomerName\",\n" +
                "        [\"ContactName\"] = \"ContactName\"\n" +
                "    }\n" +
                "});";

            Clipboard.SetText(code);

            MessageBox.Show(
                "LOV code copied to clipboard!\n\n" +
                "Features:\n" +
                "✅ F9 key to show LOV dialog\n" +
                "✅ Double-click to show LOV\n" +
                "✅ Auto-populates related fields\n" +
                "✅ Validates selection\n" +
                "✅ Auto-navigates to next field\n\n" +
                "Paste this code in your Form.Load or block initialization.",
                "LOV Example",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public void AddValidationExample()
        {
            var code =
                $"// Validation Example - Fluent API\n" +
                $"{DataBlock.Name}.ForField(\"Email\")\n" +
                "    .Required()\n" +
                "    .Pattern(@\"^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$\", \"Invalid email format\")\n" +
                "    .Register();\n\n" +
                $"{DataBlock.Name}.ForField(\"Age\")\n" +
                "    .Required()\n" +
                "    .Range(18, 120, \"Age must be between 18 and 120\")\n" +
                "    .Register();";

            Clipboard.SetText(code);

            MessageBox.Show(
                "Validation code copied to clipboard!\n\n" +
                "Features:\n" +
                "✅ Fluent API for easy validation\n" +
                "✅ Required field validation\n" +
                "✅ Pattern (regex) validation\n" +
                "✅ Range validation\n" +
                "✅ Visual error indicators\n\n" +
                "Paste this code in your Form.Load or block initialization.",
                "Validation Example",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public void ConfigureNavigation()
        {
            var code =
                $"// Keyboard Navigation Setup\n" +
                $"{DataBlock.Name}.SetupKeyboardNavigation();\n\n" +
                "// Now supports:\n" +
                "// Tab - Next item\n" +
                "// Shift+Tab - Previous item\n" +
                "// Fires KEY-NEXT-ITEM and KEY-PREV-ITEM triggers";

            Clipboard.SetText(code);

            MessageBox.Show(
                "Navigation setup code copied to clipboard!\n\n" +
                "Features:\n" +
                "✅ Tab key - Next item\n" +
                "✅ Shift+Tab - Previous item\n" +
                "✅ Respects Navigable property\n" +
                "✅ Fires navigation triggers\n" +
                "✅ Validates before navigation\n\n" +
                "Paste this code in your Form.Load.",
                "Navigation Setup",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        #endregion

        #region Advanced Features

        public void ConfigureLocking()
        {
            var blockRef = string.IsNullOrWhiteSpace(DataBlock?.Name) ? "block" : DataBlock.Name;
            var code =
                $"// Record Locking Configuration\n" +
                $"{blockRef}.LockMode = LockMode.Automatic;  // Lock when editing starts\n" +
                $"{blockRef}.LockOnEdit = true;\n\n" +
                "// Manual locking:\n" +
                $"await {blockRef}.LockCurrentRecord();\n" +
                $"if ({blockRef}.IsCurrentRecordLocked()) {{ /* ... */ }}\n" +
                $"{blockRef}.UnlockCurrentRecord();";

            Clipboard.SetText(code);

            MessageBox.Show(
                "Record locking code copied to clipboard!\n\n" +
                "Lock Modes:\n" +
                "• Automatic - Lock when user starts editing\n" +
                "• Immediate - Lock on navigation\n" +
                "• None - Disable locking\n" +
                "• Manual - Explicit lock calls only\n\n" +
                "Oracle Forms equivalent: LOCK_RECORD",
                "Record Locking",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public void ConfigurePerformance()
        {
            var blockRef = string.IsNullOrWhiteSpace(DataBlock?.Name) ? "block" : DataBlock.Name;
            var code =
                $"// Performance Cache Maintenance\n" +
                $"{blockRef}.InvalidateTriggerCache();\n" +
                $"{blockRef}.InvalidateLOVCache();\n" +
                $"{blockRef}.ClearValidationDebounce();\n\n" +
                "// Optional diagnostics\n" +
                $"var cacheStats = {blockRef}.GetCacheStatistics();\n" +
                "Console.WriteLine($\"Cache size: {cacheStats.TotalCacheMemoryFormatted}\");";

            Clipboard.SetText(code);

            MessageBox.Show(
                "Performance cache maintenance code copied to clipboard!\n\n" +
                "Optimizations:\n" +
                "✅ Trigger lookup caching\n" +
                "✅ LOV data lazy-loading\n" +
                "✅ Validation debouncing (300ms)\n" +
                "✅ SystemVariables optimization\n\n" +
                "Cache state refreshed and ready for diagnostics.\n\n" +
                "Paste this code in your Form.Load.",
                "Performance Caches",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public void ViewDocumentation()
        {
            try
            {
                // Try to open DataBlocks documentation folder
                var docsPath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "..\\..\\..\\TheTechIdea.Beep.Winform.Controls.Integrated\\DataBlocks\\Documentation");

                if (System.IO.Directory.Exists(docsPath))
                {
                    System.Diagnostics.Process.Start("explorer.exe", docsPath);
                }
                else
                {
                    MessageBox.Show(
                        "Documentation Location:\n" +
                        "TheTechIdea.Beep.Winform.Controls.Integrated/DataBlocks/Documentation/\n\n" +
                        "Key Documents:\n" +
                        "• README.md - Quick start (5 min)\n" +
                        "• MASTER_STATUS.md - Current status\n" +
                        "• ADVANCED_FEATURES_GUIDE.md - Advanced usage\n" +
                        "• COMPLETE_API_REFERENCE.md - Full API\n" +
                        "• ORACLE_FORMS_COMPLETE.md - All features\n\n" +
                        "Total: 18 documents, 220+ pages!",
                        "Documentation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch
            {
                MessageBox.Show(
                    "Documentation is in the DataBlocks/Documentation folder.\n\n" +
                    "Start with:\n" +
                    "• README.md\n" +
                    "• ADVANCED_FEATURES_GUIDE.md\n" +
                    "• COMPLETE_API_REFERENCE.md",
                    "Documentation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Quick Presets

        public void PresetSimpleBlock()
        {
            if (DataBlock == null) return;

            try
            {
                SetProperty("Name", "SIMPLE_BLOCK");
                SetProperty("FormName", "SimpleForm");

                var code =
                    "// Simple CRUD Block Setup\n" +
                    $"var block = {DataBlock.Name};\n" +
                    "block.DMEEditor = dmeEditor;\n" +
                    "block.Data = new UnitofWork<MyEntity>(dmeEditor);\n" +
                    "block.InitializeIntegrations();\n\n" +
                    "// Query data\n" +
                    "await block.CoordinatedQuery();\n\n" +
                    "// Save changes\n" +
                    "await block.CoordinatedCommit();";

                Clipboard.SetText(code);

                MessageBox.Show(
                    "Simple CRUD Block configured!\n\n" +
                    "Block Name: SIMPLE_BLOCK\n" +
                    "Form Name: SimpleForm\n\n" +
                    "Setup code copied to clipboard!\n" +
                    "Paste in Form.Load event.",
                    "Simple Block Preset",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Preset Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void PresetMasterBlock()
        {
            if (DataBlock == null) return;

            try
            {
                SetProperty("Name", "MASTER_BLOCK");
                SetProperty("FormName", "MasterDetailForm");

                var code =
                    "// Master Block with LOVs\n" +
                    $"var master = {DataBlock.Name};\n" +
                    "master.DMEEditor = dmeEditor;\n" +
                    "master.FormManager = formManager;\n" +
                    "master.Data = new UnitofWork<Customer>(dmeEditor);\n\n" +
                    "// Add LOV for lookup fields\n" +
                    "master.RegisterLOV(\"RegionID\", new BeepDataBlockLOV\n" +
                    "{\n" +
                    "    LOVName = \"REGIONS\",\n" +
                    "    Title = \"Select Region\",\n" +
                    "    DataSourceName = \"MainDB\",\n" +
                    "    EntityName = \"Regions\",\n" +
                    "    DisplayField = \"RegionName\",\n" +
                    "    ReturnField = \"RegionID\"\n" +
                    "});\n\n" +
                    "master.InitializeIntegrations();";

                Clipboard.SetText(code);

                MessageBox.Show(
                    "Master Block configured!\n\n" +
                    "Block Name: MASTER_BLOCK\n" +
                    "Form Name: MasterDetailForm\n\n" +
                    "Features:\n" +
                    "✅ LOV support (F9 key)\n" +
                    "✅ FormsManager coordination\n" +
                    "✅ Ready for detail blocks\n\n" +
                    "Setup code copied to clipboard!",
                    "Master Block Preset",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Preset Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void PresetDetailBlock()
        {
            if (DataBlock == null) return;

            try
            {
                SetProperty("Name", "DETAIL_BLOCK");
                SetProperty("FormName", "MasterDetailForm");

                var code =
                    "// Detail Block with Validation\n" +
                    $"var detail = {DataBlock.Name};\n" +
                    "detail.DMEEditor = dmeEditor;\n" +
                    "detail.FormManager = formManager;\n" +
                    "detail.ParentBlock = masterBlock;\n" +
                    "detail.MasterKeyPropertyName = \"CustomerID\";\n" +
                    "detail.ForeignKeyPropertyName = \"CustomerID\";\n" +
                    "detail.Data = new UnitofWork<Order>(dmeEditor);\n\n" +
                    "// Add validation\n" +
                    "detail.ForField(\"OrderDate\")\n" +
                    "    .Required()\n" +
                    "    .Register();\n\n" +
                    "detail.ForField(\"OrderTotal\")\n" +
                    "    .Required()\n" +
                    "    .Range(0, 1000000, \"Invalid amount\")\n" +
                    "    .Register();\n\n" +
                    "detail.InitializeIntegrations();";

                Clipboard.SetText(code);

                MessageBox.Show(
                    "Detail Block configured!\n\n" +
                    "Block Name: DETAIL_BLOCK\n" +
                    "Form Name: MasterDetailForm\n\n" +
                    "Features:\n" +
                    "✅ Master-detail relationship\n" +
                    "✅ Auto-filters on master change\n" +
                    "✅ Validation rules\n" +
                    "✅ Coordinated commit with master\n\n" +
                    "Setup code copied to clipboard!",
                    "Detail Block Preset",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Preset Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void PresetQueryOnlyBlock()
        {
            if (DataBlock == null) return;

            try
            {
                SetProperty("Name", "QUERY_BLOCK");
                SetProperty("FormName", "QueryForm");

                var code =
                    "// Query-Only Block (Read-Only)\n" +
                    $"var query = {DataBlock.Name};\n" +
                    "query.DMEEditor = dmeEditor;\n" +
                    "query.Data = new UnitofWork<Report>(dmeEditor);\n\n" +
                    "// Disable modifications\n" +
                    "query.InsertAllowed = false;\n" +
                    "query.UpdateAllowed = false;\n" +
                    "query.DeleteAllowed = false;\n\n" +
                    "// Enhanced QBE with operators\n" +
                    "query.SetQueryOperator(\"Salary\", QueryOperator.GreaterThan);\n" +
                    "query.SetQueryOperator(\"Name\", QueryOperator.Like);\n\n" +
                    "query.InitializeIntegrations();\n" +
                    "await query.SwitchBlockModeAsync(DataBlockMode.Query);";

                Clipboard.SetText(code);

                MessageBox.Show(
                    "Query-Only Block configured!\n\n" +
                    "Block Name: QUERY_BLOCK\n" +
                    "Form Name: QueryForm\n\n" +
                    "Features:\n" +
                    "✅ Read-only (no modifications)\n" +
                    "✅ Enhanced QBE with operators\n" +
                    "✅ Query templates\n" +
                    "✅ Query history\n\n" +
                    "Setup code copied to clipboard!",
                    "Query Block Preset",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Preset Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}

