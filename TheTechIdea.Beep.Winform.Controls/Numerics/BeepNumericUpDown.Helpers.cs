using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Numerics
{
    /// <summary>
    /// Partial class for BeepNumericUpDown helper methods and utilities
    /// </summary>
    public partial class BeepNumericUpDown
    {
        #region Value Management Helpers
        internal void IncrementValueInternal()
        {
            IncrementValueInternal(_incrementValue);
        }

        internal void IncrementValueInternal(decimal increment)
        {
            decimal newValue = _value + increment;
            
            if (_wrapValues && newValue > _maximumValue)
            {
                Value = _minimumValue;
            }
            else
            {
                Value = newValue;
                if (_value >= _maximumValue)
                {
                    MaximumReached?.Invoke(this, EventArgs.Empty);
                }
            }

            UpButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        internal void DecrementValueInternal()
        {
            DecrementValueInternal(_incrementValue);
        }

        internal void DecrementValueInternal(decimal decrement)
        {
            decimal newValue = _value - decrement;
            
            if (_wrapValues && newValue < _minimumValue)
            {
                Value = _maximumValue;
            }
            else
            {
                Value = newValue;
                if (_value <= _minimumValue)
                {
                    MinimumReached?.Invoke(this, EventArgs.Empty);
                }
            }

            DownButtonClicked?.Invoke(this, EventArgs.Empty);
        }

       
        #endregion

        #region Text Formatting Helpers
        internal string FormatValue(decimal value)
        {
            string valueStr;

            if (_decimalPlaces > 0)
            {
                valueStr = value.ToString($"N{_decimalPlaces}");
            }
            else
            {
                valueStr = _thousandsSeparator ? value.ToString("N0") : value.ToString();
            }

            // Apply display mode formatting
            string result = _displayMode switch
            {
                NumericUpDownDisplayMode.Percentage => $"{valueStr}%",
                NumericUpDownDisplayMode.Currency => $"${valueStr}",
                NumericUpDownDisplayMode.CustomUnit => $"{_prefix}{valueStr}{_suffix} {_unit}".Trim(),
                NumericUpDownDisplayMode.ProgressValue => $"{valueStr}%",
                _ => valueStr
            };

            return result;
        }

        private void UpdateDisplayText()
        {
            if (_textBox != null && _textBox.Visible)
            {
                _textBox.Text = FormatValue(_value);
            }
            Invalidate();
        }
        #endregion

        #region TextBox Management
        private void StartEditing()
        {
            if (_readOnly || _isEditing) return;

            _isEditing = true;

            if (_textBox == null)
            {
                _textBox = new TextBox
                {
                    BorderStyle = BorderStyle.None,
                    BackColor = _currentTheme?.TextBoxBackColor ?? SystemColors.Window,
                    ForeColor = _currentTheme?.TextBoxForeColor ?? SystemColors.ControlText,
                    Font = Font
                };

                _textBox.KeyDown += TextBox_KeyDown;
                _textBox.LostFocus += TextBox_LostFocus;
                Controls.Add(_textBox);
            }

            // Position the textbox
            int padding = 4;
            int buttonWidth = _showSpinButtons ? GetButtonWidthForSize(_buttonSize) : 0;
            int textBoxLeft = _showSpinButtons ? padding + buttonWidth : padding;
            int textWidth = Width - 2 * padding;

            if (_showSpinButtons)
            {
                textWidth -= 2 * buttonWidth;
            }

            _textBox.Bounds = new Rectangle(textBoxLeft, padding, textWidth, Height - 2 * padding);
            _textBox.Text = FormatValue(_value);
            _textBox.Visible = true;
            _textBox.Focus();

            if (_selectAllOnFocus)
            {
                _textBox.SelectAll();
            }

            Invalidate();
        }

        private void EndEditing(bool acceptValue)
        {
            if (!_isEditing) return;

            if (acceptValue && _textBox != null)
            {
                if (decimal.TryParse(_textBox.Text.Replace("%", "").Replace("$", "").Trim(), out decimal newValue))
                {
                    Value = newValue;
                }
                else
                {
                    ValueValidationFailed?.Invoke(this, EventArgs.Empty);
                }
            }

            _isEditing = false;

            if (_textBox != null)
            {
                _textBox.Visible = false;
            }

            Invalidate();
            Focus();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                EndEditing(true);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                EndEditing(false);
                e.Handled = true;
            }
        }

        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            EndEditing(true);
        }
        #endregion

        #region Geometry Helpers
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
        #endregion
    }

    #region Helper Classes
    /// <summary>
    /// Event args for value validation
    /// </summary>
  
    #endregion
}
