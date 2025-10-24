using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(250,250,251);
            this.ChartLineColor = Color.FromArgb(250,250,251);
            this.ChartFillColor = Color.FromArgb(250,250,251);
            this.ChartAxisColor = Color.FromArgb(250,250,251);
            this.ChartTitleColor = Color.FromArgb(250,250,251);
            this.ChartTextColor = Color.FromArgb(31,41,55);
            this.ChartLegendBackColor = Color.FromArgb(250,250,251);
            this.ChartLegendTextColor = Color.FromArgb(31,41,55);
            this.ChartLegendShapeColor = Color.FromArgb(250,250,251);
            this.ChartGridLineColor = Color.FromArgb(250,250,251);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(46,106,149), Color.FromArgb(107,140,110), Color.FromArgb(216,122,52), Color.FromArgb(156,163,175) };
        }
    }
}