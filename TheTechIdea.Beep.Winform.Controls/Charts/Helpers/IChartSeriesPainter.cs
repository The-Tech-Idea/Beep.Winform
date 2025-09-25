using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal interface IChartSeriesPainter
    {
        void Initialize(BaseControl owner);
        void DrawSeries(Graphics g, Rectangle plotRect, List<ChartDataSeries> data,
                        Func<ChartDataPoint, object> toX, Func<ChartDataPoint, object> toY,
                        float xMin, float xMax, float yMin, float yMax,
                        List<Color> palette, Color axisColor, Color textColor,
                        SeriesRenderOptions options);
        void UpdateHitAreas(BaseControl owner, Rectangle plotRect, List<ChartDataSeries> data,
                            Func<ChartDataPoint, PointF> toScreen, Action<string, Rectangle> notifyAreaHit);
    }
}
