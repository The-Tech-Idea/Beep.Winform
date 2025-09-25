using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.DataControls.Helpers
{
    internal interface INavigatorPainter
    {
        void Initialize(BaseControl owner, IBeepTheme theme);
        NavigatorLayout AdjustLayout(Rectangle drawingRect, NavigatorLayout ctx);
        void DrawBackground(Graphics g, NavigatorLayout ctx);
        void DrawForeground(Graphics g, NavigatorLayout ctx);
        void UpdateHitAreas(BaseControl owner, NavigatorLayout ctx, Action<string, Rectangle> notifyAreaHit);
    }

    internal sealed class NavigatorLayout
    {
        public Rectangle DrawingRect;
        public int Radius;
        public Color AccentColor;
        public bool Compact;
    }
}
