using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// ActivityCard - Activity feed item with timestamp and icon.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class ActivityCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Activity card fonts
        private Font _titleFont;
        private Font _descFont;
        private Font _timeFont;
        
        // Activity card spacing
        private const int Padding = 12;
        private const int IconSize = 36;
        private const int TimelineWidth = 2;
        private const int TitleHeight = 20;
        private const int DescHeight = 18;
        private const int TimeHeight = 16;
        private const int ElementGap = 4;
        private const int ContentGap = 12;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _titleFont?.Dispose(); } catch { }
            try { _descFont?.Dispose(); } catch { }
            try { _timeFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, 10f, FontStyle.Bold);
            _descFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _timeFont = new Font(fontFamily, 8f, FontStyle.Regular);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Timeline line (left edge)
            if (ctx.ShowStatus)
            {
                ctx.StatusRect = new Rectangle(
                    drawingRect.Left + Padding + IconSize / 2 - TimelineWidth / 2,
                    drawingRect.Top,
                    TimelineWidth,
                    drawingRect.Height);
            }
            
            // Activity icon
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + Padding,
                    drawingRect.Top + Padding,
                    IconSize,
                    IconSize);
            }
            
            int contentLeft = drawingRect.Left + Padding + (ctx.ShowImage ? IconSize + ContentGap : 0);
            int contentWidth = drawingRect.Width - (contentLeft - drawingRect.Left) - Padding;
            
            // Timestamp (top-right)
            ctx.SubtitleRect = new Rectangle(
                drawingRect.Right - Padding - 80,
                drawingRect.Top + Padding,
                80,
                TimeHeight);
            
            // Activity title
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                drawingRect.Top + Padding,
                contentWidth - 90,
                TitleHeight);
            
            // Activity description
            ctx.ParagraphRect = new Rectangle(
                contentLeft,
                ctx.HeaderRect.Bottom + ElementGap,
                contentWidth,
                Math.Max(DescHeight, drawingRect.Height - (ctx.HeaderRect.Bottom - drawingRect.Top) - Padding - ElementGap));
            
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
            // Draw timeline line
            if (ctx.ShowStatus && !ctx.StatusRect.IsEmpty)
            {
                using var lineBrush = new SolidBrush(Color.FromArgb(40, ctx.AccentColor));
                g.FillRectangle(lineBrush, ctx.StatusRect);
            }
            
            // Draw activity icon
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                DrawActivityIcon(g, ctx);
            }
            
            // Draw timestamp
            if (!string.IsNullOrEmpty(ctx.StatusText) && !ctx.SubtitleRect.IsEmpty)
            {
                using var brush = new SolidBrush(Color.FromArgb(100, Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, _timeFont, brush, ctx.SubtitleRect, format);
            }
        }
        
        private void DrawActivityIcon(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Determine icon color based on activity type (use BadgeText1)
            Color iconColor = ctx.StatusColor != Color.Empty ? ctx.StatusColor : ctx.AccentColor;
            
            // Draw circular background
            using var bgBrush = new SolidBrush(Color.FromArgb(20, iconColor));
            g.FillEllipse(bgBrush, ctx.ImageRect);
            
            // Draw border
            using var borderPen = new Pen(Color.FromArgb(60, iconColor), 2);
            g.DrawEllipse(borderPen, ctx.ImageRect);
            
            // Draw icon based on type
            string activityType = ctx.BadgeText1?.ToLower() ?? "";
            
            int cx = ctx.ImageRect.Left + ctx.ImageRect.Width / 2;
            int cy = ctx.ImageRect.Top + ctx.ImageRect.Height / 2;
            
            using var iconPen = new Pen(iconColor, 2);
            iconPen.StartCap = LineCap.Round;
            iconPen.EndCap = LineCap.Round;
            
            if (activityType.Contains("add") || activityType.Contains("create") || activityType.Contains("new"))
            {
                // Plus icon
                g.DrawLine(iconPen, cx - 6, cy, cx + 6, cy);
                g.DrawLine(iconPen, cx, cy - 6, cx, cy + 6);
            }
            else if (activityType.Contains("edit") || activityType.Contains("update") || activityType.Contains("change"))
            {
                // Pencil icon
                g.DrawLine(iconPen, cx - 5, cy + 5, cx + 5, cy - 5);
                g.DrawLine(iconPen, cx + 3, cy - 7, cx + 7, cy - 3);
            }
            else if (activityType.Contains("delete") || activityType.Contains("remove"))
            {
                // X icon
                g.DrawLine(iconPen, cx - 5, cy - 5, cx + 5, cy + 5);
                g.DrawLine(iconPen, cx + 5, cy - 5, cx - 5, cy + 5);
            }
            else if (activityType.Contains("comment") || activityType.Contains("message"))
            {
                // Speech bubble
                g.DrawEllipse(iconPen, cx - 6, cy - 5, 12, 10);
                g.DrawLine(iconPen, cx - 2, cy + 5, cx - 4, cy + 8);
            }
            else
            {
                // Default: circle/dot
                using var dotBrush = new SolidBrush(iconColor);
                g.FillEllipse(dotBrush, cx - 4, cy - 4, 8, 8);
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Full card is clickable
            if (!ctx.DrawingRect.IsEmpty)
            {
                owner.AddHitArea("Activity", ctx.DrawingRect, null,
                    () => notifyAreaHit?.Invoke("Activity", ctx.DrawingRect));
            }
            
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
            _timeFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

