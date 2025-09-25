using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    internal interface ICardPainter
    {
        void Initialize(BaseControl owner, IBeepTheme theme);
        LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx);
        void DrawBackground(Graphics g, LayoutContext ctx);
        void DrawForegroundAccents(Graphics g, LayoutContext ctx);
        // Allow painters to register hit areas directly on the owner using BaseControl hit testing
        void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit);
    }

    internal sealed class LayoutContext
    {
        public Rectangle DrawingRect;
        public Rectangle ImageRect;
        public Rectangle HeaderRect;
        public Rectangle ParagraphRect;
        public Rectangle ButtonRect;
        public bool ShowImage;
        public bool ShowButton;
        public int Radius;
        public Color AccentColor;
    }
}
