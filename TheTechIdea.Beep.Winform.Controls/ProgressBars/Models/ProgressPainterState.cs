using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Models
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class ProgressPainterState
    {
        public int Value { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public int Step { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsFocused { get; set; }
        public bool IsHovered { get; set; }
        public bool IsPressed { get; set; }
        public float Progress01 { get; set; }
        public float DisplayProgress01 { get; set; }
        public ProgressState State { get; set; } = ProgressState.Normal;
        public float IndeterminateOffset { get; set; }
    }
}
