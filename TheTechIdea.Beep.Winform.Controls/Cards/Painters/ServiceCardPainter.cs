using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// ServiceCard - Service/feature display with centered icon, title, and CTA button.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class ServiceCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Service card fonts
        private Font _titleFont;
        private Font _metaFont;
        private Font _descFont;
        private Font _badgeFont;
        
        // Service card spacing
        private const int Padding = 20;
        private const int IconSize = 60;
        private const int IconCirclePadding = 12;
        private const int TitleHeight = 28;
        private const int BadgeWidth = 100;
        private const int BadgeHeight = 22;
        private const int DescMinHeight = 50;
        private const int ButtonHeight = 38;
        private const int ButtonMaxWidth = 160;
        private const int AccentLineWidth = 50;
        private const int AccentLineThickness = 3;
        private const int ElementGap = 12;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_titleFont = titleFont;
            _metaFont = captionFont;
            _descFont = bodyFont;
            _badgeFont = captionFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int iconSize = DpiScalingHelper.ScaleValue(IconSize, _owner);
            int iconCirclePadding = DpiScalingHelper.ScaleValue(IconCirclePadding, _owner);
            int titleHeight = DpiScalingHelper.ScaleValue(TitleHeight, _owner);
            int badgeWidth = DpiScalingHelper.ScaleValue(BadgeWidth, _owner);
            int badgeHeight = DpiScalingHelper.ScaleValue(BadgeHeight, _owner);
            int descMinHeight = DpiScalingHelper.ScaleValue(DescMinHeight, _owner);
            int buttonHeight = DpiScalingHelper.ScaleValue(ButtonHeight, _owner);
            int buttonMaxWidth = DpiScalingHelper.ScaleValue(ButtonMaxWidth, _owner);
            int accentLineWidth = DpiScalingHelper.ScaleValue(AccentLineWidth, _owner);
            int accentLineThickness = DpiScalingHelper.ScaleValue(AccentLineThickness, _owner);
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
            
            // Service title (centered below icon)
            int titleTop = ctx.ShowImage ? ctx.ImageRect.Bottom + elementGap * 2 : drawingRect.Top + padding;
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + padding,
                titleTop,
                drawingRect.Width - padding * 2,
                titleHeight);

            int subtitleHeight = !string.IsNullOrEmpty(ctx.SubtitleText)
                ? DpiScalingHelper.ScaleValue(18, _owner)
                : 0;

            if (subtitleHeight > 0)
            {
                ctx.SubtitleRect = new Rectangle(
                    drawingRect.Left + padding,
                    ctx.HeaderRect.Bottom + elementGap / 2,
                    drawingRect.Width - padding * 2,
                    subtitleHeight);
            }
            
            // Category badge (centered below title)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Left + (drawingRect.Width - badgeWidth) / 2,
                    (subtitleHeight > 0 ? ctx.SubtitleRect.Bottom : ctx.HeaderRect.Bottom) + elementGap,
                    badgeWidth,
                    badgeHeight);
            }
            
            // Service description
            int descTop = string.IsNullOrEmpty(ctx.BadgeText1)
                ? (subtitleHeight > 0 ? ctx.SubtitleRect.Bottom : ctx.HeaderRect.Bottom) + elementGap * 2
                : ctx.BadgeRect.Bottom + elementGap * 2;
            int descHeight = Math.Max(descMinHeight,
                drawingRect.Height - (descTop - drawingRect.Top) - padding * 2 - 
                (ctx.ShowButton ? buttonHeight + elementGap + accentLineThickness + elementGap : 0));
            
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + padding,
                descTop,
                drawingRect.Width - padding * 2,
                descHeight);
            
            // CTA button (centered at bottom)
            if (ctx.ShowButton)
            {
                int buttonWidth = Math.Min(drawingRect.Width - padding * 2, buttonMaxWidth);
                ctx.ButtonRect = new Rectangle(
                    drawingRect.Left + (drawingRect.Width - buttonWidth) / 2,
                    drawingRect.Bottom - padding - buttonHeight,
                    buttonWidth,
                    buttonHeight);
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
            // Draw icon background circle with accent color
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                
                int iconSize = DpiScalingHelper.ScaleValue(IconSize, _owner);
                int iconCirclePadding = DpiScalingHelper.ScaleValue(IconCirclePadding, _owner);
                int circleSize = iconSize + iconCirclePadding * 2;
                var circleRect = new Rectangle(
                    ctx.ImageRect.Left - iconCirclePadding,
                    ctx.ImageRect.Top - iconCirclePadding,
                    circleSize,
                    circleSize);
                
                // Fill circle
                using var fillBrush = new SolidBrush(Color.FromArgb(20, ctx.AccentColor));
                g.FillEllipse(fillBrush, circleRect);
                
                // Border circle
                using var borderPen = new Pen(Color.FromArgb(40, ctx.AccentColor), DpiScalingHelper.ScaleValue(2, _owner));
                g.DrawEllipse(borderPen, circleRect);
            }

            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.SubtitleRect.IsEmpty)
            {
                var subtitleColor = Color.FromArgb(180, _theme?.CardTextForeColor ?? _owner?.ForeColor ?? Color.Black);
                using var subtitleBrush = new SolidBrush(subtitleColor);
                var subtitleFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.SubtitleText, _metaFont, subtitleBrush, ctx.SubtitleRect, subtitleFormat);
            }
            
            // Draw category badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw decorative accent line above button
            if (ctx.ShowButton && !ctx.ButtonRect.IsEmpty)
            {
                int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
                int accentLineWidth = DpiScalingHelper.ScaleValue(AccentLineWidth, _owner);
                int accentLineThickness = DpiScalingHelper.ScaleValue(AccentLineThickness, _owner);
                int lineY = ctx.ButtonRect.Top - elementGap;
                int lineX = ctx.DrawingRect.Left + (ctx.DrawingRect.Width - accentLineWidth) / 2;
                
                using var pen = new Pen(ctx.AccentColor, accentLineThickness);
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                
                g.DrawLine(pen, lineX, lineY, lineX + accentLineWidth, lineY);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (!ctx.BadgeRect.IsEmpty)
            {
                owner.AddHitArea("Category", ctx.BadgeRect, null,
                    () => notifyAreaHit?.Invoke("Category", ctx.BadgeRect));
            }
            
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

