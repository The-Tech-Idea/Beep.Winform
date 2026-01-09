using System;
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Images.Models
{
    /// <summary>
    /// Configuration model for image style properties
    /// Defines visual properties for image styling
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ImageStyleConfig
    {
        [Category("Style")]
        [Description("Clip shape for the image")]
        public ImageClipShape ClipShape { get; set; } = ImageClipShape.None;

        [Category("Style")]
        [Description("Associated control style")]
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        [Category("Layout")]
        [Description("Recommended base size")]
        public int RecommendedBaseSize { get; set; } = 24;

        [Category("Visual")]
        [Description("Recommended corner radius")]
        public float RecommendedCornerRadius { get; set; } = 10f;

        [Category("Visual")]
        [Description("Recommended opacity")]
        public float RecommendedOpacity { get; set; } = 1.0f;

        [Category("Visual")]
        [Description("Recommended scale factor")]
        public float RecommendedScaleFactor { get; set; } = 1.0f;

        public override string ToString() => $"Shape: {ClipShape}, Size: {RecommendedBaseSize}, Radius: {RecommendedCornerRadius}";
    }
}
