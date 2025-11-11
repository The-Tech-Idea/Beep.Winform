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

        private void ConfigureForNumericStyle()
        {
            switch (_numericStyle)
            {
                case NumericStyle.Currency:
                    _displayMode = NumericUpDownDisplayMode.Currency;
                    _decimalPlaces = 2;
                    _thousandsSeparator = true;
                    break;

                case NumericStyle.Percentage:
                    _displayMode = NumericUpDownDisplayMode.Percentage;
                    _minimumValue = 0m;
                    _maximumValue = 100m;
                    _decimalPlaces = 1;
                    break;

                case NumericStyle.Integer:
                    _decimalPlaces = 0;
                    break;

                case NumericStyle.Decimal:
                    _decimalPlaces = 2;
                    break;

                case NumericStyle.Scientific:
                    _decimalPlaces = 4;
                    break;

                case NumericStyle.Phone:
                    _decimalPlaces = 0;
                    _allowNegative = false;
                    _minimumValue = 0m;
                    break;

                case NumericStyle.Rating:
                    _minimumValue = 0m;
                    _maximumValue = 5m;
                    _incrementValue = 1m;
                    _decimalPlaces = 0;
                    break;

                case NumericStyle.Progress:
                    _minimumValue = 0m;
                    _maximumValue = 100m;
                    _displayMode = NumericUpDownDisplayMode.Percentage;
                    _readOnly = true;
                    break;

                case NumericStyle.Temperature:
                    _decimalPlaces = 1;
                    _suffix = "°C";
                    break;

                case NumericStyle.Slider:
                case NumericStyle.Dial:
                    _showSpinButtons = false;
                    break;

                case NumericStyle.Inline:
                case NumericStyle.Display:
                    _showSpinButtons = false;
                    break;

                default:
                    // Standard and other styles use default settings
                    break;
            }
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

        #region Mask Preset Configuration
        private void ApplyMaskPreset()
        {
            if (_maskPreset == NumericMaskPreset.None)
            {
                _maskConfig = null;
                return;
            }

            // Get configuration from preset
            _maskConfig = _maskPreset == NumericMaskPreset.Custom
                ? CreateCustomMaskConfig()
                : NumericMaskConfig.FromPreset(_maskPreset);

            // Apply configuration to control properties
            if (_maskConfig.MinValue.HasValue)
                MinimumValue = _maskConfig.MinValue.Value;

            if (_maskConfig.MaxValue.HasValue)
                MaximumValue = _maskConfig.MaxValue.Value;

            DecimalPlaces = _maskConfig.DecimalPlaces;
            AllowNegative = _maskConfig.AllowNegative;

            if (!string.IsNullOrEmpty(_maskConfig.Unit))
                Unit = _maskConfig.Unit;

            // Trigger validation and formatting
            UpdateDisplayText();
        }

        private NumericMaskConfig CreateCustomMaskConfig()
        {
            return new NumericMaskConfig
            {
                Preset = NumericMaskPreset.Custom,
                MaskPattern = _customMask,
                DecimalPlaces = _decimalPlaces,
                AllowNegative = _allowNegative,
                ShowIcon = false
            };
        }

        /// <summary>
        /// Gets the icon path for the current mask preset
        /// </summary>
        public string GetMaskIconPath()
        {
            return _maskConfig?.IconPath ?? "";
        }

        /// <summary>
        /// Checks if the current mask preset should show an icon
        /// </summary>
        public bool ShouldShowMaskIcon()
        {
            return _maskConfig?.ShowIcon == true && !string.IsNullOrEmpty(_maskConfig.IconPath);
        }
        #endregion

        #region Validation Helpers
        /// <summary>
        /// Validates if a value is within the allowed range
        /// </summary>
        public bool ValidateRange(decimal value, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (value < _minimumValue)
            {
                errorMessage = $"Value must be greater than or equal to {_minimumValue}";
                RangeExceeded?.Invoke(this, new RangeExceededEventArgs
                {
                    AttemptedValue = value,
                    MinimumValue = _minimumValue,
                    MaximumValue = _maximumValue,
                    IsMaximumExceeded = false
                });
                return false;
            }

            if (value > _maximumValue)
            {
                errorMessage = $"Value must be less than or equal to {_maximumValue}";
                RangeExceeded?.Invoke(this, new RangeExceededEventArgs
                {
                    AttemptedValue = value,
                    MinimumValue = _minimumValue,
                    MaximumValue = _maximumValue,
                    IsMaximumExceeded = true
                });
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates if input matches the expected format/mask
        /// </summary>
        public bool ValidateFormat(string input, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                errorMessage = "Input cannot be empty";
                return false;
            }

            // Remove formatting characters for validation
            string cleanInput = input.Replace(",", "").Replace("$", "").Replace("%", "").Trim();

            if (!decimal.TryParse(cleanInput, out decimal parsedValue))
            {
                errorMessage = "Invalid numeric format";
                InvalidInput?.Invoke(this, new InvalidInputEventArgs
                {
                    Input = input,
                    Reason = "Not a valid number"
                });
                return false;
            }

            // Validate mask pattern if present
            if (_maskConfig != null && !string.IsNullOrEmpty(_maskConfig.MaskPattern))
            {
                bool isValid = ValidateMaskPattern(input, _maskConfig.MaskPattern);
                if (!isValid)
                {
                    errorMessage = $"Input must match format: {_maskConfig.MaskPattern}";
                    FormatValidation?.Invoke(this, new FormatValidationEventArgs
                    {
                        Input = input,
                        IsValid = false,
                        ExpectedFormat = _maskConfig.MaskPattern
                    });
                }
                return isValid;
            }

            return true;
        }

        /// <summary>
        /// Validates if value matches the increment step
        /// </summary>
        public bool ValidateStep(decimal value, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (_incrementValue <= 0)
                return true;

            decimal remainder = (value - _minimumValue) % _incrementValue;
            
            if (remainder != 0 && Math.Abs(remainder) > 0.0000001m)
            {
                errorMessage = $"Value must be in increments of {_incrementValue}";
                StepValidationFailed?.Invoke(this, EventArgs.Empty);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates numeric input character by character
        /// </summary>
        public bool ValidateNumericInput(char input, string currentText, int selectionStart)
        {
            // Allow control keys
            if (char.IsControl(input))
                return true;

            // Check for valid numeric characters
            if (!char.IsDigit(input) && input != '.' && input != '-')
            {
                InvalidInput?.Invoke(this, new InvalidInputEventArgs
                {
                    Input = input.ToString(),
                    Reason = "Only numeric characters allowed"
                });
                return false;
            }

            // Check decimal point rules
            if (input == '.')
            {
                if (_decimalPlaces == 0 || currentText.Contains('.'))
                    return false;
            }

            // Check negative sign rules
            if (input == '-')
            {
                if (!_allowNegative || selectionStart != 0 || currentText.Contains('-'))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Validates decimal places in the input
        /// </summary>
        public bool ValidateDecimalPlaces(string input, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!input.Contains('.'))
                return true;

            int decimalIndex = input.IndexOf('.');
            int actualDecimalPlaces = input.Length - decimalIndex - 1;

            if (actualDecimalPlaces > _decimalPlaces)
            {
                errorMessage = $"Maximum {_decimalPlaces} decimal places allowed";
                FormatValidation?.Invoke(this, new FormatValidationEventArgs
                {
                    Input = input,
                    IsValid = false,
                    ExpectedFormat = $"Max {_decimalPlaces} decimal places"
                });
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates input against a mask pattern
        /// </summary>
        private bool ValidateMaskPattern(string input, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return true;

            // Simple pattern validation - # represents a digit
            string cleanInput = input.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")
                                     .Replace(".", "").Replace(":", "").Replace("/", "");
            string cleanPattern = pattern.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")
                                        .Replace(".", "").Replace(":", "").Replace("/", "");

            int expectedDigits = cleanPattern.Count(c => c == '#');
            int actualDigits = cleanInput.Count(char.IsDigit);

            return actualDigits <= expectedDigits;
        }

        /// <summary>
        /// Comprehensive validation of value
        /// </summary>
        public bool ValidateValue(decimal value, out string errorMessage)
        {
            // Range validation
            if (!ValidateRange(value, out errorMessage))
                return false;

            // Step validation
            if (!ValidateStep(value, out errorMessage))
                return false;

            // Fire validation event
            var validatingArgs = new NumericValueValidatingEventArgs
            {
                OldValue = _value,
                NewValue = value,
                ValidationMessage = errorMessage
            };
            ValueValidatingEx?.Invoke(this, validatingArgs);

            if (validatingArgs.Cancel)
            {
                errorMessage = validatingArgs.ValidationMessage ?? "Validation cancelled";
                return false;
            }

            // Fire validated event
            var validatedArgs = new NumericValueValidatedEventArgs
            {
                OldValue = _value,
                NewValue = value,
                IsValid = true,
                ValidationMessage = string.Empty
            };
            ValueValidated?.Invoke(this, validatedArgs);

            return true;
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
