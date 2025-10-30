using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// DataCardPainter - For structured data, forms, and settings
    /// Grid-like layout with labels, values, and minimal decoration
    /// </summary>
    internal sealed class DataCardPainter : CardPainterBase
    {
        private Font _badgeFont;
        private int pad = DefaultPad;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            try { _badgeFont?.Dispose(); } catch { }
            _badgeFont = new Font(Owner.Font.FontFamily,7.5f, FontStyle.Bold);
        }

        public override LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = drawingRect;

            // Optional icon (for settings or form type)
            if (ctx.ShowImage)
            {
                int iconSize =32;
                ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, iconSize, iconSize);
            }

            // Title/section header
            int titleLeft = ctx.DrawingRect.Left + pad + (ctx.ShowImage ?42 :0);
            int titleWidth = ctx.DrawingRect.Width - pad *2 - (ctx.ShowImage ?42 :0);
            ctx.HeaderRect = new Rectangle(titleLeft, ctx.DrawingRect.Top + pad, titleWidth, HeaderHeight);

            // Section badge or status indicator
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(ctx.DrawingRect.Right - pad -80, ctx.DrawingRect.Top + pad,75,18);
            }

            // Data section (labels and values)
            int dataTop = Math.Max(ctx.HeaderRect.Bottom, ctx.ShowImage ? ctx.ImageRect.Bottom :0) +12;
            int dataHeight = Math.Max(60, ctx.DrawingRect.Height - (dataTop - ctx.DrawingRect.Top) - pad *2 - (ctx.ShowButton ? ButtonHeight +8 :0));
            int labelWidth = (ctx.DrawingRect.Width - pad *3) /3;
            int valueWidth = ctx.DrawingRect.Width - pad *2 - labelWidth;
            ctx.SubtitleRect = new Rectangle(ctx.DrawingRect.Left + pad, dataTop, labelWidth, dataHeight);
            ctx.ParagraphRect = new Rectangle(ctx.DrawingRect.Left + pad + labelWidth + pad, dataTop, valueWidth, dataHeight);

            // Action buttons (Edit, Save, Cancel, etc.)
            if (ctx.ShowButton)
            {
                int buttonY = ctx.DrawingRect.Bottom - pad - ButtonHeight;
                if (ctx.ShowSecondaryButton)
                {
                    int buttonWidth = (ctx.DrawingRect.Width - pad *3) /2;
                    ctx.ButtonRect = new Rectangle(ctx.DrawingRect.Left + pad, buttonY, buttonWidth, ButtonHeight);
                    ctx.SecondaryButtonRect = new Rectangle(ctx.ButtonRect.Right + pad, buttonY, buttonWidth, ButtonHeight);
                }
                else
                {
                    ctx.ButtonRect = new Rectangle(ctx.DrawingRect.Left + pad, buttonY, ctx.DrawingRect.Width - pad *2, ButtonHeight);
                }
            }

            return ctx;
        }

        // Container background/shadow handled by BaseControl
        public override void DrawBackground(Graphics g, LayoutContext ctx) { }

        public override void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw status/category badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }

            // Draw divider line between header and data
            var dividerPen = PaintersFactory.GetPen(Color.FromArgb(30, ctx.AccentColor),1);
            int dividerY = ctx.HeaderRect.Bottom +6;
            g.DrawLine(dividerPen, ctx.DrawingRect.Left + pad, dividerY, ctx.DrawingRect.Right - pad, dividerY);

            // Draw subtle vertical separator between labels and values
            if (ctx.SubtitleRect.Width >0 && ctx.ParagraphRect.Width >0)
            {
                int separatorX = ctx.SubtitleRect.Right + pad /2;
                var separatorPen = PaintersFactory.GetPen(Color.FromArgb(20, ctx.AccentColor),1);
                g.DrawLine(separatorPen, separatorX, ctx.SubtitleRect.Top, separatorX, ctx.SubtitleRect.Bottom);
            }

            // Draw grid lines for structured data appearance
            if (ctx.SubtitleRect.Height >80)
            {
                int rowHeight =24;
                int numRows = Math.Min(4, ctx.SubtitleRect.Height / rowHeight);
                var gridPen = PaintersFactory.GetPen(Color.FromArgb(15, ctx.AccentColor),1);
                for (int i =1; i < numRows; i++)
                {
                    int y = ctx.SubtitleRect.Top + (i * rowHeight);
                    g.DrawLine(gridPen, ctx.DrawingRect.Left + pad, y, ctx.DrawingRect.Right - pad, y);
                }
            }
        }
    }
}
