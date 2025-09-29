using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// Interface for widget painters following the same pattern as ICardPainter
    /// </summary>
    internal interface IWidgetPainter
    {
        void Initialize(BaseControl owner, IBeepTheme theme);
        WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx);
        void DrawBackground(Graphics g, WidgetContext ctx);
        void DrawContent(Graphics g, WidgetContext ctx);
        void DrawForegroundAccents(Graphics g, WidgetContext ctx);
        void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle> notifyAreaHit);
    }

    /// <summary>
    /// Layout and data context for widget rendering
    /// </summary>
    internal sealed class WidgetContext
    {
        // Layout rectangles
        public Rectangle DrawingRect;
        public Rectangle ContentRect;
        public Rectangle HeaderRect;
        public Rectangle ValueRect;
        public Rectangle TrendRect;
        public Rectangle IconRect;
        public Rectangle ChartRect;
        public Rectangle LegendRect;
        public Rectangle FooterRect;
        
        // Display flags
        public bool ShowHeader;
        public bool ShowIcon;
        public bool ShowTrend;
        public bool ShowLegend;
        public bool ShowFooter;
        
        // Styling properties
        public Color AccentColor;
        public Color TrendColor;
        public int CornerRadius;
        
        // Data properties
        public object DataSource;
        public string Title;
        public string Value;
        public string TrendValue;
        public string TrendDirection; // "up", "down", "neutral"
        public double TrendPercentage;
        public string Units;
        public string Subtitle;
        public List<string> Labels = new List<string>();
        public List<double> Values = new List<double>();
        public List<Color> Colors = new List<Color>();
        
        // Interactive properties
        public bool IsInteractive;
        public Dictionary<string, object> CustomData = new Dictionary<string, object>();
        public List<string> CustomImagePaths = new List<string>();
        public Rectangle SubHeaderRect { get; internal set; }
        public Rectangle AvatarRect { get; internal set; }
        public string ImagePath { get;  set; }
        public bool ShowStatus { get; internal set; }
        public string? IconPath { get; internal set; }

        // Note: We don't need ClickableAreas here because BaseControl handles hit areas
        // through its AddHitArea(), ClearHitList(), HitTest(), etc. methods
        // Painters will use owner.AddHitArea() in UpdateHitAreas() method
    }
}