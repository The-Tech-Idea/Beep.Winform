using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using System;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    internal abstract class CardPainterBase : ICardPainter
    {
        protected BaseControl Owner;
        protected IBeepTheme Theme;

        public virtual void Initialize(BaseControl owner, IBeepTheme theme)
        {
            Owner = owner;
            Theme = theme;
        }

        public abstract LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx);
        public abstract void DrawBackground(Graphics g, LayoutContext ctx);
        public abstract void DrawForegroundAccents(Graphics g, LayoutContext ctx);

        // Default implementation - painters can override for custom hit areas
        public virtual void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Default: no additional hit areas beyond what BeepCard already handles
            // Individual painters can override this to add custom interactive areas
        }

        // Helper methods using CardRenderingHelpers
        protected GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            return CardRenderingHelpers.CreateRoundedPath(rect, radius);
        }

        protected void DrawSoftShadow(Graphics g, Rectangle rect, int radius, int layers = 6, int offset = 3)
        {
            CardRenderingHelpers.DrawSoftShadow(g, rect, radius, layers, offset);
        }
    }
}
