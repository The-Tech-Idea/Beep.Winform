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
        
        public void Initialize(BaseControl owner, IBeepTheme theme, Font titleFont, Font bodyFont, Font captionFont)
        {
            _owner = owner;
            _theme = theme;
_titleFont = titleFont;
            _descFont = bodyFont;
            _labelFont = captionFont;
            _requiredFont = bodyFont;
        }
        
        public LayoutContext AdjustLayout(Rectangle drawingRect, LayoutContext ctx)
        {
            ctx.DrawingRect = drawingRect;
            int padding = DpiScalingHelper.ScaleValue(Padding, _owner);
            int titleHeight = DpiScalingHelper.ScaleValue(TitleHeight, _owner);
            int descHeight = DpiScalingHelper.ScaleValue(DescHeight, _owner);
            int buttonHeight = DpiScalingHelper.ScaleValue(ButtonHeight, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            
            // Section title
            ctx.HeaderRect = new Rectangle(
                drawingRect.Left + padding,
                drawingRect.Top + padding,
                drawingRect.Width - padding * 2,
                titleHeight);
            
            // Section description
            ctx.SubtitleRect = new Rectangle(
                drawingRect.Left + padding,
                ctx.HeaderRect.Bottom + elementGap / 2,
                drawingRect.Width - padding * 2,
                descHeight);
            
            // Form fields area
            int fieldsTop = ctx.SubtitleRect.Bottom + elementGap;
            int fieldsHeight = Math.Max(60,
                drawingRect.Height - (fieldsTop - drawingRect.Top) - padding - 
                (ctx.ShowButton ? buttonHeight + elementGap : 0));
            
            ctx.ParagraphRect = new Rectangle(
                drawingRect.Left + padding,
                fieldsTop,
                drawingRect.Width - padding * 2,
                fieldsHeight);
            
            // Action buttons
            if (ctx.ShowButton)
            {
                int buttonWidth = (drawingRect.Width - padding * 3) / 2;
                
                if (ctx.ShowSecondaryButton)
                {
                    ctx.SecondaryButtonRect = new Rectangle(
                        drawingRect.Left + padding,
                        drawingRect.Bottom - padding - buttonHeight,
                        buttonWidth,
                        buttonHeight);
                    
                    ctx.ButtonRect = new Rectangle(
                        ctx.SecondaryButtonRect.Right + padding,
                        ctx.SecondaryButtonRect.Top,
                        buttonWidth,
                        buttonHeight);
                }
                else
                {
                    ctx.ButtonRect = new Rectangle(
                        drawingRect.Left + padding,
                        drawingRect.Bottom - padding - buttonHeight,
                        drawingRect.Width - padding * 2,
                        buttonHeight);
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
            int dividerY = ctx.SubtitleRect.Bottom + DpiScalingHelper.ScaleValue(ElementGap, _owner) / 2;
            using var dividerPen = new Pen(Color.FromArgb(30, ctx.AccentColor), DpiScalingHelper.ScaleValue(1, _owner));
            g.DrawLine(dividerPen,
                ctx.DrawingRect.Left + DpiScalingHelper.ScaleValue(Padding, _owner), dividerY,
                ctx.DrawingRect.Right - DpiScalingHelper.ScaleValue(Padding, _owner), dividerY);
        }
        
        private void DrawFormFields(Graphics g, LayoutContext ctx)
        {
            if (ctx.Tags == null) return;
            
            int fieldHeight = DpiScalingHelper.ScaleValue(FieldHeight, _owner);
            int labelHeight = DpiScalingHelper.ScaleValue(LabelHeight, _owner);
            int fieldGap = DpiScalingHelper.ScaleValue(FieldGap, _owner);
            int elementGap = DpiScalingHelper.ScaleValue(ElementGap, _owner);
            int y = ctx.ParagraphRect.Top + elementGap;
            int fieldWidth = ctx.ParagraphRect.Width;
            int maxY = ctx.ParagraphRect.Bottom - fieldHeight - labelHeight;
            
            foreach (var fieldLabel in ctx.Tags)
            {
                if (string.IsNullOrWhiteSpace(fieldLabel) || y > maxY) break;
                
                bool isRequired = fieldLabel.EndsWith("*");
                string label = isRequired ? fieldLabel.TrimEnd('*').Trim() : fieldLabel;
                
                // Draw label
                var labelRect = new Rectangle(ctx.ParagraphRect.Left, y, fieldWidth, labelHeight);
                using var labelBrush = new SolidBrush(Color.FromArgb(140, Color.Black));
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(label, _labelFont, labelBrush, labelRect, format);
                
                // Draw required asterisk
                if (isRequired)
                {
                    var labelSize = g.MeasureString(label, _labelFont);
                    using var reqBrush = new SolidBrush(Color.FromArgb(220, 53, 69));
                    g.DrawString("*", _requiredFont, reqBrush, labelRect.Left + labelSize.Width + DpiScalingHelper.ScaleValue(2, _owner), y);
                }
                
                // Draw field placeholder
                var fieldRect = new Rectangle(ctx.ParagraphRect.Left, y + labelHeight, fieldWidth, fieldHeight);
                DrawFieldPlaceholder(g, fieldRect, ctx.AccentColor);
                
                y += labelHeight + fieldHeight + fieldGap;
            }
        }
        
        private void DrawFieldPlaceholder(Graphics g, Rectangle rect, Color accentColor)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Field background
            using var bgPath = CardRenderingHelpers.CreateRoundedPath(rect, DpiScalingHelper.ScaleValue(6, _owner));
            using var bgBrush = new SolidBrush(Color.FromArgb(250, 250, 250));
            g.FillPath(bgBrush, bgPath);
            
            // Field border
            using var borderPen = new Pen(Color.FromArgb(200, 200, 200), DpiScalingHelper.ScaleValue(1, _owner));
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
_disposed = true;
        }
        
        #endregion
    }
}

