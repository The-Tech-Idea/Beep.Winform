using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes.Models
{
    /// <summary>
    /// Color configuration model for checkbox control
    /// Stores all color properties for theme and customization
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CheckBoxColorConfig
    {
        [Category("Background")]
        [Description("Checked background color")]
        public Color CheckedBackgroundColor { get; set; } = Color.FromArgb(33, 150, 243);

        [Category("Background")]
        [Description("Unchecked background color")]
        public Color UncheckedBackgroundColor { get; set; } = Color.Transparent;

        [Category("Background")]
        [Description("Indeterminate background color")]
        public Color IndeterminateBackgroundColor { get; set; } = Color.Transparent;

        [Category("Border")]
        [Description("Border color (checked)")]
        public Color CheckedBorderColor { get; set; } = Color.FromArgb(33, 150, 243);

        [Category("Border")]
        [Description("Border color (unchecked)")]
        public Color UncheckedBorderColor { get; set; } = Color.FromArgb(128, 128, 128);

        [Category("Border")]
        [Description("Border color (indeterminate)")]
        public Color IndeterminateBorderColor { get; set; } = Color.FromArgb(128, 128, 128);

        [Category("Text Colors")]
        [Description("Foreground/text color")]
        public Color ForegroundColor { get; set; } = Color.FromArgb(33, 33, 33);

        [Category("Visual")]
        [Description("Check mark color")]
        public Color CheckMarkColor { get; set; } = Color.White;

        [Category("Visual")]
        [Description("Indeterminate mark color")]
        public Color IndeterminateMarkColor { get; set; } = Color.FromArgb(128, 128, 128);

        public override string ToString() => $"Checked BG: {CheckedBackgroundColor}, Border: {CheckedBorderColor}, Check: {CheckMarkColor}";
    }
}
