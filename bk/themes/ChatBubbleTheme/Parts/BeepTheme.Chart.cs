using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(245,248,255);
            this.ChartLineColor = Color.FromArgb(245,248,255);
            this.ChartFillColor = Color.FromArgb(245,248,255);
            this.ChartAxisColor = Color.FromArgb(245,248,255);
            this.ChartTitleColor = Color.FromArgb(245,248,255);
            this.ChartTextColor = Color.FromArgb(24,28,35);
            this.ChartLegendBackColor = Color.FromArgb(245,248,255);
            this.ChartLegendTextColor = Color.FromArgb(24,28,35);
            this.ChartLegendShapeColor = Color.FromArgb(245,248,255);
            this.ChartGridLineColor = Color.FromArgb(245,248,255);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(74,158,255), Color.FromArgb(155,111,242), Color.FromArgb(255,100,180), Color.FromArgb(74,158,255) };
        }
    }
}