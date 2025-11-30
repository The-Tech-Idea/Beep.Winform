using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using PaintersFactory = TheTechIdea.Beep.Winform.Controls.Styling.PaintersFactory;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// SocialMediaCard - For social media posts, feed items, announcements
    /// </summary>
    internal sealed class SocialMediaCardPainter : ICardPainter, IDisposable
    {
        private BaseControl Owner;
        private IBeepTheme Theme;
        private Font _timeFont;
        private Font _iconFont;

        // Constants specific to this painter
        private const int DefaultPad = 12;
        private const int HeaderHeight = 26;
        private const int ButtonHeight = 32;

        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            Owner = owner;
            Theme = theme;
            try { _timeFont?.Dispose(); } catch { }
            try { _iconFont?.Dispose(); } catch { }
            _timeFont = new Font(Owner.Font.FontFamily, 7.5f, FontStyle.Regular);
            _iconFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
        }

        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            int pad = DefaultPad;
            ctx.DrawingRect = drawingRect;

            int avatar = 40;
            ctx.ImageRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, avatar, avatar);

            int headerLeft = ctx.ImageRect.Right + 12;
            int headerRight = Math.Max(headerLeft, ctx.DrawingRect.Right - pad - 80 - 8);
            int headerWidth = Math.Max(0, headerRight - headerLeft);
            ctx.HeaderRect = new Rectangle(headerLeft, ctx.DrawingRect.Top + pad, headerWidth, 18);
            ctx.SubtitleRect = new Rectangle(headerLeft, ctx.HeaderRect.Bottom + 2, headerWidth, 14);

            ctx.StatusRect = new Rectangle(ctx.DrawingRect.Right - pad - 80, ctx.DrawingRect.Top + pad, 75, 16);

            int contentTop = ctx.ImageRect.Bottom + 12;
            ctx.ParagraphRect = new Rectangle(ctx.DrawingRect.Left + pad, contentTop, ctx.DrawingRect.Width - pad * 2, Math.Max(40, ctx.DrawingRect.Bottom - contentTop - pad - 28));

            int buttonY = ctx.DrawingRect.Bottom - pad - 24;
            ctx.ButtonRect = new Rectangle(ctx.DrawingRect.Left + pad, buttonY, 60, 20);
            ctx.SecondaryButtonRect = new Rectangle(ctx.ButtonRect.Right + 12, buttonY, 60, 20);

            ctx.TagsRect = new Rectangle(ctx.DrawingRect.Left + pad, Math.Min(ctx.ParagraphRect.Bottom + 8, buttonY - 24), ctx.DrawingRect.Width - pad * 2, 20);

            ctx.ShowSecondaryButton = true;
            return ctx;
        }

        public void DrawBackground(Graphics g, LayoutContext ctx) { }

        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw timestamp
            if (!string.IsNullOrEmpty(ctx.StatusText))
            {
                var timeBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(120, Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, _timeFont, timeBrush, ctx.StatusRect, format);
            }
            
            // Draw hashtags/mentions
            CardRenderingHelpers.DrawChips(g, Owner, ctx.TagsRect, ctx.AccentColor, ctx.Tags);
            
            // Draw engagement icons (like, share, etc.)
            if (ctx.ShowButton)
            {
                var iconBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(150, Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("♥ Like", _iconFont, iconBrush, ctx.ButtonRect, format);
                
                if (ctx.ShowSecondaryButton)
                {
                    g.DrawString("↗ Share", _iconFont, iconBrush, ctx.SecondaryButtonRect, format);
                }
            }
        }

        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Add clickable areas for social interactions
            if (!ctx.ButtonRect.IsEmpty)
                owner.AddHitArea("Like", ctx.ButtonRect, null, () => notifyAreaHit?.Invoke("Like", ctx.ButtonRect));
            
            if (!ctx.SecondaryButtonRect.IsEmpty)
                owner.AddHitArea("Share", ctx.SecondaryButtonRect, null, () => notifyAreaHit?.Invoke("Share", ctx.SecondaryButtonRect));
            
            if (!ctx.ImageRect.IsEmpty)
                owner.AddHitArea("Profile", ctx.ImageRect, null, () => notifyAreaHit?.Invoke("Profile", ctx.ImageRect));
        }

        private bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed)
            {
                _timeFont?.Dispose();
                _iconFont?.Dispose();
                _disposed = true;
            }
        }
    }
}
