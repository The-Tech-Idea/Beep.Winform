using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = BackgroundColor;
            this.ChartLineColor = InactiveBorderColor;
            this.ChartFillColor = BackgroundColor;
            this.ChartAxisColor = InactiveBorderColor;
            this.ChartTitleColor = ForeColor;
            this.ChartTextColor = ForeColor;
            this.ChartLegendBackColor = BackgroundColor;
            this.ChartLegendTextColor = ForeColor;
            this.ChartLegendShapeColor = SurfaceColor;
            this.ChartGridLineColor = InactiveBorderColor;
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { PrimaryColor, SecondaryColor, AccentColor, PrimaryColor };
        }
    }
}