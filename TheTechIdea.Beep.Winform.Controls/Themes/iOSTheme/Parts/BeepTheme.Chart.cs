using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(242,242,247);
            this.ChartLineColor = Color.FromArgb(242,242,247);
            this.ChartFillColor = Color.FromArgb(242,242,247);
            this.ChartAxisColor = Color.FromArgb(242,242,247);
            this.ChartTitleColor = Color.FromArgb(242,242,247);
            this.ChartTextColor = Color.FromArgb(28,28,30);
            this.ChartLegendBackColor = Color.FromArgb(242,242,247);
            this.ChartLegendTextColor = Color.FromArgb(28,28,30);
            this.ChartLegendShapeColor = Color.FromArgb(242,242,247);
            this.ChartGridLineColor = Color.FromArgb(242,242,247);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(10,132,255), Color.FromArgb(88,86,214), Color.FromArgb(255,159,10), Color.FromArgb(142,142,147) };
        }
    }
}