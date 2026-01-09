using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Models
{
    /// <summary>
    /// Color configuration model for tab control
    /// Stores all color properties for theme and customization
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TabColorConfig
    {
        [Category("Colors")]
        [Description("Tab control background color")]
        public Color TabControlBackgroundColor { get; set; } = Color.White;

        [Category("Colors")]
        [Description("Header background color")]
        public Color HeaderBackgroundColor { get; set; } = Color.FromArgb(245, 245, 250);

        [Category("Colors")]
        [Description("Tab background color (normal)")]
        public Color TabBackgroundColor { get; set; } = Color.FromArgb(240, 240, 245);

        [Category("Colors")]
        [Description("Tab background color (hovered)")]
        public Color TabHoveredBackgroundColor { get; set; } = Color.FromArgb(250, 250, 250);

        [Category("Colors")]
        [Description("Tab background color (selected)")]
        public Color TabSelectedBackgroundColor { get; set; } = Color.White;

        [Category("Colors")]
        [Description("Tab text color (normal)")]
        public Color TabTextColor { get; set; } = Color.FromArgb(97, 97, 97);

        [Category("Colors")]
        [Description("Tab text color (selected)")]
        public Color TabSelectedTextColor { get; set; } = Color.FromArgb(33, 37, 41);

        [Category("Colors")]
        [Description("Tab border color (normal)")]
        public Color TabBorderColor { get; set; } = Color.FromArgb(224, 224, 224);

        [Category("Colors")]
        [Description("Tab border color (selected)")]
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(33, 150, 243);

        [Category("Colors")]
        [Description("Tab indicator/underline color")]
        public Color TabIndicatorColor { get; set; } = Color.FromArgb(33, 150, 243);

        [Category("Colors")]
        [Description("Close button color (normal)")]
        public Color CloseButtonColor { get; set; } = Color.FromArgb(158, 158, 158);

        [Category("Colors")]
        [Description("Close button color (hovered)")]
        public Color CloseButtonHoveredColor { get; set; } = Color.FromArgb(33, 150, 243);

        public override string ToString() => $"Tab: {TabBackgroundColor}, Selected: {TabSelectedBackgroundColor}";
    }
}
