using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = BackgroundColor;
            this.ChartLineColor = BorderColor;
            this.ChartFillColor = BackgroundColor;
            this.ChartAxisColor = BorderColor;
            this.ChartTitleColor = ForeColor;
            this.ChartTextColor = ForeColor;
            this.ChartLegendBackColor = BackgroundColor;
            this.ChartLegendTextColor = ForeColor;
            this.ChartLegendShapeColor = BackgroundColor;
            this.ChartGridLineColor = InactiveBorderColor;
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { PrimaryColor, SecondaryColor, AccentColor, WarningColor };
        }
    }
}