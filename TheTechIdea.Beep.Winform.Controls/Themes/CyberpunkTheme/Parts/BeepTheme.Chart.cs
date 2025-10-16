using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(10,8,20);
            this.ChartLineColor = Color.FromArgb(10,8,20);
            this.ChartFillColor = Color.FromArgb(10,8,20);
            this.ChartAxisColor = Color.FromArgb(10,8,20);
            this.ChartTitleColor = Color.FromArgb(10,8,20);
            this.ChartTextColor = Color.FromArgb(228,244,255);
            this.ChartLegendBackColor = Color.FromArgb(10,8,20);
            this.ChartLegendTextColor = Color.FromArgb(228,244,255);
            this.ChartLegendShapeColor = Color.FromArgb(10,8,20);
            this.ChartGridLineColor = Color.FromArgb(10,8,20);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(255,0,128), Color.FromArgb(0,255,230), Color.FromArgb(255,230,0), Color.FromArgb(140,0,255) };
        }
    }
}