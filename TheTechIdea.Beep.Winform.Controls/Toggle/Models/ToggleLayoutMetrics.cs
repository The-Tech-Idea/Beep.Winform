using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Models
{
    /// <summary>
    /// Layout metrics model for toggle control
    /// Stores calculated bounds for all toggle regions
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ToggleLayoutMetrics
    {
        [Category("Layout")]
        [Description("Track rectangle bounds")]
        public Rectangle TrackBounds { get; set; } = Rectangle.Empty;

        [Category("Layout")]
        [Description("Thumb rectangle bounds")]
        public Rectangle ThumbBounds { get; set; } = Rectangle.Empty;

        [Category("Layout")]
        [Description("ON label rectangle bounds")]
        public Rectangle OnLabelBounds { get; set; } = Rectangle.Empty;

        [Category("Layout")]
        [Description("OFF label rectangle bounds")]
        public Rectangle OffLabelBounds { get; set; } = Rectangle.Empty;

        [Category("Layout")]
        [Description("Icon rectangle bounds")]
        public Rectangle IconBounds { get; set; } = Rectangle.Empty;

        [Category("Layout")]
        [Description("Internal padding")]
        public Padding Padding { get; set; } = new Padding(2);

        [Category("Layout")]
        [Description("Thumb size")]
        public Size ThumbSize { get; set; } = new Size(24, 24);

        [Category("Layout")]
        [Description("Track size")]
        public Size TrackSize { get; set; } = new Size(60, 28);

        [Browsable(false)]
        private Dictionary<string, Rectangle> _regions = new Dictionary<string, Rectangle>();

        /// <summary>
        /// Check if a region exists
        /// </summary>
        public bool HasRegion(string regionName)
        {
            return _regions.ContainsKey(regionName) && !_regions[regionName].IsEmpty;
        }

        /// <summary>
        /// Get bounds for a region by name
        /// </summary>
        public Rectangle GetRegionBounds(string regionName)
        {
            return _regions.TryGetValue(regionName, out var bounds) ? bounds : Rectangle.Empty;
        }

        /// <summary>
        /// Set bounds for a region
        /// </summary>
        public void SetRegionBounds(string regionName, Rectangle bounds)
        {
            _regions[regionName] = bounds;
        }

        /// <summary>
        /// Get all region names
        /// </summary>
        public IEnumerable<string> GetRegionNames()
        {
            return _regions.Keys;
        }

        /// <summary>
        /// Clear all regions
        /// </summary>
        public void Clear()
        {
            _regions.Clear();
            TrackBounds = Rectangle.Empty;
            ThumbBounds = Rectangle.Empty;
            OnLabelBounds = Rectangle.Empty;
            OffLabelBounds = Rectangle.Empty;
            IconBounds = Rectangle.Empty;
        }

        public override string ToString() => $"Track: {TrackBounds}, Thumb: {ThumbBounds}";
    }
}
