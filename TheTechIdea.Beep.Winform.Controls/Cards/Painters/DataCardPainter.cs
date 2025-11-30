using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// DataCard - Structured data display with grid layout, labels, and values.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class DataCardPainter : ICardPainter
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Data card fonts
        private Font _titleFont;
        private Font _labelFont;
        private Font _valueFont;
        private Font _badgeFont;
        
        // Data card spacing
        private const int Padding = 16;
        private const int IconSize = 36;
        private const int TitleHeight = 26;
        private const int BadgeWidth = 80;
        private const int BadgeHeight = 20;
        private const int DataMinHeight = 60;
        private const int RowHeight = 28;
        private const int ButtonHeight = 38;
        private const int DividerThickness = 1;
        private const int ElementGap = 10;
        private const int ContentGap = 12;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _titleFont?.Dispose(); } catch { }
            try { _labelFont?.Dispose(); } catch { }
            try { _valueFont?.Dispose(); } catch { }
            try { _badgeFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, 12f, FontStyle.Bold);
            _labelFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _valueFont = new Font(fontFamily, 10f, FontStyle.Regular);
            _badgeFont = new Font(fontFamily, 8f, FontStyle.Bold);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Optional icon (for settings/form type)
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + Padding,
                    drawingRect.Top + Padding,
                    IconSize,
                    IconSize);
            }
            
            // Title/section header
            int titleLeft = drawingRect.Left + Padding + (ctx.ShowImage ? IconSize + ContentGap : 0);
            int titleWidth = drawingRect.Width - Padding * 2 - (ctx.ShowImage ? IconSize + ContentGap : 0);
            
            ctx.HeaderRect = new Rectangle(
                titleLeft,
                drawingRect.Top + Padding,
                titleWidth - BadgeWidth - ElementGap,
                TitleHeight);
            
            // Status/category badge (top-right)
            if (!string.IsNullOrEmpty(ctx.BadgeText1))
            {
                ctx.BadgeRect = new Rectangle(
                    drawingRect.Right - Padding - BadgeWidth,
                    drawingRect.Top + Padding,
                    BadgeWidth,
                    BadgeHeight);
            }
            
            // Data section (labels and values in two columns)
            int dataTop = Math.Max(ctx.HeaderRect.Bottom, ctx.ShowImage ? ctx.ImageRect.Bottom : 0) + ElementGap * 2;
            int dataHeight = Math.Max(DataMinHeight,
                drawingRect.Height - (dataTop - drawingRect.Top) - Padding * 2 - 
                (ctx.ShowButton ? ButtonHeight + ElementGap : 0));
            
            int labelWidth = (drawingRect.Width - Padding * 3) / 3;
            int valueWidth = drawingRect.Width - Padding * 2 - labelWidth - Padding;
            
            // Labels column (left)
            ctx.SubtitleRect = new Rectangle(
                drawingRect.Left + Padding,
                dataTop,
                labelWidth,
                dataHeight);
            
            // Values column (right)
            ctx.ParagraphRect = new Rectangle(
                ctx.SubtitleRect.Right + Padding,
                dataTop,
                valueWidth,
                dataHeight);
            
            // Action buttons
            if (ctx.ShowButton)
            {
                int buttonTop = drawingRect.Bottom - Padding - ButtonHeight;
                
                if (ctx.ShowSecondaryButton)
                {
                    int buttonWidth = (drawingRect.Width - Padding * 3) / 2;
                    ctx.ButtonRect = new Rectangle(
                        drawingRect.Left + Padding,
                        buttonTop,
                        buttonWidth,
                        ButtonHeight);
                    
                    ctx.SecondaryButtonRect = new Rectangle(
                        ctx.ButtonRect.Right + Padding,
                        buttonTop,
                        buttonWidth,
                        ButtonHeight);
                }
                else
                {
                    ctx.ButtonRect = new Rectangle(
                        drawingRect.Left + Padding,
                        buttonTop,
                        drawingRect.Width - Padding * 2,
                        ButtonHeight);
                }
            }
            
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw status/category badge
            if (!string.IsNullOrEmpty(ctx.BadgeText1) && !ctx.BadgeRect.IsEmpty)
            {
                CardRenderingHelpers.DrawBadge(g, ctx.BadgeRect, ctx.BadgeText1,
                    ctx.Badge1BackColor, ctx.Badge1ForeColor, _badgeFont);
            }
            
            // Draw divider line between header and data
            int dividerY = ctx.HeaderRect.Bottom + ElementGap;
            using var dividerPen = new Pen(Color.FromArgb(30, ctx.AccentColor), DividerThickness);
            g.DrawLine(dividerPen, 
                ctx.DrawingRect.Left + Padding, dividerY, 
                ctx.DrawingRect.Right - Padding, dividerY);
            
            // Draw vertical separator between labels and values
            if (ctx.SubtitleRect.Width > 0 && ctx.ParagraphRect.Width > 0)
            {
                int separatorX = ctx.SubtitleRect.Right + Padding / 2;
                using var separatorPen = new Pen(Color.FromArgb(20, ctx.AccentColor), DividerThickness);
                g.DrawLine(separatorPen, separatorX, ctx.SubtitleRect.Top, separatorX, ctx.SubtitleRect.Bottom);
            }
            
            // Draw horizontal grid lines for structured data appearance
            if (ctx.SubtitleRect.Height > 80)
            {
                int numRows = Math.Min(4, ctx.SubtitleRect.Height / RowHeight);
                using var gridPen = new Pen(Color.FromArgb(12, ctx.AccentColor), DividerThickness);
                
                for (int i = 1; i < numRows; i++)
                {
                    int y = ctx.SubtitleRect.Top + (i * RowHeight);
                    g.DrawLine(gridPen, 
                        ctx.DrawingRect.Left + Padding, y, 
                        ctx.DrawingRect.Right - Padding, y);
                }
            }
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (!ctx.BadgeRect.IsEmpty)
            {
                owner.AddHitArea("Badge", ctx.BadgeRect, null,
                    () => notifyAreaHit?.Invoke("Badge", ctx.BadgeRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _titleFont?.Dispose();
            _labelFont?.Dispose();
            _valueFont?.Dispose();
            _badgeFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}
