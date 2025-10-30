using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// DialogCard - Simple modal-Style (like confirmation dialogs)
    /// </summary>
    internal sealed class DialogCardPainter : CardPainterBase
    {
        private Font _headerFont;
        private Font _paragraphFont;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            try { _headerFont?.Dispose(); } catch { }
            try { _paragraphFont?.Dispose(); } catch { }

            // Cache lightweight fonts used by dialog painter
            _headerFont = new Font(Owner.Font.FontFamily, Owner.Font.Size +1f, FontStyle.Bold);
            _paragraphFont = new Font(Owner.Font.FontFamily, Owner.Font.Size, FontStyle.Regular);
        }

        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad +4;
            ctx.DrawingRect = drawingRect;

            int top = ctx.DrawingRect.Top + pad;
            if (ctx.ShowImage)
            {
                int icon =48;
                ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + (ctx.DrawingRect.Width - icon) /2, top, icon, icon);
                top = ctx.ImageRect.Bottom +16;
            }

            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, top, ctx.DrawingRect.Width - pad *2, HeaderHeight);
            ctx.ParagraphRect = new Rectangle(ctx.HeaderRect.Left, ctx.HeaderRect.Bottom +12, ctx.HeaderRect.Width,60);

            ctx.SecondaryButtonRect = new Rectangle(ctx.DrawingRect.Right - pad -100 *2 -12, ctx.DrawingRect.Bottom - pad - ButtonHeight,100, ButtonHeight);
            ctx.ButtonRect = new Rectangle(ctx.DrawingRect.Right - pad -100, ctx.DrawingRect.Bottom - pad - ButtonHeight,100, ButtonHeight);

            ctx.ShowSecondaryButton = true;
            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Keep dialog accents minimal and avoid allocating brushes/pens here.
            // If future accents are needed, use PaintersFactory.GetSolidBrush/GetPen
            // and cached fonts created in Initialize to avoid per-paint allocations.
        }
    }
}
