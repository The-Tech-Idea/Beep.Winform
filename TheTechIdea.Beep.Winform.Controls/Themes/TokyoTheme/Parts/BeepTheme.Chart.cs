using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(26,27,38);
            this.ChartLineColor = Color.FromArgb(26,27,38);
            this.ChartFillColor = Color.FromArgb(26,27,38);
            this.ChartAxisColor = Color.FromArgb(26,27,38);
            this.ChartTitleColor = Color.FromArgb(26,27,38);
            this.ChartTextColor = Color.FromArgb(192,202,245);
            this.ChartLegendBackColor = Color.FromArgb(26,27,38);
            this.ChartLegendTextColor = Color.FromArgb(192,202,245);
            this.ChartLegendShapeColor = Color.FromArgb(26,27,38);
            this.ChartGridLineColor = Color.FromArgb(26,27,38);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(122,162,247), Color.FromArgb(42,195,222), Color.FromArgb(187,154,247), Color.FromArgb(255,158,100) };
        }
    }
}