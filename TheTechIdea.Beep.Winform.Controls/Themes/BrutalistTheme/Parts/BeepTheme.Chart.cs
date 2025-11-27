using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = SurfaceColor;
            this.ChartLineColor = SurfaceColor;
            this.ChartFillColor = SurfaceColor;
            this.ChartAxisColor = SurfaceColor;
            this.ChartTitleColor = ForeColor;
            this.ChartTextColor = ForeColor;
            this.ChartLegendBackColor = SurfaceColor;
            this.ChartLegendTextColor = ForeColor;
            this.ChartLegendShapeColor = SurfaceColor;
            this.ChartGridLineColor = SurfaceColor;
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { PrimaryColor, AccentColor, SecondaryColor, PrimaryColor };
        }
    }
}