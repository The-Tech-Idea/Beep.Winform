using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(46,52,64);
            this.ChartLineColor = Color.FromArgb(46,52,64);
            this.ChartFillColor = Color.FromArgb(46,52,64);
            this.ChartAxisColor = Color.FromArgb(46,52,64);
            this.ChartTitleColor = Color.FromArgb(46,52,64);
            this.ChartTextColor = Color.FromArgb(216,222,233);
            this.ChartLegendBackColor = Color.FromArgb(46,52,64);
            this.ChartLegendTextColor = Color.FromArgb(216,222,233);
            this.ChartLegendShapeColor = Color.FromArgb(46,52,64);
            this.ChartGridLineColor = Color.FromArgb(46,52,64);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(136,192,208), Color.FromArgb(94,129,172), Color.FromArgb(235,203,139), Color.FromArgb(180,142,173) };
        }
    }
}