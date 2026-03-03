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
    /// IconCard - Centered icon with text below (landing page style).
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class IconCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Icon card fonts
        private Font _titleFont;
        private Font _descFont;
        
        // Icon card spacing
        private const int Padding = 24;
        private const int IconSize = 72;
        private const int IconBackgroundPadding = 16;
        private const int TitleHeight = 28;
        private const int DescMinHeight = 48;
        private const int ElementGap = 12;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_titleFont = titleFont;
            _descFont = bodyFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int iconSize = DpiScalingHelper.ScaleValue(IconSize, _owner);
            int titleHeight = DpiScalingHelper.ScaleValue(TitleHeight, _owner);
            int descMinHeight = DpiScalingHelper.ScaleValue(DescMinHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            
            // Large centered icon at top
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + (drawingRect.Width - iconSize) / 2,
                    drawingRect.Top + padding,
                    iconSize,
                    iconSize);
            }
            
            int contentTop = ctx.ShowImage ? ctx.ImageRect.Bottom + elementGap * 2 : drawingRect.Top + padding;
            
            // Title (centered)
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + padding,
                contentTop,
                drawingRect.Width - padding * 2,
                titleHeight);
            
            // Description (centered)
            int descHeight = Math.Max(descMinHeight,
                drawingRect.Height - (ctx.HeaderRect.Bottom - drawingRect.Top) - padding - elementGap);
            
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + padding,
                ctx.HeaderRect.Bottom + elementGap,
                drawingRect.Width - padding * 2,
                descHeight);
            
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
            // Draw icon background circle
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                DrawIconBackground(g, ctx);
            }
        }
        
        private void DrawIconBackground(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Large circular background
            var bgRect = new Rectangle(
                ctx.ImageRect.X - DpiScalingHelper.ScaleValue(IconBackgroundPadding, _owner),
                ctx.ImageRect.Y - DpiScalingHelper.ScaleValue(IconBackgroundPadding, _owner),
                ctx.ImageRect.Width + DpiScalingHelper.ScaleValue(IconBackgroundPadding, _owner) * 2,
                ctx.ImageRect.Height + DpiScalingHelper.ScaleValue(IconBackgroundPadding, _owner) * 2);
            
            // Gradient fill
            using var gradientBrush = new LinearGradientBrush(
                new Point(bgRect.Left, bgRect.Top),
                new Point(bgRect.Right, bgRect.Bottom),
                Color.FromArgb(25, ctx.AccentColor),
                Color.FromArgb(10, ctx.AccentColor));
            
            g.FillEllipse(gradientBrush, bgRect);
            
            // Subtle border
            using var borderPen = new Pen(Color.FromArgb(40, ctx.AccentColor), DpiScalingHelper.ScaleValue(2, _owner));
            g.DrawEllipse(borderPen, bgRect);
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Icon hit area
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                var bgRect = new Rectangle(
                    ctx.ImageRect.X - DpiScalingHelper.ScaleValue(IconBackgroundPadding, _owner),
                    ctx.ImageRect.Y - DpiScalingHelper.ScaleValue(IconBackgroundPadding, _owner),
                    ctx.ImageRect.Width + DpiScalingHelper.ScaleValue(IconBackgroundPadding, _owner) * 2,
                    ctx.ImageRect.Height + DpiScalingHelper.ScaleValue(IconBackgroundPadding, _owner) * 2);
                
                owner.AddHitArea("Icon", bgRect, null,
                    () => notifyAreaHit?.Invoke("Icon", bgRect));
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

