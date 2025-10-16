using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(246,245,244);
            this.ChartLineColor = Color.FromArgb(246,245,244);
            this.ChartFillColor = Color.FromArgb(246,245,244);
            this.ChartAxisColor = Color.FromArgb(246,245,244);
            this.ChartTitleColor = Color.FromArgb(246,245,244);
            this.ChartTextColor = Color.FromArgb(46,52,54);
            this.ChartLegendBackColor = Color.FromArgb(246,245,244);
            this.ChartLegendTextColor = Color.FromArgb(46,52,54);
            this.ChartLegendShapeColor = Color.FromArgb(246,245,244);
            this.ChartGridLineColor = Color.FromArgb(246,245,244);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(53,132,228), Color.FromArgb(98,160,234), Color.FromArgb(87,227,137), Color.FromArgb(28,113,216) };
        }
    }
}