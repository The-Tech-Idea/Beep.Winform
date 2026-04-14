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

        [Category("Map")]
        [Description("Subtitle of the map widget")]
        public string Subtitle
        {
            get => _designer.GetProperty<string>("Subtitle") ?? string.Empty;
            set => _designer.SetProperty("Subtitle", value);
        }

        [Category("Location")]
        [Description("Address shown by the map widget")]
        public string Address
        {
            get => _designer.GetProperty<string>("Address") ?? string.Empty;
            set => _designer.SetProperty("Address", value);
        }

        [Category("Location")]
        [Description("Latitude coordinate")]
        public double Latitude
        {
            get => _designer.GetProperty<double>("Latitude");
            set => _designer.SetProperty("Latitude", value);
        }

        [Category("Location")]
        [Description("Longitude coordinate")]
        public double Longitude
        {
            get => _designer.GetProperty<double>("Longitude");
            set => _designer.SetProperty("Longitude", value);
        }

        [Category("Map")]
        [Description("Map zoom level")]
        public float ZoomLevel
        {
            get => _designer.GetProperty<float>("ZoomLevel");
            set => _designer.SetProperty("ZoomLevel", value);
        }

        [Category("Map")]
        [Description("Show map markers")]
        public bool ShowMarkers
        {
            get => _designer.GetProperty<bool>("ShowMarkers");
            set => _designer.SetProperty("ShowMarkers", value);
        }

        [Category("Map")]
        [Description("Show route overlays")]
        public bool ShowRoutes
        {
            get => _designer.GetProperty<bool>("ShowRoutes");
            set => _designer.SetProperty("ShowRoutes", value);
        }

        [Category("Map")]
        [Description("Show traffic overlay")]
        public bool ShowTraffic
        {
            get => _designer.GetProperty<bool>("ShowTraffic");
            set => _designer.SetProperty("ShowTraffic", value);
        }

        [Category("Map")]
        [Description("Map provider name")]
        public string MapProvider
        {
            get => _designer.GetProperty<string>("MapProvider") ?? string.Empty;
            set => _designer.SetProperty("MapProvider", value);
        }

        [Category("Map")]
        [Description("Map view mode")]
        public MapViewType ViewType
        {
            get => _designer.GetProperty<MapViewType>("ViewType");
            set => _designer.SetProperty("ViewType", value);
        }

        public void ConfigureAsMapView()
        {
            Style = MapWidgetStyle.LocationCard;
            ShowMarkers = true;
            ShowRoutes = false;
            ShowTraffic = false;
        }

        public void ConfigureAsRouteView()
        {
            Style = MapWidgetStyle.RouteDisplay;
            ShowMarkers = true;
            ShowRoutes = true;
            ShowTraffic = false;
        }

        public void ConfigureAsLiveTracking()
        {
            Style = MapWidgetStyle.LiveTracking;
            ShowMarkers = true;
            ShowRoutes = true;
            ShowTraffic = true;
        }

        public void ConfigureAsTravelCard()
        {
            Style = MapWidgetStyle.TravelCard;
            ShowMarkers = true;
            ShowRoutes = true;
            ShowTraffic = false;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsMapView", "Map View", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsRouteView", "Route View", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsLiveTracking", "Live Tracking", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsTravelCard", "Travel Card", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            items.Add(new DesignerActionPropertyItem("Subtitle", "Subtitle", "Properties"));
            items.Add(new DesignerActionPropertyItem("Address", "Address", "Properties"));
            items.Add(new DesignerActionPropertyItem("Latitude", "Latitude", "Properties"));
            items.Add(new DesignerActionPropertyItem("Longitude", "Longitude", "Properties"));
            items.Add(new DesignerActionPropertyItem("ZoomLevel", "Zoom Level", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowMarkers", "Show Markers", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowRoutes", "Show Routes", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowTraffic", "Show Traffic", "Properties"));
            items.Add(new DesignerActionPropertyItem("MapProvider", "Map Provider", "Properties"));
            items.Add(new DesignerActionPropertyItem("ViewType", "View Type", "Properties"));
            return items;
        }
    }
}
