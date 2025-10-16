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
            this.ChartBackColor = Color.FromArgb(255,255,255);
            this.ChartLineColor = Color.FromArgb(255,255,255);
            this.ChartFillColor = Color.FromArgb(255,255,255);
            this.ChartAxisColor = Color.FromArgb(255,255,255);
            this.ChartTitleColor = Color.FromArgb(255,255,255);
            this.ChartTextColor = Color.FromArgb(31,41,55);
            this.ChartLegendBackColor = Color.FromArgb(255,255,255);
            this.ChartLegendTextColor = Color.FromArgb(31,41,55);
            this.ChartLegendShapeColor = Color.FromArgb(255,255,255);
            this.ChartGridLineColor = Color.FromArgb(255,255,255);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(59,130,246), Color.FromArgb(107,114,128), Color.FromArgb(234,179,8), Color.FromArgb(59,130,246) };
        }
    }
}