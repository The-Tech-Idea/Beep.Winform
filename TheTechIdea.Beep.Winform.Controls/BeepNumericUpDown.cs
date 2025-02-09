using System;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Numeric UpDown")]
    [Description("A custom numeric up-down control with Beep theming.")]
    public class BeepNumericUpDown : BeepControl
    {
        private BeepButton _decrementButton;
        private BeepButton _incrementButton;
        private BeepTextBox _valueTextBox;

        private decimal _minimumValue = 0m;
        private decimal _maximumValue = 1000m;
        private decimal _incrementValue = 1m;
        private decimal _value = 0m;

        public BeepNumericUpDown()
        {
            // Initialize default size
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.Size = DefaultSize;
            Margin = new Padding(0);
            // Initialize child controls
            InitializeControls();
        }

        protected override Size DefaultSize => new Size(120, _valueTextBox?.PreferredHeight ?? 30);

        private void InitializeControls()
        {
            // Decrement Button (Left)
            _decrementButton = new BeepButton
            {
                Text = "-",
                IsFramless = true,
                AutoSize = true
            };
            _decrementButton.Click += DecrementButton_Click;

            // Increment Button (Right)
            _incrementButton = new BeepButton
            {
                Text = "+",
                IsFramless = true,

                AutoSize = true
            };
            _incrementButton.Click += IncrementButton_Click;

            // Value TextBox (Middle)
            _valueTextBox = new BeepTextBox
            {
                TextAlignment = HorizontalAlignment.Center,
                IsFramless= true,
                OnlyDigits = true
            };
            _valueTextBox.TextChanged += ValueTextBox_TextChanged;

            // Add controls to the parent
            Controls.Add(_decrementButton);
            Controls.Add(_valueTextBox);
            Controls.Add(_incrementButton);

            // Update layout
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            int buttonWidth = 15; // Fixed width for the + and - buttons.
            int padding = 1;      // Padding between controls and the edges of DrawingRect.

            // DrawingRect is assumed to be up-to-date (thanks to your UpdateDrawingRect() call).

            // Calculate the available height for the controls, subtracting top and bottom padding.
            int elementHeight = DrawingRect.Height - 2 * padding;
            // Determine the Y coordinate so the controls are vertically padded.
            int elementY = DrawingRect.Top + padding;

            // Layout the Decrement Button (left side).
            _decrementButton.Location = new Point(DrawingRect.Left + padding, elementY);
            _decrementButton.Size = new Size(buttonWidth, elementHeight);

            // Layout the Increment Button (right side).
            _incrementButton.Location = new Point(DrawingRect.Right - buttonWidth - padding, elementY);
            _incrementButton.Size = new Size(buttonWidth, elementHeight);

            // Layout the Value TextBox (center).
            int textBoxX = _decrementButton.Right + padding;
            int textBoxWidth = Math.Max(0, DrawingRect.Width - (buttonWidth * 2) - (3 * padding));
            _valueTextBox.Location = new Point(textBoxX, elementY);
            _valueTextBox.Size = new Size(textBoxWidth, elementHeight);

            // Optionally, prevent the control from being resized manually by fixing its size.
            this.MaximumSize = this.MinimumSize = new Size(this.Width, this.Height);
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }

        #region "Properties"
        [Browsable(true)]
        [Category("Numeric Settings")]
        [Description("Specifies the minimum numeric value.")]
        public decimal MinimumValue
        {
            get => _minimumValue;
            set
            {
                _minimumValue = value;
                if (_value < _minimumValue)
                    _value = _minimumValue;
                UpdateTextBox();
            }
        }

        [Browsable(true)]
        [Category("Numeric Settings")]
        [Description("Specifies the maximum numeric value.")]
        public decimal MaximumValue
        {
            get => _maximumValue;
            set
            {
                _maximumValue = value;
                if (_value > _maximumValue)
                    _value = _maximumValue;
                UpdateTextBox();
            }
        }

        [Browsable(true)]
        [Category("Numeric Settings")]
        [Description("Specifies the amount by which the value changes.")]
        public decimal IncrementValue
        {
            get => _incrementValue;
            set => _incrementValue = value;
        }

        [Browsable(true)]
        [Category("Numeric Settings")]
        [Description("Specifies the current numeric value.")]
        public decimal Value
        {
            get => _value;
            set
            {
                if (value < _minimumValue || value > _maximumValue)
                    throw new ArgumentOutOfRangeException($"Value must be between {_minimumValue} and {_maximumValue}.");
                _value = value;
                UpdateTextBox();
            }
        }
        #endregion

        #region "Event Handlers"
        private void DecrementButton_Click(object sender, EventArgs e)
        {
            if (_value > _minimumValue)
            {
                _value -= _incrementValue;
                UpdateTextBox();
            }
        }

        private void IncrementButton_Click(object sender, EventArgs e)
        {
            if (_value < _maximumValue)
            {
                _value += _incrementValue;
                UpdateTextBox();
            }
        }

        private void ValueTextBox_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(_valueTextBox.Text, out decimal newValue))
            {
                if (newValue >= _minimumValue && newValue <= _maximumValue)
                {
                    _value = newValue;
                }
                else
                {
                    UpdateTextBox(); // Revert to the last valid value
                }
            }
            else
            {
                UpdateTextBox(); // Revert to the last valid value
            }
        }
        #endregion

        private void UpdateTextBox()
        {
            _valueTextBox.Text = _value.ToString();
        }
    }
}