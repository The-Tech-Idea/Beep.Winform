using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// SettingsCard - Settings option card with toggle/switch.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class SettingsCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Settings card fonts
        private Font _titleFont;
        private Font _descFont;
        private Font _statusFont;
        
        // Settings card spacing
        private const int Padding = 16;
        private const int IconSize = 40;
        private const int TitleHeight = 22;
        private const int DescHeight = 36;
        private const int ToggleWidth = 48;
        private const int ToggleHeight = 26;
        private const int ElementGap = 6;
        private const int ContentGap = 14;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _titleFont?.Dispose(); } catch { }
            try { _descFont?.Dispose(); } catch { }
            try { _statusFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, 11f, FontStyle.Bold);
            _descFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _statusFont = new Font(fontFamily, 8f, FontStyle.Regular);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Setting icon (left)
            if (ctx.ShowImage)
            {
                ctx.ImageRect = new Rectangle(
                    drawingRect.Left + Padding,
                    drawingRect.Top + (drawingRect.Height - IconSize) / 2,
                    IconSize,
                    IconSize);
            }
            
            int contentLeft = drawingRect.Left + Padding + (ctx.ShowImage ? IconSize + ContentGap : 0);
            int contentWidth = drawingRect.Width - (contentLeft - drawingRect.Left) - Padding - ToggleWidth - ContentGap;
            
            // Toggle switch (right)
            ctx.ButtonRect = new Rectangle(
                drawingRect.Right - Padding - ToggleWidth,
                drawingRect.Top + (drawingRect.Height - ToggleHeight) / 2,
                ToggleWidth,
                ToggleHeight);
            
            // Setting title
            ctx.HeaderRect = new Rectangle(
                contentLeft,
                drawingRect.Top + Padding,
                contentWidth,
                TitleHeight);
            
            // Setting description
            ctx.ParagraphRect = new Rectangle(
                contentLeft,
                ctx.HeaderRect.Bottom + ElementGap / 2,
                contentWidth,
                DescHeight);
            
            // Status text (below toggle)
            if (!string.IsNullOrEmpty(ctx.StatusText))
            {
                ctx.StatusRect = new Rectangle(
                    ctx.ButtonRect.Left,
                    ctx.ButtonRect.Bottom + 4,
                    ToggleWidth,
                    16);
            }
            
            ctx.ShowButton = true;
            ctx.ShowSecondaryButton = false;
            return ctx;
        }
        
        public void DrawBackground(Graphics g, LayoutContext ctx)
        {
            // Background handled by BaseControl
        }
        
        public void DrawForegroundAccents(Graphics g, LayoutContext ctx)
        {
            // Draw setting icon
            if (ctx.ShowImage && !ctx.ImageRect.IsEmpty)
            {
                DrawSettingIcon(g, ctx);
            }
            
            // Draw toggle switch
            if (!ctx.ButtonRect.IsEmpty)
            {
                DrawToggleSwitch(g, ctx);
            }
            
            // Draw status text
            if (!string.IsNullOrEmpty(ctx.StatusText) && !ctx.StatusRect.IsEmpty)
            {
                using var brush = new SolidBrush(Color.FromArgb(100, Color.Black));
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.StatusText, _statusFont, brush, ctx.StatusRect, format);
            }
        }
        
        private void DrawSettingIcon(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Rounded square background
            using var bgPath = CardRenderingHelpers.CreateRoundedPath(ctx.ImageRect, 10);
            using var bgBrush = new SolidBrush(Color.FromArgb(20, ctx.AccentColor));
            g.FillPath(bgBrush, bgPath);
        }
        
        private void DrawToggleSwitch(Graphics g, LayoutContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Use ShowRating as toggle state
            bool isOn = ctx.ShowRating;
            
            var rect = ctx.ButtonRect;
            int trackRadius = rect.Height / 2;
            int thumbSize = rect.Height - 4;
            
            // Track background
            Color trackColor = isOn ? ctx.AccentColor : Color.FromArgb(180, 180, 180);
            using var trackPath = CardRenderingHelpers.CreateRoundedPath(rect, trackRadius);
            using var trackBrush = new SolidBrush(trackColor);
            g.FillPath(trackBrush, trackPath);
            
            // Thumb
            int thumbX = isOn ? rect.Right - thumbSize - 2 : rect.Left + 2;
            var thumbRect = new Rectangle(thumbX, rect.Top + 2, thumbSize, thumbSize);
            
            using var thumbBrush = new SolidBrush(Color.White);
            g.FillEllipse(thumbBrush, thumbRect);
            
            // Thumb shadow
            using var shadowPen = new Pen(Color.FromArgb(30, Color.Black), 1);
            g.DrawEllipse(shadowPen, thumbRect);
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Toggle switch hit area
            if (!ctx.ButtonRect.IsEmpty)
            {
                owner.AddHitArea("Toggle", ctx.ButtonRect, null,
                    () => notifyAreaHit?.Invoke("Toggle", ctx.ButtonRect));
            }
            
            // Full card is also clickable to toggle
            if (!ctx.DrawingRect.IsEmpty)
            {
                owner.AddHitArea("Setting", ctx.DrawingRect, null,
                    () => notifyAreaHit?.Invoke("Setting", ctx.DrawingRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _titleFont?.Dispose();
            _descFont?.Dispose();
            _statusFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

