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
        public bool ShowDataLabels { get; set; }
        public bool ShowTrendLine { get; set; }
        public float AnimationProgress { get; set; } = 1f; // 0..1
        /// <summary>
        /// Center-hole ratio for Doughnut charts (0.0 = Pie, 0.5 = half-hole).
        /// Default 0.4 (classic doughnut look).
        /// </summary>
        public float DoughnutHoleRatio { get; set; } = 0.4f;
    }
}
