using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Form
{
    /// <summary>
    /// InlineForm - Horizontal form layout painter
    /// Updated from placeholder: now uses BaseControl.DrawingRect and adds simple field hit areas
    /// </summary>
    internal sealed class InlineFormPainter : WidgetPainterBase
    {
        private Rectangle[] _fieldRects = Array.Empty<Rectangle>();
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);

            // Create inline fields layout from ctx.CustomData["Fields"] (array of labels)
            int pad = 8;
            int fieldHeight = 28;
            int labelWidth = 90;
            int spacing = 8;

            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad,
                                             ctx.DrawingRect.Width - pad * 2, fieldHeight);

            // Determine number of fields
            string[] fields = ctx.CustomData.ContainsKey("Fields") ? (string[])ctx.CustomData["Fields"] : new[] { "First Name", "Last Name", "Email" };
            int count = fields.Length;
            int perFieldWidth = Math.Max(60, (ctx.ContentRect.Width - (count - 1) * spacing) / Math.Max(count, 1));
            _fieldRects = new Rectangle[count];

            for (int i = 0; i < count; i++)
            {
                int x = ctx.ContentRect.Left + i * (perFieldWidth + spacing);
                _fieldRects[i] = new Rectangle(x + labelWidth, ctx.ContentRect.Top, perFieldWidth - labelWidth, fieldHeight);
                ctx.CustomData[$"InlineFieldLabel_{i}"] = fields[i];
                ctx.CustomData[$"InlineFieldRect_{i}"] = _fieldRects[i];
            }

            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw each label and textbox-like area
            using var labelFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8.5f, FontStyle.Bold);
            using var valueFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8.5f, FontStyle.Regular);
            using var labelBrush = new SolidBrush(Theme?.TextBoxForeColor ?? Theme?.ForeColor ?? Color.Black);
            using var borderPen = new Pen(Theme?.TextBoxBorderColor ?? Theme?.BorderColor ?? Color.Gray, 1);

            for (int i = 0; i < _fieldRects.Length; i++)
            {
                string label = ctx.CustomData[$"InlineFieldLabel_{i}"]?.ToString() ?? $"Field {i + 1}";
                var labelRect = new Rectangle(_fieldRects[i].Left - 90, _fieldRects[i].Top, 86, _fieldRects[i].Height);
                var textRect = Rectangle.Inflate(_fieldRects[i], -6, -4);

                g.DrawString(label, labelFont, labelBrush, labelRect, new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });

                // Field background
                using var bg = new SolidBrush(Theme?.TextBoxBackColor ?? Theme?.BackColor ?? Color.White);
                using var path = CreateRoundedPath(_fieldRects[i], 4);
                g.FillPath(bg, path);
                g.DrawPath(borderPen, path);

                // Value (placeholder)
                string value = ctx.CustomData.ContainsKey($"InlineFieldValue_{i}") ? ctx.CustomData[$"InlineFieldValue_{i}"].ToString() : "";
                using var valueBrush = new SolidBrush(string.IsNullOrEmpty(value) ? Color.Gray : (Theme?.TextBoxForeColor ?? Theme?.ForeColor ?? Color.Black));
                g.DrawString(string.IsNullOrEmpty(value) ? "Type here..." : value, valueFont, valueBrush, textRect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });

                // Hover cue
                if (IsAreaHovered($"InlineForm_Field_{i}"))
                {
                    using var hover = new Pen(Theme?.AccentColor ?? Color.Blue, 1.5f);
                    g.DrawPath(hover, path);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) { }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            for (int i = 0; i < _fieldRects.Length; i++)
            {
                int idx = i;
                var rect = _fieldRects[i];
                owner.AddHitArea($"InlineForm_Field_{idx}", rect, null, () =>
                {
                    ctx.CustomData[$"InlineFieldFocused_{idx}"] = true;
                    notifyAreaHit?.Invoke($"InlineForm_Field_{idx}", rect);
                    Owner?.Invalidate();
                });
            }
        }
    }
}