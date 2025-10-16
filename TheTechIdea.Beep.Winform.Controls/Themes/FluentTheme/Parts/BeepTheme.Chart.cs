using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(245,246,248);
            this.ChartLineColor = Color.FromArgb(245,246,248);
            this.ChartFillColor = Color.FromArgb(245,246,248);
            this.ChartAxisColor = Color.FromArgb(245,246,248);
            this.ChartTitleColor = Color.FromArgb(245,246,248);
            this.ChartTextColor = Color.FromArgb(32,32,32);
            this.ChartLegendBackColor = Color.FromArgb(245,246,248);
            this.ChartLegendTextColor = Color.FromArgb(32,32,32);
            this.ChartLegendShapeColor = Color.FromArgb(245,246,248);
            this.ChartGridLineColor = Color.FromArgb(245,246,248);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(0,120,215), Color.FromArgb(0,153,188), Color.FromArgb(255,185,0), Color.FromArgb(0,120,215) };
        }
    }
}