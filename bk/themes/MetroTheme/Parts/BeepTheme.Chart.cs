using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(243,242,241);
            this.ChartLineColor = Color.FromArgb(243,242,241);
            this.ChartFillColor = Color.FromArgb(243,242,241);
            this.ChartAxisColor = Color.FromArgb(243,242,241);
            this.ChartTitleColor = Color.FromArgb(243,242,241);
            this.ChartTextColor = Color.FromArgb(32,31,30);
            this.ChartLegendBackColor = Color.FromArgb(243,242,241);
            this.ChartLegendTextColor = Color.FromArgb(32,31,30);
            this.ChartLegendShapeColor = Color.FromArgb(243,242,241);
            this.ChartGridLineColor = Color.FromArgb(243,242,241);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(0,120,215), Color.FromArgb(16,124,16), Color.FromArgb(255,185,0), Color.FromArgb(0,120,215) };
        }
    }
}