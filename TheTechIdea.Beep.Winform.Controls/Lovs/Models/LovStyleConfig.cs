using System;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Lovs.Models
{
    /// <summary>
    /// Configuration model for LOV style properties
    /// Defines visual properties for each control style
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LovStyleConfig
    {
        [Category("Style")]
        [Description("Show shadow effect")]
        public bool ShowShadow { get; set; } = false;

        [Category("Style")]
        [Description("Associated control style")]
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        [Category("Layout")]
        [Description("Recommended minimum height")]
        public int RecommendedMinimumHeight { get; set; } = 30;

        [Category("Layout")]
        [Description("Recommended minimum width")]
        public int RecommendedMinimumWidth { get; set; } = 200;

        [Category("Layout")]
        [Description("Recommended padding")]
        public Padding RecommendedPadding { get; set; } = new Padding(4, 2, 4, 2);

        [Category("Layout")]
        [Description("Key textbox width ratio (as percentage)")]
        public float KeyTextBoxWidthRatio { get; set; } = 0.2f;

        [Category("Layout")]
        [Description("Button width ratio (as percentage)")]
        public float ButtonWidthRatio { get; set; } = 0.1f;

        [Category("Layout")]
        [Description("Spacing between elements")]
        public int Spacing { get; set; } = 5;

        [Category("Icons")]
        [Description("Icon size ratio (as percentage of button size)")]
        public float IconSizeRatio { get; set; } = 0.6f;

        public override string ToString() => $"Key: {KeyTextBoxWidthRatio * 100:F0}%, Button: {ButtonWidthRatio * 100:F0}%";
    }
}
