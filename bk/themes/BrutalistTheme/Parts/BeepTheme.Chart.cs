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
            this.ChartBackColor = Color.FromArgb(250,250,250);
            this.ChartLineColor = Color.FromArgb(250,250,250);
            this.ChartFillColor = Color.FromArgb(250,250,250);
            this.ChartAxisColor = Color.FromArgb(250,250,250);
            this.ChartTitleColor = Color.FromArgb(250,250,250);
            this.ChartTextColor = Color.FromArgb(20,20,20);
            this.ChartLegendBackColor = Color.FromArgb(250,250,250);
            this.ChartLegendTextColor = Color.FromArgb(20,20,20);
            this.ChartLegendShapeColor = Color.FromArgb(250,250,250);
            this.ChartGridLineColor = Color.FromArgb(250,250,250);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(0,0,0), Color.FromArgb(255,208,0), Color.FromArgb(255,0,102), Color.FromArgb(0,102,255) };
        }
    }
}