using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
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
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_nameFont = titleFont;
            _titleFont = titleFont;
            _badgeFont = captionFont;
            _statusFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int nameHeight = DpiScalingHelper.ScaleValue(NameHeight, _owner);
            int titleHeight = DpiScalingHelper.ScaleValue(TitleHeight, _owner);
            int statusHeight = DpiScalingHelper.ScaleValue(StatusHeight, _owner);
            int buttonHeight = DpiScalingHelper.ScaleValue(ButtonHeight, _owner);
            int badgeWidth = DpiScalingHelper.ScaleValue(BadgeWidth, _owner);
            int badgeHeight = DpiScalingHelper.ScaleValue(BadgeHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            
            // Banner image area - 40% of card height
            int bannerHeight = Math.Max(80, Math.Min(
                (int)(drawingRect.Height * BannerHeightPercent / 100f),
                drawingRect.Height - (padding * 4)));
            
            ctx.ImageRect = new Rectangle(
                drawingRect.Left + padding,
                drawingRect.Top + padding,
                drawingRect.Width - padding * 2,
                bannerHeight);
            
            // Name below banner
            ctx.HeaderRect = new Rectangle(
                ctx.ImageRect.Left,
                ctx.ImageRect.Bottom + elementGap * 2,
                ctx.ImageRect.Width,
                nameHeight);
            
            // Title/subtitle below name
            ctx.SubtitleRect = new Rectangle(
                ctx.HeaderRect.Left,
                ctx.HeaderRect.Bottom + 2,
                ctx.HeaderRect.Width,
                titleHeight);
            
            // Status indicator below title
            ctx.StatusRect = new Rectangle(
                ctx.SubtitleRect.Left,
                ctx.SubtitleRect.Bottom + elementGap,
                ctx.SubtitleRect.Width,
                statusHeight);
            
            // Badge in top-right corner of banner
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    ctx.ImageRect.Right - badgeWidth - elementGap,
                    ctx.ImageRect.Top + elementGap,
                    badgeWidth,
                    badgeHeight);
            }
            
            // Primary button at bottom - full width
            ctx.ButtonRect = new Rectangle(
                drawingRect.Left + padding,
                drawingRect.Bottom - padding - buttonHeight,
                (drawingRect.Width - padding * 2 - elementGap) / 2,
                buttonHeight);
            
            // Secondary button next to primary
            if (ctx.ShowSecondaryButton)
            {
                ctx.SecondaryButtonRect = new Rectangle(
                    ctx.ButtonRect.Right + elementGap,
                    ctx.ButtonRect.Top,
                    ctx.ButtonRect.Width,
                    buttonHeight);
            }
            else
            {
                // Single button takes full width
                ctx.ButtonRect = new Rectangle(
                    drawingRect.Left + padding,
                    drawingRect.Bottom - padding - buttonHeight,
                    drawingRect.Width - padding * 2,
                    buttonHeight);
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
_disposed = true;
        }
        
        #endregion
    }
}
