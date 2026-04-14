using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Widgets;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepDashboardWidgetDesigner : BaseWidgetDesigner
    {
        public BeepDashboardWidget? DashboardWidget => Component as BeepDashboardWidget;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepDashboardWidgetActionList(this));
            return lists;
        }
    }

    public class BeepDashboardWidgetActionList : DesignerActionList
    {
        private readonly BeepDashboardWidgetDesigner _designer;

        public BeepDashboardWidgetActionList(BeepDashboardWidgetDesigner designer) : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        [Category("Dashboard")]
        [Description("Visual style of the dashboard widget")]
        public DashboardWidgetStyle Style
        {
            get => _designer.GetProperty<DashboardWidgetStyle>("Style");
            set => _designer.SetProperty("Style", value);
        }

        [Category("Dashboard")]
        [Description("Title of the dashboard")]
        public string Title
        {
            get => _designer.GetProperty<string>("Title") ?? string.Empty;
            set => _designer.SetProperty("Title", value);
        }

        [Category("Layout")]
        [Description("Number of columns in grid layout")]
        public int Columns
        {
            get => _designer.GetProperty<int>("Columns");
            set => _designer.SetProperty("Columns", value);
        }

        [Category("Layout")]
        [Description("Number of rows in grid layout")]
        public int Rows
        {
            get => _designer.GetProperty<int>("Rows");
            set => _designer.SetProperty("Rows", value);
        }

        [Category("Dashboard")]
        [Description("Show the dashboard title header")]
        public bool ShowTitle
        {
            get => _designer.GetProperty<bool>("ShowTitle");
            set => _designer.SetProperty("ShowTitle", value);
        }

        public void ConfigureAsMultiMetric()
        {
            Style = DashboardWidgetStyle.MultiMetric;
            ShowTitle = true;
            Columns = 2;
            Rows = 2;
        }

        public void ConfigureAsChartGrid()
        {
            Style = DashboardWidgetStyle.ChartGrid;
            ShowTitle = true;
            Columns = 2;
            Rows = 2;
        }

        public void ConfigureAsTimelineView()
        {
            Style = DashboardWidgetStyle.TimelineView;
            ShowTitle = true;
            Columns = 1;
            Rows = 1;
        }

        public void ConfigureAsStatusOverview()
        {
            Style = DashboardWidgetStyle.StatusOverview;
            ShowTitle = true;
            Columns = 2;
            Rows = 2;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsMultiMetric", "Multi Metric", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsChartGrid", "Chart Grid", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsTimelineView", "Timeline View", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsStatusOverview", "Status Overview", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowTitle", "Show Title", "Properties"));
            items.Add(new DesignerActionPropertyItem("Columns", "Columns", "Properties"));
            items.Add(new DesignerActionPropertyItem("Rows", "Rows", "Properties"));
            return items;
        }
    }
}
