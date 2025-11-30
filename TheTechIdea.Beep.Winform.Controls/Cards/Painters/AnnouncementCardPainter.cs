using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// AnnouncementCard - Announcement banner with icon, title, and action button.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class AnnouncementCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Announcement card fonts
        private Font _titleFont;
        private Font _messageFont;
        private Font _actionFont;
        private Font _badgeFont;
        
        // Announcement card spacing
        private const int Padding = 16;
        private const int IconSize = 48;
        private const int TitleHeight = 26;
        private const int MessageMinHeight = 24;
        private const int ButtonHeight = 36;
        private const int ButtonWidth = 120;
        private const int BadgeWidth = 80;
        private const int BadgeHeight = 22;
        private const int ElementGap = 10;
        private const int ContentGap = 16;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _titleFont?.Dispose(); } catch { }
            try { _messageFont?.Dispose(); } catch { }
            try { _actionFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, 13f, FontStyle.Bold);
            _messageFont = new Font(fontFamily, 10f, FontStyle.Regular);
            _actionFont = new Font(fontFamily, 9f, FontStyle.Bold);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Bold);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Announcement icon (left side)
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + Padding,
                    drawingRect.Top + (drawingRect.Height - IconSize) / 2,
                    IconSize,
                    IconSize);
            }
            
            int contentLeft = drawingRect.Left + Padding + (ctx.ShowImage ? IconSize + ContentGap : 0);
            int contentWidth = drawingRect.Width - (contentLeft - drawingRect.Left) - Padding;
            
            // Action button on right
            if (ctx.ShowButton)
            {
                ctx.ButtonRect = new Rectangle(
                    drawingRect.Right - Padding - ButtonWidth,
                    drawingRect.Top + (drawingRect.Height - ButtonHeight) / 2,
                    ButtonWidth,
                    ButtonHeight);
                contentWidth -= ButtonWidth + ElementGap;
            }
            
            // Badge (NEW, IMPORTANT, etc.) - top right of content
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    contentLeft + contentWidth - BadgeWidth,
                    drawingRect.Top + Padding,
                    BadgeWidth,
                    BadgeHeight);
            }
            
            // Title
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                drawingRect.Top + Padding,
                contentWidth - (string.IsNullOrEmpty(ctx.BadgeText1) ? 0 : BadgeWidth + ElementGap),
                TitleHeight);
            
            // Message
            int messageHeight = Math.Max(MessageMinHeight, 
                drawingRect.Height - Padding * 2 - TitleHeight - ElementGap);
            
            ctx.ParagraphRect = new Rectangle(
                contentLeft,
                ctx.HeaderRect.Bottom + ElementGap / 2,
                contentWidth,
                messageHeight);
            
            // Dismiss button (top-right corner)
            ctx.SecondaryButtonRect = new Rectangle(
                drawingRect.Right - Padding - 24,
                drawingRect.Top + Padding,
                24,
                24);
            
            ctx.ShowSecondaryButton = true;
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Draw gradient background
            if (!ctx.DrawingRect.IsEmpty)
            {
                using var gradientBrush = new LinearGradientBrush(
                    new Point(ctx.DrawingRect.Left, ctx.DrawingRect.Top),
                    new Point(ctx.DrawingRect.Right, ctx.DrawingRect.Bottom),
                    Color.FromArgb(15, ctx.AccentColor),
                    Color.FromArgb(5, ctx.AccentColor));
                
                g.FillRectangle(gradientBrush, ctx.DrawingRect);
            }
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw announcement icon with background
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                DrawAnnouncementIcon(g, ctx);
            }
            
            // Draw badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw dismiss button
            DrawDismissButton(g, ctx);
            
            // Draw accent line at left edge
            using var accentBrush = new SolidBrush(ctx.AccentColor);
            g.FillRectangle(accentBrush, ctx.DrawingRect.Left, ctx.DrawingRect.Top, 4, ctx.DrawingRect.Height);
        }
        
        private void DrawAnnouncementIcon(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw circular background
            using var bgBrush = new SolidBrush(Color.FromArgb(25, ctx.AccentColor));
            g.FillEllipse(bgBrush, ctx.ImageRect);
            
            // Draw megaphone icon (simplified)
            int cx = ctx.ImageRect.Left + ctx.ImageRect.Width / 2;
            int cy = ctx.ImageRect.Top + ctx.ImageRect.Height / 2;
            
            using var iconPen = new Pen(ctx.AccentColor, 2.5f);
            iconPen.StartCap = LineCap.Round;
            iconPen.EndCap = LineCap.Round;
            
            // Megaphone body
            Point[] megaphone = new Point[]
            {
                new Point(cx - 10, cy - 6),
                new Point(cx + 8, cy - 12),
                new Point(cx + 8, cy + 12),
                new Point(cx - 10, cy + 6)
            };
            g.DrawPolygon(iconPen, megaphone);
            
            // Handle
            g.DrawLine(iconPen, cx - 10, cy - 3, cx - 14, cy - 3);
            g.DrawLine(iconPen, cx - 10, cy + 3, cx - 14, cy + 3);
            
            // Sound waves
            using var wavePen = new Pen(Color.FromArgb(150, ctx.AccentColor), 1.5f);
            g.DrawArc(wavePen, cx + 10, cy - 6, 8, 12, -60, 120);
            g.DrawArc(wavePen, cx + 14, cy - 9, 10, 18, -60, 120);
        }
        
        private void DrawDismissButton(Graphics g, LayoutContext ctx)
        {
            if (ctx.SecondaryButtonRect.IsEmpty) return;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw X
            int margin = 6;
            var rect = ctx.SecondaryButtonRect;
            
            using var pen = new Pen(Color.FromArgb(80, Color.Black), 2);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            
            g.DrawLine(pen,
                rect.Left + margin, rect.Top + margin,
                rect.Right - margin, rect.Bottom - margin);
            g.DrawLine(pen,
                rect.Right - margin, rect.Top + margin,
                rect.Left + margin, rect.Bottom - margin);
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Dismiss button hit area
            if (!ctx.SecondaryButtonRect.IsEmpty)
            {
                owner.AddHitArea("Dismiss", ctx.SecondaryButtonRect, null,
                    () => notifyAreaHit?.Invoke("Dismiss", ctx.SecondaryButtonRect));
            }
            
            // Action button hit area
            if (ctx.ShowButton && !ctx.ButtonRect.IsEmpty)
            {
                owner.AddHitArea("Action", ctx.ButtonRect, null,
                    () => notifyAreaHit?.Invoke("Action", ctx.ButtonRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _titleFont?.Dispose();
            _messageFont?.Dispose();
            _actionFont?.Dispose();
            _badgeFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

