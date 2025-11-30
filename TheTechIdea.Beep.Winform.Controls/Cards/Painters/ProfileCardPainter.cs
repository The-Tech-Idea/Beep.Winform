using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// ProfileCard - Vertical profile with large banner image, name, status, and action buttons.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class ProfileCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Profile-specific fonts
        private Font _nameFont;
        private Font _titleFont;
        private Font _badgeFont;
        private Font _statusFont;
        
        // Profile-specific spacing
        private const int Padding = 16;
        private const int BannerHeightPercent = 40;
        private const int NameHeight = 28;
        private const int TitleHeight = 20;
        private const int StatusHeight = 18;
        private const int ButtonHeight = 40;
        private const int BadgeWidth = 50;
        private const int BadgeHeight = 22;
        private const int ElementGap = 8;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _nameFont?.Dispose(); } catch { }
            try { _titleFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            try { _statusFont?.Dispose(); } catch { }
            
            _nameFont = new Font(fontFamily, 14f, FontStyle.Bold);
            _titleFont = new Font(fontFamily, 10f, FontStyle.Regular);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Bold);
            _statusFont = new Font(fontFamily, 9f, FontStyle.Regular);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Banner image area - 40% of card height
            int bannerHeight = Math.Max(80, Math.Min(
                (int)(drawingRect.Height * BannerHeightPercent / 100f),
                drawingRect.Height - (Padding * 4)));
            
            ctx.ImageRect = new Rectangle(
                drawingRect.Left + Padding,
                drawingRect.Top + Padding,
                drawingRect.Width - Padding * 2,
                bannerHeight);
            
            // Name below banner
            ctx.HeaderRect = new Rectangle(
                ctx.ImageRect.Left,
                ctx.ImageRect.Bottom + ElementGap * 2,
                ctx.ImageRect.Width,
                NameHeight);
            
            // Title/subtitle below name
            ctx.SubtitleRect = new Rectangle(
                ctx.HeaderRect.Left,
                ctx.HeaderRect.Bottom + 2,
                ctx.HeaderRect.Width,
                TitleHeight);
            
            // Status indicator below title
            ctx.StatusRect = new Rectangle(
                ctx.SubtitleRect.Left,
                ctx.SubtitleRect.Bottom + ElementGap,
                ctx.SubtitleRect.Width,
                StatusHeight);
            
            // Badge in top-right corner of banner
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    ctx.ImageRect.Right - BadgeWidth - ElementGap,
                    ctx.ImageRect.Top + ElementGap,
                    BadgeWidth,
                    BadgeHeight);
            }
            
            // Primary button at bottom - full width
            ctx.ButtonRect = new Rectangle(
                drawingRect.Left + Padding,
                drawingRect.Bottom - Padding - ButtonHeight,
                (drawingRect.Width - Padding * 2 - ElementGap) / 2,
                ButtonHeight);
            
            // Secondary button next to primary
            if (ctx.ShowSecondaryButton)
            {
                ctx.SecondaryButtonRect = new Rectangle(
                    ctx.ButtonRect.Right + ElementGap,
                    ctx.ButtonRect.Top,
                    ctx.ButtonRect.Width,
                    ButtonHeight);
            }
            else
            {
                // Single button takes full width
                ctx.ButtonRect = new Rectangle(
                    drawingRect.Left + Padding,
                    drawingRect.Bottom - Padding - ButtonHeight,
                    drawingRect.Width - Padding * 2,
                    ButtonHeight);
            }
            
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1, 
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw status indicator with dot
            if (ctx.ShowStatus && !string.IsNullOrEmpty(ctx.StatusText) && !ctx.StatusRect.IsEmpty)
            {
                CardRenderingHelpers.DrawStatus(g, ctx.StatusRect, ctx.StatusText, 
                    ctx.StatusColor, _statusFont);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (!ctx.BadgeRect.IsEmpty)
            {
                owner.AddHitArea("Badge", ctx.BadgeRect, null, 
                    () => notifyAreaHit?.Invoke("Badge", ctx.BadgeRect));
            }
            
            if (ctx.ShowStatus && !ctx.StatusRect.IsEmpty)
            {
                owner.AddHitArea("Status", ctx.StatusRect, null, 
                    () => notifyAreaHit?.Invoke("Status", ctx.StatusRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _nameFont?.Dispose();
            _titleFont?.Dispose();
            _badgeFont?.Dispose();
            _statusFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}
