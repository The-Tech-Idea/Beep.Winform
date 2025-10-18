using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(242,242,245);
            this.ChartLineColor = Color.FromArgb(242,242,245);
            this.ChartFillColor = Color.FromArgb(242,242,245);
            this.ChartAxisColor = Color.FromArgb(242,242,245);
            this.ChartTitleColor = Color.FromArgb(242,242,245);
            this.ChartTextColor = Color.FromArgb(44,44,44);
            this.ChartLegendBackColor = Color.FromArgb(242,242,245);
            this.ChartLegendTextColor = Color.FromArgb(44,44,44);
            this.ChartLegendShapeColor = Color.FromArgb(242,242,245);
            this.ChartGridLineColor = Color.FromArgb(242,242,245);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(233,84,32), Color.FromArgb(119,33,111), Color.FromArgb(244,197,66), Color.FromArgb(156,163,175) };
        }
    }
}