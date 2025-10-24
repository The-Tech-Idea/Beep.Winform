using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(0,43,54);
            this.ChartLineColor = Color.FromArgb(0,43,54);
            this.ChartFillColor = Color.FromArgb(0,43,54);
            this.ChartAxisColor = Color.FromArgb(0,43,54);
            this.ChartTitleColor = Color.FromArgb(0,43,54);
            this.ChartTextColor = Color.FromArgb(147,161,161);
            this.ChartLegendBackColor = Color.FromArgb(0,43,54);
            this.ChartLegendTextColor = Color.FromArgb(147,161,161);
            this.ChartLegendShapeColor = Color.FromArgb(0,43,54);
            this.ChartGridLineColor = Color.FromArgb(0,43,54);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(38,139,210), Color.FromArgb(42,161,152), Color.FromArgb(108,113,196), Color.FromArgb(181,137,0) };
        }
    }
}