using System;
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.AccordionMenus.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.AccordionMenus.Models
{
    /// <summary>
    /// Configuration model for accordion menu style properties
    /// Defines visual properties for each accordion style
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AccordionStyleConfig
    {
        [Category("Style")]
        [Description("Accordion style")]
        public AccordionStyle AccordionStyle { get; set; } = AccordionStyle.Material3;

        [Category("Style")]
        [Description("Associated control style")]
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        [Category("Layout")]
        [Description("Recommended item height")]
        public int RecommendedItemHeight { get; set; } = 40;

        [Category("Layout")]
        [Description("Recommended child item height")]
        public int RecommendedChildItemHeight { get; set; } = 32;

        [Category("Layout")]
        [Description("Recommended header height")]
        public int RecommendedHeaderHeight { get; set; } = 48;

        [Category("Layout")]
        [Description("Recommended indentation width")]
        public int RecommendedIndentation { get; set; } = 20;

        [Category("Layout")]
        [Description("Recommended spacing between items")]
        public int RecommendedSpacing { get; set; } = 2;

        [Category("Layout")]
        [Description("Recommended padding")]
        public int RecommendedPadding { get; set; } = 8;

        [Category("Visual")]
        [Description("Recommended border radius")]
        public int RecommendedBorderRadius { get; set; } = 8;

        [Category("Visual")]
        [Description("Recommended highlight width")]
        public int RecommendedHighlightWidth { get; set; } = 4;

        public override string ToString() => $"Style: {AccordionStyle}, Item Height: {RecommendedItemHeight}, Header: {RecommendedHeaderHeight}";
    }
}
