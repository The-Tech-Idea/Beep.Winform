using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepListBox control
    /// </summary>
    public class BeepListBoxDesigner : BaseBeepControlDesigner
    {
        public BeepListBox? ListBox => Component as BeepListBox;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepListBoxActionList(this));
            return lists;
        }
    }

    public class BeepListBoxActionList : DesignerActionList
    {
        private readonly BeepListBoxDesigner _designer;

        public BeepListBoxActionList(BeepListBoxDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        #region Properties

        [Category("Behavior")]
        public bool MultiSelect
        {
            get => _designer.GetProperty<bool>("MultiSelect");
            set => _designer.SetProperty("MultiSelect", value);
        }

        [Category("Behavior")]
        public bool EnableSearch
        {
            get => _designer.GetProperty<bool>("ShowSearch");
            set => _designer.SetProperty("ShowSearch", value);
        }

        [Category("Appearance")]
        [Description("Visual list rendering style.")]
        public ListBoxType ListBoxType
        {
            get => _designer.GetProperty<ListBoxType>("ListBoxType");
            set => _designer.SetProperty("ListBoxType", value);
        }

        [Category("Behavior")]
        [Description("Selection interaction mode.")]
        public SelectionModeEnum SelectionMode
        {
            get => _designer.GetProperty<SelectionModeEnum>("SelectionMode");
            set => _designer.SetProperty("SelectionMode", value);
        }

        [Category("Behavior")]
        [Description("Keep a search box visible above the list.")]
        public bool ShowSearch
        {
            get => _designer.GetProperty<bool>("ShowSearch");
            set => _designer.SetProperty("ShowSearch", value);
        }

        [Category("Behavior")]
        [Description("Group items by their category.")]
        public bool ShowGroups
        {
            get => _designer.GetProperty<bool>("ShowGroups");
            set => _designer.SetProperty("ShowGroups", value);
        }

        [Category("Behavior")]
        [Description("Render nested item hierarchies.")]
        public bool ShowHierarchy
        {
            get => _designer.GetProperty<bool>("ShowHierarchy");
            set => _designer.SetProperty("ShowHierarchy", value);
        }

        [Category("Appearance")]
        [Description("Density preset for row heights.")]
        public ListDensityMode Density
        {
            get => _designer.GetProperty<ListDensityMode>("Density");
            set => _designer.SetProperty("Density", value);
        }

        [Category("Search")]
        [Description("Placeholder text displayed in the embedded search box.")]
        public string SearchPlaceholderText
        {
            get => _designer.GetProperty<string>("SearchPlaceholderText") ?? string.Empty;
            set => _designer.SetProperty("SearchPlaceholderText", value);
        }

        [Category("Data")]
        [Description("List binding source.")]
        public object? DataSource
        {
            get => _designer.GetProperty<object>("DataSource");
            set => _designer.SetProperty("DataSource", value);
        }

        [Category("Data")]
        [Description("Property used for the display text when DataSource is set.")]
        public string DisplayMember
        {
            get => _designer.GetProperty<string>("DisplayMember") ?? string.Empty;
            set => _designer.SetProperty("DisplayMember", value);
        }

        [Category("Data")]
        [Description("Property used for the selected value when DataSource is set.")]
        public string ValueMember
        {
            get => _designer.GetProperty<string>("ValueMember") ?? string.Empty;
            set => _designer.SetProperty("ValueMember", value);
        }

        [Category("Behavior")]
        [Description("Render loading skeleton rows instead of live data.")]
        public bool IsLoading
        {
            get => _designer.GetProperty<bool>("IsLoading");
            set => _designer.SetProperty("IsLoading", value);
        }

        [Category("Behavior")]
        [Description("Number of skeleton rows shown while loading.")]
        public int SkeletonRowCount
        {
            get => _designer.GetProperty<int>("SkeletonRowCount");
            set => _designer.SetProperty("SkeletonRowCount", value);
        }

        #endregion

        #region Presets

        public void ConfigureAsSearchableList()
        {
            ListBoxType = ListBoxType.SearchableList;
            ShowSearch = true;
            SearchPlaceholderText = "Search...";
        }

        public void ConfigureAsNavigationList()
        {
            ListBoxType = ListBoxType.NavigationRail;
            ShowSearch = false;
            ShowHierarchy = false;
            Density = ListDensityMode.Comfortable;
        }

        public void ConfigureAsGroupedList()
        {
            ListBoxType = ListBoxType.Grouped;
            ShowGroups = true;
            ShowSearch = true;
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Presets"));
            items.Add(new DesignerActionMethodItem(this, nameof(ConfigureAsSearchableList), "Searchable List", "Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ConfigureAsNavigationList), "Navigation List", "Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ConfigureAsGroupedList), "Grouped List", "Presets", true));

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(ListBoxType), "List Box Type", "Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(Density), "Density", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Behavior"));
            items.Add(new DesignerActionPropertyItem("MultiSelect", "Multi-Select", "Behavior"));
            items.Add(new DesignerActionPropertyItem("EnableSearch", "Enable Search", "Behavior"));
            items.Add(new DesignerActionPropertyItem(nameof(SelectionMode), "Selection Mode", "Behavior"));
            items.Add(new DesignerActionPropertyItem(nameof(ShowGroups), "Show Groups", "Behavior"));
            items.Add(new DesignerActionPropertyItem(nameof(ShowHierarchy), "Show Hierarchy", "Behavior"));
            items.Add(new DesignerActionPropertyItem(nameof(IsLoading), "Is Loading", "Behavior"));
            items.Add(new DesignerActionPropertyItem(nameof(SkeletonRowCount), "Skeleton Row Count", "Behavior"));

            items.Add(new DesignerActionHeaderItem("Search"));
            items.Add(new DesignerActionPropertyItem(nameof(SearchPlaceholderText), "Search Placeholder Text", "Search"));

            items.Add(new DesignerActionHeaderItem("Data"));
            items.Add(new DesignerActionPropertyItem(nameof(DataSource), "Data Source", "Data"));
            items.Add(new DesignerActionPropertyItem(nameof(DisplayMember), "Display Member", "Data"));
            items.Add(new DesignerActionPropertyItem(nameof(ValueMember), "Value Member", "Data"));

            return items;
        }
    }
}

