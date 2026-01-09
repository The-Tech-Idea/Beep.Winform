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

        public void ConfigureAsActivityFeed() { Style = ListWidgetStyle.ActivityFeed; }
        public void ConfigureAsDataTable() { Style = ListWidgetStyle.DataTable; }
        public void ConfigureAsTaskList() { Style = ListWidgetStyle.TaskList; }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsActivityFeed", "Activity Feed", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsDataTable", "Data Table", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsTaskList", "Task List", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            return items;
        }
    }
}
