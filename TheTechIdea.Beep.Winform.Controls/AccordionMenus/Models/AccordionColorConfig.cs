using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.AccordionMenus.Models
{
    /// <summary>
    /// Color configuration model for accordion menu control
    /// Stores all color properties for theme and customization
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AccordionColorConfig
    {
        [Category("Background")]
        [Description("Accordion background color")]
        public Color AccordionBackgroundColor { get; set; } = Color.FromArgb(240, 240, 240);

        [Category("Background")]
        [Description("Header background color")]
        public Color HeaderBackgroundColor { get; set; } = Color.FromArgb(240, 240, 240);

        [Category("Item Colors")]
        [Description("Item background color (normal)")]
        public Color ItemBackgroundColor { get; set; } = Color.Transparent;

        [Category("Item Colors")]
        [Description("Item background color (hovered)")]
        public Color ItemHoveredBackgroundColor { get; set; } = Color.FromArgb(245, 245, 250);

        [Category("Item Colors")]
        [Description("Item background color (selected)")]
        public Color ItemSelectedBackgroundColor { get; set; } = Color.FromArgb(230, 240, 255);

        [Category("Text Colors")]
        [Description("Header foreground color")]
        public Color HeaderForegroundColor { get; set; } = Color.FromArgb(33, 33, 33);

        [Category("Text Colors")]
        [Description("Item foreground color (normal)")]
        public Color ItemForegroundColor { get; set; } = Color.FromArgb(33, 33, 33);

        [Category("Text Colors")]
        [Description("Item foreground color (selected)")]
        public Color ItemSelectedForegroundColor { get; set; } = Color.FromArgb(33, 150, 243);

        [Category("Visual")]
        [Description("Highlight color (left border indicator)")]
        public Color HighlightColor { get; set; } = Color.FromArgb(33, 150, 243);

        [Category("Visual")]
        [Description("Expander icon color")]
        public Color ExpanderIconColor { get; set; } = Color.FromArgb(100, 100, 100);

        [Category("Visual")]
        [Description("Connector line color")]
        public Color ConnectorLineColor { get; set; } = Color.FromArgb(100, 200, 200, 200);

        public override string ToString() => $"BG: {AccordionBackgroundColor}, Item: {ItemBackgroundColor}, Selected: {ItemSelectedBackgroundColor}";
    }
}
