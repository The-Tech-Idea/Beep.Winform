using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = BackgroundColor;
            this.ChartLineColor = BorderColor;
            this.ChartFillColor = SurfaceColor;
            this.ChartAxisColor = BorderColor;
            this.ChartTitleColor = ForeColor;
            this.ChartTextColor = ForeColor;
            this.ChartLegendBackColor = BackgroundColor;
            this.ChartLegendTextColor = ForeColor;
            this.ChartLegendShapeColor = PrimaryColor;
            this.ChartGridLineColor = ThemeUtil.Lighten(BorderColor, 0.1);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { PrimaryColor, AccentColor, SuccessColor, SecondaryColor };
        }
    }
}
