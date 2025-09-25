using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    public enum StackedMode
    {
        None,
        Stack,
        Stack100
    }

    public enum LegendPlacement
    {
        Right,
        Top,
        Bottom,
        InsideTopRight
    }

    public sealed class SeriesRenderOptions
    {
        public bool Stacked { get; set; }
        public StackedMode Mode { get; set; } = StackedMode.None;
        public bool SmoothLines { get; set; }
        public bool ShowMarkers { get; set; } = true;
        public float AnimationProgress { get; set; } = 1f; // 0..1
    }
}
