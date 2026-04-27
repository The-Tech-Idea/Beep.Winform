using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Chips.Helpers;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using Size = System.Drawing.Size;

namespace TheTechIdea.Beep.Winform.Controls.Chips
{
    public partial class BeepMultiChipGroup
    {
      
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var availableRect = DrawingRect;
            var effectiveTheme = _renderOptions.Theme ?? GetEffectiveTheme();
            _renderOptions.Theme = effectiveTheme;
            _painter?.UpdateTheme(effectiveTheme);

            if (UseThemeColors && effectiveTheme != null)
            {
                _painter?.RenderGroupBackground(g, availableRect, _renderOptions);
            }
            else
            {
                BeepStyling.PaintStyleBackground(g, DrawingRect, ControlStyle);
            }
            if (!string.IsNullOrEmpty(_titleText) && _titleHeight > 0)
            {
                int hInset = _chipPadding;
                var titleRect = new Rectangle(
                    availableRect.X + hInset,
                    availableRect.Y,
                    availableRect.Width - hInset * 2,
                    _titleHeight);
                var titleFlags = GetTextFormatFlags(_titleAlignment);
                TextRenderer.DrawText(g, _titleText, _titleFont, titleRect, _titleColor, titleFlags);
            }

            DrawUtilityRow(g, effectiveTheme);

            _closeRects.Clear();
            for (int i = 0; i < _chips.Count; i++)
            {
                var chip = _chips[i];
                var state = new ChipVisualState 
                { 
                    IsSelected = chip.IsSelected, 
                    IsHovered = chip.IsHovered, 
                    Variant = chip.Variant, 
                    Size = chip.Size, 
                    Color = chip.Color, 
                    IsFocused = (_focusedIndex == i && Focused),
                    IsDisabled = chip.IsDisabled
                };
                Rectangle closeRect = Rectangle.Empty;
                var graphicsState = g.Save();
                try
                {
                    if (_painter != null)
                    {
                        _painter.RenderChip(g, chip.Item, chip.Bounds, state, _renderOptions, out closeRect);
                    }
                }
                catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException || ex is NullReferenceException)
                {
                    System.Diagnostics.Debug.WriteLine($"BeepMultiChipGroup.RenderChip[{i}] failed: {ex.Message}");
                }
                finally
                {
                    g.Restore(graphicsState);
                }
                if (!closeRect.IsEmpty) _closeRects[i] = closeRect;

                if (state.IsFocused)
                {
                    var focusColor = effectiveTheme?.FocusIndicatorColor ?? ColorUtils.MapSystemColor(SystemColors.Highlight);
                    var pen = (System.Drawing.Pen)PaintersFactory.GetPen(focusColor, 2f).Clone();
                    try
                    {
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                        var r = Rectangle.Inflate(chip.Bounds, 3, 3);
                        g.DrawRectangle(pen, r);
                    }
                    finally
                    {
                        pen.Dispose();
                    }
                }

                if (_renderOptions.GetBadgeCount != null)
                {
                    var cnt = _renderOptions.GetBadgeCount(chip.Item);
                    if (cnt.HasValue)
                    {
                        string s = cnt.Value.ToString();
                        var sz = TextRenderer.MeasureText(g, s, _textFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine);
                        var br = new Rectangle(chip.Bounds.Right - (sz.Width + 12), chip.Bounds.Top - 8, sz.Width + 10, 16);
                        var badgeBack = effectiveTheme?.NavigationSelectedBackColor ?? ColorUtils.MapSystemColor(SystemColors.Highlight);
                        var badgeFore = effectiveTheme?.NavigationSelectedForeColor ?? ColorUtils.MapSystemColor(SystemColors.HighlightText);
                        var bg = PaintersFactory.GetSolidBrush(badgeBack);
                        var fg = PaintersFactory.GetSolidBrush(badgeFore);
                        g.FillRectangle(bg, br);
                        TextRenderer.DrawText(g, s, _textFont, br, badgeFore, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    }
                }
            }

            DrawDragIndicators(g, effectiveTheme);
            DrawRipples(g);
            DrawAnimations(g);
            DrawTooltip(g, effectiveTheme);
        }

        private void DrawUtilityRow(Graphics g, IBeepTheme effectiveTheme)
        {
            if (!_showUtilityRow || _selectAllRect.IsEmpty || _clearAllRect.IsEmpty)
            {
                return;
            }

            var borderColor = effectiveTheme?.ButtonBorderColor ?? ColorUtils.MapSystemColor(SystemColors.ControlDark);
            var backColor = effectiveTheme?.ButtonBackColor ?? ColorUtils.MapSystemColor(SystemColors.ControlLight);
            var foreColor = effectiveTheme?.ButtonForeColor ?? ForeColor;
            var hoverBack = effectiveTheme?.ButtonHoverBackColor ?? ShiftLuminance(backColor, effectiveTheme?.IsDarkTheme == true ? -0.05f : 0.05f);

            var selectBack = _selectAllRect.Contains(_lastInteractionPoint) ? hoverBack : backColor;
            var clearBack = _clearAllRect.Contains(_lastInteractionPoint) ? hoverBack : backColor;

            using (var selectBrush = new SolidBrush(selectBack))
            using (var clearBrush = new SolidBrush(clearBack))
            using (var borderPen = new Pen(borderColor, 1f))
            {
                g.FillRectangle(selectBrush, _selectAllRect);
                g.DrawRectangle(borderPen, _selectAllRect);
                g.FillRectangle(clearBrush, _clearAllRect);
                g.DrawRectangle(borderPen, _clearAllRect);
            }

            TextRenderer.DrawText(
                g,
                "Select All",
                _textFont ?? Font,
                _selectAllRect,
                foreColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);

            TextRenderer.DrawText(
                g,
                "Clear All",
                _textFont ?? Font,
                _clearAllRect,
                foreColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        }

        private TextFormatFlags GetTextFormatFlags(ContentAlignment alignment)
        {
            var flags = TextFormatFlags.SingleLine | TextFormatFlags.PreserveGraphicsClipping;
            return alignment switch
            {
                ContentAlignment.TopLeft => flags | TextFormatFlags.Left | TextFormatFlags.Top,
                ContentAlignment.TopCenter => flags | TextFormatFlags.HorizontalCenter | TextFormatFlags.Top,
                ContentAlignment.TopRight => flags | TextFormatFlags.Right | TextFormatFlags.Top,
                ContentAlignment.MiddleLeft => flags | TextFormatFlags.Left | TextFormatFlags.VerticalCenter,
                ContentAlignment.MiddleCenter => flags | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ContentAlignment.MiddleRight => flags | TextFormatFlags.Right | TextFormatFlags.VerticalCenter,
                ContentAlignment.BottomLeft => flags | TextFormatFlags.Left | TextFormatFlags.Bottom,
                ContentAlignment.BottomCenter => flags | TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom,
                _ => flags | TextFormatFlags.Right | TextFormatFlags.Bottom,
            };
        }

        private static Color ShiftLuminance(Color color, float amount)
        {
            float h, s, l;
            ColorToHsl(color, out h, out s, out l);
            l = Math.Max(0, Math.Min(1, l + amount));
            return ColorFromHsl(h, s, l);
        }

        private static void ColorToHsl(Color color, out float h, out float s, out float l)
        {
            float r = color.R / 255.0f;
            float g = color.G / 255.0f;
            float b = color.B / 255.0f;
            float min = Math.Min(r, Math.Min(g, b));
            float max = Math.Max(r, Math.Max(g, b));
            float delta = max - min;
            l = (max + min) / 2.0f;
            if (delta == 0) { h = 0; s = 0; }
            else
            {
                s = l < 0.5f ? delta / (max + min) : delta / (2.0f - max - min);
                if (r == max) h = (g - b) / delta;
                else if (g == max) h = 2.0f + (b - r) / delta;
                else h = 4.0f + (r - g) / delta;
                h /= 6.0f;
                if (h < 0) h += 1.0f;
            }
        }

        private static Color ColorFromHsl(float h, float s, float l)
        {
            float r, g, b;
            if (s == 0) { r = g = b = l; }
            else
            {
                float q = l < 0.5f ? l * (1.0f + s) : l + s - l * s;
                float p = 2.0f * l - q;
                r = HueToRgb(p, q, h + 1.0f / 3.0f);
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - 1.0f / 3.0f);
            }
            return Color.FromArgb(255, Math.Max(0, Math.Min(255, (int)(r * 255))), Math.Max(0, Math.Min(255, (int)(g * 255))), Math.Max(0, Math.Min(255, (int)(b * 255))));
        }

        private static float HueToRgb(float p, float q, float t)
        {
            if (t < 0) t += 1.0f;
            if (t > 1) t -= 1.0f;
            if (t < 1.0f / 6.0f) return p + (q - p) * 6.0f * t;
            if (t < 1.0f / 2.0f) return q;
            if (t < 2.0f / 3.0f) return p + (q - p) * (2.0f / 3.0f - t) * 6.0f;
            return p;
        }

        private void DrawDragIndicators(Graphics g, IBeepTheme theme)
        {
            if (!_isDragging || _dropInsertIndex < 0) return;

            var indicatorColor = theme?.FocusIndicatorColor ?? ColorUtils.MapSystemColor(SystemColors.Highlight);
            var pen = new Pen(indicatorColor, 2f);
            try
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                int insertX = GetDropIndicatorX(_dropInsertIndex);
                g.DrawLine(pen, insertX, DrawingRect.Top, insertX, DrawingRect.Bottom);
            }
            finally
            {
                pen.Dispose();
            }

            if (_dragChipIndex >= 0 && _dragChipIndex < _chips.Count)
            {
                var dragChip = _chips[_dragChipIndex];
                var ghostRect = new Rectangle(
                    _dragGhostLocation.X - dragChip.Bounds.Width / 2,
                    _dragGhostLocation.Y - dragChip.Bounds.Height / 2,
                    dragChip.Bounds.Width,
                    dragChip.Bounds.Height);

                using var path = CreateRoundedPath(ghostRect, DpiScalingHelper.ScaleValue(_chipCornerRadius, _renderOptions.DpiScale));
                using var brush = new SolidBrush(Color.FromArgb(180, indicatorColor));
                g.FillPath(brush, path);
                using var borderPen = new Pen(indicatorColor, 1.5f);
                g.DrawPath(borderPen, path);

                var textRect = Rectangle.Inflate(ghostRect, -DpiScalingHelper.ScaleValue(8, _renderOptions.DpiScale), -DpiScalingHelper.ScaleValue(2, _renderOptions.DpiScale));
                TextRenderer.DrawText(g, dragChip.Item.Text, _textFont ?? Font, textRect, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
            }
        }

        private int GetDropIndicatorX(int insertIndex)
        {
            if (insertIndex >= _chips.Count)
            {
                return _chips.Count > 0 ? _chips[_chips.Count - 1].Bounds.Right + _chipPadding / 2 : DrawingRect.Left;
            }
            if (insertIndex == 0) return _chips[0].Bounds.Left - _chipPadding / 2;
            var prevChip = _chips[insertIndex - 1];
            var nextChip = _chips[insertIndex];
            return (prevChip.Bounds.Right + nextChip.Bounds.Left) / 2;
        }

        private void DrawRipples(Graphics g)
        {
            foreach (var ripple in _activeRipples)
            {
                float radius = Math.Max(ripple.Bounds.Width, ripple.Bounds.Height) * ripple.Progress;
                int alpha = (int)(80 * (1f - ripple.Progress));
                using var brush = new SolidBrush(Color.FromArgb(alpha, ripple.Color));
                g.FillEllipse(brush, 
                    ripple.Center.X - radius, 
                    ripple.Center.Y - radius, 
                    radius * 2, 
                    radius * 2);
            }
        }

        private void DrawAnimations(Graphics g)
        {
            foreach (var anim in _chipAnimations)
            {
                if (anim.ChipIndex < 0 || anim.ChipIndex >= _chips.Count) continue;
                var chip = _chips[anim.ChipIndex];
                float t = EaseInOutCubic(anim.Progress);

                if (anim.IsAdd)
                {
                    float scale = t;
                    float alpha = t;
                    var center = new Point(
                        anim.Bounds.X + anim.Bounds.Width / 2,
                        anim.Bounds.Y + anim.Bounds.Height / 2);
                    var scaledRect = new Rectangle(
                        (int)(center.X - anim.Bounds.Width / 2 * scale),
                        (int)(center.Y - anim.Bounds.Height / 2 * scale),
                        (int)(anim.Bounds.Width * scale),
                        (int)(anim.Bounds.Height * scale));

                    using var path = CreateRoundedPath(scaledRect, DpiScalingHelper.ScaleValue(_chipCornerRadius, _renderOptions.DpiScale));
                    using var brush = new SolidBrush(Color.FromArgb((int)(alpha * 255), chip.IsSelected ? GetSelectedChipColor() : GetDefaultChipColor()));
                    g.FillPath(brush, path);
                }
                else
                {
                    float alpha = 1f - t;
                    float shrink = 1f - t * 0.3f;
                    var center = new Point(
                        anim.Bounds.X + anim.Bounds.Width / 2,
                        anim.Bounds.Y + anim.Bounds.Height / 2);
                    var shrunkRect = new Rectangle(
                        (int)(center.X - anim.Bounds.Width / 2 * shrink),
                        (int)(center.Y - anim.Bounds.Height / 2 * shrink),
                        (int)(anim.Bounds.Width * shrink),
                        (int)(anim.Bounds.Height * shrink));

                    using var path = CreateRoundedPath(shrunkRect, DpiScalingHelper.ScaleValue(_chipCornerRadius, _renderOptions.DpiScale));
                    using var brush = new SolidBrush(Color.FromArgb((int)(alpha * 255), chip.IsSelected ? GetSelectedChipColor() : GetDefaultChipColor()));
                    g.FillPath(brush, path);
                }
            }
        }

        private void DrawTooltip(Graphics g, IBeepTheme theme)
        {
            if (!_tooltipShowing || _tooltipChipIndex < 0 || _tooltipChipIndex >= _chips.Count) return;
            var chip = _chips[_tooltipChipIndex];
            if (string.IsNullOrEmpty(chip.Item.Text)) return;

            var tooltipText = chip.Item.Text;
            var textSize = TextRenderer.MeasureText(g, tooltipText, _textFont ?? Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine);
            int padding = DpiScalingHelper.ScaleValue(8, _renderOptions.DpiScale);
            int tooltipWidth = textSize.Width + padding * 2;
            int tooltipHeight = textSize.Height + padding * 2;
            int pointerHeight = DpiScalingHelper.ScaleValue(6, _renderOptions.DpiScale);

            int tooltipX = chip.Bounds.Left + (chip.Bounds.Width - tooltipWidth) / 2;
            tooltipX = Math.Max(DrawingRect.Left, Math.Min(DrawingRect.Right - tooltipWidth, tooltipX));
            int tooltipY = chip.Bounds.Bottom + DpiScalingHelper.ScaleValue(4, _renderOptions.DpiScale);

            if (tooltipY + tooltipHeight + pointerHeight > DrawingRect.Bottom)
            {
                tooltipY = chip.Bounds.Top - tooltipHeight - pointerHeight - DpiScalingHelper.ScaleValue(4, _renderOptions.DpiScale);
            }

            var tooltipRect = new Rectangle(tooltipX, tooltipY, tooltipWidth, tooltipHeight);
            var backColor = theme?.IsDarkTheme == true ? Color.FromArgb(240, 45, 45, 45) : Color.FromArgb(240, 250, 250, 250);
            var foreColor = theme?.IsDarkTheme == true ? Color.White : Color.Black;
            var borderColor = theme?.IsDarkTheme == true ? Color.FromArgb(60, 60, 60) : Color.FromArgb(200, 200, 200);

            using var path = CreateTooltipPath(tooltipRect, pointerHeight, chip.Bounds);
            using var brush = new SolidBrush(backColor);
            g.FillPath(brush, path);
            using var pen = new Pen(borderColor, 1f);
            g.DrawPath(pen, path);

            var textRect = new Rectangle(tooltipX + padding, tooltipY + padding, tooltipWidth - padding * 2, tooltipHeight - padding * 2);
            TextRenderer.DrawText(g, tooltipText, _textFont ?? Font, textRect, foreColor, TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
        }

        private System.Drawing.Drawing2D.GraphicsPath CreateTooltipPath(Rectangle bodyRect, int pointerHeight, Rectangle chipBounds)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int cornerRadius = DpiScalingHelper.ScaleValue(4, _renderOptions.DpiScale);
            int pointerWidth = DpiScalingHelper.ScaleValue(10, _renderOptions.DpiScale);
            int pointerX = chipBounds.Left + chipBounds.Width / 2 - pointerWidth / 2;
            pointerX = Math.Max(bodyRect.Left, Math.Min(bodyRect.Right - pointerWidth, pointerX));

            bool showAbove = bodyRect.Top < chipBounds.Top;

            if (showAbove)
            {
                path.AddLine(bodyRect.Left + cornerRadius, bodyRect.Top, bodyRect.Right - cornerRadius, bodyRect.Top);
                path.AddArc(bodyRect.Right - cornerRadius * 2, bodyRect.Top, cornerRadius * 2, cornerRadius * 2, 270, 90);
                path.AddLine(bodyRect.Right, bodyRect.Top + cornerRadius, bodyRect.Right, bodyRect.Bottom - cornerRadius);
                path.AddArc(bodyRect.Right - cornerRadius * 2, bodyRect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
                path.AddLine(bodyRect.Right - cornerRadius, bodyRect.Bottom, pointerX + pointerWidth, bodyRect.Bottom);
                path.AddLine(pointerX + pointerWidth, bodyRect.Bottom, pointerX + pointerWidth / 2, bodyRect.Bottom + pointerHeight);
                path.AddLine(pointerX + pointerWidth / 2, bodyRect.Bottom + pointerHeight, pointerX, bodyRect.Bottom);
                path.AddLine(pointerX, bodyRect.Bottom, bodyRect.Left + cornerRadius, bodyRect.Bottom);
                path.AddArc(bodyRect.Left, bodyRect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
                path.AddLine(bodyRect.Left, bodyRect.Bottom - cornerRadius, bodyRect.Left, bodyRect.Top + cornerRadius);
                path.AddArc(bodyRect.Left, bodyRect.Top, cornerRadius * 2, cornerRadius * 2, 180, 90);
            }
            else
            {
                path.AddLine(bodyRect.Left + cornerRadius, bodyRect.Top, pointerX, bodyRect.Top);
                path.AddLine(pointerX, bodyRect.Top, pointerX + pointerWidth / 2, bodyRect.Top - pointerHeight);
                path.AddLine(pointerX + pointerWidth / 2, bodyRect.Top - pointerHeight, pointerX + pointerWidth, bodyRect.Top);
                path.AddLine(pointerX + pointerWidth, bodyRect.Top, bodyRect.Right - cornerRadius, bodyRect.Top);
                path.AddArc(bodyRect.Right - cornerRadius * 2, bodyRect.Top, cornerRadius * 2, cornerRadius * 2, 270, 90);
                path.AddLine(bodyRect.Right, bodyRect.Top + cornerRadius, bodyRect.Right, bodyRect.Bottom - cornerRadius);
                path.AddArc(bodyRect.Right - cornerRadius * 2, bodyRect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
                path.AddLine(bodyRect.Right - cornerRadius, bodyRect.Bottom, bodyRect.Left + cornerRadius, bodyRect.Bottom);
                path.AddArc(bodyRect.Left, bodyRect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
                path.AddLine(bodyRect.Left, bodyRect.Bottom - cornerRadius, bodyRect.Left, bodyRect.Top + cornerRadius);
                path.AddArc(bodyRect.Left, bodyRect.Top, cornerRadius * 2, cornerRadius * 2, 180, 90);
            }

            path.CloseFigure();
            return path;
        }

        private System.Drawing.Drawing2D.GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int r = Math.Max(0, Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2));
            if (r <= 0) { path.AddRectangle(rect); return path; }
            int d = r * 2;
            path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static float EaseInOutCubic(float t)
        {
            return t < 0.5f ? 4f * t * t * t : 1f - (float)Math.Pow(-2f * t + 2f, 3) / 2f;
        }

        private Color GetSelectedChipColor()
        {
            var theme = GetEffectiveTheme();
            return theme?.ButtonSelectedBackColor ?? Color.RoyalBlue;
        }

        private Color GetDefaultChipColor()
        {
            var theme = GetEffectiveTheme();
            return theme?.ButtonBackColor ?? Color.White;
        }
    }
}
