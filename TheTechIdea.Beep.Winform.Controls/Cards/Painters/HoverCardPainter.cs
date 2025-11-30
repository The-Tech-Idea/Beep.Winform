using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// HoverCard - Card with pronounced hover effects and transitions.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class HoverCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Hover card fonts
        private Font _titleFont;
        private Font _descFont;
        private Font _actionFont;
        
        // Hover card spacing
        private const int Padding = 20;
        private const int IconSize = 56;
        private const int TitleHeight = 28;
        private const int DescMinHeight = 48;
        private const int ActionHeight = 24;
        private const int ElementGap = 12;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _titleFont?.Dispose(); } catch { }
            try { _descFont?.Dispose(); } catch { }
            try { _actionFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, 14f, FontStyle.Bold);
            _descFont = new Font(fontFamily, 10f, FontStyle.Regular);
            _actionFont = new Font(fontFamily, 10f, FontStyle.Bold);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Centered icon at top
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + (drawingRect.Width - IconSize) / 2,
                    drawingRect.Top + Padding,
                    IconSize,
                    IconSize);
            }
            
            int contentTop = ctx.ShowImage ? ctx.ImageRect.Bottom + ElementGap : drawingRect.Top + Padding;
            
            // Title (centered)
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + Padding,
                contentTop,
                drawingRect.Width - Padding * 2,
                TitleHeight);
            
            // Description (centered)
            int descHeight = Math.Max(DescMinHeight,
                drawingRect.Height - (ctx.HeaderRect.Bottom - drawingRect.Top) - Padding - ElementGap - 
                (ctx.ShowButton ? ActionHeight + ElementGap : 0));
            
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.HeaderRect.Bottom + ElementGap / 2,
                drawingRect.Width - Padding * 2,
                descHeight);
            
            // Action link (centered at bottom)
            if (ctx.ShowButton)
            {
                ctx.ButtonRect = new Rectangle(
                    drawingRect.Left + Padding,
                    drawingRect.Bottom - Padding - ActionHeight,
                    drawingRect.Width - Padding * 2,
                    ActionHeight);
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
            // Draw icon with hover-ready styling
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                DrawHoverIcon(g, ctx);
            }
            
            // Draw action link with arrow
            if (ctx.ShowButton && !ctx.ButtonRect.IsEmpty)
            {
                DrawActionLink(g, ctx);
            }
            
            // Draw subtle top accent line
            using var accentPen = new Pen(ctx.AccentColor, 3);
            g.DrawLine(accentPen,
                ctx.DrawingRect.Left + Padding, ctx.DrawingRect.Top,
                ctx.DrawingRect.Right - Padding, ctx.DrawingRect.Top);
        }
        
        private void DrawHoverIcon(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Large circular background with gradient
            var bgRect = new Rectangle(
                ctx.ImageRect.X - 8,
                ctx.ImageRect.Y - 8,
                ctx.ImageRect.Width + 16,
                ctx.ImageRect.Height + 16);
            
            using var gradientBrush = new LinearGradientBrush(
                new Point(bgRect.Left, bgRect.Top),
                new Point(bgRect.Right, bgRect.Bottom),
                Color.FromArgb(30, ctx.AccentColor),
                Color.FromArgb(10, ctx.AccentColor));
            
            g.FillEllipse(gradientBrush, bgRect);
            
            // Inner circle
            using var innerBrush = new SolidBrush(Color.FromArgb(15, ctx.AccentColor));
            g.FillEllipse(innerBrush, ctx.ImageRect);
        }
        
        private void DrawActionLink(Graphics g, LayoutContext ctx)
        {
            // Draw action text with arrow
            string actionText = ctx.BadgeText1 ?? "Learn more";
            
            using var textBrush = new SolidBrush(ctx.AccentColor);
            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            
            // Measure text to position arrow
            var textSize = g.MeasureString(actionText, _actionFont);
            float textLeft = ctx.ButtonRect.Left + (ctx.ButtonRect.Width - textSize.Width - 20) / 2;
            
            // Draw text
            var textRect = new RectangleF(textLeft, ctx.ButtonRect.Top, textSize.Width, ctx.ButtonRect.Height);
            g.DrawString(actionText, _actionFont, textBrush, textRect, format);
            
            // Draw arrow
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var arrowPen = new Pen(ctx.AccentColor, 2);
            arrowPen.StartCap = LineCap.Round;
            arrowPen.EndCap = LineCap.Round;
            
            float arrowX = textRect.Right + 8;
            float arrowY = ctx.ButtonRect.Top + ctx.ButtonRect.Height / 2;
            
            g.DrawLine(arrowPen, arrowX, arrowY, arrowX + 8, arrowY);
            g.DrawLine(arrowPen, arrowX + 4, arrowY - 4, arrowX + 8, arrowY);
            g.DrawLine(arrowPen, arrowX + 4, arrowY + 4, arrowX + 8, arrowY);
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Full card is clickable
            if (!ctx.DrawingRect.IsEmpty)
            {
                owner.AddHitArea("Card", ctx.DrawingRect, null,
                    () => notifyAreaHit?.Invoke("Card", ctx.DrawingRect));
            }
            
            // Action link hit area
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
            _descFont?.Dispose();
            _actionFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

