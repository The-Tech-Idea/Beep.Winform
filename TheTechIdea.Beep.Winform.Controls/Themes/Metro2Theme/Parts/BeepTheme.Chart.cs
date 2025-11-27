using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = PanelBackColor;
            this.ChartLineColor = InactiveBorderColor;
            this.ChartFillColor = PanelBackColor;
            this.ChartAxisColor = InactiveBorderColor;
            this.ChartTitleColor = ForeColor;
            this.ChartTextColor = ForeColor;
            this.ChartLegendBackColor = PanelBackColor;
            this.ChartLegendTextColor = ForeColor;
            this.ChartLegendShapeColor = PanelBackColor;
            this.ChartGridLineColor = InactiveBorderColor;
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { PrimaryColor, AccentColor, SuccessColor, PrimaryColor };
        }
    }
}