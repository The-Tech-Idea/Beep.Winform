using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(40,40,40);
            this.ChartLineColor = Color.FromArgb(40,40,40);
            this.ChartFillColor = Color.FromArgb(40,40,40);
            this.ChartAxisColor = Color.FromArgb(40,40,40);
            this.ChartTitleColor = Color.FromArgb(40,40,40);
            this.ChartTextColor = Color.FromArgb(235,219,178);
            this.ChartLegendBackColor = Color.FromArgb(40,40,40);
            this.ChartLegendTextColor = Color.FromArgb(235,219,178);
            this.ChartLegendShapeColor = Color.FromArgb(40,40,40);
            this.ChartGridLineColor = Color.FromArgb(40,40,40);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(250,189,47), Color.FromArgb(142,192,124), Color.FromArgb(254,128,25), Color.FromArgb(184,187,38) };
        }
    }
}