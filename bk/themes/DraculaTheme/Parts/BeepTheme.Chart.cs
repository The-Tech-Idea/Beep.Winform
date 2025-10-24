using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(40,42,54);
            this.ChartLineColor = Color.FromArgb(40,42,54);
            this.ChartFillColor = Color.FromArgb(40,42,54);
            this.ChartAxisColor = Color.FromArgb(40,42,54);
            this.ChartTitleColor = Color.FromArgb(40,42,54);
            this.ChartTextColor = Color.FromArgb(248,248,242);
            this.ChartLegendBackColor = Color.FromArgb(40,42,54);
            this.ChartLegendTextColor = Color.FromArgb(248,248,242);
            this.ChartLegendShapeColor = Color.FromArgb(40,42,54);
            this.ChartGridLineColor = Color.FromArgb(40,42,54);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(189,147,249), Color.FromArgb(139,233,253), Color.FromArgb(255,121,198), Color.FromArgb(68,71,90) };
        }
    }
}