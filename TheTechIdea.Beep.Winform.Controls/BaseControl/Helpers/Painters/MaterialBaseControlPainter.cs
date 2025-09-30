using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// Material Design painter: provides Material Design 3 styling (borders, backgrounds, elevation)
    /// while leaving inner content (DrawingRect) for inheriting controls to handle.
    /// Uses current theme colors and Material Design specifications.
    /// </summary>
    internal sealed class MaterialBaseControlPainter : IBaseControlPainter
    {
        private Rectangle _drawingRect;
        private Rectangle _borderRect;
        private Rectangle _contentRect;

        public Rectangle DrawingRect => _drawingRect;
        public Rectangle BorderRect => _borderRect;
        public Rectangle ContentRect => _contentRect;

        public void UpdateLayout(Base.BaseControl owner)
        {
            if (owner == null)
            {
                _drawingRect = _borderRect = _contentRect = Rectangle.Empty;
                return;
            }

            var materialPadding = GetMaterialStylePadding(owner);
            var effects = GetMaterialEffectsSpace(owner);
            
            // Calculate drawing rect with Material Design spacing
            int leftPad = materialPadding.Left + effects.Width / 2;
            int topPad = materialPadding.Top + effects.Height / 2;
            int rightPad = materialPadding.Right + effects.Width / 2;
            int bottomPad = materialPadding.Bottom + effects.Height / 2;

            _drawingRect = new Rectangle(
                leftPad,
                topPad,
                Math.Max(0, owner.Width - leftPad - rightPad),
                Math.Max(0, owner.Height - topPad - bottomPad)
            );

            // Border rect for Material Design
            _borderRect = new Rectangle(
                effects.Width / 4,
                effects.Height / 4,
                Math.Max(0, owner.Width - effects.Width / 2),
                Math.Max(0, owner.Height - effects.Height / 2)
            );

            // Reserve space for floating label and helper text
            Rectangle contentRect = _drawingRect;
            try
            {
                int reserveTop = 0;
                int reserveBottom = 0;
                using var g = owner.CreateGraphics();

                if (!string.IsNullOrEmpty(owner.LabelText) && owner.FloatingLabel)
                {
                    float labelSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var lf = new Font(owner.Font.FontFamily, labelSize, FontStyle.Regular);
                    int h = TextRenderer.MeasureText(g, "Ag", lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    reserveTop = h + 4;
                }

                string support = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorText : owner.HelperText;
                if (!string.IsNullOrEmpty(support))
                {
                    float supSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var sf = new Font(owner.Font.FontFamily, supSize, FontStyle.Regular);
                    int h = TextRenderer.MeasureText(g, "Ag", sf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    reserveBottom = h + 6;
                }

                if (reserveTop > 0 || reserveBottom > 0)
                {
                    contentRect = new Rectangle(
                        contentRect.X,
                        contentRect.Y + reserveTop,
                        contentRect.Width,
                        Math.Max(0, contentRect.Height - reserveTop - reserveBottom)
                    );
                }
            }
            catch { /* best-effort */ }

            // Adjust for icons
            bool hasLeading = !string.IsNullOrEmpty(owner.LeadingIconPath) || !string.IsNullOrEmpty(owner.LeadingImagePath);
            bool hasTrailing = !string.IsNullOrEmpty(owner.TrailingIconPath) || !string.IsNullOrEmpty(owner.TrailingImagePath) || owner.ShowClearButton;
            if (hasLeading || hasTrailing)
            {
                var icons = new BaseControlIconsHelper(owner);
                icons.UpdateLayout(contentRect);
                contentRect = icons.AdjustedContentRect;
            }

            _contentRect = contentRect;
        }

        public void Paint(Graphics g, Base.BaseControl owner)
        {
            if (g == null || owner == null) return;

            var theme = owner._currentTheme;

            // Enable high-quality rendering
            var oldSmoothingMode = g.SmoothingMode;
            var oldInterpolationMode = g.InterpolationMode;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            try
            {
                // 1. Draw elevation shadow
                if (owner.MaterialUseElevation && owner.MaterialElevationLevel > 0)
                {
                    DrawMaterialElevation(g, owner);
                }

                // 2. Draw Material background
                DrawMaterialBackground(g, owner);

                // 3. Draw Material border
                DrawMaterialBorder(g, owner);

                // 4. Draw floating label and helper text
                DrawMaterialLabels(g, owner);

                // 5. Draw icons if any
                bool hasLeading = !string.IsNullOrEmpty(owner.LeadingIconPath) || !string.IsNullOrEmpty(owner.LeadingImagePath);
                bool hasTrailing = !string.IsNullOrEmpty(owner.TrailingIconPath) || !string.IsNullOrEmpty(owner.TrailingImagePath) || owner.ShowClearButton;
                if (hasLeading || hasTrailing)
                {
                    var icons = new BaseControlIconsHelper(owner);
                    icons.UpdateLayout(_drawingRect);
                    icons.Draw(g);
                }
            }
            finally
            {
                // Restore graphics state
                g.SmoothingMode = oldSmoothingMode;
                g.InterpolationMode = oldInterpolationMode;
            }
        }

        private void DrawMaterialElevation(Graphics g, Base.BaseControl owner)
        {
            var theme = owner._currentTheme;
            Color shadowColor = theme?.ShadowColor ?? Color.FromArgb(20, Color.Black);

            int elevation = Math.Min(owner.MaterialElevationLevel, 5);
            for (int i = 1; i <= elevation; i++)
            {
                int offset = i * 2;
                int alpha = Math.Max(5, 40 - (i * 6));
                Color layerColor = Color.FromArgb(alpha, shadowColor);

                Rectangle shadowRect = new Rectangle(
                    _borderRect.X + offset,
                    _borderRect.Y + offset,
                    _borderRect.Width,
                    _borderRect.Height
                );

                using var shadowBrush = new SolidBrush(layerColor);
                using var shadowPath = CreateRoundedPath(shadowRect, owner.MaterialBorderRadius);
                g.FillPath(shadowBrush, shadowPath);
            }
        }

        private void DrawMaterialBackground(Graphics g, Base.BaseControl owner)
        {
            var theme = owner._currentTheme;
            
            Color backgroundColor = GetMaterialBackgroundColor(owner, theme);
            
            using var backgroundBrush = new SolidBrush(backgroundColor);
            using var backgroundPath = CreateRoundedPath(_borderRect, owner.MaterialBorderRadius);
            g.FillPath(backgroundBrush, backgroundPath);
        }

        private void DrawMaterialBorder(Graphics g, Base.BaseControl owner)
        {
            var theme = owner._currentTheme;
            Color borderColor = GetMaterialBorderColor(owner, theme);
            int borderWidth = owner.HasError ? 2 : 1;

            using var borderPen = new Pen(borderColor, borderWidth);
            using var borderPath = CreateRoundedPath(_borderRect, owner.MaterialBorderRadius);
            
            // Special handling for different Material variants
            switch (owner.MaterialVariant)
            {
                case MaterialTextFieldVariant.Outlined:
                    g.DrawPath(borderPen, borderPath);
                    break;
                case MaterialTextFieldVariant.Filled:
                    // Only draw bottom border for filled variant
                    g.DrawLine(borderPen, _borderRect.Left, _borderRect.Bottom, _borderRect.Right, _borderRect.Bottom);
                    break;
                case MaterialTextFieldVariant.Standard:
                    // Only draw bottom border for standard variant
                    g.DrawLine(borderPen, _borderRect.Left, _borderRect.Bottom, _borderRect.Right, _borderRect.Bottom);
                    break;
            }

            // Draw focus indicator
            if (owner.IsFocused)
            {
                Color focusColor = theme?.FocusIndicatorColor ?? owner.MaterialPrimaryColor;
                using var focusPen = new Pen(focusColor, 2);
                
                if (owner.MaterialVariant == MaterialTextFieldVariant.Outlined)
                {
                    g.DrawPath(focusPen, borderPath);
                }
                else
                {
                    // Focus underline for filled and standard variants
                    g.DrawLine(focusPen, _borderRect.Left, _borderRect.Bottom, _borderRect.Right, _borderRect.Bottom);
                }
            }
        }

        private void DrawMaterialLabels(Graphics g, Base.BaseControl owner)
        {
            var theme = owner._currentTheme;

            // Floating label
            if (!string.IsNullOrEmpty(owner.LabelText))
            {
                bool isFloating = owner.FloatingLabel && (owner.IsFocused || !string.IsNullOrEmpty(owner.Text));
                float labelSize = isFloating ? Math.Max(8f, owner.Font.Size - 2f) : Math.Max(8f, owner.Font.Size - 1f);
                
                using var labelFont = new Font(owner.Font.FontFamily, labelSize, FontStyle.Regular);
                Color labelColor = owner.HasError ? owner.ErrorColor : 
                                  (owner.IsFocused ? (theme?.FocusIndicatorColor ?? owner.MaterialPrimaryColor) : 
                                  (theme?.SecondaryTextColor ?? Color.Gray));

                Rectangle labelRect;
                if (isFloating)
                {
                    labelRect = new Rectangle(_borderRect.Left + 12, _borderRect.Top - 8, _borderRect.Width - 24, 16);
                }
                else
                {
                    labelRect = new Rectangle(_drawingRect.Left + 4, _drawingRect.Y + 4, _drawingRect.Width - 8, owner.Font.Height);
                }

                TextRenderer.DrawText(g, owner.LabelText, labelFont, labelRect, labelColor, 
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }

            // Helper text
            string supporting = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorText : owner.HelperText;
            if (!string.IsNullOrEmpty(supporting))
            {
                float supSize = Math.Max(8f, owner.Font.Size - 2f);
                using var supportFont = new Font(owner.Font.FontFamily, supSize, FontStyle.Regular);
                Color supportColor = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorColor : 
                                   (theme?.SecondaryTextColor ?? Color.Gray);

                Rectangle supportRect = new Rectangle(_borderRect.Left + 12, _borderRect.Bottom + 4, 
                    _borderRect.Width - 24, 20);

                TextRenderer.DrawText(g, supporting, supportFont, supportRect, supportColor,
                    TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis);
            }
        }

        private Color GetMaterialBackgroundColor(Base.BaseControl owner, TheTechIdea.Beep.Vis.Modules.IBeepTheme theme)
        {
            if (!owner.Enabled)
                return theme?.DisabledBackColor ?? Color.FromArgb(250, 250, 250);

            if (owner.MaterialVariant == MaterialTextFieldVariant.Filled && owner.MaterialShowFill)
            {
                return owner.MaterialFillColor;
            }

            return owner.BackColor != Color.Transparent ? owner.BackColor : 
                   (theme?.BackColor ?? Color.White);
        }

        private Color GetMaterialBorderColor(Base.BaseControl owner, TheTechIdea.Beep.Vis.Modules.IBeepTheme theme)
        {
            if (owner.HasError)
                return owner.ErrorColor;
            if (!owner.Enabled)
                return theme?.DisabledBorderColor ?? Color.Gray;
            if (owner.IsFocused)
                return theme?.FocusIndicatorColor ?? owner.MaterialPrimaryColor;
            
            return owner.MaterialOutlineColor;
        }

        private Padding GetMaterialStylePadding(Base.BaseControl owner)
        {
            if (owner.MaterialCustomPadding != Padding.Empty)
                return owner.MaterialCustomPadding;

            switch (owner.MaterialVariant)
            {
                case MaterialTextFieldVariant.Outlined:
                    return new Padding(16, 12, 16, 12);
                case MaterialTextFieldVariant.Filled:
                    return new Padding(16, 16, 16, 8);
                case MaterialTextFieldVariant.Standard:
                    return new Padding(0, 8, 0, 8);
                default:
                    return new Padding(16, 12, 16, 12);
            }
        }

        private Size GetMaterialEffectsSpace(Base.BaseControl owner)
        {
            int focusSpace = 4;
            int elevationSpace = owner.MaterialUseElevation ? Math.Min(owner.MaterialElevationLevel, 5) * 4 : 0;
            return new Size(focusSpace + elevationSpace, focusSpace + elevationSpace);
        }

        public void UpdateHitAreas(Base.BaseControl owner, Action<string, Rectangle, Action> register)
        {
            if (owner == null || register == null) return;
            
            bool hasLeading = !string.IsNullOrEmpty(owner.LeadingIconPath) || !string.IsNullOrEmpty(owner.LeadingImagePath);
            bool hasTrailing = !string.IsNullOrEmpty(owner.TrailingIconPath) || !string.IsNullOrEmpty(owner.TrailingImagePath) || owner.ShowClearButton;
            if (!(hasLeading || hasTrailing)) return;

            var icons = new BaseControlIconsHelper(owner);
            icons.UpdateLayout(_drawingRect);
            var lead = icons.LeadingRect;
            var trail = icons.TrailingRect;
            if (!lead.IsEmpty && owner.LeadingIconClickable) register("MaterialLeadingIcon", lead, owner.TriggerLeadingIconClick);
            if (!trail.IsEmpty && owner.TrailingIconClickable) register("MaterialTrailingIcon", trail, owner.TriggerTrailingIconClick);
        }

        public Size GetPreferredSize(Base.BaseControl owner, Size proposedSize)
        {
            if (owner == null) return Size.Empty;

            var materialPadding = GetMaterialStylePadding(owner);
            var effects = GetMaterialEffectsSpace(owner);
            
            int minWidth = 120 + materialPadding.Horizontal + effects.Width;
            int minHeight = owner.Font.Height + materialPadding.Vertical + effects.Height;

            // Add space for label and helper text
            if (!string.IsNullOrEmpty(owner.LabelText)) minHeight += 20;
            if (!string.IsNullOrEmpty(owner.HelperText) || !string.IsNullOrEmpty(owner.ErrorText)) minHeight += 20;

            return new Size(
                Math.Max(minWidth, proposedSize.Width),
                Math.Max(minHeight, proposedSize.Height)
            );
        }

        private GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
