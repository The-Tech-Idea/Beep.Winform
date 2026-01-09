using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Painters;

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

            int checkBoxSize = Math.Min(CheckBoxSize, Math.Min(rectangle.Width - Padding.Horizontal, rectangle.Height - Padding.Vertical));
            Rectangle checkBoxRect;
            Rectangle textRect = Rectangle.Empty;
            Size textSize = HideText || string.IsNullOrEmpty(Text) ? Size.Empty : TextRenderer.MeasureText(Text, TextFont);

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
                        checkBoxRect = new Rectangle(rectangle.X + Padding.Left,
                            rectangle.Y + (rectangle.Height - checkBoxSize) / 2,
                            checkBoxSize, checkBoxSize);
                        textRect = new Rectangle(checkBoxRect.Right + Spacing,
                            rectangle.Y + (rectangle.Height - textSize.Height) / 2,
                            textSize.Width, textSize.Height);
                        break;

                    case TextAlignment.Left:
                        textRect = new Rectangle(rectangle.X + Padding.Left,
                            rectangle.Y + (rectangle.Height - textSize.Height) / 2,
                            textSize.Width, textSize.Height);
                        checkBoxRect = new Rectangle(textRect.Right + Spacing,
                            rectangle.Y + (rectangle.Height - checkBoxSize) / 2,
                            checkBoxSize, checkBoxSize);
                        break;

                    case TextAlignment.Above:
                        textRect = new Rectangle(rectangle.X + (rectangle.Width - textSize.Width) / 2,
                            rectangle.Y + Padding.Top,
                            textSize.Width, textSize.Height);
                        checkBoxRect = new Rectangle(rectangle.X + (rectangle.Width - checkBoxSize) / 2,
                            textRect.Bottom + Spacing,
                            checkBoxSize, checkBoxSize);
                        break;

                    case TextAlignment.Below:
                        checkBoxRect = new Rectangle(rectangle.X + (rectangle.Width - checkBoxSize) / 2,
                            rectangle.Y + Padding.Top,
                            checkBoxSize, checkBoxSize);
                        textRect = new Rectangle(rectangle.X + (rectangle.Width - textSize.Width) / 2,
                            checkBoxRect.Bottom + Spacing,
                            textSize.Width, textSize.Height);
                        break;

                    default:
                        checkBoxRect = new Rectangle(rectangle.X, rectangle.Y, checkBoxSize, checkBoxSize);
                        break;
                }
            }

            // Create render options
            var renderOptions = new CheckBoxRenderOptions
            {
                Theme = _currentTheme,
                UseThemeColors = UseThemeColors,
                ControlStyle = ControlStyle,
                CheckBoxStyle = _checkBoxStyle,
                CheckBoxSize = checkBoxSize,
                Spacing = Spacing,
                Padding = Padding.Left,
                BorderRadius = CheckBoxStyleHelpers.GetRecommendedBorderRadius(_checkBoxStyle, ControlStyle),
                BorderWidth = CheckBoxStyleHelpers.GetRecommendedBorderWidth(_checkBoxStyle),
                CheckMarkThickness = CheckBoxStyleHelpers.GetRecommendedCheckMarkThickness(_checkBoxStyle),
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
                IsFocused = IsFocused,
                IsDisabled = !Enabled
            };

            // Use painter to draw checkbox
            _painter.PaintCheckBox(graphics, checkBoxRect, itemState, renderOptions);

            // Paint text if not hidden
            if (!HideText && !string.IsNullOrEmpty(Text) && !textRect.IsEmpty)
            {
                _painter.PaintText(graphics, textRect, Text, renderOptions);
            }
        }

        private void DrawForGrid(Graphics graphics, Rectangle rectangle)
        {
            if (_currentTheme == null)
            {
                _currentTheme = BeepThemesManager.GetDefaultTheme();
            }

            // Calculate checkbox position (centered or left-aligned based on text)
            int checkBoxSize = Math.Min(16, Math.Min(rectangle.Width, rectangle.Height) - 4); // Smaller for grid
            Rectangle checkBoxRect;

            if (HideText || string.IsNullOrEmpty(Text))
            {
                // Center the checkbox
                checkBoxRect = new Rectangle(
                    rectangle.X + (rectangle.Width - checkBoxSize) / 2,
                    rectangle.Y + (rectangle.Height - checkBoxSize) / 2,
                    checkBoxSize, checkBoxSize);
            }
            else
            {
                // Checkbox on left, text on right
                checkBoxRect = new Rectangle(
                    rectangle.X + 2,
                    rectangle.Y + (rectangle.Height - checkBoxSize) / 2,
                    checkBoxSize, checkBoxSize);
            }

            // Draw themed background with cached brush
            Color backColor = _state == CheckBoxState.Checked
                ? CheckBoxThemeHelpers.GetCheckedBackgroundColor(_currentTheme, UseThemeColors)
                : CheckBoxThemeHelpers.GetUncheckedBackgroundColor(_currentTheme, UseThemeColors);

            if (!_brushCache.TryGetValue(backColor, out SolidBrush backBrush))
            {
                backBrush = new SolidBrush(backColor);
                _brushCache[backColor] = backBrush;
            }

            graphics.FillRectangle(backBrush, checkBoxRect);

            // Draw themed border with cached pen
            Color borderColor = CheckBoxThemeHelpers.GetBorderColor(
                _currentTheme,
                UseThemeColors,
                _state == CheckBoxState.Checked,
                _state == CheckBoxState.Indeterminate);

            if (!_penCache.TryGetValue(borderColor, out Pen borderPen))
            {
                borderPen = new Pen(borderColor, 1);
                _penCache[borderColor] = borderPen;
            }

            graphics.DrawRectangle(borderPen, checkBoxRect);

            // Draw themed check mark based on state
            switch (_state)
            {
                case CheckBoxState.Checked:
                    DrawThemedCheckMark(graphics, checkBoxRect);
                    break;
                case CheckBoxState.Indeterminate:
                    DrawThemedIndeterminateMark(graphics, checkBoxRect);
                    break;
                case CheckBoxState.Unchecked:
                    // Already drawn background and border
                    break;
            }

            // Draw themed text if needed
            if (!HideText && !string.IsNullOrEmpty(Text))
            {
                Rectangle textRect = new Rectangle(
                    checkBoxRect.Right + 4,
                    rectangle.Y,
                    rectangle.Width - checkBoxRect.Width - 6,
                    rectangle.Height);

                // Use theme foreground color
                Color textColor = CheckBoxThemeHelpers.GetForegroundColor(
                    _currentTheme,
                    UseThemeColors);

                TextRenderer.DrawText(graphics, Text, TextFont, textRect, textColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }
        }

        // Add optimized themed drawing methods for grid
        private void DrawThemedCheckMark(Graphics graphics, Rectangle bounds)
        {
            Color checkColor = CheckBoxThemeHelpers.GetCheckMarkColor(
                _currentTheme,
                UseThemeColors);

            if (!_penCache.TryGetValue(checkColor, out Pen checkPen))
            {
                checkPen = new Pen(checkColor, 2);
                _penCache[checkColor] = checkPen;
            }

            // Optimized check mark points
            Point[] checkMarkPoints = new Point[]
            {
                new Point(bounds.X + bounds.Width / 4, bounds.Y + bounds.Height / 2),
                new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height * 3 / 4),
                new Point(bounds.X + bounds.Width * 3 / 4, bounds.Y + bounds.Height / 4)
            };

            graphics.DrawLines(checkPen, checkMarkPoints);
        }

        private void DrawThemedIndeterminateMark(Graphics graphics, Rectangle bounds)
        {
            Color indeterminateColor = CheckBoxThemeHelpers.GetIndeterminateMarkColor(
                _currentTheme,
                UseThemeColors);

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
            Size textSize = hasText ? TextRenderer.MeasureText(Text, Font) : Size.Empty;
            int checkBoxSize = CheckBoxSize;

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

            return new Size(Math.Max(width, 100), Math.Max(height, 30)); // Minimum size to prevent collapse
        }
        #endregion
    }
}
