using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Widgets;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepChartWidgetDesigner : BaseWidgetDesigner
    {
        public BeepChartWidget? ChartWidget => Component as BeepChartWidget;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepChartWidgetActionList(this));
            return lists;
        }
    }

    public class BeepChartWidgetActionList : DesignerActionList
    {
        private readonly BeepChartWidgetDesigner _designer;

        public BeepChartWidgetActionList(BeepChartWidgetDesigner designer) : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        [Category("Widget")]
        [Description("Visual style of the chart widget")]
        public ChartWidgetStyle Style
        {
            get => _designer.GetProperty<ChartWidgetStyle>("Style");
            set => _designer.SetProperty("Style", value);
        }

        [Category("Widget")]
        [Description("Title of the chart")]
        public string Title
        {
            get => _designer.GetProperty<string>("Title") ?? string.Empty;
            set => _designer.SetProperty("Title", value);
        }

        public void ConfigureAsBarChart() { Style = ChartWidgetStyle.BarChart; }
        public void ConfigureAsLineChart() { Style = ChartWidgetStyle.LineChart; }
        public void ConfigureAsPieChart() { Style = ChartWidgetStyle.PieChart; }
        public void ConfigureAsGaugeChart() { Style = ChartWidgetStyle.GaugeChart; }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsBarChart", "Bar Chart", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsLineChart", "Line Chart", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsPieChart", "Pie Chart", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsGaugeChart", "Gauge Chart", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            return items;
        }
    }
}
