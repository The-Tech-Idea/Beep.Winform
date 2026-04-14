using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Widgets;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepListWidgetDesigner : BaseWidgetDesigner
    {
        public BeepListWidget? ListWidget => Component as BeepListWidget;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepListWidgetActionList(this));
            return lists;
        }
    }

    public class BeepListWidgetActionList : DesignerActionList
    {
        private readonly BeepListWidgetDesigner _designer;

        public BeepListWidgetActionList(BeepListWidgetDesigner designer) : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        [Category("Widget")]
        [Description("Visual style of the list widget")]
        public ListWidgetStyle Style
        {
            get => _designer.GetProperty<ListWidgetStyle>("Style");
            set => _designer.SetProperty("Style", value);
        }

        [Category("Widget")]
        [Description("Title of the list")]
        public string Title
        {
            get => _designer.GetProperty<string>("Title") ?? string.Empty;
            set => _designer.SetProperty("Title", value);
        }

        [Category("List")]
        [Description("Show the list header")]
        public bool ShowHeader
        {
            get => _designer.GetProperty<bool>("ShowHeader");
            set => _designer.SetProperty("ShowHeader", value);
        }

        [Category("List")]
        [Description("Allow list item selection")]
        public bool AllowSelection
        {
            get => _designer.GetProperty<bool>("AllowSelection");
            set => _designer.SetProperty("AllowSelection", value);
        }

        [Category("List")]
        [Description("Index of the selected item")]
        public int SelectedIndex
        {
            get => _designer.GetProperty<int>("SelectedIndex");
            set => _designer.SetProperty("SelectedIndex", value);
        }

        [Category("List")]
        [Description("Maximum number of items rendered before truncation/scrolling")]
        public int MaxVisibleItems
        {
            get => _designer.GetProperty<int>("MaxVisibleItems");
            set => _designer.SetProperty("MaxVisibleItems", value);
        }

        public void ConfigureAsActivityFeed()
        {
            Style = ListWidgetStyle.ActivityFeed;
            ShowHeader = true;
            AllowSelection = false;
            MaxVisibleItems = 6;
        }

        public void ConfigureAsDataTable()
        {
            Style = ListWidgetStyle.DataTable;
            ShowHeader = true;
            AllowSelection = true;
            MaxVisibleItems = 10;
        }

        public void ConfigureAsTaskList()
        {
            Style = ListWidgetStyle.TaskList;
            ShowHeader = true;
            AllowSelection = true;
            MaxVisibleItems = 8;
        }

        public void ConfigureAsCompactStatusList()
        {
            Style = ListWidgetStyle.StatusList;
            ShowHeader = false;
            AllowSelection = true;
            MaxVisibleItems = 5;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsActivityFeed", "Activity Feed", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsDataTable", "Data Table", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsTaskList", "Task List", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsCompactStatusList", "Compact Status List", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowHeader", "Show Header", "Properties"));
            items.Add(new DesignerActionPropertyItem("AllowSelection", "Allow Selection", "Properties"));
            items.Add(new DesignerActionPropertyItem("SelectedIndex", "Selected Index", "Properties"));
            items.Add(new DesignerActionPropertyItem("MaxVisibleItems", "Max Visible Items", "Properties"));
            return items;
        }
    }
}
