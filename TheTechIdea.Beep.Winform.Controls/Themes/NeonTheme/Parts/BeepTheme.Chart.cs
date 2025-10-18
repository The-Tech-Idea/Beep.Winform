using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(10,12,20);
            this.ChartLineColor = Color.FromArgb(10,12,20);
            this.ChartFillColor = Color.FromArgb(10,12,20);
            this.ChartAxisColor = Color.FromArgb(10,12,20);
            this.ChartTitleColor = Color.FromArgb(10,12,20);
            this.ChartTextColor = Color.FromArgb(235,245,255);
            this.ChartLegendBackColor = Color.FromArgb(10,12,20);
            this.ChartLegendTextColor = Color.FromArgb(235,245,255);
            this.ChartLegendShapeColor = Color.FromArgb(10,12,20);
            this.ChartGridLineColor = Color.FromArgb(10,12,20);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(255,64,160), Color.FromArgb(64,224,255), Color.FromArgb(180,86,255), Color.FromArgb(57,255,20) };
        }
    }
}