using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Widgets;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepMapWidgetDesigner : BaseWidgetDesigner
    {
        public BeepMapWidget? MapWidget => Component as BeepMapWidget;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepMapWidgetActionList(this));
            return lists;
        }
    }

    public class BeepMapWidgetActionList : DesignerActionList
    {
        private readonly BeepMapWidgetDesigner _designer;

        public BeepMapWidgetActionList(BeepMapWidgetDesigner designer) : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        [Category("Map")]
        [Description("Visual style of the map widget")]
        public MapWidgetStyle Style
        {
            get => _designer.GetProperty<MapWidgetStyle>("Style");
            set => _designer.SetProperty("Style", value);
        }

        [Category("Map")]
        [Description("Title of the map widget")]
        public string Title
        {
            get => _designer.GetProperty<string>("Title") ?? string.Empty;
            set => _designer.SetProperty("Title", value);
        }

        public void ConfigureAsMapView() { Style = MapWidgetStyle.LocationCard; }
        public void ConfigureAsRouteView() { Style = MapWidgetStyle.RouteDisplay; }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsMapView", "Map View", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsRouteView", "Route View", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            return items;
        }
    }
}
