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
                // Paint background based on selected Style
                BeepStyling.PaintStyleBackground(g, DrawingRect, ControlStyle);
            }
            if (!string.IsNullOrEmpty(_titleText) && _titleHeight > 0)
            {
                // Indent title by chipPadding to align with chip grid and utility row
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
            for (int i =0; i < _chips.Count; i++)
            {
                var chip = _chips[i];
                var state = new ChipVisualState { IsSelected = chip.IsSelected, IsHovered = chip.IsHovered, Variant = chip.Variant, Size = chip.Size, Color = chip.Color, IsFocused = (_focusedIndex == i && Focused) };
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

                // Focus ring
                if (state.IsFocused)
                {
                    var focusColor = effectiveTheme?.FocusIndicatorColor ?? ColorUtils.MapSystemColor(SystemColors.Highlight);
                    var pen = (System.Drawing.Pen)PaintersFactory.GetPen(focusColor,2f).Clone();
                    try
                    {
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                        var r = Rectangle.Inflate(chip.Bounds,3,3);
                        g.DrawRectangle(pen, r);
                    }
                    finally
                    {
                        pen.Dispose();
                    }
                }

                // Badge/counter (right small pill)
                if (_renderOptions.GetBadgeCount != null)
                {
                    var cnt = _renderOptions.GetBadgeCount(chip.Item);
                    if (cnt.HasValue)
                    {
                        string s = cnt.Value.ToString();
                        var sz = TextRenderer.MeasureText(g, s, _textFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine);
                        var br = new Rectangle(chip.Bounds.Right - (sz.Width +12), chip.Bounds.Top -8, sz.Width +10,16);
                        var badgeBack = effectiveTheme?.NavigationSelectedBackColor ?? ColorUtils.MapSystemColor(SystemColors.Highlight);
                        var badgeFore = effectiveTheme?.NavigationSelectedForeColor ?? ColorUtils.MapSystemColor(SystemColors.HighlightText);
                        var bg = PaintersFactory.GetSolidBrush(badgeBack);
                        var fg = PaintersFactory.GetSolidBrush(badgeFore);
                        g.FillRectangle(bg, br);
                        TextRenderer.DrawText(g, s, _textFont, br, badgeFore, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    }
                }
            }
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
    }
}
