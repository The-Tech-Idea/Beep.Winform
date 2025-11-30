using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// PricingCard - Pricing tier with price, features list, and CTA button.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class PricingCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Pricing card fonts
        private Font _tierFont;
        private Font _priceFont;
        private Font _periodFont;
        private Font _featureFont;
        private Font _badgeFont;
        
        // Pricing card spacing
        private const int Padding = 20;
        private const int TierHeight = 28;
        private const int PriceHeight = 48;
        private const int PeriodHeight = 18;
        private const int FeatureRowHeight = 24;
        private const int BadgeWidth = 80;
        private const int BadgeHeight = 22;
        private const int ButtonHeight = 44;
        private const int ElementGap = 8;
        private const int FeatureGap = 6;
        private const int CheckmarkSize = 16;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _tierFont?.Dispose(); } catch { }
            try { _priceFont?.Dispose(); } catch { }
            try { _periodFont?.Dispose(); } catch { }
            try { _featureFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            
            _tierFont = new Font(fontFamily, 14f, FontStyle.Bold);
            _priceFont = new Font(fontFamily, 32f, FontStyle.Bold);
            _periodFont = new Font(fontFamily, 10f, FontStyle.Regular);
            _featureFont = new Font(fontFamily, 10f, FontStyle.Regular);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Bold);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Popular/Recommended badge (top-right)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - Padding - BadgeWidth,
                    drawingRect.Top + Padding,
                    BadgeWidth,
                    BadgeHeight);
            }
            
            // Tier name (header)
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + Padding,
                drawingRect.Top + Padding,
                drawingRect.Width - Padding * 2 - (string.IsNullOrEmpty(ctx.BadgeText1) ? 0 : BadgeWidth + ElementGap),
                TierHeight);
            
            // Price display (large number)
            ctx.SubtitleRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.HeaderRect.Bottom + ElementGap,
                drawingRect.Width - Padding * 2,
                PriceHeight);
            
            // Billing period (/month, /year)
            ctx.StatusRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.SubtitleRect.Bottom,
                drawingRect.Width - Padding * 2,
                PeriodHeight);
            
            // Features list area
            int featuresTop = ctx.StatusRect.Bottom + ElementGap * 2;
            int featuresHeight = Math.Max(60, drawingRect.Bottom - featuresTop - Padding - ButtonHeight - ElementGap * 2);
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + Padding,
                featuresTop,
                drawingRect.Width - Padding * 2,
                featuresHeight);
            
            // CTA button at bottom
            if (ctx.ShowButton)
            {
                ctx.ButtonRect = new Rectangle(
                    drawingRect.Left + Padding,
                    drawingRect.Bottom - Padding - ButtonHeight,
                    drawingRect.Width - Padding * 2,
                    ButtonHeight);
            }
            
            ctx.ShowSecondaryButton = false;
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw "Popular" or "Recommended" badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw price with currency
            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.SubtitleRect.IsEmpty)
            {
                using var brush = new SolidBrush(ctx.AccentColor);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, _priceFont, brush, ctx.SubtitleRect, format);
            }
            
            // Draw billing period
            if (!string.IsNullOrEmpty(ctx.StatusText) && !ctx.StatusRect.IsEmpty)
            {
                using var brush = new SolidBrush(Color.FromArgb(120, ctx.AccentColor));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, _periodFont, brush, ctx.StatusRect, format);
            }
            
            // Draw divider line
            int dividerY = ctx.StatusRect.Bottom + ElementGap;
            using var dividerPen = new Pen(Color.FromArgb(30, ctx.AccentColor), 1);
            g.DrawLine(dividerPen, 
                ctx.DrawingRect.Left + Padding, dividerY,
                ctx.DrawingRect.Right - Padding, dividerY);
            
            // Draw feature list with checkmarks
            DrawFeatureList(g, ctx);
        }
        
        private void DrawFeatureList(Graphics g, LayoutContext ctx)
        {
            if (ctx.Tags == null || ctx.ParagraphRect.IsEmpty) return;
            
            int y = ctx.ParagraphRect.Top + ElementGap;
            int maxY = ctx.ParagraphRect.Bottom - FeatureRowHeight;
            
            using var checkBrush = new SolidBrush(Color.FromArgb(76, 175, 80)); // Green
            using var textBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
            var format = new StringFormat { LineAlignment = StringAlignment.Center };
            
            foreach (var feature in ctx.Tags)
            {
                if (string.IsNullOrWhiteSpace(feature) || y > maxY) break;
                
                // Draw checkmark circle
                var checkRect = new Rectangle(
                    ctx.ParagraphRect.Left,
                    y + (FeatureRowHeight - CheckmarkSize) / 2,
                    CheckmarkSize,
                    CheckmarkSize);
                
                g.FillEllipse(checkBrush, checkRect);
                
                // Draw checkmark
                using var checkPen = new Pen(Color.White, 2);
                checkPen.StartCap = LineCap.Round;
                checkPen.EndCap = LineCap.Round;
                
                int cx = checkRect.Left + checkRect.Width / 2;
                int cy = checkRect.Top + checkRect.Height / 2;
                g.DrawLine(checkPen, cx - 3, cy, cx - 1, cy + 3);
                g.DrawLine(checkPen, cx - 1, cy + 3, cx + 4, cy - 2);
                
                // Draw feature text
                var textRect = new Rectangle(
                    checkRect.Right + ElementGap,
                    y,
                    ctx.ParagraphRect.Width - CheckmarkSize - ElementGap,
                    FeatureRowHeight);
                
                g.DrawString(feature, _featureFont, textBrush, textRect, format);
                
                y += FeatureRowHeight + FeatureGap;
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
            
            _tierFont?.Dispose();
            _priceFont?.Dispose();
            _periodFont?.Dispose();
            _featureFont?.Dispose();
            _badgeFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

