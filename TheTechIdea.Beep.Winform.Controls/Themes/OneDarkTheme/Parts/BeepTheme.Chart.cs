using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(40,44,52);
            this.ChartLineColor = Color.FromArgb(40,44,52);
            this.ChartFillColor = Color.FromArgb(40,44,52);
            this.ChartAxisColor = Color.FromArgb(40,44,52);
            this.ChartTitleColor = Color.FromArgb(40,44,52);
            this.ChartTextColor = Color.FromArgb(171,178,191);
            this.ChartLegendBackColor = Color.FromArgb(40,44,52);
            this.ChartLegendTextColor = Color.FromArgb(171,178,191);
            this.ChartLegendShapeColor = Color.FromArgb(40,44,52);
            this.ChartGridLineColor = Color.FromArgb(40,44,52);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(97,175,239), Color.FromArgb(86,182,194), Color.FromArgb(198,120,221), Color.FromArgb(209,154,102) };
        }
    }
}