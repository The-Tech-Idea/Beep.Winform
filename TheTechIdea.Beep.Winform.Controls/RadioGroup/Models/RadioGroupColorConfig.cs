using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Models
{
    /// <summary>
    /// Color configuration model for radio group control
    /// Stores all color properties for theme and customization
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class RadioGroupColorConfig
    {
        [Category("Colors")]
        [Description("Group background color")]
        public Color GroupBackgroundColor { get; set; } = Color.White;

        [Category("Colors")]
        [Description("Item background color (normal)")]
        public Color ItemBackgroundColor { get; set; } = Color.White;

        [Category("Colors")]
        [Description("Item background color (hovered)")]
        public Color ItemHoveredBackgroundColor { get; set; } = Color.FromArgb(250, 250, 250);

        [Category("Colors")]
        [Description("Item background color (selected)")]
        public Color ItemSelectedBackgroundColor { get; set; } = Color.FromArgb(245, 245, 255);

        [Category("Colors")]
        [Description("Item background color (focused)")]
        public Color ItemFocusedBackgroundColor { get; set; } = Color.FromArgb(250, 250, 255);

        [Category("Colors")]
        [Description("Item background color (pressed)")]
        public Color ItemPressedBackgroundColor { get; set; } = Color.FromArgb(240, 240, 250);

        [Category("Colors")]
        [Description("Indicator color (normal)")]
        public Color IndicatorColor { get; set; } = Color.FromArgb(97, 97, 97);

        [Category("Colors")]
        [Description("Indicator color (selected)")]
        public Color IndicatorSelectedColor { get; set; } = Color.FromArgb(33, 150, 243);

        [Category("Colors")]
        [Description("Border color (normal)")]
        public Color BorderColor { get; set; } = Color.FromArgb(224, 224, 224);

        [Category("Colors")]
        [Description("Border color (selected)")]
        public Color BorderSelectedColor { get; set; } = Color.FromArgb(33, 150, 243);

        [Category("Colors")]
        [Description("Border color (focused)")]
        public Color BorderFocusedColor { get; set; } = Color.FromArgb(33, 150, 243);

        [Category("Colors")]
        [Description("Text color")]
        public Color TextColor { get; set; } = Color.FromArgb(73, 80, 87);

        [Category("Colors")]
        [Description("Text color (selected)")]
        public Color TextSelectedColor { get; set; } = Color.FromArgb(33, 37, 41);

        [Category("Colors")]
        [Description("Text color (disabled)")]
        public Color TextDisabledColor { get; set; } = Color.FromArgb(180, 180, 180);

        [Category("Colors")]
        [Description("State layer color (hover/focus/press)")]
        public Color StateLayerColor { get; set; } = Color.FromArgb(12, Color.Black);

        public override string ToString() => $"Item: {ItemBackgroundColor}, Indicator: {IndicatorColor}";
    }
}
