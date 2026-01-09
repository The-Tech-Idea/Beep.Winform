using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Winform.Controls.GridX.Layouts;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepGridPro control
    /// Provides smart tags for grid configuration, data binding, and styling
    /// </summary>
    public class BeepGridProDesigner : BaseBeepControlDesigner
    {
        public BeepGridPro? Grid => Component as BeepGridPro;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepGridProActionList(this));
            return lists;
        }
    }

    /// <summary>
    /// Action list for BeepGridPro smart tags
    /// Provides quick configuration presets and common property access
    /// </summary>
    public class BeepGridProActionList : DesignerActionList
    {
        private readonly BeepGridProDesigner _designer;

        public BeepGridProActionList(BeepGridProDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepGridPro? Grid => Component as BeepGridPro;

        #region Properties (for smart tags)

        [Category("Data")]
        [Description("Data source for the grid")]
        public object? DataSource
        {
            get => _designer.GetProperty<object>("DataSource");
            set => _designer.SetProperty("DataSource", value);
        }

        [Category("Data")]
        [Description("Data member for the grid")]
        public string DataMember
        {
            get => _designer.GetProperty<string>("DataMember") ?? string.Empty;
            set => _designer.SetProperty("DataMember", value);
        }

        [Category("Layout")]
        [Description("Height of each row in pixels")]
        public int RowHeight
        {
            get => _designer.GetProperty<int>("RowHeight");
            set => _designer.SetProperty("RowHeight", value);
        }

        [Category("Layout")]
        [Description("Height of column headers in pixels")]
        public int ColumnHeaderHeight
        {
            get => _designer.GetProperty<int>("ColumnHeaderHeight");
            set => _designer.SetProperty("ColumnHeaderHeight", value);
        }

        [Category("Layout")]
        [Description("Show column headers")]
        public bool ShowColumnHeaders
        {
            get => _designer.GetProperty<bool>("ShowColumnHeaders");
            set => _designer.SetProperty("ShowColumnHeaders", value);
        }

        [Category("Layout")]
        [Description("Show navigation bar")]
        public bool ShowNavigator
        {
            get => _designer.GetProperty<bool>("ShowNavigator");
            set => _designer.SetProperty("ShowNavigator", value);
        }

        [Category("Behavior")]
        [Description("Allow users to resize columns")]
        public bool AllowUserToResizeColumns
        {
            get => _designer.GetProperty<bool>("AllowUserToResizeColumns");
            set => _designer.SetProperty("AllowUserToResizeColumns", value);
        }

        [Category("Behavior")]
        [Description("Allow users to reorder columns")]
        public bool AllowColumnReorder
        {
            get => _designer.GetProperty<bool>("AllowColumnReorder");
            set => _designer.SetProperty("AllowColumnReorder", value);
        }

        [Category("Behavior")]
        [Description("Allow multiple row selection")]
        public bool MultiSelect
        {
            get => _designer.GetProperty<bool>("MultiSelect");
            set => _designer.SetProperty("MultiSelect", value);
        }

        [Category("Behavior")]
        [Description("Grid is read-only")]
        public bool ReadOnly
        {
            get => _designer.GetProperty<bool>("ReadOnly");
            set => _designer.SetProperty("ReadOnly", value);
        }

        [Category("Appearance")]
        [Description("Grid style preset")]
        public BeepGridStyle GridStyle
        {
            get => _designer.GetProperty<BeepGridStyle>("GridStyle");
            set => _designer.SetProperty("GridStyle", value);
        }

        [Category("Appearance")]
        [Description("Navigation bar style")]
        public navigationStyle NavigationStyle
        {
            get => _designer.GetProperty<navigationStyle>("NavigationStyle");
            set => _designer.SetProperty("NavigationStyle", value);
        }

        [Category("Layout")]
        [Description("Layout preset for grid structure")]
        public GridLayoutPreset LayoutPreset
        {
            get => _designer.GetProperty<GridLayoutPreset>("LayoutPreset");
            set => _designer.SetProperty("LayoutPreset", value);
        }

        [Category("Behavior")]
        [Description("Show selection checkboxes")]
        public bool ShowCheckBox
        {
            get => _designer.GetProperty<bool>("ShowCheckBox");
            set => _designer.SetProperty("ShowCheckBox", value);
        }

        #endregion

        #region Quick Configuration Actions

        /// <summary>
        /// Configure grid for data display (read-only, with headers and navigator)
        /// </summary>
        public void ConfigureAsDataDisplay()
        {
            ReadOnly = true;
            ShowColumnHeaders = true;
            ShowNavigator = true;
            MultiSelect = true;
            AllowUserToResizeColumns = true;
            AllowColumnReorder = true;
        }

        /// <summary>
        /// Configure grid for data entry (editable, with validation)
        /// </summary>
        public void ConfigureAsDataEntry()
        {
            ReadOnly = false;
            ShowColumnHeaders = true;
            ShowNavigator = true;
            MultiSelect = false;
            AllowUserToResizeColumns = true;
        }

        /// <summary>
        /// Configure grid as a simple list (minimal features)
        /// </summary>
        public void ConfigureAsSimpleList()
        {
            ShowColumnHeaders = false;
            ShowNavigator = false;
            MultiSelect = false;
            AllowUserToResizeColumns = false;
            AllowColumnReorder = false;
        }

        /// <summary>
        /// Configure grid for selection (multi-select with checkboxes)
        /// </summary>
        public void ConfigureAsSelectionGrid()
        {
            MultiSelect = true;
            _designer.SetProperty("ShowCheckBox", true);
            ShowColumnHeaders = true;
            ReadOnly = true;
        }

        /// <summary>
        /// Generate sample data for design-time preview
        /// </summary>
        public void GenerateSampleData()
        {
            // This would use DesignTimeDataHelper to populate the grid
            // For now, just a placeholder - actual implementation would add sample rows
        }

        /// <summary>
        /// Set standard row height (24px)
        /// </summary>
        public void SetStandardRowHeight()
        {
            RowHeight = 24;
        }

        /// <summary>
        /// Set compact row height (20px)
        /// </summary>
        public void SetCompactRowHeight()
        {
            RowHeight = 20;
        }

        /// <summary>
        /// Set comfortable row height (28px)
        /// </summary>
        public void SetComfortableRowHeight()
        {
            RowHeight = 28;
        }

        /// <summary>
        /// Enable all interactive features
        /// </summary>
        public void EnableAllFeatures()
        {
            AllowUserToResizeColumns = true;
            AllowColumnReorder = true;
            MultiSelect = true;
            ShowNavigator = true;
            ShowColumnHeaders = true;
        }

        /// <summary>
        /// Disable all interactive features (read-only display)
        /// </summary>
        public void DisableInteractiveFeatures()
        {
            ReadOnly = true;
            AllowUserToResizeColumns = false;
            AllowColumnReorder = false;
            MultiSelect = false;
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // Quick configuration presets
            items.Add(new DesignerActionHeaderItem("Quick Configuration"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsDataDisplay", "Data Display Grid", "Quick Configuration", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsDataEntry", "Data Entry Grid", "Quick Configuration", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsSimpleList", "Simple List", "Quick Configuration", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsSelectionGrid", "Selection Grid", "Quick Configuration", true));
            items.Add(new DesignerActionMethodItem(this, "GenerateSampleData", "Generate Sample Data", "Quick Configuration", true));

            // Layout presets
            items.Add(new DesignerActionHeaderItem("Row Height Presets"));
            items.Add(new DesignerActionMethodItem(this, "SetStandardRowHeight", "Standard (24px)", "Row Height Presets", true));
            items.Add(new DesignerActionMethodItem(this, "SetCompactRowHeight", "Compact (20px)", "Row Height Presets", true));
            items.Add(new DesignerActionMethodItem(this, "SetComfortableRowHeight", "Comfortable (28px)", "Row Height Presets", true));

            // Feature toggles
            items.Add(new DesignerActionHeaderItem("Features"));
            items.Add(new DesignerActionMethodItem(this, "EnableAllFeatures", "Enable All Features", "Features", true));
            items.Add(new DesignerActionMethodItem(this, "DisableInteractiveFeatures", "Disable Interactive Features", "Features", true));

            // Data properties
            items.Add(new DesignerActionHeaderItem("Data Properties"));
            items.Add(new DesignerActionPropertyItem("DataSource", "Data Source", "Data Properties"));
            items.Add(new DesignerActionPropertyItem("DataMember", "Data Member", "Data Properties"));

            // Layout properties
            items.Add(new DesignerActionHeaderItem("Layout Properties"));
            items.Add(new DesignerActionPropertyItem("RowHeight", "Row Height", "Layout Properties"));
            items.Add(new DesignerActionPropertyItem("ColumnHeaderHeight", "Column Header Height", "Layout Properties"));
            items.Add(new DesignerActionPropertyItem("ShowColumnHeaders", "Show Column Headers", "Layout Properties"));
            items.Add(new DesignerActionPropertyItem("ShowNavigator", "Show Navigator", "Layout Properties"));

            // Behavior properties
            items.Add(new DesignerActionHeaderItem("Behavior Properties"));
            items.Add(new DesignerActionPropertyItem("ReadOnly", "Read Only", "Behavior Properties"));
            items.Add(new DesignerActionPropertyItem("MultiSelect", "Multi-Select", "Behavior Properties"));
            items.Add(new DesignerActionPropertyItem("AllowUserToResizeColumns", "Allow Resize Columns", "Behavior Properties"));
            items.Add(new DesignerActionPropertyItem("AllowColumnReorder", "Allow Column Reorder", "Behavior Properties"));

            // Appearance properties
            items.Add(new DesignerActionHeaderItem("Appearance Properties"));
            items.Add(new DesignerActionPropertyItem("GridStyle", "Grid Style", "Appearance Properties"));
            items.Add(new DesignerActionPropertyItem("NavigationStyle", "Navigation Style", "Appearance Properties"));
            items.Add(new DesignerActionPropertyItem("LayoutPreset", "Layout Preset", "Appearance Properties"));
            items.Add(new DesignerActionPropertyItem("ShowCheckBox", "Show Check Box", "Appearance Properties"));

            return items;
        }
    }
}
