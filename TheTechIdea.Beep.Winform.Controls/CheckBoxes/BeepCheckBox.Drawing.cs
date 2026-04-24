using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Painters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes
{
    public partial class BeepCheckBox<T>
    {
        #region Painting
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            // Use simplified drawing for grid mode
            if (GridMode)
            {
                DrawForGrid(graphics, rectangle);
                return;
            }

            // Check if we need to redraw (state tracking)
            bool needsRedraw = CheckIfStateChanged(rectangle);
            if (!needsRedraw)
            {
                // If nothing changed, we could potentially skip redrawing entirely
                // For now, we'll still draw but with cached resources
            }

            if (_currentTheme == null)
            {
                _currentTheme = BeepThemesManager.GetDefaultTheme();
            }

            // Scale dimensions
            int baseCheckBoxSize = CheckBoxSize;
            int scaledCheckBoxSize = DpiScalingHelper.ScaleValue(baseCheckBoxSize, this);
            int scaledSpacing = DpiScalingHelper.ScaleValue(Spacing, this);
            int scaledPaddingLeft = DpiScalingHelper.ScaleValue(Padding.Left, this);
            int scaledPaddingTop = DpiScalingHelper.ScaleValue(Padding.Top, this);

            int checkBoxSize = Math.Min(scaledCheckBoxSize, Math.Min(rectangle.Width - Padding.Horizontal, rectangle.Height - Padding.Vertical));
            Rectangle checkBoxRect;
            Rectangle textRect = Rectangle.Empty;

            // Ensure font is scaled (managed by helper, but good to ensure here if using local)
            // But we use TextFont property which should be set via ApplyTheme correctly.
            // Let's rely on CheckBoxFontHelpers updates in ApplyTheme.

            Size textSize = HideText || string.IsNullOrEmpty(Text) ? Size.Empty : TextRenderer.MeasureText(graphics, Text, TextFont);

            if (HideText)
            {
                checkBoxRect = new Rectangle(
                    rectangle.X + (rectangle.Width - checkBoxSize) / 2,
                    rectangle.Y + (rectangle.Height - checkBoxSize) / 2,
                    checkBoxSize, checkBoxSize);
            }
            else
            {
                switch (TextAlignRelativeToCheckBox)
                {
                    case TextAlignment.Right:
                        checkBoxRect = new Rectangle(rectangle.X + scaledPaddingLeft,
                            rectangle.Y + (rectangle.Height - checkBoxSize) / 2,
                            checkBoxSize, checkBoxSize);
                        textRect = new Rectangle(
                            checkBoxRect.Right + scaledSpacing,
                            rectangle.Y + scaledPaddingTop,
                            Math.Max(0, rectangle.Right - (checkBoxRect.Right + scaledSpacing) - Padding.Right),
                            Math.Max(0, rectangle.Height - Padding.Vertical));
                        break;

                    case TextAlignment.Left:
                        int checkBoxX = rectangle.Right - Padding.Right - checkBoxSize;
                        checkBoxRect = new Rectangle(checkBoxX,
                            rectangle.Y + (rectangle.Height - checkBoxSize) / 2,
                            checkBoxSize, checkBoxSize);
                        textRect = new Rectangle(
                            rectangle.X + scaledPaddingLeft,
                            rectangle.Y + scaledPaddingTop,
                            Math.Max(0, checkBoxRect.Left - scaledSpacing - (rectangle.X + scaledPaddingLeft)),
                            Math.Max(0, rectangle.Height - Padding.Vertical));
                        break;

                    case TextAlignment.Above:
                        int aboveTextWidth = Math.Max(0, rectangle.Width - Padding.Horizontal);
                        textRect = new Rectangle(rectangle.X + Padding.Left,
                            rectangle.Y + scaledPaddingTop,
                            aboveTextWidth, textSize.Height);
                        checkBoxRect = new Rectangle(rectangle.X + (rectangle.Width - checkBoxSize) / 2,
                            textRect.Bottom + scaledSpacing,
                            checkBoxSize, checkBoxSize);
                        break;

                    case TextAlignment.Below:
                        checkBoxRect = new Rectangle(rectangle.X + (rectangle.Width - checkBoxSize) / 2,
                            rectangle.Y + scaledPaddingTop,
                            checkBoxSize, checkBoxSize);
                        int belowTextWidth = Math.Max(0, rectangle.Width - Padding.Horizontal);
                        textRect = new Rectangle(rectangle.X + Padding.Left,
                            checkBoxRect.Bottom + scaledSpacing,
                            belowTextWidth, textSize.Height);
                        break;

                    default:
                        checkBoxRect = new Rectangle(rectangle.X, rectangle.Y, checkBoxSize, checkBoxSize);
                        break;
                }
            }

            _lastCheckBoxRect = checkBoxRect;
            _lastTextRect = textRect;

            // Create render options
            var renderOptions = new CheckBoxRenderOptions
            {
                Theme = _currentTheme,
                UseThemeColors = UseThemeColors,
                ControlStyle = ControlStyle,
                CheckBoxStyle = _checkBoxStyle,
                CheckBoxSize = checkBoxSize,
                Spacing = scaledSpacing,
                Padding = scaledPaddingLeft,
                BorderRadius = DpiScalingHelper.ScaleValue(CheckBoxStyleHelpers.GetRecommendedBorderRadius(_checkBoxStyle, ControlStyle), this),
                BorderWidth = DpiScalingHelper.ScaleValue(CheckBoxStyleHelpers.GetRecommendedBorderWidth(_checkBoxStyle), this),
                CheckMarkThickness = DpiScalingHelper.ScaleValue(CheckBoxStyleHelpers.GetRecommendedCheckMarkThickness(_checkBoxStyle), this),
                GlyphSizeRatio = _checkBoxStyle == CheckBoxStyle.Switch ? 0.5f : 0.62f,
                CheckIconPath = string.IsNullOrWhiteSpace(ImagePath) ? CheckIconPath : ImagePath,
                IndeterminateIconPath = IndeterminateIconPath,
                TextFont = TextFont,
                Text = Text,
                HideText = HideText,
                TextAlignment = TextAlignRelativeToCheckBox
            };

            // Create item state
            var itemState = new CheckBoxItemState
            {
                IsChecked = _state == CheckBoxState.Checked,
                IsIndeterminate = _state == CheckBoxState.Indeterminate,
                IsHovered = IsHovered,
                IsFocused = IsFocused && _keyboardFocusVisible,
                IsDisabled = !Enabled
            };

            // Use painter to draw checkbox
            _painter.PaintCheckBox(graphics, checkBoxRect, itemState, renderOptions);

            // Paint text if not hidden
            if (!HideText && !string.IsNullOrEmpty(Text) && !textRect.IsEmpty)
            {
                _painter.PaintText(graphics, textRect, Text, renderOptions);
            }

            _lastDrawnState = _state;
            _lastDrawnText = Text;
            _lastDrawnRect = rectangle;
            _stateChanged = false;
        }

        private void DrawForGrid(Graphics graphics, Rectangle rectangle)
        {
            if (_currentTheme == null)
            {
                _currentTheme = BeepThemesManager.GetDefaultTheme();
            }

            int baseSize = CheckBoxSize;
            int checkBoxSize = DpiScalingHelper.ScaleValue(Math.Min(16, baseSize), this);
            checkBoxSize = Math.Min(checkBoxSize, Math.Min(rectangle.Width, rectangle.Height) - 4);
            Rectangle checkBoxRect;

            if (HideText || string.IsNullOrEmpty(Text))
            {
                checkBoxRect = new Rectangle(
                    rectangle.X + (rectangle.Width - checkBoxSize) / 2,
                    rectangle.Y + (rectangle.Height - checkBoxSize) / 2,
                    checkBoxSize, checkBoxSize);
            }
            else
            {
                int scaledSpacing = DpiScalingHelper.ScaleValue(Spacing, this);
                checkBoxRect = new Rectangle(
                    rectangle.X + 2,
                    rectangle.Y + (rectangle.Height - checkBoxSize) / 2,
                    checkBoxSize, checkBoxSize);
            }

            Color backColor = _state == CheckBoxState.Checked
                ? CheckBoxThemeHelpers.GetCheckedBackgroundColor(_currentTheme, UseThemeColors)
                : CheckBoxThemeHelpers.GetUncheckedBackgroundColor(_currentTheme, UseThemeColors);

            if (!Enabled)
            {
                backColor = ColorUtils.ShiftLuminance(backColor, 0.25f);
            }

            if (!_brushCache.TryGetValue(backColor, out SolidBrush backBrush))
            {
                backBrush = new SolidBrush(backColor);
                _brushCache[backColor] = backBrush;
            }

            graphics.FillRectangle(backBrush, checkBoxRect);

            Color borderColor = CheckBoxThemeHelpers.GetBorderColor(
                _currentTheme,
                UseThemeColors,
                _state == CheckBoxState.Checked,
                _state == CheckBoxState.Indeterminate);

            if (!Enabled)
            {
                borderColor = ColorUtils.ShiftLuminance(borderColor, 0.30f);
            }

            if (!_penCache.TryGetValue(borderColor, out Pen borderPen))
            {
                borderPen = new Pen(borderColor, 1);
                _penCache[borderColor] = borderPen;
            }

            graphics.DrawRectangle(borderPen, checkBoxRect);

            switch (_state)
            {
                case CheckBoxState.Checked:
                    DrawThemedCheckMark(graphics, checkBoxRect, !Enabled);
                    break;
                case CheckBoxState.Indeterminate:
                    DrawThemedIndeterminateMark(graphics, checkBoxRect, !Enabled);
                    break;
            }

            if (!HideText && !string.IsNullOrEmpty(Text))
            {
                Rectangle textRect = new Rectangle(
                    checkBoxRect.Right + 4,
                    rectangle.Y,
                    rectangle.Width - checkBoxRect.Width - 6,
                    rectangle.Height);

                Color textColor = CheckBoxThemeHelpers.GetForegroundColor(
                    _currentTheme,
                    UseThemeColors);

                if (!Enabled)
                {
                    textColor = ColorUtils.ShiftLuminance(textColor, 0.35f);
                }

                TextRenderer.DrawText(graphics, Text, TextFont, textRect, textColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }
        }

        // Add optimized themed drawing methods for grid
        private void DrawThemedCheckMark(Graphics graphics, Rectangle bounds, bool isDisabled = false)
        {
            Color checkColor = CheckBoxThemeHelpers.GetCheckMarkColor(
                _currentTheme,
                UseThemeColors);

            if (isDisabled)
            {
                checkColor = ColorUtils.ShiftLuminance(checkColor, 0.35f);
            }

            if (!_penCache.TryGetValue(checkColor, out Pen checkPen))
            {
                checkPen = new Pen(checkColor, 2);
                _penCache[checkColor] = checkPen;
            }

            Point[] checkMarkPoints = new Point[]
            {
                new Point(bounds.X + bounds.Width / 4, bounds.Y + bounds.Height / 2),
                new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height * 3 / 4),
                new Point(bounds.X + bounds.Width * 3 / 4, bounds.Y + bounds.Height / 4)
            };

            graphics.DrawLines(checkPen, checkMarkPoints);
        }

        private void DrawThemedIndeterminateMark(Graphics graphics, Rectangle bounds, bool isDisabled = false)
        {
            Color indeterminateColor = CheckBoxThemeHelpers.GetIndeterminateMarkColor(
                _currentTheme,
                UseThemeColors);

            if (isDisabled)
            {
                indeterminateColor = ColorUtils.ShiftLuminance(indeterminateColor, 0.35f);
            }

            if (!_brushCache.TryGetValue(indeterminateColor, out SolidBrush indeterminateBrush))
            {
                indeterminateBrush = new SolidBrush(indeterminateColor);
                _brushCache[indeterminateColor] = indeterminateBrush;
            }

            Rectangle indeterminateRect = new Rectangle(
                bounds.X + bounds.Width / 4,
                bounds.Y + bounds.Height / 4,
                bounds.Width / 2,
                bounds.Height / 2);

            graphics.FillRectangle(indeterminateBrush, indeterminateRect);
        }

        // State change detection
        private bool CheckIfStateChanged(Rectangle rectangle)
        {
            bool changed = _stateChanged ||
                           _lastDrawnState != _state ||
                           _lastDrawnText != Text ||
                           _lastDrawnRect != rectangle;

            return changed;
        }

        /// <summary>
        /// DrawContent override - called by BaseControl
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            Paint(g, DrawingRect);
        }

        /// <summary>
        /// Main paint function - centralized painting logic
        /// </summary>
        private void Paint(Graphics g, Rectangle bounds)
        {
            // Ensure painter updates the layout and DrawingRect
            UpdateDrawingRect();

            // Now draw the checkbox content
            Draw(g, bounds);
        }

        private void DrawAlignedText(Graphics g, string text, Font font, Color color, Rectangle textRect)
        {
            TextRenderer.DrawText(
                g,
                text,
                font,
                textRect,
                color,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            bool hasText = !HideText && !string.IsNullOrEmpty(Text);
            Size textSize = hasText ? TextRenderer.MeasureText(Text, TextFont) : Size.Empty;
            int checkBoxSize = CheckBoxSize;
            int minimumTarget = DpiScalingHelper.ScaleValue(_minimumHitTargetSize, this);

            int width, height;

            if (!hasText)
            {
                // Use CheckBoxSize only when no text, fitting the grid cell
                width = Padding.Left + checkBoxSize + Padding.Right;
                height = Padding.Top + checkBoxSize + Padding.Bottom;
            }
            else
            {
                switch (TextAlignRelativeToCheckBox)
                {
                    case TextAlignment.Right:
                    case TextAlignment.Left:
                        width = Padding.Left + checkBoxSize + Spacing + textSize.Width + Padding.Right;
                        height = Padding.Top + Math.Max(checkBoxSize, textSize.Height) + Padding.Bottom;
                        break;
                    case TextAlignment.Above:
                    case TextAlignment.Below:
                        width = Padding.Left + Math.Max(checkBoxSize, textSize.Width) + Padding.Right;
                        height = Padding.Top + checkBoxSize + Spacing + textSize.Height + Padding.Bottom;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(TextAlignRelativeToCheckBox));
                }
            }

            return new Size(Math.Max(width, Math.Max(minimumTarget, 100)), Math.Max(height, minimumTarget));
        }
        #endregion
    }
}
