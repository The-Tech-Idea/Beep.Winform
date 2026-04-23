using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    internal class ComboBoxPopupRow : Control
    {
        private ComboBoxPopupRowModel _model;
        private ComboBoxPopupHostProfile _profile = ComboBoxPopupHostProfile.OutlineDefault();
        private ComboBoxThemeTokens _themeTokens = ComboBoxThemeTokens.Fallback();
        private bool _isHovered;
        private bool _isPressed;
        private bool _isKeyboardFocused;

        // ── Hover transition animation ─────────────────────────────────────
        private readonly System.Windows.Forms.Timer _hoverTimer;
        private float _hoverProgress;      // 0.0 = normal, 1.0 = hover
        private float _hoverTarget;        // where we're animating to
        private const float HOVER_ANIMATION_STEP = 0.15f; // ~100ms at 16ms interval
        private const float HOVER_THRESHOLD = 0.01f;

        public event EventHandler<ComboBoxPopupRowModel> RowCommitted;

        public ComboBoxPopupRowModel Model => _model;

        public ComboBoxPopupRow()
        {
            DoubleBuffered = true;
            Height = 32;
            Margin = Padding.Empty;
            TabStop = true;

            _hoverTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _hoverTimer.Tick += OnHoverAnimationTick;
        }

        public void SetModel(ComboBoxPopupRowModel model)
        {
            _model = model;
            _isKeyboardFocused = model?.IsKeyboardFocused == true;
            Height = GetPreferredRowHeight(model, _profile);
            Invalidate();
        }

        public void ApplyProfile(ComboBoxPopupHostProfile profile)
        {
            _profile = profile ?? ComboBoxPopupHostProfile.OutlineDefault();
            Height = GetPreferredRowHeight(_model, _profile);
            Invalidate();
        }

        public void ApplyThemeTokens(ComboBoxThemeTokens tokens)
        {
            _themeTokens = tokens ?? ComboBoxThemeTokens.Fallback();
            Invalidate();
        }

        public void SetKeyboardFocused(bool focused)
        {
            if (_isKeyboardFocused == focused)
            {
                return;
            }

            _isKeyboardFocused = focused;
            Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            CommitRow();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovered = true;
            _hoverTarget = 1.0f;
            if (!_hoverTimer.Enabled)
                _hoverTimer.Start();
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            _isPressed = false;
            _hoverTarget = 0.0f;
            if (!_hoverTimer.Enabled)
                _hoverTimer.Start();
            Invalidate();
        }

        private void OnHoverAnimationTick(object sender, EventArgs e)
        {
            float step = _hoverTarget > _hoverProgress ? HOVER_ANIMATION_STEP : -HOVER_ANIMATION_STEP;
            _hoverProgress += step;
            if (Math.Abs(_hoverTarget - _hoverProgress) < HOVER_THRESHOLD)
            {
                _hoverProgress = _hoverTarget;
                _hoverTimer.Stop();
            }
            // Clamp
            _hoverProgress = Math.Max(0f, Math.Min(1f, _hoverProgress));
            Invalidate();
            if (_hoverProgress == _hoverTarget)
                _hoverTimer.Stop();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _isPressed = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_isPressed)
            {
                _isPressed = false;
                Invalidate();
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return keyData switch
            {
                Keys.Enter => true,
                Keys.Space => true,
                _ => base.IsInputKey(keyData)
            };
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                CommitRow();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            Rectangle bounds = ClientRectangle;
            if (bounds.Width <= 0 || bounds.Height <= 0 || _model == null)
            {
                return;
            }

            Color fore = _model.IsEnabled ? SystemColors.ControlText : SystemColors.GrayText;
            Color back = BackColor;
            Color border = Color.Transparent;
            Rectangle surfaceRect = bounds;
            if (_profile.RowHorizontalInset > 0 || _profile.RowVerticalInset > 0)
            {
                surfaceRect = Rectangle.Inflate(bounds, -_profile.RowHorizontalInset, -_profile.RowVerticalInset);
                if (surfaceRect.Width <= 2 || surfaceRect.Height <= 2)
                {
                    surfaceRect = bounds;
                }
            }

            if (_model.RowKind == ComboBoxPopupRowKind.GroupHeader)
            {
                back = _themeTokens.PopupGroupHeaderBack;
                fore = _themeTokens.PopupGroupHeaderFore;
            }
            else if (_model.RowKind == ComboBoxPopupRowKind.Separator)
            {
                using var sepPen = new Pen(_themeTokens.PopupSeparatorColor);
                int y = bounds.Top + (bounds.Height / 2);
                g.DrawLine(sepPen, bounds.Left + 8, y, bounds.Right - 8, y);
                return;
            }
            else if (_model.RowKind == ComboBoxPopupRowKind.EmptyState
                  || _model.RowKind == ComboBoxPopupRowKind.LoadingState
                  || _model.RowKind == ComboBoxPopupRowKind.NoResults)
            {
                DrawStateRow(g, bounds);
                return;
            }
            else if (!_model.IsEnabled)
            {
                back = _themeTokens.DisabledBackColor;
                border = _themeTokens.BorderColor;
                fore = _themeTokens.DisabledForeColor;
            }
            else if (_isPressed)
            {
                back = _themeTokens.PopupRowSelectedColor;
                border = _themeTokens.FocusBorderColor;
            }
            else if (_isHovered || _hoverProgress > 0f)
            {
                // Animate hover background blend
                Color normalBack = BackColor;
                if (_isKeyboardFocused || _model.IsKeyboardFocused)
                    normalBack = _themeTokens.PopupRowFocusColor;
                else if (_model.IsSelected || _model.RowKind == ComboBoxPopupRowKind.Selected)
                    normalBack = _themeTokens.PopupRowSelectedColor;

                back = BlendColor(normalBack, _themeTokens.PopupRowHoverColor, _hoverProgress);
                border = _hoverProgress > 0.5f ? _themeTokens.HoverBorderColor : Color.Transparent;
            }
            else if (_isKeyboardFocused || _model.IsKeyboardFocused)
            {
                back = _themeTokens.PopupRowFocusColor;
                border = _themeTokens.FocusBorderColor;
            }
            else if (_model.IsSelected || _model.RowKind == ComboBoxPopupRowKind.Selected)
            {
                back = _themeTokens.PopupRowSelectedColor;
                border = _themeTokens.OpenBorderColor;
            }

            if (_profile.UseCardRows && _model.RowKind != ComboBoxPopupRowKind.GroupHeader)
            {
                border = border == Color.Transparent ? _themeTokens.PopupSeparatorColor : border;
            }

            using (var backBrush = new SolidBrush(back))
            {
                g.FillRectangle(backBrush, surfaceRect);
            }
            if (border != Color.Transparent)
            {
                using var borderPen = new Pen(border);
                g.DrawRectangle(borderPen, surfaceRect.Left, surfaceRect.Top, surfaceRect.Width - 1, surfaceRect.Height - 1);
            }
            else if (_profile.ShowRowSeparators && _model.RowKind != ComboBoxPopupRowKind.GroupHeader)
            {
                using var sepPen = new Pen(_themeTokens.PopupSeparatorColor);
                g.DrawLine(sepPen, surfaceRect.Left + 8, surfaceRect.Bottom - 1, surfaceRect.Right - 8, surfaceRect.Bottom - 1);
            }

            var textRect = new Rectangle(surfaceRect.Left + 10, surfaceRect.Top, Math.Max(1, surfaceRect.Width - 20), surfaceRect.Height);
            var iconRect = Rectangle.Empty;
            if (!string.IsNullOrWhiteSpace(_model.ImagePath))
            {
                int imgSize = _profile.UseCircularImages ? 24 : 16;
                iconRect = new Rectangle(surfaceRect.Left + 8, surfaceRect.Top + Math.Max(2, (surfaceRect.Height - imgSize) / 2), imgSize, imgSize);
                textRect = new Rectangle(iconRect.Right + 8, surfaceRect.Top, Math.Max(1, surfaceRect.Right - iconRect.Right - 8), surfaceRect.Height);

                if (_profile.UseCircularImages)
                {
                    var state = g.Save();
                    using var clipPath = new System.Drawing.Drawing2D.GraphicsPath();
                    clipPath.AddEllipse(iconRect);
                    g.SetClip(clipPath, System.Drawing.Drawing2D.CombineMode.Intersect);
                    StyledImagePainter.Paint(g, iconRect, _model.ImagePath, BeepControlStyle.Minimal);
                    g.Restore(state);
                }
                else
                {
                    StyledImagePainter.Paint(g, iconRect, _model.ImagePath, BeepControlStyle.Minimal);
                }
            }

            if (_model.RowKind == ComboBoxPopupRowKind.CheckRow)
            {
                var checkRect = new Rectangle(surfaceRect.Left + 8, surfaceRect.Top + Math.Max(2, (surfaceRect.Height - 16) / 2), 16, 16);
                using var checkPen = new Pen(Color.FromArgb(120, 120, 120));
                g.DrawRectangle(checkPen, checkRect);
                if (_model.IsChecked)
                {
                    using var iconPath = new System.Drawing.Drawing2D.GraphicsPath();
                    iconPath.AddRectangle(checkRect);
                    StyledImagePainter.PaintWithTint(g, iconPath, SvgsUI.Check, _themeTokens.FocusBorderColor, 0.9f);
                }
                textRect = new Rectangle(checkRect.Right + 8, surfaceRect.Top, Math.Max(1, surfaceRect.Right - checkRect.Right - 8), surfaceRect.Height);
            }
            else if (_profile.ShowCheckmarkForSelected && (_model.IsSelected || _model.RowKind == ComboBoxPopupRowKind.Selected))
            {
                var checkRect = new Rectangle(surfaceRect.Right - 22, surfaceRect.Top + Math.Max(2, (surfaceRect.Height - 16) / 2), 16, 16);
                using var iconPath = new System.Drawing.Drawing2D.GraphicsPath();
                iconPath.AddRectangle(checkRect);
                StyledImagePainter.PaintWithTint(g, iconPath, SvgsUI.Check, _themeTokens.FocusBorderColor, 0.9f);
                textRect.Width = Math.Max(1, checkRect.Left - textRect.Left - 6);
            }

            if (!string.IsNullOrWhiteSpace(_model.TrailingValueText))
            {
                int reserved = DrawTrailingValueText(g, surfaceRect, _model.TrailingValueText);
                textRect.Width = Math.Max(1, textRect.Width - reserved);
            }

            if (!string.IsNullOrWhiteSpace(_model.TrailingText))
            {
                int reserved = DrawTrailingText(g, surfaceRect, _model.TrailingText);
                textRect.Width = Math.Max(1, textRect.Width - reserved);
            }

            bool renderSubTextBlock = !string.IsNullOrEmpty(_model.SubText) &&
                                      (_model.RowKind == ComboBoxPopupRowKind.WithSubText ||
                                       _model.RowKind == ComboBoxPopupRowKind.CheckRow ||
                                       _model.LayoutPreset == ComboBoxPopupRowLayoutPreset.ChecklistRich);

            if (renderSubTextBlock)
            {
                var labelFont = _themeTokens.LabelFont ?? Font;
                var subFont = _themeTokens.SubTextFont ?? Font;
                var titleRect = new Rectangle(textRect.Left, textRect.Top + 4, textRect.Width, Math.Max(1, (textRect.Height / 2) - 2));
                var subRect = new Rectangle(textRect.Left, titleRect.Bottom, textRect.Width, Math.Max(1, textRect.Height - titleRect.Height - 4));
                DrawTextWithHighlight(g, _model.Text ?? string.Empty, titleRect, labelFont, fore);
                TextRenderer.DrawText(g, _model.SubText, subFont, subRect, _themeTokens.PopupSubTextColor,
                    TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
            else
            {
                var labelFont = _themeTokens.LabelFont ?? Font;
                var text = _model.RowKind == ComboBoxPopupRowKind.GroupHeader
                    ? (_model.GroupName ?? _model.Text ?? string.Empty)
                    : (_model.Text ?? string.Empty);
                DrawTextWithHighlight(g, text, textRect, labelFont, fore);
            }
        }

        private void DrawStateRow(Graphics g, Rectangle bounds)
        {
            using var backBrush = new SolidBrush(_themeTokens.PopupBackColor);
            g.FillRectangle(backBrush, bounds);

            string iconPath;
            string text;
            switch (_model.RowKind)
            {
                case ComboBoxPopupRowKind.LoadingState:
                    iconPath = SvgsUI.CircleDot;
                    text = _model.Text ?? "Loading…";
                    break;
                case ComboBoxPopupRowKind.NoResults:
                    iconPath = SvgsUI.Search;
                    text = _model.Text ?? "No results";
                    break;
                default: // EmptyState
                    iconPath = SvgsUI.MoodEmpty;
                    text = _model.Text ?? "No items";
                    break;
            }

            Color fore = _themeTokens.DisabledForeColor;
            int iconSize = 18;
            Size textSize = TextRenderer.MeasureText(text, Font);
            int totalWidth = iconSize + 6 + textSize.Width;
            int x = bounds.Left + Math.Max(0, (bounds.Width - totalWidth) / 2);
            int iconY = bounds.Top + Math.Max(0, (bounds.Height - iconSize) / 2);

            var iconRect = new Rectangle(x, iconY, iconSize, iconSize);
            StyledImagePainter.PaintWithTint(g, iconRect, iconPath, fore, 0.7f);

            var textRect = new Rectangle(iconRect.Right + 6, bounds.Top, Math.Max(1, bounds.Right - iconRect.Right - 6), bounds.Height);
            TextRenderer.DrawText(g, text, Font, textRect, fore,
                TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        private void DrawTextWithHighlight(Graphics g, string text, Rectangle rect, Font font, Color fore)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            int start = _model?.MatchStart ?? -1;
            int length = _model?.MatchLength ?? 0;
            if (start < 0 || length <= 0 || start >= text.Length)
            {
                TextRenderer.DrawText(g, text, font, rect, fore,
                    TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                return;
            }

            if (start + length > text.Length)
            {
                length = text.Length - start;
            }

            string prefix = start > 0 ? text.Substring(0, start) : string.Empty;
            string match = text.Substring(start, length);
            string suffix = (start + length) < text.Length ? text.Substring(start + length) : string.Empty;

            using var normalBrush = new SolidBrush(fore);
            using var highlightBrush = new SolidBrush(_themeTokens.FocusBorderColor);
            using var sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };

            // Draw as three segments so only the match span is highlighted.
            float x = rect.Left;
            float y = rect.Top;
            float h = rect.Height;

            if (!string.IsNullOrEmpty(prefix))
            {
                g.DrawString(prefix, font, normalBrush, new RectangleF(x, y, rect.Width, h), sf);
                x += TextRenderer.MeasureText(g, prefix, font).Width;
            }

            if (!string.IsNullOrEmpty(match) && x < rect.Right)
            {
                g.DrawString(match, font, highlightBrush, new RectangleF(x, y, rect.Right - x, h), sf);
                x += TextRenderer.MeasureText(g, match, font).Width;
            }

            if (!string.IsNullOrEmpty(suffix) && x < rect.Right)
            {
                g.DrawString(suffix, font, normalBrush, new RectangleF(x, y, rect.Right - x, h), sf);
            }
        }

        private void CommitRow()
        {
            if (_model == null || !_model.IsEnabled || _model.RowKind == ComboBoxPopupRowKind.GroupHeader || _model.RowKind == ComboBoxPopupRowKind.Separator)
            {
                return;
            }

            if (_model.RowKind == ComboBoxPopupRowKind.CheckRow)
            {
                RowCommitted?.Invoke(this, ToggleCheckRow(_model));
                return;
            }

            RowCommitted?.Invoke(this, _model);
        }

        private static ComboBoxPopupRowModel ToggleCheckRow(ComboBoxPopupRowModel model)
        {
            return new ComboBoxPopupRowModel
            {
                SourceItem = model.SourceItem,
                RowKind = model.RowKind,
                Text = model.Text,
                SubText = model.SubText,
                TrailingText = model.TrailingText,
                TrailingValueText = model.TrailingValueText,
                ImagePath = model.ImagePath,
                GroupName = model.GroupName,
                LayoutPreset = model.LayoutPreset,
                IsSelected = !model.IsChecked,
                IsEnabled = model.IsEnabled,
                IsKeyboardFocused = model.IsKeyboardFocused,
                IsCheckable = model.IsCheckable,
                IsChecked = !model.IsChecked,
                ListIndex = model.ListIndex
            };
        }

        private static int GetPreferredRowHeight(ComboBoxPopupRowModel model, ComboBoxPopupHostProfile profile)
        {
            if (model == null)
            {
                return profile?.BaseRowHeight ?? 32;
            }

            return model.RowKind switch
            {
                ComboBoxPopupRowKind.WithSubText => 44,
                ComboBoxPopupRowKind.CheckRow when model.LayoutPreset == ComboBoxPopupRowLayoutPreset.ChecklistRich => 44,
                ComboBoxPopupRowKind.GroupHeader => profile?.GroupHeaderHeight ?? 28,
                ComboBoxPopupRowKind.Separator => 14,
                ComboBoxPopupRowKind.EmptyState => 36,
                ComboBoxPopupRowKind.LoadingState => 36,
                ComboBoxPopupRowKind.NoResults => 36,
                _ => profile?.BaseRowHeight ?? 32
            };
        }

        private int DrawTrailingText(Graphics g, Rectangle surfaceRect, string trailing)
        {
            if (string.IsNullOrWhiteSpace(trailing))
            {
                return 0;
            }

            var shortcutFont = _themeTokens.SubTextFont ?? Font;
            var shortcutColor = _themeTokens.PopupSubTextColor;
            int marginRight = 8;
            int horizontalPadding = 8;
            int measuredWidth = TextRenderer.MeasureText(g, trailing, shortcutFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Width;
            int boxWidth = measuredWidth + (horizontalPadding * 2);
            int boxHeight = Math.Max(18, shortcutFont.Height + 4);
            var boxRect = new Rectangle(
                surfaceRect.Right - boxWidth - marginRight,
                surfaceRect.Top + Math.Max(1, (surfaceRect.Height - boxHeight) / 2),
                boxWidth,
                boxHeight);

            TextRenderer.DrawText(
                g,
                trailing,
                shortcutFont,
                boxRect,
                shortcutColor,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);

            return boxWidth + marginRight + 4;
        }

        private int DrawTrailingValueText(Graphics g, Rectangle surfaceRect, string trailingValue)
        {
            if (string.IsNullOrWhiteSpace(trailingValue))
            {
                return 0;
            }

            var metricFont = _themeTokens.LabelFont ?? Font;
            var metricColor = _themeTokens.PopupSubTextColor;
            int marginRight = 8;
            int measuredWidth = TextRenderer.MeasureText(g, trailingValue, metricFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Width;
            int width = measuredWidth + 8;
            var rect = new Rectangle(
                surfaceRect.Right - width - marginRight,
                surfaceRect.Top,
                width,
                surfaceRect.Height);

            TextRenderer.DrawText(
                g,
                trailingValue,
                metricFont,
                rect,
                metricColor,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);

            return width + marginRight + 4;
        }

        /// <summary>
        /// Linearly blends between two colors by factor t (0.0–1.0).
        /// </summary>
        private static Color BlendColor(Color from, Color to, float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            int r = (int)(from.R + (to.R - from.R) * t);
            int g = (int)(from.G + (to.G - from.G) * t);
            int b = (int)(from.B + (to.B - from.B) * t);
            int a = (int)(from.A + (to.A - from.A) * t);
            return Color.FromArgb(a, r, g, b);
        }
    }
}
