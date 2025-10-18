using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyChart()
        {
            this.ChartBackColor = Color.FromArgb(248,249,250);
            this.ChartLineColor = Color.FromArgb(248,249,250);
            this.ChartFillColor = Color.FromArgb(248,249,250);
            this.ChartAxisColor = Color.FromArgb(248,249,250);
            this.ChartTitleColor = Color.FromArgb(248,249,250);
            this.ChartTextColor = Color.FromArgb(33,37,41);
            this.ChartLegendBackColor = Color.FromArgb(248,249,250);
            this.ChartLegendTextColor = Color.FromArgb(33,37,41);
            this.ChartLegendShapeColor = Color.FromArgb(248,249,250);
            this.ChartGridLineColor = Color.FromArgb(248,249,250);
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<Color> { Color.FromArgb(61,174,233), Color.FromArgb(41,128,185), Color.FromArgb(0,188,212), Color.FromArgb(156,163,175) };
        }
    }
}