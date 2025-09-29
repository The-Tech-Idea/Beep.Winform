using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Form
{
    /// <summary>
    /// InputCard - Styled input container painter with enhanced visual presentation and hit areas
    /// </summary>
    internal sealed class InputCardPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        // Cached interactive rects
        private Rectangle _headerIconRect;
        private Rectangle _headerLabelRect;
        private Rectangle _inputRectCache;
        private Rectangle _validationIconRect;
        private Rectangle _footerRectCache;

        public InputCardPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -4, -4);
            
            // Input card header
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                24
            );
            
            // Main input area
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                40
            );
            
            // Help text and validation area
            ctx.FooterRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.ContentRect.Bottom + 4,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Bottom - ctx.ContentRect.Bottom - pad - 4
            );

            // Cache hit areas
            _headerIconRect = new Rectangle(ctx.HeaderRect.X, ctx.HeaderRect.Y + 2, 16, 16);
            _headerLabelRect = new Rectangle(_headerIconRect.Right + 6, ctx.HeaderRect.Y, ctx.HeaderRect.Width - _headerIconRect.Width - 6, ctx.HeaderRect.Height);
            _inputRectCache = ctx.ContentRect;
            _footerRectCache = ctx.FooterRect;
            // Validation icon resides at right of input
            _validationIconRect = new Rectangle(ctx.ContentRect.Right - 24, ctx.ContentRect.Y + (ctx.ContentRect.Height - 16) / 2, 16, 16);
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 6, layers: 2, offset: 1);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Configure ImagePainter with theme
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.UseThemeColors = true;

            var fields = ctx.CustomData.ContainsKey("Fields") ? 
                (List<FormField>)ctx.CustomData["Fields"] : new List<FormField>();
            var validationResults = ctx.CustomData.ContainsKey("ValidationResults") ? 
                (List<ValidationResult>)ctx.CustomData["ValidationResults"] : new List<ValidationResult>();
            var validColor = Color.FromArgb(76, 175, 80);
            var errorColor = Color.FromArgb(244, 67, 54);
            var showRequired = ctx.CustomData.ContainsKey("ShowRequired") && ctx.CustomData["ShowRequired"] is bool b1 ? b1 : true;
            var isReadOnly = ctx.CustomData.ContainsKey("IsReadOnly") && ctx.CustomData["IsReadOnly"] is bool b2 ? b2 : false;

            // Get primary field (first field or focused field)
            var primaryField = fields.FirstOrDefault();
            if (primaryField == null) 
            {
                DrawSampleInputCard(g, ctx, showRequired, isReadOnly, validColor, errorColor);
                return;
            }
            
            var validation = validationResults.FirstOrDefault(v => v.FieldName == primaryField.Name);

            // Draw input card
            DrawInputCard(g, ctx, primaryField, validation, showRequired, isReadOnly, validColor, errorColor);
        }

        private void DrawSampleInputCard(Graphics g, WidgetContext ctx, bool showRequired, bool isReadOnly, Color validColor, Color errorColor)
        {
            var sampleField = new FormField { Name = "email", Label = "Email Address", Type = FormFieldType.Email, Placeholder = "Enter your email", IsRequired = true };
            DrawInputCard(g, ctx, sampleField, null, showRequired, isReadOnly, validColor, errorColor);
        }

        private void DrawInputCard(Graphics g, WidgetContext ctx, FormField field, ValidationResult validation, 
            bool showRequired, bool isReadOnly, Color validColor, Color errorColor)
        {
            // Draw field label
            DrawCardLabel(g, ctx.HeaderRect, field, showRequired, errorColor, ctx.AccentColor);
            
            // Draw input area
            DrawCardInput(g, ctx.ContentRect, field, validation, isReadOnly, validColor, errorColor, ctx.AccentColor);
            
            // Draw help text and validation
            DrawCardFooter(g, ctx.FooterRect, field, validation, errorColor);
        }

        private void DrawCardLabel(Graphics g, Rectangle rect, FormField field, bool showRequired, 
            Color errorColor, Color accentColor)
        {
            // Field type icon
            var iconRect = _headerIconRect;
            string iconName = GetFieldTypeIcon(field.Type.ToString());
            _imagePainter.DrawSvg(g, iconName, iconRect, 
                Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243), 0.8f);

            // Field label
            var labelRect = _headerLabelRect;
            using var labelFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 11f, FontStyle.Regular);
            using var labelBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            g.DrawString(field.Label, labelFont, labelBrush, labelRect, format);
            
            // Required indicator
            if (showRequired && field.IsRequired)
            {
                var reqRect = new Rectangle(rect.Right - 20, rect.Y + 2, 16, 16);
                _imagePainter.DrawSvg(g, "alert-circle", reqRect, errorColor, 0.7f);
            }
        }

        private string GetFieldTypeIcon(string fieldType)
        {
            return fieldType?.ToLower() switch
            {
                "email" => "mail",
                "phone" => "phone",
                "password" => "lock",
                "url" => "link",
                "number" => "hash",
                "date" => "calendar",
                "time" => "clock",
                "file" => "upload",
                "search" => "search",
                "textarea" => "align-left",
                _ => "edit-3"
            };
        }

        private void DrawCardInput(Graphics g, Rectangle rect, FormField field, ValidationResult validation, 
            bool isReadOnly, Color validColor, Color errorColor, Color accentColor)
        {
            // Determine border color based on validation state
            Color borderColor = Color.FromArgb(200, 200, 200);
            if (validation != null)
            {
                borderColor = validation.IsValid ? validColor : errorColor;
            }
            else if (!isReadOnly)
            {
                borderColor = accentColor;
            }
            
            // Draw input background
            Color bgColor = isReadOnly ? Color.FromArgb(245, 245, 245) : Color.White;
            using var inputBrush = new SolidBrush(bgColor);
            using var borderPen = new Pen(borderColor, 2);
            using var inputPath = CreateRoundedPath(rect, 6);
            
            g.FillPath(inputBrush, inputPath);
            g.DrawPath(borderPen, inputPath);
            
            // Draw input content
            DrawInputContent(g, rect, field, isReadOnly);
            
            // Draw validation indicator
            if (validation != null)
            {
                DrawInputValidationIndicator(g, rect, validation, validColor, errorColor);
            }
        }

        private void DrawInputContent(Graphics g, Rectangle rect, FormField field, bool isReadOnly)
        {
            var contentRect = new Rectangle(rect.X + 12, rect.Y + 8, rect.Width - 40, rect.Height - 16);
            
            // Draw field value or placeholder
            string displayText = "";
            Color textColor = Theme?.ForeColor ?? Color.FromArgb(200, Color.Black);
            
            if (field.Value != null && !string.IsNullOrEmpty(field.Value.ToString()))
            {
                displayText = field.Value.ToString();
                if (field.Type == FormFieldType.Password)
                {
                    displayText = new string('•', displayText.Length);
                }
            }
            else if (!string.IsNullOrEmpty(field.Placeholder))
            {
                displayText = field.Placeholder;
                textColor = Theme?.TextBoxPlaceholderColor ?? Color.FromArgb(120, Color.Gray);
            }
            
            if (!string.IsNullOrEmpty(displayText))
            {
                using var contentFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 10f, FontStyle.Regular);
                using var contentBrush = new SolidBrush(textColor);
                var contentFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                
                g.DrawString(displayText, contentFont, contentBrush, contentRect, contentFormat);
            }
            
            // Draw focus indicator (cursor)
            if (!isReadOnly && field.Value != null)
            {
                DrawFocusIndicator(g, contentRect);
            }
        }

        private void DrawFocusIndicator(Graphics g, Rectangle contentRect)
        {
            using var cursorPen = new Pen(Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243), 1);
            int cursorX = contentRect.X + contentRect.Width - 20;
            g.DrawLine(cursorPen, cursorX, contentRect.Y + 4, cursorX, contentRect.Bottom - 4);
        }

        private void DrawInputValidationIndicator(Graphics g, Rectangle rect, ValidationResult validation, 
            Color validColor, Color errorColor)
        {
            Color indicatorColor = validation.IsValid ? validColor : errorColor;
            string indicatorText = validation.IsValid ? "✓" : "!";
            
            var indicatorRect = _validationIconRect;
            
            // Draw indicator background
            using var indicatorBrush = new SolidBrush(Color.FromArgb(200, indicatorColor));
            g.FillEllipse(indicatorBrush, indicatorRect);
            
            // Draw indicator symbol
            using var symbolFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Bold);
            using var symbolBrush = new SolidBrush(Color.White);
            var symbolFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(indicatorText, symbolFont, symbolBrush, indicatorRect, symbolFormat);
        }

        private void DrawCardFooter(Graphics g, Rectangle rect, FormField field, ValidationResult validation, Color errorColor)
        {
            int y = rect.Y;
            using var footerFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Regular);
            
            // Draw help text
            if (!string.IsNullOrEmpty(field.HelpText))
            {
                using var helpBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));
                g.DrawString(field.HelpText, footerFont, helpBrush, rect.X, y);
                y += 14;
            }
            
            // Draw validation message
            if (validation != null && !validation.IsValid && !string.IsNullOrEmpty(validation.Message))
            {
                using var errorBrush = new SolidBrush(errorColor);
                g.DrawString(validation.Message, footerFont, errorBrush, rect.X, y);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Focus indicator
            if (ctx.CustomData.ContainsKey("IsFocused") && ctx.CustomData["IsFocused"] is bool isFocused && isFocused)
            {
                var focusRect = Rectangle.Inflate(ctx.DrawingRect, 2, 2);
                using var focusPen = new Pen(Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243), 2);
                g.DrawRoundedRectangle(focusPen, focusRect, 8);
            }

            // Hover effects for header label and icon
            if (IsAreaHovered("InputCard_HeaderLabel"))
            {
                using var hover = new SolidBrush(Color.FromArgb(8, Theme?.PrimaryColor ?? Color.Blue));
                g.FillRectangle(hover, _headerLabelRect);
            }
            if (IsAreaHovered("InputCard_HeaderIcon"))
            {
                using var pen = new Pen(Theme?.PrimaryColor ?? Color.Blue, 1.2f);
                g.DrawRectangle(pen, _headerIconRect);
            }
            // Input hover
            if (IsAreaHovered("InputCard_Input"))
            {
                using var hover = new SolidBrush(Color.FromArgb(6, Theme?.PrimaryColor ?? Color.Blue));
                using var p = CreateRoundedPath(Rectangle.Inflate(_inputRectCache, 2, 2), 6);
                g.FillPath(hover, p);
            }
            // Validation icon hover
            if (IsAreaHovered("InputCard_ValidationIcon"))
            {
                using var pen = new Pen(Theme?.AccentColor ?? Color.Gray, 1.2f);
                g.DrawEllipse(pen, _validationIconRect);
            }
            // Footer hover
            if (IsAreaHovered("InputCard_Footer"))
            {
                using var hover = new SolidBrush(Color.FromArgb(4, Theme?.PrimaryColor ?? Color.Blue));
                g.FillRectangle(hover, _footerRectCache);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            owner.AddHitArea("InputCard_HeaderIcon", _headerIconRect, null, () =>
            {
                ctx.CustomData["HeaderIconClicked"] = true;
                notifyAreaHit?.Invoke("InputCard_HeaderIcon", _headerIconRect);
                Owner?.Invalidate();
            });
            owner.AddHitArea("InputCard_HeaderLabel", _headerLabelRect, null, () =>
            {
                ctx.CustomData["HeaderLabelClicked"] = true;
                notifyAreaHit?.Invoke("InputCard_HeaderLabel", _headerLabelRect);
                Owner?.Invalidate();
            });
            owner.AddHitArea("InputCard_Input", _inputRectCache, null, () =>
            {
                ctx.CustomData["InputClicked"] = true;
                notifyAreaHit?.Invoke("InputCard_Input", _inputRectCache);
                Owner?.Invalidate();
            });
            owner.AddHitArea("InputCard_ValidationIcon", _validationIconRect, null, () =>
            {
                ctx.CustomData["ValidationIconClicked"] = true;
                notifyAreaHit?.Invoke("InputCard_ValidationIcon", _validationIconRect);
                Owner?.Invalidate();
            });
            owner.AddHitArea("InputCard_Footer", _footerRectCache, null, () =>
            {
                ctx.CustomData["FooterClicked"] = true;
                notifyAreaHit?.Invoke("InputCard_Footer", _footerRectCache);
                Owner?.Invalidate();
            });
        }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }
}