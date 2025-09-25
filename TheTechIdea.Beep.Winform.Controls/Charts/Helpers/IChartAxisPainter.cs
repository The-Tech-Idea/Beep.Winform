using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    public enum TimeTickGranularity
    {
        Auto,
        Hours,
        Days,
        Months,
        Years
    }

    internal sealed class AxisLayout
    {
        public Rectangle Bounds;
        public Rectangle PlotRect;
        public AxisType BottomAxisType;
        public AxisType LeftAxisType;
        public float XMin;
        public float XMax;
        public float YMin;
        public float YMax;
        public string XTitle;
        public string YTitle;
        public Font TitleFont;
        public Font LabelFont;
        public Color TextColor;
        public Color AxisColor;
        public Color GridColor;
        public Dictionary<string,int> XCategories;
        public Dictionary<string,int> YCategories;
        public DateTime XDateMin;
        public DateTime YDateMin;
        public bool ShowLegend;
        public float XLabelAngle;
        public float YLabelAngle;
        public TimeTickGranularity XTimeGranularity = TimeTickGranularity.Auto;
        public TimeTickGranularity YTimeGranularity = TimeTickGranularity.Auto;
    }

    internal interface IChartAxisPainter
    {
        AxisLayout AdjustPlotRect(Graphics g, AxisLayout ctx);
        void DrawAxes(Graphics g, AxisLayout ctx);
        void DrawTicks(Graphics g, AxisLayout ctx);
        void UpdateHitAreas(BaseControl owner, AxisLayout ctx, Action<string, Rectangle> notifyAreaHit);
    }
}
