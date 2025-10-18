using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(15,16,32);
            this.ChartLineColor = Color.FromArgb(15,16,32);
            this.ChartFillColor = Color.FromArgb(15,16,32);
            this.ChartAxisColor = Color.FromArgb(15,16,32);
            this.ChartTitleColor = Color.FromArgb(15,16,32);
            this.ChartTextColor = Color.FromArgb(245,247,255);
            this.ChartLegendBackColor = Color.FromArgb(15,16,32);
            this.ChartLegendTextColor = Color.FromArgb(245,247,255);
            this.ChartLegendShapeColor = Color.FromArgb(15,16,32);
            this.ChartGridLineColor = Color.FromArgb(15,16,32);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(255,122,217), Color.FromArgb(122,252,255), Color.FromArgb(176,141,255), Color.FromArgb(159,255,169) };
        }
    }
}