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
        public int XLabelInterval = 1;
        public int YLabelInterval = 1;
        /// <summary>Where the legend is placed, so the axis painter
        /// can reserve space on that edge.</summary>
        public LegendPlacement LegendPlacement;
        /// <summary>Number of visible legend items, used to estimate
        /// the space needed for Right-side legends.</summary>
        public int LegendItemCount;
        /// <summary>Current DPI scale factor (DeviceDpi / 96).
        /// Painters multiply fixed pixel values by this so
        /// margins and gaps scale with the display.</summary>
        public float DpiScale = 1f;
    }

    internal interface IChartAxisPainter
    {
        AxisLayout AdjustPlotRect(Graphics g, AxisLayout ctx);
        void DrawAxes(Graphics g, AxisLayout ctx);
        void DrawTicks(Graphics g, AxisLayout ctx);
        void UpdateHitAreas(BaseControl owner, AxisLayout ctx, Action<string, Rectangle> notifyAreaHit);
    }
}
