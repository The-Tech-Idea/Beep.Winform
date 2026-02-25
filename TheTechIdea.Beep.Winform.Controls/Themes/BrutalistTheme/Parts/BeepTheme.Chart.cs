using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor         = SurfaceColor;                              // near-white card background
            this.ChartLineColor         = PrimaryColor;                              // black — series line
            this.ChartFillColor         = Color.FromArgb(40, 0, 0, 0);              // subtle dark fill
            this.ChartAxisColor         = ForeColor;                                 // black axes
            this.ChartTitleColor        = ForeColor;                                 // black title
            this.ChartTextColor         = ForeColor;                                 // black labels
            this.ChartLegendBackColor   = SurfaceColor;                              // near-white legend bg
            this.ChartLegendTextColor   = ForeColor;                                 // black legend text
            this.ChartLegendShapeColor  = ForeColor;                                 // black legend swatch border
            this.ChartGridLineColor     = Color.FromArgb(210, 210, 210);             // subtle light gray grid
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color>
            {
                AccentColor,                                                          // yellow
                PrimaryColor,                                                         // black
                Color.FromArgb(80, 80, 80),                                          // dark gray
                SuccessColor,                                                         // green
                ErrorColor,                                                           // red
                WarningColor                                                          // orange
            };
        }
    }
}