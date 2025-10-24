using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(236,244,255);
            this.ChartLineColor = Color.FromArgb(236,244,255);
            this.ChartFillColor = Color.FromArgb(236,244,255);
            this.ChartAxisColor = Color.FromArgb(236,244,255);
            this.ChartTitleColor = Color.FromArgb(236,244,255);
            this.ChartTextColor = Color.FromArgb(17,24,39);
            this.ChartLegendBackColor = Color.FromArgb(236,244,255);
            this.ChartLegendTextColor = Color.FromArgb(17,24,39);
            this.ChartLegendShapeColor = Color.FromArgb(236,244,255);
            this.ChartGridLineColor = Color.FromArgb(236,244,255);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(99,102,241), Color.FromArgb(56,189,248), Color.FromArgb(16,185,129), Color.FromArgb(56,189,248) };
        }
    }
}