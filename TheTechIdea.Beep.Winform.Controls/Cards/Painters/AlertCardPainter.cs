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
    /// AlertCard - Alert/warning card with icon, message, and action buttons.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class AlertCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Alert card fonts
        private Font _titleFont;
        private Font _metaFont;
        private Font _messageFont;
        private Font _actionFont;
        
        // Alert card spacing
        private const int Padding = 16;
        private const int IconSize = 32;
        private const int AccentBarWidth = 4;
        private const int TitleHeight = 24;
        private const int MessageMinHeight = 32;
        private const int ButtonHeight = 32;
        private const int ButtonWidth = 90;
        private const int ElementGap = 10;
        private const int ContentGap = 14;
        
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
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Accent bar on left edge (color indicates severity)
            ctx.StatusRect = new Rectangle(
                drawingRect.Left,
                drawingRect.Top,
                AccentBarWidth,
                drawingRect.Height);
            
            int contentLeft = drawingRect.Left + AccentBarWidth + Padding;
            int contentWidth = drawingRect.Width - AccentBarWidth - Padding * 2;
            
            // Alert icon
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    contentLeft,
                    drawingRect.Top + Padding,
                    IconSize,
                    IconSize);
                contentLeft = ctx.ImageRect.Right + ContentGap;
                contentWidth -= IconSize + ContentGap;
            }
            
            // Close/dismiss button (top-right)
            ctx.SecondaryButtonRect = new Rectangle(
                drawingRect.Right - Padding - 24,
                drawingRect.Top + Padding,
                24,
                24);
            contentWidth -= 24 + ElementGap;
            
            // Alert title
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                drawingRect.Top + Padding,
                contentWidth,
                TitleHeight);

            int subtitleHeight = !string.IsNullOrEmpty(ctx.SubtitleText) ? 18 : 0;
            if (subtitleHeight > 0)
            {
                ctx.SubtitleRect = new Rectangle(
                    contentLeft,
                    ctx.HeaderRect.Bottom + ElementGap / 2,
                    contentWidth + 24 + ElementGap,
                    subtitleHeight);
            }
            
            // Alert message
            int messageHeight = Math.Max(MessageMinHeight, 
                drawingRect.Height - Padding * 2 - TitleHeight - subtitleHeight - ElementGap - 
                (ctx.ShowButton ? ButtonHeight + ElementGap : 0));
            
            ctx.ParagraphRect = new Rectangle(
                contentLeft,
                (subtitleHeight > 0 ? ctx.SubtitleRect.Bottom : ctx.HeaderRect.Bottom) + ElementGap / 2,
                contentWidth + 24 + ElementGap, // Full width for message
                messageHeight);
            
            // Action buttons at bottom
            if (ctx.ShowButton)
            {
                int buttonTop = drawingRect.Bottom - Padding - ButtonHeight;
                
                // Primary action (right-aligned)
                ctx.ButtonRect = new Rectangle(
                    drawingRect.Right - Padding - ButtonWidth,
                    buttonTop,
                    ButtonWidth,
                    ButtonHeight);
            }
            
            ctx.ShowSecondaryButton = false; // Close button is painter-owned
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Draw subtle tinted background based on alert type
            if (!ctx.DrawingRect.IsEmpty)
            {
                var bgBrush = CardPaintCache.Brush(Color.FromArgb(8, ctx.StatusColor));
                g.FillRectangle(bgBrush, ctx.DrawingRect);
            }
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw accent bar
            if (!ctx.StatusRect.IsEmpty)
            {
                var brush = CardPaintCache.Brush(ctx.StatusColor);
                g.FillRectangle(brush, ctx.StatusRect);
            }

            if (!string.IsNullOrEmpty(ctx.SubtitleText) && !ctx.SubtitleRect.IsEmpty)
            {
                var subtitleColor = Color.FromArgb(180, _theme?.CardTextForeColor ?? _owner?.ForeColor ?? Color.Black);
                TextRenderer.DrawText(g, ctx.SubtitleText, _metaFont, ctx.SubtitleRect, subtitleColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
            }
            
            // Draw alert icon
            DrawAlertIcon(g, ctx);
            
            // Draw close button
            DrawCloseButton(g, ctx);
        }
        
        private void DrawAlertIcon(Graphics g, LayoutContext ctx)
        {
            if (!ctx.ShowImage || ctx.ImageRect.IsEmpty) return;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw icon background circle
            var bgBrush = CardPaintCache.Brush(Color.FromArgb(25, ctx.StatusColor));
            g.FillEllipse(bgBrush, ctx.ImageRect);
            
            // Draw icon based on alert type (using StatusText as type indicator)
            string alertType = ctx.BadgeText1?.ToLower() ?? "info";
            
            int cx = ctx.ImageRect.Left + ctx.ImageRect.Width / 2;
            int cy = ctx.ImageRect.Top + ctx.ImageRect.Height / 2;
            
            using var iconPen = new Pen(ctx.StatusColor, DpiScalingHelper.ScaleValue(2.5f, _owner));
            iconPen.StartCap = LineCap.Round;
            iconPen.EndCap = LineCap.Round;

            if (alertType.Contains("error") || alertType.Contains("danger"))
            {
                // X icon
                int offset = DpiScalingHelper.ScaleValue(7, _owner);
                g.DrawLine(iconPen, cx - offset, cy - offset, cx + offset, cy + offset);
                g.DrawLine(iconPen, cx + offset, cy - offset, cx - offset, cy + offset);
            }
            else if (alertType.Contains("warning"))
            {
                // Triangle with exclamation
                int size = DpiScalingHelper.ScaleValue(12, _owner);
                Point[] triangle = new Point[]
                {
                    new Point(cx, cy - size),
                    new Point(cx - size, cy + size - DpiScalingHelper.ScaleValue(4, _owner)),
                    new Point(cx + size, cy + size - DpiScalingHelper.ScaleValue(4, _owner))
                };
                g.DrawPolygon(iconPen, triangle);
                g.DrawLine(iconPen, cx, cy - DpiScalingHelper.ScaleValue(4, _owner), cx, cy + DpiScalingHelper.ScaleValue(2, _owner));
                var dotBrush = CardPaintCache.Brush(ctx.StatusColor);
                g.FillEllipse(dotBrush, cx - DpiScalingHelper.ScaleValue(2, _owner), cy + DpiScalingHelper.ScaleValue(5, _owner), DpiScalingHelper.ScaleValue(4, _owner), DpiScalingHelper.ScaleValue(4, _owner));
            }
            else if (alertType.Contains("success"))
            {
                // Checkmark
                g.DrawLine(iconPen, cx - DpiScalingHelper.ScaleValue(7, _owner), cy, cx - DpiScalingHelper.ScaleValue(2, _owner), cy + DpiScalingHelper.ScaleValue(5, _owner));
                g.DrawLine(iconPen, cx - DpiScalingHelper.ScaleValue(2, _owner), cy + DpiScalingHelper.ScaleValue(5, _owner), cx + DpiScalingHelper.ScaleValue(8, _owner), cy - DpiScalingHelper.ScaleValue(5, _owner));
            }
            else
            {
                // Info icon (i)
                var dotBrush = CardPaintCache.Brush(ctx.StatusColor);
                g.FillEllipse(dotBrush, cx - DpiScalingHelper.ScaleValue(2, _owner), cy - DpiScalingHelper.ScaleValue(8, _owner), DpiScalingHelper.ScaleValue(5, _owner), DpiScalingHelper.ScaleValue(5, _owner));
                g.DrawLine(iconPen, cx, cy - DpiScalingHelper.ScaleValue(2, _owner), cx, cy + DpiScalingHelper.ScaleValue(8, _owner));
            }
        }
        
        private void DrawCloseButton(Graphics g, LayoutContext ctx)
        {
            if (ctx.SecondaryButtonRect.IsEmpty) return;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw X button
            int margin = DpiScalingHelper.ScaleValue(6, _owner);
            var rect = ctx.SecondaryButtonRect;

            using var pen = new Pen(Color.FromArgb(100, _theme?.CardTextForeColor ?? Color.Black), DpiScalingHelper.ScaleValue(2, _owner));
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
            // Close button hit area
            if (!ctx.SecondaryButtonRect.IsEmpty)
            {
                owner.AddHitArea("Close", ctx.SecondaryButtonRect, null,
                    () => notifyAreaHit?.Invoke("Close", ctx.SecondaryButtonRect));
            }
            
            // Primary action button hit area
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

