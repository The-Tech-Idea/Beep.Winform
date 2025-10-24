using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(245,246,247);
            this.ChartLineColor = Color.FromArgb(245,246,247);
            this.ChartFillColor = Color.FromArgb(245,246,247);
            this.ChartAxisColor = Color.FromArgb(245,246,247);
            this.ChartTitleColor = Color.FromArgb(245,246,247);
            this.ChartTextColor = Color.FromArgb(43,45,48);
            this.ChartLegendBackColor = Color.FromArgb(245,246,247);
            this.ChartLegendTextColor = Color.FromArgb(43,45,48);
            this.ChartLegendShapeColor = Color.FromArgb(245,246,247);
            this.ChartGridLineColor = Color.FromArgb(245,246,247);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(82,148,226), Color.FromArgb(27,106,203), Color.FromArgb(76,201,240), Color.FromArgb(156,163,175) };
        }
    }
}