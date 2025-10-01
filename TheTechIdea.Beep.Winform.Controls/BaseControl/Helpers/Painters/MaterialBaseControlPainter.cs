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
    /// Material Design painter that draws: background, outlined/filled/standard borders,
    /// label centered on the top border with a notched gap, and helper/error text below.
    /// The inner content rectangle is exposed to derived controls via owner.DrawingRect.
    /// </summary>
    internal sealed class MaterialBaseControlPainter : IBaseControlPainter
    {
        private Rectangle _drawingRect;
        private Rectangle _borderRect;
        private Rectangle _contentRect;

        public Rectangle DrawingRect => _drawingRect;
        public Rectangle BorderRect => _borderRect;
        public Rectangle ContentRect => _contentRect;

        // Small type sizes for Material floating label + helper
        private const float LabelPt = 9f;   // small label font
        private const float HelperPt = 8f;  // small helper font

        public void UpdateLayout(Base.BaseControl owner)
        {
            if (owner == null)
            {
                _drawingRect = _borderRect = _contentRect = Rectangle.Empty;
                return;
            }

            var materialPadding = GetMaterialStylePadding(owner);
            var effects = GetMaterialEffectsSpace(owner);

            // Base inner area before label/helper reservations
            int leftPad = materialPadding.Left + effects.Width / 2;
            int topPad = materialPadding.Top + effects.Height / 2;
            int rightPad = materialPadding.Right + effects.Width / 2;
            int bottomPad = materialPadding.Bottom + effects.Height / 2;

            Rectangle baseRect = new Rectangle(
                leftPad,
                topPad,
                Math.Max(0, owner.Width - leftPad - rightPad),
                Math.Max(0, owner.Height - topPad - bottomPad)
            );

            // Calculate label/helper reserves with small fonts (fixed small sizes)
            int reserveTop = 0, reserveBottom = 0, halfLabelH = 0;
            try
            {
                using var g = owner.CreateGraphics();
                using var lf = new Font(owner.Font.FontFamily, LabelPt, FontStyle.Regular);
                int labelH = TextRenderer.MeasureText(g, "Ag", lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                halfLabelH = (int)Math.Ceiling(labelH / 2f);
                reserveTop = halfLabelH + 4; // room above the border to center label on top line

                using var sf = new Font(owner.Font.FontFamily, HelperPt, FontStyle.Regular);
                int helperH = TextRenderer.MeasureText(g, "Ag", sf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                reserveBottom = helperH + 6; // space for helper/error and a small gap
            }
            catch
            {
                halfLabelH = 8;
                reserveTop = 12;
                reserveBottom = 18;
            }

            // Final drawable inner area
            _drawingRect = new Rectangle(
                baseRect.X,
                baseRect.Y + reserveTop,
                baseRect.Width,
                Math.Max(0, baseRect.Height - reserveTop - reserveBottom)
            );

            // Border rectangle (shrink to leave top/bottom reserves). Offset down by half label height so the label
            // can sit centered across the top border inside the control.
            int bx = effects.Width / 4;
            int by = effects.Height / 4 + halfLabelH; // push down half label height
            int bw = Math.Max(0, owner.Width - effects.Width / 2);
            int bh = Math.Max(0, owner.Height - effects.Height / 2 - halfLabelH - reserveBottom);
            _borderRect = new Rectangle(bx, by, bw, bh);

            // Content rectangle (adjust for icons)
            Rectangle contentRect = _drawingRect;
            bool hasLeading = !string.IsNullOrEmpty(owner.LeadingIconPath) || !string.IsNullOrEmpty(owner.LeadingImagePath);
            bool hasTrailing = !string.IsNullOrEmpty(owner.TrailingIconPath) || !string.IsNullOrEmpty(owner.TrailingImagePath) || owner.ShowClearButton;
            if (hasLeading || hasTrailing)
            {
                var icons = new BaseControlIconsHelper(owner);
                icons.UpdateLayout(contentRect);
                contentRect = icons.AdjustedContentRect;
            }

            _contentRect = contentRect;
            owner.DrawingRect = _contentRect; // expose to owner for content drawing
        }

        public void Paint(Graphics g, Base.BaseControl owner)
        {
            if (g == null || owner == null) return;

            var oldSmoothingMode = g.SmoothingMode;
            var oldInterpolationMode = g.InterpolationMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            try
            {
                // Elevation
                if (owner.MaterialUseElevation && owner.MaterialElevationLevel > 0)
                {
                    DrawMaterialElevation(g, owner);
                }

                // Background
                DrawMaterialBackground(g, owner);

                // Notch for label
                RectangleF labelGap = RectangleF.Empty;
                using var labelFont = GetLabelFont(owner);
                if (!string.IsNullOrEmpty(owner.LabelText))
                {
                    var size = TextRenderer.MeasureText(g, owner.LabelText, labelFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                    int padX = 6;
                    int labelX = _borderRect.Left + 12;
                    int labelW = size.Width + padX * 2;
                    int labelH = labelFont.Height;
                    labelGap = new RectangleF(labelX - 2, _borderRect.Top - labelH / 2f - 1, labelW + 4, labelH + 2);
                }

                DrawMaterialBorder(g, owner, labelGap);
                DrawMaterialLabels(g, owner);

                // Icons
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

        private void DrawMaterialBorder(Graphics g, Base.BaseControl owner, RectangleF labelGap)
        {
            var theme = owner._currentTheme;
            Color borderColor = GetMaterialBorderColor(owner, theme);
            int borderWidth = owner.HasError ? 2 : 1;

            using var borderPen = new Pen(borderColor, borderWidth);

            switch (owner.MaterialBorderVariant)
            {
                case MaterialTextFieldVariant.Outlined:
                    using (var path = CreateRoundedPathWithTopGap(_borderRect, owner.MaterialBorderRadius, labelGap))
                        g.DrawPath(borderPen, path);
                    break;
                case MaterialTextFieldVariant.Filled:
                case MaterialTextFieldVariant.Standard:
                    g.DrawLine(borderPen, _borderRect.Left, _borderRect.Bottom, _borderRect.Right, _borderRect.Bottom);
                    break;
            }

            if (owner.IsFocused)
            {
                using var focusPen = new Pen(owner.MaterialPrimaryColor, 2);
                if (owner.MaterialBorderVariant == MaterialTextFieldVariant.Outlined)
                {
                    using var fpath = CreateRoundedPathWithTopGap(_borderRect, owner.MaterialBorderRadius, labelGap);
                    g.DrawPath(focusPen, fpath);
                }
                else
                {
                    g.DrawLine(focusPen, _borderRect.Left, _borderRect.Bottom, _borderRect.Right, _borderRect.Bottom);
                }
            }
        }

        private void DrawMaterialLabels(Graphics g, Base.BaseControl owner)
        {
            var theme = owner._currentTheme;

            // Label color must match the border color
            if (!string.IsNullOrEmpty(owner.LabelText))
            {
                using var labelFont = GetLabelFont(owner);
                Color labelColor = GetMaterialBorderColor(owner, theme);

                var size = TextRenderer.MeasureText(g, owner.LabelText, labelFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                int padX = 6;
                int labelX = _borderRect.Left + 12 + padX;
                int labelH = labelFont.Height;
                int textY = _borderRect.Top - (labelH / 2); // center across top border
                var textRect = new Rectangle(labelX, textY, size.Width, labelH);

                TextRenderer.DrawText(g, owner.LabelText, labelFont, textRect, labelColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }

            // Helper/error text below: red for error, green for normal helper
            string supporting = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorText : owner.HelperText;
            if (!string.IsNullOrEmpty(supporting))
            {
                using var supportFont = new Font(owner.Font.FontFamily, HelperPt, FontStyle.Regular);
                Color supportColor = !string.IsNullOrEmpty(owner.ErrorText)
                    ? owner.ErrorColor
                    : (theme?.SuccessColor ?? Color.Green);

                Rectangle supportRect = new Rectangle(_borderRect.Left + 12, _borderRect.Bottom + 4,
                    Math.Max(0, _borderRect.Width - 24), supportFont.Height + 2);

                TextRenderer.DrawText(g, supporting, supportFont, supportRect, supportColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
        }

        private Color GetMaterialBackgroundColor(Base.BaseControl owner, TheTechIdea.Beep.Vis.Modules.IBeepTheme theme)
        {
            if (!owner.Enabled)
                return theme?.DisabledBackColor ?? Color.FromArgb(250, 250, 250);

            if (owner.MaterialBorderVariant == MaterialTextFieldVariant.Filled && owner.MaterialShowFill)
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

            return owner.MaterialBorderVariant switch
            {
                MaterialTextFieldVariant.Outlined => new Padding(16, 12, 16, 12),
                MaterialTextFieldVariant.Filled => new Padding(16, 16, 16, 8),
                MaterialTextFieldVariant.Standard => new Padding(0, 8, 0, 8),
                _ => new Padding(16, 12, 16, 12)
            };
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

            // Compute reserves using the same small fonts used for drawing
            int extraTop, extraBottom;
            try
            {
                using var g = owner.CreateGraphics();
                using var lf = new Font(owner.Font.FontFamily, LabelPt, FontStyle.Regular);
                extraTop = (int)Math.Ceiling(TextRenderer.MeasureText(g, "Ag", lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height / 2f) + 4;
                using var sf = new Font(owner.Font.FontFamily, HelperPt, FontStyle.Regular);
                extraBottom = TextRenderer.MeasureText(g, "Ag", sf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height + 6;
            }
            catch
            {
                extraTop = 12;
                extraBottom = 18;
            }

            return new Size(
                Math.Max(minWidth, proposedSize.Width),
                Math.Max(minHeight + extraTop + extraBottom, proposedSize.Height)
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

        private GraphicsPath CreateRoundedPathWithTopGap(Rectangle rect, int radius, RectangleF gap)
        {
            if (gap == RectangleF.Empty)
                return CreateRoundedPath(rect, radius);

            var path = new GraphicsPath();
            if (radius <= 0)
            {
                float left = rect.Left;
                float right = rect.Right;
                float top = rect.Top;
                float bottom = rect.Bottom;

                float gapStart = Math.Max(left, gap.Left);
                float gapEnd = Math.Min(right, gap.Right);

                path.StartFigure();
                path.AddLine(left, top, left, bottom);
                path.AddLine(left, bottom, right, bottom);
                path.AddLine(right, bottom, right, top);
                path.AddLine(right, top, gapEnd, top);
                path.StartFigure();
                path.AddLine(gapStart, top, left, top);
                return path;
            }

            int diameter = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            float leftX = rect.Left;
            float rightX = rect.Right;
            float topY = rect.Top;
            float bottomY = rect.Bottom;

            float gapStartX = Math.Max(leftX + radius, gap.Left);
            float gapEndX = Math.Min(rightX - radius, gap.Right);

            path.StartFigure();
            path.AddArc(rect.Left, rect.Top, diameter, diameter, 180, 90);
            path.AddLine(leftX + radius, topY, gapStartX, topY);

            path.StartFigure();
            path.AddLine(gapEndX, topY, rightX - radius, topY);
            path.AddArc(rect.Right - diameter, rect.Top, diameter, diameter, 270, 90);
            path.AddLine(rightX, topY + radius, rightX, bottomY - radius);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddLine(rightX - radius, bottomY, leftX + radius, bottomY);
            path.AddArc(rect.Left, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.AddLine(leftX, bottomY - radius, leftX, topY + radius);
            return path;
        }

        private Font GetLabelFont(Base.BaseControl owner)
        {
            // Force small label font regardless of owner font to match Material style
            return new Font(owner.Font.FontFamily, LabelPt, FontStyle.Regular);
        }
    }
}
