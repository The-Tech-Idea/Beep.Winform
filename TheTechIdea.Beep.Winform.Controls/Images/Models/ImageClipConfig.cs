using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Images.Models
{
    /// <summary>
    /// Configuration model for image clipping properties
    /// Stores all clipping-related properties
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ImageClipConfig
    {
        [Category("Clipping")]
        [Description("Clip shape type")]
        public ImageClipShape ClipShape { get; set; } = ImageClipShape.None;

        [Category("Clipping")]
        [Description("Corner radius for rounded shapes")]
        public float CornerRadius { get; set; } = 10f;

        [Category("Clipping")]
        [Description("Custom clip path (when ClipShape is Custom)")]
        [Browsable(false)]
        public GraphicsPath CustomClipPath { get; set; }

        [Category("Clipping")]
        [Description("Use region-based clipping (more efficient for complex shapes)")]
        public bool UseRegionClipping { get; set; } = false;

        public override string ToString() => $"Shape: {ClipShape}, Radius: {CornerRadius}, UseRegion: {UseRegionClipping}";
    }
}
