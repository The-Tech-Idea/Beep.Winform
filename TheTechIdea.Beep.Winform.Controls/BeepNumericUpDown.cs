using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

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
            int buttonWidth = 15; // Width of the buttons
            int padding = 1;      // Padding between controls

            // Set the control's height based on the text box's preferred height
            int controlHeight = _valueTextBox.PreferredHeight + 2 * padding;
            this.Height = controlHeight;
            UpdateDrawingRect();
            // Compute the vertical alignment to keep everything centered in DrawingRect.
            int verticalCenter = DrawingRect.Top + (DrawingRect.Height - _valueTextBox.PreferredHeight) / 2;
            // Decrement button (left)
            _decrementButton.Location = new Point(DrawingRect.Left + padding, verticalCenter);
            _decrementButton.Size = new Size(buttonWidth, _valueTextBox.PreferredHeight);

            // Increment button (right)
            _incrementButton.Location = new Point(DrawingRect.Right - buttonWidth - padding, verticalCenter);
            _incrementButton.Size = new Size(buttonWidth, _valueTextBox.PreferredHeight);

            // Value TextBox (center)
            _valueTextBox.Location = new Point(_decrementButton.Right + padding, verticalCenter);
            _valueTextBox.Size = new Size(
                Math.Max(0, DrawingRect.Width - _decrementButton.Width - _incrementButton.Width - 3 * padding),
                _valueTextBox.PreferredHeight
            );

            // Prevent further manual resizing of the control
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