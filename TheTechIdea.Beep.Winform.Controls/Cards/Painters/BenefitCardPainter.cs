using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// BenefitCard - Benefit/value proposition with checkmarks.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class BenefitCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Benefit card fonts
        private Font _titleFont;
        private Font _descFont;
        private Font _benefitFont;
        
        // Benefit card spacing
        private const int Padding = 20;
        private const int IconSize = 48;
        private const int TitleHeight = 28;
        private const int DescHeight = 24;
        private const int BenefitRowHeight = 28;
        private const int CheckmarkSize = 20;
        private const int ElementGap = 10;
        private const int BenefitGap = 8;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _titleFont?.Dispose(); } catch { }
            try { _descFont?.Dispose(); } catch { }
            try { _benefitFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, 14f, FontStyle.Bold);
            _descFont = new Font(fontFamily, 10f, FontStyle.Regular);
            _benefitFont = new Font(fontFamily, 10f, FontStyle.Regular);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Icon (top-left or centered)
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + Padding,
                    drawingRect.Top + Padding,
                    IconSize,
                    IconSize);
            }
            
            int contentTop = ctx.ShowImage ? ctx.ImageRect.Bottom + ElementGap : drawingRect.Top + Padding;
            
            // Title
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + Padding,
                contentTop,
                drawingRect.Width - Padding * 2,
                TitleHeight);
            
            // Short description
            ctx.SubtitleRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.HeaderRect.Bottom + ElementGap / 2,
                drawingRect.Width - Padding * 2,
                DescHeight);
            
            // Benefits list area (using ParagraphRect)
            int benefitsHeight = Math.Max(60,
                drawingRect.Height - (ctx.SubtitleRect.Bottom - drawingRect.Top) - Padding - ElementGap);
            
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.SubtitleRect.Bottom + ElementGap,
                drawingRect.Width - Padding * 2,
                benefitsHeight);
            
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
            // Draw icon background
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                DrawIconBackground(g, ctx);
            }
            
            // Draw benefits list with checkmarks
            if (ctx.Tags != null && !ctx.ParagraphRect.IsEmpty)
            {
                DrawBenefitsList(g, ctx);
            }
        }
        
        private void DrawIconBackground(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Rounded square background
            using var bgPath = CardRenderingHelpers.CreateRoundedPath(ctx.ImageRect, 12);
            using var bgBrush = new SolidBrush(Color.FromArgb(20, ctx.AccentColor));
            g.FillPath(bgBrush, bgPath);
        }
        
        private void DrawBenefitsList(Graphics g, LayoutContext ctx)
        {
            int y = ctx.ParagraphRect.Top;
            int maxY = ctx.ParagraphRect.Bottom - BenefitRowHeight;
            
            foreach (var benefit in ctx.Tags)
            {
                if (string.IsNullOrWhiteSpace(benefit) || y > maxY) break;
                
                // Draw checkmark circle
                var checkRect = new Rectangle(
                    ctx.ParagraphRect.Left,
                    y + (BenefitRowHeight - CheckmarkSize) / 2,
                    CheckmarkSize,
                    CheckmarkSize);
                
                DrawCheckmark(g, checkRect, ctx.AccentColor);
                
                // Draw benefit text
                var textRect = new Rectangle(
                    checkRect.Right + ElementGap,
                    y,
                    ctx.ParagraphRect.Width - CheckmarkSize - ElementGap,
                    BenefitRowHeight);
                
                using var textBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(benefit, _benefitFont, textBrush, textRect, format);
                
                y += BenefitRowHeight + BenefitGap;
            }
        }
        
        private void DrawCheckmark(Graphics g, Rectangle rect, Color color)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Circle background
            using var bgBrush = new SolidBrush(Color.FromArgb(76, 175, 80)); // Green
            g.FillEllipse(bgBrush, rect);
            
            // Checkmark
            using var checkPen = new Pen(Color.White, 2.5f);
            checkPen.StartCap = LineCap.Round;
            checkPen.EndCap = LineCap.Round;
            
            int cx = rect.Left + rect.Width / 2;
            int cy = rect.Top + rect.Height / 2;
            
            g.DrawLine(checkPen, cx - 4, cy, cx - 1, cy + 3);
            g.DrawLine(checkPen, cx - 1, cy + 3, cx + 5, cy - 3);
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Icon hit area
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                owner.AddHitArea("Icon", ctx.ImageRect, null,
                    () => notifyAreaHit?.Invoke("Icon", ctx.ImageRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _titleFont?.Dispose();
            _descFont?.Dispose();
            _benefitFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

