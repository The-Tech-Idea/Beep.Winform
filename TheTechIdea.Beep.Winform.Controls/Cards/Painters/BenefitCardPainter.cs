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
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_titleFont = titleFont;
            _descFont = bodyFont;
            _benefitFont = bodyFont;
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
            using var bgPath = CardRenderingHelpers.CreateRoundedPath(ctx.ImageRect, DpiScalingHelper.ScaleValue(12, _owner));
            var bgBrush = CardPaintCache.Brush(Color.FromArgb(20, ctx.AccentColor));
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

                TextRenderer.DrawText(g, benefit, _benefitFont, textRect, Color.FromArgb(180, _theme?.CardTextForeColor ?? Color.Black),
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
                
                y += BenefitRowHeight + BenefitGap;
            }
        }
        
        private void DrawCheckmark(Graphics g, Rectangle rect, Color color)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Circle background
            var bgBrush = CardPaintCache.Brush(_theme?.SuccessColor ?? Color.FromArgb(76, 175, 80)); // Green
            g.FillEllipse(bgBrush, rect);

            // Checkmark
            using var checkPen = new Pen(Color.White, DpiScalingHelper.ScaleValue(2.5f, _owner));
            checkPen.StartCap = LineCap.Round;
            checkPen.EndCap = LineCap.Round;

            int cx = rect.Left + rect.Width / 2;
            int cy = rect.Top + rect.Height / 2;

            g.DrawLine(checkPen, cx - DpiScalingHelper.ScaleValue(4, _owner), cy, cx - DpiScalingHelper.ScaleValue(1, _owner), cy + DpiScalingHelper.ScaleValue(3, _owner));
            g.DrawLine(checkPen, cx - DpiScalingHelper.ScaleValue(1, _owner), cy + DpiScalingHelper.ScaleValue(3, _owner), cx + DpiScalingHelper.ScaleValue(5, _owner), cy - DpiScalingHelper.ScaleValue(3, _owner));
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
_disposed = true;
        }
        
        #endregion
    }
}

