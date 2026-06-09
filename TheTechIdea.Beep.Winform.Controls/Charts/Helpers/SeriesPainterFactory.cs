using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal static class SeriesPainterFactory
    {
        public static IChartSeriesPainter Get(ChartType type)
        {
            return type switch
            {
                ChartType.Bar      => new BarSeriesPainter(),
                ChartType.Pie      => new PieSeriesPainter(),
                ChartType.Doughnut => new PieSeriesPainter(),  // same painter, hole via options
                ChartType.Bubble   => new BubbleSeriesPainter(),
                ChartType.Area     => new AreaSeriesPainter(),
                ChartType.Scatter  => new ScatterSeriesPainter(),
                _                  => new LineSeriesPainter()
            };
        }

        // Alias for consistency with the refactored code
        public static IChartSeriesPainter GetPainter(ChartType type) => Get(type);
    }
}
