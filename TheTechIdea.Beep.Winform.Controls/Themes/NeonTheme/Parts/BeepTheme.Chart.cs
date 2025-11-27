using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = PanelGradiantMiddleColor;
            this.ChartLineColor = PanelGradiantMiddleColor;
            this.ChartFillColor = PanelGradiantMiddleColor;
            this.ChartAxisColor = PanelGradiantMiddleColor;
            this.ChartTitleColor = PanelGradiantMiddleColor;
            this.ChartTextColor = ForeColor;
            this.ChartLegendBackColor = PanelGradiantMiddleColor;
            this.ChartLegendTextColor = ForeColor;
            this.ChartLegendShapeColor = PanelGradiantMiddleColor;
            this.ChartGridLineColor = InactiveBorderColor;
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { PrimaryColor, SecondaryColor, AccentColor, SuccessColor };
        }
    }
}