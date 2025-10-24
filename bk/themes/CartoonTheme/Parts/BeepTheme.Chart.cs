using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(255,251,235);
            this.ChartLineColor = Color.FromArgb(255,251,235);
            this.ChartFillColor = Color.FromArgb(255,251,235);
            this.ChartAxisColor = Color.FromArgb(255,251,235);
            this.ChartTitleColor = Color.FromArgb(255,251,235);
            this.ChartTextColor = Color.FromArgb(33,37,41);
            this.ChartLegendBackColor = Color.FromArgb(255,251,235);
            this.ChartLegendTextColor = Color.FromArgb(33,37,41);
            this.ChartLegendShapeColor = Color.FromArgb(255,251,235);
            this.ChartGridLineColor = Color.FromArgb(255,251,235);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(255,107,107), Color.FromArgb(255,199,0), Color.FromArgb(72,199,142), Color.FromArgb(30,144,255) };
        }
    }
}