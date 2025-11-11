using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Map;

namespace TheTechIdea.Beep.Winform.Controls.Widgets
{
    public enum MapWidgetStyle
    {
        LiveTracking,     // Real-time location tracking
        RouteDisplay,     // Route/path visualization
        LocationCard,     // Location information display
        GeographicHeatmap,// Location-based data heatmap
        AddressCard,      // Address/location details
        MapPreview,       // Small map preview
        LocationList,     // List of locations
        TravelCard,       // Travel/shipping card
        RegionMap,        // Regional data display
        PlaceCard         // Place/venue information
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Map Widget")]
    [Category("Beep Widgets")]
    [Description("Map widget for location tracking, routing, geographic data, and location-based services.")]
    public class BeepMapWidget : BaseControl
    {
        #region Fields
        private MapWidgetStyle _style = MapWidgetStyle.LocationCard;
        private IWidgetPainter _painter;
        private string _title = "Map Widget";
        private string _subtitle = "Location Data";
        private string _address = "";
        private string _city = "";
        private string _region = "";
        private string _country = "";
        private string _postalCode = "";
        private double _latitude = 0.0;
        private double _longitude = 0.0;
        private float _zoomLevel = 10.0f;
        private Color _accentColor = Color.FromArgb(34, 139, 34); // Forest Green
        private Color _routeColor = Color.FromArgb(33, 150, 243); // Blue
        private Color _markerColor = Color.FromArgb(244, 67, 54); // Red
        private Color _mapBackColor = Color.White;
        private Color _surfaceColor = Color.FromArgb(250, 250, 250);
        private Color _panelBackColor = Color.FromArgb(250, 250, 250);
        private Color _secondaryColor = Color.FromArgb(158, 158, 158);
        private Color _validRouteColor = Color.FromArgb(76, 175, 80);
        private Color _warningColor = Color.FromArgb(255, 193, 7);
        private Color _errorColor = Color.FromArgb(244, 67, 54);
        private Color _titleForeColor = Color.Black;
        private Color _textForeColor = Color.FromArgb(100, 100, 100);
        private Color _labelForeColor = Color.FromArgb(150, 150, 150);
        private Color _hoverBackColor = Color.FromArgb(245, 245, 245);
        private Color _selectedBackColor = Color.FromArgb(240, 240, 240);
        private Color _borderColor = Color.FromArgb(200, 200, 200);
        private List<MapLocation> _locations = new List<MapLocation>();
        private List<MapRoute> _routes = new List<MapRoute>();
        private bool _showMarkers = true;
        private bool _showRoutes = true;
        private bool _showTraffic = false;
        private bool _showSatellite = false;
        private string _mapProvider = "OpenStreetMap";
        private MapViewType _viewType = MapViewType.Standard;
        private DateTime _lastUpdated = DateTime.Now;

        // Events
        public event EventHandler<BeepEventDataArgs> LocationClicked;
        public event EventHandler<BeepEventDataArgs> RouteClicked;
        public event EventHandler<BeepEventDataArgs> MarkerClicked;
        public event EventHandler<BeepEventDataArgs> AddressClicked;
        public event EventHandler<BeepEventDataArgs> MapClicked;
        #endregion

        #region Constructor
        public BeepMapWidget() : base()
        {
            IsChild = false;
            Padding = new Padding(5);
            this.Size = new Size(300, 200);
            ApplyThemeToChilds = false;
            InitializeSampleData();
            ApplyTheme();
            CanBeHovered = true;
            InitializePainter();
        }

        private void InitializeSampleData()
        {
            _locations.AddRange(new[]
            {
                new MapLocation { Name = "New York", Address = "New York, NY", Latitude = 40.7128, Longitude = -74.0060, Type = "City" },
                new MapLocation { Name = "Los Angeles", Address = "Los Angeles, CA", Latitude = 34.0522, Longitude = -118.2437, Type = "City" },
                new MapLocation { Name = "Chicago", Address = "Chicago, IL", Latitude = 41.8781, Longitude = -87.6298, Type = "City" },
                new MapLocation { Name = "Houston", Address = "Houston, TX", Latitude = 29.7604, Longitude = -95.3698, Type = "City" },
                new MapLocation { Name = "Phoenix", Address = "Phoenix, AZ", Latitude = 33.4484, Longitude = -112.0740, Type = "City" }
            });

            _routes.AddRange(new[]
            {
                new MapRoute { Name = "Route 1", StartLocation = _locations[0], EndLocation = _locations[1], Distance = 2445.5, EstimatedTime = "5h 30m" },
                new MapRoute { Name = "Route 2", StartLocation = _locations[1], EndLocation = _locations[2], Distance = 1745.3, EstimatedTime = "4h 15m" }
            });

            // Set default location to New York
            if (_locations.Count > 0)
            {
                var defaultLocation = _locations[0];
                _latitude = defaultLocation.Latitude;
                _longitude = defaultLocation.Longitude;
                _address = defaultLocation.Address;
            }
        }

        private void InitializePainter()
        {
            switch (_style)
            {
                case MapWidgetStyle.LiveTracking:
                    _painter = new LiveTrackingPainter();
                    break;
                case MapWidgetStyle.RouteDisplay:
                    _painter = new RouteDisplayPainter();
                    break;
                case MapWidgetStyle.LocationCard:
                    _painter = new LocationCardPainter();
                    break;
                case MapWidgetStyle.GeographicHeatmap:
                    _painter = new GeographicHeatmapPainter();
                    break;
                case MapWidgetStyle.AddressCard:
                    _painter = new AddressCardPainter();
                    break;
                case MapWidgetStyle.MapPreview:
                    _painter = new MapPreviewPainter();
                    break;
                case MapWidgetStyle.LocationList:
                    _painter = new LocationListPainter();
                    break;
                case MapWidgetStyle.TravelCard:
                    _painter = new TravelCardPainter();
                    break;
                case MapWidgetStyle.RegionMap:
                    _painter = new RegionMapPainter();
                    break;
                case MapWidgetStyle.PlaceCard:
                    _painter = new PlaceCardPainter();
                    break;
                default:
                    _painter = new LocationCardPainter();
                    break;
            }
            _painter?.Initialize(this, _currentTheme);
        }
        #endregion

        #region Properties
        [Category("Map")]
        [Description("Visual Style of the map widget.")]
        public MapWidgetStyle Style
        {
            get => _style;
            set { _style = value; InitializePainter(); Invalidate(); }
        }

        [Category("Map")]
        [Description("Title text for the map widget.")]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        [Category("Map")]
        [Description("Subtitle text for the map widget.")]
        public string Subtitle
        {
            get => _subtitle;
            set { _subtitle = value; Invalidate(); }
        }

        [Category("Location")]
        [Description("Full address of the location.")]
        public string Address
        {
            get => _address;
            set { _address = value; Invalidate(); }
        }

        [Category("Location")]
        [Description("City name.")]
        public string City
        {
            get => _city;
            set { _city = value; Invalidate(); }
        }

        [Category("Location")]
        [Description("Region or state name.")]
        public string Region
        {
            get => _region;
            set { _region = value; Invalidate(); }
        }

        [Category("Location")]
        [Description("Country name.")]
        public string Country
        {
            get => _country;
            set { _country = value; Invalidate(); }
        }

        [Category("Location")]
        [Description("Postal or ZIP code.")]
        public string PostalCode
        {
            get => _postalCode;
            set { _postalCode = value; Invalidate(); }
        }

        [Category("Location")]
        [Description("Latitude coordinate.")]
        public double Latitude
        {
            get => _latitude;
            set { _latitude = value; Invalidate(); }
        }

        [Category("Location")]
        [Description("Longitude coordinate.")]
        public double Longitude
        {
            get => _longitude;
            set { _longitude = value; Invalidate(); }
        }

        [Category("Map")]
        [Description("Map zoom level (1-20).")]
        public float ZoomLevel
        {
            get => _zoomLevel;
            set { _zoomLevel = Math.Max(1f, Math.Min(20f, value)); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Primary accent color for the map widget.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Color for route lines.")]
        public Color RouteColor
        {
            get => _routeColor;
            set { _routeColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Color for location markers.")]
        public Color MarkerColor
        {
            get => _markerColor;
            set { _markerColor = value; Invalidate(); }
        }

        [Category("Map")]
        [Description("Whether to show location markers.")]
        public bool ShowMarkers
        {
            get => _showMarkers;
            set { _showMarkers = value; Invalidate(); }
        }

        [Category("Map")]
        [Description("Whether to show route lines.")]
        public bool ShowRoutes
        {
            get => _showRoutes;
            set { _showRoutes = value; Invalidate(); }
        }

        [Category("Map")]
        [Description("Whether to show traffic information.")]
        public bool ShowTraffic
        {
            get => _showTraffic;
            set { _showTraffic = value; Invalidate(); }
        }

        [Category("Map")]
        [Description("Whether to show satellite view.")]
        public bool ShowSatellite
        {
            get => _showSatellite;
            set { _showSatellite = value; Invalidate(); }
        }

        [Category("Map")]
        [Description("Map service provider.")]
        public string MapProvider
        {
            get => _mapProvider;
            set { _mapProvider = value; Invalidate(); }
        }

        [Category("Map")]
        [Description("Map view type.")]
        public MapViewType ViewType
        {
            get => _viewType;
            set { _viewType = value; Invalidate(); }
        }

        [Category("Map")]
        [Description("Last updated timestamp.")]
        public DateTime LastUpdated
        {
            get => _lastUpdated;
            set { _lastUpdated = value; Invalidate(); }
        }

        [Category("Map")]
        [Description("Collection of map locations.")]
        public List<MapLocation> Locations
        {
            get => _locations;
            set { _locations = value ?? new List<MapLocation>(); Invalidate(); }
        }

        [Category("Map")]
        [Description("Collection of map routes.")]
        public List<MapRoute> Routes
        {
            get => _routes;
            set { _routes = value ?? new List<MapRoute>(); Invalidate(); }
        }
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            var ctx = new WidgetContext
            {
                DrawingRect = DrawingRect,
                Title = _title,
                Value = _subtitle,
                AccentColor = _accentColor,
                ShowIcon = true,
                IsInteractive = true,
                CornerRadius = BorderRadius,
                
                // Map-specific typed properties
                Address = _address,
                City = _city,
                Region = _region,
                Country = _country,
                Latitude = _latitude,
                Longitude = _longitude
            };

            _painter?.Initialize(this, _currentTheme);
            ctx = _painter?.AdjustLayout(DrawingRect, ctx) ?? ctx;

            _painter?.DrawBackground(g, ctx);
            _painter?.DrawContent(g, ctx);
            _painter?.DrawForegroundAccents(g, ctx);

            RefreshHitAreas(ctx);
            _painter?.UpdateHitAreas(this, ctx, (name, rect) => { });
        }

        private void RefreshHitAreas(WidgetContext ctx)
        {
            ClearHitList();

            if (!ctx.ContentRect.IsEmpty)
            {
                AddHitArea("Map", ctx.ContentRect, null, () =>
                {
                    MapClicked?.Invoke(this, new BeepEventDataArgs("MapClicked", this));
                });
            }

            if (!ctx.HeaderRect.IsEmpty)
            {
                AddHitArea("Address", ctx.HeaderRect, null, () =>
                {
                    AddressClicked?.Invoke(this, new BeepEventDataArgs("AddressClicked", this));
                });
            }

            if (!ctx.FooterRect.IsEmpty)
            {
                AddHitArea("Location", ctx.FooterRect, null, () =>
                {
                    LocationClicked?.Invoke(this, new BeepEventDataArgs("LocationClicked", this));
                });
            }

            // Add hit areas for individual locations
            if (_style == MapWidgetStyle.LocationList)
            {
                for (int i = 0; i < _locations.Count && i < 5; i++) // Limit to 5 visible items
                {
                    int itemIndex = i; // Capture for closure
                    AddHitArea($"Location{i}", new Rectangle(), null, () =>
                    {
                        LocationClicked?.Invoke(this, new BeepEventDataArgs("LocationClicked", this) { EventData = _locations[itemIndex] });
                    });
                }
            }

            // Add hit areas for routes
            if (_showRoutes && _routes.Count > 0)
            {
                for (int i = 0; i < _routes.Count && i < 3; i++) // Limit to 3 visible routes
                {
                    int routeIndex = i; // Capture for closure
                    AddHitArea($"Route{i}", new Rectangle(), null, () =>
                    {
                        RouteClicked?.Invoke(this, new BeepEventDataArgs("RouteClicked", this) { EventData = _routes[routeIndex] });
                    });
                }
            }
        }
        #endregion

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;

            // Apply map-specific theme colors
            BackColor = _currentTheme.BackColor;
            ForeColor = _currentTheme.ForeColor;
            
            // Update map surface colors
            _mapBackColor = _currentTheme.BackColor;
            _surfaceColor = _currentTheme.SurfaceColor;
            _panelBackColor = _currentTheme.PanelBackColor;
            
            // Update location and marker colors
            _accentColor = _currentTheme.AccentColor;         // Location markers
            _routeColor = _currentTheme.PrimaryColor;         // Routes, paths
            _secondaryColor = _currentTheme.SecondaryColor;   // Additional elements
            
            // Update status colors for routes/navigation
            _validRouteColor = _currentTheme.SuccessColor;
            _warningColor = _currentTheme.WarningColor;
            _errorColor = _currentTheme.ErrorColor;
            
            // Update text colors
            _titleForeColor = _currentTheme.CardTitleForeColor;
            _textForeColor = _currentTheme.CardTextForeColor;
            _labelForeColor = _currentTheme.OnBackgroundColor;
            
            // Update interactive colors
            _hoverBackColor = _currentTheme.ButtonHoverBackColor;
            _selectedBackColor = _currentTheme.HighlightBackColor;
            _borderColor = _currentTheme.BorderColor;
            
            InitializePainter();
            Invalidate();
        }
    }

    /// <summary>
    /// Map view type enumeration
    /// </summary>
    public enum MapViewType
    {
        Standard,     // Standard map view
        Satellite,    // Satellite imagery
        Terrain,      // Terrain view
        Hybrid,       // Hybrid satellite/map
        Transit       // Public transit overlay
    }

    /// <summary>
    /// Map location data structure
    /// </summary>
    public class MapLocation
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double Latitude { get; set; } = 0.0;
        public double Longitude { get; set; } = 0.0;
        public string Description { get; set; } = string.Empty;
        public Color MarkerColor { get; set; } = Color.Red;
        public string Icon { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public object Tag { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Map route data structure
    /// </summary>
    public class MapRoute
    {
        public string Name { get; set; } = string.Empty;
        public MapLocation StartLocation { get; set; }
        public MapLocation EndLocation { get; set; }
        public List<MapLocation> Waypoints { get; set; } = new List<MapLocation>();
        public double Distance { get; set; } = 0.0;
        public string EstimatedTime { get; set; } = string.Empty;
        public Color RouteColor { get; set; } = Color.Blue;
        public string RouteType { get; set; } = "Driving";
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public object Tag { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }
}