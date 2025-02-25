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
            // Enable smooth resizing
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.Size = DefaultSize;
            Margin = new Padding(0);
            // Initialize child controls
            InitializeControls();
        }

        protected override Size DefaultSize => new Size(100, _valueTextBox?.PreferredHeight ?? 20);

        private void InitializeControls()
        {
            // Decrement Button (Left)
            _decrementButton = new BeepButton
            {
                Text = "-",
                IsFrameless = true,
                AutoSize = false  // We'll size it manually
            };
            _decrementButton.Click += DecrementButton_Click;

            // Increment Button (Right)
            _incrementButton = new BeepButton
            {
                Text = "+",
                IsFrameless = true,
                AutoSize = false  // We'll size it manually
            };
            _incrementButton.Click += IncrementButton_Click;

            // Value TextBox (Center)
            _valueTextBox = new BeepTextBox
            {
                TextAlignment = HorizontalAlignment.Center,
                IsFrameless = true,
                OnlyDigits = true,
                // If your BeepTextBox supports vertical centering, set it here.
                // Otherwise, consider making the TextBox tall enough so that the text appears centered.
            };
            _valueTextBox.TextChanged += ValueTextBox_TextChanged;

            // Add controls to the parent
            Controls.Add(_decrementButton);
            Controls.Add(_valueTextBox);
            Controls.Add(_incrementButton);

            // Update layout initially
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            _valueTextBox.AutoSize = false; // Disable auto-sizing for manual layout.
            // First update your drawing rectangle.
            UpdateDrawingRect();
            Rectangle rect = DrawingRect;
            int padding = 1; // uniform padding on all sides

            // Get the preferred size of the text box.
            Size preferredSize = _valueTextBox.GetPreferredSize(Size.Empty);
            int preferredHeight = preferredSize.Height;

            // Calculate available width and height inside the DrawingRect.
            int availableWidth = rect.Width - 2 * padding;
            int availableHeight = rect.Height - 2 * padding;

            // Determine the height for child controls:
            // Use the smaller of the available height and the preferred height.
            // This will effectively "limit" the height to the preferred height.
            int elementHeight = Math.Min(availableHeight, preferredHeight);
            // Center the children vertically if there's extra space.
            int elementY = rect.Top + padding + ((availableHeight - elementHeight) / 2);

            // Define your desired minimum sizes.
            const int desiredButtonWidth = 15; // desired width for the buttons
            const int minTextBoxWidth = 10;      // minimum width for the textbox (adjust as needed)

            // Choose a button width:
            // For a square button, we base it on the available height.
            int buttonWidth = Math.Min(desiredButtonWidth, availableHeight);

            // Compute the remaining width for the textbox.
            // Layout: left padding + left button + padding + textbox + padding + right button + right padding.
            int textBoxWidth = availableWidth - (2 * buttonWidth) - (2 * padding);

            // If the available width is too narrow (i.e. textBoxWidth is less than its minimum),
            // adjust the button widths down so that the textbox gets at least minTextBoxWidth.
            if (textBoxWidth < minTextBoxWidth)
            {
                buttonWidth = Math.Max(0, (availableWidth - (2 * padding) - minTextBoxWidth) / 2);
                textBoxWidth = Math.Max(0, availableWidth - (2 * buttonWidth) - (2 * padding));
            }

            // Layout the Decrement Button (left side):
            int leftButtonX = rect.Left + padding;
            _decrementButton.Location = new Point(leftButtonX, elementY);
            _decrementButton.Size = new Size(buttonWidth, elementHeight);

            // Layout the TextBox (center):
            int textBoxX = leftButtonX + buttonWidth + padding;
            _valueTextBox.Location = new Point(textBoxX, elementY);
            _valueTextBox.Size = new Size(textBoxWidth, elementHeight);

            // Layout the Increment Button (right side):
            int rightButtonX = textBoxX + textBoxWidth + padding;
            _incrementButton.Location = new Point(rightButtonX, elementY);
            _incrementButton.Size = new Size(buttonWidth, elementHeight);
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
