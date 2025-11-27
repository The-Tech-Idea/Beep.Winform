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
            this.ChartBackColor = SurfaceColor;
            this.ChartLineColor = BorderColor;
            this.ChartFillColor = SecondaryColor;
            this.ChartAxisColor = BorderColor;
            this.ChartTitleColor = ForeColor;
                this.ChartTextColor = ForeColor;
                this.ChartLegendBackColor = PanelBackColor;
                this.ChartLegendTextColor = ForeColor;
                this.ChartLegendShapeColor = PanelBackColor;
                this.ChartGridLineColor = BorderColor;
                this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { PrimaryColor, SecondaryColor, AccentColor, BorderColor };
        }
    }
}