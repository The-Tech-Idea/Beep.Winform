using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using static TheTechIdea.Beep.Winform.Controls.Base.BaseControl;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// Classic mode painter: computes inner/border/content rects, draws background/shadow/borders/labels, and icons.
    /// </summary>
    internal sealed class ClassicBaseControlPainter : IBaseControlPainter
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

            // Compute border thickness to consider
            int border = 0;
            if (owner.ShowAllBorders)
            {
                border = owner.BorderThickness;
            }
            else if (owner.BorderThickness > 0 && (owner.ShowTopBorder || owner.ShowBottomBorder || owner.ShowLeftBorder || owner.ShowRightBorder))
            {
                border = owner.BorderThickness;
            }

            // Base paddings + optional offsets
            var padding = owner.Padding;
            int leftPad = padding.Left + owner.LeftoffsetForDrawingRect;
            int topPad = padding.Top + owner.TopoffsetForDrawingRect;
            int rightPad = padding.Right + owner.RightoffsetForDrawingRect;
            int bottomPad = padding.Bottom + owner.BottomoffsetForDrawingRect;

            // Shadow offset
            int shadow = owner.ShowShadow ? owner.ShadowOffset : 0;

            // Calculate inner drawing rect
            int calculatedWidth = owner.Width - (shadow * 2 + border * 2 + leftPad + rightPad);
            int calculatedHeight = owner.Height - (shadow * 2 + border * 2 + topPad + bottomPad);

            var inner = new Rectangle(
                shadow + border + leftPad,
                shadow + border + topPad,
                Math.Max(0, calculatedWidth),
                Math.Max(0, calculatedHeight)
            );

            // Reserve space for label and helper when not material
            try
            {
                int reserveTop = 0;
                int reserveBottom = 0;
                using var g = owner.CreateGraphics();

                if (!string.IsNullOrEmpty(owner.LabelText))
                {
                    float labelSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var lf = new Font(owner.Font.FontFamily, labelSize, FontStyle.Regular);
                    int h = TextRenderer.MeasureText(g, "Ag", lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    reserveTop = h + 2;
                }

                string support = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorText : owner.HelperText;
                if (!string.IsNullOrEmpty(support))
                {
                    float supSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var sf = new Font(owner.Font.FontFamily, supSize, FontStyle.Regular);
                    int h = TextRenderer.MeasureText(g, "Ag", sf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    reserveBottom = h + 4;
                }

                if (reserveTop > 0 || reserveBottom > 0)
                {
                    inner = new Rectangle(
                        inner.X,
                        inner.Y + reserveTop,
                        inner.Width,
                        Math.Max(0, inner.Height - reserveTop - reserveBottom)
                    );
                }
            }
            catch { /* best-effort */ }

            // Border rectangle like BeepControl
            int halfPen = (int)Math.Ceiling(owner.BorderThickness / 2f);
            var borderRect = new Rectangle(
                shadow + halfPen,
                shadow + halfPen,
                Math.Max(0, owner.Width - (shadow + halfPen) * 2),
                Math.Max(0, owner.Height - (shadow + halfPen) * 2)
            );

            // Compute icon-adjusted content
            Rectangle contentRect = inner;
            bool hasLeading = !string.IsNullOrEmpty(owner.LeadingIconPath) || !string.IsNullOrEmpty(owner.LeadingImagePath);
            bool hasTrailing = !string.IsNullOrEmpty(owner.TrailingIconPath) || !string.IsNullOrEmpty(owner.TrailingImagePath) || owner.ShowClearButton;
            if (hasLeading || hasTrailing)
            {
                var icons = new BaseControlIconsHelper(owner);
                icons.UpdateLayout(inner);
                contentRect = icons.AdjustedContentRect;
            }

            _drawingRect = inner;    // inner rectangle equivalent for derived controls to use
            _borderRect = borderRect;
            _contentRect = contentRect;
        }

        public void Paint(Graphics g, Base.BaseControl owner)
        {
            if (g == null || owner == null) return;

            // Background (classic)
            var backColor = GetEffectiveBackColor(owner);
            using (var brush = new SolidBrush(backColor))
            {
                if (owner.IsRounded && owner.BorderRadius > 0)
                {
                    using var path = ControlPaintHelper.GetRoundedRectPath(_drawingRect, owner.BorderRadius);
                    g.FillPath(brush, path);
                }
                else
                {
                    g.FillRectangle(brush, _drawingRect);
                }
            }

            // Shadow
            if (owner.ShowShadow && owner.ShadowOpacity > 0)
            {
                DrawShadow(g, owner);
            }

            // Borders (classic)
            if (!owner.IsFrameless)
            {
                DrawBorders(g, owner);
            }

            // Material-like label/helper positioning for classic too
            DrawLabelAndHelperNonMaterial(g, owner);

            // Draw icons if any
            bool hasLeading = !string.IsNullOrEmpty(owner.LeadingIconPath) || !string.IsNullOrEmpty(owner.LeadingImagePath);
            bool hasTrailing = !string.IsNullOrEmpty(owner.TrailingIconPath) || !string.IsNullOrEmpty(owner.TrailingImagePath) || owner.ShowClearButton;
            if (hasLeading || hasTrailing)
            {
                var icons = new BaseControlIconsHelper(owner);
                icons.UpdateLayout(_drawingRect);
                icons.Draw(g);
            }
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
            if (!lead.IsEmpty && owner.LeadingIconClickable) register("ClassicLeadingIcon", lead, owner.TriggerLeadingIconClick);
            if (!trail.IsEmpty && owner.TrailingIconClickable) register("ClassicTrailingIcon", trail, owner.TriggerTrailingIconClick);
        }

        public Size GetPreferredSize(Base.BaseControl owner, Size proposedSize)
        {
            if (owner == null) return Size.Empty;

            int shadow = owner.ShowShadow ? owner.ShadowOffset * 2 : 0;
            int border = 0;
            if (owner.ShowAllBorders) border = owner.BorderThickness * 2;
            else if (owner.BorderThickness > 0 && (owner.ShowTopBorder || owner.ShowBottomBorder || owner.ShowLeftBorder || owner.ShowRightBorder))
                border = owner.BorderThickness * 2;

            var pad = owner.Padding;
            int padW = pad.Horizontal;
            int padH = pad.Vertical;

            // label/supporting reserves
            int extraTop = 0;
            int extraBottom = 0;
            try
            {
                using var g = owner.CreateGraphics();
                if (!string.IsNullOrEmpty(owner.LabelText))
                {
                    float labelSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var lf = new Font(owner.Font.FontFamily, labelSize, FontStyle.Regular);
                    int h = TextRenderer.MeasureText(g, "Ag", lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    extraTop = h + 2;
                }
                string support = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorText : owner.HelperText;
                if (!string.IsNullOrEmpty(support))
                {
                    float supSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var sf = new Font(owner.Font.FontFamily, supSize, FontStyle.Regular);
                    int h = TextRenderer.MeasureText(g, "Ag", sf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    extraBottom = h + 4;
                }
            }
            catch { }

            // measure text
            int textW = 0;
            int textH = owner.Font.Height;
            try
            {
                using var g = owner.CreateGraphics();
                var sz = TextRenderer.MeasureText(g, string.IsNullOrEmpty(owner.Text) ? "" : owner.Text, owner.Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                textW = sz.Width;
                textH = sz.Height;
            }
            catch { }

            // icon width contribution
            int iconW = 0;
            if (!string.IsNullOrEmpty(owner.LeadingIconPath) || !string.IsNullOrEmpty(owner.LeadingImagePath))
                iconW += owner.IconSize + (owner.IconPadding * 2) + 8;
            if (!string.IsNullOrEmpty(owner.TrailingIconPath) || !string.IsNullOrEmpty(owner.TrailingImagePath) || owner.ShowClearButton)
                iconW += owner.IconSize + (owner.IconPadding * 2) + 8;

            int minWidth = 60;
            int width = Math.Max(minWidth, textW + iconW + padW + border + shadow);
            int height = Math.Max(textH + padH + border + shadow + extraTop + extraBottom, owner.Height);

            return new Size(width, height);
        }

        private static Color GetEffectiveBackColor(Base.BaseControl owner)
        {
            if (!owner.Enabled) return owner.DisabledBackColor;
            if (owner.IsPressed && owner.CanBePressed) return owner.PressedBackColor;
            if (owner.IsHovered && owner.CanBeHovered) return owner.HoverBackColor;
            if (owner.Focused && owner.CanBeFocused) return owner.FocusBackColor;
            if (owner.IsSelected && owner.CanBeSelected) return owner.SelectedBackColor;
            return owner.BackColor;
        }

        private void DrawBorders(Graphics g, Base.BaseControl owner)
        {
            Color borderColor = owner.BorderColor;
            if (!owner.Enabled) borderColor = owner.DisabledBorderColor;
            else if (owner.Focused) borderColor = owner.FocusBorderColor;
            else if (owner.IsHovered) borderColor = owner.HoverBorderColor;
            else if (owner.IsPressed) borderColor = owner.PressedBorderColor;
            else if (owner.IsSelected) borderColor = owner.SelectedBorderColor;

            if (owner.ShowAllBorders && owner.BorderThickness > 0)
            {
                using var pen = new Pen(borderColor, owner.BorderThickness) { DashStyle = owner.BorderDashStyle, Alignment = PenAlignment.Inset };
                if (owner.IsRounded && owner.BorderRadius > 0)
                {
                    using var path = ControlPaintHelper.GetRoundedRectPath(_borderRect, owner.BorderRadius);
                    g.DrawPath(pen, path);
                }
                else
                {
                    g.DrawRectangle(pen, _borderRect);
                }
            }
            else if (owner.BorderThickness > 0)
            {
                using var pen = new Pen(borderColor, owner.BorderThickness) { DashStyle = owner.BorderDashStyle };
                if (owner.ShowTopBorder)
                    g.DrawLine(pen, _borderRect.Left, _borderRect.Top, _borderRect.Right, _borderRect.Top);
                if (owner.ShowBottomBorder)
                    g.DrawLine(pen, _borderRect.Left, _borderRect.Bottom, _borderRect.Right, _borderRect.Bottom);
                if (owner.ShowLeftBorder)
                    g.DrawLine(pen, _borderRect.Left, _borderRect.Top, _borderRect.Left, _borderRect.Bottom);
                if (owner.ShowRightBorder)
                    g.DrawLine(pen, _borderRect.Right, _borderRect.Top, _borderRect.Right, _borderRect.Bottom);
            }
        }

        private void DrawShadow(Graphics g, Base.BaseControl owner)
        {
            int shadowDepth = Math.Max(1, owner.ShadowOffset / 2);
            int maxLayers = Math.Min(shadowDepth, 6);

            Rectangle shadowRect = new Rectangle(
                _drawingRect.X + owner.ShadowOffset,
                _drawingRect.Y + owner.ShadowOffset,
                _drawingRect.Width,
                _drawingRect.Height);

            for (int i = 1; i <= maxLayers; i++)
            {
                float layerOpacityFactor = (float)(maxLayers - i + 1) / maxLayers;
                float finalOpacity = owner.ShadowOpacity * layerOpacityFactor * 0.6f;
                int layerAlpha = Math.Max(5, (int)(255 * finalOpacity));

                Color layerShadowColor = Color.FromArgb(layerAlpha, owner.ShadowColor);
                int spread = i - 1;
                Rectangle layerRect = new Rectangle(
                    shadowRect.X - spread,
                    shadowRect.Y - spread,
                    shadowRect.Width + (spread * 2),
                    shadowRect.Height + (spread * 2));

                using var shadowBrush = new SolidBrush(layerShadowColor);
                if (owner.IsRounded && owner.BorderRadius > 0)
                {
                    int shadowRadius = Math.Max(0, owner.BorderRadius + spread);
                    using var shadowPath = ControlPaintHelper.GetRoundedRectPath(layerRect, shadowRadius);
                    g.FillPath(shadowBrush, shadowPath);
                }
                else
                {
                    g.FillRectangle(shadowBrush, layerRect);
                }
            }
        }

        private void DrawLabelAndHelperNonMaterial(Graphics g, Base.BaseControl owner)
        {
            // Draw label text just above the top border
            if (!string.IsNullOrEmpty(owner.LabelText))
            {
                float labelSize = Math.Max(8f, owner.Font.Size - 1f);
                using var lf = new Font(owner.Font.FontFamily, labelSize, FontStyle.Regular);
                var labelHeight = TextRenderer.MeasureText(g, "Ag", lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                var labelRect = new Rectangle(_borderRect.Left + 6, Math.Max(0, _borderRect.Top - labelHeight - 2), Math.Max(10, _borderRect.Width - 12), labelHeight);
                Color labelColor = string.IsNullOrEmpty(owner.ErrorText) ? (owner.ForeColor) : owner.ErrorColor;
                TextRenderer.DrawText(g, owner.LabelText, lf, labelRect, labelColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }

            // Draw helper or error text just below the bottom border
            string supporting = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorText : owner.HelperText;
            if (!string.IsNullOrEmpty(supporting))
            {
                float supSize = Math.Max(8f, owner.Font.Size - 1f);
                using var sf = new Font(owner.Font.FontFamily, supSize, FontStyle.Regular);
                var supportHeight = TextRenderer.MeasureText(g, "Ag", sf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                var supportRect = new Rectangle(_borderRect.Left + 6, _borderRect.Bottom + 2, Math.Max(10, _borderRect.Width - 12), supportHeight);
                Color supportColor = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorColor : (owner.ForeColor);
                TextRenderer.DrawText(g, supporting, sf, supportRect, supportColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }
        }
    }
}
