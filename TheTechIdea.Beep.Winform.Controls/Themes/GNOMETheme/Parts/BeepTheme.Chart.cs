using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = SurfaceColor;
            this.ChartLineColor = ForeColor;
            this.ChartFillColor = SurfaceColor;
            this.ChartAxisColor = InactiveBorderColor;
            this.ChartTitleColor = ForeColor;
            this.ChartTextColor = ForeColor;
            this.ChartLegendBackColor = SurfaceColor;
            this.ChartLegendTextColor = ForeColor;
            this.ChartLegendShapeColor = AccentColor;
            this.ChartGridLineColor = InactiveBorderColor;
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { PrimaryColor, SecondaryColor, AccentColor, SuccessColor };
        }
    }
}