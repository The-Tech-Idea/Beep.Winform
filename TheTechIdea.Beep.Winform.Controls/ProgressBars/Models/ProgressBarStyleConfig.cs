using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Models
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ProgressBarStyleConfig
    {
        [Category("Style")]
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        [Category("Layout")]
        public int BorderRadius { get; set; } = 6;

        [Category("Layout")]
        public int BorderThickness { get; set; } = 1;

        [Category("Layout")]
        public int SegmentCount { get; set; } = 10;

        [Category("Layout")]
        public int StripeWidth { get; set; } = 10;

        [Category("Animation")]
        public bool AnimateValueChanges { get; set; } = true;

        [Category("Animation")]
        public bool ShowGlowEffect { get; set; } = true;
    }
}
