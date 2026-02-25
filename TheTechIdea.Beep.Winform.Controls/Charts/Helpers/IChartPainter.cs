using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal interface IChartPainter
    {
        void Initialize(BaseControl owner, IBeepTheme theme);
        ChartLayout AdjustLayout(Rectangle drawingRect, ChartLayout ctx);
        void DrawBackground(Graphics g, ChartLayout ctx);
        void DrawForeground(Graphics g, ChartLayout ctx);
        void UpdateHitAreas(BaseControl owner, ChartLayout ctx, Action<string, Rectangle> notifyAreaHit);
    }

    internal sealed class ChartLayout
    {
        public Rectangle DrawingRect;
        public Rectangle PlotRect;
        public int Radius;
        public Color AccentColor;
        /// <summary>Y coordinate (absolute) where the title section ends. Set by Paint() after DrawTitleSection.</summary>
        public int TitleBottom;
    }
}
