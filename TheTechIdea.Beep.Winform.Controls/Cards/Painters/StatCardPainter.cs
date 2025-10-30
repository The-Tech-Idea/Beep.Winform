using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// StatCard - For displaying key metrics, KPIs, and statistics
    /// </summary>
    internal sealed class StatCardPainter : CardPainterBase
    {
        private Font _valueFont;
        private Font _trendFont;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            try { _valueFont?.Dispose(); } catch { }
            try { _trendFont?.Dispose(); } catch { }
            _valueFont = new Font(Owner.Font.FontFamily, 24f, FontStyle.Bold);
            _trendFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
        }

        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = drawingRect;

            if (ctx.ShowImage)
            {
                int iconSize = 32;
                ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, iconSize, iconSize);
            }

            int headerTop = ctx.DrawingRect.Top + pad + (ctx.ShowImage ? 40 : 0);
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, headerTop, ctx.DrawingRect.Width - pad * 2, 36);
            ctx.ParagraphRect = new Rectangle(ctx.HeaderRect.Left, ctx.HeaderRect.Bottom + 4, ctx.HeaderRect.Width, 20);
            ctx.SubtitleRect = new Rectangle(Math.Max(ctx.DrawingRect.Right - pad - 80, ctx.HeaderRect.Left), Math.Max(ctx.DrawingRect.Bottom - pad - 16, ctx.ParagraphRect.Bottom + 4), 75, 16);

            if (ctx.ShowStatus)
            {
                ctx.StatusRect = new Rectangle(ctx.DrawingRect.Left, ctx.DrawingRect.Bottom - 4, ctx.DrawingRect.Width, 4);
            }

            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            if (!string.IsNullOrEmpty(ctx.SubtitleText)) // Main stat value in SubtitleText
            {
                var valueBrush = PaintersFactory.GetSolidBrush(ctx.AccentColor);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, _valueFont, valueBrush, ctx.HeaderRect, format);
            }
            
            // Draw trend indicator with color coding
            if (!string.IsNullOrEmpty(ctx.StatusText))
            {
                Color trendColor = ctx.StatusText.StartsWith("+") ? Color.Green : 
                                   ctx.StatusText.StartsWith("-") ? Color.Red : Color.Gray;
                var trendBrush = PaintersFactory.GetSolidBrush(trendColor);
                var format = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, _trendFont, trendBrush, ctx.SubtitleRect, format);
            }
            
            // Draw status accent line at bottom
            if (ctx.ShowStatus)
            {
                var statusBrush = PaintersFactory.GetSolidBrush(ctx.StatusColor);
                g.FillRectangle(statusBrush, ctx.StatusRect);
            }
        }
    }
}
