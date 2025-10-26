using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = SurfaceColor;
            this.ChartLineColor = AccentColor;
            this.ChartFillColor = ThemeUtil.Lighten(SurfaceColor, 0.06);
            this.ChartAxisColor = ThemeUtil.Lighten(BackgroundColor, 0.15);
            this.ChartTitleColor = ForeColor;
            this.ChartTextColor = ForeColor;
            this.ChartLegendBackColor = SurfaceColor;
            this.ChartLegendTextColor = ForeColor;
            this.ChartLegendShapeColor = PrimaryColor;
            this.ChartGridLineColor = ThemeUtil.Darken(SurfaceColor, 0.08);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { PrimaryColor, AccentColor, SuccessColor, ThemeUtil.Lighten(AccentColor, 0.08) };
        }
    }
}
