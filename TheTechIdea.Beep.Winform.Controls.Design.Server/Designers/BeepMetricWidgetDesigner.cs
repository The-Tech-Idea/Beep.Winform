using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Widgets;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepMetricWidget
    /// </summary>
    public class BeepMetricWidgetDesigner : BaseWidgetDesigner
    {
        public BeepMetricWidget? MetricWidget => Component as BeepMetricWidget;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepMetricWidgetActionList(this));
            return lists;
        }
    }

    public class BeepMetricWidgetActionList : DesignerActionList
    {
        private readonly BeepMetricWidgetDesigner _designer;

        public BeepMetricWidgetActionList(BeepMetricWidgetDesigner designer) : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        [Category("Widget")]
        [Description("Visual style of the metric widget")]
        public MetricWidgetStyle Style
        {
            get => _designer.GetProperty<MetricWidgetStyle>("Style");
            set => _designer.SetProperty("Style", value);
        }

        [Category("Widget")]
        [Description("Title/label for the metric")]
        public string Title
        {
            get => _designer.GetProperty<string>("Title") ?? string.Empty;
            set => _designer.SetProperty("Title", value);
        }

        [Category("Widget")]
        [Description("The main metric value to display")]
        public string Value
        {
            get => _designer.GetProperty<string>("Value") ?? string.Empty;
            set => _designer.SetProperty("Value", value);
        }

        [Category("Widget")]
        [Description("Units for the metric value")]
        public string Units
        {
            get => _designer.GetProperty<string>("Units") ?? string.Empty;
            set => _designer.SetProperty("Units", value);
        }

        [Category("Widget")]
        [Description("Trend value text")]
        public string TrendValue
        {
            get => _designer.GetProperty<string>("TrendValue") ?? string.Empty;
            set => _designer.SetProperty("TrendValue", value);
        }

        [Category("Widget")]
        [Description("Trend direction: up, down, or neutral")]
        public string TrendDirection
        {
            get => _designer.GetProperty<string>("TrendDirection") ?? string.Empty;
            set => _designer.SetProperty("TrendDirection", value);
        }

        [Category("Widget")]
        [Description("Trend percentage value")]
        public double TrendPercentage
        {
            get => _designer.GetProperty<double>("TrendPercentage");
            set => _designer.SetProperty("TrendPercentage", value);
        }

        [Category("Widget")]
        [Description("Whether to show the trend indicator")]
        public bool ShowTrend
        {
            get => _designer.GetProperty<bool>("ShowTrend");
            set => _designer.SetProperty("ShowTrend", value);
        }

        [Category("Widget")]
        [Description("Whether to show an icon")]
        public bool ShowIcon
        {
            get => _designer.GetProperty<bool>("ShowIcon");
            set => _designer.SetProperty("ShowIcon", value);
        }

        public void ConfigureAsSimpleValue()
        {
            Style = MetricWidgetStyle.SimpleValue;
            ShowTrend = false;
            ShowIcon = false;
        }

        public void ConfigureAsValueWithTrend()
        {
            Style = MetricWidgetStyle.ValueWithTrend;
            ShowTrend = true;
            TrendDirection = "up";
            TrendPercentage = 12.5;
        }

        public void ConfigureAsProgressMetric()
        {
            Style = MetricWidgetStyle.ProgressMetric;
            ShowTrend = false;
            ShowIcon = false;
        }

        public void ConfigureAsGaugeMetric()
        {
            Style = MetricWidgetStyle.GaugeMetric;
            ShowTrend = false;
            ShowIcon = false;
        }

        public void ConfigureAsComparisonMetric()
        {
            Style = MetricWidgetStyle.ComparisonMetric;
            ShowTrend = true;
            TrendDirection = "neutral";
            TrendPercentage = 0;
        }

        public void ConfigureAsCardMetric()
        {
            Style = MetricWidgetStyle.CardMetric;
            ShowTrend = false;
            ShowIcon = true;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsSimpleValue", "Simple Value", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsValueWithTrend", "Value With Trend", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsProgressMetric", "Progress Metric", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsGaugeMetric", "Gauge Metric", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsComparisonMetric", "Comparison Metric", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsCardMetric", "Card Metric", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            items.Add(new DesignerActionPropertyItem("Value", "Value", "Properties"));
            items.Add(new DesignerActionPropertyItem("Units", "Units", "Properties"));
            items.Add(new DesignerActionPropertyItem("TrendValue", "Trend Value", "Properties"));
            items.Add(new DesignerActionPropertyItem("TrendDirection", "Trend Direction", "Properties"));
            items.Add(new DesignerActionPropertyItem("TrendPercentage", "Trend Percentage", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowTrend", "Show Trend", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowIcon", "Show Icon", "Properties"));
            return items;
        }
    }
}
