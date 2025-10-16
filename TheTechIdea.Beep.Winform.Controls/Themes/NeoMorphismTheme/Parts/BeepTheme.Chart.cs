using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(236,240,243);
            this.ChartLineColor = Color.FromArgb(236,240,243);
            this.ChartFillColor = Color.FromArgb(236,240,243);
            this.ChartAxisColor = Color.FromArgb(236,240,243);
            this.ChartTitleColor = Color.FromArgb(236,240,243);
            this.ChartTextColor = Color.FromArgb(58,66,86);
            this.ChartLegendBackColor = Color.FromArgb(236,240,243);
            this.ChartLegendTextColor = Color.FromArgb(58,66,86);
            this.ChartLegendShapeColor = Color.FromArgb(236,240,243);
            this.ChartGridLineColor = Color.FromArgb(236,240,243);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(76,110,245), Color.FromArgb(129,140,248), Color.FromArgb(255,173,94), Color.FromArgb(52,152,219) };
        }
    }
}