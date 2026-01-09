using System;
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Chips.Models
{
    /// <summary>
    /// Configuration model for chip style properties
    /// Defines visual properties for each chip style
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ChipStyleConfig
    {
        [Category("Style")]
        [Description("Chip style")]
        public ChipStyle ChipStyle { get; set; } = ChipStyle.Default;

        [Category("Style")]
        [Description("Associated control style")]
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        [Category("Style")]
        [Description("Chip variant")]
        public ChipVariant ChipVariant { get; set; } = ChipVariant.Filled;

        [Category("Style")]
        [Description("Chip color")]
        public ChipColor ChipColor { get; set; } = ChipColor.Default;

        [Category("Style")]
        [Description("Chip size")]
        public ChipSize ChipSize { get; set; } = ChipSize.Medium;

        [Category("Style")]
        [Description("Chip shape")]
        public ChipShape ChipShape { get; set; } = ChipShape.Rounded;

        [Category("Layout")]
        [Description("Recommended chip height")]
        public int RecommendedChipHeight { get; set; } = 32;

        [Category("Layout")]
        [Description("Recommended horizontal padding")]
        public int RecommendedHorizontalPadding { get; set; } = 12;

        [Category("Layout")]
        [Description("Recommended vertical padding")]
        public int RecommendedVerticalPadding { get; set; } = 6;

        [Category("Layout")]
        [Description("Recommended gap between chips")]
        public int RecommendedGap { get; set; } = 6;

        [Category("Visual")]
        [Description("Recommended border width")]
        public int RecommendedBorderWidth { get; set; } = 1;

        [Category("Visual")]
        [Description("Show borders")]
        public bool ShowBorders { get; set; } = true;

        [Category("Visual")]
        [Description("Recommended corner radius")]
        public int RecommendedCornerRadius { get; set; } = 15;

        public override string ToString() => $"Style: {ChipStyle}, Variant: {ChipVariant}, Size: {ChipSize}";
    }
}
