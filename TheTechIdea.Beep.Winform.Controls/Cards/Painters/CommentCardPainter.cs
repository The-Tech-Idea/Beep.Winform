using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// CommentCard - Comment/reply card with nested structure support.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class CommentCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Comment card fonts
        private Font _authorFont;
        private Font _timeFont;
        private Font _contentFont;
        private Font _actionFont;
        
        // Comment card spacing
        private const int Padding = 12;
        private const int AvatarSize = 36;
        private const int NestingIndent = 40;
        private const int AuthorHeight = 18;
        private const int ContentMinHeight = 40;
        private const int ActionHeight = 24;
        private const int ActionButtonWidth = 50;
        private const int ElementGap = 6;
        private const int ContentGap = 10;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _authorFont?.Dispose(); } catch { }
            try { _timeFont?.Dispose(); } catch { }
            try { _contentFont?.Dispose(); } catch { }
            try { _actionFont?.Dispose(); } catch { }
            
            _authorFont = new Font(fontFamily, 10f, FontStyle.Bold);
            _timeFont = new Font(fontFamily, 8f, FontStyle.Regular);
            _contentFont = new Font(fontFamily, 9.5f, FontStyle.Regular);
            _actionFont = new Font(fontFamily, 8f, FontStyle.Regular);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Nesting indicator (use Rating as nesting level)
            int nestLevel = ctx.ShowRating ? ctx.Rating : 0;
            int indent = nestLevel * NestingIndent;
            
            // Thread line for nested comments
            if (nestLevel > 0)
            {
                ctx.StatusRect = new Rectangle(
                    drawingRect.Left + Padding + indent - 20,
                    drawingRect.Top,
                    2,
                    drawingRect.Height);
            }
            
            int contentLeft = drawingRect.Left + Padding + indent;
            
            // Avatar
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    contentLeft,
                    drawingRect.Top + Padding,
                    AvatarSize,
                    AvatarSize);
                contentLeft = ctx.ImageRect.Right + ContentGap;
            }
            
            int contentWidth = drawingRect.Width - (contentLeft - drawingRect.Left) - Padding;
            
            // Author name
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                drawingRect.Top + Padding,
                contentWidth / 2,
                AuthorHeight);
            
            // Timestamp
            ctx.SubtitleRect = new Rectangle(
                ctx.HeaderRect.Right + ElementGap,
                ctx.HeaderRect.Top,
                contentWidth / 2 - ElementGap,
                AuthorHeight);
            
            // Comment content
            int contentHeight = Math.Max(ContentMinHeight,
                drawingRect.Height - Padding * 2 - AuthorHeight - ActionHeight - ElementGap * 2);
            
            ctx.ParagraphRect = new Rectangle(
                contentLeft,
                ctx.HeaderRect.Bottom + ElementGap / 2,
                contentWidth,
                contentHeight);
            
            // Action buttons (Like, Reply, etc.)
            ctx.ButtonRect = new Rectangle(
                contentLeft,
                ctx.ParagraphRect.Bottom + ElementGap,
                ActionButtonWidth,
                ActionHeight);
            
            ctx.SecondaryButtonRect = new Rectangle(
                ctx.ButtonRect.Right + ElementGap,
                ctx.ButtonRect.Top,
                ActionButtonWidth,
                ActionHeight);
            
            ctx.ShowButton = true;
            ctx.ShowSecondaryButton = true;
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw thread line for nested comments
            if (ctx.ShowRating && ctx.Rating > 0 && !ctx.StatusRect.IsEmpty)
            {
                using var lineBrush = new SolidBrush(Color.FromArgb(40, ctx.AccentColor));
                g.FillRectangle(lineBrush, ctx.StatusRect);
            }
            
            // Draw avatar
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var bgBrush = new SolidBrush(Color.FromArgb(40, ctx.AccentColor));
                g.FillEllipse(bgBrush, ctx.ImageRect);
                
                using var borderPen = new Pen(Color.FromArgb(30, ctx.AccentColor), 1);
                g.DrawEllipse(borderPen, ctx.ImageRect);
            }
            
            // Draw timestamp
            if (!string.IsNullOrEmpty(ctx.StatusText) && !ctx.SubtitleRect.IsEmpty)
            {
                using var brush = new SolidBrush(Color.FromArgb(100, Color.Black));
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, _timeFont, brush, ctx.SubtitleRect, format);
            }
            
            // Draw action buttons
            DrawActionButtons(g, ctx);
        }
        
        private void DrawActionButtons(Graphics g, LayoutContext ctx)
        {
            // Like button
            if (!ctx.ButtonRect.IsEmpty)
            {
                using var brush = new SolidBrush(Color.FromArgb(120, Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                
                // Like count in BadgeText1
                string likeText = string.IsNullOrEmpty(ctx.BadgeText1) ? "♥ Like" : $"♥ {ctx.BadgeText1}";
                g.DrawString(likeText, _actionFont, brush, ctx.ButtonRect, format);
            }
            
            // Reply button
            if (!ctx.SecondaryButtonRect.IsEmpty)
            {
                using var brush = new SolidBrush(Color.FromArgb(120, Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("↩ Reply", _actionFont, brush, ctx.SecondaryButtonRect, format);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Avatar hit area
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                owner.AddHitArea("Avatar", ctx.ImageRect, null,
                    () => notifyAreaHit?.Invoke("Avatar", ctx.ImageRect));
            }
            
            // Like button
            if (!ctx.ButtonRect.IsEmpty)
            {
                owner.AddHitArea("Like", ctx.ButtonRect, null,
                    () => notifyAreaHit?.Invoke("Like", ctx.ButtonRect));
            }
            
            // Reply button
            if (!ctx.SecondaryButtonRect.IsEmpty)
            {
                owner.AddHitArea("Reply", ctx.SecondaryButtonRect, null,
                    () => notifyAreaHit?.Invoke("Reply", ctx.SecondaryButtonRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _authorFont?.Dispose();
            _timeFont?.Dispose();
            _contentFont?.Dispose();
            _actionFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

