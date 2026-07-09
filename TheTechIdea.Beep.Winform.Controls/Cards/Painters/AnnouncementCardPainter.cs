using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
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
        private Font _metaFont;
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
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_titleFont = titleFont;
            _metaFont = captionFont;
            _messageFont = bodyFont;
            _actionFont = bodyFont;
            _badgeFont = captionFont;
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

            int subtitleHeight = !string.IsNullOrEmpty(ctx.SubtitleText) ? 18 : 0;
            if (subtitleHeight > 0)
            {
                ctx.SubtitleRect = new Rectangle(
                    contentLeft,
                    ctx.HeaderRect.Bottom + ElementGap / 2,
                    contentWidth,
                    subtitleHeight);
            }
            
            // Message
            int messageHeight = Math.Max(MessageMinHeight, 
                drawingRect.Height - Padding * 2 - TitleHeight - subtitleHeight - ElementGap);
            
            ctx.ParagraphRect = new Rectangle(
                contentLeft,
                (subtitleHeight > 0 ? ctx.SubtitleRect.Bottom : ctx.HeaderRect.Bottom) + ElementGap / 2,
                contentWidth,
                messageHeight);
            
            // Dismiss button (top-right corner)
            ctx.SecondaryButtonRect = new Rectangle(
                drawingRect.Right - Padding - 24,
                drawingRect.Top + Padding,
                24,
                24);
            
            ctx.ShowSecondaryButton = false; // Dismiss button is painter-owned
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

            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.SubtitleRect.IsEmpty)
            {
                var subtitleColor = Color.FromArgb(180, _theme?.CardTextForeColor ?? _owner?.ForeColor ?? Color.Black);
                TextRenderer.DrawText(g, ctx.SubtitleText, _metaFont, ctx.SubtitleRect, subtitleColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
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
            var accentBrush = CardPaintCache.Brush(ctx.AccentColor);
            g.FillRectangle(accentBrush, ctx.DrawingRect.Left, ctx.DrawingRect.Top, DpiScalingHelper.ScaleValue(4, _owner), ctx.DrawingRect.Height);
        }
        
        private void DrawAnnouncementIcon(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw circular background
            var bgBrush = CardPaintCache.Brush(Color.FromArgb(25, ctx.AccentColor));
            g.FillEllipse(bgBrush, ctx.ImageRect);
            
            // Draw megaphone icon (simplified)
            int cx = ctx.ImageRect.Left + ctx.ImageRect.Width / 2;
            int cy = ctx.ImageRect.Top + ctx.ImageRect.Height / 2;
            
            using var iconPen = new Pen(ctx.AccentColor, DpiScalingHelper.ScaleValue(2.5f, _owner));
            iconPen.StartCap = LineCap.Round;
            iconPen.EndCap = LineCap.Round;

            // Megaphone body
            Point[] megaphone = new Point[]
            {
                new Point(cx - DpiScalingHelper.ScaleValue(10, _owner), cy - DpiScalingHelper.ScaleValue(6, _owner)),
                new Point(cx + DpiScalingHelper.ScaleValue(8, _owner), cy - DpiScalingHelper.ScaleValue(12, _owner)),
                new Point(cx + DpiScalingHelper.ScaleValue(8, _owner), cy + DpiScalingHelper.ScaleValue(12, _owner)),
                new Point(cx - DpiScalingHelper.ScaleValue(10, _owner), cy + DpiScalingHelper.ScaleValue(6, _owner))
            };
            g.DrawPolygon(iconPen, megaphone);

            // Handle
            g.DrawLine(iconPen, cx - DpiScalingHelper.ScaleValue(10, _owner), cy - DpiScalingHelper.ScaleValue(3, _owner), cx - DpiScalingHelper.ScaleValue(14, _owner), cy - DpiScalingHelper.ScaleValue(3, _owner));
            g.DrawLine(iconPen, cx - DpiScalingHelper.ScaleValue(10, _owner), cy + DpiScalingHelper.ScaleValue(3, _owner), cx - DpiScalingHelper.ScaleValue(14, _owner), cy + DpiScalingHelper.ScaleValue(3, _owner));

            // Sound waves
            var wavePen = CardPaintCache.Pen(Color.FromArgb(150, ctx.AccentColor), DpiScalingHelper.ScaleValue(1.5f, _owner));
            g.DrawArc(wavePen, cx + DpiScalingHelper.ScaleValue(10, _owner), cy - DpiScalingHelper.ScaleValue(6, _owner), DpiScalingHelper.ScaleValue(8, _owner), DpiScalingHelper.ScaleValue(12, _owner), -60, 120);
            g.DrawArc(wavePen, cx + DpiScalingHelper.ScaleValue(14, _owner), cy - DpiScalingHelper.ScaleValue(9, _owner), DpiScalingHelper.ScaleValue(10, _owner), DpiScalingHelper.ScaleValue(18, _owner), -60, 120);
        }
        
        private void DrawDismissButton(Graphics g, LayoutContext ctx)
        {
            if (ctx.SecondaryButtonRect.IsEmpty) return;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw X
            int margin = DpiScalingHelper.ScaleValue(6, _owner);
            var rect = ctx.SecondaryButtonRect;

            using var pen = new Pen(Color.FromArgb(80, _theme?.CardTextForeColor ?? Color.Black), DpiScalingHelper.ScaleValue(2, _owner));
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
_disposed = true;
        }
        
        #endregion
    }
}

