using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Cards.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Painters
{
    /// <summary>
    /// FormCard - Form section card with grouped inputs.
    /// Distinct painter with its own layout, spacing, and rendering logic.
    /// </summary>
    internal sealed class FormCardPainter : ICardPainter, IDisposable
    {
        #region Fields
        
        private BaseControl _owner;
        private IBeepTheme _theme;
        private bool _disposed;
        
        // Form card fonts
        private Font _titleFont;
        private Font _descFont;
        private Font _labelFont;
        private Font _requiredFont;
        
        // Form card spacing
        private const int Padding = 16;
        private const int TitleHeight = 28;
        private const int DescHeight = 20;
        private const int FieldHeight = 36;
        private const int LabelHeight = 18;
        private const int ButtonHeight = 40;
        private const int ElementGap = 8;
        private const int FieldGap = 16;
        
        #endregion
        
        #region ICardPainter Implementation
        
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            
            var fontFamily = owner?.Font?.FontFamily ?? FontFamily.GenericSansSerif;
            
            try { _titleFont?.Dispose(); } catch { }
            try { _descFont?.Dispose(); } catch { }
            try { _labelFont?.Dispose(); } catch { }
            try { _requiredFont?.Dispose(); } catch { }
            
            _titleFont = new Font(fontFamily, 13f, FontStyle.Bold);
            _descFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _labelFont = new Font(fontFamily, 9f, FontStyle.Regular);
            _requiredFont = new Font(fontFamily, 9f, FontStyle.Bold);
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            
            // Section title
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + Padding,
                drawingRect.Top + Padding,
                drawingRect.Width - Padding * 2,
                TitleHeight);
            
            // Section description
            ctx.SubtitleRect = new Rectangle(
                drawingRect.Left + Padding,
                ctx.HeaderRect.Bottom + ElementGap / 2,
                drawingRect.Width - Padding * 2,
                DescHeight);
            
            // Form fields area
            int fieldsTop = ctx.SubtitleRect.Bottom + ElementGap;
            int fieldsHeight = Math.Max(60,
                drawingRect.Height - (fieldsTop - drawingRect.Top) - Padding - 
                (ctx.ShowButton ? ButtonHeight + ElementGap : 0));
            
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + Padding,
                fieldsTop,
                drawingRect.Width - Padding * 2,
                fieldsHeight);
            
            // Action buttons
            if (ctx.ShowButton)
            {
                int buttonWidth = (drawingRect.Width - Padding * 3) / 2;
                
                if (ctx.ShowSecondaryButton)
                {
                    ctx.SecondaryButtonRect = new Rectangle(
                        drawingRect.Left + Padding,
                        drawingRect.Bottom - Padding - ButtonHeight,
                        buttonWidth,
                        ButtonHeight);
                    
                    ctx.ButtonRect = new Rectangle(
                        ctx.SecondaryButtonRect.Right + Padding,
                        ctx.SecondaryButtonRect.Top,
                        buttonWidth,
                        ButtonHeight);
                }
                else
                {
                    ctx.ButtonRect = new Rectangle(
                        drawingRect.Left + Padding,
                        drawingRect.Bottom - Padding - ButtonHeight,
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
            // Draw form field placeholders
            if (!ctx.ParagraphRect.IsEmpty)
            {
                DrawFormFields(g, ctx);
            }
            
            // Draw section divider
            int dividerY = ctx.SubtitleRect.Bottom + ElementGap / 2;
            using var dividerPen = new Pen(Color.FromArgb(30, ctx.AccentColor), 1);
            g.DrawLine(dividerPen,
                ctx.DrawingRect.Left + Padding, dividerY,
                ctx.DrawingRect.Right - Padding, dividerY);
        }
        
        private void DrawFormFields(Graphics g, LayoutContext ctx)
        {
            if (ctx.Tags == null) return;
            
            int y = ctx.ParagraphRect.Top + ElementGap;
            int fieldWidth = ctx.ParagraphRect.Width;
            int maxY = ctx.ParagraphRect.Bottom - FieldHeight - LabelHeight;
            
            foreach (var fieldLabel in ctx.Tags)
            {
                if (string.IsNullOrWhiteSpace(fieldLabel) || y > maxY) break;
                
                bool isRequired = fieldLabel.EndsWith("*");
                string label = isRequired ? fieldLabel.TrimEnd('*').Trim() : fieldLabel;
                
                // Draw label
                var labelRect = new Rectangle(ctx.ParagraphRect.Left, y, fieldWidth, LabelHeight);
                using var labelBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(label, _labelFont, labelBrush, labelRect, format);
                
                // Draw required asterisk
                if (isRequired)
                {
                    var labelSize = g.MeasureString(label, _labelFont);
                    using var reqBrush = new SolidBrush(Color.FromArgb(220, 53, 69));
                    g.DrawString("*", _requiredFont, reqBrush, labelRect.Left + labelSize.Width + 2, y);
                }
                
                // Draw field placeholder
                var fieldRect = new Rectangle(ctx.ParagraphRect.Left, y + LabelHeight, fieldWidth, FieldHeight);
                DrawFieldPlaceholder(g, fieldRect, ctx.AccentColor);
                
                y += LabelHeight + FieldHeight + FieldGap;
            }
        }
        
        private void DrawFieldPlaceholder(Graphics g, Rectangle rect, Color accentColor)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Field background
            using var bgPath = CardRenderingHelpers.CreateRoundedPath(rect, 6);
            using var bgBrush = new SolidBrush(Color.FromArgb(250, 250, 250));
            g.FillPath(bgBrush, bgPath);
            
            // Field border
            using var borderPen = new Pen(Color.FromArgb(200, 200, 200), 1);
            g.DrawPath(borderPen, bgPath);
        }
        
        public void UpdateHitAreas(BaseControl owner, LayoutContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            // Form fields area
            if (!ctx.ParagraphRect.IsEmpty)
            {
                owner.AddHitArea("Fields", ctx.ParagraphRect, null,
                    () => notifyAreaHit?.Invoke("Fields", ctx.ParagraphRect));
            }
        }
        
        #endregion
        
        #region IDisposable
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _titleFont?.Dispose();
            _descFont?.Dispose();
            _labelFont?.Dispose();
            _requiredFont?.Dispose();
            
            _disposed = true;
        }
        
        #endregion
    }
}

