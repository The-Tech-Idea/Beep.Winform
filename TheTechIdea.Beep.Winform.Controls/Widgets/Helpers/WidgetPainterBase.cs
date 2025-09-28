using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using System;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// Base class for widget painters providing common functionality
    /// </summary>
    internal abstract class WidgetPainterBase : IWidgetPainter
    {
        protected BaseControl Owner;
        protected IBeepTheme Theme;

        public virtual void Initialize(BaseControl owner, IBeepTheme theme)
        {
            Owner = owner;
            Theme = theme;
        }

        public abstract WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx);
        public abstract void DrawBackground(Graphics g, WidgetContext ctx);
        public abstract void DrawContent(Graphics g, WidgetContext ctx);
        public abstract void DrawForegroundAccents(Graphics g, WidgetContext ctx);

        // Default implementation - painters can override for custom hit areas
        public virtual void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Default: no additional hit areas beyond what widget already handles
            // Individual painters can override this to add custom interactive areas
        }

        // Helper methods using WidgetRenderingHelpers
        protected GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            return WidgetRenderingHelpers.CreateRoundedPath(rect, radius);
        }

        protected void DrawSoftShadow(Graphics g, Rectangle rect, int radius, int layers = 4, int offset = 2)
        {
            WidgetRenderingHelpers.DrawSoftShadow(g, rect, radius, layers, offset);
        }

        protected void DrawTrendArrow(Graphics g, Rectangle rect, string direction, Color color)
        {
            WidgetRenderingHelpers.DrawTrendArrow(g, rect, direction, color);
        }

        protected void DrawProgressBar(Graphics g, Rectangle rect, double percentage, Color fillColor, Color backgroundColor)
        {
            WidgetRenderingHelpers.DrawProgressBar(g, rect, percentage, fillColor, backgroundColor);
        }

        protected void DrawValue(Graphics g, Rectangle rect, string value, string units, Font font, Color color, StringAlignment alignment = StringAlignment.Center)
        {
            WidgetRenderingHelpers.DrawValue(g, rect, value, units, font, color, alignment);
        }
    }
}