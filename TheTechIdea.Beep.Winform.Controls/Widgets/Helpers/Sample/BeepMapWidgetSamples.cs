using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Sample
{
    /// <summary>
    /// Sample implementations for BeepMapWidget with all map styles
    /// </summary>
    public static class BeepMapWidgetSamples
    {
        /// <summary>
        /// Creates a live tracking map widget
        /// Uses LiveTrackingPainter.cs
        /// </summary>
        public static BeepMapWidget CreateLiveTrackingWidget()
        {
            return new BeepMapWidget
            {
                Style = MapWidgetStyle.LiveTracking,
                Title = "Live Vehicle Tracking",
                Subtitle = "Fleet Management",
                Latitude = 40.7128,
                Longitude = -74.0060,
                Address = "New York, NY",
                ShowMarkers = true,
                ShowRoutes = true,
                ZoomLevel = 12.0f,
                Size = new Size(320, 200),
                AccentColor = Color.FromArgb(76, 175, 80)
            };
        }

        /// <summary>
        /// Creates a route display map widget
        /// Uses RouteDisplayPainter.cs
        /// </summary>
        public static BeepMapWidget CreateRouteDisplayWidget()
        {
            return new BeepMapWidget
            {
                Style = MapWidgetStyle.RouteDisplay,
                Title = "Delivery Route",
                Subtitle = "Optimized Path",
                Latitude = 34.0522,
                Longitude = -118.2437,
                Address = "Los Angeles, CA",
                ShowRoutes = true,
                RouteColor = Color.FromArgb(33, 150, 243),
                ZoomLevel = 10.0f,
                Size = new Size(350, 220),
                AccentColor = Color.FromArgb(33, 150, 243)
            };
        }

        /// <summary>
        /// Creates a location card map widget
        /// Uses LocationCardPainter.cs
        /// </summary>
        public static BeepMapWidget CreateLocationCardWidget()
        {
            return new BeepMapWidget
            {
                Style = MapWidgetStyle.LocationCard,
                Title = "Head Office",
                Subtitle = "Corporate Headquarters",
                Address = "123 Business Street, Chicago, IL 60601",
                City = "Chicago",
                Region = "Illinois",
                Country = "United States",
                PostalCode = "60601",
                Latitude = 41.8781,
                Longitude = -87.6298,
                ZoomLevel = 15.0f,
                Size = new Size(300, 180),
                AccentColor = Color.FromArgb(156, 39, 176)
            };
        }

        /// <summary>
        /// Creates a geographic heatmap map widget
        /// Uses GeographicHeatmapPainter.cs
        /// </summary>
        public static BeepMapWidget CreateGeographicHeatmapWidget()
        {
            return new BeepMapWidget
            {
                Style = MapWidgetStyle.GeographicHeatmap,
                Title = "Sales Heatmap",
                Subtitle = "Regional Performance",
                Latitude = 39.8283,
                Longitude = -98.5795,
                ZoomLevel = 4.0f,
                Size = new Size(350, 250),
                AccentColor = Color.FromArgb(244, 67, 54)
            };
        }

        /// <summary>
        /// Creates an address card map widget
        /// Uses AddressCardPainter.cs
        /// </summary>
        public static BeepMapWidget CreateAddressCardWidget()
        {
            return new BeepMapWidget
            {
                Style = MapWidgetStyle.AddressCard,
                Title = "Customer Address",
                Address = "456 Oak Avenue",
                City = "Houston",
                Region = "Texas",
                Country = "United States",
                PostalCode = "77001",
                Latitude = 29.7604,
                Longitude = -95.3698,
                Size = new Size(280, 160),
                AccentColor = Color.FromArgb(255, 193, 7)
            };
        }

        /// <summary>
        /// Creates a map preview widget
        /// Uses MapPreviewPainter.cs
        /// </summary>
        public static BeepMapWidget CreateMapPreviewWidget()
        {
            return new BeepMapWidget
            {
                Style = MapWidgetStyle.MapPreview,
                Title = "Location Preview",
                Latitude = 33.4484,
                Longitude = -112.0740,
                Address = "Phoenix, AZ",
                ZoomLevel = 11.0f,
                Size = new Size(200, 120),
                AccentColor = Color.FromArgb(255, 152, 0)
            };
        }

        /// <summary>
        /// Creates a location list map widget
        /// Uses LocationListPainter.cs
        /// </summary>
        public static BeepMapWidget CreateLocationListWidget()
        {
            return new BeepMapWidget
            {
                Style = MapWidgetStyle.LocationList,
                Title = "Store Locations",
                Subtitle = "Branch Network",
                ShowMarkers = true,
                Size = new Size(320, 250),
                AccentColor = Color.FromArgb(103, 58, 183)
            };
        }

        /// <summary>
        /// Creates a travel card map widget
        /// Uses TravelCardPainter.cs
        /// </summary>
        public static BeepMapWidget CreateTravelCardWidget()
        {
            return new BeepMapWidget
            {
                Style = MapWidgetStyle.TravelCard,
                Title = "Business Trip",
                Subtitle = "Travel Itinerary",
                Address = "San Francisco, CA",
                Latitude = 37.7749,
                Longitude = -122.4194,
                Size = new Size(300, 170),
                AccentColor = Color.FromArgb(0, 150, 136)
            };
        }

        /// <summary>
        /// Creates a region map widget
        /// Uses RegionMapPainter.cs
        /// </summary>
        public static BeepMapWidget CreateRegionMapWidget()
        {
            return new BeepMapWidget
            {
                Style = MapWidgetStyle.RegionMap,
                Title = "Regional Coverage",
                Subtitle = "Service Areas",
                Region = "West Coast",
                Country = "United States",
                ZoomLevel = 6.0f,
                Size = new Size(350, 220),
                AccentColor = Color.FromArgb(63, 81, 181)
            };
        }

        /// <summary>
        /// Creates a place card map widget
        /// Uses PlaceCardPainter.cs
        /// </summary>
        public static BeepMapWidget CreatePlaceCardWidget()
        {
            return new BeepMapWidget
            {
                Style = MapWidgetStyle.PlaceCard,
                Title = "Central Park",
                Subtitle = "Tourist Attraction", 
                Address = "Central Park, New York, NY 10024",
                City = "New York",
                Region = "New York",
                Latitude = 40.7829,
                Longitude = -73.9654,
                Size = new Size(280, 180),
                AccentColor = Color.FromArgb(76, 175, 80)
            };
        }

        /// <summary>
        /// Gets all map widget samples
        /// </summary>
        public static BeepMapWidget[] GetAllSamples()
        {
            return new BeepMapWidget[]
            {
                CreateLiveTrackingWidget(),
                CreateRouteDisplayWidget(),
                CreateLocationCardWidget(),
                CreateGeographicHeatmapWidget(),
                CreateAddressCardWidget(),
                CreateMapPreviewWidget(),
                CreateLocationListWidget(),
                CreateTravelCardWidget(),
                CreateRegionMapWidget(),
                CreatePlaceCardWidget()
            };
        }
    }
}