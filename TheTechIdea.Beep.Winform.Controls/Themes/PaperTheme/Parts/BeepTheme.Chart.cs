using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(250,250,250);
            this.ChartLineColor = Color.FromArgb(250,250,250);
            this.ChartFillColor = Color.FromArgb(250,250,250);
            this.ChartAxisColor = Color.FromArgb(250,250,250);
            this.ChartTitleColor = Color.FromArgb(250,250,250);
            this.ChartTextColor = Color.FromArgb(33,33,33);
            this.ChartLegendBackColor = Color.FromArgb(250,250,250);
            this.ChartLegendTextColor = Color.FromArgb(33,33,33);
            this.ChartLegendShapeColor = Color.FromArgb(250,250,250);
            this.ChartGridLineColor = Color.FromArgb(250,250,250);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(33,150,243), Color.FromArgb(0,150,136), Color.FromArgb(255,193,7), Color.FromArgb(158,158,158) };
        }
    }
}