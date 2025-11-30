using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// CompactProfile - Horizontal profile card for lists with small avatar.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class CompactProfileCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Compact profile fonts
        private Font _nameFont;
        private Font _titleFont;
        private Font _badgeFont;
        private Font _statusFont;
        
        // Compact profile spacing
        private const int Padding = 12;
        private const int AvatarSize = 48;
        private const int NameHeight = 20;
        private const int TitleHeight = 16;
        private const int StatusHeight = 16;
        private const int BadgeWidth = 40;
        private const int BadgeHeight = 18;
        private const int ContentGap = 12;
        private const int ElementGap = 4;
        
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
            
            _nameFont = new Font(fontFamily, 11f, FontStyle.Bold);
            _titleFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _badgeFont = new Font(fontFamily, 7f, FontStyle.Bold);
            _statusFont = new Font(fontFamily, 8f, FontStyle.Regular);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Avatar on left
            ctx.ImageRect = new Rectangle(
                drawingRect.Left + Padding,
                drawingRect.Top + Padding,
                AvatarSize,
                AvatarSize);
            
            // Content area to the right of avatar
            int contentLeft = ctx.ImageRect.Right + ContentGap;
            int contentWidth = Math.Max(60, drawingRect.Width - contentLeft - Padding);
            
            // Name at top
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                drawingRect.Top + Padding,
                contentWidth - BadgeWidth - ElementGap,
                NameHeight);
            
            // Title/subtitle below name
            ctx.SubtitleRect = new Rectangle(
                contentLeft,
                ctx.HeaderRect.Bottom + ElementGap,
                contentWidth,
                TitleHeight);
            
            // Status indicator below title
            ctx.StatusRect = new Rectangle(
                contentLeft,
                ctx.SubtitleRect.Bottom + ElementGap,
                contentWidth,
                StatusHeight);
            
            // Badge at top-right
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - Padding - BadgeWidth,
                    drawingRect.Top + Padding,
                    BadgeWidth,
                    BadgeHeight);
            }
            
            // No buttons for compact profile
            ctx.ShowButton = false;
            ctx.ShowSecondaryButton = false;
            
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
            
            // Draw status with dot
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
