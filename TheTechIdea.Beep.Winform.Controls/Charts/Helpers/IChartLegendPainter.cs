using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal interface IChartLegendPainter
    {
        void DrawLegend(Graphics g,
                        Rectangle chartRect,
                        List<ChartDataSeries> data,
                        List<Color> palette,
                        Font font,
                        Color textColor,
                        Color backColor,
                        Color shapeColor,
                        BaseControl owner,
                        Action<int> onToggleSeries,
                        LegendPlacement placement = LegendPlacement.Right);
    }
}
