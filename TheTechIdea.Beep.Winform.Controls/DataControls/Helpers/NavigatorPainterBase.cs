using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.DataControls.Helpers
{
    internal abstract class NavigatorPainterBase : INavigatorPainter
    {
        protected BaseControl Owner;
        protected IBeepTheme Theme;

        public virtual void Initialize(BaseControl owner, IBeepTheme theme)
        {
            Owner = owner;
            Theme = theme;
        }

        public abstract NavigatorLayout AdjustLayout(Rectangle drawingRect, NavigatorLayout ctx);
        public abstract void DrawBackground(Graphics g, NavigatorLayout ctx);
        public abstract void DrawForeground(Graphics g, NavigatorLayout ctx);
        public virtual void UpdateHitAreas(BaseControl owner, NavigatorLayout ctx, System.Action<string, Rectangle> notifyAreaHit) { }

        protected GraphicsPath Round(Rectangle r, int radius)
        {
            int d = radius * 2;
            var p = new GraphicsPath();
            if (radius <= 0) { p.AddRectangle(r); return p; }
            p.AddArc(r.Left, r.Top, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        protected void SoftShadow(Graphics g, Rectangle rect, int radius, int layers = 4, int offset = 2)
        {
            for (int i = layers; i > 0; i--)
            {
                int spread = i * 2;
                int alpha = (int)(18 * (i / (float)layers));
                using var b = new SolidBrush(Color.FromArgb(alpha, Color.Black));
                var r = new Rectangle(rect.X + offset - spread / 2, rect.Y + offset - spread / 2, rect.Width + spread, rect.Height + spread);
                using var path = Round(r, radius + i);
                g.FillPath(b, path);
            }
        }
    }
}
