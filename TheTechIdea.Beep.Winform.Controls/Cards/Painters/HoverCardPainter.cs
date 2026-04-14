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
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_titleFont = titleFont;
            _descFont = bodyFont;
            _actionFont = bodyFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int iconSize = DpiScalingHelper.ScaleValue(IconSize, _owner);
            int titleHeight = DpiScalingHelper.ScaleValue(TitleHeight, _owner);
            int descMinHeight = DpiScalingHelper.ScaleValue(DescMinHeight, _owner);
            int actionHeight = DpiScalingHelper.ScaleValue(ActionHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            
            // Centered icon at top
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + (drawingRect.Width - iconSize) / 2,
                    drawingRect.Top + padding,
                    iconSize,
                    iconSize);
            }
            
            int contentTop = ctx.ShowImage ? ctx.ImageRect.Bottom + elementGap : drawingRect.Top + padding;
            
            // Title (centered)
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + padding,
                contentTop,
                drawingRect.Width - padding * 2,
                titleHeight);
            
            // Description (centered)
            int descHeight = Math.Max(descMinHeight,
                drawingRect.Height - (ctx.HeaderRect.Bottom - drawingRect.Top) - padding - elementGap - 
                (ctx.ShowButton ? actionHeight + elementGap : 0));
            
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + padding,
                ctx.HeaderRect.Bottom + elementGap / 2,
                drawingRect.Width - padding * 2,
                descHeight);
            
            // Action link (centered at bottom)
            bool showActionLink = ctx.ShowButton;
            if (showActionLink)
            {
                ctx.ButtonRect = new Rectangle(
                    drawingRect.Left + padding,
                    drawingRect.Bottom - padding - actionHeight,
                    drawingRect.Width - padding * 2,
                    actionHeight);
            }
            else
            {
                ctx.ButtonRect = Rectangle.Empty;
            }
            
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
            // Draw icon with hover-ready styling
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                DrawHoverIcon(g, ctx);
            }
            
            // Draw action link with arrow
            if (!ctx.ButtonRect.IsEmpty)
            {
                DrawActionLink(g, ctx);
            }
            
            // Draw subtle top accent line
            using var accentPen = new Pen(ctx.AccentColor, DpiScalingHelper.ScaleValue(3, _owner));
            g.DrawLine(accentPen,
                ctx.DrawingRect.Left + DpiScalingHelper.ScaleValue(Padding, _owner), ctx.DrawingRect.Top,
                ctx.DrawingRect.Right - DpiScalingHelper.ScaleValue(Padding, _owner), ctx.DrawingRect.Top);
        }
        
        private void DrawHoverIcon(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Large circular background with gradient
            var bgRect = new Rectangle(
                ctx.ImageRect.X - DpiScalingHelper.ScaleValue(8, _owner),
                ctx.ImageRect.Y - DpiScalingHelper.ScaleValue(8, _owner),
                ctx.ImageRect.Width + DpiScalingHelper.ScaleValue(16, _owner),
                ctx.ImageRect.Height + DpiScalingHelper.ScaleValue(16, _owner));
            
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
            float textLeft = ctx.ButtonRect.Left + (ctx.ButtonRect.Width - textSize.Width - DpiScalingHelper.ScaleValue(20, _owner)) / 2;
            
            // Draw text
            var textRect = new RectangleF(textLeft, ctx.ButtonRect.Top, textSize.Width, ctx.ButtonRect.Height);
            g.DrawString(actionText, _actionFont, textBrush, textRect, format);
            
            // Draw arrow
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var arrowPen = new Pen(ctx.AccentColor, DpiScalingHelper.ScaleValue(2, _owner));
            arrowPen.StartCap = LineCap.Round;
            arrowPen.EndCap = LineCap.Round;
            
            float arrowX = textRect.Right + DpiScalingHelper.ScaleValue(8, _owner);
            float arrowY = ctx.ButtonRect.Top + ctx.ButtonRect.Height / 2;
            
            g.DrawLine(arrowPen, arrowX, arrowY, arrowX + DpiScalingHelper.ScaleValue(8, _owner), arrowY);
            g.DrawLine(arrowPen, arrowX + DpiScalingHelper.ScaleValue(4, _owner), arrowY - DpiScalingHelper.ScaleValue(4, _owner), arrowX + DpiScalingHelper.ScaleValue(8, _owner), arrowY);
            g.DrawLine(arrowPen, arrowX + DpiScalingHelper.ScaleValue(4, _owner), arrowY + DpiScalingHelper.ScaleValue(4, _owner), arrowX + DpiScalingHelper.ScaleValue(8, _owner), arrowY);
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
            if (!ctx.ButtonRect.IsEmpty)
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

