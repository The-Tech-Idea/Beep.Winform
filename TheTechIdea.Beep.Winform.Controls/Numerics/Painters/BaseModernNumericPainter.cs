using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Numerics.Painters
{
    /// <summary>
    /// Base class for modern numeric painters.
    /// Provides common layout calculations and icon painting.
    /// Visual styling (backgrounds, borders, shadows) handled by BeepStyling.
    /// </summary>
    public abstract class BaseModernNumericPainter : INumericUpDownPainter
    {
        protected const int DefaultButtonWidth = 24;
        protected const int DefaultButtonHeight = 20;
        protected const int DefaultPadding = 4;
        protected const int IconSize = 12;

        public abstract NumericLayoutInfo CalculateLayout(INumericUpDownPainterContext context, Rectangle bounds);
        
        public virtual string FormatValue(INumericUpDownPainterContext context)
        {
            string valueStr = context.DecimalPlaces > 0
                ? context.Value.ToString($"N{context.DecimalPlaces}")
                : (context.ThousandsSeparator ? context.Value.ToString("N0") : context.Value.ToString());

            return context.DisplayMode switch
            {
                NumericUpDownDisplayMode.Percentage => $"{valueStr}%",
                NumericUpDownDisplayMode.Currency => $"${valueStr}",
                NumericUpDownDisplayMode.CustomUnit => $"{context.Prefix}{valueStr}{context.Suffix} {context.Unit}".Trim(),
                NumericUpDownDisplayMode.ProgressValue => $"{valueStr}%",
                _ => valueStr
            };
        }

        public abstract void PaintButtonIcons(Graphics g, INumericUpDownPainterContext context, 
            Rectangle upButtonRect, Rectangle downButtonRect);

        public virtual void PaintValueText(Graphics g, INumericUpDownPainterContext context, 
            Rectangle textRect, string formattedText)
        {
            if (string.IsNullOrEmpty(formattedText)) return;

            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Color textColor = GetTextColor(context);
            
            using (var textBrush = new SolidBrush(textColor))
            using (var font = GetFont(context))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                g.DrawString(formattedText, font, textBrush, textRect, sf);
            }
        }

        public virtual void UpdateHitAreas(INumericUpDownPainterContext context, Rectangle bounds,
            Action<string, Rectangle, Action> registerHitArea)
        {
            var layout = CalculateLayout(context, bounds);

            if (layout.ShowButtons)
            {
                if (layout.UpButtonRect != Rectangle.Empty)
                {
                    registerHitArea("UpButton", layout.UpButtonRect, context.IncreaseValue);
                }

                if (layout.DownButtonRect != Rectangle.Empty)
                {
                    registerHitArea("DownButton", layout.DownButtonRect, context.DecrementValue);
                }
            }

            if (layout.TextRect != Rectangle.Empty)
            {
                registerHitArea("TextArea", layout.TextRect, () => { /* Start editing */ });
            }
        }

        protected virtual Color GetTextColor(INumericUpDownPainterContext context)
        {
            // Use theme helpers for consistent color retrieval
            return TheTechIdea.Beep.Winform.Controls.Numerics.Helpers.NumericThemeHelpers.GetNumericTextColor(
                context.Theme,
                context.Theme != null, // Assume UseThemeColors if theme is available
                context.IsHovered,
                context.IsFocused,
                !context.IsEnabled);
        }

        protected virtual Font GetFont(INumericUpDownPainterContext context)
        {
            // Use font helpers for consistent font retrieval
            // Note: We need ControlStyle from context, but it's not available
            // For now, use default Material3 style
            return TheTechIdea.Beep.Winform.Controls.Numerics.Helpers.NumericFontHelpers.GetValueFont(
                TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.Material3,
                context.IsEditing);
        }

        /// <summary>
        /// Paint arrow icon (up or down) using SVG
        /// </summary>
        protected void PaintArrowIcon(Graphics g, Rectangle rect, bool isUp, Color color)
        {
            if (rect.IsEmpty || rect.Width <= 0 || rect.Height <= 0) return;

            // Use icon helpers for consistent icon rendering
            string svgPath = isUp 
                ? TheTechIdea.Beep.Winform.Controls.Numerics.Helpers.NumericIconHelpers.GetUpIconPath()
                : TheTechIdea.Beep.Winform.Controls.Numerics.Helpers.NumericIconHelpers.GetDownIconPath();
            
            TheTechIdea.Beep.Winform.Controls.Numerics.Helpers.NumericIconHelpers.PaintIcon(
                g, rect, svgPath, color, null, false, 
                TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.Material3, 1.0f);
        }

        /// <summary>
        /// Paint plus/minus icon using SVG
        /// </summary>
        protected void PaintPlusMinusIcon(Graphics g, Rectangle rect, bool isPlus, Color color)
        {
            if (rect.IsEmpty || rect.Width <= 0 || rect.Height <= 0) return;

            // Use icon helpers for consistent icon rendering
            string svgPath = isPlus 
                ? TheTechIdea.Beep.Winform.Controls.Numerics.Helpers.NumericIconHelpers.GetPlusIconPath()
                : TheTechIdea.Beep.Winform.Controls.Numerics.Helpers.NumericIconHelpers.GetMinusIconPath();
            
            TheTechIdea.Beep.Winform.Controls.Numerics.Helpers.NumericIconHelpers.PaintIcon(
                g, rect, svgPath, color, null, false, 
                TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.Material3, 1.0f);
        }

        /// <summary>
        /// Paint a custom SVG icon with tint
        /// </summary>
        protected void PaintSvgIcon(Graphics g, Rectangle rect, string svgPath, Color color, float opacity = 1f)
        {
            if (rect.IsEmpty || rect.Width <= 0 || rect.Height <= 0 || string.IsNullOrEmpty(svgPath)) return;

            using (var iconPath = GraphicsExtensions.GetRoundedRectPath(rect, 0))
            {
                StyledImagePainter.PaintWithTint(g, iconPath, svgPath, color, opacity, 0);
            }
        }

        /// <summary>
        /// Get icon color based on button state
        /// </summary>
        protected Color GetIconColor(INumericUpDownPainterContext context, bool isHovered, bool isPressed)
        {
            if (!context.IsEnabled)
                return context.Theme?.DisabledForeColor ?? Color.LightGray;

            if (isPressed)
                return context.Theme?.ButtonPressedForeColor ?? Color.FromArgb(0, 90, 158);

            if (isHovered)
                return context.Theme?.ButtonHoverForeColor ?? Color.FromArgb(0, 120, 212);

            return context.Theme?.ButtonForeColor ?? Color.FromArgb(96, 96, 96);
        }
    }
}
