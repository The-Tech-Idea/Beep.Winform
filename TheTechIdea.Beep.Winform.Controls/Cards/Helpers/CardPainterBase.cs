using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    internal abstract class CardPainterBase : ICardPainter
    {
        protected BaseControl Owner;
        protected IBeepTheme Theme;

        // Shared spacing constants
        protected const int DefaultCornerRadius = 12;
        protected const int DefaultPad = 12;
        protected const int HeaderHeight = 26; // consistent title line height
        protected const int ButtonHeight = 32;

        public virtual void Initialize(BaseControl owner, IBeepTheme theme)
        {
            Owner = owner;
            Theme = theme;
        }

        public abstract LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx);
        public abstract void DrawBackground(Graphics g, LayoutContext ctx);
        public abstract void DrawForegroundAccents(Graphics g, LayoutContext ctx);

        public virtual void UpdateHitAreas(BaseControl owner, LayoutContext ctx, System.Action<string, Rectangle> notifyAreaHit)
        {
            // Default: no additional hit areas
        }

        // Common helpers
        protected GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
            => CardRenderingHelpers.CreateRoundedPath(rect, radius);

        protected void DrawSoftShadow(Graphics g, Rectangle rect, int radius, int layers = 6, int offset = 3)
            => CardRenderingHelpers.DrawSoftShadow(g, rect, radius, layers, offset);

        // Aligns a rectangle within a container using ContentAlignment
        protected Rectangle AlignToContent(Rectangle container, Size size, ContentAlignment alignment)
        {
            int x = alignment switch
            {
                ContentAlignment.TopLeft or ContentAlignment.MiddleLeft or ContentAlignment.BottomLeft => container.Left,
                ContentAlignment.TopCenter or ContentAlignment.MiddleCenter or ContentAlignment.BottomCenter => container.Left + (container.Width - size.Width) / 2,
                _ => container.Right - size.Width
            };
            int y = alignment switch
            {
                ContentAlignment.TopLeft or ContentAlignment.TopCenter or ContentAlignment.TopRight => container.Top,
                ContentAlignment.MiddleLeft or ContentAlignment.MiddleCenter or ContentAlignment.MiddleRight => container.Top + (container.Height - size.Height) / 2,
                _ => container.Bottom - size.Height
            };
            return new Rectangle(new Point(x, y), size);
        }

        // Applies a uniform inset from card edges; ensures no negative sizes.
        protected static Rectangle Inset(Rectangle r, int inset)
        {
            var rr = Rectangle.Inflate(r, -inset, -inset);
            if (rr.Width < 0 || rr.Height < 0) return Rectangle.Empty;
            return rr;
        }

        // Returns a vertically stacked block with margin between rows
        protected static Rectangle StackBelow(Rectangle top, int margin, int height, Rectangle bounds)
        {
            var y = top.Bottom + margin;
            var rect = new Rectangle(bounds.Left, y, bounds.Width, height);
            return rect;
        }
    }
}
