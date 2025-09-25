using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal static class SeriesPainterFactory
    {
        public static IChartSeriesPainter Get(ChartType type)
        {
            return type switch
            {
                ChartType.Bar => new BarSeriesPainter(),
                ChartType.Pie => new PieSeriesPainter(),
                ChartType.Bubble => new BubbleSeriesPainter(),
                ChartType.Area => new AreaSeriesPainter(),
                _ => new LineSeriesPainter()
            };
        }
    }
}
