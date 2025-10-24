using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(250,250,252);
            this.ChartLineColor = Color.FromArgb(250,250,252);
            this.ChartFillColor = Color.FromArgb(250,250,252);
            this.ChartAxisColor = Color.FromArgb(250,250,252);
            this.ChartTitleColor = Color.FromArgb(250,250,252);
            this.ChartTextColor = Color.FromArgb(28,28,30);
            this.ChartLegendBackColor = Color.FromArgb(250,250,252);
            this.ChartLegendTextColor = Color.FromArgb(28,28,30);
            this.ChartLegendShapeColor = Color.FromArgb(250,250,252);
            this.ChartGridLineColor = Color.FromArgb(250,250,252);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(0,122,255), Color.FromArgb(88,86,214), Color.FromArgb(52,199,89), Color.FromArgb(0,122,255) };
        }
    }
}