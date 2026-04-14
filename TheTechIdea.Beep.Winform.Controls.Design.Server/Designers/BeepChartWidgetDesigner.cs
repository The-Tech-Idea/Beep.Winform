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

        [Category("Chart")]
        [Description("Show or hide the chart legend")]
        public bool ShowLegend
        {
            get => _designer.GetProperty<bool>("ShowLegend");
            set => _designer.SetProperty("ShowLegend", value);
        }

        [Category("Chart")]
        [Description("Show or hide chart grid lines")]
        public bool ShowGrid
        {
            get => _designer.GetProperty<bool>("ShowGrid");
            set => _designer.SetProperty("ShowGrid", value);
        }

        [Category("Chart")]
        [Description("Minimum scale value for the chart")]
        public double MinValue
        {
            get => _designer.GetProperty<double>("MinValue");
            set => _designer.SetProperty("MinValue", value);
        }

        [Category("Chart")]
        [Description("Maximum scale value for the chart")]
        public double MaxValue
        {
            get => _designer.GetProperty<double>("MaxValue");
            set => _designer.SetProperty("MaxValue", value);
        }

        public void ConfigureAsBarChart()
        {
            Style = ChartWidgetStyle.BarChart;
            ShowGrid = true;
            ShowLegend = true;
        }

        public void ConfigureAsLineChart()
        {
            Style = ChartWidgetStyle.LineChart;
            ShowGrid = true;
            ShowLegend = true;
        }

        public void ConfigureAsPieChart()
        {
            Style = ChartWidgetStyle.PieChart;
            ShowGrid = false;
            ShowLegend = true;
        }

        public void ConfigureAsGaugeChart()
        {
            Style = ChartWidgetStyle.GaugeChart;
            ShowGrid = false;
            ShowLegend = false;
            MinValue = 0;
            MaxValue = 100;
        }

        public void ConfigureAsSparkline()
        {
            Style = ChartWidgetStyle.Sparkline;
            ShowGrid = false;
            ShowLegend = false;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsBarChart", "Bar Chart", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsLineChart", "Line Chart", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsPieChart", "Pie Chart", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsGaugeChart", "Gauge Chart", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsSparkline", "Sparkline", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowLegend", "Show Legend", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowGrid", "Show Grid", "Properties"));
            items.Add(new DesignerActionPropertyItem("MinValue", "Min Value", "Properties"));
            items.Add(new DesignerActionPropertyItem("MaxValue", "Max Value", "Properties"));
            return items;
        }
    }
}
