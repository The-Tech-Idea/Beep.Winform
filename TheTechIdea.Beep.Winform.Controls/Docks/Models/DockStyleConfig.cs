using System;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Docks;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Models
{
    /// <summary>
    /// Configuration model for dock style properties
    /// Defines visual properties for each dock style
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DockStyleConfig
    {
        [Category("Style")]
        [Description("Dock style")]
        public DockStyle DockStyle { get; set; } = DockStyle.AppleDock;

        [Category("Style")]
        [Description("Associated control style")]
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        [Category("Style")]
        [Description("Show shadow effect")]
        public bool ShowShadow { get; set; } = true;

        [Category("Layout")]
        [Description("Recommended item size")]
        public int RecommendedItemSize { get; set; } = 56;

        [Category("Layout")]
        [Description("Recommended dock height")]
        public int RecommendedDockHeight { get; set; } = 72;

        [Category("Layout")]
        [Description("Recommended spacing between items")]
        public int RecommendedSpacing { get; set; } = 8;

        [Category("Layout")]
        [Description("Recommended padding")]
        public int RecommendedPadding { get; set; } = 12;

        [Category("Animation")]
        [Description("Recommended maximum scale for hover")]
        public float RecommendedMaxScale { get; set; } = 1.5f;

        [Category("Visual")]
        [Description("Recommended background opacity")]
        public float RecommendedBackgroundOpacity { get; set; } = 0.85f;

        [Category("Icons")]
        [Description("Icon size ratio (as percentage of item size)")]
        public float IconSizeRatio { get; set; } = 0.8f;

        public override string ToString() => $"Style: {DockStyle}, Item Size: {RecommendedItemSize}, Height: {RecommendedDockHeight}";
    }
}
