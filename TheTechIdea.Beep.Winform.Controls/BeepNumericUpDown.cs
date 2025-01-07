using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// A numeric input control that extends BeepControl and implements IBeepUIComponent.
    /// </summary>
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Numeric UpDown")]
    [Description("A numeric up-down control with Beep theming and IBeepUIComponent support.")]
    public class BeepNumericUpDown : BeepControl
    {
        private NumericUpDown numericUpDownControl;

        public BeepNumericUpDown()
        {
            // Ensure default size if none has been specified
            if (Width <= 0 || Height <= 0)
            {
                Width = 100;
                Height = 30;
            }
            InitializeNumericUpDown();
        }

        private void InitializeNumericUpDown()
        {
            numericUpDownControl = new NumericUpDown
            {
                Anchor= AnchorStyles.Top|AnchorStyles.Bottom| AnchorStyles.Left| AnchorStyles.Right,
                Minimum = _minimumValue,
                Maximum = _maximumValue,
                Increment = _incrementValue,
                Value = _defaultValue,
                DecimalPlaces = _decimalPlaces,
                ReadOnly = _readOnly
            };
            UpdateDrawingRect();
            // set locationa and size inside on DrawingRect
            numericUpDownControl.Location = new Point(DrawingRect.X, DrawingRect.Y);
            numericUpDownControl.Size = new Size(DrawingRect.Width, DrawingRect.Height);
            // Sync the base Text property whenever the numeric value changes
            numericUpDownControl.ValueChanged += (s, e) => Text = numericUpDownControl.Value.ToString();

            Controls.Add(numericUpDownControl);
        }

        #region "Numeric Properties"

        private decimal _minimumValue = 0m;
        [Browsable(true)]
        [Category("Numeric Settings")]
        [Description("Specifies the minimum numeric value.")]
        public decimal MinimumValue
        {
            get => _minimumValue;
            set
            {
                _minimumValue = value;
                if (numericUpDownControl != null)
                    numericUpDownControl.Minimum = value;
            }
        }

        private decimal _maximumValue = 100m;
        [Browsable(true)]
        [Category("Numeric Settings")]
        [Description("Specifies the maximum numeric value.")]
        public decimal MaximumValue
        {
            get => _maximumValue;
            set
            {
                _maximumValue = value;
                if (numericUpDownControl != null)
                    numericUpDownControl.Maximum = value;
            }
        }

        private decimal _incrementValue = 1m;
        [Browsable(true)]
        [Category("Numeric Settings")]
        [Description("Specifies the amount by which the value changes.")]
        public decimal IncrementValue
        {
            get => _incrementValue;
            set
            {
                _incrementValue = value;
                if (numericUpDownControl != null)
                    numericUpDownControl.Increment = value;
            }
        }

        private decimal _defaultValue = 0m;
        [Browsable(true)]
        [Category("Numeric Settings")]
        [Description("Specifies the default numeric value.")]
        public decimal DefaultValue
        {
            get => _defaultValue;
            set
            {
                _defaultValue = value;
                if (numericUpDownControl != null)
                    numericUpDownControl.Value = value;
            }
        }

        private int _decimalPlaces;
        [Browsable(true)]
        [Category("Numeric Settings")]
        [Description("Specifies the number of decimal places to display.")]
        public int DecimalPlaces
        {
            get => _decimalPlaces;
            set
            {
                _decimalPlaces = value;
                if (numericUpDownControl != null)
                    numericUpDownControl.DecimalPlaces = value;
            }
        }

        private bool _readOnly;
        [Browsable(true)]
        [Category("Numeric Settings")]
        [Description("Determines whether the user can modify the value via the keyboard.")]
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                _readOnly = value;
                if (numericUpDownControl != null)
                    numericUpDownControl.ReadOnly = value;
            }
        }

        #endregion "Numeric Properties"

        #region "Overridden Text Property"

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The text associated with the numeric value.")]
        public override string Text
        {
            get => numericUpDownControl?.Value.ToString() ?? string.Empty;
            set
            {
                if (numericUpDownControl == null) return;

                if (decimal.TryParse(value, out decimal result))
                {
                    if (result < MinimumValue || result > MaximumValue)
                        throw new ArgumentOutOfRangeException(
                            $"Value '{value}' is out of range. Must be between {MinimumValue} and {MaximumValue}.");

                    numericUpDownControl.Value = result;
                }
                else
                {
                    throw new FormatException($"Invalid numeric format: '{value}'.");
                }
            }
        }

        #endregion "Overridden Text Property"

        #region "IBeepUIComponent Implementation"

        /// <summary>
        /// Validates the numeric value based on any business rules or constraints.
        /// </summary>
        /// <param name="message">An output parameter for validation result messages.</param>
        /// <returns>True if valid, otherwise false.</returns>
        public bool ValidateData(out string message)
        {
            // Example: Simple range validation. Adjust based on your requirements.
            if (numericUpDownControl.Value < MinimumValue || numericUpDownControl.Value > MaximumValue)
            {
                message = $"Value must be between {MinimumValue} and {MaximumValue}.";
                return false;
            }

            // If additional custom rules exist, apply them here or call EntityHelper if you prefer.
            message = "Valid";
            return true;
        }

        /// <summary>
        /// Applies the current theme to the numeric control.
        /// </summary>
        public override void ApplyTheme()
        {
            // Use base theming logic from BeepControl
            base.ApplyTheme();

            if (numericUpDownControl != null && _currentTheme != null)
            {
                numericUpDownControl.ForeColor = _currentTheme.PrimaryTextColor;
                numericUpDownControl.BackColor = _currentTheme.TextBoxBackColor;
               // numericUpDownControl.BorderStyle = BorderStyle.FixedSingle;

                // Optionally, set the font if desired
                numericUpDownControl.Font = _currentTheme.GetBlockTextFont();
            }
        }

        #endregion "IBeepUIComponent Implementation"
    }
}
